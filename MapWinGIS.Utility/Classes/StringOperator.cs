using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MapWinGIS.Utility
{
    public class StringOperator
    {
        /// <summary>
        /// 检查字符串是否为空
        /// </summary>
        public static bool IsEmpty(string str)
        {
            if (str == null || str.Length < 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将一个文本文件中的文本全部以字符串的方式读出来
        /// </summary>
        /// <param name="filename">包含完整路劲的文件名</param>
        /// <param name="timeoutMilliseconds">设置读取时长</param>
        /// <returns>返回所有的字符串</returns>
        public static string WholeFileString(string filename, int timeoutMilliseconds = 1000)
        {
            DateTime tryUntil = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            while (tryUntil > DateTime.Now)
            {
                try
                {
                   return File.ReadAllText(filename);
                }
                catch (Exception)
                {
                    if (DateTime.Now > tryUntil)
                    {
                        return "";
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 将指定字符串写入指定的文件中
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="fileContents">要写入的字符串</param>
        public static void SaveFileString(string filename, string fileContents)
        {
            try
            {
                string directory = Path.GetDirectoryName(filename);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(filename, fileContents);
            }
            catch (Exception ex)
            {
                Logger.Dbg("保存文本到文件失败，" + ex.ToString());
            }
        }

    }
}
