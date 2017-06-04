/****************************************************************************
 * 文件名:clsToolbar.cs (F)
 * 描  述:插件和宿主可以通过接口的方法，添加、删除、获取一个Toolbar、Button、
 *        ComboBoxItem对象。
 *            
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public class Toolbar : MapWinGIS.Interfaces.Toolbar
    {
        /// <summary>
        /// 存储全部的工具条对象,包括系统自带的4个工具条
        /// Key - name， Value - ToolStripEx/ToolStrip
        /// </summary>
        internal Hashtable m_CustomToolbars = new Hashtable();
        
        /// <summary>
        /// 存储工具条上的全部对象，包括按钮、分割线
        /// Key - name， Value - toolStripItem/ToolStripSeparator
        /// </summary>
        internal Hashtable m_Buttons;

        /// <summary>
        /// 存储以控件方式动态添加到Toolbar的Combo boxes 
        /// Key- name , Value - ToolStripComboBox
        /// </summary>
        internal Dictionary<string, ToolStripComboBox> m_ComboBoxes = new Dictionary<string, ToolStripComboBox>();


        public Toolbar()
        {
            m_Buttons = new Hashtable(107);            
        }

        ~Toolbar()
        {
            m_Buttons = null;
        }


        #region Toolbar接口实现

        /// <summary>
        /// 添加一个工具条到TopToolStripPanl中
        /// </summary>
        /// <param name="name">工具条的名字</param>
        /// <returns>是否添加成功</returns>
        public bool AddToolbar(string name)
        {
            try
            {
                //加载所有的默认工具条
                GetLastLocationOnToolStrip(Program.frmMain.StripDocker.TopToolStripPanel);

                if (m_CustomToolbars.ContainsKey(name))
                {
                    Program.g_error = "无效的工具条名，工具名不能重复";
                    return false;
                }
                else
                {
                    ToolStripExtensions.ToolStripEx tsEx = new ToolStripExtensions.ToolStripEx();
                    tsEx.Name = name;
                    tsEx.ImageScalingSize = new System.Drawing.Size(24, 24);
                    tsEx.ClickThrough = true;
                    tsEx.SuppressHighlighting = true;
                    tsEx.AllowItemReorder = true;
                    tsEx.AutoSize = true;

                    tsEx.ItemClicked +=new ToolStripItemClickedEventHandler(Program.frmMain.tlbMain_ItemClicked);
                    tsEx.ContextMenuStrip = Program.frmMain.ContextToolStrip; ;

                    //将指定名字的工具条添加到末尾，若添加不成功，则另起一行添加
                    try
                    {
                        System.Drawing.Point lastLocation = GetLastLocationOnToolStrip(Program.frmMain.StripDocker.TopToolStripPanel);
                        Program.frmMain.StripDocker.TopToolStripPanel.Join(tsEx, lastLocation);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("添加工具条异常 AddToolbar()" + ex.ToString());
                        Program.frmMain.StripDocker.TopToolStripPanel.Join(tsEx, Program.frmMain.StripDocker.TopToolStripPanel.Rows.Length - 1);
                    }

                    m_CustomToolbars.Add(name, tsEx);
                    return true;
                }

            }
            catch (Exception e)
            {
                Program.g_error = e.Message;
                Program.ShowError(e);
                return false;
            }
        }

        /// <summary>
        /// 移除工具条，以及工具条上所有的按钮的下拉列表框
        /// </summary>
        /// <param name="name">要移除的工具条的名字</param>
        /// <returns>是否成功</returns>
        public bool RemoveToolbar(string name)
        {
            if (name == "") return false;

            ToolStrip toolStrip;
            try
            { 
                toolStrip = m_CustomToolbars[name] as ToolStrip;
                if (toolStrip == null) 
                {
                    if (name == Program.frmMain.tlbMain.Name)
                    {
                        toolStrip = Program.frmMain.tlbMain;
                    }
                }
                else
                {
                    m_CustomToolbars.Remove(name);
                }


                if (toolStrip != null) //存在该Toolstrip对象，移除
                {
                    Program.frmMain.StripDocker.TopToolStripPanel.Controls.Remove(toolStrip);
                    if (Program.frmMain.StripDocker.TopToolStripPanel.Controls.Count == 0) //没有，隐藏
                    {
                        Program.frmMain.StripDocker.TopToolStripPanel.Hide();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 移除所有已经加载的工具条
        /// </summary>
        /// <returns>是否成功</returns>
        public bool RemoveAllToolbars()
        {
            try
            {
                IList<string> allToolbars = ToolbarNames();
                foreach (string toolbarName in allToolbars)
                {
                    RemoveToolbar(toolbarName);
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定新添加的工具条按钮的名字</param>
        public Interfaces.ToolbarButton AddButton(string name)
        {
            return AddButtonNewStyle(name, false, "", null);
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="isDropDown">按钮是否支持DropDown</param>
        public Interfaces.ToolbarButton AddButton(string name, bool isDropDown)
        {
            return AddButtonNewStyle(name, isDropDown, "", null);
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="toolbar">按钮所属于的工具条的名字  (if null or empty, then the default Toolbar will be used</param>
        /// <param name="isDropDown">按钮是否支持DropDown</param>
        public Interfaces.ToolbarButton AddButton(string name, string toolbarName, bool isDropDown)
        {
            return AddButtonNewStyle(name, isDropDown, toolbarName, null);
        }

        /// <summary>
        /// 在工具条中添加一个分隔符
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="toolbar">按钮所依附的工具条的名字</param>
        /// <param name="parentButton">工具条按钮的名字 ，以子条目的方式添加分割线</param>
        public void AddButtonDropDownSeparator(string name, string toolbar, string parentButton)
        {
            ToolStripSeparator newButton = new ToolStripSeparator();
            ToolStrip toolStrip;

            if (name == "")
            {
                Program.g_error = "在AddButtonDropDownSeparator方法中没有指定name参数。";
                return;
            }

            if (m_Buttons.ContainsKey(name))
            {
                Program.g_error = "给定的name参数与其他对象重名。";
                return;
            }

            try
            {
                //如果没有指定Toolbar的名字，把该直线添加到tlbMain工具条上
                if (toolbar == "")
                {
                    newButton.Tag = name;
                    newButton.Name = name;
                    if (parentButton == null || parentButton.Trim() == "") //添加到末尾
                    {
                        Program.frmMain.tlbMain.Items.Add(newButton);
                    }
                    else
                    {
                        bool added = false;
                        for (int i = 0; i < Program.frmMain.tlbMain.Items.Count; i++)
                        {
                            if (Program.frmMain.tlbMain.Items[i].Name.ToLower() == parentButton.ToLower())
                            {
                                added = true;
                                ToolStripDropDownButton dropDownButton = Program.frmMain.tlbMain.Items[i] as ToolStripDropDownButton;
                                if (dropDownButton != null)
                                {
                                    dropDownButton.DropDownItems.Add(newButton);
                                }
                            }
                        }
                        if (!added)
                        {
                            Program.frmMain.tlbMain.Items.Add(newButton);
                        }
                    }
                    m_Buttons.Add(name, newButton);
                    return;
                }
                else //添加到指定工具条上
                {
                    //检查该工具条是否存在
                    toolStrip = (ToolStrip)(m_CustomToolbars[toolbar]);
                    if (toolStrip != null)
                    {
                        newButton.Tag = name;
                        if (parentButton == null || parentButton.Trim() == "")
                        {
                            toolStrip.Items.Add(newButton);
                        }
                        else
                        {
                            bool added = false;
                            for (int i = 0; i < toolStrip.Items.Count; i++)
                            {
                                if (toolStrip.Items[i].Name.ToLower() == parentButton.ToLower())
                                {
                                    ToolStripButton tsButton = toolStrip.Items[i] as ToolStripButton;
                                    if (tsButton != null)
                                    {
                                        toolStrip.Items.Insert(i + 1, newButton);
                                        added = true;
                                        break;
                                    }
                                  
                                    ToolStripDropDownButton dropDownButton = toolStrip.Items[i] as ToolStripDropDownButton;
                                    if (dropDownButton != null)
                                    {
                                        dropDownButton.DropDownItems.Add(newButton);
                                        added = true;
                                        break;
                                    }
                                    
                                }
                            }
                            if (!added)
                            {
                                toolStrip.Items.Add(newButton);
                            }
                        }
                        m_Buttons.Add(name, newButton);
                        return;
                    }
                    else
                    {
                        Program.g_error = "指定的工具条不存在.";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return;
            }
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="picture">一图片的方式改变按钮的显示样式</param>
        public Interfaces.ToolbarButton AddButton(string name, object picture)
        {
            return AddButtonNewStyle(name, false, "", picture);
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="picture">按钮的图片样式</param>
        /// <param name="text">可以显示在toolbar上名字</param>
        public Interfaces.ToolbarButton AddButton(string name, object picture, string text)
        {
            return AddButtonNewStyle(name, false, "", picture, text);
        }

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="after">新添加的按钮的后面按钮的名字</param>
        /// <param name="parentButton">以子按钮的形式添加，父按钮的名字</param>
        /// <param name="toolbar">工具条的名字 (if null or empty, then the default Toolbar will be used</param>
        public Interfaces.ToolbarButton AddButton(string name, string toolbar, string parentButton, string after)
        {
            if (parentButton == "")
            {
                return AddButtonNewStyle(name, false, toolbar, null, "", after);
            }
            else
            {
                ToolStripItem newButton;
                ToolStrip toolStrip;
                if (name == "")
                {
                    Program.g_error = "在添加按钮时没有指定按钮名";
                    return null;
                }
                if (m_Buttons.ContainsKey(name))
                {
                    Program.g_error = "指定了重复名字的按钮";
                    return new ToolbarButton((ToolStripItem)m_Buttons[name]);
                }
                try
                {
                    if (toolbar == "")//没有指定toolbar则默认添加到tlbMain中
                    {
                        if (string.IsNullOrEmpty(parentButton.Trim())) //没有指定parentButton，则添加到tlbMain的指定按钮后面
                        {
                            newButton = new ToolStripButton();
                            newButton.Tag = name;
                            newButton.Name = name;
                            newButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            newButton.TextImageRelation = TextImageRelation.ImageAboveText;
                            AddButtonAfter(Program.frmMain.tlbMain.Items, newButton, after);
                        }
                        else
                        {
                            newButton = new ToolStripButton();
                            newButton.Tag = name;
                            newButton.Name = name;
                            newButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            newButton.TextImageRelation = TextImageRelation.ImageAboveText;
                            bool added = false;
                            for (int i = 0; i < Program.frmMain.tlbMain.Items.Count; i++)
                            {
                                if (Program.frmMain.tlbMain.Items[i].Name.ToLower() == parentButton.ToLower())
                                {
                                    ToolStripItemCollection items = ((ToolStripDropDownButton)Program.frmMain.tlbMain.Items[i]).DropDownItems;
                                    AddButtonAfter(items, newButton, after);
                                    added = true;
                                }
                            }
                            if (!added)
                            {
                                AddButtonAfter(Program.frmMain.tlbMain.Items, newButton, after);
                            }

                        }

                        m_Buttons.Add(name, newButton);
                        return new ToolbarButton(newButton);

                    }
                    else //添加到指定的工具条
                    {
                        toolStrip = m_CustomToolbars[toolbar] as ToolStrip;
                        if (toolStrip != null)
                        {
                            newButton = new ToolStripButton();
                            newButton.Tag = name;
                            newButton.Name = name;
                            newButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            newButton.TextImageRelation = TextImageRelation.ImageAboveText;

                            if (string.IsNullOrEmpty(parentButton.Trim()))
                            {
                                AddButtonAfter(toolStrip.Items, newButton, after);
                            }
                            else
                            {
                                bool added = false;
                                for (int i = 0; i <= toolStrip.Items.Count - 1; i++)
                                {
                                    if (toolStrip.Items[i].Name.ToLower() == parentButton.ToLower())
                                    {
                                        ToolStripDropDownButton dropDownButton = toolStrip.Items[i] as ToolStripDropDownButton;
                                        if (dropDownButton != null)
                                        {
                                            ToolStripItemCollection items = dropDownButton.DropDownItems;
                                            AddButtonAfter(items, newButton, after);
                                            added = true;
                                        }
                                    }
                                }

                                if (!added)
                                {
                                    AddButtonAfter(toolStrip.Items, newButton, after);
                                }

                            }
                            m_Buttons.Add(name, newButton);
                            return new ToolbarButton(newButton);

                        }
                        else
                        {
                            Program.g_error = "指定的工具条不存在.";
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// 返回指定工具条按钮 (null on failure)
        /// </summary>
        /// <param name="name">要寻找的工具条名字</param>
        public Interfaces.ToolbarButton ButtonItem(string name)
        {
            ToolStripButton button;
            button = m_Buttons[name] as ToolStripButton;
            if (button != null)
            {
                return new ToolbarButton(button);
            }
            return null;
        }

        /// <summary>
        /// 移除指定的按钮
        /// </summary>
        /// <param name="name">要移除的按钮的名字</param>
        /// <returns>是否成功</returns>
        public bool RemoveButton(string name)
        {
            //为什么整个都比比对Name属性和ToolStripButton对象不判定？？？
            string bName;
            bool found = false;
            ToolStrip toolStrip;
            ToolStripDropDownItem dropDownItem;
            try
            {
                //尝试从tlbMain中移除按钮
                foreach (ToolStripItem tsItem in Program.frmMain.tlbMain.Items)
                {
                    dropDownItem = tsItem as ToolStripDropDownButton;
                    bName = Convert.ToString(tsItem.Tag);
                    if (bName == name)
                    {                        
                        if (dropDownItem != null)
                        {
                            dropDownItem.DropDownItemClicked -= new ToolStripItemClickedEventHandler(Program.frmMain.tlbMain_ItemClicked);
                        }
                        Program.frmMain.tlbMain.Items.Remove(tsItem);
                        m_Buttons.Remove(name);
                        found = true;
                        break;
                    }

                    if (dropDownItem !=null)
                    {
                        for (int i = 0; i < dropDownItem.DropDownItems.Count; i++)
                        {
                            if (dropDownItem.DropDownItems[i].Name == name)
                            {
                                dropDownItem.DropDownItems.RemoveAt(i);
                                m_Buttons.Remove(name);
                                found = true;
                                break;
                            }
                        }
                    }
                }

                //从自定义的Toolbar中查找，移除
                foreach (string tname in m_CustomToolbars.Keys)
                {
                    toolStrip = m_CustomToolbars[tname] as ToolStrip;
                    foreach (ToolStripItem tsItem in toolStrip.Items)
                    {
                        bName =Convert.ToString(tsItem.Tag);
                        if (bName == name)
                        {
                            toolStrip.Items.Remove(tsItem);
                            m_Buttons.Remove(name);
                            found = true;
                            break;
                        }

                        dropDownItem = tsItem as ToolStripDropDownButton;
                        if (dropDownItem != null)
                        {
                            for (int i = 0; i < dropDownItem.DropDownItems.Count; i++)
                            {
                                if (dropDownItem.DropDownItems[i].Name == name)
                                {
                                    dropDownItem.DropDownItems.RemoveAt(i);
                                    m_Buttons.Remove(name);
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (found)
                {
                    return true;
                }
                else
                {
                    Program.g_error = "指定的按钮未找到。";
                    return false;
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
        /// 添加一个下拉列表框插件到工具条上
        /// </summary>
        /// <param name="name">新建下拉列表框的名字</param>
        /// <param name="after">新的下拉列表框后面的插件的名字</param>
        /// <param name="toolbar">这个插件所依附的工具条的名字</param>
        public Interfaces.ComboBoxItem AddComboBox(string name, string toolbar, string after)
        {
            ToolStrip toolStrip;
            ToolStripComboBox comboBox = new ToolStripComboBox();
            try
            {
                if (name == "")
                {
                    Program.g_error = "ComboBox没有指定Name";
                    return null;
                }
                if (m_ComboBoxes.ContainsKey(name))
                {
                    Program.g_error = "指定的Name重复";
                    return new ComboBoxItem(m_ComboBoxes[name]);
                }
                //没有指定Toolbar，则默认添加到tlbMain上
                if (toolbar == "")
                {
                    comboBox.Width = 100;
                    comboBox.Height = 16;
                    comboBox.Name = name;
                    Program.frmMain.tlbMain.Items.Add(comboBox);
                    comboBox.SelectedIndexChanged += new EventHandler(Program.frmMain.CustomCombo_SelectedIndexChanged);
                    comboBox.Visible = true;
                    m_ComboBoxes.Add(name, comboBox);
                    return new ComboBoxItem(comboBox);

                }
                else //指定了Toolbar，检查是否存在
                {
                    toolStrip = m_CustomToolbars[toolbar] as ToolStrip;
                    if (toolStrip != null)
                    {
                        comboBox.Width = 100;
                        comboBox.Height = 16;
                        comboBox.Name = name;
                        toolStrip.Items.Add(comboBox);
                        comboBox.SelectedIndexChanged += new EventHandler(Program.frmMain.CustomCombo_SelectedIndexChanged);
                        comboBox.Visible = true;
                        m_ComboBoxes.Add(name, comboBox); 
                        return new ComboBoxItem(comboBox);
                    }
                    else
                    {
                        Program.g_error = "指定的工具条不存在.";
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回指定的下拉列表框
        /// </summary>
        /// <param name="name">要查找条目的名字</param>
        public Interfaces.ComboBoxItem ComboBoxItem(string name)
        {
            try
            {
                ToolStripComboBox item;
                ToolStrip strip;
                ToolStripPanel topPanel = Program.frmMain.StripDocker.TopToolStripPanel;
                for (int i = 0; i < topPanel.Controls.Count; i++) //遍历TopPanel中的对象
                {
                    strip = topPanel.Controls[i] as ToolStrip;
                    if (strip != null) //找到ToolStrip类型的对象
                    {
                        for (int j = 0; i < strip.Items.Count; j++) //遍历Toolstrip中的对象
                        {
                            item = strip.Items[j] as ToolStripComboBox;
                            if (item != null && item.Name == name) //找到ToolStripComboBox对象,比对
                            {
                                return new ComboBoxItem(item);
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
                return null; 
            }
        }

        /// <summary>
        /// 移除指定的下拉列表框
        /// </summary>
        /// <param name="name">要移除的下拉列表框的名字</param>
        /// <returns>是否成功</returns>
        public bool RemoveComboBox(string name)
        {
            if (name == "")
            {
                return false;
            }
            ToolStrip strip;
            ToolStripComboBox comboBox;
            try
            {
                ToolStripPanel topPanel = Program.frmMain.StripDocker.TopToolStripPanel;
                for (int i = 0; i < topPanel.Controls.Count; i++)//遍历TopPanel中的对象
                {
                    strip = topPanel.Controls[i] as ToolStrip;
                    if (strip != null)
                    {
                        for (int j = 0; j < strip.Items.Count; j++)//遍历Toolstrip中的对象
                        {
                            comboBox = strip.Items[j] as ToolStripComboBox;
                            if (comboBox != null && comboBox.Name == name && m_ComboBoxes.ContainsKey(name))
                            {
                                strip.Items.RemoveAt(j);
                                m_ComboBoxes.Remove(name);
                            }
                        }
                    }
                }
                Program.g_error = "移除下拉列表框失败。";
                return false;
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取所有的当前已经加载的工具条名字
        /// 存在泛型集合IList中
        /// </summary>
        /// <returns>返回名字列表</returns>
        public System.Collections.Generic.IList<string> ToolbarNames()
        {
            IList<string> toolbarNamesList = new List<string>();
            foreach (string name in m_CustomToolbars.Keys)
            {
                toolbarNamesList.Add(name);
            }
            return toolbarNamesList;
        }

        /// <summary>
        /// 返回指定工具条上按钮的数量
        /// </summary>
        /// <param name="toolbarName">工具条的名字</param>
        /// <returns>0代表工具条没有找到</returns>
        public int NumToolbarButtons(string toolbarName)
        {
            try
            {
                return ((ToolStrip)m_CustomToolbars[toolbarName]).Items.Count;
            }
            catch(Exception ex)
            {
                Program.g_error = "NumToolbarButtons方法出错" + ex.ToString();
                return 0;
            }
        }

        /// <summary>
        /// 是否按下一个指定的工具条按钮
        /// </summary>
        /// <param name="name">被按下的按钮的名字</param>
        /// <returns>true，按下，false，没按</returns>
        public bool PressToolbarButton(string name)
        {
            try
            {
                foreach (ToolStripItem tsItem in Program.frmMain.tlbMain.Items)
                {
                    if (tsItem.Name.ToLower() == name.ToLower() && tsItem.Enabled)
                    {
                        ToolStripItemClickedEventArgs e = new ToolStripItemClickedEventArgs(tsItem);
                        Program.frmMain.tlbMain_ItemClicked(tsItem, e);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
            }
            return false;
        }

        /// <summary>
        /// 是否按下一个指定的工具条按钮
        /// </summary>
        /// <param name="toolbarName">工具条的名字</param>
        /// <param name="buttonName">被按下的按钮的名字</param>
        /// <returns>true，成功 false，失败</returns>
        public bool PressToolbarButton(string toolbarName, string buttonName)
        {
            try
            {
                foreach (ToolStripItem tsItem in ((ToolStrip)(m_CustomToolbars[toolbarName])).Items)
                {
                    if (tsItem.Name.ToLower() == buttonName.Trim().ToLower() && tsItem.Enabled)
                    {
                        System.Windows.Forms.ToolStripItemClickedEventArgs e = new System.Windows.Forms.ToolStripItemClickedEventArgs(tsItem);
                        Program.frmMain.tlbMain_ItemClicked(tsItem, e);
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Program.g_error = ex.ToString();
            }

            return false;
        }

        #endregion

        internal bool Contains(object key)
        {
            if (m_Buttons.Contains(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// AddToolbar调用，获取最后一个toolstrip的位置，以便确定新的toolstrip的位置
        /// </summary>
        private System.Drawing.Point GetLastLocationOnToolStrip(System.Windows.Forms.ToolStripPanel myToolstripPanel)
        {
            int x_end = 3;
            int y = 3;

            foreach (Control myControl in myToolstripPanel.Controls)
            {
                if (!(myControl is ToolStrip))
                {
                    continue;
                }

                ToolStrip myToolstrip = myControl as ToolStrip;
                if (myToolstrip == null)
                {
                    continue;
                }
                if (myToolstrip.Name == "MenuStrip1")
                {
                    continue;
                }

                if (!m_CustomToolbars.ContainsKey(myToolstrip.Name))//将系统自带的工具条对象添加到m_CustomToolbars中
                {
                    m_CustomToolbars.Add(myToolstrip.Name, myToolstrip);
                }

                // Get the highest values:
                if (myToolstrip.Location.X + myToolstrip.Width > x_end && myToolstrip.Location.Y >= y)
                {
                    x_end = myToolstrip.Location.X + myToolstrip.Width;
                }

                if (myToolstrip.Location.Y > y)
                {
                    //Next row:
                    x_end = myToolstrip.Location.X + myToolstrip.Width;
                    y = myToolstrip.Location.Y;
                }
            }

            return new System.Drawing.Point(x_end, y);
        }

        /// <summary>
        /// 添加一个toolbar buttom
        /// </summary>
        /// <param name="buttonName">按钮名</param>
        /// <param name="isDropDown">是否是下拉按钮</param>
        /// <param name="toolbarName">toolbar的Name</param>
        /// <param name="picture">按钮图片</param>
        /// <param name="buttonLabel">按钮下面的文字</param>
        /// <param name="after"></param>
        /// <returns></returns>
        private Interfaces.ToolbarButton AddButtonNewStyle(string buttonName, bool isDropDown, string toolbarName, object picture, string buttonLabel = "", string after = "")
        {
            ToolStripItem newButton;

            if (buttonName == "")
            {
                Program.g_error = "没有指定要添加按钮的Name.";
                return null;
            }

            if (m_Buttons.ContainsKey(buttonName))
            {
                Program.g_error = "指定了一个重复的Name.";
                return new ToolbarButton((ToolStripItem)(m_Buttons[buttonName]));
            }

            try
            {
                if (buttonName != "-")
                {
                    if (isDropDown)
                    {
                        newButton = new ToolStripDropDownButton();
                        ((ToolStripDropDownButton)newButton).DropDownItemClicked += new ToolStripItemClickedEventHandler(Program.frmMain.tlbMain_ItemClicked);

                    }
                    else
                    {
                        newButton = new ToolStripButton();
                    }

                    //获取按钮要添加的工具条按钮
                    ToolStrip toolStrip = null;
                    if (m_CustomToolbars.Contains(toolbarName))
                    {
                        toolStrip = (ToolStrip)(m_CustomToolbars[toolbarName]);
                    }
                    //默认用tlbMain工具条
                    if (toolStrip == null)
                    {
                        toolStrip = Program.frmMain.tlbMain;
                    }

                    newButton.Tag = buttonName;
                    newButton.Name = buttonName;
                    if (string.IsNullOrEmpty(buttonLabel))
                    {
                        buttonLabel = buttonName;
                    }

                    if (Convert.ToString(toolStrip.Tag) != "Image")
                    {
                        newButton.Text = buttonLabel;
                        newButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                        newButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                    }

                    if (picture != null)
                    {
                        newButton.Image = (System.Drawing.Image)picture;
                    }

                    AddButtonAfter(toolStrip.Items, newButton, after);

                    m_Buttons.Add(buttonName, newButton);
                    return new ToolbarButton(newButton);

                }
                else //是“—”添加到哪里？
                {
                    Program.frmMain.menuStrip1.Items.Add(new ToolStripSeparator());
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
                return null;
            }

            return null;
        }

        /// <summary>
        /// 未实现
        /// 添加按钮到指定按钮的后面，若找不到给定的，则添加到最后
        /// </summary>
        /// <param name="items"></param>
        /// <param name="NewButton"></param>
        /// <param name="After"></param>
        private void AddButtonAfter(ToolStripItemCollection items, ToolStripItem newButton, string after)
        {
            bool found = false;
            if (after != "")
            {
                for (int i = 0; i < items.Count - 1; i++) 
                {
                    if (items[i].Name.ToLower() == after.ToLower())
                    {
                        items.Insert(i + 1, newButton);
                        found = true;
                    }
                }
            }
            //添加到最后面
            if (!found)
            {
                items.Add(newButton);
            }
        }

        /// <summary>
        /// 未实现
        /// </summary>
        public Interfaces.ToolbarButton DropDownButtonItem(string name)
        {
            ToolStripDropDownButton button;
            button = m_Buttons[name] as ToolStripDropDownButton;
            if (button != null)
            {
                return new ToolbarButton(button);
            }
            return null;
        }


    }//24
}
