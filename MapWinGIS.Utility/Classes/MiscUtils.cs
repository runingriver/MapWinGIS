using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MapWinGIS.Utility
{
    /// <summary>
    /// 包括获得文件名，扩展名，调试信息
    /// </summary>
    public class MiscUtils
    {
        /// <summary>
        /// 从包含文件名的完整路径中提取文件名
        /// eg：C:\Test\test.cs --> test
        /// </summary>
        public static string GetBaseName(string Filename)
        {
            int i;
            string tStr = "";

            Filename.Replace("/", "\\");

            //handle ESRI Grids correctly
            if (Filename.Substring(Filename.LastIndexOf("\\") + 1).Trim().ToLower() == "sta.adf")
            {
                tStr = Filename.Substring(0, Filename.LastIndexOf("\\"));
                tStr = tStr.Substring(tStr.LastIndexOf("\\") + 1);
                return tStr;
            }

            int len = Filename.Length;
            for (i = len; i >= 1; i--) //获得包含扩展名的文件名
            {
                if (Filename.Substring(i - 1, 1) == "\\")
                {
                    tStr = Filename.Substring(i, len - i); //此处由1改为i
                    break;
                }
            }
            if (tStr.Length == 0)//若没有获得含扩展名的文件名，返回去掉扩展名但包含路径的文件名
            {
                tStr = Filename;
            }
            for (i = tStr.Length; i >= 1; i--)//去掉扩展名
            {
                if (tStr.Substring(i - 1, 1) == ".")
                {
                    tStr = tStr.Substring(0, i - 1);
                    return tStr;
                }
            }
            return tStr;
        }

        /// <summary>
        /// 获得给定字符串的扩展名
        /// </summary>
        public static string GetExtensionName(string filename)
        {
            int i;
            string str = filename;
            for (i = str.Length; i >= 1; i--)
            {
                if (str.Substring(i-1, 1) == ".")
                {
                    str = str.Substring(i, str.Length - i);
                    break;
                }
            }
            return str;
        }

        /// <summary>
        /// 获取调试信息。
        /// 包括：系统信息、命令行信息、环境变量、性能信息、模块信息
        /// </summary>
        public static string GetDebugInfo()
        {
            StringBuilder retStringBuf = new StringBuilder();
            try
            {
                retStringBuf.AppendLine("MapWinGIS.Utility(调试报告)程序集版本：" + Environment.Version.Major.ToString() + "." + Environment.Version.Minor.ToString() + "." + Environment.Version.Revision.ToString() + "." + Environment.Version.Build.ToString());
                retStringBuf.AppendLine("操作系统：" + Environment.OSVersion.Platform.ToString());
                retStringBuf.AppendLine("系统补丁包：" + Environment.OSVersion.ServicePack);
                retStringBuf.AppendLine("系统主版本号：" + Environment.OSVersion.Version.Major.ToString());
                retStringBuf.AppendLine("系统次版本号：" + Environment.OSVersion.Version.Minor.ToString());
                retStringBuf.AppendLine("系统修订版本号：" + Environment.OSVersion.Version.MajorRevision.ToString());
                retStringBuf.AppendLine("系统内部版本号：" + Environment.OSVersion.Version.Build.ToString());
                retStringBuf.AppendLine(Environment.NewLine);
                retStringBuf.AppendLine("-----------------------------------------------");
                retStringBuf.Append("本地磁盘：");
                foreach (string s in Environment.GetLogicalDrives())
                {
                    retStringBuf.Append(s + " ");
                }
                retStringBuf.Append(Environment.NewLine);
                retStringBuf.AppendLine("系统目录：" + Environment.SystemDirectory);
                retStringBuf.AppendLine("当前目录：" + Environment.CurrentDirectory);
                retStringBuf.AppendLine("命令行：" + Environment.CommandLine);
                retStringBuf.Append("命名行参数：");
                foreach (string s in Environment.GetCommandLineArgs())
                {
                    retStringBuf.Append(s + " ");
                }
                retStringBuf.Append(Environment.NewLine);
                retStringBuf.Append(Environment.NewLine);
                retStringBuf.AppendLine("--------------环境变量--------------");
                foreach (DictionaryEntry ienum in Environment.GetEnvironmentVariables())
                {
                    retStringBuf.AppendLine(ienum.Key + "==" + ienum.Value);
                    retStringBuf.AppendLine("--------------------------------------------");
                }
                retStringBuf.Append(Environment.NewLine);
                retStringBuf.AppendLine("--------------性能信息(Bytes)--------------");
                Process currentPro = Process.GetCurrentProcess();
                currentPro.Refresh();
                retStringBuf.Append("私有内存空间：").AppendLine(currentPro.PrivateMemorySize64.ToString());
                retStringBuf.Append("虚拟内存空间：").AppendLine(currentPro.VirtualMemorySize64.ToString());
                retStringBuf.Append("CPU 总时间：").AppendLine(currentPro.TotalProcessorTime.ToString());
                retStringBuf.Append("用户占用CPU 总时间：").AppendLine(currentPro.UserProcessorTime.ToString());
                retStringBuf.Append(Environment.NewLine);

                retStringBuf.AppendLine("--------------模块信息--------------");
                ProcessModuleCollection myProcessModuleCollection = currentPro.Modules;
                foreach (ProcessModule myProcessModule in myProcessModuleCollection)
                {
                    retStringBuf.Append("模块名：").AppendLine(myProcessModule.ModuleName);
                    retStringBuf.Append("路径：").AppendLine(myProcessModule.FileName);
                    if (myProcessModule.FileVersionInfo.FileVersion != null)
                    {
                        retStringBuf.Append("版本：").AppendLine(myProcessModule.FileVersionInfo.FileVersion.ToString());
                    }
                    retStringBuf.AppendLine("--------------------------------------------");
                }
                retStringBuf.Append(Environment.NewLine);

                retStringBuf.AppendLine("--------------获取调试信息完成--------------");
            }
            catch
            { }

            return retStringBuf.ToString();
        }

        /// <summary>
        /// 将字符串转换成双精度浮点数，如果转失败尝试将","转换成"."再试一次.
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defaultValue">转换失败返回的默认值</param>
        public static double ParseDouble(string str, double defaultValue)
        {
            bool success = false;
            double val;
            success = System.Convert.ToBoolean(double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val));
            if (!success)
            {
                str = str.Replace(",", ".");
                success = System.Convert.ToBoolean(double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val));
            }
            return success ? val : defaultValue;
        }

    }
}
