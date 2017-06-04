using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace MapWinGIS.Utility
{
    public class DataManagement
    {
        /// <summary>
        /// 是否存在与FileName同名不同扩展名的文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool MetaDataExists(string FileName)
        {
            if (GetMetaDataFiles(FileName) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取同一目录下与FileName同名不同扩展名的文件集合
        /// </summary>
        public static string[] GetMetaDataFiles(string FileName)
        {
            try
            {
                string tStr;
                ArrayList list = new ArrayList();

                //删除‘\’
                if (FileName[FileName.Length - 1] == '\\')
                {
                    FileName = FileName.Substring(0, FileName.Length - 1);
                }

                // 尝试添加扩展名
                tStr = FileName + ".htm";
                if (System.IO.File.Exists(tStr))
                {
                    list.Add(tStr);
                }

                tStr = FileName + ".html";
                if (System.IO.File.Exists(tStr))
                {
                    list.Add(tStr);
                }

                tStr = FileName + ".xml";
                if (System.IO.File.Exists(tStr))
                {
                    list.Add(tStr);
                }

                //尝试去掉扩展名，添加上指定的扩展名，看该文件是否存在
                string ext = System.IO.Path.GetExtension(FileName);
                FileName = FileName.Substring(0, FileName.Length - ext.Length);

                tStr = FileName + ".htm";
                if (System.IO.File.Exists(tStr) && list.Contains(tStr) == false)
                {
                    list.Add(tStr);
                }

                tStr = FileName + ".html";
                if (System.IO.File.Exists(tStr) && list.Contains(tStr) == false)
                {
                    list.Add(tStr);
                }

                tStr = FileName + ".xml";
                if (System.IO.File.Exists(tStr) && list.Contains(tStr) == false)
                {
                    list.Add(tStr);
                }

                if (list.Count == 0)
                {
                    return null;
                }
                else
                {
                    return ((string[])(list.ToArray(typeof(string))));
                }
            }
            catch
            {
                return null;
            }
        }

        ///<summary>
        /// 比较文件1和文件2，看文件2是否为最新的
        /// </summary>
        /// <param name="SameIfWithinXMinutes">设置时间搓，指示如果两个文件创建时间在这个时间搓内，视为相同true</param>
        public static bool CheckFile2Newest(string File1, string File2, float SameIfWithinXMinutes = 2)
        {
            if (System.IO.File.Exists(File1) && System.IO.File.Exists(File2))
            {
                DateTime f1_d = System.IO.File.GetLastWriteTime(File1);
                DateTime f2_d = System.IO.File.GetLastWriteTime(File2);

                if (f1_d.Equals(f2_d)) //创建时间相同
                {
                    return true;
                }
                if (Math.Abs(f1_d.Subtract(f2_d).TotalMinutes) < SameIfWithinXMinutes) //创建时间在允许范围内
                {
                    return true;
                }

                //文件2是最新的。
                if (f1_d.Subtract(f2_d).TotalMinutes < 0)
                {
                    return true;
                }

                return false;
            }
            else
            {
               //有文件不存在，无法比较
                return false;
            }
        }
    }
}
