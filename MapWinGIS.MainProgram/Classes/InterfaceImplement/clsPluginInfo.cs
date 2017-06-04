/**************************************************************************************
 * 文件名:clsPluginInfo.cs （F）
 * 描  述:此类包含插件所必须的信息，并将插件加载到内存中
 * 提 示：插件是一个一个的加载到内存中的，通过Init及其关联方法将插件加载到内存中。
 *        遍历组件对象的类型信息，从而获取该插件的相关信息。这些信息都将存储在PluginManager
 *        中对应的容器中。
 * *************************************************************************************/

using System;
using MapWinGIS.Interfaces;

namespace MapWinGIS.MainProgram
{
    public class PluginInfo : MapWinGIS.Interfaces.PluginInfo
    {
        //成员变量
        /// <summary>
        /// 组件对象的GUID，即插件的GUID
        /// </summary>
        private string m_CoClassGUID; 
        /// <summary>
        /// 组件对象名，即实现IPlugin接口的接口名
        /// </summary>
        private string m_CoClassName;
        /// <summary>
        /// 类型的完全限定名
        /// </summary>
        private string m_CreateString;
        private string m_Filename;
        private string m_Name;
        private string m_Description;
        private string m_Version;
        private string m_Author;
        private string m_BuildDate;
        /// <summary>
        /// 根据插件接口名得到的key
        /// eg：MapWinGIS_Interface_A
        /// </summary>
        private string m_Key;
        /// <summary>
        /// 是否初始化了插件
        /// </summary>
        private bool m_Initialized;
        /// <summary>
        /// 插件接口的GUID
        /// </summary>
        private string IPluginGUID;

        public PluginInfo()
        {
            m_Initialized = false;
            IPluginGUID = ("{" + typeof(MapWinGIS.Interfaces.IPlugin).GUID.ToString() + "}").ToUpper();
        }

