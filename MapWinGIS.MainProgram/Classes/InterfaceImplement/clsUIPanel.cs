/****************************************************************************
 * 文件名:clsUIPanel.cs （F）
 * 描  述:提供给插件添加一个可浮动窗体对象，并可以往panel里面添加想要的内容。 
 *        1.虽然提供了可浮动窗体的功能，但是没有可视化的编辑，实用性变得很低？
 *     还是可以先在form中做好，最后将代码复制过来以快速实现？     
 *        2.由于无法肯定该类的实际使用，所以所有实现逻辑都是根据想象而来。
 *        3.由插件创建的浮动窗体还不能实现记忆窗体的状态。
 *        4.尝试改进吗，或者根本就不是一个好的设计，累赘的设计？
 * **************************************************************************/

using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public class UIPanel : MapWinGIS.Interfaces.UIPanel
    {
        /// <summary>
        /// 存储添加到宿主的窗体。
        /// key-窗体名，value -MWDockPanel窗体对象
        /// </summary>
        internal Dictionary<string, MWDockPanel> m_Panels = new Dictionary<string, MWDockPanel>();

        /// <summary>
        /// 存储由插件创建的窗体的关闭事件的处理方法，一个窗体可能绑定多个OnPanelClose类型的委托
        /// key- Panel名，Value - ArrayList - Interfaces.OnPanelClose
        /// </summary>
        internal Dictionary<string, ArrayList> m_OnCloseHandlers = new Dictionary<string, ArrayList>();
        

        #region  --------------------UIPanel 接口实现----------------------

        /// <summary>
        /// 返回一个能够用作添加可停靠内容（dockable content）到MainProgram的系统panel。
        /// 先创建一个可停靠Form，再在Form里面添加一个Panel，返回这个Panel给调用者。
        /// </summary>
        public System.Windows.Forms.Panel CreatePanel(string caption, Interfaces.MapWinGISDockStyle dockStyle)
        {
            if (m_Panels.ContainsKey(caption))
            {
                if (m_Panels[caption].Controls["ContentPanel"] != null)
                {
                    return (System.Windows.Forms.Panel)(m_Panels[caption].Controls["ContentPanel"]);
                }               
                return (System.Windows.Forms.Panel)(m_Panels[caption].Controls[0]);
            }

            Panel contentPanel = new Panel();
            contentPanel.Name = "ContentPanel";
            contentPanel.Dock = DockStyle.Fill;

            MWDockPanel floatPanel = new MWDockPanel(caption);
            floatPanel.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            floatPanel.Controls.Add(contentPanel);
            floatPanel.FormClosing +=new FormClosingEventHandler(MarkClosed);
            floatPanel.Show(Program.frmMain.dckPanel);
            floatPanel.Icon = MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;
            if (dockStyle.ToString() == DockStyle.None.ToString())
            {
                floatPanel.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            }
            else
            {
                floatPanel.DockState = SimplifyDockstate(dockStyle);
            }

            //我希望创建该窗体时也要创建一个该窗体显示与关闭的菜单
            Program.frmMain.m_Menu.AddMenu("mnu" + caption, "mnuRestoreMenu", (object)null, caption).Checked = floatPanel == null ? false : true;

            m_Panels.Add(caption, floatPanel);
            m_OnCloseHandlers.Add(caption, null); //创建一个窗体时，要添加进去
            return contentPanel;
        }

        /// <summary>
        /// 返回一个能够用作添加可停靠内容（dockable content）到MainProgram的系统panel
        /// 先创建一个可停靠Form，再在Form里面添加一个Panel，返回这个Panel给调用者。
        /// </summary>
        public System.Windows.Forms.Panel CreatePanel(string caption, System.Windows.Forms.DockStyle dockStyle)
        {
            if (m_Panels.ContainsKey(caption))
            {
                if (m_Panels[caption].Controls["ContentPanel"] != null)
                {
                    return (System.Windows.Forms.Panel)(m_Panels[caption].Controls["ContentPanel"]);
                }
                return (System.Windows.Forms.Panel)(m_Panels[caption].Controls[0]);
            }

            Panel contentPanel = new Panel();
            contentPanel.Name = "ContentPanel";
            contentPanel.Dock = DockStyle.Fill;

            MWDockPanel floatPanel = new MWDockPanel(caption);
            floatPanel.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            floatPanel.Controls.Add(contentPanel);
            floatPanel.FormClosing += new FormClosingEventHandler(MarkClosed);
            floatPanel.Show(Program.frmMain.dckPanel);
            floatPanel.Icon = MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;
            if (dockStyle == DockStyle.None)
            {
                floatPanel.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            }
            else
            {
                floatPanel.DockState = SimplifyDockstate(dockStyle);
            }

            //我希望创建该窗体时也要创建一个该窗体显示与关闭的菜单
            Program.frmMain.m_Menu.AddMenu("mnu" + caption, "mnuRestoreMenu", (object)null, caption).Checked = floatPanel == null ? false : true;

            m_Panels.Add(caption, floatPanel);
            m_OnCloseHandlers.Add(caption, null);
            return contentPanel;
        }

        /// <summary>
        /// 删除指定的panel,执行关闭窗口相关的方法，回收该窗体包含的所有资源。
        /// 一般用于Terminate()方法中
        /// </summary>
        public void DeletePanel(string caption)
        {
            if (Program.frmMain.m_Menu.m_MenuTable.ContainsKey("mnu" + caption) && !Program.frmMain.m_Menu.Remove("mnu" + caption)) //如果移除失败，则尝试手动移除一次。
            {
                try
                {
                    System.Windows.Forms.ToolStripItem submenu = (System.Windows.Forms.ToolStripItem)Program.frmMain.m_Menu.m_MenuTable["mnu" + caption];
                    Program.frmMain.menuStrip1.Items.Remove(submenu);
                    if (Program.frmMain.m_Menu.m_MenuTable.ContainsKey("mnu" + caption)) //可能在Remove方法中移除了。
                    {
                        Program.frmMain.m_Menu.m_MenuTable.Remove("mnu" + caption);
                    }
                }
                catch (System.Exception ex)
                {
                    MapWinGIS.Utility.Logger.Dbg("移除可浮动窗体显示菜单失败！菜单名: mnu" + caption + "描述:" + ex.ToString());
                }
            }

            if (m_OnCloseHandlers[caption] != null)
            {
                ArrayList arrylist = m_OnCloseHandlers[caption];
                while (arrylist.Count > 0)
                {
                    //触发绑定该委托的方法。一个窗体可以能绑定了多个OnPanelClose类型的方法，所以一一执行后删除。
                    ((Interfaces.OnPanelClose)(arrylist[0])).Invoke(caption);
                    arrylist.RemoveAt(0);
                }
            }

            if (m_OnCloseHandlers.ContainsKey(caption)) //移除存储该窗体的关闭事件关联的委托
            {
                m_OnCloseHandlers.Remove(caption);
            }

            MWDockPanel floatPanel = m_Panels[caption];
            floatPanel.FormClosing -= new FormClosingEventHandler(MarkClosed);

            if (m_Panels.ContainsKey(caption)) //注销窗体
            {
                if (m_Panels[caption] == null)
                {
                    m_Panels.Remove(caption);
                    return;
                }
                WeifenLuo.WinFormsUI.Docking.DockContent child = m_Panels[caption];
                while (child.Controls.Count > 0)
                {
                    child.Controls.RemoveAt(0);
                }

                m_Panels.Remove(caption);
                child.Close(); //该窗体关闭时会注销掉所有拥有的资源，并被回收。
            }
        }

        /// <summary>
        /// 如果一个panel没有必要删除，可以设置它的可见性来显示与隐藏该窗体
        /// </summary>
        public void SetPanelVisible(string caption, bool visible)
        {
            if (m_Panels[caption] != null)
            {
                WeifenLuo.WinFormsUI.Docking.DockContent child = m_Panels[caption];

                if (Program.frmMain.m_Menu["mnu" + caption].Checked)
                {
                    child.Hide();
                    Program.frmMain.m_Menu["mnu" + caption].Checked = false;
                }
                else
                {
                    child.Show();
                    Program.frmMain.m_Menu["mnu" + caption].Checked = true;
                }
            }
        }

        /// <summary>
        /// 将指定Panel中要注销的句柄添加到m_OnCloseHandlers中
        /// 一般在往Panel里面添加句柄时，就应该调用该函数添加
        /// </summary>
        public void AddOnCloseHandler(string caption, Interfaces.OnPanelClose onCloseFunction)
        {
            if (!m_OnCloseHandlers.ContainsKey(caption))
            {
                MapWinGIS.Utility.Logger.Message("没有名为:" + caption + "的窗体，无法添加关闭句柄!");
                return;
            }
            if (m_OnCloseHandlers[caption] == null)
            {
                m_OnCloseHandlers[caption] = new ArrayList();
            }
            m_OnCloseHandlers[caption].Add(onCloseFunction);
        }

        #endregion

        /// <summary>
        /// 根据Forms.DockStyle设置WeiFenLuo的Dock方式
        /// </summary>
        public static WeifenLuo.WinFormsUI.Docking.DockState SimplifyDockstate(System.Windows.Forms.DockStyle dockStyle)
        {
            switch (dockStyle)
            {
                case System.Windows.Forms.DockStyle.Bottom:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
                case System.Windows.Forms.DockStyle.Fill:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Document;
                case System.Windows.Forms.DockStyle.Left:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                case System.Windows.Forms.DockStyle.None:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
                case System.Windows.Forms.DockStyle.Right:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
                case System.Windows.Forms.DockStyle.Top:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
                default:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
            }

        }

        /// <summary>
        /// 根据接口中的DockStyle设置WeiFenLuo的dock方式
        /// </summary>
        public static WeifenLuo.WinFormsUI.Docking.DockState SimplifyDockstate(Interfaces.MapWinGISDockStyle dockStyle)
        {
            switch (dockStyle)
            {
                case Interfaces.MapWinGISDockStyle.Bottom:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
                case Interfaces.MapWinGISDockStyle.BottomAutoHide:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
                case Interfaces.MapWinGISDockStyle.Left:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                case Interfaces.MapWinGISDockStyle.LeftAutoHide:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide;
                case Interfaces.MapWinGISDockStyle.Right:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                case Interfaces.MapWinGISDockStyle.RightAutoHide:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide;
                case Interfaces.MapWinGISDockStyle.Top:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
                case Interfaces.MapWinGISDockStyle.TopAutoHide:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide;
                case Interfaces.MapWinGISDockStyle.None:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
                default :
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
            }
        }

        /// <summary>
        /// 窗体关闭事件
        /// 这里关闭窗体事件，只将窗体隐藏，并执行窗体关闭关联的方法。
        /// </summary>
        private void MarkClosed(object sender, FormClosingEventArgs e)
        {
            if (sender != null)
            {
                MWDockPanel dockForm = sender as MWDockPanel;
                if (dockForm != null)
                {
                    dockForm.Hide();
                    e.Cancel = true;

                    Program.frmMain.m_Menu["mnu" + dockForm.Text].Checked = false;
                }    
            }
        }


    }
}
