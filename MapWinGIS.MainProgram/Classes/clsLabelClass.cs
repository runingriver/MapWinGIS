using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace MapWinGIS.MainProgram
{
    internal class LabelInfo
    {
        //成员变量
        public double scale;
        public MapWinGIS.Extents extents;
        public bool UseMinZoomLevel;

        public LabelInfo(bool UseZoomLevel, MapWinGIS.Extents ex)
        {
            extents = ex;
            scale = Program.frmMain.ExtentsToScale(extents);
            UseMinZoomLevel = UseZoomLevel;
        }

        public LabelInfo(bool UseZoomLevel, double xMin, double yMin, double xMax, double yMax)
        {
            extents = new MapWinGIS.Extents();
            extents.SetBounds(xMin, yMin, 0, xMax, yMax, 0);
            scale = Program.frmMain.ExtentsToScale(extents);
            UseMinZoomLevel = UseZoomLevel;
        }
    }

    internal class LabelClass
    {
        private class Point
        {
            public double x;
            public double y;

            public Point(double newX, double newY)
            {
                x = newX;
                y = newY;
            }

            //极端当前点到目标点的距离
            public double Dist(Point p)
            {
                return Math.Sqrt(Math.Pow((y - p.y), 2) + Math.Pow((x - p.x), 2));
            }

        }

        private Hashtable m_Layers;

        public LabelClass()
        {
            m_Layers = new Hashtable();
        }

        /// <summary>
        /// 未实现
        /// Loads .lbl file
        /// </summary>
        public bool LoadLabelInfo(MapWinGIS.MainProgram.Layer layer, System.Windows.Forms.Form owner, string OverrideFilename = "")
        {
            return false;
        }
        /// <summary>
        /// 未实现
        /// </summary>
        public void TestLabelZoomExtents()
        {
        }
    }
}
