
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MapWinGIS;

namespace MapWinGIS.Controls.Projections
{


    /// <summary>
    /// Performs transformation of shaefiles, images and grids from one coordinate system to another
    /// </summary>
    public class CoordinateTransformation
    {
        #region Reprojection generic
        /// <summary>
        /// Reprojects layer of undefined type
        /// </summary>
        /// <param name="filename">Filename of layer to reproject. A changed name will be returned when new file was created</param>
        /// <param name="projection">New projection</param>
        /// <param name="inPlace">Whether new files should be written</param>
        /// <param name="report">A reference to report form</param>
        /// <returns>True on success and false otherwise</returns>
        public static TestingResult ReprojectLayer(string filename, out string newFilename, MapWinGIS.GeoProjection projection, frmTesterReport callback)
        {
            newFilename = "";
            LayerSource source = new LayerSource(filename, callback);

            if (source.Type != LayerSourceType.Undefined)
            {
                LayerSource sourceNew = null;
                TestingResult result = CoordinateTransformation.ReprojectLayer(source, out sourceNew, projection, callback);
                if (sourceNew != null)
                    newFilename = sourceNew.Filename;

                return result;
            }
            else
            {
                return TestingResult.Error;
            }
        }

        /// <summary>
        /// Reprojects layer of undefined type
        /// </summary>
        /// <param name="filename">Filename of layer to reproject. A changed name will be returned when new file was created</param>
        /// <param name="projection">New projection</param>
        /// <param name="inPlace">Whether new files should be written</param>
        /// <param name="report">A reference to report form</param>
        /// <returns>True on success and false otherwise</returns>
        public static TestingResult ReprojectLayer(LayerSource layer, out LayerSource newLayer, MapWinGIS.GeoProjection projection, frmTesterReport report)
        {
            newLayer = null;
            TestingResult result = TestingResult.Error;
            switch (layer.Type)
            {
                case LayerSourceType.Shapefile:
                    MapWinGIS.Shapefile sfNew = null;
                    result = CoordinateTransformation.Reproject(layer.Shapefile, out sfNew, projection, report);
                    if (sfNew != null)
                        newLayer = new LayerSource(sfNew);
                    break;

                case LayerSourceType.Grid:
                    MapWinGIS.Grid gridNew = null;
                    result = CoordinateTransformation.Reproject(layer.Grid, out gridNew, projection, report);
                    if (gridNew != null)
                        newLayer = new LayerSource(gridNew);
                    break;

                case LayerSourceType.Image:
                    MapWinGIS.Image imageNew = null;
                    result = CoordinateTransformation.Reproject(layer.Image, out imageNew, projection, report);
                    if (imageNew != null)
                        newLayer = new LayerSource(imageNew);
                    break;

                default:
                    System.Diagnostics.Debug.Print("Coordinate transformation: unsupported interface");
                    break;

            }
            return result;
        }
        #endregion

        #region Shapefile reprojection
        /// <summary>
        /// Reprojects a shapefile
        /// </summary>
        /// <param name="grid">Shapefile object to reproject</param>
        /// <param name="inPlace">Whether reprojected file should replace the initial one</param>
        /// <returns>True on success and false otherwise</returns>
        public static TestingResult Reproject(MapWinGIS.Shapefile sfSource, out MapWinGIS.Shapefile sfNew, MapWinGIS.GeoProjection projection, frmTesterReport report)
        {
            sfNew = null;

            string origFilename = sfSource.Filename;
            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            int count = 0;

            LayerSource obj = new LayerSource(sfSource);
            LayerSource objNew = null;

            if (CoordinateTransformation.SeekSubstituteFile(obj, projection, out objNew))
            {
                sfNew = objNew.Shapefile;
                return TestingResult.Substituted;
            }

            string newFilename = CoordinateTransformation.FilenameWithProjectionSuffix(origFilename, sfSource.GeoProjection, projection);
            newFilename = CoordinateTransformation.GetSafeNewName(newFilename);

            // settings callback
            MapWinGIS.ICallback callback = sfSource.GlobalCallback;
            if (report != null)
            {
                sfSource.GlobalCallback = report;

                if (!report.Visible)
                    report.InitProgress(projection);

                report.ShowFilename(sfSource.Filename);
            }

            // doing the job
            sf = sfSource.Reproject(projection, ref count);

            // restoring callback
            if (report != null)
            {
                sfSource.GlobalCallback = callback;
                report.ClearFilename();
            }

            if (sf != null && count == sfSource.NumShapes)
            {
                sf.GlobalCallback = sfSource.GlobalCallback;
                bool result = sf.SaveAs(newFilename, null);        // it doesn't close the editing mode
                if (!result)
                {
                    System.Diagnostics.Debug.Print("Error while saving reprojected shapefile: " + sf.get_ErrorMsg(sf.LastErrorCode));
                }
                sfNew = sf;
                return TestingResult.Ok;
            }

            //sf.Close();
            return TestingResult.Error;
        }
        #endregion

