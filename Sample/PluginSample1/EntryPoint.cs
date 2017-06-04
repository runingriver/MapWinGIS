using System;
using System.Collections.Generic;
using System.Text;
using MapWinGIS.Interfaces;
using System.Resources;
using System.Drawing;

namespace PluginSample1
{
    public class EntryPoint : IPlugin
    {
        IMapWin m_MapWin;
        int m_ParentHandle;

        ResourceManager res = new ResourceManager(typeof(PluginSample1.Resource));



        #region 插件基本信息属性
        public string Author { get { return "huzongzhe"; } }

        public string Description { get { return "aaaaaaaaaaaaaaaa"; } }

        public string SerialNumber { get { return "dagdgasdg"; } }

        public string Name { get { return "PluginSample1"; } }

        public string BuildDate { get { return "2014-07-16"; } }

        public string Version { get { return "1.0.0.0"; } }
        #endregion

        #region 插件接口方法

        public void Initialize(IMapWin mapWin, int parenthandle)
        {
            this.m_MapWin = mapWin;
            this.m_ParentHandle = parenthandle;

            MapWinGIS.Interfaces.Toolbar toolbar = m_MapWin.Toolbar;
            if (toolbar.AddToolbar("tlbPluginSample1"))
            {
                MapWinGIS.Interfaces.ToolbarButton btn = toolbar.AddButton("tlbPlugintest", "tlbPluginSample1", false);
                btn.BeginsGroup = true;
                btn.Text = "测试";
                btn.Tooltip = "测试按钮";
                btn.Picture = res.GetObject("sample");

                toolbar.AddButtonDropDownSeparator("sep", "tlbPluginSample1", "tlbPlugintest");
                toolbar.AddButtonDropDownSeparator("sep1", "tlbZoom", "tbbZoomExtent");
                MapWinGIS.Interfaces.ComboBoxItem cmb = toolbar.AddComboBox("comboBox1", "tlbPluginSample1", "tlbPlugintest");
                cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            }
        }

        public void Terminate()
        {
            MapWinGIS.Interfaces.Toolbar toolbar = m_MapWin.Toolbar;
            toolbar.RemoveButton("tlbPlugintest");
            toolbar.RemoveButton("sep");
            toolbar.RemoveButton("sep1");
            toolbar.RemoveComboBox("comboBox1");
            toolbar.RemoveToolbar("tlbPluginSample1");
            this.m_MapWin = null;
        }

        public void ItemClicked(string itemName, ref bool handled)
        { }

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
