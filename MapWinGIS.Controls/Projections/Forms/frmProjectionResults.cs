
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
    /// Displays results of assigning projection
    /// </summary>
    public partial class frmProjectionResults : Form
    {
        #region Declarations
        // data grid view columns
        private const int CMN_FILENAME = 0;
        private const int CMN_COMMENTS = 1;

        // a handle of the reprojected layer on the map
        private int m_layerHandle = -1;

        // refernce to MapWinGIS
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        // shapefile that was reprojected
        private MapWinGIS.Shapefile m_shapefile = null;
        #endregion

        /// <summary>
        /// Creates a new instance of the frmProjectionResults class
        /// </summary>
        public frmProjectionResults(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();

            if (mapWin == null)
                throw new ArgumentException("Reference to MapWinGIS wasn't passed.");

            m_mapWin = mapWin;

            projectionMap1.LoadStateFromExeName(Application.ExecutablePath);
            projectionMap1.CoordinatesChanged += new ProjectionMap.CoordinatesChangedDelegate(projectionMap1_CoordinatesChanged);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //dgv.SelectionChanged += new EventHandler(dgv_SelectionChanged);
        }

        /// <summary>
        /// Displays map coordinate when a mouse moves
        /// </summary>
        void projectionMap1_CoordinatesChanged(double x, double y, string textX, string textY)
        {
            lblX.Text = "X: " + textX + "deg.";
            lblY.Text = "Y: " + textY + "deg.";
        }

        /// <summary>
        /// Destructor. Closes all the opened shapefiles
        /// </summary>
        ~frmProjectionResults()
        {
            projectionMap1.RemoveAllLayers();
            if (m_shapefile != null)
            {
                m_shapefile.Close();
                m_shapefile = null;
            }
            //foreach (DataGridViewRow row in dgv.Rows)
            //{
            //    if (row.Tag != null)
            //    {
            //        MapWinGIS.Shapefile sf = row.Tag as MapWinGIS.Shapefile;
            //        if (sf != null)
            //        {
            //            sf.Close();
            //            sf = null;
            //        }
            //    }
            //}
        }
        
        /// <summary>
        /// Assignes selected projection and displays results
        /// </summary>
        public bool Assign(string filename, MapWinGIS.GeoProjection proj) 
        {
            MapWinGIS.GeoProjection projWGS84 = new MapWinGIS.GeoProjection();
            if (!projWGS84.ImportFromEPSG(4326))
            {
                MessageBox.Show("Failed to initialize WGS84 coordinate system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            bool sameProj = proj.get_IsSame(projWGS84);
            int count = 0;
            
            bool success = false;
            int rowIndex = dgv.Rows.Add();

            m_shapefile = new MapWinGIS.Shapefile();
            if (m_shapefile.Open(filename, m_mapWin.Layers as MapWinGIS.ICallback))
            {
                this.Text = "Assigning projection: " + System.IO.Path.GetFileName(filename);
                this.lblProjection.Text = "Projection: " + proj.Name;
                
                // it will be faster to assing new instance of class
                // as ImportFromEPSG() is slow according to GDAL documentation
                m_shapefile.GeoProjection = proj;

                if (!sameProj)
                {
                    // we can't show preview on map without reprojection
                    if ((m_shapefile.StartEditingShapes(true, null)))
                    {
                        if (m_shapefile.ReprojectInPlace(projWGS84, ref count))
                            success = true;
                    }
                }
                else
                {
                    success = true;
                }
            }

            if (success)
            {
                this.AddShapefile(m_shapefile);
                return true;
            }
            else
            {
                // no success in reprojection
                m_shapefile.Close();
                return false;
            }
        }

        /// <summary>
        /// Displaying selected layer on the map
        /// </summary>
        private void AddShapefile(MapWinGIS.Shapefile sf)
        {
            if (sf != null)
            {
                m_layerHandle = projectionMap1.AddLayer(sf, true);
                MapWinGIS.Utils utils = new MapWinGIS.Utils();
                MapWinGIS.ShapeDrawingOptions options = sf.DefaultDrawingOptions;
                options.FillColor = utils.ColorByName(MapWinGIS.tkMapColor.Orange);
                options.FillTransparency = 110;
                options.LineColor = utils.ColorByName(MapWinGIS.tkMapColor.Orange);
                projectionMap1.ZoomToLayer(m_layerHandle);
            }
        }
    }
}