        #region Grid reprojection
        /// <summary>
        /// Reprojects a grid
        /// </summary>
        /// <param name="grid">Grid object to reproject</param>
        /// <param name="inPlace">Whether reprojected file should replace the initial one</param>
        /// <returns>True on success and false otherwise</returns>
        public static TestingResult Reproject(MapWinGIS.Grid grid, out MapWinGIS.Grid gridNew, MapWinGIS.GeoProjection projection, frmTesterReport report)
        {
            string sourcePrj = grid.Header.GeoProjection.ExportToProj4();
            string targetPrj = projection.ExportToProj4();
            string origFilename = grid.Filename;
            MapWinGIS.ICallback callback = grid.GlobalCallback;
            gridNew = null;

            LayerSource obj = new LayerSource(grid);
            LayerSource objNew = null;

            if (CoordinateTransformation.SeekSubstituteFile(obj, projection, out objNew))
            {
                gridNew = objNew.Grid;
                return TestingResult.Substituted;
            }

            string newFilename = CoordinateTransformation.FilenameWithProjectionSuffix(origFilename, grid.Header.GeoProjection, projection);
            newFilename = CoordinateTransformation.GetSafeNewName(newFilename);

            // setting callback
            if (report != null)
            {
                if (!report.Visible)
                    report.InitProgress(projection);

                report.ShowFilename(grid.Filename);
            }

            bool result = MapWinGIS.GeoProcess.SpatialReference.ProjectGrid(ref sourcePrj, ref targetPrj, ref origFilename, ref newFilename, true, report);

            if (report != null)
                report.ClearFilename();

            if (!result)
                return TestingResult.Error;

            // TODO: no need to open it if only a name is supposed to be returned
            gridNew = new MapWinGIS.Grid();
            gridNew.Open(newFilename, MapWinGIS.GridDataType.UnknownDataType, false, MapWinGIS.GridFileType.UseExtension, callback);
            gridNew.AssignNewProjection(projection.ExportToProj4());

            return TestingResult.Ok;
        }
        #endregion

        #region Image reprojection
        /// <summary>
        /// Reprojects image
        /// </summary>
        public static TestingResult Reproject(MapWinGIS.Image image, out MapWinGIS.Image imageNew, MapWinGIS.GeoProjection projection, frmTesterReport report)
        {
            MapWinGIS.GeoProjection projImage = new MapWinGIS.GeoProjection();
            projImage.ImportFromProj4(image.GetProjection());

            string sourcePrj = image.GetProjection();
            string targetPrj = projection.ExportToProj4();
            string origFilename = image.Filename;
            MapWinGIS.ICallback callback = image.GlobalCallback;
            imageNew = null;

            LayerSource obj = new LayerSource(image);
            LayerSource objNew = null;

            if (CoordinateTransformation.SeekSubstituteFile(obj, projection, out objNew))
            {
                imageNew = objNew.Image;
                return TestingResult.Substituted;
            }

            string newFilename = CoordinateTransformation.FilenameWithProjectionSuffix(origFilename, projImage, projection);
            newFilename = CoordinateTransformation.GetSafeNewName(newFilename);

            // setting callback
            if (report != null)
            {
                if (!report.Visible)
                    report.InitProgress(projection);

                report.ShowFilename(image.Filename);
            }

            MapWinGIS.GeoProcess.SpatialReference.ProjectImage(sourcePrj, targetPrj, origFilename, newFilename, report);

            if (report != null)
                report.ClearFilename();

            imageNew = new MapWinGIS.Image();
            if (imageNew.Open(newFilename, MapWinGIS.ImageType.USE_FILE_EXTENSION, false, callback))
            {
                return TestingResult.Ok;
            }
            else
            {
                imageNew = null;
                return TestingResult.Error;
            }
        }
        #endregion

        #region Substitute Filename
        /// <summary>
        /// Gets filename with correct projection suffix included in it
        /// It's used to search for substitute file, or to create reprojected file
        /// </summary>
        /// <returns></returns>
        public static string FilenameWithProjectionSuffix(string filename, MapWinGIS.GeoProjection oldProjection, MapWinGIS.GeoProjection newProjection)
        {
            if (String.IsNullOrEmpty(filename))
                return filename;

            // projection name will be included in file name
            string oldProjName = oldProjection.Name;
            oldProjName = oldProjName.Replace(@"/", "-");

            string testName = "";
            string extention = System.IO.Path.GetExtension(filename);
            if (filename.EndsWith("." + oldProjName + extention, StringComparison.OrdinalIgnoreCase))
            {
                // in case current projection included in file name already, it should be be removed
                testName = filename.Substring(0, filename.Length - oldProjName.Length - extention.Length - 1);
            }
            else
            {
                // otherwise take the whole name
                testName = Path.GetDirectoryName(filename) + @"\" + Path.GetFileNameWithoutExtension(filename);
            }

            string newProjName = newProjection.Name.Replace(@"/", @"-");
            return testName = testName + "." + newProjName + extention;
        }

        /// <summary>
        /// Projection name is written as suffix to the filename in case of reprojection.
        /// Let's try to find file with the same name, but correct suffix for projection.
        /// It's assumed here that projection for the layer passed doesn't match the project one.
        /// </summary>
        public static bool SeekSubstituteFile(LayerSource layer, MapWinGIS.GeoProjection targetProjection, out LayerSource newLayer)
        {
            newLayer = null;
            string testName = CoordinateTransformation.FilenameWithProjectionSuffix(layer.Filename, layer.Projection, targetProjection);

            if (File.Exists(testName))
            {
                LayerSource layerTest = new LayerSource(testName);
                if (layerTest.Type != layer.Type)
                    return false;

                FileInfo f1 = new FileInfo(layer.Filename);
                FileInfo f2 = new FileInfo(testName);

                // the size of .shp files must be exactly the same
                bool equalSize = !(layerTest.Type == LayerSourceType.Shapefile && f1.Length != f2.Length);

                if (layerTest.Projection.get_IsSameExt(targetProjection, layerTest.Extents, 10) && equalSize)
                {
                    newLayer = layerTest;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns new filename and ensures that such file doesn't exist. 
        /// Adds index if necessary.
        /// </summary>
        public static string GetSafeNewName(string filename)
        {
            int index = 0;
            string testName = filename;
            while (File.Exists(testName))
            {
                testName = Path.GetFileNameWithoutExtension(filename) + "_" + index.ToString() + Path.GetExtension(filename);
                index++;
            }
            return testName;
        }
        #endregion
    }
}
