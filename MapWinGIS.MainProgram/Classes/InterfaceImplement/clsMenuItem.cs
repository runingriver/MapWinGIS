/**************************************************************************************
 * 文件名:clsMenuItem.cs  （F）
 * 描  述:菜单项对象，提供对菜单项进行管理的类。
 * 注  意：使用该类是要注意区分MainProgram.ToolStripMenuItem和Forms.ToolStripMenuItem。
 * ************************************************************************************/

using System;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// （F）一个菜单项对象（System.Windows.Forms.ToolStripMenuItem）
    /// </summary>
    public class ToolStripMenuItem : MapWinGIS.Interfaces.MenuItem
    {
        private System.Windows.Forms.ToolStripMenuItem m_Menu;

        private bool m_BeginsGroup;
        private System.Windows.Forms.Cursor m_Cursor;
        private string m_Category;
        private string m_Description;
        private bool m_Displayed;
        private string m_Name;
        private object m_Picture;
        private string m_Tooltip;

        private bool m_IsSeparator = false;

        //构造函数
        public ToolStripMenuItem(System.Windows.Forms.ToolStripMenuItem menu)
        {
            this.m_Menu = menu;
            this.m_Name = menu.Name;
        }
        public ToolStripMenuItem(string name, System.Windows.Forms.ToolStripMenuItem menu)
        {
            this.m_Menu = menu;
            this.m_Name = name;
        }
        public ToolStripMenuItem(string name, bool isSeparator)
        {
            this.m_Name = name;
            m_IsSeparator = true;
        }


        #region *******************接口实现*******************
        /// <summary>
        /// MenuItem的显示文本
        /// </summary>
        public string Text
        {
            get
            {
                if (m_IsSeparator)
                {
                    return "-";
                }

                return this.m_Menu.Text;
            }
            set
            {
                if (m_IsSeparator)
                {
                    return;
                }

                this.m_Menu.Text = value;
            }
        }

        /// <summary>
        /// 菜单项的icon
        /// </summary>
        public object Picture
        {
            get 
            {
                if (m_IsSeparator)
                {
                    return null;
                }
                return this.m_Picture;
            }
            set 
            { 
                if(m_IsSeparator)
                {
                    return;
                }
                this.m_Menu.Image = (System.Drawing.Image)value;
                this.m_Picture = value;
            }

        }

        /// <summary>
        /// 从item中获取设置菜单的种类
        /// </summary>
        public string Category
        {
            get { return this.m_Category; }
            set { this.m_Category = value; }
        }

        /// <summary>
        /// item的选择状态
        /// </summary>
        public bool Checked
        {
            get
            {
                if (m_IsSeparator)
                {
                    return false;
                }
                return this.m_Menu.Checked;
            }
            set 
            {
                if (m_IsSeparator)
                {
                    return;
                }
                this.m_Menu.Checked = value;
            }
        }

        /// <summary>
        /// 当鼠标over过该item显示的文本提示
        /// 鼠标over事件发生
        /// </summary>
        public string Tooltip
        {
            get { return this.m_Tooltip; }
            set { this.m_Tooltip = value; }
        }

        /// <summary>
        /// 是否在该item前面画一条分割线
        /// </summary>
        public bool BeginsGroup
        {
            get { return this.m_BeginsGroup; }
            set { this.m_BeginsGroup = value; }
        }

        /// <summary>
        /// 获取设置鼠标
        /// </summary>
        public System.Windows.Forms.Cursor Cursor
        {
            get { return this.m_Cursor; }
            set { this.m_Cursor = value; }
        }

        /// <summary>
        /// 获取设置菜单项的描述
        /// </summary>
        public string Description
        {
            get { return this.m_Description; }
            set { this.m_Description = value; }
        }

        /// <summary>
        /// 获取设置显示状态
        /// </summary>
        public bool Displayed
        {
            get { return this.m_Displayed; }
            set { this.m_Displayed = value; }
        }

        /// <summary>
        /// 获取设置菜单项的可用（enabled）的状态
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (m_IsSeparator)
                {
                    return true;
                }
                return this.m_Menu.Enabled;
            }
            set 
            {
                if (m_IsSeparator)
                {
                    return;
                }
                this.m_Menu.Enabled = value;
            }
        }

        /// <summary>
        /// 获取菜单项名字
        /// </summary>
        public string Name
        {
            get 
            {
                return this.m_Name;
            }
        }

        /// <summary>
        /// 获取设置菜单项的可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                if (m_IsSeparator)
                {
                    if (Program.frmMain.m_Menu.m_MenuTable.ContainsKey(Program.frmMain.m_Menu.MenuTableKey(m_Name)))//包含该对象
                    {
                        //是分割线？
                        if ((Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)]) is System.Windows.Forms.ToolStripSeparator)
                        {
                            return ((System.Windows.Forms.ToolStripSeparator)(Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)])).Visible;
                        }
                    }
                }
                    return this.m_Menu.Visible;
            }
            set
            {
                if (m_IsSeparator)
                {
                    if (Program.frmMain.m_Menu.m_MenuTable.ContainsKey(Program.frmMain.m_Menu.MenuTableKey(m_Name)))
                    {
                        if ((Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)]) is System.Windows.Forms.ToolStripSeparator)
                        {
                            ((System.Windows.Forms.ToolStripSeparator)(Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)])).Visible = value;
                        }
                    }
                }
                else
                {
                    this. m_Menu.Visible = value;
                }
            }
        }

        /// <summary>
        /// 获取包含在这个菜单项下面的子菜单项的数目
        /// </summary>
        public int NumSubItems
        {
            get 
            {
                if (m_IsSeparator)
                {
                    return 0;
                }
                int cnt=0;
                int mdd_Count = m_Menu.DropDownItems.Count;
                for (int i = 0; i < mdd_Count; i++)
                {
                    if (m_Menu.DropDownItems[i].Text.Trim() != "")
                    {
                        cnt++;
                    }
                }
                return cnt;
            }

        }

        /// <summary>
        /// 返回这个菜单项的子菜单项，通过以零开始的索引
        /// </summary>
        public Interfaces.MenuItem SubItem(int index)
        {
            try
            {
                if (m_IsSeparator)
                {
                    return null;
                }
                object lItem = m_Menu.DropDownItems[index];
                if (lItem.GetType().Name.Equals("ToolStripSeparator"))
                {
                    System.Windows.Forms.ToolStripMenuItem tlb_Item = (System.Windows.Forms.ToolStripMenuItem)lItem;
                    return new MapWinGIS.MainProgram.ToolStripMenuItem(tlb_Item.Name, true);
                }
                else
                {
                    return new MapWinGIS.MainProgram.ToolStripMenuItem((System.Windows.Forms.ToolStripMenuItem)lItem);
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 返回这个菜单项的子菜单项，通过子菜单的名字
        /// </summary>
        public Interfaces.MenuItem SubItem(string name)
        {
            if (m_IsSeparator)
                return null;

            int i;
            int count = m_Menu.DropDownItems.Count;
            for (i = 0; i < count; i++)
            {
                //"文件(&F)"-->“文件”,该项可去？
                if ((m_Menu.DropDownItems[i].Text.Split('('))[0].ToString() == name || m_Menu.DropDownItems[i].Name == name)
                {
                    return new MapWinGIS.MainProgram.ToolStripMenuItem((System.Windows.Forms.ToolStripMenuItem)(m_Menu.DropDownItems[i]));
                }
            }
            return null;
        }

        /// <summary>
        /// 获取，这个菜单项是否是第一个可见的子菜单项
        /// true，是第一个子菜单，false不是
        /// </summary>
        public bool IsFirstVisibleSubmenuItem
        {
            get
            {
                if (m_IsSeparator)
                { 
                    System.Windows.Forms.ToolStrip tsItemParent = ((System.Windows.Forms.ToolStripItem)(Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)])).GetCurrentParent();
                    bool conKey = Program.frmMain.m_Menu.m_MenuTable.ContainsKey(Program.frmMain.m_Menu.MenuTableKey(m_Name));
                    if (conKey && tsItemParent != null)//菜单中存在该对象，并且存在父对象
                    {
                        for (int i = 0; i < tsItemParent.Items.Count ; i++)
                        {
                            if (tsItemParent.Items[i] != null)//父对象中的子项不为空
                            {
                                //可见且不是该对象，说明存在父对象
                                if (tsItemParent.Items[i].Visible && tsItemParent.Items[i].Name != ((System.Windows.Forms.ToolStripMenuItem)Program.frmMain.m_Menu.m_MenuTable[Program.frmMain.m_Menu.MenuTableKey(m_Name)]).Name)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else //不是分割线
                {
                    if ((m_Menu.GetCurrentParent() != null))//存在父对象
                    {
                        int pcount = m_Menu.GetCurrentParent().Items.Count;
                        for (int i = 0; i < pcount; i++)
                        {
                            if (m_Menu.GetCurrentParent().Items[i] != null)//父对象的子对象不为空
                            {
                                //可见且不是该对象，说明存在父对象
                                if (m_Menu.GetCurrentParent().Items[i].Visible && (m_Menu.GetCurrentParent().Items[i].Name != m_Menu.Name))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }
        #endregion

        /// <summary>
        /// MainPtogram.ToolStripMenuItem
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }

        /// <summary>
        /// System.Windows.Forms.ToolStripMenuItem
        /// </summary>
        /// <returns></returns>
        public object ToObject()
        {
            return this.m_Menu;
        }


    }
}