        /// <summary>
        /// 对应版本的插件初始化是否成功
        /// </summary>
        /// <param name="path">包含完整路径的插件（dll）文件名</param>
        /// <param name="searchingGUID">IPlugin的GUID</param>
        /// <returns>true- ,false -</returns>
        internal bool Init(string path, Guid searchingGUID)
        {
            bool bRes;
            try
            {
                path = path.Replace("\"", ""); //去掉路径中的双引号
                if (!System.IO.File.Exists(path))
                {
                    return false; //确保目录存在
                }
                bRes = AddFromFileDotNetAssembly(path, searchingGUID);
                m_Initialized = bRes;
            }
            catch (System.Exception ex)
            {
                bRes = false;
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
            return bRes;
        }

        /// <summary>
        /// 从给定的目录文件名，加载.NET程序集，解析插件中的接口类型
        /// </summary>
        /// <param name="pathToFile">包含完整路径的插件（dll）文件名</param>
        /// <param name="searchingGUID">IPlugin的GUID</param>
        /// <returns></returns>
        private bool AddFromFileDotNetAssembly(string pathToFile, Guid searchingGUID)
        {
            bool CanLoad = false; //指示加载是否能够加载该程序集
            System.Reflection.Assembly asm;
            Type[] CoClassList; //在程序集中(items的)类型列表,组件对象列表
            Type CoClass; //当前程序集item的类型信息 ，组件对象
            Type Infc; //当前接口类型
            Type[] InfcList; //当前程序集实现的接口列表

            try
            {
                //加载程序集
                asm = System.Reflection.Assembly.LoadFrom(pathToFile.Trim());
                try
                {
                    CoClassList = asm.GetTypes();
                }
                catch (System.Reflection.ReflectionTypeLoadException rtlex)
                {
                    Program.g_error = rtlex.Message;
                    return false;

                }
                catch (System.Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return false;
                }

                foreach (Type tempLoopVar_CoClass in CoClassList)//各种类型列表
                {
                    CoClass = tempLoopVar_CoClass;
                    InfcList = CoClass.GetInterfaces();
                    foreach (Type tempLoopVar_Infc in InfcList)//接口类型列表
                    {
                        Infc = tempLoopVar_Infc;
                        if (Infc.GUID == searchingGUID) //比较插件继承的接口的GUID和此程序中的Interfaces的GUID是否相等
                        {
                            if (!CanLoad)
                            {
                                CanLoad = InitAssembly(CoClass, pathToFile);

                                if (CanLoad) { return true; }
                            }
                        }
                    }
                }
            }
            catch (System.BadImageFormatException)
            {
                //不是.Net程序集，不加载
            }
            catch (System.Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
            CoClassList = null;
            asm = null;
            InfcList = null;
            return CanLoad;
        }

        /// <summary>
        /// 初始化.Net插件程序集，获取程序集的基本信息
        /// </summary>
        /// <param name="assemblyInfo">当前程序集中的某个coclass类型（这里是获取的接口类型）信息</param>
        /// <param name="pathToFile">程序集的路径（包含文件名）</param>
        /// <returns></returns>
        private bool InitAssembly(System.Type assemblyInfo, string pathToFile)
        {
            System.Reflection.Assembly asm;

            try
            {
                m_CoClassGUID = ("{" + assemblyInfo.GUID.ToString() + "}").ToUpper();//获取插件的GUID
                m_CoClassName = assemblyInfo.Name;//类型名，实现IPlugin接口的接口名
                m_Filename = pathToFile;//插件完整路径
                m_CreateString = PluginManagementTools.GetCreateString(assemblyInfo); //程序集的全名eg：GIStools.GISTool
                m_Key = PluginManagementTools.GenerateKey(assemblyInfo); //key eg：GIStools_GISTool

                asm = System.Reflection.Assembly.GetAssembly(assemblyInfo);//获取该程序集

                try
                {
                    //abstract类不加载
                    if (asm.GetType(m_CreateString).Attributes == System.Reflection.TypeAttributes.Abstract)
                    {
                        return false;
                    }
                }
                catch(Exception e)
                {
                    MapWinGIS.Utility.Logger.Dbg("检查类型是否为抽象类型是出错: "+ asm.GetType(m_CreateString).Attributes.ToString() + e.ToString());
                }

                try
                {
                    object o = asm.CreateInstance(m_CreateString);
                    if (o is IPlugin)
                    {
                        IPlugin plugin = (IPlugin)o;

                        m_Author = plugin.Author;
                        m_BuildDate = plugin.BuildDate;
                        m_Description = plugin.Description;
                        m_Name = plugin.Name;
                        if (m_Name == null || m_Name.Length == 0)
                        {
                            return false;
                        }
                        m_Version = plugin.Version;

                        plugin = null;
                        asm = null;
                        o = null;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    MapWinGIS.Utility.Logger.Message("插件 \'" + pathToFile + ("\' 加载失败！" + "\r\n" + "原因: " + ex.ToString()));

                    MapWinGIS.Utility.Logger.Dbg(ex.ToString());
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                Program.g_error = ex.ToString();
                MapWinGIS.Utility.Logger.Dbg(ex.ToString());
                return false;
            }

            return true;
        }


        #region **********************PluginInfo接口实现***************************
        /// <summary>
        /// 作者信息
        /// </summary>
        public string Author { get { return this.m_Author; } }

        /// <summary>
        /// 制作时间
        /// </summary>
        public string BuildDate { get { return this.m_BuildDate; } }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get { return this.m_Description; } }

        /// <summary>
        /// 插件名字
        /// </summary>
        public string Name { get { return this.m_Name; } }

        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version { get { return this.m_Version; } }

        /// <summary>
        /// 如果插件设置了GUID，就获取
        /// </summary>
        public string GUID { get { return this.m_CoClassGUID; } }

        /// <summary>
        /// Key识别插件，区别于其他其他可用插件的标识
        /// </summary>
        public string Key { get { return this.m_Key; } }

        #endregion

        /// <summary>
        /// 获取插件包含文件名的完整路径名
        /// </summary>
        internal string FileName { get { return this.m_Filename; } }
        
        /// <summary>
        /// 程序集中某类型的Name
        /// </summary>
        internal string CoClassName { get { return this.m_CoClassName; } }
        
        /// <summary>
        /// 程序集的全名
        /// </summary>
        internal string CreateString { get { return this.m_CreateString; } }
    }
}
