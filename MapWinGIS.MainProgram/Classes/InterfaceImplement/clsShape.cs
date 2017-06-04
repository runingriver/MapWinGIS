/************************************************************************************
 * 文件名:clsShape.cs  (F)
 * 描  述:提供一个图层上所有shape的信息。包括shape的点、线、多边形的绘制、填充、颜色等等。
 *        一个shape对象存储一个图层上的一个shape信息。
 *        宿主内可以获取该shape的shapeindex和layerhandle。
 * **********************************************************************************/

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 图层上的一个shape对象
    /// </summary>
    public class Shape : MapWinGIS.Interfaces.Shape
    {
        private int m_LayerHandle;
        private int m_ShapeIndex;

        public Shape()
        {
            m_LayerHandle = -1;
            m_ShapeIndex = -1;
        }

        internal Shape(int LayerHandle, int ShapeIndex)
        {
            m_LayerHandle = LayerHandle;
            m_ShapeIndex = ShapeIndex;
        }

        #region ------------接口-----------------
        /// <summary>
        /// 移动shape
        /// View.ExtentPad将增加一个小的空间，以便容易看见这个shape
        /// </summary>
        public void ZoomTo()
        {
            if (m_LayerHandle == -1)
            {
                Program.g_error = "ZoomTo: 无效的LayerHandle";
            }
            else if (m_ShapeIndex == -1)
            {
                Program.g_error = "ZoomTo: 无效的ShapeIndex";
            }
            else
            {
                Program.frmMain.MapMain.ZoomToShape(m_LayerHandle, m_ShapeIndex);
                Program.frmMain.m_PreviewMap.UpdateLocatorBox();
            }
        }

        /// <summary>
        /// 获取设置这个shape的颜色,
        /// </summary>
        public System.Drawing.Color Color 
        {
            get
            {
                System.Drawing.Color defaultColor = System.Drawing.Color.Red;
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "Color: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "Color: 无效的ShapeIndex";
                }
                else
                {
                    object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                    if (o == null)
                    {
                        return defaultColor;
                    }

                    if (o is MapWinGIS.Shapefile)
                    {
                        MapWinGIS.Shapefile s;
                        s = (MapWinGIS.Shapefile)o;
                        if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTZ))
                        {
                            return Program.frmMain.MapMain.get_ShapePointColor(m_LayerHandle, m_ShapeIndex);
                        }
                        else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTZ))
                        {
                            return Program.frmMain.MapMain.get_ShapePointColor(m_LayerHandle, m_ShapeIndex);
                        }
                        else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ))
                        {
                            return Program.frmMain.MapMain.get_ShapeFillColor(m_LayerHandle, m_ShapeIndex);
                        }
                        else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINE) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEZ))
                        {
                            return Program.frmMain.MapMain.get_ShapeLineColor(m_LayerHandle, m_ShapeIndex);
                        }
                    }
                    o = null;
                }

                MapWinGIS.Utility.Logger.Message("获取shape的颜色失败！返回默认颜色(Red).");
                return defaultColor;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "Color: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "Color: 无效的ShapeIndex";
                }
                else
                {
                    try
                    {
                        object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                        if (o == null)
                        {
                            return;
                        }

                        if (o is MapWinGIS.Shapefile)
                        {
                            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                            try
                            {
                                Program.frmMain.m_View.ClearSelectedShapes();
                                MapWinGIS.Shapefile s = (MapWinGIS.Shapefile)o;

                                if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTZ))
                                {
                                    Program.frmMain.MapMain.set_ShapePointColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
                                }
                                else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTZ))
                                {
                                    Program.frmMain.MapMain.set_ShapePointColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
                                }
                                else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ))
                                {
                                    Program.frmMain.MapMain.set_ShapeFillColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
                                }
                                else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINE) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEZ))
                                {
                                    Program.frmMain.MapMain.set_ShapeLineColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
                                }
                            }
                            finally
                            {
                                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                            }
                        }
                        o = null;
                    }
                    catch (System.Exception ex)
                    {
                        Program.g_error = ex.Message;
                        Program.ShowError(ex);
                    }
                }
            }
        }

        /// <summary>
        /// 是否填充这个shape
        /// 只能应用于多边形的shape
        /// </summary>
        public bool DrawFill
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "DrawFill: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "DrawFill: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeDrawFill(m_LayerHandle, m_ShapeIndex);
                }
                return false;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "DrawFill: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "DrawFill: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeDrawFill(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// 填充方式
        /// 只能应用于多边形的shape
        /// 可用 的方式如下：
        /// <list type="bullet">
        /// <item>fsCustom</item>
        /// <item>fsDiagonalDownLeft</item>
        /// <item>fsDiagonalDownRight</item>
        /// <item>fsHorizontalBars</item>
        /// <item>fsNone</item>
        /// <item>fsPolkaDot</item>
        /// <item>fsVerticalBars</item>
        /// </list>
        /// </summary>
        public MapWinGIS.tkFillStipple FillStipple 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "FillStipple: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "FillStipple: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeFillStipple(m_LayerHandle, m_ShapeIndex);
                }

                MapWinGIS.Utility.Logger.Message("获取shape的Fill填充方式失败！返回默认填充方式(fsNone).");
                return tkFillStipple.fsNone;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "FillStipple: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "FillStipple: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeFillStipple(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// 获取设置点或线的尺寸.  
        /// 如果PointType是ptUserDefined类型，点或线的尺寸将和用户定义的并联
        /// </summary>
        public float LineOrPointSize 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "LineOrPointSize: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "LineOrPointSize: 无效的ShapeIndex";
                }
                else
                {
                    Interfaces.eLayerType LayerType = Program.frmMain.m_Layers[m_LayerHandle].LayerType;
                    if (LayerType == Interfaces.eLayerType.PointShapefile)
                    {
                        return System.Convert.ToSingle(Program.frmMain.MapMain.get_ShapeLayerPointSize(m_LayerHandle));
                    }
                    else if (LayerType == Interfaces.eLayerType.LineShapefile || LayerType == Interfaces.eLayerType.PolygonShapefile)
                    {
                        return System.Convert.ToSingle(Program.frmMain.MapMain.get_ShapeLayerLineWidth(m_LayerHandle));
                    }
                }
                MapWinGIS.Utility.Logger.Message("获取shape的LineOrPointSize失败！返回-1.");
                return -1;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "LineOrPointSize: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "LineOrPointSize: 无效的ShapeIndex";
                }
                else
                {
                    if (Program.frmMain.m_Layers[m_LayerHandle].LayerType == Interfaces.eLayerType.LineShapefile)
                    {
                        Program.frmMain.MapMain.set_ShapeLineWidth(m_LayerHandle, m_ShapeIndex, value);
                    }
                    else if (Program.frmMain.m_Layers[m_LayerHandle].LayerType == Interfaces.eLayerType.PointShapefile)
                    {
                        Program.frmMain.MapMain.set_ShapePointSize(m_LayerHandle, m_ShapeIndex, value);
                    }
                    else if (Program.frmMain.m_Layers[m_LayerHandle].LayerType == Interfaces.eLayerType.PolygonShapefile)
                    {
                        Program.frmMain.MapMain.set_ShapeLineWidth(m_LayerHandle, m_ShapeIndex, value);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制shape的line的类型
        /// 可用类型如下：
        /// <list type="bullet">
        /// <item>lsCustom</item>
        /// <item>lsDashDotDash</item>
        /// <item>lsDashed</item>
        /// <item>lsDotted</item>
        /// <item>lsNone</item>
        /// </list>
        /// </summary>
        public MapWinGIS.tkLineStipple LineStipple 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "LineStipple: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "LineStipple: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeLineStipple(m_LayerHandle, m_ShapeIndex);

                }

                MapWinGIS.Utility.Logger.Message("获取shape的Line填充方式失败！返回默认填充方式(lsNone).");
                return tkLineStipple.lsNone;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "LineStipple: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "LineStipple: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeLineStipple(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// outline（轮廓） color for this shape.  
        /// Only applies to polygon shapes.
        /// </summary>
        public System.Drawing.Color OutlineColor 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "OutlineColor: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "OutlineColor: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeLineColor(m_LayerHandle, m_ShapeIndex);

                }
                MapWinGIS.Utility.Logger.Message("获取shape的OutlineColor失败！返回默认颜色(Red).");
                return System.Drawing.Color.Red;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "OutlineColor: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "OutlineColor: 无效的ShapeIndex";
                }
                else
                {
                    object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                    if (o == null)
                    {
                        return;
                    }

                    if (o is MapWinGIS.Shapefile)
                    {
                        MapWinGIS.Shapefile s = (MapWinGIS.Shapefile)o;
                        if (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON || s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM || s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ)
                        {
                            Program.frmMain.MapMain.set_ShapeLineColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
                        }

                    }
                    o = null;
                }
            }
        }

        /// <summary>
        /// 点的类型
        /// 可用类型:
        /// <list type="bullet">
        /// <item>ptCircle</item>
        /// <item>ptDiamond</item>
        /// <item>ptImageList</item>
        /// <item>ptSquare</item>
        /// <item>ptTriangleDown</item>
        /// <item>ptTriangleLeft</item>
        /// <item>ptTriangleRight</item>
        /// <item>ptTriangleUp</item>
        /// <item>ptUserDefined</item>
        /// </list>
        /// </summary>
        public MapWinGIS.tkPointType PointType
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "PointType: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "PointType: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapePointType(m_LayerHandle, m_ShapeIndex);

                }
                MapWinGIS.Utility.Logger.Message("获取shape的tkPointType失败！返回默认值（ptCircle）.");
                return tkPointType.ptCircle;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "PointType: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "PointType: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapePointType(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// shape是否可见
        /// </summary>
        public bool Visible 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "Visible: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "Visible: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeVisible(m_LayerHandle, m_ShapeIndex);
                }
                MapWinGIS.Utility.Logger.Message("获取shape的Visible失败！返回默认值（true）.");
                return true;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "Visible: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "Visible: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeVisible(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// 显示顶点
        /// 顶点能够被隐藏
        /// </summary>
        /// <param name="color">顶点颜色</param>
        /// <param name="vertexSize">顶点大小（像素）</param>
        public void ShowVertices(System.Drawing.Color color, int vertexSize)
        {
            if (Program.frmMain.m_Layers[m_LayerHandle].LayerType != Interfaces.eLayerType.PointShapefile)
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                try
                {
                    Program.frmMain.MapMain.set_ShapePointSize(m_LayerHandle, m_ShapeIndex, vertexSize);
                    Program.frmMain.MapMain.set_ShapePointColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(color));
                    Program.frmMain.MapMain.set_ShapeDrawPoint(m_LayerHandle, m_ShapeIndex, true);
                }
                finally
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
            }
        }

        /// <summary>
        /// 隐藏顶点
        /// </summary>
        public void HideVertices()
        {
            if (Program.frmMain.m_Layers[m_LayerHandle].LayerType != Interfaces.eLayerType.PointShapefile)
            {
                Program.frmMain.MapMain.set_ShapeDrawPoint(m_LayerHandle, m_ShapeIndex, false);
            }
        }

        /// <summary>
        /// 从images列表中获取设置图片索引
        /// </summary>
        public long ShapePointImageListID 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapePointImageListID(m_LayerHandle, m_ShapeIndex);
                }
                MapWinGIS.Utility.Logger.Message("获取shape的ShapePointImageListID失败！返回（-1）.");
                return -1;
            }
            set
            { 
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的ShapeIndex";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapePointImageListID(m_LayerHandle, m_ShapeIndex, (int)value);
                }
            }
        }

        /// <summary>
        /// 多边形shape的透明百分比
        /// </summary>
        public float ShapeFillTransparency 
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的ShapeIndex";
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeFillTransparency(m_LayerHandle, m_ShapeIndex);
                }
                MapWinGIS.Utility.Logger.Message("获取shape的ShapePointImageListID失败！返回（0）.");
                return 0;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的LayerHandle";
                }
                else if (m_ShapeIndex == -1)
                {
                    Program.g_error = "ShapePointImageListID: 无效的ShapeIndex";
                }
                else
                {
                   Program.frmMain.MapMain.set_ShapeFillTransparency(m_LayerHandle, m_ShapeIndex, value);
                }
            }
        }

        /// <summary>
        /// shape填充颜色
        /// </summary>
        public System.Drawing.Color ShapeFillColor 
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeFillColor(m_LayerHandle, m_ShapeIndex);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeFillColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
            } 
        }

        /// <summary>
        /// shape的line的颜色
        /// </summary>
        public System.Drawing.Color ShapeLineColor 
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLineColor(m_LayerHandle, m_ShapeIndex);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLineColor(m_LayerHandle, m_ShapeIndex, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// shape的line的宽
        /// </summary>
        public float ShapeLineWidth 
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLineWidth(m_LayerHandle, m_ShapeIndex);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLineWidth(m_LayerHandle, m_ShapeIndex, value);
            }
        }

        #endregion //16

        /// <summary>
        /// 获取或设置选择的shape的图层句柄
        /// </summary>
        internal int LayerHandle
        {
            get
            {
                int returnValue = default(int);
                returnValue = System.Convert.ToInt32(m_LayerHandle);
                return returnValue;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_LayerHandle = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置选择的shape的在地图中的索引
        /// </summary>
        internal int ShapeIndex
        {
            get
            {
                return m_ShapeIndex;
            }
            set
            {
                if (m_ShapeIndex == -1)
                {
                    m_ShapeIndex = value;

                }
                else
                {
                    Program.g_error = "ShapeIndex:  设置shapeIndex后无法修改shape的属性";
                }
            }
        }
    }
}
