/****************************************************************************
 * 文件名:clsLegendPanel.cs (F)
 * 描  述:开发给插件管理Legend的方法，包括控制legend的关闭，停靠位置。
 * **************************************************************************/

namespace MapWinGIS.MainProgram
{
    public class LegendPanel : MapWinGIS.Interfaces.LegendPanel
    {

        /// <summary>
        /// 关闭LegendPanel
        /// </summary>
        public void Close()
        { 
            if (Program.frmMain.legendPanel != null)
            {
                Program.frmMain.legendPanel.Hide();
            }
        }

        /// <summary>
        /// 浮动，停靠LegendPanel
        /// </summary>
        public void DockTo(MapWinGIS.Interfaces.MapWinGISDockStyle dockStyle)
        {
            if (Program.frmMain.legendPanel != null)
            {
                Program.frmMain.legendPanel.Show();
                Program.frmMain.legendPanel.Visible = true;
            }
        }
    }
}
