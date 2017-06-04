/****************************************************************************
 * 文件名:clsSelectedShape.cs （F）
 * 描  述: 提供选择的shape的信息。包括shape的Extents、Add、color、index
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 一个选择的shape对象。
    /// </summary>
    public class SelectedShape : MapWinGIS.Interfaces.SelectedShape
    {
        /// <summary>
        /// shape在地图中的实际索引
        /// </summary>
        private int m_ShapeIndex;

        private uint m_OriginalColor;
        private bool m_OriginalDrawFill;
        private float m_OriginalTransparency;
        private uint m_OriginalOutlineColor;


        public void Add(int shapeIndex)
        {
            m_ShapeIndex = shapeIndex;
        }


        /// <summary>
        /// 初始化所有信息，然后高亮显示shape
        /// </summary>
        /// <param name="ShapeIndex">Index of the shape in the shapefile.</param>
        /// <param name="SelectColor">Color to use when highlighting the shape.</param>
        public void Add(int ShapeIndex, System.Drawing.Color SelectColor)
        {
            MapWinGIS.Shapefile tShpObj;
            int curLyr = Program.frmMain.Legend.SelectedLayer;
            if (Program.frmMain.Legend.SelectedLayer == -1)
            {
                return;
            }
            tShpObj = (MapWinGIS.Shapefile)(Program.frmMain.MapMain.get_GetObject(curLyr));
            if (tShpObj == null)
            {
                return;
            }

            m_ShapeIndex = ShapeIndex;
            tShpObj.ShapeSelected[ShapeIndex] = true;
        }

        /// <summary>
        /// 返回选择的shape的范围
        /// </summary>
        public MapWinGIS.Extents Extents 
        {
            get 
            {
                MapWinGIS.Shapefile tShpObj;
                int tLyr;

                if (Program.frmMain.Legend.SelectedLayer == -1)
                {
                    return null;
                }

                tLyr =Program.frmMain.Legend.SelectedLayer;

                tShpObj = (MapWinGIS.Shapefile)(Program.frmMain.MapMain.get_GetObject(tLyr));
                if (tShpObj == null)
                {
                    return null;
                }

                return tShpObj.Shape[m_ShapeIndex].Extents;
            }
        }

        /// <summary>
        /// 返回所选择的shape的索引
        /// </summary>
        public int ShapeIndex 
        {
            get
            {
                return m_ShapeIndex;
            }
        }

        /// <summary>
        /// 最初的颜色
        /// </summary>
        internal uint OriginalColor
        {
            get
            {
                return m_OriginalColor;
            }
        }

        /// <summary>
        /// 是否填充
        /// </summary>
        internal bool OriginalDrawFill
        {
            get
            {
                return m_OriginalDrawFill;
            }
        }
        /// <summary>
        /// 最初的透明度
        /// </summary>
        internal float OriginalTransparency
        {
            get
            {
                return m_OriginalTransparency;
            }
        }
        /// <summary>
        /// 最初的轮廓色
        /// </summary>
        internal uint OriginalOutlineColor
        {
            get
            {
                return m_OriginalOutlineColor;
            }
        }

    }
}
