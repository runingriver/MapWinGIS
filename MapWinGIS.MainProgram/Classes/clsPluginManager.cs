/****************************************************************************
 * 文件名:clsPluginManager.cs (F)
 * 描  述:PluginTracker类用于解析插件，此处只实现了旧接口Plugins，若需要可以实现
 *        新接口PluginInterfaces.PluginTracker。但是做兼容处理  41
 * 实现思路：程序将插件分为内部插件（ApplicationPlugins）和外部插件(Plugins)，内
 *          部插件程序运行即加载。外部插件程序运行时只将插件对象提取并不实例化，
 *          并由用户动态决定是否加载。所有的内部插件存储在m_ApplicationPlugins中。
 *          所有的内部插件存储在m_PluginList中，将已经加载到程序中的插件存储在
 *          m_LoadedPlugins中以便管理
 * 插件工作过程：                
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MapWinGIS.Interfaces;
using Microsoft.VisualBasic;

namespace MapWinGIS.MainProgram
{
    public class PluginTracker : IEnumerable, MapWinGIS.Interfaces.Plugins
    {
        #region ****************实现枚举功能************************
        /// <summary>
        /// 实现该插件类的枚举器
        /// 对非泛型集合的简单迭代
        /// </summary>
        public class PluginEnumerator : IEnumerator
        {
            private MapWinGIS.Interfaces.Plugins m_Collection;
            private int m_Idx = -1;

            public PluginEnumerator(MapWinGIS.Interfaces.Plugins inp)
            {
                m_Collection = inp;
                m_Idx = -1;
            }

            public void Reset()
            {
                m_Idx = -1;
            }

            public object Current
            {
                get
                {
                    //引用接口中的this 索引器
                    return m_Collection[m_Idx];
                }
            }

            public bool MoveNext()
            {
                m_Idx++;
                if (m_Idx >= m_Collection.Count)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 实现IEnumerable接口
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new PluginEnumerator(this);
        }
        #endregion

        /// <summary>
        /// 存储外部插件，Key：插件的Key，Value：PluginInfo对象
        /// </summary>
        public Dictionary<string,PluginInfo> m_PluginList;        

        /// <summary>
        /// 存储内部插件，Key：插件的Key，Value：IPlugin对象
        /// </summary>
        public Dictionary<string, IPlugin> m_ApplicationPlugins;

        /// <summary>
        /// 存储已经加载到程序的插件对象
        /// key - info.key, value - IPlugin对象
        /// </summary>
        public Dictionary<string, IPlugin> m_LoadedPlugins;

        private string m_PluginFolder;
        internal PluginsForm m_dlg;
        private bool m_Locked = false;

        //构造函数
        public PluginTracker()
        {
            m_PluginList = new Dictionary<string, PluginInfo>();
            m_PluginList.Clear();
            m_LoadedPlugins = new Dictionary<string,IPlugin>();
            m_ApplicationPlugins = new Dictionary<string, IPlugin>();
            m_dlg = new PluginsForm();
        }
        //析构函数
        ~PluginTracker()
        {
            m_dlg = null;
            m_PluginList.Clear();
            m_PluginList = null;
            m_LoadedPlugins = null;
            m_ApplicationPlugins = null;
        }

        #region *********************Plugins接口实现*************************

        /// <summary>
        /// 实现Interfaces.Plugins.Clear
        /// </summary>
        public void Clear()
        {
            m_PluginList.Clear();
        }

        /// <summary>
        /// 从指定包含文件名的路径添加一个插件到m_Pluginlist中
        /// 实现Plugins.AddFromFile接口
        /// </summary>
        /// <param name="path">包含文件名的路径</param>
        /// <returns></returns>
        public bool AddFromFile(string path)
        {
            if (m_Locked) { return false; }

            PluginInfo info = new PluginInfo();
            bool retval; //指示插件是否加载成功
            try
            {
                retval = info.Init(path, typeof(MapWinGIS.Interfaces.IPlugin).GUID);
                if (retval == true) //插件加载成功
                {
                    if (m_PluginList.ContainsKey(info.Key) == false) //没有相同插件
                    {
                        m_PluginList.Add(info.Key, info);
                        Program.frmMain.SynchPluginMenu();
                    }
                    else //有相同的插件
                    {
                        string dupfile = (m_PluginList[info.Key]).FileName;//插件路径
                        if (dupfile.ToLower() != path.ToLower())
                        {
                            MapWinGIS.Utility.Logger.Msg("警告: 检测到插件重复."
                                    + "\r\n" + "\r\n" + "加载的插件是: " + dupfile + "\r\n"
                                    + "重复的插件被忽略: "
                                    + path, "插件重复", System.Windows.Forms.MessageBoxIcon.Information);
                        }
                    }
                }
                return retval;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return false;
            }
 
        }

        /// <summary>
        /// 从指定目录添加目录中的插件到m_Pluginlist中
        /// 实现Plugins.AddFromDir接口
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns></returns>
        public bool AddFromDir(string path)
        {
            if (m_Locked) { return false; }
            int count = 0;
            ArrayList ar = PluginManagementTools.FindPluginDLLs(path);
            foreach (string s in ar)
            {
                if (AddFromFile(s))
                    count++;
            }
            if (count > 0 && count <= ar.Count)
                return true;
            else
                return false;
 
        }

        /// <summary>
        /// 是否存在指定键的插件。若存在于m_Pluginlist中，则添加到m_LoadPlugins中
        /// 实现Plugins.StartPlugin接口
        /// </summary>
        /// <param name="key">插件的Key</param>
        /// <returns></returns>
        public bool StartPlugin(string key)
        {
            if (m_Locked) { return false; }

            MapWinGIS.Interfaces.IPlugin plugin ;
            PluginInfo info ;

            if (m_LoadedPlugins.ContainsKey(key))//在m_LoadedPlugins中搜索,有 - 已经加载不再加载
            {
                return true;
            }

            if (m_PluginList.ContainsKey(key)) //在m_PluginList中搜索，有 - 创建实例，添加到m_LoadedPlugins中
            {
                try
                {
                    info = m_PluginList[key];
                    plugin = (IPlugin)(PluginManagementTools.CreatePluginObject(info.FileName, info.CreateString));
                    if (plugin == null)
                    {
                        return false;
                    }

                    m_LoadedPlugins.Add(info.Key, plugin);
                    plugin.Initialize(Program.frmMain, Program.frmMain.Handle.ToInt32());//调用插件中的方法进行初始化
                    Program.frmMain.SynchPluginMenu();
                    return true;
                }
                catch (Exception ex)
                {
                    Program.g_error = "插件出现错误:  " + ex.ToString();
                    Program.ShowError(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
 
        }

        /// <summary>
        /// 卸载指定（key）插件
        /// 实现Plugins.StopPlugin接口
        /// </summary>
        /// <param name="key">插件的Key</param>
        public void StopPlugin(string key)
        {
            if (m_Locked) { return; }

            try
            {
                if (m_LoadedPlugins.ContainsKey(key))
                {
                    IPlugin plugin;
                    if (m_LoadedPlugins[key] != null)
                    {
                        plugin = m_LoadedPlugins[key];
                        if (plugin != null)
                        {
                            try
                            {
                                plugin.Terminate();
                            }
                            catch (Exception ex)
                            {
                                MapWinGIS.Utility.Logger.Msg("警告: 插件 \'" + key + "\'  Terminate()方法发生错误." + "\r\n", "插件错误警告", System.Windows.Forms.MessageBoxIcon.Exclamation);
                                MapWinGIS.Utility.Logger.Dbg(key + " 插件异常: " + ex.ToString());
                            }
                            plugin = null;
                        }
                    }
                    m_LoadedPlugins.Remove(key);
                    Program.frmMain.SynchPluginMenu();
                }

                Program.frmMain.m_Menu.MenuTrackerRemoveIfLastOwner(key);
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }

        /// <summary>
        /// 可用插件的数量（m_PluginList）
        /// </summary>
        public int Count
        {
            get { return this.m_PluginList.Count; }
        }

        /// <summary>
        /// 索引，从加载插件列表中,获取一个IPlugin对象
        /// <param name="index">0-based index into the list of plugins</param>
        /// </summary>
        public Interfaces.IPlugin this[int index]
        {
            get
            {
                List<IPlugin> loadPlugins = new List<IPlugin>();
                int i = 0;
                foreach (KeyValuePair<string, IPlugin> p_Item in m_LoadedPlugins)
                {
                    loadPlugins.Add(p_Item.Value);
                    if (i == index) { break; }
                    i++;
                }

                if (index <= m_LoadedPlugins.Count && index >= 0)
                {
                    return ((MapWinGIS.Interfaces.IPlugin)(loadPlugins[index]));
                }

                return null;
            }
        }

        /// <summary>
        /// 通过插件的Key获得插件对象
        /// </summary>
        public Interfaces.IPlugin this[string key]
        {
            get
            {
                if (m_LoadedPlugins.ContainsKey(key))
                {
                    return m_LoadedPlugins[key];
                }
                return null;
            }
        }

        /// <summary>
        /// 从可用插件列表，移除一个插件，如果插件已经加载，卸载这个插件
        /// </summary>
        /// <param name="key">以零开始的索引或者字符串关键字</param>
        public void Remove(string key)
        {
            if (m_Locked) { return; }

            try
            {
                if (m_LoadedPlugins.ContainsKey(key))
                {
                    StopPlugin((key).ToString());
                }
                m_PluginList.Remove(key);
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
 
        }

        /// <summary>
        /// 插件目录bin/Plugins
        /// 实现Interfaces.Plugins.PluginFolder接口
        /// </summary>
        public string PluginFolder
        {
            get { return m_PluginFolder; }
            set { this.m_PluginFolder = value; }
        }

        /// <summary>
        /// 插件是否加载
        /// 在m_LoadedPlugins和m_ApplicationPlugins中是否存在指定key的插件
        /// Plugins.PluginIsLoaded接口
        /// </summary>
        /// <param name="Key">插件的唯一识别关键字key</param>
        /// <returns></returns>
        public bool PluginIsLoaded(string key)
        {
            return (m_LoadedPlugins.ContainsKey(key) || m_ApplicationPlugins.ContainsKey(key));
        }

        /// <summary>
        /// 显示插件对话框
        /// 实现Plugins.ShowPluginDialog接口
        /// </summary>
        public void ShowPluginDialog()
        {
            if (!m_Locked)
            {
                m_dlg.ShowDialog();
            }
        }

        /// <summary>
        /// 根据插件的名称，返回插件的关键字key
        /// </summary>
        /// <param name="pluginName">显示在菜单上的插件名</param>
        /// <returns>返回空字符串代表该插件没有找到</returns>
        public string GetPluginKey(string pluginName)
        {
            foreach (PluginInfo info in m_PluginList.Values)
            {
                if (info.Name == pluginName)
                {
                    return info.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// 以广播的方式发送消息给所有已经加载的插件
        /// </summary>
        /// <param name="message">要发送的消息</param>
        public void BroadcastMessage(string message)
        {
            this.Message(message);
        }

        /// <summary>
        /// 从实例对象中加载一个插件
        /// </summary>
        /// <param name="plugin">要加载的插件对象</param>
        /// <param name="pluginKey">插件唯一标识Key</param>
        /// <returns>true on success, false otherwise</returns>
        public bool LoadFromObject(MapWinGIS.Interfaces.IPlugin plugin, string pluginKey)
        {
            try
            {
                if (plugin == null)
                {
                    Program.g_error = "LoadFromObject failed.  \'Plugin\' 参数没有设置 ";
                    return false;
                }
                else
                {
                    if (m_LoadedPlugins.ContainsKey(pluginKey))
                    {
                        Program.g_error = "Key 已经存在!  不能加载具有相同key的插件.";
                        return false;
                    }
                    else
                    {
                        plugin.Initialize(Program.frmMain, Program.frmMain.Handle.ToInt32());
                        plugin.ProjectLoading(Program.projInfo.ProjectFileName, "");
                        m_LoadedPlugins.Add(pluginKey, plugin);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 从一个实例对象中加载一个插件
        /// </summary>
        /// <param name="plugin">要加载的插件对象</param>
        /// <param name="pluginKey">插件唯一标识Key</param>
        /// <param name="settingsString">当插件加载后传给插件的设置</param>
        public bool LoadFromObject(MapWinGIS.Interfaces.IPlugin plugin, string pluginKey, string settingsString)
        {
            try
            {
                if (plugin == null)
                {
                    Program.g_error = "LoadFromObject failed.  \'Plugin\' 参数没有设置 ";
                    return false;
                }
                else
                {
                    if (m_LoadedPlugins.ContainsKey(pluginKey))
                    {
                        Program.g_error = "Key 已经存在!  不能加载具有相同key的插件.";
                        return false;
                    }
                    else
                    {
                        plugin.Initialize(Program.frmMain, Program.frmMain.Handle.ToInt32());
                        plugin.ProjectLoading(Program.projInfo.ProjectFileName, settingsString);
                        m_LoadedPlugins.Add(pluginKey, plugin);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return false;
            }
        }

        //15
        #endregion

        #region *********************事件处理********************************
        /// <summary>
        /// 在插件中响应legend上的双击事件
        /// </summary>
        internal bool LegendDoubleClick(int handle, MapWinGIS.Interfaces.ClickLocation location)
        {
            bool handled = false;
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.LegendDoubleClick(handle, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LegendDoubleClick(handle, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;

        }

        /// <summary>
        /// 在插件中响应LegendMouseDown事件
        /// </summary>
        internal bool LegendMouseDown(int handle, int button, MapWinGIS.Interfaces.ClickLocation location)
        {
            bool handled = false;
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.LegendMouseDown(handle, button, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LegendMouseDown(handle, button, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }
        
        /// <summary>
        /// 在插件中响应LegendMouseUp事件
        /// </summary>
        internal bool LegendMouseUp(int handle, int button, MapWinGIS.Interfaces.ClickLocation location)
        {
            bool handled = false;
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.LegendMouseUp(handle, button, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LegendMouseUp(handle, button, location, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 在插件中响应MapExtentsChanged事件
        /// </summary>
        internal void MapExtentsChanged()
        {
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.MapExtentsChanged();
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.MapExtentsChanged();
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 在插件中响应MapMouseDown事件
        /// </summary>
        internal bool MapMouseDown(int button, int shift, int x, int y)
        {
            bool handled = false;
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.MapMouseDown(button, shift, x, y, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.MapMouseDown(button, shift, x, y, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>
        ///  在插件中响应MapMouseMove事件
        /// </summary>
        internal bool MapMouseMove(int screenX, int screenY)
        {
            bool handled = false;
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.MapMouseMove(screenX, screenY, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.MapMouseMove(screenX, screenY, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>
        ///  在插件中响应MapMouseUp事件
        /// </summary>
        internal bool MapMouseUp(int button, int shift, int x, int y)
        {
            bool handled = false;
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.MapMouseUp(button, shift, x, y, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.MapMouseUp(button, shift, x, y, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>
        ///  在插件中响应MapDragFinished事件
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal bool MapDragFinished(System.Drawing.Rectangle bounds)
        {
            bool handled = false;
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.MapDragFinished(bounds, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.MapDragFinished(bounds, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        #endregion
        
        #region *********************Layer事件*******************************

        /// <summary>
        /// 在插件中响应LayersAdded事件
        /// </summary>
        /// <param name="handle"></param>
        internal void LayersAdded(MapWinGIS.Interfaces.Layer[] handle)
        {
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.LayersAdded(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LayersAdded(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 在插件中响应LayerRemoved事件
        /// </summary>
        /// <param name="handle"></param>
        internal void LayerRemoved(int handle)
        {
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.LayerRemoved(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LayerRemoved(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 在插件中响应LayerSelected事件
        /// </summary>
        /// <param name="handle"></param>
        internal void LayerSelected(int handle)
        {
            if (handle == -1)
            {
                return;
            }
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.LayerSelected(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LayerSelected(handle);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

        }

        /// <summary>
        /// 在插件中响应LayersCleared事件
        /// </summary>
        internal void LayersCleared()
        {
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.LayersCleared();
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.LayersCleared();
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 让插件响应ShapesSelected事件
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="selectInfo"></param>
        internal void ShapesSelected(int handle, MapWinGIS.MainProgram.SelectInfo selectInfo)
        {
            Program.frmMain.UpdateButtons();
            foreach (IPlugin aplugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    aplugin.ShapesSelected(handle, (MapWinGIS.Interfaces.SelectInfo)selectInfo);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.ShapesSelected(handle, (MapWinGIS.Interfaces.SelectInfo)selectInfo);
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 处理插件按钮或菜单项点击事件
        /// </summary>
        /// <param name="itemName">被点击对象的Name</param>
        /// <returns>true - 插件相应成功，false - 没有执行请求</returns>
        internal bool ItemClicked(string itemName)
        {
            bool handled = false;
            //在内部插件中寻找处理
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.ItemClicked(itemName, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            //从外部插件中寻找处理
            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.ItemClicked(itemName, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 将项目加载时事件，发送给插件
        /// </summary>
        /// <param name="key">插件的Key</param>
        /// <param name="projectFile">项目目录</param>
        /// <param name="settingsString">从项目文件中提取的与插件相关的描述</param>
        internal void ProjectLoading(string key, string projectFile, string settingsString)
        {
            IPlugin tPlugin = null;
            try
            {
                if (!m_ApplicationPlugins.ContainsKey(key))
                {
                    return;
                }
                tPlugin = m_ApplicationPlugins[key];
                tPlugin.ProjectLoading(projectFile, settingsString);
                tPlugin = null;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                if (tPlugin != null)
                {
                    Exception new_ex = new Exception("在插件中出现一个错误，插件名：" + tPlugin.Name, ex);
                    Program.ShowError(ex);
                }
                else
                {
                    Program.ShowError(ex);                    
                }
            }

            try
            {
                if (!m_LoadedPlugins.ContainsKey(key))
                {
                    return;
                }
                tPlugin = m_LoadedPlugins[key];
                tPlugin.ProjectLoading(projectFile, settingsString);
                tPlugin = null;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                if (tPlugin != null)
                {
                    Exception new_ex = new Exception("在插件中出现一个错误，插件名：" + tPlugin.Name, ex);
                    Program.ShowError(ex);
                }
                else
                {
                    Program.ShowError(ex);
                }
            }

        }

        /// <summary>
        /// 将项目保存时的事件发送给插件
        /// </summary>
        internal void ProjectSaving(string key, string projectFile, string settingsString)
        {
            IPlugin tPlugin = null;
            try
            {
                if (!m_ApplicationPlugins.ContainsKey(key))
                {
                    return;
                }
                tPlugin = m_ApplicationPlugins[key];
                tPlugin.ProjectSaving(projectFile, ref settingsString);
                tPlugin = null;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }

            try
            {
                if (!m_LoadedPlugins.ContainsKey(key))
                {
                    return;
                }
                tPlugin = m_LoadedPlugins[key];
                tPlugin.ProjectSaving(projectFile, ref settingsString);
                tPlugin = null;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }

        #endregion

        /// <summary>
        /// 将宿主程序消息广播给插件
        /// </summary>
        internal bool Message(string msg)
        {
            bool handled = false;
            //在Application插件中广播消息
            foreach (IPlugin plugin in m_ApplicationPlugins.Values)
            {
                try
                {
                    plugin.Message(msg, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }

            //在外部插件中广播消息
            foreach (IPlugin plugin in m_LoadedPlugins.Values)
            {
                try
                {
                    plugin.Message(msg, ref handled);
                    if (handled)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
            return handled;
        }

        /// <summary>
        /// 从默认目录(Plugins)将插件添加到m_Pluginlist中
        /// </summary>
        /// <returns></returns>
        internal bool LoadPlugins()
        {
            if (m_Locked) { return false; }

            int count = 0;
            PluginFolder = App.Path + "\\Plugins"; //bin目录下的Plugins文件夹
            ArrayList PotentialPlugins = PluginManagementTools.FindPluginDLLs(PluginFolder); //包含完整路径的文件名
            for (int i = 0; i < PotentialPlugins.Count; i++)
            {
                PluginInfo info = new PluginInfo();
                try
                {
                    if (PotentialPlugins.IndexOf("RemoveMe-Script") > 0) //删除用RemoveMe-Script标识的插件
                    {
                        System.IO.File.Delete(PotentialPlugins[i].ToString());
                    }
                    else if (info.Init(PotentialPlugins[i].ToString(), typeof(MapWinGIS.Interfaces.IPlugin).GUID)) //存在
                    {
                        if (m_PluginList.ContainsKey(info.Key) == false)
                        {
                            m_PluginList.Add(info.Key, info);
                            count++;
                        }
                        else
                        {
                            //插件重复
                            string dupfile = m_PluginList[info.Key].FileName;
                            if (!(dupfile.ToLower() == PotentialPlugins[i].ToString().ToLower()))
                            {
                                MapWinGIS.Utility.Logger.Msg("警告: 检测到插件重复."
                                    + "\r\n" + "\r\n" + "加载的插件是: " + dupfile + "\r\n"
                                    + "重复的插件被忽略: "
                                    + PotentialPlugins[i].ToString(), "插件重复", System.Windows.Forms.MessageBoxIcon.Exclamation);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MapWinGIS.Utility.Logger.Dbg(ex.ToString());
                }
            }
            //确保有插件记录添加到m_PluginList中
            if (count > 0 && count <= PotentialPlugins.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 从ApplicationPlugins文件夹中加载内部插件
        /// </summary>
        /// <param name="applicationPluginPath"></param>
        internal void LoadApplicationPlugins(string applicationPluginPath)
        {
            if (applicationPluginPath == "" || !System.IO.Directory.Exists(applicationPluginPath))
            {
                return;
            }

            ArrayList arr = new ArrayList();
            try 
            {
                ArrayList potentialPlugins = PluginManagementTools.FindPluginDLLs(applicationPluginPath);
                PluginInfo info;
                IPlugin plugin;

                foreach (string pluginName in potentialPlugins)
                {
                    info = new PluginInfo();
                    if (info.Init(pluginName, typeof(IPlugin).GUID))
                    {
                        if (info.Key != null && !m_ApplicationPlugins.ContainsKey(info.Key))
                        {
                            plugin = (IPlugin)PluginManagementTools.CreatePluginObject(pluginName, info.CreateString);
                            if (plugin != null)
                            {
                                m_ApplicationPlugins.Add(info.Key, plugin);
                                plugin.Initialize(Program.frmMain, Program.frmMain.Handle.ToInt32());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MapWinGIS.Utility.Logger.Dbg(e.ToString());
            }
        }

        /// <summary>
        /// 卸载所有m_LoadedPlugins中的插件
        /// </summary>
        internal void UnloadAll()
        {
            try
            {
                //先搜索，再删除
                List<string> keys = new List<string>();
                foreach (string pluginKey in m_LoadedPlugins.Keys)
                {
                    keys.Add(pluginKey);
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    StopPlugin(keys[i]);
                }

                m_LoadedPlugins = new Dictionary<string, IPlugin>();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }

        /// <summary>
        /// 卸载所有m_ApplicationPlugins中的插件
        /// </summary>
        internal void UnloadApplicationPlugins()
        {
            try
            {
                foreach (IPlugin plugin in m_ApplicationPlugins.Values)
                {
                    plugin.Terminate();
                }

                m_ApplicationPlugins.Clear();
                m_ApplicationPlugins = new Dictionary<string, IPlugin>();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }

        internal Dictionary<string, IPlugin> LoadedPlugins
        {
            get { return this.m_LoadedPlugins; }
        }

        internal Dictionary<string, PluginInfo> PluginsList
        {
            get { return this.m_PluginList; }
        }

        internal bool Contains(string Key)
        {
            return m_PluginList.ContainsKey(Key);
        }

        /// <summary>
        /// 检查m_LoadedPlugins中是否包含指定(key)的插件 舍弃?
        /// 在StartPlugin中调用了此方法，m_PluginList.ContainsKey(key)
        /// </summary>
        internal bool ContainsKey(Dictionary<string, IPlugin> c, string key)
        {
            return c.ContainsKey(key);
        }

    }
}
