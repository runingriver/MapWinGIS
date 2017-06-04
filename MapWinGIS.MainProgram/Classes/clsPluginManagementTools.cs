using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace MapWinGIS.MainProgram
{
    public class PluginManagementTools
    {
        /// <summary>
        /// 搜索插件目录中的插件类型的Dll
        /// 返回包含插件名完整路径的列表
        /// </summary>
        /// <param name="pluginFolder">插件目录</param>
        /// <returns>返回所有包含插件名完整路径的列表</returns>
        public static ArrayList FindPluginDLLs(string pluginFolder)
        {
            ArrayList finalist = new ArrayList();
            try
            {
                if (System.IO.Directory.Exists(pluginFolder))//插件目录存在
                {
                    //filename是包含文件名的完整路径
                    foreach (string filename in System.IO.Directory.GetFiles(pluginFolder, "*dll", System.IO.SearchOption.AllDirectories))
                    {
                        if (!filename.Contains("Interop.") && !filename.Contains(".resources."))//不是Interop.和.resources.类型的DLL
                        {
                            finalist.Add(filename);
                        }
                    }
                }
                return finalist;
            }
            catch (Exception e)
            {
                MapWinGIS.Utility.Logger.Dbg(e.ToString());
                return finalist;
            }          
        }

        /// <summary>
        /// 创建一个插件对象
        /// </summary>
        /// <param name="fileName">插件的路径</param>
        /// <param name="createString">指定的类型字符串</param>
        /// <returns>根据查找到的指定类型，使用系统激活器创建一个给定类型（createstring）的实例</returns>
        public static object CreatePluginObject(string fileName, string createString)
        {
            try
            { 
                return System.Reflection.Assembly.LoadFrom(fileName).CreateInstance(createString);
            }
            catch (Exception e)
            {
                MapWinGIS.Utility.Logger.Dbg(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 指定类型的完全限定名
        /// eg;MapWinGIS.GisTool.ABC
        /// </summary>
        public static string GetCreateString(Type assem)
        {
            return assem.FullName;
        }

        /// <summary>
        /// 创建插件的Key
        /// eg:MapWinGIS_GisTool_ABC
        /// </summary>
        public static string GenerateKey(Type assem)
        {
            return GetCreateString(assem).Replace(".", "_");
        }

    }
}
