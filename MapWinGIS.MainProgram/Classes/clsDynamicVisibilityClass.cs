/****************************************************************************
 * 文件名:clsDynamicVisibilityClass.cs
 * 描  述:
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MapWinGIS.MainProgram
{
    internal class DynamicVisibilityClass
    {
        private Hashtable ht = new Hashtable();

        private class Point
        {
            public double x;
            public double y;

            public Point(double newX, double newY)
            {
                x = newX;
                y = newY;
            }

            /// <summary>
            /// 计算冲一个点到另外一个点的距离
            /// </summary>
            public double Dist(Point p)
            {
                return Math.Sqrt(Math.Pow((y - p.y), 2) + Math.Pow((x - p.x), 2));
            }

        }

        internal class DVInfo
        {
            private double m_Scale;
            private MapWinGIS.Extents m_Extents;
            private bool m_UseExtents = false;
            private int m_handle;

            public DVInfo(MapWinGIS.Extents Exts, bool UseExts, int LayerHandle)
            {
                this.m_Extents = Exts;
                this.m_Scale = Program.frmMain.ExtentsToScale(m_Extents);
                m_handle = LayerHandle;

                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    m_UseExtents = UseExts;
                }
                else
                {
                    m_UseExtents = false;
                }
            }

            /// <summary>
            /// 未实现
            /// </summary>
            public bool UseDynamicExtents
            {
                get
                {
                    return m_UseExtents;
                }
                set
                {
                    return;
                }
            }

            public MapWinGIS.Extents DynamicExtents
            {
                get
                {
                    return m_Extents;
                }
                set
                {
                    m_Extents = value;
                    m_Scale = Program.frmMain.ExtentsToScale(m_Extents);
                }
            }

            /// <summary>
            /// 未实现
            /// </summary>
            public double DynamicScale
            {
                get
                {
                    return m_Scale;
                }
                set
                {
                    m_Scale = value;
                    //m_Extents = frmMain.ScaleToExtents(m_Scale, frmMain.MapMain.Extents);
                }
            }
        }

        public DVInfo this[int LayerHandle]
        {
            get
            {
                if (!ht.ContainsKey(LayerHandle))
                {
                    MapWinGIS.Extents emptyexts = new MapWinGIS.Extents();
                    emptyexts.SetBounds(0, 0, 0, 0, 0, 0);
                    Add(LayerHandle, emptyexts, false);
                }

                return ((DVInfo)(ht[LayerHandle]));
            }
            set
            {
                if (ht.ContainsKey(LayerHandle))
                {
                    ht[LayerHandle] = value;
                }
                else
                {
                    ht.Add(LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 未实现
        /// </summary>
        public void Add(int LayerHandle, MapWinGIS.Extents Extents, bool FeatureEnabled)
        {
          
        }

        public void Remove(int LayerHandle)
        {
            ht.Remove(LayerHandle);
        }

        public bool Contains(int LayerHandle)
        {
            return ht.Contains(LayerHandle);
        }

        public void Clear()
        {
            ht.Clear();
        }

        /// <summary>
        /// 未实现
        /// </summary>
        internal void TestLayerZoomExtents()
        {
        }
    }
}
