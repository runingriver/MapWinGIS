/****************************************************************************
 * 文件名:lodMain.cs
 * 描  述:程序入口
 * **************************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualBasic;


namespace MapWinGIS.MainProgram
{
    //为避免与命名空间名字冲突，修改类名MainProgram为Program
    sealed class Program
    {
        //全局变量
        internal static MapWinForm frmMain;
        internal static XmlProjectFile projInfo; //存储当前MapWinGIS project信息
        internal static AppInfo appInfo ;  //存储当前MapWinGIS 配置信息
        internal static frmScript scripts; //当执行了LoadCulture()后初始化,内嵌脚本编辑器
        internal static Utility.CustomMessageBox messageBox; //自定义消息框

        /// <summary>
        /// 获得主程序所在目录 eg：D:\MapWinGIS\MapWinGIS\bin
        /// </summary>
        internal static string binFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static string g_error; //最后一条错误信息
        public static bool g_ShowDetaileErrors = true;
        public static ArrayList g_KillList = new ArrayList(); //要释放的资源列表
        /// <summary>
        /// 是否延迟同步插件菜单开关
        /// false-不延迟同步插件菜单中(需要刷新菜单)，true-延迟同步插件菜单(暂不刷新菜单)
        /// </summary>
        internal static bool g_SyncPluginMenuDefer = false;

        public static MapWinGIS.Controls.Projections.ProjectionDatabase ProjectionDB = new Controls.Projections.ProjectionDatabase();

        /// <summary>
        /// 程序入口
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //无法捕捉异常将送到CustomExceptionHandler类处理

            Application.ThreadException += new ThreadExceptionEventHandler(CustomExceptionHandler.OnThreadException);

            messageBox = new Utility.CustomMessageBox();

            // 检查ocx是否可用，没有就尝试注册
            if (!CheckOCXRegistration())
            {
                return;
            }

            projInfo = new XmlProjectFile();

            //启动一个线程，确保projnad文件存在并设置为环境变量setenv.exe
            try
            {
                Thread projnadCheck = new Thread(new System.Threading.ThreadStart(CheckPROJNAD));
                projnadCheck.SetApartmentState(ApartmentState.STA);
                projnadCheck.Start();
            }
            catch { }

            appInfo = new AppInfo();
            //在配置文件中读取使用语言种类
            LoadCulture();

            //设置了语言后，需要立即初始化脚本窗体，以便可以翻译成为相应的语言版本
            scripts = new frmScript();

            //如果双击的是配置文件(.mwcfg),则从配置文件启动
            bool broadcastCmdLine = false;
            RunConfigCommandLine(ref broadcastCmdLine);

            // 初始化程序界面
            frmLogo formLogo = new frmLogo();

            formLogo.Show();
            formLogo.lblVersion.Text = "Version: " + App.VersionString;
            Application.DoEvents();

            // 创建一个MainWinForm实例
            formLogo.lblStatus.Text = "初始化...";
            formLogo.lblStatus.Refresh();

            frmMain = new MapWinForm();
            MapWinGIS.Utility.Logger.ProgressStatus = new MWGProgressStatus();

            // 恢复到old symbology 默认不恢复
            // frmMain.MapMain.ShapeDrawingMethod = MapWinGIS.tkShapeDrawingMethod.dmStandard;

            // 加载投影数据库
            formLogo.lblStatus.Text = "读取投影数据库...";
            formLogo.lblStatus.Refresh();
            //ProjectionDB.ReadFromExecutablePath(Application.ExecutablePath); // MapWindow.Controls 中的

            // 创建菜单
            frmMain.CreateMenus();

            // 加载应用程序插件（application plugins)
            formLogo.lblStatus.Text = "加载应用程序插件...";
            formLogo.lblStatus.Refresh();

            frmMain.m_PluginManager.LoadApplicationPlugins(appInfo.ApplicationPluginDir);

            // 外部插件加载（plugin-in） 

            formLogo.lblStatus.Text = "加载外部插件...";
            formLogo.lblStatus.Refresh();
            formLogo.Refresh();

            frmMain.m_PluginManager.LoadPlugins();

            //确保在plugin目录中的所有插件都加载了
            frmMain.SynchPluginMenu();

            // 加载配置，窗体的位置和大小
            LoadConfig();

            // 关闭，释放frmLogo
            formLogo.Close();
            formLogo.Dispose();

            //显示主窗体（MainWinForm）
            frmMain.LoadToolStripSettings(frmMain.StripDocker);
            frmMain.mapPanel.Show(frmMain.dckPanel);

            frmMain.Show();
            frmMain.Update();
            frmMain.SetModified(false);
            frmMain.m_HasBeenSaved = true;
            frmMain.MapMain.Focus();
            Application.DoEvents();

            //决定是否显示欢迎界面
            //如果程序是双击配置文件(.mwcfg)形式启动，则不显示
            if (appInfo.ShowWelcomeScreen && !broadcastCmdLine)
            {
                ShowWelcomeScreen();
            }

            //如果双击项目文件并且存在项目文件，则将其提取到projinfo对象中
            bool broadcastCmdLine_2 = false;
            RunProjectCommandLine(Environment.CommandLine, ref broadcastCmdLine_2);

            if (broadcastCmdLine || broadcastCmdLine_2) //从.mwcfg或mwprj启动的话，就把该消息广播给插件
            {
                frmMain.m_PluginManager.BroadcastMessage("COMMAND_LINE:" + Environment.CommandLine);
            }

            //一切就绪，如果一个script等待执行，则现在就处理
            //if (Scripts.pFileName != "")
            //{
            //    Scripts.RunSavedScript();
            //}

            try
            {
                Application.Run(frmMain);
            }
            catch (ObjectDisposedException)
            {
                //当应用程序退出时引发的异常，所以忽略
            }
            catch (Exception)
            { }

            /**********************************************************************************************************************/
            //下面的代码将在程序终止后执行

            //移除自定义错误处理handler
            Application.ThreadException -= new ThreadExceptionEventHandler(CustomExceptionHandler.OnThreadException);

            foreach (string s in g_KillList)
            {
                try
                {
                    System.IO.File.Delete(s);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("删除临时文件失败: " + s + " " + e.Message);
                }
            }
            g_KillList.Clear();

        }

        /// <summary>
        /// 运行在命令行中指定的项目
        /// </summary>
        /// <param name="CommandLine">命令行字符</param>
        /// <param name="broadcastCmdLine">如果没有处理，则广播命令行信息给所有插件</param>
        public static void RunProjectCommandLine(string CommandLine, ref bool broadcastCmdLine)
        { }

        /// <summary>
        /// 显示欢迎界面，选择一个项目
        /// </summary>
        private static void ShowWelcomeScreen()
        { }

        /// <summary>
        /// 加载关于，程序的配置
        /// </summary>
        private static void LoadConfig()
        {
            string defaultConfigFileName = projInfo.DefaultConfigFile;
            g_SyncPluginMenuDefer = true;
            
            //如果存在项目目录，就加载            
            if (projInfo.ProjectFileName.Length > 0)
            {                
                projInfo.LoadProject(projInfo.ProjectFileName);
            }
            //自动设置ConfigFilename 的值
            if (projInfo.ConfigFileName.Length == 0)
            {               
                projInfo.ConfigFileName = projInfo.UserConfigFile;

                //从默认配置文件创建一个新的用户配置文件
                if (!File.Exists(projInfo.ConfigFileName))
                {
                    projInfo.CreateConfigFileFromDefault(projInfo.ConfigFileName);
                }
            }

            //检查执行目录下的default.mwgcfg是否被修改，若修改更新
            if (projInfo.CompareFilesByTime(defaultConfigFileName, projInfo.ConfigFileName) > 0)
            {
                projInfo.CreateConfigFileFromDefault(projInfo.ConfigFileName);
            }

            //判断配置是否加载，若没加载则加载配置
            if (projInfo.ConfigLoaded == false)
            {
               projInfo.LoadConfig(true);
            }

            g_SyncPluginMenuDefer = false;
            frmMain.SynchPluginMenu();
        }

        /// <summary>
        /// 注册MapWinGIS.ocx
        /// </summary>
        /// <returns>true,注册成功 false，注册失败</returns>
        private static bool CheckOCXRegistration()
        {
            bool Created = false;
            MapWinGIS.Point pnt;
            string MsgDetails = "";
            string RegisterFilename = binFolder + "\\MapWinGIS.ocx";
            Process registOCX;

            //尝试新建一个COM类的实例，MapWinGIS注册经常会出现问题
            try
            {
                pnt = new MapWinGIS.Point();
                Created = true;
            }
            catch (FileNotFoundException NoFileException) //可能没有MapWinGIS.ocx,也反注册，注册一遍
            {
                if (!File.Exists(RegisterFilename))
                {
                    RegisterFilename = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\MapWinGIS\\MapWinGIS.ocx"; 
                }
                if (File.Exists(RegisterFilename))
                {
                    try
                    {
                        registOCX = Process.Start("regsvr32.exe", "/u /s" + RegisterFilename);
                        registOCX.WaitForExit();
                        registOCX = Process.Start("regsvr32.exe", "/s " + RegisterFilename);
                        registOCX.WaitForExit();
                        pnt = new MapWinGIS.Point();
                        Created = true;

                        RegisterFilename = binFolder + "\\Plugins\\watershed_delin\\mwtaudem.dll";
                        if (System.IO.File.Exists(RegisterFilename))
                        {
                            try
                            {
                                registOCX = Process.Start("regsvr32.exe", "/u /s" + RegisterFilename);
                                registOCX.WaitForExit();
                                registOCX = Process.Start("regsvr32.exe", "/s " + RegisterFilename);
                                registOCX.WaitForExit();
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        MsgDetails = "FileNotFoundException中尝试注册失败：" + ex.ToString();
                    }

                }
                else
                {
                    if (messageBox.Display("找不到MapWInGIS.ocx，请将ocx文件复制到: " + RegisterFilename + "并重启程序", "MapWinGIS启动失败", Utility.MessageButton.YseNo) == DialogResult.Yes)
                    {
                        ShowError(NoFileException);
                    }
                }

            }
            catch (COMException CreateMainFormException) //可能MapWinGIS.ocx没有注册
            {
                //不存在，再在CommandProgramFiles中找.ocx
                if (!System.IO.File.Exists(RegisterFilename))
                {
                    RegisterFilename = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\MapWinGIS\\MapWinGIS.ocx";
                }
                //存在，注册
                if (System.IO.File.Exists(RegisterFilename))
                {
                    try
                    {
                        //  /u反注册，/s 安静模式silent
                        registOCX = Process.Start("regsvr32.exe", "/u /s" + RegisterFilename);
                        registOCX.WaitForExit();
                        registOCX = Process.Start("regsvr32.exe", "/s " + RegisterFilename);
                        registOCX.WaitForExit();

                        pnt = new MapWinGIS.Point();
                        Created = true;
                        //mwtaudem.dll  tkTaudem.dll的作用，地形分析使用数字高程模型，为了能够使用它的某些功能
                        RegisterFilename = binFolder + "\\Plugins\\watershed_delin\\mwtaudem.dll";
                        if (System.IO.File.Exists(RegisterFilename))
                        {
                            try
                            {
                                registOCX = Process.Start("regsvr32.exe", "/u /s" + RegisterFilename);
                                registOCX.WaitForExit();
                                registOCX = Process.Start("regsvr32.exe", "/s " + RegisterFilename);
                                registOCX.WaitForExit();

                                //RegisterFilename = binFolder + "\\Plugins\\watershed_delin\\tkTaudem.dll";
                                //Process.Start("regsvr32.exe", "/u /s " + RegisterFilename);
                                //Process.Start("regsvr32.exe", "/s " + RegisterFilename);
                            }
                            catch { }
                        }
                    }
                    catch
                    {
                        MsgDetails = "MapWinGIS.ocx 注册失败！";
                    }

                }
                else
                {
                    MsgDetails = "找不到MapWInGIS.ocx，请将ocx文件复制到: " + RegisterFilename + "并重启程序";
                }

                if (!Created)
                {
                    if (messageBox.Display(MsgDetails + "\r\n\r\n" + "若存在ocx并重启无效，请尝试手动注册!" + "\r\n\r\n" + "查看并提交错误？", "MapWinGIS启动失败！", Utility.MessageButton.YseNo) == DialogResult.Yes)
                    {
                         ShowError(CreateMainFormException);
                    }
                }

            }
            catch (Exception NonComException)
            {
                ShowError(NonComException);
            }

            return Created;
        }

        /// <summary>
        /// 显示一个错误对话框
        /// </summary>
        /// <param name="ex">错误的异常信息</param>
        /// <param name="email">可选参数，Email地址</param>
        public static void ShowError(Exception ex, string email = "")
        {
            MapWinGIS.Utility.Logger.Dbg(ex.ToString());
            CustomExceptionHandler.SendNextToEmail = email;
            CustomExceptionHandler.OnThreadException(ex);
        }

        /// <summary>
        /// 处理无法捕捉异常
        /// 嵌套类
        /// </summary>
        public class CustomExceptionHandler
        {
            public static string SendNextToEmail = "";

            // 异常处理事件
            public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                OnThreadException(t.Exception);
            }
            //重载，异常处理事件方法
            public static void OnThreadException(Exception e)
            {
                if (e.Message.Contains("UnauthorizedAccessException"))
                {
                    MapWinGIS.Utility.Logger.Msg("产生一个与权限有关的错误！请确保你对所有文件都有访问权，也可能文件被其他程序占用！", "无权访问异常", MessageBoxIcon.Exclamation);
                    SendNextToEmail = "";
                    return;
                }

                try
                {
                    if (!projInfo.NoPromptToSendErrors)
                    {
                        ErrorDialog errorBox = new ErrorDialog(e, SendNextToEmail);
                        errorBox.ShowDialog();
                    }
                    else
                    {
                        ErrorDialogNoSend errorBox = new ErrorDialogNoSend(e);
                        errorBox.ShowDialog();
                    }
                }
                catch (Exception)
                {
                    ErrorDialog errorBox = new ErrorDialog(e, SendNextToEmail);
                    errorBox.ShowDialog();
                }
                finally
                {
                    SendNextToEmail = "";
                }

            }
        }

        /// <summary>
        /// 确保proj_nad环境变量存在
        /// 当proj_nad环境变量不存在时，会引发错误
        /// </summary>
        private static void CheckPROJNAD()
        {           
            if (System.IO.File.Exists(binFolder + "\\setenv.exe") && System.IO.Directory.Exists(binFolder + "\\PROJ_NAD"))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = binFolder + "\\setenv.exe";
                    psi.Arguments = "-a PROJ_LIB " + binFolder + "\\PROJ_NAD";
                    psi.CreateNoWindow = true;
                    //此处，发布时设为Hidden
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process.Start(psi);
                }
                catch (Exception e)
                {
                    MapWinGIS.Utility.Logger.Dbg("DEBUG: " + e.ToString());
                }
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = binFolder + "\\setenv.exe";
                    psi.Arguments = "-a GDAL_DATA " + binFolder + "\\GDAL_DATA";
                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process.Start(psi);
                }
                catch (Exception e)
                {
                    MapWinGIS.Utility.Logger.Dbg("DEBUG: " + e.ToString());
                }

                // setenv.exe有时会不起作用，可返回到系统环境中设置
                try
                {
                    if (Environment.GetEnvironmentVariable("PROJ_LIB", EnvironmentVariableTarget.Machine) == null)
                    {
                        Environment.SetEnvironmentVariable("PROJ_LIB", binFolder + "\\PROJ_NAD", EnvironmentVariableTarget.Machine);
                    }

                    if (Environment.GetEnvironmentVariable("PROJ_LIB", EnvironmentVariableTarget.User) == null)
                    {
                        Environment.SetEnvironmentVariable("PROJ_LIB", binFolder + "\\PROJ_NAD", EnvironmentVariableTarget.User);
                    }

                    Environment.SetEnvironmentVariable("PROJ_LIB", binFolder + "\\PROJ_NAD");

                    if (Environment.GetEnvironmentVariable("GDAL_DATA", EnvironmentVariableTarget.Machine) == null)
                    {
                        Environment.SetEnvironmentVariable("GDAL_DATA", binFolder + "\\GDAL_DATA", EnvironmentVariableTarget.Machine);
                    }

                    if (Environment.GetEnvironmentVariable("GDAL_DATA", EnvironmentVariableTarget.User) == null)
                    {
                        Environment.SetEnvironmentVariable("GDAL_DATA", binFolder + "\\GDAL_DATA", EnvironmentVariableTarget.User);
                    }

                    Environment.SetEnvironmentVariable("GDAL_DATA", binFolder + "\\GDAL_DATA");
                }
                catch (Exception e)
                {
                    MapWinGIS.Utility.Logger.Dbg("DEBUG: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 从命令行中读取配置文件的名字
        /// </summary>
        /// <param name="broadcastCmdLine"></param>
        private static void RunConfigCommandLine(ref bool broadcastCmdLine)
        {
        }

        /// <summary>
        /// 从注册表中加载窗体位置大小等信息
        /// </summary>
        public static void LoadFormPosition(System.Windows.Forms.Form Fo)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\\\MapWinGISConfig", false);
            try
            {
                if ((rk.GetValue(Fo.Name + "_x").ToString() != "") && (rk.GetValue(Fo.Name + "_y").ToString() != "") && (rk.GetValue(Fo.Name + "_w").ToString() != "") && (rk.GetValue(Fo.Name + "_h").ToString() != ""))
                {
                    Fo.Location = new System.Drawing.Point(int.Parse(rk.GetValue(Fo.Name + "_x").ToString(), CultureInfo.InvariantCulture), int.Parse(rk.GetValue(Fo.Name + "_y").ToString(), CultureInfo.InvariantCulture));
                    Fo.Size = new System.Drawing.Size(int.Parse(rk.GetValue(Fo.Name + "_w").ToString(), CultureInfo.InvariantCulture), int.Parse(rk.GetValue(Fo.Name + "_h").ToString(), CultureInfo.InvariantCulture));
                }
            }
            catch
            {
            }
            finally
            {
                rk.Close();
            }
        }
        /// <summary>
        /// 将窗体位置大小信息保存在注册表中
        /// </summary>
        public static void SaveFormPosition(System.Windows.Forms.Form Fo)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\\\MapWinGISConfig");
            if (Fo.Visible && Fo.WindowState != System.Windows.Forms.FormWindowState.Minimized && Fo.Location.X > -1 && Fo.Location.Y > -1 && Fo.Size.Width > 1 && Fo.Size.Height > 1)
            {
                rk.SetValue(Fo.Name + "_x", Fo.Location.X);
                rk.SetValue(Fo.Name + "_y", Fo.Location.Y);
                rk.SetValue(Fo.Name + "_w", Fo.Size.Width);
                rk.SetValue(Fo.Name + "_h", Fo.Size.Height);
            }
            rk.Close();
        }

        /// <summary>
        /// 加载语言
        /// </summary>
        public static void LoadCulture()
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\\\MapWinGISConfig", false);
            if (reg == null) return;
            try
            {
                string isOverrid = reg.GetValue("OverrideSystemLocale").ToString();
                string local = Convert.ToString(reg.GetValue("Local"));

                if (local == Thread.CurrentThread.CurrentUICulture.Name)
                {
                    return;
                }

                if (isOverrid == "" || Convert.ToBoolean(isOverrid) == false)
                {
                    appInfo.OverrideSystemLocale = false;
                    appInfo.Locale = Thread.CurrentThread.CurrentUICulture.Name;
                }
                else if (Convert.ToBoolean(isOverrid) == true && !string.IsNullOrEmpty(local))
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(local);
                    appInfo.Locale = local;
                    appInfo.OverrideSystemLocale = true;
                }
                else //local 为空
                {
                    appInfo.OverrideSystemLocale = false;
                    appInfo.Locale = Thread.CurrentThread.CurrentUICulture.Name;
                }
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("加载语言出错: " + ex.ToString());
            }
            finally
            {
                reg.Close();
            }
        }

        /// <summary>
        /// 提供一个创建临时文件的路径
        /// </summary>
        public static string GetMWGTempFile()
        {
            string ret = Path.GetTempFileName();
            try
            {
                File.Delete(ret);
            }
            catch
            { }
            g_KillList.Add(ret);
            return ret;
        }


    }
}
