/****************************************************************************
 * 文件名:lodAppInfo.cs
 * 描  述:
 * **************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace MapWinGIS.MainProgram
{
    public sealed class App
    {
        /// <summary>
        /// 获取程序执行路径
        /// eg:D:\MapWinGIS\MapWinGIS\bin(\MapWinGIS.MainProgram.exe)
        /// </summary>
        public static string Path
        {
            get
            {
                string tStr = System.Windows.Forms.Application.ExecutablePath; //D:\MapWinGIS\MapWinGIS\bin\MapWinGIS.MainProgram.exe
                return tStr.Substring(0, tStr.LastIndexOf("\\"));
            }
        }

        /// <summary>
        /// 获取版本信息 主，次，内部
        /// </summary>
        public static string VersionString
        {
            get
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                //不用使用default(string)，default一般在泛型中使用default(T)             
                string versionStr = version.FileMajorPart + "." + version.FileMinorPart + "." + version.FileBuildPart;
                return versionStr;
            }

        }












    }
}
