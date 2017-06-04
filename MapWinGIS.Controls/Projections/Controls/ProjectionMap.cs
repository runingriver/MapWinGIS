// ----------------------------------------------------------------------------
// MapWinGIS.Controls.Projections: store controls to work with EPSG projections
// database
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Drawing;
    using System.IO;
    using AxMapWinGIS;

    /// <summary>
    /// A control which encapsulates the loading of World map and drawing the bounds of coordinate systems (projections) on it
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(AxMapWinGIS.AxMap))]
    public class ProjectionMap : AxMapWinGIS.AxMap
    {
        // handle of the layer to display projections
        int m_handleCS = -1;

        // shapefile layer with currently selected bounds
        int m_handleBounds = -1;

        /// <summary>
        /// Delegate for CoordinatesChanged event
        /// </summary>
        public delegate void CoordinatesChangedDelegate(double x, double y, string textX, string textY);
        
        /// <summary>
        /// Event fired when map coordinates are changed
        /// </summary>
        public event CoordinatesChangedDelegate CoordinatesChanged;
        
        /// <summary>
        /// Passes CoordinatesChanged event to all listeners
        /// </summary>
        internal void FireCoordinateChanged(double x, double y, string textX, string textY)
        {
            if (CoordinatesChanged != null)
                CoordinatesChanged(x, y, textX, textY);
        }

        /// <summary>
        /// Creates a new instance of projection map class
        /// </summary>
        public ProjectionMap()
        {
            this.MouseMoveEvent += new _DMapEvents_MouseMoveEventHandler(ProjectionMap_MouseMoveEvent);
        }

        /// <summary>
        /// Fires coordinate changed event
        /// </summary>
        void ProjectionMap_MouseMoveEvent(object sender, _DMapEvents_MouseMoveEvent e)
        {
            double x = 0.0, y = 0.0;
            this.PixelToProj((double)e.x, (double)e.y, ref x, ref y);

            string format = "#.000";
            string sx = (x < -180.000) ? "<180.0" : (x > 180.0) ? ">180.0" : x.ToString(format);
            string sy = (y < -90.0) ? "<90.0" : (y > 90.0) ? ">90.0" : y.ToString(format);
            FireCoordinateChanged(x, y, sx, sy);
        }

        /// <summary>
        /// Loads map state based on relative path, build from executable name.
        /// It's assumed: \EPSG Reference\world.state.
        /// </summary>
        /// <param name="exeName">The filename of executable to build the path to state, MapWinGIS.exe is expected</param>
        /// <returns></returns>
        public bool LoadStateFromExeName(string exeName)
        {
            string path = System.IO.Path.GetDirectoryName(exeName) + @"\Projections\";
            string filename = path + @"world.state";
            return this.LoadStateFromFile(filename);
        }

        /// <summary>
        /// Loads world project
        /// </summary>
        /// <param name="filename">The filename to load map state from</param>
        /// <returns>True on success and false otherwise</returns>
        public bool LoadStateFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                if (!this.LoadMapState(filename, null))
                {
                    System.Diagnostics.Debug.Print("Failed to load map state: " + filename + Environment.NewLine + "Application is closing...");
                    return false;
                }
                else
                {
                    this.ZoomToMaxExtents();
                    this.ShowRedrawTime = false;
                    this.ShowVersionNumber = false;
                    return true;
                }
            }
            else
            {
                System.Diagnostics.Debug.Print("World state file wasn't found: " + filename + Environment.NewLine + "Application is closing...");
                return false;
            }
        }

        /// <summary>
        /// Draws selected bounds on map
        /// </summary>
        /// <param name="ext">Bounding box to search CS</param>
        public void DrawSelectedBounds(MapWinGIS.Extents ext)
        {
            this.RemoveLayer(m_handleBounds);

            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            if (sf.CreateNew("", MapWinGIS.ShpfileType.SHP_POLYGON))
            {
                MapWinGIS.Shape shp = new MapWinGIS.Shape();
                shp.Create(MapWinGIS.ShpfileType.SHP_POLYGON);
                this.InsertPart(shp, ext.xMin, ext.xMax, ext.yMin, ext.yMax);

                int index = 0;
                sf.EditInsertShape(shp, ref index);

                m_handleBounds = this.AddLayer(sf, true);

                MapWinGIS.ShapeDrawingOptions options = sf.DefaultDrawingOptions;
                MapWinGIS.Utils ut = new MapWinGIS.Utils();
                options.FillColor = ut.ColorByName(MapWinGIS.tkMapColor.Orange);
                options.LineColor = ut.ColorByName(MapWinGIS.tkMapColor.Orange);
                options.LineWidth = 3;
                options.LineStipple = MapWinGIS.tkDashStyle.dsDash;
                options.FillTransparency = 100;
            }
            else
                System.Diagnostics.Debug.Print("Failed to create in-memory shapefile");
        }

        /// <summary>
        /// Zooms map to the bounds of coordinate system
        /// </summary>
        /// <param name="cs"></param>
        public void ZoomToCoordinateSystem(Territory cs)
        {
            if (cs == null)
                return;

            MapWinGIS.Extents ext = new MapWinGIS.Extents();
            
            double dx = cs.Right - cs.Left;
            double dy = cs.Top - cs.Bottom;
            if (dx >= 0)
            {
                ext.SetBounds(cs.Left - dx / 4.0, cs.Bottom - dy / 4.0, 0.0, cs.Right + dx / 4.0, cs.Top + dy / 4.0, 0.0);
            }
            else
            {
                dx = 360.0;
                ext.SetBounds(-180.0 - dx / 4.0, cs.Bottom - dy / 4.0, 0.0, 180.0 + dx / 4.0, cs.Top + dy / 4.0, 0.0);
            }
            this.Extents = ext;
        }

        /// <summary>
        /// Zooms map to the bounds of coordinate system
        /// </summary>
        public void ZoomToCoordinateSystem()
        {
            MapWinGIS.Shapefile sf = this.get_Shapefile(m_handleCS);
            if (sf != null)
            {
                MapWinGIS.Extents sfExt = sf.Extents;
                double dx = sfExt.xMax - sfExt.xMin;
                double dy = sfExt.xMax - sfExt.xMin;
                MapWinGIS.Extents ext = new MapWinGIS.Extents();
                ext.SetBounds(sfExt.xMin - dx / 4.0, sfExt.yMin - dy / 4.0, 0.0, sfExt.xMax + dx / 4.0, sfExt.yMax + dy / 4.0, 0.0);
                this.Extents = ext;
            }
        }

        /// <summary>
        /// Draws coordinate system at map
        /// </summary>
        /// <param name="cs">The territory (coordinate system or country) to draw</param>
        public void DrawCoordinateSystem(Territory cs)
        {
            this.RemoveLayer(m_handleCS);

            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            sf.CreateNew("", MapWinGIS.ShpfileType.SHP_POLYGON);
            MapWinGIS.Shape shp = new MapWinGIS.Shape();
            shp.Create(MapWinGIS.ShpfileType.SHP_POLYGON);

            double xMax = cs.Right;
            double xMin = cs.Left;
            double yMin = cs.Bottom;
            double yMax = cs.Top;

            if (xMax < xMin)
            {
                InsertPart(shp, -180, xMax, yMin, yMax);
                InsertPart(shp, xMin, 180, yMin, yMax);
                System.Diagnostics.Debug.Print(shp.NumParts.ToString());
            }
            else
            {
                InsertPart(shp, xMin, xMax, yMin, yMax);
            }

            int shpIndex = sf.NumShapes;
            sf.EditInsertShape(shp, ref shpIndex);

            m_handleCS = this.AddLayer(sf, true);

            MapWinGIS.Utils utils = new MapWinGIS.Utils();
            sf.DefaultDrawingOptions.FillColor = utils.ColorByName(MapWinGIS.tkMapColor.LightBlue);
            sf.DefaultDrawingOptions.FillTransparency = 120;
            sf.DefaultDrawingOptions.LineColor = utils.ColorByName(MapWinGIS.tkMapColor.Blue);
            sf.DefaultDrawingOptions.LineStipple = MapWinGIS.tkDashStyle.dsDash;
            sf.DefaultDrawingOptions.LineWidth = 2;

            this.Redraw();
        }

        /// <summary>
        /// Insers part to polygon based on given rectange
        /// </summary>
        private void InsertPart(MapWinGIS.Shape shp, double xMin, double xMax, double yMin, double yMax)
        {
            int numParts = shp.NumParts;
            shp.InsertPart(shp.numPoints, ref numParts);

            // to left
            int index = shp.numPoints;
            MapWinGIS.Point pnt = new MapWinGIS.Point();
            pnt.x = xMin; pnt.y = yMax;
            shp.InsertPoint(pnt, ref index); index++;

            pnt = new MapWinGIS.Point();
            pnt.x = xMax; pnt.y = yMax;
            shp.InsertPoint(pnt, ref index); index++;

            pnt = new MapWinGIS.Point();
            pnt.x = xMax; pnt.y = yMin;
            shp.InsertPoint(pnt, ref index); index++;

            pnt = new MapWinGIS.Point();
            pnt.x = xMin; pnt.y = yMin;
            shp.InsertPoint(pnt, ref index); index++;

            pnt = new MapWinGIS.Point();
            pnt.x = xMin; pnt.y = yMax;
            shp.InsertPoint(pnt, ref index); index++;

            if (!shp.get_PartIsClockWise(0))
            {
                bool val = shp.ReversePointsOrder(0);
                if (!val)
                {
                    System.Diagnostics.Debug.Print("CCW");
                }
            }

            if (!shp.get_PartIsClockWise(0))
            {
                System.Diagnostics.Debug.Print("CCW");
            }
        }

        /// <summary>
        /// Removes layer with coordinate system
        /// </summary>
        public void ClearCoordinateSystem()
        {
            this.RemoveLayer(m_handleCS);
        }

        /// <summary>
        /// Removes layer with bounds
        /// </summary>
        public void ClearBounds()
        {
            this.RemoveLayer(m_handleBounds);
        }
    }
}