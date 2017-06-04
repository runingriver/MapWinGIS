using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using System.Drawing;
using System.Threading;
using System.Resources;
using System.Globalization;

namespace AppPluginSample
{
    public class AppPlugin : IPlugin
    {
        IMapWin m_MapWin;
        int m_ParentHandle;

        ResourceManager resources = new ResourceManager("AppPluginSample.Global", System.Reflection.Assembly.GetExecutingAssembly());

        #region 插件基本信息属性
        public string Author { get { return "hehheh"; } }

        public string Description { get { return "aaaaaaaaaaa"; } }

        public string SerialNumber { get { return "dagsdg"; } }

        public string Name { get { return "AppPluginSample"; } }

        public string BuildDate { get { return "2014-07-18"; } }

        public string Version { get { return "1.0.0.0"; } }
        #endregion

        #region 插件接口方法

        public void Initialize(IMapWin mapWin, int parenthandle)
        {
            m_MapWin = mapWin;
            this.m_ParentHandle = parenthandle;

            CreateToolbarAndMenu();

        }

        private void CreateToolbarAndMenu(string local = null)
        {
            //添加按钮到tlbMain工具条上
            //if (local == null)
            //{
            //    local = Thread.CurrentThread.CurrentUICulture.Name;
            //    //info = CultureInfo.InvariantCulture;
            //}

            //CultureInfo info = new CultureInfo(local);

            MapWinGIS.Interfaces.Toolbar toolbar = m_MapWin.Toolbar;
            MapWinGIS.Interfaces.ToolbarButton btn = toolbar.AddButton("PluginSample");
            btn.BeginsGroup = true;
            btn.Category = "tlbAppPlugin";
            btn.Description = resources.GetString("btn_Description"); //"this is a sample plugin.";
            btn.Enabled = true;
            btn.Tooltip = resources.GetString("btn_Tooltip"); //"this is a sample plugin.";
            btn.Text = resources.GetString("btn_Text"); //"AppPlugin";
            btn.Picture = new System.Drawing.Icon(this.GetType(), "run.ico");


            //添加菜单到顶级菜单栏
            MapWinGIS.Interfaces.Menus menu = m_MapWin.Menus;
            MapWinGIS.Interfaces.MenuItem mnuItem = menu.AddMenu("mnuAppPlugin");
            mnuItem.BeginsGroup = true;
            mnuItem.Category = "mnuAppPlugin";
            mnuItem.Tooltip = "thi is a Sample Plugin menu text.";
            mnuItem.Text = resources.GetString("mnuItem_Text"); //"插件测试(&A)";

            MapWinGIS.Interfaces.MenuItem subMnuItem1 = menu.AddMenu("mnuAppPlugin1", "mnuAppPlugin");
            subMnuItem1.BeginsGroup = true;
            subMnuItem1.Category = "mnuAppPlugin";
            subMnuItem1.Tooltip = "thi is a Sample Plugin menu text.";
            subMnuItem1.Text = resources.GetString("subMnuItem1_Text"); //"插件1(&B)";

            MapWinGIS.Interfaces.MenuItem subMnuItem2 = menu.AddMenu("mnuAppPlugin2", "mnuAppPlugin");
            subMnuItem2.BeginsGroup = true;
            subMnuItem2.Category = "mnuAppPlugin";
            subMnuItem2.Tooltip = "thi is a Sample Plugin menu text.";
            subMnuItem2.Text = resources.GetString("subMnuItem2_Text");// "插件2(&B)";

            //添加到"图层"菜单中
            MapWinGIS.Interfaces.Menus layerMenu = m_MapWin.Menus; //"图层插件菜单(&T)"
            MapWinGIS.Interfaces.MenuItem mnuItem3 = layerMenu.AddMenu("mnuAppPlugin3", "mnuLayer", null, resources.GetString("mnuAppPlugin3_Text"), "mnuLayerBreak3");
            mnuItem3.BeginsGroup = true;
            mnuItem3.Category = "mnuAppPlugin";
            mnuItem3.Tooltip = "thi is a Sample Plugin menu text.";
        }

        public void Terminate()
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

        public void ItemClicked(string itemName, ref bool handled)
        {
            if (itemName == "PluginSample")
            {
             System.Windows.Forms.MessageBox.Show("ddd");
            handled = true;
            }

        }

        public void Message(string msg, ref bool handled)
        {
            if (msg.Contains("Language"))
            {
                //string local = msg.Substring(9);
                //string name = Thread.CurrentThread.CurrentUICulture.Name;
                //string loc = Thread.CurrentThread.CurrentCulture.Name;

                UninstallToolbarAndMenu();
                CreateToolbarAndMenu();
            }
        }

        private void UninstallToolbarAndMenu()
        {
            MapWinGIS.Interfaces.Toolbar toolbar = m_MapWin.Toolbar;
            toolbar.RemoveButton("PluginSample");

            MapWinGIS.Interfaces.Menus menu = m_MapWin.Menus;
            menu.Remove("mnuAppPlugin");
            menu.Remove("mnuAppPlugin1");
            menu.Remove("mnuAppPlugin2");
            
            menu.Remove("mnuAppPlugin3");
        }

        #endregion
    }
}
