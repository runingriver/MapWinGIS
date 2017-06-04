/****************************************************************************
 * 文件名:clsReports.cs （F）
 * 描  述:提供给插件获取图层图片的方法。包括获取legend中图层图像和scalebar图像。
 * **************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace MapWinGIS.MainProgram
{
    public class Reports : MapWinGIS.Interfaces.Reports
    {
        #region Reports 接口实现

        /// <summary>
        /// 返回一个（north arrow）指北针的图像
        /// </summary>
        public System.Drawing.Image GetNorthArrow()
        {
            return null;
        }

        /// <summary>
        /// 在指定的范围返回一个MapWinGIS.Image类型的对象的视角
        /// </summary>
        /// <param name="boundBox">The area that you wish to take the picture of.  Uses projected map units.</param>
        public MapWinGIS.Image GetScreenPicture(MapWinGIS.Extents boundBox)
        {
            try
            {
                return (MapWinGIS.Image)Program.frmMain.MapMain.SnapShot(boundBox);
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回一张高质量的闪照图片
        /// </summary>
        /// <param name="layerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="width">Maximum width of the image.  The height of the image depends on the coloring scheme of the layer.</param>
        /// <param name="columns">The number of columns to generate</param>
        /// <param name="fontFamily">Font family</param>
        /// <param name="minFontSize">Minimum font size</param>
        /// <param name="maxFontSize">Maximum Font size</param>
        /// <param name="underlineLayerTitles"></param>
        /// <param name="boldLayerTitles"></param>
        /// <returns></returns>
        public System.Drawing.Image GetLegendSnapshotHQ(int layerHandle, int width, int columns, string fontFamily, int minFontSize, int maxFontSize, bool underlineLayerTitles, bool boldLayerTitles)
        {
            try
            {
                return Program.frmMain.Legend.SnapshotHQ(layerHandle, width, columns, fontFamily, minFontSize, maxFontSize, underlineLayerTitles, boldLayerTitles);
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回legend中的一个指定的层和其中的color breaks的一张高质量的闪照图片
        /// </summary>
        /// <param name="layerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="category">The color break to use</param>
        /// <param name="width">Width in pixels of the box to create</param>
        /// <param name="height">Height in pixels of the box to create</param>
        /// <returns></returns>
        public System.Drawing.Image GetLegendSnapshotBreakHQ(int layerHandle, int category, int width, int height)
        {
            try
            {
                System.Drawing.Bitmap tempBitmap = new System.Drawing.Bitmap(width, height);
                Graphics g = Graphics.FromImage(tempBitmap);
                Program.frmMain.Legend.DrawHQLayerSymbolBreaks(g, layerHandle, category, 0, 0, width, height);
                g.Dispose();
                return tempBitmap;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回legend中的一张图片
        /// </summary>
        /// <param name="visibleLayersOnly">指定只截取可见的层的部分</param>
        /// <param name="imgWidth">图片最大宽度，高度取决于已加载层的数量</param>
        public System.Drawing.Image GetLegendSnapshot(bool visibleLayersOnly, int imgWidth)
        {
            try
            {
                return Program.frmMain.Legend.Snapshot(visibleLayersOnly, imgWidth);
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 类似于GetLegendSnapshot，此时不应只只考虑一个layer在内
        /// </summary>
        /// <param name="LayerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="imgWidth">图片最大宽度，高度取决于已加载层的颜色配置</param>
        public System.Drawing.Image GetLegendLayerSnapshot(int layerHandle, int imgWidth)
        {
            try
            {
                return Program.frmMain.Legend.Layers.ItemByHandle(layerHandle).Snapshot(imgWidth);
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// 包含精确比例尺的图片
        /// </summary>
        /// <param name="mapUnits">采用的地图的单位</param>
        /// <param name="scalebarUnits">显示在比例尺上的尺量单位，能够转换成地图的任何单位</param>
        /// <param name="maxWidth">含比例尺照片最大宽度</param>
        public System.Drawing.Image GetScaleBar(Interfaces.UnitOfMeasure mapUnits, Interfaces.UnitOfMeasure scalebarUnits, int maxWidth)
        {
            try
            {
                ScaleBarUtils sb = new ScaleBarUtils();
                System.Drawing.Image img;
                img = sb.GenerateScaleBar((MapWinGIS.Extents)(Program.frmMain.MapMain.Extents), mapUnits, scalebarUnits, maxWidth, Color.White, Color.Black);
                return img;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }

        }

        /// <summary>
        /// 包含精确比例尺的图片
        /// </summary>
        /// <param name="MapUnits">采用的地图的单位</param>
        /// <param name="ScalebarUnits">显示在比例尺上的尺量单位，能够转换成地图的任何单位</param>
        /// <param name="MaxWidth">含比例尺图片最大宽度</param>
        public System.Drawing.Image GetScaleBar(string mapUnits, string scalebarUnits, int maxWidth)
        {
            try
            {
                ScaleBarUtils sb = new ScaleBarUtils();
                System.Drawing.Image img;
                img = sb.GenerateScaleBar((MapWinGIS.Extents)(Program.frmMain.MapMain.Extents), StringToUOM(mapUnits), StringToUOM(scalebarUnits), maxWidth, Color.White, Color.Black);
                return img;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 将字符串转换成Scalebar支持的单位
        /// </summary>
        public static Interfaces.UnitOfMeasure StringToUOM(string inStr)
        {
            switch (inStr.ToLower())
            {
                case "centimeters":
                    return Interfaces.UnitOfMeasure.Centimeters;
                case "decimaldegrees":
                case "longlat":
                case "latlong":
                case "lat/long":
                case "long/lat":
                    return Interfaces.UnitOfMeasure.DecimalDegrees;
                case "feet":
                    return Interfaces.UnitOfMeasure.Feet;
                case "inches":
                    return Interfaces.UnitOfMeasure.Inches;
                case "kilometers":
                    return Interfaces.UnitOfMeasure.Kilometers;
                case "meters":
                    return Interfaces.UnitOfMeasure.Meters;
                case "miles":
                    return Interfaces.UnitOfMeasure.Miles;
                case "millimeters":
                    return Interfaces.UnitOfMeasure.Millimeters;
                case "yards":
                    return Interfaces.UnitOfMeasure.Yards;
                default:
                    return Interfaces.UnitOfMeasure.Meters;
            }
        }

    } //10
}
