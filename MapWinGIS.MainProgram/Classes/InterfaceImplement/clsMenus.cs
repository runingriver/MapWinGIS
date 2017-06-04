/****************************************************************************
 * 文件名:clsMenus.cs (F)
 * 描  述: 将菜单项添加到菜单栏显示，插件也通过该类添加按钮到菜单
 * **************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public class Menus : MapWinGIS.Interfaces.Menus
    {
        /// <summary>
        /// 存储菜单中包含的所有对象，包括由系统内部添加和插件添加的顶级菜单项及其子菜单项.
        /// Key：菜单名 
        /// Value：System.Windows.Forms.ToolStripItem对象
        /// </summary>
        internal Hashtable m_MenuTable;
        /// <summary>
        /// 存储插件菜单对象,即显示在"插件"菜单下面的子项，显示系统有哪些插件.
        /// Key：最上级菜单名 
        /// value：Arraylist对象，插件的key（一个菜单下可能有多个插件）
        /// </summary>
        internal Hashtable m_PluginAddedMenus = new Hashtable();

        public Menus()
        {
            m_MenuTable = new Hashtable();
        }
        ~Menus()
        {
            m_MenuTable = null;
        }

        #region ************************接口实现*************************
        /// <summary>
        /// 用指定的名字，添加一个菜单
        /// </summary>
        /// <param name="name">指定菜单的名字</param>
        /// <returns>菜单对象</returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name)
        {
            return AddMenu(name, "", null, name, "");
        }

        /// <summary>
        /// 用指定的名字，图片，添加一个菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, object picture)
        { 
            return AddMenu(name, "", picture, name, "");
        }

        /// <summary>
        /// 用指定的名字，图片，文本。添加一个菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="picture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, object picture, string text)
        {
            return AddMenu(name, "", picture, text);
        }

        /// <summary>
        /// 用指定的名字添加一个菜单到父菜单上
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentMenu"></param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu)
        {
            return AddMenu(name, parentMenu, null, name , "");
        }

        /// <summary>
        /// 用指定的名字，图片，添加一个菜单到父菜单栏上
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentMenu"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu, object picture)
        {
            return AddMenu(name, parentMenu, picture, name,"");
        }

        /// <summary>
        /// 用指定的名字，图片，文本。添加一个菜单到父菜单上
        /// </summary>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu, object picture, string text)
        {
            if (picture == null && text != null)
            {
                return AddMenu(name, parentMenu, null, text, "", "");
            }
            else
            {
                return AddMenu(name, parentMenu, picture, text, "", "");
            }
        }

        /// <summary>
        /// 指定名字，图片，文本、添加一个菜单到父菜单上，并且在指定菜单项的后面添加
        /// </summary>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu, object picture, string text, string after)
        { 
            return AddMenu(name, parentMenu, picture, text, after, "");
        }

        /// <summary>
        /// 指定名字，图片，文本、添加一个菜单到父菜单上，并且在指定菜单项的前面添加
        /// </summary>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu, string text, string before)
        {
            if (text == null && before != null)
            {
                return AddMenu(name, parentMenu, null, before, "");
            }
            else
            {
                return AddMenu(name, parentMenu, null, text, "", before);
            }
        }

        /// <summary>
        /// 索引，通过名字，获取该菜单项
        /// </summary>
        public MapWinGIS.Interfaces.MenuItem this[string menuName]
        {
            get 
            {
                System.Windows.Forms.ToolStripMenuItem menu = MenuTableItem(menuName);
                if (menu == null)
                {
                    return null;
                }
                else
                {
                    return new ToolStripMenuItem(menu);
                }
            }
        }

        /// <summary>
        /// 从可用插件列表移除一个插件，若已加载，卸载
        /// 实现Menus.Remove接口
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        public bool Remove(string menuName)
        {
            System.Windows.Forms.ToolStripItem m;
            string key = MenuTableKey(menuName);
            System.Windows.Forms.MenuStrip parentMenu = null;
            System.Windows.Forms.ToolStrip parentStrip = null;
            try
            {
                if (m_MenuTable.ContainsKey(key))
                {
                    m = (System.Windows.Forms.ToolStripItem)(m_MenuTable[key]);
                    if (m.GetType().Name == "ToolStripMenuItem")//是Form.ToolStripMenuItem类型，则从m_MenuTable移除该菜单项 的子项
                    {
                        RemoveSubMenusFromMenuTable((System.Windows.Forms.ToolStripMenuItem)m);
                    }
                    m_MenuTable.Remove(key);//移除该项

                    try
                    {
                        object o = m.GetCurrentParent();//ToolStripItem的父对象ToolStrip
                        if (o is System.Windows.Forms.MenuStrip)
                        {
                            parentMenu = (System.Windows.Forms.MenuStrip)o;
                        }
                        if (o is System.Windows.Forms.ToolStrip)
                        {
                            parentStrip = (System.Windows.Forms.ToolStrip)o;
                        }
                        if (parentMenu != null)//是MenuStrip类型，从窗体中移除该菜单项
                        {
                            parentMenu.Items.Remove(m);
                        }
                        else if (parentStrip != null)//是ToolStrip类型，从窗体中移除该菜单项
                        {
                            parentStrip.Items.Remove(m);
                        }
                        else //既不是MenuStrip也不是Toolstrip，是上级菜单项，则直接移除
                        {
                            Program.frmMain.menuStrip1.Items.Remove(m);
                        }
                    }
                    catch (Exception e)
                    {
                        Program.CustomExceptionHandler.OnThreadException(e);
                        return false;
                    }

                    System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1);
                    string pluginfile = stackFrame.GetMethod().Module.FullyQualifiedName;
                    if (!pluginfile.ToLower().EndsWith("mapwingis.exe"))//不是以mapwingis.exe结尾，则表示是插件菜单
                    {
                        //获取插件Key
                        string pluginkey = "";
                        PluginInfo info = new PluginInfo();
                        if (info.Init(pluginfile, typeof(Interfaces.IPlugin).GUID) && info.Key != "")
                        {
                            pluginkey = info.Key;
                        }
                        //如果获取插件Key成功，则移除该插件
                        if (pluginkey != "")
                        {
                            MenuTrackerRemove(menuName, pluginkey);
                        }
                    }

                    // 当菜单栏所有项都移除了，则移除菜单栏（menuStrip1）
                    if (Program.frmMain.menuStrip1.Items.Count == 0)
                    {
                        //修改，menuStrip1不是在TopToolStripPanel中
                        Program.frmMain.Controls.Remove(Program.frmMain.menuStrip1);
                        //Program.frmMain.StripDocker.TopToolStripPanel.Controls.Remove(Program.frmMain.menuStrip1);
                        //如果所有的控件都移除了，就隐藏工具条（TopToolStripPanel）
                        if (Program.frmMain.StripDocker.TopToolStripPanel.Controls.Count == 0)
                        {
                            Program.frmMain.StripDocker.TopToolStripPanel.Hide();
                        }
                    }
                    return true;
                }
                    //菜单中不包含该名字的菜单，则移除失败
                    return false;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return false;
            }

        }

        /// <summary>
        /// 当鼠标点击，激活该菜单项
        /// </summary>
        public bool PerformClick(string name)
        {
            try
            {
                System.Windows.Forms.ToolStripItem menuItem = MenuTableItem(name);
                if (menuItem == null)
                {
                    return false;
                }
                menuItem.PerformClick();
                return true;
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("在clsMenu.PerformClick(" + name + ")中发生一个错误：" + ex.ToString());
            }

            return false;

        }

        /// <summary>
        /// 泛型集合，当前已经加载的菜单的名字
        /// </summary>
        /// <returns>菜单名列表</returns>
        public System.Collections.Generic.IList<string> MenuNames() //此处需要用泛型？
        {
            System.Collections.Generic.IList<string> menuNamesList = new System.Collections.Generic.List<string>();

            foreach (DictionaryEntry de in m_MenuTable)
            {
                menuNamesList.Add(de.Key.ToString());
            }
            return menuNamesList;
 
        }

        /// <summary>
        /// 移除所有已经加载的菜单
        /// </summary>
        /// <returns>True移除成功</returns>
        public bool RemoveAllMenus()
        {
            try
            {
                IList<string> allMenus = MenuNames();
                foreach (string menuName in allMenus)
                {
                    Remove(menuName);
                }
                return true;
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("在RemoveAllMenus()是发生错误：" + ex.ToString());
            }
            return false;
        }

        //13
        #endregion
        /// <summary>
        /// 真正实现的添加菜单功能
        /// </summary>
        /// <param name="name">菜单名</param>
        /// <param name="parentMenu">父菜单名</param>
        /// <param name="picture">菜单图象</param>
        /// <param name="text">显示在菜单上的文本</param>
        /// <param name="after">菜单名，新菜单添加到该菜单后面</param>
        /// <param name="before">菜单名，新菜单添加到该菜单前面</param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.MenuItem AddMenu(string name, string parentMenu, object picture, string text, string after, string before)
        {
            System.Windows.Forms.ToolStripItem newMenu;
            System.Windows.Forms.ToolStripItem afterMenu;
            System.Windows.Forms.ToolStripItem beforeMenu;

            try
            {
                if (name == "")
                {
                    Program.g_error = "没有指定菜单名！";
                    return null;
                }
                if (text == "")
                {
                    text = name;
                }

                //首先尝试从toolbar中获取menu item，即检测该menu是否已经存在
                newMenu = MenuTableItem(name);
                if (newMenu == null)
                {
                    //该Menu item不存在，故添加进去
                    Program.frmMain.menuStrip1.Visible = true;//确保菜单条可见

                    if (parentMenu == null || parentMenu.Length == 0)//没有父菜单
                    {
                        //通过after和before找到正确的插入位置
                        int mnu_count = Program.frmMain.menuStrip1.Items.Count;//已存在菜单数量
                        int insertPosition = mnu_count;//插入位置，默认最后
                        if (after.Length > 0)//如果指定了after参数
                        {
                            afterMenu = MenuTableItem(after);
                            if (afterMenu != null)//找到该指定（after）的菜单
                            {     
                                for (int i = 0; i < mnu_count; i++)
                                {
                                    if (Program.frmMain.menuStrip1.Items[i].Name == afterMenu.Name)//找到该位置
                                    {
                                        insertPosition = i + 1;
                                        break;
                                    }
                                }
                            }
                            else//没找到
                            {
                                Program.g_error = "不存在名为： " + after + " 的菜单项.";
                            }
                        }
                        else if (before.Length > 0)//如果指定了before参数
                        {
                            beforeMenu = MenuTableItem(before);
                            if (beforeMenu != null)//Menu_table中找到
                            {
                                for (int i = 0; i < mnu_count; i++)
                                {
                                    if (Program.frmMain.menuStrip1.Items[i].Name == beforeMenu.Name)
                                    {
                                        insertPosition = i;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Program.g_error = "不存在名为： " + before + " 的菜单项.";
                            }
                        }

                        newMenu = Program.frmMain.menuStrip1.Items.Add(text, (System.Drawing.Image)picture, new System.EventHandler(Program.frmMain.CustomMenu_Click));
                        newMenu.Name = name;
                        //只有通过上面的方法绑定事件，所以先移除再添加，将其插入点指定的位置
                        Program.frmMain.menuStrip1.Items.Remove(newMenu);
                        Program.frmMain.menuStrip1.Items.Insert(insertPosition, newMenu);
                    }
                    else //newMenu存在于m_MenuTable中,即存在于toolbar中
                    {
                        System.Windows.Forms.ToolStripMenuItem parent = MenuTableItem(parentMenu);
                        if (parent != null) //存在父菜单
                        {
                            //通过after和before找到正确的插入位置
                            int pdd_Count = parent.DropDownItems.Count;
                            int insertPosition = pdd_Count;
                            if (after.Length > 0)//如果指定了after参数
                            {
                                afterMenu = MenuTableItem(after);
                                if (afterMenu != null)
                                {
                                    for (int i = 0; i < pdd_Count; i++)
                                    {
                                        if (parent.DropDownItems[i].Name == afterMenu.Name)//确定插入位置
                                        {
                                            insertPosition = i + 1;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Program.g_error = "不存在名为： " + after + " 的菜单项.";
                                }
                            }
                            else if (before.Length > 0)//如果指定了before参数
                            {
                                beforeMenu = MenuTableItem(before);
                                if (beforeMenu != null)
                                {
                                    for (int i = 0; i < pdd_Count; i++)
                                    {
                                        if (parent.DropDownItems[i].Name == beforeMenu.Name)//确定插入位置
                                        {
                                            insertPosition = i;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Program.g_error = "不存在名为： " + before + " 的菜单项.";
                                }
                            }

                            newMenu = parent.DropDownItems.Add(text, (System.Drawing.Image)picture, new EventHandler(Program.frmMain.CustomMenu_Click));
                            newMenu.Name = name;
                            
                            //只有通过上面的方法绑定事件，所以先移除再添加，将其插入点指定的位置
                            parent.DropDownItems.Remove(newMenu);
                            parent.DropDownItems.Insert(insertPosition, newMenu);
                        }
                        else //parent对象不存在
                        {
                            Program.g_error = "parent菜单不存在.";
                            return null;
                        }
                    }
                    MenuTableAdd(name, newMenu);
                }

                MapWinGIS.Interfaces.MenuItem newItem;
                if (!((newMenu) is ToolStripSeparator))//不是分割线
                {
                    newItem = new ToolStripMenuItem(name, (System.Windows.Forms.ToolStripMenuItem)newMenu);
                }
                else//分割线
                {
                    newItem = new ToolStripMenuItem(name, true);
                }

                return newItem;
            }
            catch (System.Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return null;
            }

        }

        /// <summary>
        /// 插件菜单，将菜单添加到m_PluginAddedMenus中
        /// </summary>
        private void MenuTrackerAdd(string item, string plugin)
        {
            if (m_PluginAddedMenus.Contains(item))//插件菜单中存在该项
            {
                if (!((ArrayList)(m_PluginAddedMenus[item])).Contains(plugin)) //Value 存储的是PluginInfo对象，故转为ArrayList
                {
                    ((ArrayList)(m_PluginAddedMenus[item])).Add(plugin);
                }
            }
            else//不存在，添加进去
            {
                m_PluginAddedMenus.Add(item, new ArrayList());
                ((ArrayList)(m_PluginAddedMenus[item])).Add(plugin);
            }
        }

        /// <summary>
        /// 从m_PluginAddedMenus中移除该插件菜单
        /// </summary>
        /// <param name="item">插件名，也是菜单名</param>
        /// <param name="plugin">插件的Key</param>
        private void MenuTrackerRemove(string item, string plugin)
        {
            if (m_PluginAddedMenus.Contains(item))
            {
                if (((ArrayList)(m_PluginAddedMenus[item])).Contains(plugin))
                {
                    ((ArrayList)(m_PluginAddedMenus[item])).Remove(plugin);
                }

                if (((ArrayList)(m_PluginAddedMenus[item])).Count == 0)
                {
                    m_PluginAddedMenus.Remove(item);
                }
            }
        }

        /// <summary>
        /// 移除插件，StopPlugins调用
        /// </summary>
        /// <param name="plugin"></param>
        public void MenuTrackerRemoveIfLastOwner(string plugin)
        {
            ArrayList dellist = new ArrayList();//移除插件菜单列表
            IDictionaryEnumerator ienum = m_PluginAddedMenus.GetEnumerator();
            while (ienum.MoveNext())
            {
                if (((ArrayList)ienum.Value).Contains(plugin) && ((ArrayList)ienum.Value).Count == 1)
                {
                    dellist.Add(ienum.Key.ToString());
                }
            }

            for (int i = 0; i < dellist.Count; i++)
            {
                try
                {
                    Remove(dellist[i].ToString());
                }
                catch
                {
                }
                try
                {
                    m_PluginAddedMenus.Remove(dellist[i]);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 移除指定ToolStripMenuItem中包含的子菜单项
        /// </summary>
        private void RemoveSubMenusFromMenuTable(System.Windows.Forms.ToolStripMenuItem parentToolStripMenuItem)
        {
            if (parentToolStripMenuItem != null && parentToolStripMenuItem.DropDownItems != null)//存在且有子项
            {
                foreach (object subMenu in parentToolStripMenuItem.DropDownItems)
                {
                    if (subMenu.GetType().Name == "ToolStripMenuItem")
                    {
                        RemoveSubMenusFromMenuTable((System.Windows.Forms.ToolStripMenuItem)subMenu);
                    }
                    foreach (string key in m_MenuTable.Keys)//移除m_MenuTable中对应的菜单项
                    {
                        if (m_MenuTable[key].Equals(subMenu))
                        {
                            m_MenuTable.Remove(key);
                            break;
                        }
                    }
                }
            }
        }

        internal bool Contains(object key)
        {
            return m_MenuTable.ContainsKey(MenuTableKey((string)key));
        }

        /// <summary>
        /// 检索m_MenuTable中的子项，并返回ToolStripMenuItem对象
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem MenuTableItem(string name)
        {
            //从m_MenuTable中检索ToolStripMenuItem
            string key = MenuTableKey(name);//格式化菜单名
            if (m_MenuTable.ContainsKey(key))
            {
                if (!((m_MenuTable[key]) is System.Windows.Forms.ToolStripSeparator))
                {
                    //不是分割线
                    return ((System.Windows.Forms.ToolStripMenuItem)(m_MenuTable[key]));
                }
                else
                {
                    //是分割线，则添加一个分割线
                    System.Windows.Forms.ToolStripMenuItem newItem = new System.Windows.Forms.ToolStripMenuItem("-");
                    newItem.Name = name;
                    return newItem;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 向m_MenuTable中添加指定菜单项
        /// </summary>
        /// <param name="Name">菜单名（key）</param>
        /// <param name="NewMenu">菜单对象（value）</param>
        private void MenuTableAdd(string name, System.Windows.Forms.ToolStripItem newMenu)
        {
            StackFrame stackFrame = new StackFrame(1); //???
            string pluginkey = FindPluginKey();
            if (pluginkey != "")
            {
                MenuTrackerAdd(name, pluginkey);
            }
            //添加到m_MenuTable中
            m_MenuTable.Add(MenuTableKey(name), newMenu);
        }

        /// <summary>
        /// 搜索插件的Key
        /// </summary>
        /// <returns></returns>
        private string FindPluginKey()
        {
            try
            {
                int i = 1;
                while (true)
                {
                    StackFrame sf = new StackFrame(i);
                    if (sf == null)
                    {
                        return "";
                    }
                    if (sf.GetMethod() == null)
                    {
                        return "";
                    }
                    if (sf.GetMethod().Module == null)
                    {
                        return "";
                    }
                    if (sf.GetMethod().Module.FullyQualifiedName.ToLower().Contains("plugins"))
                    {
                        PluginInfo info = new PluginInfo();
                        if (info.Init(sf.GetMethod().Module.FullyQualifiedName, typeof(Interfaces.IPlugin).GUID) && info.Key != "")
                        {
                            return info.Key;
                        }
                        //兼容最新接口
                        //else if (info.Init(sf.GetMethod().Module.FullyQualifiedName, typeof(MapWinGIS.PluginInterfaces.IBasePlugin).GUID) && info.Key != "")
                        //{
                        //    return info.Key;
                        //}
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return "";
        }

        /// <summary>
        /// 确保帮助菜单保持在最后面
        /// </summary>
        public void EnsureHelpItemLast()
        {
            System.Resources.ResourceManager resources =
                new System.Resources.ResourceManager("MapWinGIS.MainProgram.GlobalResource", System.Reflection.Assembly.GetExecutingAssembly());
            int i;
            int count= Program.frmMain.menuStrip1.Items.Count;
            for (i = 0; i < count; i++)
            {
                if (Program.frmMain.menuStrip1.Items[i].Text == resources.GetString("mnuHelp_Text"))
                {
                    System.Windows.Forms.ToolStripItem helpItem = Program.frmMain.menuStrip1.Items[i];
                    //移除再添加，系统会自动将帮助菜单添加到最后
                    Program.frmMain.menuStrip1.Items.RemoveAt(i);
                    Program.frmMain.menuStrip1.Items.Add(helpItem);
                }
            }
        }

        /// <summary>
        /// 删除字符串中的地址符和空格字符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string MenuTableKey(string name)
        {
            return name.Replace("&", "").Replace(" ", "");
        }
        //24
    }
}
