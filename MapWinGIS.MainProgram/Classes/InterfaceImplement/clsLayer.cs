/****************************************************************************
 * 文件名:clsLayer.cs (F)
 * 描  述: 提供一个层对象，并提供对该层对象进行操作的各种方法和属性。
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MapWinGIS.MainProgram
{
    public class Layer : MapWinGIS.Interfaces.Layer
    {
        private int m_LayerHandle;
        private MainProgram.Shapes m_Shapes;
        private string m_error;

        public Layer()
        {
            m_LayerHandle = -1;
        }

        #region  ------------------Layer接口实现-------------------------
        /// <summary>
        /// 舍弃的属性
        /// </summary>
        public int LineSeparationFactor
        {
            get
            {
                return Program.frmMain.MapMain.LineSeparationFactor;
            }
            set
            {
                Program.frmMain.MapMain.LineSeparationFactor = value;
            }
        }

        /// <summary>
        /// 添加一个label到当前层
        /// </summary>
        public void AddLabel(string Text, System.Drawing.Color TextColor, double xPos, double yPos, MapWinGIS.tkHJustification Justification)
        {
            Program.frmMain.MapMain.AddLabel(m_LayerHandle, Text, ColorScheme.ColorToUInt(TextColor), xPos, yPos, Justification);
        }

        /// <summary>
        /// 添加一个扩展的label到当前层
        /// </summary>
        public void AddLabelEx(string Text, System.Drawing.Color TextColor, double xPos, double yPos, MapWinGIS.tkHJustification Justification, double Rotation)
        {
            Program.frmMain.MapMain.AddLabelEx(m_LayerHandle, Text, ColorScheme.ColorToUInt(TextColor), xPos, yPos, Justification, Rotation);
        }

        /// <summary>
        /// 从对应的.lbl文件中更新图层的label信息
        /// </summary>
        public void UpdateLabelInfo()
        {
            Program.frmMain.m_Labels.LoadLabelInfo(this, null);
        }

        /// <summary>
        /// 清空这个层上的所有label
        /// </summary>
        public void ClearLabels()
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return;
            }
            Program.frmMain.MapMain.ClearLabels(m_LayerHandle);
        }

        /// <summary>
        /// 设置当前层上所有label的字体
        /// </summary>
        public void Font(string FontName, int FontSize)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return;
            }
            Program.frmMain.MapMain.LayerFont(m_LayerHandle, FontName, FontSize);
        }

        /// <summary>
        /// 设置当前层上所有label的字体
        /// </summary>
        public void Font(string FontName, int FontSize, System.Drawing.FontStyle FontStyle)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return;
            }
            else
            {
                bool isBold = false;
                bool isItalic = false; //斜体
                bool isUnderline = false;
                if (FontStyle == System.Drawing.FontStyle.Bold)
                {
                    isBold = true;
                }
                if (FontStyle == System.Drawing.FontStyle.Italic)
                {
                    isItalic = true;
                }
                if (FontStyle == System.Drawing.FontStyle.Underline)
                {
                    isUnderline = true;
                }
                if (FontStyle == (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))
                {
                    isBold = true;
                    isItalic = true;
                }
                if (FontStyle == (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))
                {
                    isBold = true;
                    isUnderline = true;
                }
                if (FontStyle == (System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))
                {
                    isItalic = true;
                    isUnderline = true;
                }
                if (FontStyle == (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))
                {
                    isBold = true;
                    isItalic = true;
                    isUnderline = true;
                }

                Program.frmMain.MapMain.LayerFontEx(m_LayerHandle, FontName, FontSize, isBold, isItalic, isUnderline);
            }
        }

        /// <summary>
        /// 移动这个层的显示
        /// 把View.ExtentPad考虑在内
        /// </summary>
        public void ZoomTo()
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return;
            }
            Program.frmMain.MapMain.ZoomToLayer(m_LayerHandle);
            Program.frmMain.m_PreviewMap.UpdateLocatorBox();
        }

        /// <summary>
        /// 获取shapefile、Image对象
        /// 如果是Grid对象，则用GetGridObject方法重新检索
        /// </summary>
        public object GetObject()
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return null;
            }
            return Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
        }

        /// <summary>
        /// 在layer列表中将当前层移动到一个新的位置
        /// </summary>
        public void MoveTo(int NewPosition, int TargetGroup)
        {
            if (Program.frmMain.Legend.Groups.IsValidHandle(TargetGroup) == false)
            {
                Program.frmMain.Legend.Layers.MoveLayerWithinGroup(m_LayerHandle, NewPosition);
            }
            else
            {
                Program.frmMain.Legend.Layers.MoveLayer(m_LayerHandle, TargetGroup, NewPosition);
            }
        }

        /// <summary>
        /// shapefile的颜色，只应用与shapefile
        /// 设置shapefile的颜色将清空所有选择的shapes并且重置每个shape为相同的颜色
        /// 同样颜色配置也会被覆盖
        /// </summary>
        public System.Drawing.Color Color
        {
            get
            {
                System.Drawing.Color returnValue = System.Drawing.Color.AliceBlue;
                try
                {
                    if (m_LayerHandle == -1)
                    {
                        m_error = "图层对象未使用有效句柄初始化";
                        return returnValue;
                    }
                    else
                    {
                        object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                        if (o == null)
                        {
                            return returnValue;
                        }

                        if (o is MapWinGIS.Shapefile)
                        {
                            MapWinGIS.Shapefile s = (MapWinGIS.Shapefile)o;
                            if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTZ))
                            {
                                returnValue = Program.frmMain.MapMain.get_ShapeLayerPointColor(m_LayerHandle);
                            }
                            else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINT) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTZ))
                            {
                                returnValue = Program.frmMain.MapMain.get_ShapeLayerPointColor(m_LayerHandle);
                            }
                            else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ))
                            {
                                returnValue = Program.frmMain.MapMain.get_ShapeLayerFillColor(m_LayerHandle);
                            }
                            else if (((s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINE) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEM)) || (s.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEZ))
                            {
                                returnValue = Program.frmMain.MapMain.get_ShapeLayerLineColor(m_LayerHandle);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.Message;
                    Program.ShowError(ex);
                }
                return returnValue;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";

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
                        Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                        try
                        {
                            Program.frmMain.m_View.ClearSelectedShapes();

                            MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)o;
                            if (((sf.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINT) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTM)) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTZ))
                            {
                                Program.frmMain.MapMain.set_ShapeLayerPointColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
                            }
                            else if (((sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POINT) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTM)) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTZ))
                            {
                                Program.frmMain.MapMain.set_ShapeLayerPointColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
                            }
                            else if (((sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM)) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ))
                            {
                                Program.frmMain.MapMain.set_ShapeLayerFillColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
                            }
                            else if (((sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINE) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEM)) || (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEZ))
                            {
                                Program.frmMain.MapMain.set_ShapeLayerLineColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
                            }

                            Program.frmMain.Legend.Refresh();

                        }
                        finally
                        {
                            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                        }
                    }
                    o = null;
                }
            }
        }

        /// <summary>
        /// 是否绘制填充多边形的shapefile
        /// </summary>
        public bool DrawFill
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return false;
                }
                return Program.frmMain.MapMain.get_ShapeLayerDrawFill(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeLayerDrawFill(m_LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 是否层的颜色配置也显示在legend中
        /// </summary>
        public bool Expanded
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return false;
                }
                else
                {
                    return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Expanded;
                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                }
                else
                {
                    Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Expanded = value;
                }
            }
        }

        /// <summary>
        /// 当前选择图层的extents
        /// </summary>
        public MapWinGIS.Extents Extents
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return null;
                }

                MapWinGIS.Extents newExts = new MapWinGIS.Extents();
                object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                try
                {
                    if (o == null)
                    {
                        return null;
                    }

                    if (o is MapWinGIS.Shapefile)
                    {
                        return ((MapWinGIS.Shapefile)o).Extents;
                    }
                    else if (o is MapWinGIS.Image)
                    {
                        MapWinGIS.Image x = (MapWinGIS.Image)o;
                        newExts.SetBounds(x.OriginalXllCenter - (0.5 * x.OriginalDX), x.OriginalYllCenter - (0.5 * x.OriginalDY), 0,
                                          x.OriginalXllCenter - (0.5 * x.OriginalDX) + (x.OriginalWidth * x.OriginalDX), x.OriginalYllCenter - (0.5 * x.OriginalDY) + (x.OriginalHeight * x.OriginalDY), 0);

                        return newExts;

                    }
                    else //可能是一个grid或不支持的格式
                    {
                        return null;
                    }
                }
                finally
                {
                    o = null;
                }
            }
        }

        /// <summary>
        /// 获得层的文件名.  
        /// 如果是memory-based only，也可能没有文件名
        /// </summary>
        public string FileName
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return null;
                }

                object o;
                try
                {
                    if (this.LayerType == Interfaces.eLayerType.Grid)
                    {
                        return Program.frmMain.MapMain.get_GridFileName(m_LayerHandle);
                    }
                    else
                    {
                        o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                        if (o == null)
                        {
                            return null;
                        }

                        if (this.LayerType == Interfaces.eLayerType.Image)
                        {
                            return ((MapWinGIS.Image)o).Filename;
                        }
                        else if (this.LayerType == Interfaces.eLayerType.Invalid)
                        {
                            return null;
                        }
                        else
                        {
                            return ((MapWinGIS.Shapefile)o).Filename;
                        }
                    }
                }
                finally
                {
                    o = null;
                }
            }
        }

        /// <summary>
        /// 获得这个层的projection
        /// projections必须是PROJ4格式
        /// 没有则null
        /// 如果提供的是一个无效的projection，会保存不成功
        /// </summary>
        public string Projection
        {
            get
            {
                try
                {
                    if (m_LayerHandle == -1)
                    {
                        m_error = "图层对象未使用有效句柄初始化";
                        return "";
                    }
                    else
                    {
                        object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                        if (o is MapWinGIS.Grid)
                        {
                            return ((MapWinGIS.Grid)o).Header.Projection;
                        }
                        else if (o is MapWinGIS.Image)
                        {
                            return ((MapWinGIS.Image)o).GetProjection();
                        }
                        else if (o is MapWinGIS.Shapefile)
                        {
                            string projectionstring = ((MapWinGIS.Shapefile)o).Projection;
                            if (projectionstring == "")
                            {
                                MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)o;
                                projectionstring = sf.GeoProjection.ExportToProj4();
                            }
                            return projectionstring;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    if (m_LayerHandle == -1)
                    {
                        m_error = "图层对象未使用有效句柄初始化";
                    }
                    else
                    {
                        object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                        if (o is MapWinGIS.Grid)
                        {
                            ((MapWinGIS.Grid)o).AssignNewProjection(value);
                        }
                        else if (o is MapWinGIS.Image)
                        {
                            ((MapWinGIS.Image)o).SetProjection(value);
                        }
                        else if (o is MapWinGIS.Shapefile)
                        {
                            ((MapWinGIS.Shapefile)o).Projection = value;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 获取设置填充整个shapefile的样式
        /// </summary>
        public MapWinGIS.tkFillStipple FillStipple
        {
            get
            {
                MapWinGIS.tkFillStipple returnValue = MapWinGIS.tkFillStipple.fsNone;
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                }
                else
                {
                    returnValue = Program.frmMain.MapMain.get_ShapeLayerFillStipple(m_LayerHandle);
                }
                return returnValue;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeLayerFillStipple(m_LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 返回层的Grid类型对象，如果没有grid layer则返回Nothing
        /// </summary>
        public MapWinGIS.Grid GetGridObject
        {
            get
            {
                if (Program.frmMain.m_Layers != null && Program.frmMain.m_Layers.m_Grids.ContainsKey(m_LayerHandle))
                {
                    return ((MapWinGIS.Grid)(Program.frmMain.m_Layers.m_Grids[m_LayerHandle]));
                }
                return null;
            }
        }

        /// <summary>
        /// 层的handle，MapWin自动为层设置一个LayerHandle，并且不能被重置清空
        /// </summary>
        public int Handle
        {
            get
            {
                return m_LayerHandle;
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
        /// 设置文本起点到标签的距离（像素）
        /// </summary>
        public int LabelsOffset
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerLabelsOffset(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerLabelsOffset(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 决定标签是否可以按比例改变
        /// </summary>
        public bool LabelsScale
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerLabelsScale(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerLabelsScale(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// label的阴影颜色
        /// </summary>
        public System.Drawing.Color LabelsShadowColor
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerLabelsShadowColor(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerLabelsShadowColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        public void HatchingRecalculate()
        {
            if (!Program.frmMain.m_FillStippleSchemes.ContainsKey(m_LayerHandle) || Program.frmMain.m_FillStippleSchemes[m_LayerHandle] == null || Program.frmMain.m_FillStippleSchemes[m_LayerHandle].NumHatches() == 0)
            {
                if (!(Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).HatchingScheme == null))
                {
                    Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).HatchingScheme.ClearHatches();
                }
                Program.frmMain.MapMain.set_ShapeLayerFillStipple(m_LayerHandle, FillStipple);
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Refresh();
                return;
            }

            object obj = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
            if (obj is MapWinGIS.Shapefile)
            {
                MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)obj;
                Interfaces.ShapefileFillStippleScheme fs = Program.frmMain.m_FillStippleSchemes[m_LayerHandle];
                for (int i = 0; i <= sf.NumShapes - 1; i++)
                {
                    Interfaces.ShapefileFillStippleBreak brk = fs.GetHatch(sf.CellValue[(int)fs.FieldHandle, i].ToString());
                    if (brk != null)
                    {
                        Program.frmMain.MapMain.set_ShapeFillStipple(m_LayerHandle, i, brk.Hatch);
                        Program.frmMain.MapMain.set_ShapeStippleTransparent(m_LayerHandle, i, brk.Transparent);
                        Program.frmMain.MapMain.set_ShapeStippleColor(m_LayerHandle, i, ColorScheme.ColorToUInt(brk.LineColor));
                    }
                }
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Refresh();
            }
        }

        /// <summary>
        /// 标签是否被层所遮盖
        /// </summary>
        public bool LabelsShadow
        {
            get
            {
                return false;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// label的可见性
        /// </summary>
        public bool LabelsVisible
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return false;
                }
                return Program.frmMain.MapMain.get_LayerLabelsVisible(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                Program.frmMain.MapMain.set_LayerLabelsVisible(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 当与已存在的标签冲突时，是否让MapWinGIS ocx隐藏标签
        /// </summary>
        public bool UseLabelCollision
        {
            get
            {
                return Program.frmMain.MapMain.get_UseLabelCollision(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_UseLabelCollision(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 层的类型，可选如下：
        /// <list type="bullet">
        /// <item>Grid</item>
        /// <item>Image</item>
        /// <item>Invalid</item>
        /// <item>LineShapefile</item>
        /// <item>PointShapefile</item>
        /// <item>PolygonShapefile</item>
        /// </list>
        /// </summary>
        public MapWinGIS.Interfaces.eLayerType LayerType
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return Interfaces.eLayerType.Invalid;
                }
                return this.GetLayerType(m_LayerHandle);
            }
        }

        /// <summary>
        /// 颜色配置，shapefile和grid有各自的配色方案，
        /// </summary>
        public object ColoringScheme
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return null;
                }
                else
                {
                    return Program.frmMain.MapMain.GetColorScheme(m_LayerHandle);
                }
            }
            set
            {
                bool bRes;
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                else
                {
                    if (value == null) //值为空
                    {
                        if (LayerType == Interfaces.eLayerType.Grid)
                        {
                            Program.frmMain.MapMain.SetImageLayerColorScheme(m_LayerHandle, null);
                        }
                        else if (LayerType == Interfaces.eLayerType.LineShapefile || LayerType == Interfaces.eLayerType.PointShapefile || LayerType == Interfaces.eLayerType.PolygonShapefile)
                        {
                            MapWinGIS.ShapefileColorScheme newScheme = new MapWinGIS.ShapefileColorScheme();
                            newScheme.LayerHandle = m_LayerHandle;
                            Program.frmMain.MapMain.ApplyLegendColors(newScheme);
                        }
                        Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Refresh();
                        return;
                    }
                    else //值不为空
                    {
                        if (value is MapWinGIS.ShapefileColorScheme)
                        {
                            MapWinGIS.ShapefileColorScheme scheme = (MapWinGIS.ShapefileColorScheme)value;
                            scheme.LayerHandle = m_LayerHandle;
                            bRes = Program.frmMain.MapMain.ApplyLegendColors(value);
                            if (bRes == false)
                            {
                                m_error = Program.frmMain.MapMain.get_ErrorMsg(Program.frmMain.MapMain.LastErrorCode);
                                return;
                            }
                        }
                        else if (value is MapWinGIS.GridColorScheme)
                        {
                            MapWinGIS.GridColorScheme scheme = (MapWinGIS.GridColorScheme)value;
                            if (LayerType == Interfaces.eLayerType.Grid)
                            {
                                Program.frmMain.Layers.RebuildGridLayer(m_LayerHandle, GetGridObject, scheme);
                                ColoringSchemeTools.ExportScheme(Program.frmMain.m_Layers[m_LayerHandle], Path.ChangeExtension(this.FileName, ".mwleg"));
                            }
                            else
                            {
                                Program.frmMain.MapMain.SetImageLayerColorScheme(m_LayerHandle, value);
                            }
                        }
                        Program.frmMain.MapMain.Invalidate();
                        Program.frmMain.MapMain.Redraw();
                        Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 舍弃的属性，可以用Shapefile.DefaultDrawingOptions替代
        /// </summary>
        public MapWinGIS.Interfaces.ShapefileFillStippleScheme FillStippleScheme
        {
            get
            {
                if (Program.frmMain.m_FillStippleSchemes.ContainsKey(m_LayerHandle))
                {
                    return Program.frmMain.m_FillStippleSchemes[m_LayerHandle];
                }
                return null;
            }
            set
            {
                if (value.LayerHandle != Handle)
                {
                    value.LayerHandle = Handle;
                }
                if (Program.frmMain.m_FillStippleSchemes == null)
                {
                    Program.frmMain.m_FillStippleSchemes = new Dictionary<int, Interfaces.ShapefileFillStippleScheme>();
                }
                if (Program.frmMain.m_FillStippleSchemes.ContainsKey(m_LayerHandle))
                {
                    Program.frmMain.m_FillStippleSchemes[m_LayerHandle] = value;
                }
                else
                {
                    Program.frmMain.m_FillStippleSchemes.Add(m_LayerHandle, value);
                }
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).HatchingScheme = value;
                HatchingRecalculate();
            }
        }

        /// <summary>
        /// 画线颜色设置，用于定义多边形shap
        /// </summary>
        public System.Drawing.Color FillStippleLineColor
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerStippleColor(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerStippleColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// hatching的透明度，用于多边形的shap
        /// </summary>
        public bool FillStippleTransparency
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerStippleTransparent(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerStippleTransparent(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 设置在legend中层的icon
        /// </summary>
        public object Icon
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return null;
                }
                else
                {
                    return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Icon;
                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                }
                else
                {
                    Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Icon = value;
                }
            }
        }

        /// <summary>
        /// 线和点的大小，如果PiontType是ptUserDefined类型，则由LineOrPointSize整合（像素）
        /// </summary>
        public float LineOrPointSize
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return 0;
                }
                else
                {
                    if (LayerType == Interfaces.eLayerType.PointShapefile)
                    {
                        return Program.frmMain.MapMain.get_ShapeLayerPointSize(m_LayerHandle);
                    }
                    else if (LayerType == Interfaces.eLayerType.LineShapefile || LayerType == Interfaces.eLayerType.PolygonShapefile)
                    {
                        return Program.frmMain.MapMain.get_ShapeLayerLineWidth(m_LayerHandle);
                    }
                    return 0;
                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                else
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                    try
                    {
                        if (LayerType == Interfaces.eLayerType.LineShapefile)
                        {
                            Program.frmMain.MapMain.set_ShapeLayerLineWidth(m_LayerHandle, value);
                        }
                        else if (LayerType == Interfaces.eLayerType.PointShapefile)
                        {
                            Program.frmMain.MapMain.set_ShapeLayerPointSize(m_LayerHandle, value);
                        }
                        else if (LayerType == Interfaces.eLayerType.PolygonShapefile)
                        {
                            Program.frmMain.MapMain.set_ShapeLayerLineWidth(m_LayerHandle, value);
                        }
                    }
                    finally
                    {
                        Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                    }
                }
            }
        }

        /// <summary>
        /// expansion box设置是否显示
        /// </summary>
        public bool ExpansionBoxForceAllowed
        {
            get
            {
                return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxForceAllowed;
            }
            set
            {
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxForceAllowed = value;
            }
        }

        /// <summary>
        /// 为层扩充一个区域，当ExpansionBoxForceAllowed为true是可用
        /// 并且，要用这个功能，必须设置ExpansionBoxCustomHeight
        /// </summary>
        public LegendControl.ExpansionBoxCustomRenderer ExpansionBoxCustomRenderFunction
        {
            get
            {
                return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxCustomRenderFunction;
            }
            set
            {
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxCustomRenderFunction = value;
            }
        }

        /// <summary>
        /// 告诉legend你要设置的高度
        /// </summary>
        public LegendControl.ExpansionBoxCustomHeight ExpansionBoxCustomHeightFunction
        {
            get
            {
                return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxCustomHeightFunction;
            }
            set
            {
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).ExpansionBoxCustomHeightFunction = value;
            }
        }

        /// <summary>
        /// 获取设置整个shapefile的绘制方案
        /// 可用属性如下：
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
                    m_error = "图层对象未使用有效句柄初始化";
                    return tkLineStipple.lsNone;
                }
                else
                {
                   return Program.frmMain.MapMain.get_ShapeLayerLineStipple(m_LayerHandle);
                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeLayerLineStipple(m_LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 层的名字
        /// </summary>
        public string Name
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerName(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerName(m_LayerHandle, value);
                Program.frmMain.Legend.Refresh();
            }
        }

        /// <summary>
        /// 设置这个层的多边形的shapefile的轮廓线条颜色
        /// </summary>
        public System.Drawing.Color OutlineColor
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return System.Drawing.Color.AliceBlue;
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeLayerLineColor(m_LayerHandle);
                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
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
                        MapWinGIS.Shapefile sf;
                        sf = (MapWinGIS.Shapefile)o;
                        if (sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGON || sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONM || sf.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYGONZ)
                        {
                            Program.frmMain.MapMain.set_ShapeLayerLineColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
                            Program.frmMain.Legend.Refresh();
                        }
                    }
                    o = null;
                }
            }
        }

        /// <summary>
        /// 设置shapefile点的类型
        /// 可选类型如下：
        /// <list type="bullet">
        /// <item>ptCircle</item>
        /// <item>ptDiamond</item>
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
                    m_error = "图层对象未使用有效句柄初始化";
                    return tkPointType.ptCircle;
                }
                else
                {
                    return Program.frmMain.MapMain.get_ShapeLayerPointType(m_LayerHandle);

                }
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                else
                {
                    Program.frmMain.MapMain.set_ShapeLayerPointType(m_LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 全局位置
        /// 获取设置不依赖与任何组的层的位置
        /// </summary>
        public int GlobalPosition
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerPosition(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.MoveLayer(Program.frmMain.MapMain.get_LayerPosition(m_LayerHandle), value);
            }
        }

        /// <summary>
        /// 在组中的层的位置
        /// </summary>
        public int GroupPosition
        {
            get
            {
                return Program.frmMain.Legend.Layers.PositionInGroup(m_LayerHandle);
            }
            set
            {
                Program.frmMain.Legend.Layers.MoveLayerWithinGroup(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 获取设置层所属于的组的handle
        /// </summary>
        public int GroupHandle
        {
            get
            {
                return Program.frmMain.Legend.Layers.GroupOf(m_LayerHandle);
            }
            set
            {
                int TopLayer;
                if (Program.frmMain.Legend.Groups.IsValidHandle(value))
                {
                    TopLayer = Program.frmMain.Legend.Groups.ItemByHandle(value).LayerCount;
                    Program.frmMain.Legend.Layers.MoveLayer(m_LayerHandle, value, TopLayer);
                }
            }
        }

        /// <summary>
        /// 获取在一个层中的所有shapes，但是只应用与shapefile类型的层
        /// </summary>
        public MapWinGIS.Interfaces.Shapes Shapes
        {
            get
            {
                if (m_LayerHandle >= 0)
                {
                    if (m_Shapes == null)
                    {
                        m_Shapes = new MainProgram.Shapes();
                        m_Shapes.LayerHandle = m_LayerHandle;
                    }
                    return m_Shapes;
                }
                else
                {
                    m_error = "设置了无效的层句柄(handle).";
                    return null;
                }
            }
        }

        /// <summary>
        /// 让用户可以选择这个层的标准视图宽度
        /// </summary>
        public double StandardViewWidth
        {
            get
            {
                return 0; //Unsupported on get.
            }
            set
            {
                Program.frmMain.MapMain.SetLayerStandardViewWidth(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 获取设置这个层的tag（标签，一个可以存储任何信息的字符串）
        /// </summary>
        public string Tag
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerKey(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerKey(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 获取设置Image类型的层的透明色
        /// </summary>
        public System.Drawing.Color ImageTransparentColor
        {
            get
            {
                MapWinGIS.Image img;
                try
                {
                    img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(m_LayerHandle));
                    if (img == null)
                    {
                        return System.Drawing.Color.DarkGray;
                    }
                    return ColorScheme.UIntToColor(img.TransparencyColor);
                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                    return System.Drawing.Color.DarkGray;
                }
                finally
                {
                    img = null;
                }
            }
            set
            {
                try
                {
                    MapWinGIS.Image img;

                   object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                    if (!(o is MapWinGIS.Image))
                    {
                        Program.g_error = "错误的图层类型!";
                        return;
                    }

                    img = (MapWinGIS.Image)o;
                    if (img == null)
                    {
                        return;
                    }

                    img.TransparencyColor = ColorScheme.ColorToUInt(value);

                    if (img.UseTransparencyColor)
                    {
                        Program.frmMain.MapMain.Redraw();
                    }
                    img = null;
                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                }

            }
        }

        /// <summary>
        /// 获取设置Image类型层的透明色2
        /// </summary>
        public System.Drawing.Color ImageTransparentColor2
        {
            get
            {
                MapWinGIS.Image img;
                try
                {
                    img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(m_LayerHandle));
                    if (img == null)
                    {
                        return System.Drawing.Color.DarkGray;
                    }
                    return ColorScheme.UIntToColor(img.TransparencyColor2);
                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                    return System.Drawing.Color.DarkGray;
                }
                finally
                {
                    img = null;
                }
            }
            set
            {
                try
                {
                    MapWinGIS.Image img;

                    object o = Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                    if (!(o is MapWinGIS.Image))
                    {
                        Program.g_error = "错误的图层类型!";
                        return;
                    }

                    img = (MapWinGIS.Image)o;
                    if (img == null)
                    {
                        return;
                    }

                    img.TransparencyColor2 = ColorScheme.ColorToUInt(value);

                    if (img.UseTransparencyColor)
                    {
                        Program.frmMain.MapMain.Redraw();
                    }
                    img = null;
                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                }

            }
        }

        /// <summary>
        /// 获取设置是否在Image类型层中用透明色
        /// </summary>
        public bool UseTransparentColor
        {
            get
            {
                MapWinGIS.Image img;
                try
                {
                    img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(m_LayerHandle));
                    if (img == null)
                    {
                        return false;
                    }
                    return img.UseTransparencyColor;
                }
                finally
                {
                    img = null;
                }
            }
            set
            {
                MapWinGIS.Image img;

                try
                {
                    img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(m_LayerHandle));
                }
                catch
                {
                    return;
                }
                if (img == null)
                {
                    return;
                }

                if (value != img.UseTransparencyColor)
                {
                    img.UseTransparencyColor = value;
                    Program.frmMain.MapMain.Refresh();
                    Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Refresh();
                }

                img = null;
            }
        }

        /// <summary>
        /// 用户自定义线条
        /// </summary>
        public int UserLineStipple
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return 0;
                }
                return Program.frmMain.MapMain.get_UDLineStipple(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                Program.frmMain.MapMain.set_UDLineStipple(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 在用户自定义线条中获取单一行
        /// 在fill stipple中有32行，每行都用同样的UserLineStipple
        /// </summary>
        /// <param name="Row">在0-31中选择一个要选择的行的索引</param>
        /// <returns>A single stipple row.</returns>
        public int GetUserFillStipple(int Row)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return -1;
            }
            return Program.frmMain.MapMain.get_UDFillStipple(m_LayerHandle, Row);
        }

        /// <summary>
        /// 舍弃的方法，设置用户自定义中的一个单行
        /// </summary>
        /// <param name="Row">The index of the row to set.  Must be between 0 and 31 inclusive.</param>
        /// <param name="Value">The row value to set in the fill stipple.</param>
        public void SetUserFillStipple(int Row, int Value)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return;
            }
            Program.frmMain.MapMain.set_UDFillStipple(m_LayerHandle, Row, Value);
        }

        /// <summary>
        /// 在这个层中设置获取用户自定义的点图
        /// 要显示用户定义的点累心，PointType必须设置为ptUserDefined
        /// </summary>
        public MapWinGIS.Image UserPointType
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return null;
                }
                return (MapWinGIS.Image)(Program.frmMain.MapMain.get_UDPointType(m_LayerHandle));
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }
                Program.frmMain.MapMain.set_UDPointType(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 获取设置层是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return false;
                }
                return Program.frmMain.MapMain.get_LayerVisible(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "在图层对象中设置了无效的handle";
                    return;
                }
                else
                {
                    Program.frmMain.MapMain.set_LayerVisible(m_LayerHandle, value);
                    Program.frmMain.Plugins.BroadcastMessage("LayerVisibleChanged " + value.ToString() + " Handle=" + m_LayerHandle.ToString());
                    Program.frmMain.Legend.Refresh();
                }
            }
        }

        /// <summary>
        /// 为整个shapefile设置所有的顶点，只应用于line和polygon类型的shapefile
        /// </summary>
        /// <param name="color">顶点颜色</param>
        /// <param name="vertexSize">顶点大小</param>
        public void ShowVertices(System.Drawing.Color color, int vertexSize)
        {
            if (Program.frmMain.MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                MapWinGIS.Utility.Logger.Dbg("ShowVertices 方法已过时.");
                return;
            }
            if (this.LayerType != Interfaces.eLayerType.PointShapefile)
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                try
                {
                    Program.frmMain.MapMain.set_ShapeLayerPointSize(m_LayerHandle, vertexSize);
                    Program.frmMain.MapMain.set_ShapeLayerPointColor(m_LayerHandle, ColorScheme.ColorToUInt(color));
                    Program.frmMain.MapMain.set_ShapeLayerDrawPoint(m_LayerHandle, true);
                }
                finally
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
            }
        }

        /// <summary>
        /// line和polygon的顶点是否显示
        /// (Doesn't apply to line shapefiles)
        public bool VerticesVisible
        {
            get
            {
                LegendControl.Layer Current = Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle);
                if (Current == null)
                {
                    return false;
                }

                // Old symbology? Use old calls.
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    return Current.VerticesVisible;
                }

                // Point shapefile? 顶点必须显示
                if (Current.Type == Interfaces.eLayerType.PointShapefile)
                {
                    return true;
                }

                // Not a shapefile? 顶点不显示
                if (Current.Type != Interfaces.eLayerType.LineShapefile && Current.Type != Interfaces.eLayerType.PolygonShapefile)
                {
                    return false;
                }

                MapWinGIS.Shapefile SF = Program.frmMain.MapMain.get_Shapefile(m_LayerHandle);
                if (SF == null)
                {
                    return false;
                }

                MapWinGIS.ShapeDrawingOptions Options = SF.DefaultDrawingOptions;
                if (Options == null)
                {
                    return false;
                }

                return Options.VerticesVisible;
            }
            set
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    MapWinGIS.Utility.Logger.Dbg("VerticesVisible属性已过时.");
                    return;
                }

                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).VerticesVisible = value;
                Program.frmMain.MapMain.set_ShapeLayerDrawPoint(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 隐藏所有的顶点，只应用于line和polygon类型的shapefile
        /// </summary>
        public void HideVertices()
        {
            if (Program.frmMain.MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                MapWinGIS.Utility.Logger.Dbg("HideVertices 方法已过时.");
                return;
            }
            if (this.LayerType != Interfaces.eLayerType.PointShapefile)
            {
                Program.frmMain.MapMain.set_ShapeLayerDrawPoint(m_LayerHandle, false);
            }
        }

        /// <summary>
        /// 获取设置正在改变的scale的可见性
        /// 当这个scale在别的scales上面的时候，可见，直到移动到下面时才不可见
        /// </summary>
        public double DynamicVisibilityScale
        {
            get
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    if (Program.frmMain.m_AutoVis == null)
                    {
                        Program.frmMain.m_AutoVis = new DynamicVisibilityClass();
                    }
                    return Program.frmMain.m_AutoVis[m_LayerHandle].DynamicScale;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Dbg("DynamicVisibilityScale 属性已过时. 使用 MinVisibleScale 和 MaxVisibleScale 代替.");
                    return 0.0;
                }
            }
            set
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    if (Program.frmMain.m_AutoVis == null)
                    {
                        Program.frmMain.m_AutoVis = new DynamicVisibilityClass();
                    }
                    Program.frmMain.m_AutoVis[m_LayerHandle].DynamicScale = value;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Dbg("DynamicVisibilityScale 属性已过时. 使用 MinVisibleScale 和 MaxVisibleScale 代替.");
                }
            }
        }

        /// <summary>
        /// 获取设置正在改变的exten的可见性
        /// 当这个extent在别的extents的上面，可见，直到移动到下面才会不可见
        /// </summary>
        public MapWinGIS.Extents DynamicVisibilityExtents
        {
            get
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    if (Program.frmMain.m_AutoVis == null)
                    {
                        Program.frmMain.m_AutoVis = new DynamicVisibilityClass();
                    }
                    return Program.frmMain.m_AutoVis[m_LayerHandle].DynamicExtents;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Dbg("DynamicVisibilityScale 属性已过时. 使用 MinVisibleScale 和 MaxVisibleScale 代替.");
                    return null;
                }
            }
            set
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    if (Program.frmMain.m_AutoVis == null)
                    {
                        Program.frmMain.m_AutoVis = new DynamicVisibilityClass();
                    }
                    Program.frmMain.m_AutoVis[m_LayerHandle].DynamicExtents = value;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Dbg("DynamicVisibilityScale 属性已过时. 使用 MinVisibleScale 和 MaxVisibleScale 代替.");
                }
            }
        }

        /// <summary>
        /// 指定是否使用DynamicVisibility
        /// </summary>
        public bool UseDynamicVisibility
        {
            get
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    return Program.frmMain.m_AutoVis[m_LayerHandle].UseDynamicExtents;
                }
                else
                {
                    return Program.frmMain.MapMain.get_LayerDynamicVisibility(m_LayerHandle);
                }
            }
            set
            {
                if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    Program.frmMain.m_AutoVis[m_LayerHandle].UseDynamicExtents = value;
                }
                else
                {
                    Program.frmMain.MapMain.set_LayerDynamicVisibility(m_LayerHandle, value);
                }
            }
        }

        /// <summary>
        /// 当DynamicVisibilityScale使用时，获取设置最小的scale
        /// </summary>
        public double MinVisibleScale
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerMinVisibleScale(this.m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerMinVisibleScale(this.m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 当DynamicVisibilityScale使用时，获取设置最大的scale
        /// </summary>
        public double MaxVisibleScale
        {
            get
            {
                return Program.frmMain.MapMain.get_LayerMaxVisibleScale(this.m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_LayerMaxVisibleScale(this.m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 重新加载在.lbl文件中的指定的label
        /// </summary>
        /// <param name="lblFilename">要为这个层加载的Label file</param>
        public void ReloadLabels(string lblFilename)
        {
            Program.frmMain.m_Labels.LoadLabelInfo(this, null, lblFilename);
        }

        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="newValue">The new image to add.</param>
        /// <returns>The index for this image, to be passed to ShapePointImageListID or other functions.</returns>
        public long UserPointImageListAdd(MapWinGIS.Image newValue)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return -1;
            }
            return Program.frmMain.MapMain.set_UDPointImageListAdd(m_LayerHandle, newValue);
        }

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        /// <param name="ImageIndex">The image index to retrieve.</param>
        /// <returns>The index associated with this index; or null/nothing if nonexistant.</returns>
        public MapWinGIS.Image UserPointImageListItem(long ImageIndex)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return null;
            }
            else
            {
                if (ImageIndex > Program.frmMain.MapMain.get_UDPointImageListCount(m_LayerHandle) - 1)
                {
                    m_error = "在iamge对象中没有设置与参数对应的索引";
                    return null;
                }
                else
                {
                    return ((MapWinGIS.Image)(Program.frmMain.MapMain.get_UDPointImageListItem(m_LayerHandle, (int)ImageIndex)));
                }
            }
        }

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        public void ClearUDPointImageList()
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
            }
            Program.frmMain.MapMain.ClearUDPointImageList(m_LayerHandle);
        }

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        /// <returns>image list的数量.</returns>
        public long UserPointImageListCount()
        {
            if (m_LayerHandle == -1)
            {
                m_error = "图层对象未使用有效句柄初始化";
                return -1;
            }
            return Program.frmMain.MapMain.get_UDPointImageListCount(m_LayerHandle);
        }

        /// <summary>
        /// 指示是否在保存一个项目时略过这个层
        /// </summary>
        public bool SkipOverDuringSave
        {
            get
            {
                for (int g = 0; g < Program.frmMain.Legend.Groups.Count; g++)
                {
                    for (int l = 0; l < Program.frmMain.Legend.Groups[g].LayerCount; l++)
                    {
                        if (((LegendControl.Group)Program.frmMain.Legend.Groups[g])[l].Handle == m_LayerHandle)
                        {
                            return ((LegendControl.Group)Program.frmMain.Legend.Groups[g])[l].SkipOverDuringSave;
                        }
                    }
                }

                return false;
            }
            set
            {
                for (int g = 0; g < Program.frmMain.Legend.Groups.Count; g++)
                {
                    for (int l = 0; l < Program.frmMain.Legend.Groups[g].LayerCount; l++)
                    {
                        if (((LegendControl.Group)Program.frmMain.Legend.Groups[g])[l].Handle == m_LayerHandle)
                        {
                            ((LegendControl.Group)Program.frmMain.Legend.Groups[g])[l].SkipOverDuringSave = value;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否略过这个层，当绘制legend时。
        /// </summary>
        public bool HideFromLegend
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    return false;
                }
                if (Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle) == null)
                {
                    return false;
                }

                return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).HideFromLegend;
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    return;
                }
                if (Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle) == null)
                {
                    return;
                }

                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).HideFromLegend = value;
            }
        }

        /// <summary>
        /// 为给定的polygon shapefile层，设置透明百分比 
        /// </summary>
        public float ShapeLayerFillTransparency
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return 0;
                }

                return Program.frmMain.MapMain.get_ShapeLayerFillTransparency(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }

                Program.frmMain.MapMain.set_ShapeLayerFillTransparency(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 为给定的image layer，设置透明百分比.
        /// </summary>
        public float ImageLayerFillTransparency
        {
            get
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return 0;
                }

                return Program.frmMain.MapMain.get_ImageLayerPercentTransparent(m_LayerHandle);
            }
            set
            {
                if (m_LayerHandle == -1)
                {
                    m_error = "图层对象未使用有效句柄初始化";
                    return;
                }

                Program.frmMain.MapMain.set_ImageLayerPercentTransparent(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// Deprecated.
        /// </summary>
        public MapWinGIS.Interfaces.ShapefilePointImageScheme PointImageScheme
        {
            get
            {
                return Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).PointImageScheme;
            }
            set
            {
                Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).PointImageScheme = value;
            }
        }

        /// <summary>
        /// 保存shapefile的rendering properties到指定目录
        /// 如果这个层是grid类型，则忽视
        /// </summary>
        /// <param name="saveToFilename">The filename.</param>
        /// <returns>True on success</returns>
        public bool SaveShapeLayerProps(string saveToFilename)
        {
            if (m_LayerHandle == -1)
            {
                return false;
            }
            return Program.frmMain.SaveShapeLayerProps(m_LayerHandle, saveToFilename);
        }

        /// <summary>
        /// 保存shapefile的属性到.mwsr文件中（与shapefile匹配的filename）
        /// 如果层是grid类型，则忽视并返回错误
        /// </summary>
        /// <returns>True on success</returns>
        public bool SaveShapeLayerProps()
        {
            if (m_LayerHandle == -1)
            {
                return false;
            }
            return Program.frmMain.SaveShapeLayerProps(m_LayerHandle);
        }

        /// <summary>
        /// 从指定的文件中加载shapefile的属性
        /// 层不是能是grid类型
        /// </summary>
        /// <param name="loadFromFilename">文件路径</param>
        public bool LoadShapeLayerProps(string loadFromFilename)
        {
            if (m_LayerHandle == -1)
            {
                return false;
            }
            return Program.frmMain.LoadRenderingOptions(m_LayerHandle, loadFromFilename);
        }

        /// <summary>
        /// 从.mwsr中加载shapefile的属性
        /// 如果文件没有找到，返回false
        /// 如果层是grid类型，则忽视并返回错误
        /// </summary>
        /// <returns>True on success</returns>
        public bool LoadShapeLayerProps()
        {
            if (m_LayerHandle == -1)
            {
                return false;
            }
            return Program.frmMain.LoadRenderingOptions(m_LayerHandle);
        }

        /// <summary>
        /// 从指定的文件中加载shapefile目录
        /// Supports files created by Categories dialog (.mwleg extention).
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>True on success</returns>
        public bool LoadShapefileCategories(string filename)
        {
            MapWinGIS.Shapefile sf = this.GetObject() as MapWinGIS.Shapefile;
            if (sf != null)
            {
                if (!System.IO.File.Exists(filename))
                {
                    return false;
                }

                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filename);

                    if (!xmlDoc.DocumentElement.HasAttribute("FileVersion") || !xmlDoc.DocumentElement.HasAttribute("FileType"))
                    {
                        MapWinGIS.Utility.Logger.Message("加载categories失败: 无效的文件", Program.frmMain.ApplicationInfo.ApplicationName,System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxButtons.OK);
                    }
                    else
                    {
                        string s = Convert.ToString(xmlDoc.DocumentElement.Attributes["FileType"].InnerText);
                        if (s.ToLower() == "shapefilecategories")
                        {
                            XmlElement xel = xmlDoc.DocumentElement["Categories"];
                            sf.Categories.Deserialize(xel.InnerText);
                            return true;
                        }
                        else
                        {
                            MapWinGIS.Utility.Logger.Message("加载categories失败: 无效的文件", Program.frmMain.ApplicationInfo.ApplicationName, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxButtons.OK);
                        }

                    }
                }
                catch (Exception ex)
                {
                    MapWinGIS.Utility.Logger.Message("加载categories失败 /r/n" + ex.Message, Program.frmMain.ApplicationInfo.ApplicationName, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            return false;
        }

        /// <summary>
        /// 将shapefile保存到指定目录
        /// 指定shapefile层能用
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>True on success</returns>
        public bool SaveShapefileCategories(string filename)
        {
            MapWinGIS.Shapefile sf = this.GetObject() as MapWinGIS.Shapefile;
            if (sf != null)
            {
                if (System.IO.File.Exists(filename))
                {
                    return false;
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<MapWindow version= \'" + "\'></MapWindow>");

                XmlElement xelRoot = xmlDoc.DocumentElement;
                XmlAttribute attr = xmlDoc.CreateAttribute("FileType");
                attr.InnerText = "ShapefileCategories";
                xelRoot.Attributes.Append(attr);

                attr = xmlDoc.CreateAttribute("FileVersion");
                attr.InnerText = "0";
                xelRoot.Attributes.Append(attr);

                XmlElement xel = xmlDoc.CreateElement("Categories");
                string s = sf.Categories.Serialize();
                xel.InnerText = s;
                xelRoot.AppendChild(xel);

                try
                {
                    xmlDoc.Save(filename);
                    return true;
                }
                catch (Exception ex)
                {
                    MapWinGIS.Utility.Logger.Message("保存Shapefile的categories失败 /r/n" + ex.Message, Program.frmMain.ApplicationInfo.ApplicationName, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxButtons.OK);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建或覆写.mwsymb文件
        /// </summary>
        /// <returns>True on success</returns>
        public bool SaveOptions()
        {
            return this.SaveOptions(string.Empty);
        }

        /// <summary>创建或覆写.mwsymb文件</summary>
        public bool SaveOptions(string filename)
        {
             if (m_LayerHandle == -1)
            {
                return false;
            }
             return Program.frmMain.MapMain.SaveLayerOptions(m_LayerHandle, filename, true, string.Empty);
        
        }

        /// <summary>加载 .mwsymb 文件</summary>
        public bool LoadOptions()
        {
            return this.LoadOptions(string.Empty);
        }

        /// <summary>加载 .mwsymb 文件</summary>
        public bool LoadOptions(string filename)
        {
            if (m_LayerHandle == -1)
            {
                return false;
            }
            string des = "";
            return Program.frmMain.MapMain.LoadLayerOptions(m_LayerHandle, filename, ref des);
        }

        /// <summary>
        /// 重写ToString方法，返回给定图层的基本信息
        /// </summary>
        public override string ToString()
        {
            string s = "Layer Name: " + Name + Environment.NewLine;
            s += "Layer Filename: " + FileName + Environment.NewLine;
            switch (this.LayerType)
            {
                case Interfaces.eLayerType.Grid :
                    s += "Layer type: Grid" + Environment.NewLine; break;
                case Interfaces.eLayerType.Image :
                    s += "Layer type: Image" + Environment.NewLine; break;
                case Interfaces.eLayerType.Invalid:
                    s += "Layer type: Invalid" + Environment.NewLine; break;
                case Interfaces.eLayerType.LineShapefile:
                    s += "Layer type: Polyline Shapefile" + Environment.NewLine; break;
                case Interfaces.eLayerType.PointShapefile:
                    s += "Layer type: Point Shapefile" + Environment.NewLine; break;
                case Interfaces.eLayerType.PolygonShapefile:
                    s += "Layer type: Polygon Shapefile" + Environment.NewLine; break;
            }
            s += "Extents: (" + Extents.xMin.ToString() + ", " + Extents.yMin.ToString() + " " + Extents.xMax.ToString() + ", " + Extents.yMax.ToString() + ")";
            return s;
        }

        /// <summary>
        /// Shape图层的填充色
        /// </summary>
        public System.Drawing.Color ShapeLayerFillColor
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerFillColor(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerFillColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// Shape图层线条的颜色
        /// </summary>
        public System.Drawing.Color ShapeLayerLineColor
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerLineColor(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerLineColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// Shape图层点的颜色
        /// </summary>
        public System.Drawing.Color ShapeLayerPointColor
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerPointColor(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerPointColor(m_LayerHandle, ColorScheme.ColorToUInt(value));
            }
        }

        /// <summary>
        /// shape图层的点的大小
        /// </summary>
        public float ShapeLayerPointSize
        {
            get
            {
                return Program.frmMain.MapMain.get_ShapeLayerPointSize(m_LayerHandle);
            }
            set
            {
                Program.frmMain.MapMain.set_ShapeLayerPointSize(m_LayerHandle, value);
            }
        }

        /// <summary>
        /// 获取层中选择的shape，如果没有shapefile层，则返回空。
        /// </summary>
        public MapWinGIS.Interfaces.SelectInfo SelectedShapes
        {
            get
            {
                if (this.LayerType == Interfaces.eLayerType.LineShapefile || this.LayerType == Interfaces.eLayerType.PolygonShapefile || this.LayerType == Interfaces.eLayerType.PointShapefile)
                {
                    MainProgram.SelectInfo info = new SelectInfo(m_LayerHandle);
                    MapWinGIS.Shapefile sf = Program.frmMain.MapMain.get_Shapefile(m_LayerHandle);
                    if (sf != null)
                    {
                        for (int i = 0; i < sf.NumShapes; i++)
                        {
                            if (sf.ShapeSelected[i])
                            {
                                SelectedShape shp = new SelectedShape();
                                shp.Add(i);
                                info.AddSelectedShape(shp);
                            }
                        }
                    }
                    return info;
                }
                else
                {
                    SelectInfo info = new SelectInfo(m_LayerHandle);
                    return info;
                }
            }
        }

        /// <summary>
        /// 清空shapefile上选择的shape
        /// </summary>
        public void ClearSelection()
        { 
            MapWinGIS.Shapefile sf = Program.frmMain.MapMain.get_Shapefile(m_LayerHandle);
            if (sf != null)
            {
                sf.SelectNone();
            }
            if (this.Handle == Program.frmMain.m_Layers.CurrentLayer)
            {
                Program.frmMain.UpdateButtons();
            }
        }

        /// <summary>
        /// 获取与layer相关的对象，以便插件开发者存储可扩展类
        /// </summary>
        /// <param name="key">A key of object to return</param>
        /// <returns>对于指定的key不存在，返回null</returns>
        public object GetCustomObject(string key)
        {
            LegendControl.Layer layer = Program.frmMain.Legend.Layers.ItemByHandle(this.Handle);
            if (layer != null)
            {
                return layer.GetCustomObject(key);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置与layer相关的对象，以便插件开发者存储可扩展类
        /// </summary>
        /// <param name="obj">自定义对象</param>
        /// <param name="key">自定义对象的key，以便后面可以通过该key来访问这个对象</param>
        public void SetCustomObject(object obj, string key)
        {
            LegendControl.Layer layer = Program.frmMain.Legend.Layers.ItemByHandle(this.Handle);
            if (layer != null)
            {
                layer.SetCustomObject(obj, key);
            }
        }
        #endregion
        

        /// <summary>
        /// 获取图层的类型
        /// </summary>
        internal MapWinGIS.Interfaces.eLayerType GetLayerType(int LayerHandle)
        {
            object lyrObj;
            lyrObj = Program.frmMain.MapMain.get_GetObject(LayerHandle);
            if (lyrObj == null)
            {
                return Interfaces.eLayerType.Invalid;
            }

            if (lyrObj is MapWinGIS.Shapefile)
            {
                MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)lyrObj;
                switch (sf.ShapefileType)
                {
                    case ShpfileType.SHP_POLYGON: 
                    case ShpfileType.SHP_POLYGONM:
                    case ShpfileType.SHP_POLYGONZ:
                        return Interfaces.eLayerType.PolygonShapefile;
                    case ShpfileType.SHP_POINT:
                    case ShpfileType.SHP_POINTM:
                    case ShpfileType.SHP_POINTZ:
                        return Interfaces.eLayerType.PointShapefile;
                    case ShpfileType.SHP_POLYLINE:
                    case ShpfileType.SHP_POLYLINEM:
                    case ShpfileType.SHP_POLYLINEZ:
                        return Interfaces.eLayerType.LineShapefile;
                    case ShpfileType.SHP_MULTIPOINT: 
                    case ShpfileType.SHP_MULTIPOINTM:
                    case ShpfileType.SHP_MULTIPOINTZ:
                        return Interfaces.eLayerType.PointShapefile;
                    default :
                        return Interfaces.eLayerType.Invalid;
                }
            }
            else if (lyrObj is MapWinGIS.Image)
            {
                if (Program.frmMain.Legend.Layers.ItemByHandle(m_LayerHandle).Type == Interfaces.eLayerType.Grid)
                {
                    return Interfaces.eLayerType.Grid;
                }
                else
                {
                    return Interfaces.eLayerType.Image;
                }
            }
            else
            {
                return Interfaces.eLayerType.Invalid;
            }
        }
        
        /// <summary>
        /// shapefile中的shape标签颜色
        /// </summary>
        public void LabelColor(System.Drawing.Color labelColor)
        {
            if (m_LayerHandle == -1)
            {
                m_error = "";
                return;
            }
            Program.frmMain.MapMain.LabelColor(m_LayerHandle, ColorScheme.ColorToUInt(labelColor));
        }

    }
}
