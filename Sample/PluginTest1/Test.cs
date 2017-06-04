using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using System.Windows.Forms;

namespace PluginTest1
{
    public class Test :IPlugin
    {
        IMapWin m_MapWin;
        int m_ParentHandle;
        string caption = "测试窗体";

        #region 插件基本信息属性

        public string Author { get { return "huzongzhe"; } }

        public string Description { get { return "dfasdfa"; } }

        public string SerialNumber { get { return "dagdgasdg"; } }

        public string Name { get { return "PluginSample3::MyTestProject"; } }

        public string BuildDate { get { return "2014-07-15"; } }

        public string Version { get { return "1.1.1.0"; } }
        #endregion

        #region 插件接口方法

        public void Initialize(IMapWin mapWin, int parenthandle)
        {
            this.m_MapWin = mapWin;
            this.m_ParentHandle = parenthandle;

            MapWinGIS.Interfaces.UIPanel myFormPanel = m_MapWin.UIPanel;
            Panel panel = myFormPanel.CreatePanel("测试窗体", MapWinGISDockStyle.None);
            MapWinGIS.Interfaces.OnPanelClose panelClose = this.OnFormClose;
            //MapWinGIS.Interfaces.OnPanelClose panel2close = this.OnForm2Close;
            System.Windows.Forms.Form form = panel.Parent as System.Windows.Forms.Form;
            if (form != null)
            {
                form.StartPosition = FormStartPosition.CenterScreen;
            }

            Button btn = new Button();
            btn.Name = "btn";
            btn.Text = "确定";
            btn.Location = new System.Drawing.Point(20, 20);
            btn.Size = new System.Drawing.Size(75, 25);

            Button btn2 = new Button();
            btn2.Name = "btn2";
            btn2.Text = "取消";
            btn2.Location = new System.Drawing.Point(70, 100);
            btn2.Size = new System.Drawing.Size(75, 25);

            //panel.Controls.AddRange(new System.Windows.Forms.Control[] { btn, btn2 });
            panel.Controls.Add(btn);
            panel.Controls.Add(btn2);

            m_MapWin.UIPanel.AddOnCloseHandler("测试窗体", panelClose);

        }

        public void OnFormClose(string caption)
        {
            //MessageBox.Show("你确定关闭此窗体吗？", "关闭", MessageBoxButtons.YesNo);
            //if (MessageBox.Show("你确定关闭此窗体吗？", "关闭", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    return;
            //}
        }

        public void Terminate()
        {
            m_MapWin.UIPanel.DeletePanel(caption);
        }

        public void ItemClicked(string itemName, ref bool handled)
        {
            //MessageBox.Show("sss");
            if (itemName == "mnu测试窗体")
            {
                //MessageBox.Show("进来了");
                m_MapWin.UIPanel.SetPanelVisible(caption, false);
                handled = true;
            }
        }

        public void Message(string msg, ref bool handled)
        { }

        public void ProjectLoading(string projectFile, string settings)
        { }

        public void ProjectSaving(string projectFile, ref string settings)
        { }

        public void LegendDoubleClick(int handle, ClickLocation location, ref bool handled)
        { }

        public void LegendMouseDown(int handle, int button, ClickLocation location, ref bool handled)
        { }

        public void LegendMouseUp(int handle, int button, ClickLocation location, ref bool handled)
        { }

        public void MapExtentsChanged()
        { }

        public void MapMouseDown(int button, int shift, int x, int y, ref bool handled)
        { }

        public void MapMouseMove(int screenX, int screenY, ref bool handled)
        { }

        public void MapMouseUp(int button, int shift, int x, int y, ref bool handled)
        { }

        public void MapDragFinished(System.Drawing.Rectangle bounds, ref bool handled)
        { }

        public void LayersAdded(Layer[] layers)
        { }

        public void LayerRemoved(int handle)
        { }

        public void LayerSelected(int handle)
        { }

        public void LayersCleared()
        { }

        public void ShapesSelected(int handle, SelectInfo selectInfo)
        { }

        #endregion
    }
}
