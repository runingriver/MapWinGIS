/******************************************************************************
 * 文件名:clsXmlProjectConfig.cs (F)
 * 描  述:实现加载、保存MapWINGIS的配置,后续可以根据需要增减配置文件中的节点或属性。
 *    Tip：1.本次修改删去了在配置文件中存储内部插件信息，因为内部插件默认全部加载。
 *         2.FavoriteProjections和SaveCulture暂不实现
 * ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.Windows.Forms;


namespace MapWinGIS.MainProgram
{
    partial class XmlProjectFile
    {
        #region 加载（Loading）
        /// <summary>
        /// 加载配置
        /// </summary>
        public bool LoadConfig(bool load_Plugins)
        {
            string odir = Directory.GetCurrentDirectory();
            try
            { 
                XmlDocument doc = new XmlDocument();
                XmlElement root;

                if (Program.frmMain != null)//先卸载所有外部插件，后续根据配置信息添加
                {
                    Program.frmMain.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    System.Windows.Forms.Application.DoEvents();
                    Program.frmMain.m_PluginManager.UnloadAll();
                }
                //检查是否存在配置文件目录，不存在则用默认的目录并设置配置文件目录
                string configDir = Path.GetDirectoryName(ConfigFileName);
                if (!Directory.Exists(configDir))
                {
                    configDir = GetApplicationDataDir();
                    ConfigFileName = this.UserConfigFile;
                }
                Directory.SetCurrentDirectory(configDir);  //设置当前工作目录

                if (!File.Exists(ConfigFileName))
                {
                    //MapWinGIS.Utility.Logger.Dbg("从默认配置文件创建一个配置文件");
                    CreateConfigFileFromDefault(ConfigFileName);
                }

                MapWinGIS.Utility.Logger.Dbg("加载配置：" + ConfigFileName);
                doc.Load(ConfigFileName);
                root = doc.DocumentElement;

                //加载View
                LoadView(root["View"]);
                
                Application.DoEvents();

                LoadAppInfo(root["AppInfo"]);

                LoadRecentProjects(root["RecentProjects"]);

                if (root["ColorPalettes"] != null)
                {
                    LoadColorPalettes(root["ColorPalettes"]);
                }
                else
                {
                    if (p_Doc != null && p_Doc.DocumentElement != null)
                    {
                        g_ColorPalettes = p_Doc.CreateElement("ColorPalettes");
                    }
                    else
                    {
                        throw new Exception("画板节点不存在，且无法创建！");
                    }
                }

                if (load_Plugins)
                {
                    LoadPlugins(root["Plugins"], true);
                }

                //加载喜爱的项目，没有做事，先注释掉
                //XmlElement nodeProjection = root["FavoriteProjections"];
                //if (nodeProjection != null)
                //{
                //    List<int> list = Program.appInfo.FavoriteProjections;
                //    list.Clear();
                //    string text = nodeProjection.InnerText;
                //    string[] separator =new string[] { ":" };
                //    int code;
                //    foreach (string s in text.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                //    {
                //        if (int.TryParse(s, out code))
                //        {
                //            list.Add(code);
                //        }
                //    }
                //}

                Program.frmMain.Update();
                ConfigLoaded = true;
                Program.frmMain.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            { 
                m_ErrorOccured = true;
                m_ErrorMsg = ex.ToString();
            }
            finally
            {
                System.IO.Directory.SetCurrentDirectory(odir);
            }
            if (m_ErrorOccured)
            {
                MapWinGIS.Utility.Logger.Msg(m_ErrorMsg, "配置文件错误报告", MessageBoxIcon.Exclamation);
                m_ErrorOccured = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 默认配置文件目录将默认配置文件复制到指定的目录作为用户配置文件
        /// 若不存在默认文件，则保存当前程序配置为默认文件，再复制并更正路径程序的路径信息
        /// </summary>
        /// <param name="newConfigFile">新配置文件保存的路径(包含文件名的完整路径)</param>
        public void CreateConfigFileFromDefault(string newConfigFile)
        {
            string defaultConfigFileName = this.DefaultConfigFile;//默认配置文件
            
            //确保当前执行目录为可执行文件目录，可以让其他相关的路径使用生效
            string odir = Directory.GetCurrentDirectory();
            string mapWinGISDir = Program.binFolder;
            if (odir != mapWinGISDir)
            {
                Directory.SetCurrentDirectory(mapWinGISDir);
            }
            //不存在默认的配置文件,将当前运行的程序设置保存为一个默认文件
            if (!File.Exists(defaultConfigFileName))
            {
                string originalCfgFile = ConfigFileName;
                ConfigFileName = defaultConfigFileName; //为了新建一个默认配置文件到执行目录
                SaveConfig(false, false);
                ConfigFileName = originalCfgFile;
            }

            try
            { 
                File.Copy(defaultConfigFileName, newConfigFile, true);

                XmlDocument doc = new XmlDocument();
                doc.Load(defaultConfigFileName);
                XmlElement root = doc.DocumentElement;

                //更正帮助目录和默认目录
                XmlElement appInfoElement = root["AppInfo"];
                string helpFilePath = "";
                string newHelpFilePath = "";
                if (appInfoElement.HasAttribute("HelpFilePath"))
                {
                    try
                    {
                        helpFilePath = Path.GetFullPath(appInfoElement.Attributes["HelpFilePath"].Value);
                    }
                    catch
                    {
                        helpFilePath = mapWinGISDir;
                    }
                    newHelpFilePath = GetRelativePath(helpFilePath, newConfigFile);
                }
                string defaultDir = "";
                string newDefaultDir = "";
                if (appInfoElement.HasAttribute("DefaultDir"))
                {
                    try
                    {
                        defaultDir = Path.GetFullPath(appInfoElement.Attributes["DefaultDir"].Value);
                    }
                    catch
                    {
                        defaultDir = Path.GetFullPath("SampleProjects");//默认将目录设为执行目录的SampleProjects目录
                    }
                    newDefaultDir = GetRelativePath(defaultDir, mapWinGISDir);
                }
                //更正内部插件的路径
                XmlElement applicationPluginElement = root["ApplicationPlugins"];
                string appPluginDir = "";
                string newAppPluginDir = "";
                if (applicationPluginElement.HasAttribute("PluginDir"))
                {
                    try
                    {
                        appPluginDir = Path.GetFullPath(applicationPluginElement.Attributes["PluginDir"].Value);
                    }
                    catch
                    {
                        appPluginDir = Path.Combine(Program.binFolder, "ApplicationPlugins");
                    }
                    newAppPluginDir = GetRelativePath(appPluginDir, newConfigFile);
                }
                
                //将更正的值填入XML文档(MapWinGIS.wmgcfg)
                doc.Load(newConfigFile);
                root = doc.DocumentElement;
                appInfoElement = root["AppInfo"];
                appInfoElement.Attributes["HelpFilePath"].Value = newHelpFilePath;
                appInfoElement.Attributes["DefaultDir"].Value = newDefaultDir;

                applicationPluginElement = root["ApplicationPlugins"];
                applicationPluginElement.Attributes["PluginDir"].Value = newAppPluginDir;
                doc.Save(newConfigFile);
                MapWinGIS.Utility.Logger.Dbg("复制配置文件，从" + defaultConfigFileName + "到" + newConfigFile);
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("从默认配置文件创建用户配置文件失败。默认配置文件：" + defaultConfigFileName + "用户配置文件：" + newConfigFile + ex.ToString());
                newConfigFile = defaultConfigFileName;
            }
            finally
            {
                if (mapWinGISDir != odir)
                {
                    Directory.SetCurrentDirectory(odir);
                }
            }
        }

        /// <summary>
        /// 从给定节点开始加载程序视觉样式的配置
        /// 包括WindowState、ViewBackColor、窗体位置大小、LoadTIFFandIMGasgrid、鼠标状态、Legend、PreviewMap...等
        /// </summary>
        private bool LoadView(XmlElement view)
        { 
            try
            { 

                //设置窗体的的状态
                Program.frmMain.WindowState = (FormWindowState)(Convert.ToInt32(view.Attributes["WindowState"].InnerText));
                 //设置背景色
                if (view.Attributes["ViewBackColor"].InnerText.Length != 0)
                {
                    g_ViewBackColor = Convert.ToInt32(view.Attributes["ViewBackColor"].InnerText);
                    System.Drawing.Color backColor;
                    backColor = ColorScheme.IntToColor(g_ViewBackColor);
                    Program.appInfo.DefaultBackColor = backColor;
                    Program.frmMain.m_View.BackColor = backColor;
                }
                //设置窗体的大小和位置
                if (Program.frmMain.WindowState == FormWindowState.Normal)
                {
                    int w = Convert.ToInt32(view.Attributes["WindowWidth"].InnerText);
                    int h = Convert.ToInt32(view.Attributes["WindowHeight"].InnerText);
                    System.Drawing.Point drawPoint = new System.Drawing.Point(Convert.ToInt32(view.Attributes["LocationX"].InnerText), Convert.ToInt32(view.Attributes["LocationY"].InnerText));
                    FindSafeWindowLocation(ref w, ref h, ref drawPoint);
                    Program.frmMain.Width = w;
                    Program.frmMain.Height = h;
                    Program.frmMain.Location = drawPoint;
                }

                try
                {
                    string loadTIFFandIMGasgrid = view.Attributes["LoadTIFFandIMGasgrid"].InnerText;
                    if (loadTIFFandIMGasgrid == GeoTIFFAndImgBehavior.Automatic.ToString())
                    {
                        Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.Automatic;
                    }
                    else if (loadTIFFandIMGasgrid == GeoTIFFAndImgBehavior.LoadAsGrid.ToString())
                    {
                        Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.LoadAsGrid;
                    }
                    else if (loadTIFFandIMGasgrid == GeoTIFFAndImgBehavior.LoadAsImage.ToString())
                    {
                        Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.LoadAsImage;
                    }
                    else
                    {
                        Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.Automatic;
                    }
                }
                catch
                {
                    Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.Automatic;
                }

                try
                {
                    string mouseWheelBehavior = view.Attributes["MouseWheelBehavior"].InnerText;
                    if (mouseWheelBehavior == MouseWheelZoomDir.NoAction.ToString())
                    {
                        Program.appInfo.MouseWheelZoom = MouseWheelZoomDir.NoAction;
                    }
                    else if (mouseWheelBehavior == MouseWheelZoomDir.WheelUpZoomsIn.ToString())
                    {
                        Program.appInfo.MouseWheelZoom = MouseWheelZoomDir.WheelUpZoomsIn;
                    }
                    else if (mouseWheelBehavior == MouseWheelZoomDir.WheelUpZoomsOut.ToString())
                    {
                        Program.appInfo.MouseWheelZoom = MouseWheelZoomDir.WheelUpZoomsOut;
                    }
                    else
                    {
                        Program.appInfo.MouseWheelZoom = MouseWheelZoomDir.NoAction;
                    }
                }
                catch
                {
                    Program.appInfo.MouseWheelZoom = MouseWheelZoomDir.NoAction;
                }

                try
                {
                    string loadESRIAsGrid = view.Attributes["LoadESRIAsGrid"].InnerText;
                    if (loadESRIAsGrid == ESRIBehavior.LoadAsImage.ToString())
                    {
                        Program.appInfo.LoadESRIAsGrid = ESRIBehavior.LoadAsImage;
                    }
                    else if (loadESRIAsGrid == ESRIBehavior.LoadAsGrid.ToString())
                    {
                        Program.appInfo.LoadESRIAsGrid = ESRIBehavior.LoadAsGrid;
                    }
                    else
                    {
                        Program.appInfo.LoadESRIAsGrid = ESRIBehavior.LoadAsGrid; 
                    }
                }
                catch
                {
                    Program.appInfo.LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.LoadAsGrid;
                }

                try
                {
                    Program.appInfo.LabelsUseProjectLevel = bool.Parse(view.Attributes["LabelsUseProjectLevel"].InnerText);
                }
                catch
                {
                    Program.appInfo.LabelsUseProjectLevel = false;
                }

                if (Program.frmMain.WindowState == FormWindowState.Maximized)
                {
                    Program.frmMain.WindowState = FormWindowState.Normal;
                }

                try
                {
                    bool.TryParse(view.Attributes["TransparentSelection"].InnerText, out TransparentSelection);
                }
                catch
                {
                    TransparentSelection = true;
                }

                //legend的可见性
                bool visible = true;
                try
                {
                    bool.TryParse(view.Attributes["LegendVisible"].InnerText, out visible);
                }
                catch
                {
                    visible = true;
                }
                Program.frmMain.UpdateLegendPanel(visible);
                //PreviewMap的可见性
                try
                {
                    bool.TryParse(view.Attributes["PreviewVisible"].InnerText, out visible);
                }
                catch
                {
                    visible = true;
                }
                Program.frmMain.UpdatePreviewPanel(visible);

                ICollection col = Program.frmMain.m_UIPanel.m_Panels.Keys;
                foreach (string item in col)
                {
                    Program.frmMain.m_UIPanel.SetPanelVisible(item, true);
                }
            }
            catch (Exception ex)
            {
                m_ErrorMsg += "在LoadView方法中出现错误：" + ex.ToString();
                m_ErrorOccured = true;
            }
            if (m_ErrorOccured)
            {
                MapWinGIS.Utility.Logger.Msg(m_ErrorMsg, "LoadView方法出错", MessageBoxIcon.Exclamation);
                m_ErrorOccured = false;
                return false;
            }
            return true; 
        }

        /// <summary>
        /// 从给定节点取值去设置clsAppInfo中的成员 25
        /// </summary>
        private void LoadAppInfo(XmlElement appInfoXml)
        {
            try
            { 
                Program.appInfo.ApplicationName = appInfoXml.Attributes["Name"].InnerText;
                Program.appInfo.Version = appInfoXml.Attributes["Version"].InnerText;
                Program.appInfo.BuildDate = appInfoXml.Attributes["BuildDate"].InnerText;
                Program.appInfo.Developer = appInfoXml.Attributes["Developer"].InnerText;
                Program.appInfo.Comments = appInfoXml.Attributes["Comments"].InnerText;
                Program.appInfo.SplashTime = Convert.ToDouble(appInfoXml.Attributes["SplashTime"].InnerText);

                try
                {
                    string logFile = appInfoXml.Attributes["LogfilePath"].InnerText;
                    if (MapWinGIS.Utility.FileOperator.FileOrDirExists(logFile))
                    {
                        Program.appInfo.LogfilePath = logFile;
                    }
                    MapWinGIS.Utility.Logger.StartToFile(Program.appInfo.LogfilePath, false, true, false);
                }
                catch(Exception ex)
                {
                    MapWinGIS.Utility.Logger.Dbg("LoadAppInfo中开始记录日志异常：" + ex.ToString());
                    //Program.appInfo.LogfilePath = "";
                }

                try
                {
                    NoPromptToSendErrors = Convert.ToBoolean(appInfoXml.Attributes["NoPromptToSendErrors"].InnerText);
                }
                catch
                {
                    NoPromptToSendErrors = false;
                }

                try
                {
                    Program.appInfo.NeverShowProjectionDialog = Convert.ToBoolean(appInfoXml.Attributes["NeverShowProjectionDialog"].InnerText);
                }
                catch
                {
                    Program.appInfo.NeverShowProjectionDialog = false;
                }

                if (appInfoXml.Attributes["WelcomePlugin"].InnerText == null)
                {
                    Program.appInfo.WelcomePlugin = null;
                }
                else
                {
                    Program.appInfo.WelcomePlugin = appInfoXml.Attributes["WelcomePlugin"].InnerText;
                }

                try
                {
                    string strPath = Path.GetFullPath(appInfoXml.Attributes["DefaultDir"].InnerText);
                    if (strPath != null)
                    {
                        Program.appInfo.DefaultDir = strPath;
                    }
                }
                catch
                { }

                if (appInfoXml.HasAttribute("URL") && appInfoXml.Attributes["URL"].InnerText != null)
                {
                    Program.appInfo.URL = appInfoXml.Attributes["URL"].InnerText;
                }
                if (appInfoXml.HasAttribute("ShowWelcomeScreen") && appInfoXml.Attributes["ShowWelcomeScreen"].InnerText != null)
                {
                    Program.appInfo.ShowWelcomeScreen = bool.Parse(appInfoXml.Attributes["ShowWelcomeScreen"].InnerText);
                }
                if (appInfoXml.HasAttribute("DisplayAutoCreatespatialindex"))
                {
                    Program.appInfo.ShowAutoCreateSpatialindex = bool.Parse(appInfoXml.Attributes["DisplayAutoCreatespatialindex"].InnerText);
                }
                if (appInfoXml.HasAttribute("DisplayFloatingScalebar"))
                {
                    Program.appInfo.ShowFloatingScalebar = bool.Parse(appInfoXml.Attributes["DisplayFloatingScalebar"].InnerText);
                }
                if (appInfoXml.HasAttribute("DisplayAvoidCollision"))
                {
                    Program.appInfo.ShowAvoidCollision = bool.Parse(appInfoXml.Attributes["DisplayAvoidCollision"].InnerText);
                }
                if (appInfoXml.HasAttribute("DisplayRedrawSpeed"))
                {
                    Program.appInfo.ShowRedrawSpeed = bool.Parse(appInfoXml.Attributes["DisplayRedrawSpeed"].InnerText);
                }
                if (appInfoXml.HasAttribute("HelpFilePath") && appInfoXml.Attributes["HelpFilePath"].InnerText != null)
                {
                    Program.appInfo.HelpFilePath = Path.GetFullPath(appInfoXml.Attributes["HelpFilePath"].InnerText);
                }
                else
                {
                    Program.appInfo.HelpFilePath = "";
                }
                if (Program.appInfo.SplashTime < 0) { Program.appInfo.SplashTime = 0; }
                if (appInfoXml.HasAttribute("ShowDynVisWarnings"))
                {
                    Program.appInfo.ShowDynamicVisibilityWarnings = bool.Parse(appInfoXml.Attributes["ShowDynVisWarnings"].InnerText);
                }
                if (appInfoXml.HasAttribute("ShowLayerAfterDynVis"))
                {
                    Program.appInfo.ShowLayerAfterDynamicVisibility = bool.Parse(appInfoXml.Attributes["ShowLayerAfterDynVis"].InnerText);
                }
                if (appInfoXml.HasAttribute("SymbologyLoadingBehavior"))
                {
                    Program.appInfo.SymbologyLoadingBehavior = (Interfaces.SymbologyBehavior)Enum.Parse(typeof(Interfaces.SymbologyBehavior), (appInfoXml.Attributes["SymbologyLoadingBehavior"].InnerText));		
                }
                if (appInfoXml.HasAttribute("ProjectionMismatchBehavior"))
                {
                    Program.appInfo.ProjectionMismatchBehavior = (Interfaces.ProjectionMismatchBehavior)Enum.Parse(typeof(Interfaces.ProjectionMismatchBehavior), (appInfoXml.Attributes["ProjectionMismatchBehavior"].InnerText));		 
                }
                if (appInfoXml.HasAttribute("ProjectionAbsenceBehavior"))
                {
                    Program.appInfo.ProjectionAbsenceBehavior = (Interfaces.ProjectionAbsenceBehavior)Enum.Parse(typeof(Interfaces.ProjectionAbsenceBehavior), (appInfoXml.Attributes["ProjectionAbsenceBehavior"].InnerText));
                }
                if (appInfoXml.HasAttribute("ShowLoadingReport"))
                {
                    Program.appInfo.ShowLoadingReport = bool.Parse(appInfoXml.Attributes["ShowLoadingReport"].InnerText);
                }
                if (appInfoXml.HasAttribute("ProjectReloading"))
                {
                    Program.appInfo.ProjectReloading = bool.Parse(appInfoXml.Attributes["ProjectReloading"].InnerText); 
                }

                //闪屏图片和程序图标均不再存储在Xml中
            }
            catch(Exception)
            {
                m_ErrorMsg += "在LoadAppInfo方法中出现错误";
                m_ErrorOccured = true;
            }
            if (m_ErrorOccured)
            {
                MapWinGIS.Utility.Logger.Msg(m_ErrorMsg, "LoadAppInfo方法出错",MessageBoxIcon.Exclamation);
                m_ErrorOccured = false;
            }
        }

        private void LoadRecentProjects(XmlElement recentFiles)
        {
            try
            {               
                int iChild, iRecentProject;
                string path, pathLower;
                XmlNode file;
                int numChildNodes = recentFiles.ChildNodes.Count;

                if (numChildNodes == 0) { return; }

                //清空ArrayList中之前所有的文件
                RecentProjects.Clear();

                for (iChild = 0; iChild < numChildNodes; iChild++)
                {
                    file = recentFiles.ChildNodes[iChild];
                    path = Path.GetFullPath(file.InnerText); //获取绝对路径
                    
                    //消除重复的条目
                    pathLower = path.ToLower();                   
                    iRecentProject = 0;
                    while (iRecentProject < RecentProjects.Count && RecentProjects[iRecentProject].ToString().ToLower() != pathLower)
                    {
                        iRecentProject++;
                    }
                    //添加没有被删除的近期项目完整路径
                    if (iRecentProject == RecentProjects.Count && File.Exists(path))
                    {
                        RecentProjects.Add(path);
                    }
                }

                Program.frmMain.BuildRecentProjectsMenu();

            }
            catch(Exception ex)
            {
                m_ErrorMsg += "在LoadRecentProjects方法中出现错误:" + ex.ToString();
                m_ErrorOccured = true;
                return;
            }
        }

        private void LoadColorPalettes(XmlElement colorPalettes)
        {
            try
            {
                g_ColorPalettes = colorPalettes;
            }
            catch (Exception ex)
            {
                m_ErrorMsg = "在LoadColorPalettes方法中出现错误：" + ex.ToString();
                m_ErrorOccured = true;
                return;
            }
        }

        #endregion

        #region 保存（Saving）
        /// <summary>
        /// 将配置保存到配置文件（configuration file）
        /// </summary>
        /// <returns>保存配置成功与否</returns>
        public bool SaveConfig(bool isSaveDocker = true, bool isSaveCulture = false)
        {
            if (isSaveDocker)
            {
                //保存Dock的配置,到独立的文件
                string dockConfigFile = System.IO.Path.Combine(XmlProjectFile.GetApplicationDataDir(), "MapWinGISDock.config");
                try
                {
                    MapWinGIS.Utility.Logger.Dbg("保存Dock Panel配置：" + dockConfigFile);
                    Program.frmMain.dckPanel.SaveAsXml(dockConfigFile);
                }
                catch (IOException e)
                {
                    MapWinGIS.Utility.Logger.Dbg("保存Dock Panel配置：异常：" + e.ToString());
                }
            }

            if (isSaveCulture)
            {
                //保存语言
                SaveCulture();
            }

            //保存系统设置
            XmlElement root;
            try
            {
                //从指定的字符串加载Xml文档
                p_Doc.LoadXml("<Mapwin type='configurationfile' version='" + App.VersionString + "'></Mapwin>");
                root = p_Doc.DocumentElement;

                //保存AppInfo的设置到用户配置文件
                AddAppInfo(p_Doc, root);

                //保存最近项目到用户配置文件
                AddRecentProjects(p_Doc, root);

                //保存程序视图样式到用户配置文件
                AddViewElement(p_Doc, root);

                //保存已经加载的插件信息到用户配置文件
                AddPluginsElement(p_Doc, root, true);

                //保存内部插件信息到用户配置文件
                //注释掉，因为在加载时没有从xml加载内部插件
                //AddApplicationPluginsElement(p_Doc, root, true);

                //保存调色板信息到用户配置文件
                AddColorPalettes(p_Doc, root);

                // 保存喜爱的项目到用户配置文件
                // 该主题没有工作，先注释掉
                //List<int> list = Program.appInfo.FavoriteProjections;
                //if (list.Count > 0)
                //{
                //    string s = "";
                //    foreach (int proj in list)
                //    {
                //        s += proj.ToString() + ";";
                //    }
                //    //移除最后一个分号（semicolon）
                //    if (s.Length > 0)
                //    {
                //        s = s.Substring(0, s.Length - 1);
                //    }
                //    XmlElement el = p_Doc.CreateElement("FavoriteProjections");
                //    el.InnerText = s;
                //    root.AppendChild(el);
                //}

                MapWinGIS.Utility.Logger.Dbg("保存配置: " + ConfigFileName);
                p_Doc.Save(ConfigFileName);

                return true;
            }
            catch (System.Exception ex)
            {
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 保存自定义语言设置，若OverrideSystemLocale=true，则不采用系统语言采用Local设置的语言
        /// </summary>
        private void SaveCulture()
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\\\MapWinGISConfig");
            try
            {
                reg.SetValue("OverrideSystemLocale", Program.appInfo.OverrideSystemLocale.ToString());
                reg.SetValue("Local", Program.appInfo.Locale);
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("保存区域性语言，出现异常: " + ex.ToString());
            }
            finally
            {
                reg.Close();
            }
        }

        /// <summary>
        /// 保存应用程序配置
        /// </summary>
        /// <param name="m_Doc">XmlDocument对象</param>
        /// <param name="root">根节点</param>
        private void AddAppInfo(XmlDocument m_Doc, XmlElement root)
        {
            //添加节点
            XmlElement appInfoXML = m_Doc.CreateElement("AppInfo");
            //添加节点属性
            XmlAttribute Name = m_Doc.CreateAttribute("Name");
            XmlAttribute Version = m_Doc.CreateAttribute("Version");
            XmlAttribute BuildDate = m_Doc.CreateAttribute("BuildDate");
            XmlAttribute Developer = m_Doc.CreateAttribute("Developer");
            XmlAttribute Comments = m_Doc.CreateAttribute("Comments");
            XmlAttribute HelpFilePath = m_Doc.CreateAttribute("HelpFilePath");
            XmlAttribute UseSplashScreen = m_Doc.CreateAttribute("UseSplashScreen");
            XmlAttribute SplashPicture = m_Doc.CreateAttribute("SplashPicture");
            XmlAttribute SplashTime = m_Doc.CreateAttribute("SplashTime");
            XmlAttribute DefaultDir = m_Doc.CreateAttribute("DefaultDir");
            XmlAttribute URL = m_Doc.CreateAttribute("URL");
            XmlAttribute ShowWelcomeScreen = m_Doc.CreateAttribute("ShowWelcomeScreen");
            XmlAttribute WelcomePlugin = m_Doc.CreateAttribute("WelcomePlugin");
            XmlAttribute NeverShowProjectionDialog = m_Doc.CreateAttribute("NeverShowProjectionDialog");
            XmlAttribute NoPromptToSendErrorsXml = m_Doc.CreateAttribute("NoPromptToSendErrors");
            XmlAttribute LogfilePathXml = m_Doc.CreateAttribute("LogfilePath");
            XmlAttribute ShowDynVisWarningsXml = m_Doc.CreateAttribute("ShowDynVisWarnings");
            XmlAttribute ShowLayerAfterDynVisXml = m_Doc.CreateAttribute("ShowLayerAfterDynVis");
            XmlAttribute SymbologyLoadingBehavior = m_Doc.CreateAttribute("SymbologyLoadingBehavior");
            XmlAttribute ProjectionMismatch = m_Doc.CreateAttribute("ProjectionMismatchBehavior");
            XmlAttribute ProjectionAbsence = m_Doc.CreateAttribute("ProjectionAbsenceBehavior");
            XmlAttribute ShowLoadingReport = m_Doc.CreateAttribute("ShowLoadingReport");
            XmlAttribute ProjectReloading = m_Doc.CreateAttribute("ProjectReloading");
            XmlAttribute DisplayFloatingScalebar = m_Doc.CreateAttribute("DisplayFloatingScalebar");
            XmlAttribute DisplayRedrawSpeed = m_Doc.CreateAttribute("DisplayRedrawSpeed");
            XmlAttribute DisplayMapWinGISVersion = m_Doc.CreateAttribute("DisplayMapWinGISVersion");
            XmlAttribute DisplayAvoidCollision = m_Doc.CreateAttribute("DisplayAvoidCollision");
            XmlAttribute DisplayAutoCreatespatialindex = m_Doc.CreateAttribute("DisplayAutoCreatespatialindex");

            //设置属性
            ShowLayerAfterDynVisXml.InnerText = Convert.ToString(Program.appInfo.ShowLayerAfterDynamicVisibility);
            ShowDynVisWarningsXml.InnerText = Convert.ToString(Program.appInfo.ShowDynamicVisibilityWarnings);
            LogfilePathXml.InnerText = Program.appInfo.LogfilePath;
            Name.InnerText = Program.appInfo.ApplicationName;        
            Version.InnerText = App.VersionString;
            BuildDate.InnerText = Program.appInfo.BuildDate; //设为null 
            Developer.InnerText = Program.appInfo.Developer;
            Comments.InnerText = Program.appInfo.Comments;
            HelpFilePath.InnerText = GetRelativePath(Program.appInfo.HelpFilePath, ConfigFileName);
            SplashTime.InnerText = Convert.ToString(Program.appInfo.SplashTime);
            DefaultDir.InnerText = GetRelativePath(Program.appInfo.DefaultDir, Program.binFolder);
            URL.InnerText = Program.appInfo.URL;
            ShowWelcomeScreen.InnerText = Convert.ToString(Program.appInfo.ShowWelcomeScreen);
            WelcomePlugin.InnerText = Convert.ToString(Program.appInfo.WelcomePlugin);
            NeverShowProjectionDialog.InnerText = Convert.ToString(Program.appInfo.NeverShowProjectionDialog);
            NoPromptToSendErrorsXml.InnerText = NoPromptToSendErrors.ToString();
            SymbologyLoadingBehavior.InnerText =Convert.ToString(Program.appInfo.SymbologyLoadingBehavior);
            ProjectionMismatch.InnerText = Convert.ToString(Program.appInfo.ProjectionMismatchBehavior);
            ProjectionAbsence.InnerText = Convert.ToString(Program.appInfo.ProjectionAbsenceBehavior);
            ShowLoadingReport.InnerText = Convert.ToString(Program.appInfo.ShowLoadingReport);
            ProjectReloading.InnerText = Convert.ToString(Program.appInfo.ProjectReloading);
            DisplayFloatingScalebar.InnerText = Convert.ToString(Program.appInfo.ShowFloatingScalebar);
            DisplayRedrawSpeed.InnerText = Convert.ToString(Program.appInfo.ShowRedrawSpeed);
            DisplayMapWinGISVersion.InnerText = Convert.ToString(Program.appInfo.ShowMapWinGISVersion);
            DisplayAvoidCollision.InnerText = Convert.ToString(Program.appInfo.ShowAvoidCollision);
            DisplayAutoCreatespatialindex.InnerText = Convert.ToString(Program.appInfo.ShowAutoCreateSpatialindex);

            //将属性添加到节点中
            appInfoXML.Attributes.Append(Name);
            appInfoXML.Attributes.Append(Version);
            appInfoXML.Attributes.Append(BuildDate);
            appInfoXML.Attributes.Append(Developer);
            appInfoXML.Attributes.Append(Comments);
            appInfoXML.Attributes.Append(HelpFilePath);
            appInfoXML.Attributes.Append(UseSplashScreen);
            appInfoXML.Attributes.Append(SplashTime);
            appInfoXML.Attributes.Append(DefaultDir);
            appInfoXML.Attributes.Append(URL);
            appInfoXML.Attributes.Append(ShowWelcomeScreen);
            appInfoXML.Attributes.Append(WelcomePlugin);
            appInfoXML.Attributes.Append(NeverShowProjectionDialog);
            appInfoXML.Attributes.Append(NoPromptToSendErrorsXml);
            appInfoXML.Attributes.Append(LogfilePathXml);
            appInfoXML.Attributes.Append(ShowDynVisWarningsXml);
            appInfoXML.Attributes.Append(ShowLayerAfterDynVisXml);
            appInfoXML.Attributes.Append(SymbologyLoadingBehavior);
            appInfoXML.Attributes.Append(ProjectionMismatch);
            appInfoXML.Attributes.Append(ProjectionAbsence);
            appInfoXML.Attributes.Append(ShowLoadingReport);
            appInfoXML.Attributes.Append(ProjectReloading);
            appInfoXML.Attributes.Append(DisplayFloatingScalebar);
            appInfoXML.Attributes.Append(DisplayRedrawSpeed);
            appInfoXML.Attributes.Append(DisplayMapWinGISVersion);
            appInfoXML.Attributes.Append(DisplayAvoidCollision);
            appInfoXML.Attributes.Append(DisplayAutoCreatespatialindex);

            root.AppendChild(appInfoXML);
        }

        /// <summary>
        /// 保存最近打开的项目（projects）
        /// </summary>
        /// <param name="m_Doc">XmlDocument对象</param>
        /// <param name="root">根节点</param>
        private void AddRecentProjects(XmlDocument m_Doc, XmlElement root)
        {
            try
            {
                XmlElement recentPrj = m_Doc.CreateElement("RecentProjects");
                XmlElement fileToXml;
                int count = RecentProjects.Count;
                if (count != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        fileToXml = m_Doc.CreateElement("Project");
                        fileToXml.InnerText = GetRelativePath(RecentProjects[i].ToString(), ConfigFileName);
                        recentPrj.AppendChild(fileToXml);
                    }
                }
                root.AppendChild(recentPrj);
            }
            catch(Exception ex)
            {
                Program.ShowError(ex);
            }

        }

        /// <summary>
        /// 保存当前程序的显示样式设置到用户配置文件
        /// 执行此操作需要保证程序存在且没有使用任何功能了吗？
        /// </summary>
        /// <param name="m_Doc">xmldocument对象</param>
        /// <param name="root">根节点</param>
        private void AddViewElement(XmlDocument m_Doc, XmlElement root)
        {
            XmlElement View = m_Doc.CreateElement("View");
            XmlAttribute WindowWidth = m_Doc.CreateAttribute("WindowWidth");
            XmlAttribute WindowHeight = m_Doc.CreateAttribute("WindowHeight");
            XmlAttribute LocationX = m_Doc.CreateAttribute("LocationX");
            XmlAttribute LocationY = m_Doc.CreateAttribute("LocationY");
            XmlAttribute WindowState = m_Doc.CreateAttribute("WindowState");
            XmlAttribute ViewColor = m_Doc.CreateAttribute("ViewBackColor");
            XmlAttribute LoadTIFFandIMGasgridAttr = m_Doc.CreateAttribute("LoadTIFFandIMGasgrid");
            XmlAttribute LoadESRIAsGridAttr = m_Doc.CreateAttribute("LoadESRIAsGrid");
            XmlAttribute MouseWheelBehavior = m_Doc.CreateAttribute("MouseWheelBehavior");
            XmlAttribute TransparentSelectionAttr = m_Doc.CreateAttribute("TransparentSelection");
            XmlAttribute LabelsUseProjectLevel = m_Doc.CreateAttribute("LabelsUseProjectLevel");
            XmlAttribute LegendVisible = m_Doc.CreateAttribute("LegendVisible");
            XmlAttribute PreviewVisible = m_Doc.CreateAttribute("PreviewVisible");

            //设置属性
            if (Program.frmMain.WindowState == FormWindowState.Maximized)
            {
                WindowState.InnerText = ((int)(FormWindowState.Maximized)).ToString();
                LocationX.InnerText = "200";
                LocationY.InnerText = "70";
                WindowWidth.InnerText = "1000";
                WindowHeight.InnerText = "620";
            }
            else if (Program.frmMain.WindowState == FormWindowState.Normal)
            {
                WindowState.InnerText = ((int)(FormWindowState.Normal)).ToString();
                LocationX.InnerText = Program.frmMain.Location.X.ToString();
                LocationY.InnerText = Program.frmMain.Location.Y.ToString();
                WindowWidth.InnerText = Program.frmMain.Width.ToString();
                WindowHeight.InnerText = Program.frmMain.Height.ToString();
            }
            else if (Program.frmMain.WindowState == FormWindowState.Minimized)
            {
                WindowState.InnerText = (System.Convert.ToInt32(FormWindowState.Minimized)).ToString();
                LocationX.InnerText = "200";
                LocationY.InnerText = "70";
                WindowWidth.InnerText = "1000";
                WindowHeight.InnerText = "620";
            }

            LoadTIFFandIMGasgridAttr.InnerText = Program.appInfo.LoadTIFFandIMGasgrid.ToString();
            LoadESRIAsGridAttr.InnerText = Program.appInfo.LoadESRIAsGrid.ToString();
            MouseWheelBehavior.InnerText = Program.appInfo.MouseWheelZoom.ToString();
            LabelsUseProjectLevel.InnerText = Program.appInfo.LabelsUseProjectLevel.ToString();

            //保存背景色
            ViewColor.InnerText = ColorScheme.ColorToInt(Program.appInfo.DefaultBackColor).ToString();
            TransparentSelectionAttr.InnerText = TransparentSelection.ToString();

            LegendVisible.InnerText = Program.frmMain.legendPanel.Visible.ToString();

            if (Program.frmMain.previewPanel != null)
            {
                PreviewVisible.InnerText = Program.frmMain.previewPanel.Visible.ToString();
            }
            else
            {
                PreviewVisible.InnerText = "false";
            }

            //将属性添加到节点
            View.Attributes.Append(LabelsUseProjectLevel);
            View.Attributes.Append(TransparentSelectionAttr);
            View.Attributes.Append(WindowWidth);
            View.Attributes.Append(WindowHeight);
            View.Attributes.Append(LocationX);
            View.Attributes.Append(LocationY);
            View.Attributes.Append(WindowState);
            View.Attributes.Append(ViewColor);
            View.Attributes.Append(LoadTIFFandIMGasgridAttr);
            View.Attributes.Append(LoadESRIAsGridAttr);
            View.Attributes.Append(MouseWheelBehavior);
            View.Attributes.Append(LegendVisible);
            View.Attributes.Append(PreviewVisible);

            root.AppendChild(View);
        }

        /// <summary>
        /// 添加插件配置到配置文件
        /// </summary>
        /// <param name="m_Doc">XmlDocument对象</param>
        /// <param name="root">根节点</param>
        /// <param name="loadingConfig"></param>
        private void AddPluginsElement(XmlDocument m_Doc, XmlElement root, bool loadingConfig)
        {
            XmlElement plugins = m_Doc.CreateElement("Plugins");
            Dictionary<string, Interfaces.IPlugin> loadedPlugins = Program.frmMain.m_PluginManager.m_LoadedPlugins;
            foreach (KeyValuePair<string, Interfaces.IPlugin> tPlugin in loadedPlugins)
            {
                AddPluginElement(m_Doc, tPlugin.Value, tPlugin.Key, plugins, loadingConfig);
            }

            root.AppendChild(plugins);
        }

        /// <summary>
        /// 将某一个插件的信息添加到配置文件
        /// </summary>
        private void AddPluginElement(XmlDocument m_Doc, Interfaces.IPlugin iPlugin, string pluginKey, XmlElement parent, bool loadingConfig)
        {
            XmlElement newPlugin = m_Doc.CreateElement("Plugin");
            XmlAttribute settingsString = m_Doc.CreateAttribute("SettingsString");
            XmlAttribute keyXml = m_Doc.CreateAttribute("Key");
            string setString = "";

            if (loadingConfig == false)
            {
                iPlugin.ProjectSaving(ProjectFileName, ref setString);
            }

            settingsString.InnerText = setString;
            keyXml.InnerText = pluginKey;

            newPlugin.Attributes.Append(settingsString);
            newPlugin.Attributes.Append(keyXml);

            parent.AppendChild(newPlugin);
        }

        /// <summary>
        /// 添加应用程序插件配置
        /// </summary>
        private void AddApplicationPluginsElement(XmlDocument m_Doc, XmlElement root, bool loadingConfig)
        {
            XmlElement appPlugins = m_Doc.CreateElement("ApplicationPlugins");
            XmlAttribute Dir = m_Doc.CreateAttribute("PluginDir");

            //保存内部插件的相对路径
            Dir.InnerText = GetRelativePath(Program.appInfo.ApplicationPluginDir, ConfigFileName);

            //保存所有的内部插件
            foreach (KeyValuePair<string, Interfaces.IPlugin> tPlugin in Program.frmMain.m_PluginManager.m_ApplicationPlugins)
            {
                AddPluginElement(m_Doc, tPlugin.Value, tPlugin.Key, appPlugins, loadingConfig);              
            }

            appPlugins.Attributes.Append(Dir);
            root.AppendChild(appPlugins);
        }

        /// <summary>
        /// 添加调色板配置信息到配置文件
        /// </summary>
        private void AddColorPalettes(XmlDocument m_Doc, XmlElement root)
        {
            try
            {
                XmlElement colorPalettes = m_Doc.CreateElement("ColorPalettes");

                if (g_ColorPalettes != null)
                {
                    XmlDocumentFragment docFragment = m_Doc.CreateDocumentFragment();
                    docFragment.InnerXml = g_ColorPalettes.InnerXml;

                    colorPalettes.AppendChild(docFragment);
                }

                root.AppendChild(colorPalettes);
            }
            catch (System.Exception ex)
            {
                Program.ShowError(ex);
            }
        }

        #endregion
    }
}
