using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public partial class LegendEditorForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        Interfaces.eLayerType lyrType;
        MapWinGIS.Interfaces.Layer lyr;
        object obj;
        AxMapWinGIS.AxMap map;
        public int m_groupHandle = -1;

        public LegendEditorForm(int handle, bool isLayer, AxMapWinGIS.AxMap axmap)
        {
            InitializeComponent();
            LoadProperties(handle, isLayer);
            map = axmap;
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
        }

        /// <summary>
        /// 为当前层创建和显示属性编辑框
        /// </summary>
        public static LegendEditorForm CreateAndShowLYR()
        {
            return CreateAndShowLYR(Program.frmMain.Layers.CurrentLayer);
        }

        /// <summary>
        /// 为指定的层创建和显示属性编辑框
        /// </summary>
        public static LegendEditorForm CreateAndShowLYR(int layerHandle)
        {
            Size sz = new Size(385, 430);//后续，用C#提供的保存窗体的大小方法
            LegendEditorForm newLegend = new LegendEditorForm(layerHandle, true, Program.frmMain.MapMain);
            newLegend.Size = sz;
            Program.frmMain.AddOwnedForm(newLegend);
            newLegend.Show(Program.frmMain.dckPanel, WeifenLuo.WinFormsUI.Docking.DockState.Float);
            return newLegend;
        }

        public static LegendEditorForm CreateAndShowGRP(int GroupHandle)
        {
            LegendEditorForm newLeg = new LegendEditorForm(GroupHandle, false, Program.frmMain.MapMain);

            Program.frmMain.AddOwnedForm(newLeg);
            newLeg.Show(Program.frmMain.dckPanel, WeifenLuo.WinFormsUI.Docking.DockState.Float);
            return newLeg;
        }






        /// <summary>
        /// 未实现
        /// </summary>
        public void LoadProperties(int handle, bool isLayer)
        { }



    }
}
