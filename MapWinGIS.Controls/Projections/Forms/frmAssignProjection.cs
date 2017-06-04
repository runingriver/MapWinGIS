// frmAssignProjection
// Author: Sergei Leschinski

namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Linq;
    using System.Windows.Forms;
    using MapWinGIS.Controls.General;
    
    /// <summary>
    /// A form providing GUI for assigning projection
    /// </summary>
    public partial class frmAssignProjection : Form
    {
        // Reference to MapWinGIS
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        /// <summary>
        /// Creates a new instance of the frmAssignProjection class
        /// </summary>
        public frmAssignProjection(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();
            
            if (mapWin == null)
                throw new NullReferenceException("No reference to MapWinGIS was passed");

            ProjectionDatabase database = mapWin.ProjectionDatabase as ProjectionDatabase;
            if (database == null)
                throw new InvalidCastException("Invalid instance of projection database was passed");

            m_mapWin = mapWin;
            LayersControl1.Initialize(mapWin);
            LayersControl1.LayerAdded += delegate(string filename, DataGridView dgv, int rowIndex)
            {
                this.RefreshControlState();
            };
            LayersControl1.LayerRemoved += delegate()
            {
                this.RefreshControlState();
            };
            if (ProjectionTreeView1.Initialize(database, mapWin))
            {
                this.ProjectionTreeView1.RefreshList();
            }

            this.LayersControl1.ControlType = LayersControl.CustomType.Projection;

            this.RefreshControlState();
        }

        /// <summary>
        /// Updates the state of test button
        /// </summary>
        private void RefreshControlState()
        {
            btnTest.Enabled = LayersControl1.SelectedFilename != "";
            btnOk.Enabled = LayersControl1.SelectedFilename != "";
        }

        #region Assign projection
        /// <summary>
        /// Runs the operation
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            CoordinateSystem cs = this.ProjectionTreeView1.SelectedCoordinateSystem;
            if (cs == null)
            {
                MessageBox.Show("No projection is selected", m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (this.LayersControl1.Filenames.Count() == 0)
                {
                    MessageBox.Show("No files are selected", m_mapWin.ApplicationInfo.ApplicationName,
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MapWinGIS.GeoProjection projection = new MapWinGIS.GeoProjection();
                    if (projection.ImportFromEPSG(cs.Code))
                    {
                        frmTesterReport report = new frmTesterReport();
                        int count = 0;  // number of successfully processed files

                        foreach (string name in this.LayersControl1.Filenames)
                        {
                            LayerSource layer = new LayerSource(name);
                            string projName = layer.Projection != null ? layer.Projection.Name : "";
                            if (layer.Type != LayerSourceType.Undefined)
                            {
                                layer.Projection = projection;
                                count++;
                            }
                            else
                            {
                                report.AddFile(name, projName, ProjectionOperaion.Skipped, "");
                            }
                        }

                        if (count > 0)
                        {
                            MessageBox.Show(string.Format("The projection was successfully assigned to the files: {0}", count), m_mapWin.ApplicationInfo.ApplicationName,
                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (report.MismatchedCount > 0)
                        {
                            report.ShowReport(projection, "The following files were not processed:", ReportType.Assignment);
                        }
                        this.LayersControl1.UpdateProjections();
                    }
                    else
                    {
                        MessageBox.Show("Failed to initialize the selected projection", m_mapWin.ApplicationInfo.ApplicationName,
                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        #endregion

        #region Testing
        /// <summary>
        /// Tries to assign the projection and reproject the data back to WGS 84 to make sure
        /// that it is placed right on the workld map
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            string filename = LayersControl1.SelectedFilename;

            if (filename == "")
            {
                MessageBox.Show("No file is selected", m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // initializing target projection
            CoordinateSystem cs = this.ProjectionTreeView1.SelectedCoordinateSystem;
            if (cs == null)
            {
                MessageBox.Show("No projection is selected", m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
            if (!proj.ImportFromEPSG(cs.Code))
            {
                MessageBox.Show("Failed to initialize projection: " + cs.Name, "Unsupported projection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                frmProjectionResults form = new frmProjectionResults(m_mapWin);
                if (form.Assign(filename, proj))
                {
                    form.ShowDialog(this);
                }
                form.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// Shows the selected projection as a label
        /// </summary>
        private void ProjectionTreeView1_CoordinateSystemSelected(Territory cs)
        {
            this.lblProjection.Text = cs != null ? "Projection: " + cs.Name : "";
        }
    }
}
