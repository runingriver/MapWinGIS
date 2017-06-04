
namespace MapWinGIS.Controls.Projections
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Collections;
    #endregion

    /// <summary>
    /// Provides GUI for identification of unknown projection (finding exiting EPSG code for it)
    /// </summary>
    public partial class frmIdentifyProjection : Form
    {
        /// Refrence to MapWinGIS
        MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        // Bounds to compare projection
        MapWinGIS.Extents m_bounds = null;

        // prevents undesired events on loading        
        private bool m_noEvents = false;

        #region Initilization
        /// <summary>
        /// Creates a new instance of the frmIdentifyProjection class
        /// </summary>
        public frmIdentifyProjection(MapWinGIS.Interfaces.IMapWin mapWin): this(mapWin, null)
        {
        }

        /// <summary>
        /// Constructor with bounds
        /// </summary>
        public frmIdentifyProjection(MapWinGIS.Interfaces.IMapWin mapWin, MapWinGIS.Extents bounds)
        {
            InitializeComponent();

            if (mapWin == null)
            {
                throw new NullReferenceException("No reference to MapWinGIS was provided");
            }

            m_mapWin = mapWin;
            m_bounds = bounds;

            m_noEvents = true;
            cboLayer.DataSource = m_mapWin.Layers.Where(l => !l.HideFromLegend).ToList();
            cboLayer.DisplayMember = "Name";
            cboLayer.ValueMember = "Handle";
            m_noEvents = false;

            if (cboLayer.Items.Count > 0)
            {
                cboLayer.SelectedIndex = 0;
                cboLayer_SelectedIndexChanged(null, null);
            }
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Displays projection for the layer
        /// </summary>
        private void cboLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_noEvents)
                return;

            MapWinGIS.Interfaces.Layer layer = m_mapWin.Layers[(int)cboLayer.SelectedValue];
            if (layer != null)
            {
                textBox1.Text = layer.Projection;
            }
        }

        /// <summary>
        /// Shows properties for the selected CS
        /// </summary>
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                CoordinateSystem cs = (CoordinateSystem)listBox1.SelectedItem;
                frmProjectionProperties form = new frmProjectionProperties(cs, m_mapWin.ProjectionDatabase as ProjectionDatabase);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // do something
                }
                form.Dispose();
            }
        }
        #endregion

        #region Identification
        /// <summary>
        /// Starts identification
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                Globals.MessageBoxInformation("No input projection is specified", "Enter projection");
                return;
            }
            
            MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
            if (!proj.ImportFromProj4(textBox1.Text))
            {
                if (!proj.ImportFromWKT(textBox1.Text))
                {
                    if (!proj.ImportFromESRI(textBox1.Text))
                    {
                        Globals.MessageBoxInformation("The string can't be identified as one of the following formats: proj4, OGC WKT, ESRI WKT", "Invalid projection");
                        return;
                    }
                }
            }

            ProjectionDatabase db = m_mapWin.ProjectionDatabase as ProjectionDatabase;
            if (db == null)
            {
                Globals.MessageBoxInformation("Projection database wasn't loaded", "Database inaccessible");
                return;
            }

            listBox1.Items.Clear();
                
            CoordinateSystem cs = db.GetCoordinateSystem(proj, ProjectionSearchType.UseDialects);
            if (cs != null)
            {
                // easy case - it was found by name
                listBox1.Items.Add(cs);
                Globals.MessageBoxExlamation("Projection was identified.", "Success");
                return;
            }

            if (listBox1.Items.Count == 0 || !chkBreak.Checked)
            {
                Cursor oldCursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // difficult one
                    MapWinGIS.GeoProjection projTest = new MapWinGIS.GeoProjection();
                    bool isSame = false;

                    if (proj.IsGeographic)
                    {
                        foreach (GeographicCS gcs in db.GeographicCS)
                        {
                            if (projTest.ImportFromProj4(gcs.proj4))
                            {
                                if (m_bounds != null)
                                {
                                    isSame = projTest.get_IsSameExt(proj, m_bounds, 6);
                                }
                                else
                                {
                                    isSame = projTest.get_IsSame(proj);
                                }
                                if (isSame)
                                {
                                    listBox1.Items.Add(gcs);
                                    if (chkBreak.Checked)
                                        break;
                                }
                            }
                        }
                    }
                    else if (proj.IsProjected)
                    {
                        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        int count = 0;
                        foreach (ProjectedCS pcs in db.ProjectedCS)
                        {
                            
                            if (projTest.ImportFromProj4(pcs.proj4))
                            {
                                count++;
                                if (m_bounds != null)
                                {
                                    isSame = projTest.get_IsSameExt(proj, m_bounds, 6);
                                }
                                else
                                {
                                    isSame = projTest.get_IsSame(proj);
                                }

                                if (isSame)
                                {
                                    listBox1.Items.Add(pcs);
                                    if (chkBreak.Checked)
                                        break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this.Cursor = oldCursor;
                }
            }                    

            if (listBox1.Items.Count == 0)
            {
                Globals.MessageBoxExlamation("Projection isn't present in the database", "Unknown projection");
            }
            else
            {
                // Globals.MessageBoxExlamation("Projection wasn't identified. One of the listed projections could be the right one", "Results");
              Globals.MessageBoxExlamation("Projection was identified. One of the listed projections should be the right one", "Results");
            }
        }
        #endregion

        #region Additional
        /// <summary>
        /// Adds proj4 string for database
        /// </summary>
        private void btnUpdateDb_Click(object sender, EventArgs e)
        {
            ProjectionDatabase db = (ProjectionDatabase)m_mapWin.ProjectionDatabase;
            if (db != null)
            {
                db.UpdateProj4Strings(db.Name);
            }
        }
        #endregion
    }
}
