
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
    using MapWinGIS.Controls.General;
    using System.Collections;

    public partial class frmReproject : Form
    {
        // Reference to MapWinGIS
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        /// <summary>
        /// Creates a new instance of the frmAssignProjection class
        /// </summary>
        public frmReproject(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();
            
            if (mapWin == null)
                throw new ArgumentException("No reference to MapWinGIS was passed");

            ProjectionDatabase database = mapWin.ProjectionDatabase as ProjectionDatabase;
            if (database == null)
                throw new InvalidCastException("Invalid instance of projection database was passed");

            m_mapWin = mapWin;
            LayersControl1.Initialize(mapWin);
           
            if (ProjectionTreeView1.Initialize(database, mapWin))
            {
                this.ProjectionTreeView1.RefreshList();
            }

            this.LayersControl1.ControlType = LayersControl.CustomType.Projection;
        }

        /// <summary>
        /// Checks user input to start transformation
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            CoordinateSystem cs = this.ProjectionTreeView1.SelectedCoordinateSystem;
            if (cs == null)
            {
                MessageBox.Show("No projection is selected", m_mapWin.ApplicationInfo.ApplicationName,
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IEnumerable<string> filenames = LayersControl1.Filenames;
            if (filenames.Count() == 0)
            {
                MessageBox.Show("No files are selected", m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
            if (!proj.ImportFromEPSG(cs.Code))
            {
                MessageBox.Show("Failed to initialize the selected projection", m_mapWin.ApplicationInfo.ApplicationName,
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            this.DoReprojection(filenames, proj, false);
        }

        /// <summary>
        /// Does the reprojection work
        /// </summary>
        private void DoReprojection(IEnumerable<string> filenames, MapWinGIS.GeoProjection projection, bool inPlace)
        {
            frmTesterReport report = new frmTesterReport();
            report.InitProgress(projection);
            List<string> files = new List<string>();

            int count = 0;  // number of successfully reprojected shapefiles
            foreach (string filename in filenames)
            {
                LayerSource layer = new LayerSource(filename);
                LayerSource layerNew = null;
                
                if (projection.get_IsSame(layer.Projection))
                {
                    report.AddFile(layer.Filename, projection.Name, ProjectionOperaion.SameProjection, "");
                    files.Add(layer.Filename);
                }
                else
                {
                    TestingResult result = CoordinateTransformation.ReprojectLayer(layer, out layerNew, projection, report);
                    if (result == TestingResult.Ok || result == TestingResult.Substituted)
                    {
                        ProjectionOperaion oper = result == TestingResult.Ok ? ProjectionOperaion.Reprojected : ProjectionOperaion.Substituted;
                        string newName = layerNew == null ? "" : layerNew.Filename;
                        report.AddFile(layer.Filename, layer.Projection.Name, oper, newName);
                        files.Add(newName == "" ? layer.Filename : newName);
                        count++;
                    }
                    else
                    {
                        ProjectionOperaion operation = result == TestingResult.Error ? ProjectionOperaion.FailedToReproject : ProjectionOperaion.Skipped;
                        report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.Skipped, "");
                    }
                }
                
                layer.Close();
                if (layerNew != null)
                {
                    layerNew.Close();
                }
            }
            report.ShowReport(projection, "Reprojection results:", ReportType.Loading);

            IEnumerable<string> names = m_mapWin.Layers.Select(l => l.FileName);
            names = files.Except(names);

            if (count > 0)
            {
                if (projection.get_IsSame(m_mapWin.Project.GeoProjection))
                {
                    if (names.Count() > 0)
                    {
                        if (MessageBox.Show("Do you want to add layers to the project?", m_mapWin.ApplicationInfo.ApplicationName,
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            m_mapWin.Layers.StartAddingSession();

                            foreach (string filename in names)
                            {
                                m_mapWin.Layers.Add(filename);
                            }
                            m_mapWin.Layers.StopAddingSession();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No files to add to the map.",
                                        m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Chosen projection is different from the project one. The layers can't be added to map.",
                                    m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No files to add to the map.",
                                m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Updating label
        /// </summary>
        private void ProjectionTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CoordinateSystem cs = ProjectionTreeView1.SelectedCoordinateSystem;
            lblProjection.Text = cs == null ? "" : "Projection: " + cs.Name;
        }
    }
}
