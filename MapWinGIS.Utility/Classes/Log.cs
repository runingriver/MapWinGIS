using System;
using System.Diagnostics;
using System.IO;

namespace MapWinGIS.Utility
{
    /// <summary>
    /// 支持在指定的目录下创建一个指定名称的日志文件
    /// 可以获取指定路径文件的最近一条错误信息
    /// 可以将信息写入你指定的文件中
    /// </summary>
    public class Log
    {
        private static string fileName =Environment.CurrentDirectory + "\\Log\\UtilsLog.txt";

        /// <summary>
        /// 设置日志文件，需包含文件名的完整路径
        /// </summary>
        /// <param name="newFilename"></param>
        public static void SetLogFile(string newFilename)
        {
            fileName = newFilename;
        }

        ///<summary>
        ///删除之前所有的错误信息，即将日志文件删除
        ///</summary>
        public static void ClearLog()
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }

        /// <summary>
        /// 从日志文件读取最后一条错误信息
        /// </summary>
        public static string GetLastMsg()
        {
            StreamReader sr;
            string sLine = "";
            string errorMsg = "";
            if (File.Exists(fileName))
            {
                try
                {
                    sr = new StreamReader(fileName);
                    do
                    {
                        sLine = sr.ReadLine();
                        if (sLine != null)
                        {
                            errorMsg = sLine;
                        }
                    } while (sLine != null);

                    sr.Close();
                }
                catch (IOException)
                {
                    return ("读取文件失败!!");
                }
            }
            if (errorMsg.Equals(""))
            {
                errorMsg = "没有记录任何错误信息.";
            }
            return errorMsg;
        }

        /// <summary>
        /// 将小心写入日志文件
        /// </summary>
        public static void PutMsg(string Msg)
        {
            StreamWriter sw;
            if (!File.Exists(fileName)) //日志文件不存在，创建并写入
            {
                try
                {
                    sw = File.CreateText(fileName);
                    sw.WriteLine(DateTime.Now.ToString() + " " + Msg);
                    sw.Close();
                }
                catch (IOException)
                {
                    Debug.WriteLine("写入新建的文件失败!!");
                }
            }
            else //存在，直接附加
            {
                try
                {
                    sw = File.AppendText(fileName);
                    sw.WriteLine(DateTime.Now.ToString() + " " + Msg);
                    sw.Close();
                }
                catch (IOException)
                {
                    Debug.WriteLine("写入已存在的文件错误!!!");
                }
            }
        }
    }
}
