/******************************************************************
 * 文件名：frmProjectionCompare.cs
 * 描  述：
 * ***************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.Controls.Projections
{
    public partial class frmProjectionCompare : Form
    {
        //项目投影
        private MapWinGIS.GeoProjection m_projectProj = null;

        //图层投影
        private MapWinGIS.GeoProjection m_layerProj = null;

        private ProjectionDatabase m_database = null;

        /// <summary>
        /// 创建一个frmProjectionCompare类的新实例
        /// 构造函数
        /// </summary>
        /// <param name="projectProj">项目投影</param>
        /// <param name="layerProj">图层投影</param>
        /// <param name="database">数据库</param>
        public frmProjectionCompare(MapWinGIS.GeoProjection projectProj, MapWinGIS.GeoProjection layerProj, ProjectionDatabase database)
        {
            InitializeComponent();

            m_projectProj = projectProj;
            m_layerProj = layerProj;
            m_database = database;

            this.lblProject.Text = "项目： " + projectProj.Name;
            this.lblLayer.Text = "图层： " + layerProj.Name;

            this.txtProject.Text = projectProj.ExportToProj4();
            this.txtLayer.Text = layerProj.ExportToProj4();

            this.btnLayer.Click += delegate(object sender, EventArgs e)
            {
                this.ShowProjectionProperties(m_layerProj);
            };

            this.btnProject.Click += delegate(object sender, EventArgs e)
            {
                this.ShowProjectionProperties(m_projectProj);
            };
        }

        /// <summary>
        /// 显示选择的投影的属性
        /// </summary>
        /// <param name="proj">选择的投影</param>
        private void ShowProjectionProperties(MapWinGIS.GeoProjection proj)
        {
            if (proj == null || proj.IsEmpty)
                return;

            CoordinateSystem cs = null;
            if (m_database != null)
                cs = m_database.GetCoordinateSystem(proj, ProjectionSearchType.Enhanced);

            if (cs != null)
            {
                frmProjectionProperties form = new frmProjectionProperties(cs, m_database);
                form.ShowDialog(this);
                form.Dispose();
            }
            else
            {
                frmProjectionProperties form = new frmProjectionProperties(proj);
                form.ShowDialog(this);
                form.Dispose();
            }
        }
    }
}
