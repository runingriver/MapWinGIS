using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MapWinGIS.Utility
{
    public class FileOperator
    {
        /// <summary>
        /// 检测指定的文件或目录是否存在
        /// 修改FileExits为FileOrDirExists
        /// </summary>
        /// <param name="pathName">含文件名的完整路径或目录路径</param>
        /// <param name="acceptDirectories">指示接收的是目录类型</param>
        /// <param name="acceptFiles">指示接收的是文件类型</param>
        public static bool FileOrDirExists(string pathName, bool acceptDirectories = false, bool acceptFiles = true)
        {
            try
            {
                if (pathName != null)
                {
                    if (acceptFiles && File.Exists(pathName))
                    {
                        return true;
                    }
                    else if (acceptDirectories && Directory.Exists(pathName))
                    {
                        return true;
                    }
                }
            }
            catch (Exception) //如果出现异常，则说明文件或目录不存在
            {
            }
            return false;
        }

        /// <summary>
        /// 创建一个指定的目录
        /// </summary>
        /// <param name="newDirectory">要创建的目录</param>
        public static void MkDirPath(string newDirectory)
        {
            if (newDirectory != null && newDirectory.Length > 0 && FileOrDirExists(newDirectory, true, false))
            {
                Directory.CreateDirectory(newDirectory);
            }
        }

        /// <summary>
        /// 返回指定路径的字符串信息
        /// 除去了完整路径中的文件名和扩展名
        /// </summary>
        /// <param name="filename">包含路劲和文件名的字符串</param>
        public static string PathNameOnly(string filename)
        {
            try
            {
                return Path.GetDirectoryName(filename);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 返回没有扩展名的路径
        /// Example: FilenameNoExt ("C:\foo\bar.txt") = "C:\foo\bar"
        /// </summary>
        public static string FilenameNoExt(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
        }

        /// <summary>
        /// 比较两个文件内容是否相同
        /// </summary>
        public static bool FilesMatch(string filename1, string filename2)
        {
            long fileLength = new FileInfo(filename1).Length;
            //如果两个文件大小不一样，就不需要比较了
            if (new FileInfo(filename2).Length != fileLength)
            {
                return false;
            }

            BinaryReader binaryReader1 = null;
            BinaryReader binaryReader2 = null;
            try
            {
                binaryReader1 = new BinaryReader(new FileStream(filename1, FileMode.Open, FileAccess.Read));
                binaryReader2 = new BinaryReader(new FileStream(filename2, FileMode.Open, FileAccess.Read));
                for (long i = 1; i <= fileLength; i++)
                {
                    if (binaryReader1.ReadByte() != binaryReader2.ReadByte())
                    {
                        return false;
                    }
                }
                return true; //比较到末尾
            }
            finally
            {
                if (binaryReader1 != null)
                {
                    binaryReader1.Close();
                }
                if (binaryReader2 != null)
                {
                    binaryReader2.Close();
                }
            }
        }

        /// <summary>
        /// 比较两个文件内容是文本的文件是否相同
        /// 可以指定是否忽略大小写
        /// </summary>
        public static bool FilesMatchText(string filename1, string filename2, bool ignoreCase = false)
        {
            long lFileLength = new FileInfo(filename1).Length;

            //文件大小不等，文件肯定不同
            if (new FileInfo(filename2).Length != lFileLength)
            {
                return false;
            }

            StreamReader streamReader1 = new StreamReader(filename1);
            StreamReader streamReader2 = new StreamReader(filename2);
            string line1 = streamReader1.ReadLine();
            string line2 = streamReader2.ReadLine();
            bool isMatch = true;
            while (isMatch == true || line1 != null)
            {
                if (line1 != line2)
                {
                    if (ignoreCase)
                    {
                        if (line1.ToLower() != line2.ToLower())
                        {
                            isMatch = false;
                        }
                    }
                    else
                    {
                        isMatch = false;
                    }
                }
                line1 = streamReader1.ReadLine();
                line2 = streamReader2.ReadLine();
            }
            streamReader1.Close();
            streamReader2.Close();
            return isMatch;
        }

        /// <summary>
        /// 直接删除指定的目录或文件，不记录到日志文件中
        /// </summary>
        public static bool TryDelete(string path)
        {
            return TryDelete(path, false);
        }

        /// <summary>
        /// 尝试删除指定的目录或文件
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="isLogging">是否记录到日志文件中</param>
        /// <returns>删除成功与否</returns>
        public static bool TryDelete(string path, bool isLogging)
        {
            bool tryDelete = false;
            try
            {
                System.GC.WaitForPendingFinalizers();
                if (FileOrDirExists(path)) //文件
                {
                    File.Delete(path);
                    if (isLogging)
                    {
                        Logger.Dbg("删除文件：" + path);
                    }
                }
                else if (FileOrDirExists(path, true, false)) //文件夹
                {
                    Directory.Delete(path, true);
                    if (isLogging)
                    {
                        Logger.Dbg("指定路径文件或文件夹没有找到，无需删除：" + path);
                    }
                }
                else //没找到
                {
                    if (isLogging)
                    {
                        Logger.Dbg("删除文件夹及其下面的所有文件：" + path + "当前目录:" + Environment.CurrentDirectory);
                    }
                }
                tryDelete = true;
            }
            catch(Exception ex)
            {
                if (isLogging)
                {
                    Logger.Dbg("删除文件或文件夹异常:" + path + ":" + ex.ToString());
                }
            }
            return tryDelete;
        }

        /// <summary>
        /// 将一个文件复制到另一个路径下
        /// 如果复制失败不会抛出异常，也不记录在日志文件中
        /// </summary>
        public bool TryCopy(string formPath, string toPath)
        {
            return TryCopy(formPath, toPath, false);
        }

        /// <summary>
        /// 将一个文件复制到另一个路径下
        /// 如果复制失败不会抛出异常，但会记录在日志文件中
        /// </summary>
        public bool TryCopy(string fromPath, string toPath, bool isLogging)
        {
            try
            {
                if (File.Exists(fromPath))
                {
                    if (File.Exists(toPath)) //若目标路径也存在该文件，删除
                    {
                        TryDelete(toPath);
                    }
                    string directory = Path.GetDirectoryName(toPath);
                    if (directory.Length > 3 && !Directory.Exists(directory))//若不存在该目录，创建一个
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.Copy(fromPath, toPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (isLogging)
                {
                    Logger.Dbg("复制文件异常，从" + fromPath + "到" + toPath + ":" + ex.ToString());
                }
            }
            return false;
        }
    }
}
