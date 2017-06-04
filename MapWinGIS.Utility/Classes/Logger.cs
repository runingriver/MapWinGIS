using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace MapWinGIS.Utility
{
    public class Logger
    {
        /// <summary>
        /// 毫秒，一天有86400000毫秒
        /// </summary>
        private const double MillisecondsPerDay = 86400000; 
       
        /// <summary>
        /// 是否自动将缓冲区的数据写入到文件
        /// </summary>
        public static bool AutoFlush = true;

        /// <summary>
        /// 应用于自定义消息盒子的图标
        /// </summary>
        public static System.Drawing.Icon Icon = null;

        /// <summary>
        /// 进度条刷新时间间隔
        /// </summary>
        private static double pProgressRefresh = 200.0 / MillisecondsPerDay;
        /// <summary>
        /// 进度条开始时间
        /// </summary>
        private static double pProgressStartTime = 0;
        /// <summary>
        /// 进度条最近一次更新时间
        /// </summary>
        private static double pProgressLastUpdate = 0;

        /// <summary>
        /// 设置默认值，方便在每次使用时不必去检测进度条是否已经设置
        /// </summary>
        private static IProgressStatus pProgressStatus = new NullProgressStatus(); 
        /// <summary>
        /// 是否能够取消进度条，也就是是否实现了IProgressStatusCancel接口
        /// </summary>
        private static bool pCancelable = false;

        /// <summary>
        /// 以一种特定的编码向流中写入字符对象的StreamWriter变量。
        /// </summary>
        private static StreamWriter pFileStream;

        /// <summary>
        /// 是否在每次记录前面附加时间，默认附加
        /// </summary>
        private static bool pTimeStamp = true; 
        /// <summary>
        /// 是否在每次运行时将时间置0
        /// </summary>
        private static bool pTimeStampRelative = true; 
        /// <summary>
        /// 开始运行的时间
        /// </summary>
        private static DateTime pTimeStampStart = DateTime.Now; 

        /// <summary>
        /// 包含完整路径名的“记录文件”
        /// </summary>
        private static string pFileName = ""; 

        /// <summary>
        /// 是否显示消息框
        /// </summary>
        private static bool pDisplayMessageBoxes = true; 

        /// <summary>
        /// 最近一次记录的信息
        /// </summary>
        private static string pLastDbgText = "";

        #region----------------------属性-------------------------------------
        /// <summary>
        /// 获取包含文件名、扩展名的日志文件的完整路径
        /// </summary>
        public static string FileName
        {
            get { return pFileName; }
        }
        /// <summary>
        ///  获取或设置是否在每次记录日志时在开头添加时间戳
        /// </summary>
        public static bool TimeStamping
        {
            get { return pTimeStamp; }
            set { pTimeStamp = value; }
        }
        /// <summary>
        /// 获取或设置以哪种方式记录时间
        /// true-以程序开始运行的时间记录：hh:mm:ss.milliseconds
        /// false- 以当前北京时间记录 ：YYYY-MM-DD hh:mm:ss.milliseconds
        /// </summary>
        public static bool TimeStampRelative
        {
            get { return pTimeStampRelative; }
            set { pTimeStampRelative = value; }
        }
        
        /// <summary>
        /// 获取或设置进度条的状态和进度
        /// </summary>
        public static IProgressStatus ProgressStatus
        {
            get { return pProgressStatus; }
            set 
            {
                pProgressStatus = value;
                pCancelable = value is IProgressStatusCancel;
            }
        }
        /// <summary>
        /// 获取或设置用户是否请求取消当前操作
        /// </summary>
        public static bool Canceled
        {
            get { return pCancelable && (((IProgressStatusCancel)pProgressStatus).Canceled); }
            set
            {
                if (pCancelable)
                {
                    ((IProgressStatusCancel)pProgressStatus).Canceled = value;
                }
            }
        }
        
        /// <summary>
        /// 获取或设置最近一次调试的信息
        /// </summary>
        public static string LastDbgText
        {
            get { return pLastDbgText; }
            set { pLastDbgText = value; }
        }
        /// <summary>
        /// 是否显示消息框
        /// </summary>
        public static bool DisplayMessageBoxes
        {
            get { return pDisplayMessageBoxes; }
            set { pDisplayMessageBoxes = value; }
        }

        public static double ProgressRefresh
        {
            get
            {
                return pProgressRefresh * MillisecondsPerDay;
            }
            set
            {
                pProgressRefresh = value / MillisecondsPerDay;
            }
        }

        #endregion---------------------------------------------------------------

        #region----------------------方法-------------------------------------
        /// <summary>
        /// 开始向指定文件中写入程序运行日志
        /// </summary>
        /// <param name="logFileName">要写入运行日志的文件名</param>
        /// <param name="append">true-将记录附加到现有文件中，false-创建一个新的文件记录</param>
        /// <param name="renameExisting">true-重命名现有文件作为新的日志文件，false-覆写当前存在的文件</param>
        /// <param name="forceNameChange">true-改变日志文件</param>
        public static void StartToFile(string logFileName, bool append = false, bool renameExisting = true, bool forceNameChange = false)
        {
            if (forceNameChange || pFileStream == null)//允许修改日志文件的文件名
            {
                if (pFileStream != null) //forceNameChange=true,改变文件名，并且文件已打开，关闭已经存在的文件
                {
                    try
                    {
                        pFileStream.Close();
                    }
                    catch
                    {
                        throw new Exception("日志文件关闭失败！");
                    }
                    pFileStream = null;
                }

                pFileName = logFileName;

                if (pFileName.Length > 0)
                {
                    FileOperator.MkDirPath(FileOperator.PathNameOnly(pFileName));

                    if (FileOperator.FileOrDirExists(pFileName)) //该文件是否存在
                    {
                        if (!append) //文件存在，创建一个新的文件
                        {
                            if (renameExisting)//新建一个新名字的该文件，原文件保留
                            {
                                File.Move(pFileName, MakeLogName(pFileName));
                            }
                            else//删除原文件，以便可以新建一个新的该文件名的文件
                            {
                                File.Delete(pFileName);
                            }
                        }
                    }
                    else //文件不存在，则给的路径名
                    {
                        pFileName += "\\" + CreatLogName(pFileName);
                    }
                    pFileStream = new StreamWriter(pFileName, append);
                    Dbg("开始写入日志文件 " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                }
            }
        }

        /// <summary>
        /// 将缓冲区中的log信息写入日志文件,并清空缓冲区
        /// </summary>
        public static void Flush()
        {
            if (pFileStream != null)
            {
                pFileStream.Flush();
            }
        }

        /// <summary>
        /// 将指定的消息写入日志文件
        /// </summary>
        public static void Dbg(params string[] aMessages)
        {
            string lText = ""; //记录要写入日志文件的字符
            string lFullMessage = ""; //整合所有传过来的消息

            foreach (string lMessage in aMessages)//将参数传过来的string组合
            {
                if (lFullMessage.Length > 0)
                {
                    lFullMessage += ":" + lMessage.ToString();
                }
                else
                {
                    lFullMessage += lMessage.ToString();
                }
            }

            if (pTimeStamp)//在消息前加时间
            {
                if (pTimeStampRelative)//采用相对时间
                {
                    lText += (DateTime.Now - pTimeStampStart).Hours + ":" + (DateTime.Now - pTimeStampStart).Minutes + ":" + (DateTime.Now - pTimeStampStart).Seconds + "." + (DateTime.Now - pTimeStampStart).Milliseconds + "\t";
                }
                else//采用实际时间
                {

                    lText += DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" +DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "\t";
                }
            }

            pLastDbgText = MethodCallingLogger() + ":" + lFullMessage; //在消息前面，事件后面添加调用该方法的"文件名：方法名"
            lText += pLastDbgText;

            try //写入日志文件
            {
                if (pFileStream != null)
                {
                    pFileStream.WriteLine(lText);
                    if (AutoFlush)
                    {
                        pFileStream.Flush();
                    }
                }
                Debug.WriteLine(lText);
            }
            catch (Exception)
            {
                Debug.Write("记录失败:" + lText);
            }
        }

        /// <summary>
        /// 返回要将日志写入日志文件的方法
        /// eg：返回lodMain:CheckPROJNAD
        /// </summary>
        /// <returns></returns>
        private static string MethodCallingLogger()
        {
            try
            {
                StackTrace lStackTrace = new StackTrace(true);
                //Frame 0=这个方法本身，1=在Logger中的方法(由于这是一个私有方法)，2= Logger外的第一个栈帧
                //堆栈帧从 0 开始编号，0 表示最后推出的堆栈帧。
                System.Reflection.Module lModule = lStackTrace.GetFrame(0).GetMethod().Module;
                int lFrameIndex = 2;
                StackFrame lFrame = lStackTrace.GetFrame(lFrameIndex);

                //查找外部模块调用本模块函数的栈帧
                try
                {
                    while (lFrameIndex < lStackTrace.FrameCount && lFrame.GetMethod().Module.Equals(lModule))
                    {
                        lFrameIndex++;
                        lFrame = lStackTrace.GetFrame(lFrameIndex);
                    }
                }
                catch (Exception)
                {
                    //无法继续向上寻找栈帧
                }

                string lFrameFilename = lFrame.GetFileName();//获取模块外部调用本方法的文件名
                if (lFrameFilename != null)
                {
                    return MiscUtils.GetBaseName(lFrameFilename) + ":" + lFrame.GetMethod().Name;
                }
                else
                {
                    return lFrame.GetMethod().Name;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 如果需要文件名独一无二，则添加一个数字到该文件名中
        /// </summary>
        /// <param name="logFilename">要重命名的文件名</param>
        /// <returns>eg:C:\Log\MapWinGIS#3.log</returns>
        private static string MakeLogName(string logFilename)
        {
            string tryName;
            int ltry = 1;
            do
            {
                tryName = FileOperator.FilenameNoExt(logFilename) + "#" + ltry.ToString() + ".log";
                ltry++;
            } while (FileOperator.FileOrDirExists(tryName));
            return tryName;
        }

        /// <summary>
        /// 创建一个独一无二Log的文件名
        /// 返回文件名和扩展名
        /// </summary>
        /// <param name="logDir">不含文件名的路径</param>
        private static string CreatLogName(string logDir)
        {
            string newName;
            int num = 1;
            do
            {
                newName = DateTime.Now.ToShortDateString() + "#" + num.ToString() + ".log";
                num++;
            } while (File.Exists(logDir + "\\" + newName));
            return newName;
        }

        /// <summary>
        /// 自定义消息对话框
        /// </summary>
        public static string MsgCustom(string message, string title, params string[] buttonLabels)
        {
            Dbg("自定义消息框 MsgCustom:" + message + ":标题:" + title + ":样式:" + string.Join(",", buttonLabels));
            string lResult = "";
            if (pDisplayMessageBoxes)
            {
                CustomMsgBox lMsgBox = CustomMsgBox();
                lResult = lMsgBox.AskUser(message, title, buttonLabels);
                Dbg("自定义消息框结果:" + lResult);
                pLastDbgText = "";
            }
            else //不显示自定义消息框
            {
                lResult = buttonLabels[0];
                foreach (string lButtonLabel in buttonLabels)
                {
                    if (lButtonLabel.Contains("+"))
                    {
                        lResult = lButtonLabel;
                    }
                }
                Dbg("用户忽略该消息框:结果:" + lResult);
                pLastDbgText = message;
            }
            return lResult;
        }

        /// <summary>
        /// 自定义消息对话框
        /// 可以设置设置拥有此窗体的窗体
        /// </summary>
        public static string MsgCustomOwned(string message, string title, Form owner, params string[] buttonLabels)
        {
            Dbg("自定义消息框 MsgCustomOwned:" + message + ":标题:" + title + ":样式:" + string.Join(",", buttonLabels));
            string lResult = "";
            if (pDisplayMessageBoxes)
            {
                CustomMsgBox lMsgBox = CustomMsgBox();
                lMsgBox.Owner = owner;
                lResult = lMsgBox.AskUser(message, title, buttonLabels);
                Dbg("消息对话框结果:" + lResult);
                pLastDbgText = "";
            }
            else
            {
                lResult = buttonLabels[0];
                foreach (string lButtonLabel in buttonLabels)
                {
                    if (lButtonLabel.Contains("+"))
                    {
                        lResult = lButtonLabel;
                    }
                }
                Dbg("用户忽略该消息框:结果:" + lResult);
                pLastDbgText = message;
            }
            return lResult;
        }

        /// <summary>
        /// 自定义消息对话框
        /// 可以设置设置Checkbox
        /// </summary>
        public static string MsgCustomCheckbox(string aMessage, string aTitle, string aRegistryCheckboxText, string aRegistryAppName, string aRegistrySection, string aRegistryKey, params string[] aButtonLabels)
        {
            Dbg("MsgCustom:" + aMessage + ":标题:" + aTitle + ":样式:" + string.Join(",", aButtonLabels));
            string lResult = "";

            string lRegistryLabel = null; //Interaction.GetSetting(aRegistryAppName, aRegistrySection, aRegistryKey, "");
            if (lRegistryLabel.Length > 0)
            {
                lResult = lRegistryLabel;
            }
            else if (pDisplayMessageBoxes)
            {
                CustomMsgBox lMsgBox = CustomMsgBox();
                lMsgBox.RegistryCheckboxText = aRegistryCheckboxText;
                lMsgBox.RegistryAppName = aRegistryAppName;
                lMsgBox.RegistrySection = aRegistrySection;
                lMsgBox.RegistryKey = aRegistryKey;
                lResult = (string)(lMsgBox.AskUser(aMessage, aTitle, aButtonLabels));
                Dbg("消息对话框结果:" + lResult);
                pLastDbgText = "";
            }
            else 
            {
                lResult = aButtonLabels[0];
                foreach (string lButtonLabel in aButtonLabels)
                {
                    if (lButtonLabel.Contains("+"))
                    {
                        lResult = lButtonLabel;
                    }
                }
                Dbg("用户忽略该消息框:结果:" + lResult);
                pLastDbgText = aMessage; 
            }
            return lResult;
        }

        /// <summary>
        /// 创建一个自定义的消息盒
        /// 该方法主要是设置是否显示图标
        /// </summary>
        /// <returns></returns>
        private static CustomMsgBox CustomMsgBox()
        {
            CustomMsgBox lMsgBox = new CustomMsgBox();
            if (Logger.Icon == null)
            {
                lMsgBox.ShowIcon = false;
            }
            else
            {
                lMsgBox.Icon = Logger.Icon;
                lMsgBox.ShowIcon = true;
            }
            return lMsgBox;
        }


        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Msg(string message)
        {
            return Msg(message, MessageBoxButtons.OK);

        }
        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Msg(string message, string title)
        {
            return Msg(message, MessageBoxButtons.OK, title);
        }

        public static void Message(string message)
        {
            Dbg("消息Message：" + message);
            MessageBox.Show(message);
        }
        public static void Message(string message, string title)
        {
            Dbg("消息Message：" + message + "标题：" + title);
            MessageBox.Show(message,title);
        }
        public static void Message(string message, string title, MessageBoxIcon icon, MessageBoxButtons button)
        {
            Dbg("消息(Message):" + message + ":标题:" + title + ":按钮:" + button.ToString());
            Flush();
            if (pDisplayMessageBoxes) //显示消息盒
            {
                MessageBox.Show(message, title, button, icon);
                pLastDbgText = "";
            }
            else //不显示消息盒
            {
                pLastDbgText = message; //设置调试信息
            }
        }

        public static void Msg(string message, string title, MessageBoxIcon icon)
        {
            Msg(message, title, icon, MessageBoxButtons.OK);
        }
        public static void Msg(string message, string title, MessageBoxButtons button)
        {
            Msg(message, title, MessageBoxIcon.None, MessageBoxButtons.OK);
        }
        public static void Msg(string message, string title, MessageBoxIcon icon, MessageBoxButtons button)
        {
            Dbg("消息(Msg):" + message + ":标题:" + title + ":按钮:" + button.ToString());
            Flush();
            if (pDisplayMessageBoxes) //显示消息盒
            {
                MessageBox.Show(message, title, button, icon);
                pLastDbgText = "";
            }
            else //不显示消息盒
            {
                pLastDbgText = message; //设置调试信息
            }
        }

        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Msg(string message, MessageBoxButtons msgBoxBtn)
        {
            Dbg("消息Message：" + message);

            string msgTitle = "";
            if (message.IndexOf(":") > 0)
            {
                msgTitle = message.Substring(0, message.IndexOf(":"));
            }
            return Msg(message, msgBoxBtn, msgTitle);
        }
        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Msg(string message, MessageBoxButtons msgBoxBtn, string title)
        {
            DialogResult msgBoxResult;
            switch (msgBoxBtn)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    msgBoxResult = DialogResult.Abort;
                    break;
                case MessageBoxButtons.OKCancel:
                    msgBoxResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.OK:
                    msgBoxResult = DialogResult.OK;
                    break;
                case MessageBoxButtons.RetryCancel:
                    msgBoxResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    msgBoxResult = DialogResult.No;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    msgBoxResult = DialogResult.Cancel;
                    break;
                default :
                    msgBoxResult = DialogResult.Cancel;
                    break;
            }
            return Msg(message, msgBoxBtn, msgBoxResult, title);
        }
        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Msg(string message, MessageBoxButtons msgBoxBtn, DialogResult msgBoxDefaultResult, string title)
        {
            Dbg("消息(Msg):" + message + ":标题:" + title + ":按钮:" + msgBoxBtn.ToString());
            Flush();

            DialogResult dialogResult;

            if (pDisplayMessageBoxes) //显示消息盒
            {
                dialogResult = MessageBox.Show(message, title, msgBoxBtn);
                Dbg("消息盒的结果:" + dialogResult + ":" + System.Enum.GetName(dialogResult.GetType(), dialogResult));
                pLastDbgText = "";
            }
            else //不显示消息盒则，用默认值
            {
                dialogResult = msgBoxDefaultResult;
                Dbg("用户忽略了消息盒的按钮:采用默认值:" + dialogResult + ":" + System.Enum.GetName(dialogResult.GetType(), dialogResult));
                pLastDbgText = message; //设置调试信息
            }
            return dialogResult;
        }


        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Message(string text, string caption, MessageBoxButtons buttons)
        {
            Dbg("消息Message:" + text + ":标题:" + caption + ":按钮:" + buttons.ToString());
            Flush();
            DialogResult lResult = DialogResult.OK;
            if (pDisplayMessageBoxes)
            {
                lResult = MessageBox.Show(text, caption, buttons);
                Dbg("消息框结果:" + lResult + ":" + System.Enum.GetName(lResult.GetType(), lResult));
                pLastDbgText = "";
            }
            else
            {
                Dbg("用户没有点击消息框的按钮:使用默认按钮:" + System.Enum.GetName(lResult.GetType(), lResult));
                pLastDbgText = text; //消息没有写入日志，设置最后一次的消息 
            }
            return lResult;
        }
        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Message(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, DialogResult defaultResult)
        {
            Dbg("消息Message:" + text + ":标题:" + caption + ":按钮:" + buttons.ToString());
            Flush();
            DialogResult lResult = DialogResult.OK;
            if (pDisplayMessageBoxes)
            {
                lResult = MessageBox.Show(text, caption, buttons, icon, DialogResultToMessageBoxDefaultButton(buttons, defaultResult));
                Dbg("消息框结果:" + lResult + ":" + System.Enum.GetName(lResult.GetType(), lResult));
                pLastDbgText = "";
            }
            else
            {
                lResult = defaultResult;
                Dbg("用户没有点击消息框的按钮:使用默认按钮:" + lResult.ToString() + ":" + System.Enum.GetName(lResult.GetType(), lResult));
                pLastDbgText = text; //消息没有写入日志，设置最后一次的消息 
            }
            return lResult;
        }
        /// <summary>
        /// 显示消息对话框，并将消息写入日志
        /// VB请用Logger.Msg()，C#请用Logger.Message()
        /// </summary>
        public static DialogResult Message(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Message(text, caption, buttons, icon, MessageBoxDefaultButtonToDialogResult(buttons, defaultButton));
        }

        /// <summary>
        /// 通过消息对话框的默认按钮，获得对话框的结果
        /// </summary>
        private static DialogResult MessageBoxDefaultButtonToDialogResult(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            if (buttons == MessageBoxButtons.AbortRetryIgnore)
            {
                if (defaultButton == MessageBoxDefaultButton.Button1)
                {
                    return DialogResult.Abort;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    return DialogResult.Retry;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button3)
                {
                    return DialogResult.Ignore;
                }
            }
            else if (buttons == MessageBoxButtons.OK)
            {
                return DialogResult.OK;
            }
            else if (buttons == MessageBoxButtons.OKCancel)
            {
                if (defaultButton == MessageBoxDefaultButton.Button1)
                {
                    return DialogResult.OK;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    return DialogResult.Cancel;
                }
            }
            else if (buttons == MessageBoxButtons.RetryCancel)
            {
                if (defaultButton == MessageBoxDefaultButton.Button1)
                {
                    return DialogResult.Retry;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    return DialogResult.Cancel;
                }
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                if (defaultButton == MessageBoxDefaultButton.Button1)
                {
                    return DialogResult.Yes;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    return DialogResult.No;
                }
            }
            else if (buttons == MessageBoxButtons.YesNoCancel)
            {
                if (defaultButton == MessageBoxDefaultButton.Button1)
                {
                    return DialogResult.Yes;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    return DialogResult.No;
                }
                else if (defaultButton == MessageBoxDefaultButton.Button3)
                {
                    return DialogResult.Cancel;
                }
            }
            return DialogResult.OK;
        }

        /// <summary>
        /// 通过对话框返回值设置消息对话框上的默认按钮
        /// </summary>
        private static MessageBoxDefaultButton DialogResultToMessageBoxDefaultButton(MessageBoxButtons buttons, DialogResult defaultButton)
        {
            if (buttons == MessageBoxButtons.AbortRetryIgnore)
            {
                if (defaultButton == DialogResult.Abort)
                {
                    return MessageBoxDefaultButton.Button1;
                }
                else if (defaultButton == DialogResult.Retry)
                {
                    return MessageBoxDefaultButton.Button2;
                }
                else if (defaultButton == DialogResult.Ignore)
                {
                    return MessageBoxDefaultButton.Button3;
                }
            }
            else if (buttons == MessageBoxButtons.OK)
            {
                return MessageBoxDefaultButton.Button1;
            }
            else if (buttons == MessageBoxButtons.OKCancel)
            {
                if (defaultButton == DialogResult.OK)
                {
                    return MessageBoxDefaultButton.Button1;
                }
                else if (defaultButton == DialogResult.Cancel)
                {
                    return MessageBoxDefaultButton.Button2;
                }
            }
            else if (buttons == MessageBoxButtons.RetryCancel)
            {
                if (defaultButton == DialogResult.Retry)
                {
                    return MessageBoxDefaultButton.Button1;
                }
                else if (defaultButton == DialogResult.Cancel)
                {
                    return MessageBoxDefaultButton.Button2;
                }
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                if (defaultButton == DialogResult.Yes)
                {
                    return MessageBoxDefaultButton.Button1;
                }
                else if (defaultButton == DialogResult.No)
                {
                    return MessageBoxDefaultButton.Button2;
                }
            }
            else if (buttons == MessageBoxButtons.YesNoCancel)
            {
                if (defaultButton == DialogResult.Yes)
                {
                    return MessageBoxDefaultButton.Button1;
                }
                else if (defaultButton == DialogResult.No)
                {
                    return MessageBoxDefaultButton.Button2;
                }
                else if (defaultButton == DialogResult.Cancel)
                {
                    return MessageBoxDefaultButton.Button3;
                }
            }
            return MessageBoxDefaultButton.Button1;
        }


        /// <summary>
        /// 在状态条上显示当前状态
        /// </summary>
        public static void Status(string message)
        {
            try
            {
                pProgressStatus.Status(message);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 在状态条上显示当前状态,并指示是否写入日志
        /// </summary>
        public static void Status(string message, bool isLog)
        {
            if (isLog)
            {
                Dbg("状态信息: " + message);
            }
            Status(message);
        }

        /// <summary>
        /// 显示长时间运行任务的进度
        /// </summary>
        /// <param name="current">任务在进度条的当前位置</param>
        /// <param name="last">任务完成位置</param>
        public static void Progress(int current, int last)
        {
            if (pCancelable && ((IProgressStatusCancel)pProgressStatus).Canceled)
            {
                throw new ProgressCancelException();
            }
            try
            {
                double lCurTime = DateTime.Now.ToOADate();
                if (current == last) //进度条走完，关闭显示进度条
                {
                    pProgressStatus.Progress(current, last);
                    pProgressStartTime = 0;
                    pProgressLastUpdate = 0;
                    Flush();
                }
                else if (pProgressStartTime == 0) //开始新的进度条显示
                {
                    pProgressStatus.Progress(current, last);
                    pProgressStartTime = lCurTime;
                    pProgressLastUpdate = lCurTime;
                    Flush();
                }
                else if (pProgressRefresh == 0 || lCurTime - pProgressLastUpdate > pProgressRefresh)
                {
                    //自上一次更新，时间间隔足够长
                    pProgressStatus.Progress(current, last);
                    Application.DoEvents(); 
                    Flush();
                    pProgressLastUpdate = lCurTime;
                }
            }
            catch (Exception) //进度条显示过程中，忽视任何异常
            {
            }
        }

        /// <summary>
        /// 整合状态条信息和进度条信息
        /// </summary>
        public static void Progress(string message, int current, int last)
        {
            Status(message);
            Progress(current, last);
        }


        #endregion--------------------------------------------------------------
    }
}
