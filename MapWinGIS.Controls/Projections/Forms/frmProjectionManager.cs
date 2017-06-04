// ----------------------------------------------------------------------------
// MapWinGIS.Controls.Projections:
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    
    /// <summary>
    /// Provides GUI to work with favorite and custom projections
    /// </summary>
    public partial class frmProjectionManager : Form
    {
        // Reference to MapWinGIS
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        // the previous projection code shown
        private int m_lastCode = 4326;

        /// <summary>
        /// Creates a new instance of frmProjectionManager class
        /// </summary>
        public frmProjectionManager(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();

            if (mapWin == null)
                throw new NullReferenceException("No reference to MapWinGIS was passed");

            ProjectionDatabase database = mapWin.ProjectionDatabase as ProjectionDatabase;
            if (database == null)
                throw new InvalidCastException("Invalid instance of projection database was passed");

            m_mapWin = mapWin;

            // initializing tree
            if (this.projectionTreeView1.Initialize(database, m_mapWin))
            {
                int gcsCount, pcsCount;
                this.projectionTreeView1.RefreshList(out gcsCount, out pcsCount);
                lblGcsCount.Text = "Coordinate systems: " + gcsCount.ToString();
                lblPcsCount.Text = "Projections: " + pcsCount.ToString();
            }
            projectionTreeView1.CoordinateSystemSelected += new ProjectionTreeView.CoordinateSystemSelectedDelegate(projectionTreeView1_CoordinateSystemSelected);

            // initializing map
            projectionMap1.LoadStateFromExeName(Application.ExecutablePath);
            projectionMap1.CoordinatesChanged += new ProjectionMap.CoordinatesChangedDelegate(projectionMap1_CoordinatesChanged);

            // showing information on WGS 84
            IEnumerable<GeographicCS> list = projectionTreeView1.CoordinateSystems.Where(cs => cs.Code == 4326);
            projectionTreeView1_CoordinateSystemSelected((Territory)list.First());
        }

        /// <summary>
        /// Updates coordianates in the status bar
        /// </summary>
        void projectionMap1_CoordinatesChanged(double x, double y, string textX, string textY)
        {
            lblX.Text = "Long: " + textX;
            lblY.Text = "Lat: " + textY;
        }

        /// <summary>
        /// Updting map and text
        /// </summary>
        void projectionTreeView1_CoordinateSystemSelected(Territory cs)
        {
            projectionMap1.DrawCoordinateSystem(cs);
            projectionMap1.ZoomToCoordinateSystem(cs);

            m_lastCode = cs.Code;
            txtCode.Text = cs.Code.ToString();
            txtName.Text = cs.Name;
            CoordinateSystem coord = cs as CoordinateSystem;
            if (coord != null)
            {
                txtRemarks.Text = coord.Remarks != "" ? coord.Remarks : "No remarks";
                txtScope.Text = coord.Scope != "" ? coord.Scope: "No description";
            }
        }

        /// <summary>
        /// Show projection at spatialreference.org site
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string link = "http://spatialreference.org/ref/epsg/" + txtCode.Text + "/";
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked." + Environment.NewLine + ex.Message);
            }
        }
        
        /// <summary>
        /// Zoomes to max extents
        /// </summary>
        private void btnZoomToMaxExtents_Click(object sender, EventArgs e)
        {
            projectionMap1.ZoomToMaxExtents();
        }

        /// <summary>
        /// Shows properties for selected coorinate system
        /// </summary>
        private void btnProperties_Click(object sender, EventArgs e)
        {
            int code;
            if (Int32.TryParse(txtCode.Text, out code))
            {
                projectionTreeView1.ShowProjectionProperties(code);
            }
        }

        /// <summary>
        /// Zoomes to coordinate system
        /// </summary>
        private void btnZoomToProjection_Click(object sender, EventArgs e)
        {
            projectionMap1.ZoomToCoordinateSystem();
        }

        /// <summary>
        /// Seeking coordinate system by code enetered by user
        /// </summary>
        private void txtCode_Validating(object sender, CancelEventArgs e)
        {
            this.ShowProjectionByCode();
        }

        /// <summary>
        /// Seeking coordinate system by code enetered by user
        /// </summary>
        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.ShowProjectionByCode();
            }
        }

        /// <summary>
        /// Seeking coordinate system by code enetered by user
        /// </summary>
        private void ShowProjectionByCode()
        {
            int val;
            if (!Int32.TryParse(txtCode.Text, out val))
            {
                txtCode.Text = m_lastCode.ToString();
                val = m_lastCode;     
            }

            if (val == m_lastCode)
                return;

            // showing information on WGS 84
            IEnumerable<GeographicCS> list = projectionTreeView1.CoordinateSystems.Where(cs => cs.Code == val);
            if (list.Count() > 0)
            {
                projectionTreeView1_CoordinateSystemSelected((CoordinateSystem)list.First());
            }
            else
            {
                IEnumerable<ProjectedCS> list2 = projectionTreeView1.Projections.Where(cs => cs.Code == val);
                if (list2.Count() > 0)
                {
                    projectionTreeView1_CoordinateSystemSelected((CoordinateSystem)list2.First());
                }
                else
                {
                    txtCode.Text = m_lastCode.ToString();
                    MessageBox.Show("Failed to find coordinate system with EPSG code: " + val, m_mapWin.ApplicationInfo.ApplicationName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
