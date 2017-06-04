/***********************************************************************************
 * 文件名:clsView.cs （F）
 * 描  述:提供给插件与mapMain交互的接口。包括地图操作，legend、previewmap操作等多种功能
 *        插件只有通过该类才能获取MainProg.Draw类中的方法。
 * *********************************************************************************/

using System;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 提供地图上shape的视图操作和属性
    /// </summary>
    public class View : MapWinGIS.Interfaces.View
    {
        /// <summary>
        /// SelectInfo对象，存储一个层中所有选择的shape。
        /// 层改变之后应该清理掉
        /// </summary>
        private MapWinGIS.Interfaces.SelectInfo m_selection;

        /// <summary>
        /// 选择的shape的颜色，默认黄色
        /// </summary>
        private int m_SelectColor;
        /// <summary>
        /// 是否保持默认选择模式
        /// </summary>
        private bool m_SelectionPersistence;
        /// <summary>
        /// 容差
        /// </summary>
        private double m_SelectionTolerance;
        /// <summary>
        /// shape选择模式
        /// </summary>
        private MapWinGIS.SelectMode m_Selectmethod; //交叉、包含选择
        /// <summary>
        /// 在层上选择shape的标记存储模式
        /// </summary>
        private MapWinGIS.Interfaces.SelectionOperation m_SelectionOperation; //选择shape模式

        public View()
        {
            m_selection = null;
            m_SelectColor = System.Drawing.Color.Yellow.ToArgb();
            m_Selectmethod = SelectMode.INTERSECTION;
            m_SelectionOperation = MapWinGIS.Interfaces.SelectionOperation.SelectInvert;
        }

        #region  ------------------View接口实现-------------------------
        /// <summary>
        /// 在所有的层上清空所有选择的shapes
        /// </summary>
        public void ClearSelectedShapes()
        {
            for (int i = 0; i < Program.frmMain.Layers.NumLayers; i++)
            {
                Interfaces.Layer layer = Program.frmMain.Layers[Program.frmMain.Layers.GetHandle(i)];
                if (layer != null)
                {
                    layer.ClearSelection();
                }
            }
        }

        /// <summary>
        /// 在给定的点下，查询所有的活动的shapefile层
        /// Queries all of the active shapefile layers for any within the specified tolerance of the given point.
        /// </summary>
        /// <param name="ProjX">要查询的相对于地图的x点坐标</param>
        /// <param name="ProjY">要查询的相对于地图的y点坐标</param>
        /// <param name="Tolerance">在所查询点坐标周围，允许的距离</param>
        /// <returns>Returns an <c>IdentifiedLayers</c> object containing query results.</returns>
        public MapWinGIS.Interfaces.IdentifiedLayers Identify(double ProjX, double ProjY, double Tolerance)
        {
            MainProgram.IdentifiedShapes identifyshpfile;
            MainProgram.IdentifiedLayers ilyr = new MainProgram.IdentifiedLayers();
            int i, j;
            MapWinGIS.Shapefile shpfile;
            object o,res=null;
            MapWinGIS.Extents box;
            int count = Program.frmMain.MapMain.NumLayers;
            for (i = 0; i < count; i++) //遍历每一个图层
            {
                int lyrHandle = Program.frmMain.MapMain.get_LayerHandle(i);
                if (Program.frmMain.MapMain.get_LayerVisible(lyrHandle)) //是可见的图层
                {
                    o = Program.frmMain.MapMain.get_GetObject(lyrHandle);
                    if (o is MapWinGIS.Shapefile) //获取是shapefile类型的图层
                    {
                        shpfile = (MapWinGIS.Shapefile)o;
                        o = null;
                        box = new MapWinGIS.Extents();
                        box.SetBounds(ProjX, ProjY, 0, ProjX, ProjY, 0);
                        if (shpfile.SelectShapes(box, Tolerance, MapWinGIS.SelectMode.INTERSECTION, ref res)) //搜索该点，结果存在res中
                        {
                            identifyshpfile = new MainProgram.IdentifiedShapes();
                            Array arr;
                            arr = (Array)res; //得到在该点下的所有shape的索引集合
                            for (j = 0; j < arr.Length; j++)
                            {
                                identifyshpfile.Add((int)(arr.GetValue(j)));
                            }
                            ilyr.Add(identifyshpfile, lyrHandle);
                        }                       
                    }
                }
            }
            return ilyr;

        }

        /// <summary>
        /// 在没有解锁之前，阻止legend中的任何操作。
        /// 并且，保持所有锁定前的数据
        /// </summary>
        public void LockLegend()
        {
            Program.frmMain.Legend.Lock(); 
        }

        /// <summary>
        /// 解锁legend，允许通过它重绘和更新视图，并反应出我们所做的任何改变
        /// </summary>
        public void UnlockLegend()
        {
            Program.frmMain.Legend.Unlock(); 
        }

        /// <summary>
        /// 锁地图，在没有解锁地图（Unlocked）前，阻止在层上做的任何改变，更新显示在地图上
        /// 并保持锁前所有的数据
        /// </summary>
        public void LockMap()
        {
            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock); 
        }

        /// <summary>
        /// 决定PreviewMap控件是否可见
        /// </summary>
        public bool PreviewVisible
        {
            get
            {
                return Program.frmMain.m_Menu["mnuPreviewVisible"].Checked;
            }
            set
            {
                if (Program.frmMain.m_Menu["mnuPreviewVisible"].Checked != value)
                {
                    Program.frmMain.HandleClickedMenu("mnuPreviewVisible");
                }
            }
        }

        /// <summary>
        /// 决定legend插件是否可见
        /// </summary>
        public bool LegendVisible
        {
            get
            {
                return Program.frmMain.m_Menu["mnuLegendVisible"].Checked;
            }
            set
            {
                if (value)
                {
                    Program.frmMain.legendPanel.Show();
                    Program.frmMain.legendPanel.VisibleState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                }
                else
                {
                    Program.frmMain.legendPanel.Hide();
                }
                Program.frmMain.legendPanel.Visible = value;

                if (Program.frmMain.m_Menu["mnuLegendVisible"].Checked != value)
                {
                    Program.frmMain.HandleClickedMenu("mnuLegendVisible");
                }
            }
        }

        /// <summary>
        /// 解锁，当做出改变时，允许地图重绘
        /// </summary>
        public void UnlockMap()
        {
            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock); 
        }

        /// <summary>
        /// 将屏幕上的坐标点（像素）转换成地图上的坐标点
        /// </summary>
        /// <param name="PixelX">ref 屏幕坐标点的x坐标</param>
        /// <param name="PixelY">ref 屏幕坐标点的y坐标</param>
        /// <param name="ProjX">ref 地图上的x坐标</param>
        /// <param name="ProjY">ref 地图上的y坐标</param>
        public void PixelToProj(double PixelX, double PixelY, ref double ProjX, ref double ProjY)
        {
            Program.frmMain.MapMain.PixelToProj(PixelX, PixelY, ref ProjX, ref ProjY);
        }

        /// <summary>
        /// 将地图上的坐标点转换成屏幕上的坐标点
        /// </summary>
        /// <param name="ProjX">ref 地图上的x坐标</param>
        /// <param name="ProjY">ref 地图上的y坐标</param>
        /// <param name="PixelX">ref 屏幕上的x坐标</param>
        /// <param name="PixelY">ref 屏幕上的y坐标</param>
        public void ProjToPixel(double projX, double projY, ref double pixelX, ref double pixelY)
        {
            Program.frmMain.MapMain.ProjToPixel(projX, projY, ref pixelX, ref pixelY);
        }

        /// <summary>
        /// 让地图重绘，但是当地图锁住时，无法执行该功能
        /// </summary>
        public void Redraw()
        {
            Program.frmMain.MapMain.Redraw();
        }

        /// <summary>
        /// 显示一个提示在地图上的鼠标下面
        /// </summary>
        /// <param name="Text">要显示的文本</param>
        /// <param name="Milliseconds">显示时间</param>
        public void ShowToolTip(string Text, int Milliseconds)
        {
            Program.frmMain.MapMain.ShowToolTip(Text, Milliseconds);
        }

        /// <summary>
        /// 将所有的加载的可见的层Zoom为全图
        /// </summary>
        public void ZoomToMaxExtents()
        {
            Program.frmMain.MapMain.ZoomToMaxExtents();
            Program.frmMain.m_PreviewMap.UpdateLocatorBox();
        }

        /// <summary>
        /// 根据给定的百分比，缩小地图显示
        /// </summary>
        /// <param name="Percent">缩小的比例</param>
        public void ZoomIn(double Percent)
        {
            Program.frmMain.MapMain.ZoomIn(Percent);
            Program.frmMain.m_PreviewMap.UpdateLocatorBox();
        }

        /// <summary>
        /// 根据给定的百分比，放大地图显示
        /// </summary>
        /// <param name="Percent">放大的比例</param>
        public void ZoomOut(double Percent)
        {
            Program.frmMain.MapMain.ZoomOut(Percent);
            Program.frmMain.m_PreviewMap.UpdateLocatorBox(); 
        }

        /// <summary>
        /// 返回到前一次显示样式
        /// </summary>
        public void ZoomToPrev()
        {
            Program.frmMain.MapMain.ZoomToPrev();
            Program.frmMain.m_PreviewMap.UpdateLocatorBox();
        }

        /// <summary>
        /// 获取一个活动的legend
        /// </summary>
        public LegendControl.Legend LegendControl
        {
            get { return Program.frmMain.Legend; }
        }

        /// <summary>
        /// 获取设置存储地图放大缩小等操作历史的数组
        /// </summary>
        public System.Collections.ArrayList ExtentHistory
        {
            get { return Program.frmMain.m_Extents; }
            set { Program.frmMain.m_Extents = value; }
        }

        /// <summary>
        /// 在指定的范围内，当前可见的层中，指定的范围内截图Takes a snapshot
        /// </summary>
        /// <param name="Bounds">要截图的范围</param>
        public MapWinGIS.Image Snapshot(MapWinGIS.Extents Bounds)
        {
            return (MapWinGIS.Image)(Program.frmMain.MapMain.SnapShot(Bounds));
        }

        /// <summary>
        /// 获取设置地图的背景色
        /// </summary>
        public System.Drawing.Color BackColor
        {
            get
            {
                MapWinGIS.Map map = (MapWinGIS.Map)(((AxHost)Program.frmMain.MapMain).GetOcx());
                return ColorScheme.UIntToColor(map.BackColor);
            }
            set
            {

                MapWinGIS.Map map = (MapWinGIS.Map)(((AxHost)Program.frmMain.MapMain).GetOcx());
                map.BackColor = ColorScheme.ColorToUInt(value);
                map = null;
            }
        }

        /// <summary>
        /// 获取设置当前鼠标的样式. 可用样式：
        /// <list type="bullet">
        /// <item>cmNone</item>
        /// <item>cmPan</item>
        /// <item>cmSelection</item>
        /// <item>cmZoomIn</item>
        /// <item>cmZoomOut</item>
        /// </list>
        /// </summary>
        public MapWinGIS.tkCursorMode CursorMode
        {
            get
            {
                return  Program.frmMain.MapMain.CursorMode;
            }
            set
            {
                Program.frmMain.MapMain.CursorMode = value;
                Program.frmMain.UpdateButtons();
            }
        }

        /// <summary>
        /// 获取常用的Drawing
        /// </summary>
        public MapWinGIS.Interfaces.Draw Draw
        {
            get
            {
                MainProgram.Draw df = new MainProgram.Draw();
                return df;
            }
        }

        /// <summary>
        /// Gets or sets the amount to pad around the extents when calling <c>ZoomToMaxExtents</c>,
        /// <c>ZoomToLayer</c>, and <c>ZoomToShape</c>.
        /// </summary>
        public double ExtentPad
        {
            get
            {
                return Program.frmMain.MapMain.ExtentPad;
            }
            set
            {
                Program.frmMain.MapMain.ExtentPad = value;
            }
        }

        /// <summary>
        /// Gets or sets the map's current extents.
        /// </summary>
        public MapWinGIS.Extents Extents
        {
            get
            {
                return (MapWinGIS.Extents)Program.frmMain.MapMain.Extents;
            }
            set
            {
                Program.frmMain.MapMain.Extents = value;
                Program.frmMain.m_PreviewMap.UpdateLocatorBox();
            }
        }

        /// <summary>
        /// 作用在地图上鼠标样式.  可用样式如下:
        /// <list type="bullet">
        /// <item>crsrAppStarting</item>
        /// <item>crsrArrow</item>
        /// <item>crsrCross</item>
        /// <item>crsrHelp</item>
        /// <item>crsrIBeam</item>
        /// <item>crsrMapDefault</item>
        /// <item>crsrNo</item>
        /// <item>crsrSizeAll</item>
        /// <item>crsrSizeNESW</item>
        /// <item>crsrSizeNS</item>
        /// <item>crsrSizeNWSE</item>
        /// <item>crsrSizeWE</item>
        /// <item>crsrUpArrow</item>
        /// <item>crsrUserDefined</item>
        /// <item>crsrWait</item>
        /// </list>
        /// </summary>
        public MapWinGIS.tkCursor MapCursor
        {
            get
            {
                return Program.frmMain.MapMain.MapCursor;
            }
            set
            {
                Program.frmMain.MapMain.MapCursor = value;
            }
        }

        /// <summary>
        /// Indicates that the map should handle file drag-drop events (as opposed to firing a message indicating file(s) were dropped).
        /// </summary>
        public bool HandleFileDrop
        {
            get
            {
                return Program.frmMain.m_HandleFileDrop;
            }
            set
            {
                Program.frmMain.m_HandleFileDrop = value;
            }
        }

        /// <summary>
        /// MapState是描述整个地图状态的字符串，包括：层和配色
        /// </summary>
        public string MapState
        {
            get
            {
                return Program.frmMain.MapMain.MapState;
            }
            set
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                try
                {
                    Program.frmMain.Legend.Lock();
                    Program.frmMain.Legend.Layers.Clear();
                    Program.frmMain.MapMain.RemoveAllLayers();
                    Program.frmMain.MapPreview.ClearDrawings();
                    Program.frmMain.MapPreview.RemoveAllLayers();
                    Program.frmMain.MapMain.MapState = value;
                    Program.frmMain.Legend.Unlock();
                }
                finally
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
            }
        }

        /// <summary>
        /// 获取SelectInfo对象所包含的关于当前层的所有shapes的信息
        /// </summary>
        public MapWinGIS.Interfaces.SelectInfo SelectedShapes
        {
            get
            {
                if (m_selection != null && m_selection.LayerHandle == Program.frmMain.Layers.CurrentLayer)
                {
                    return m_selection;
                }

                if (Program.frmMain.Layers.CurrentLayer > -1)
                {
                    Interfaces.Layer layer = Program.frmMain.m_Layers[Program.frmMain.Layers.CurrentLayer];
                    if (layer != null)
                    {
                        m_selection = Program.frmMain.Layers[Program.frmMain.Layers.CurrentLayer].SelectedShapes;
                        return m_selection;
                    }
                }
                SelectInfo info = new SelectInfo(-1);
                return info;
            }
        }

        /// <summary>
        /// 决定标签是否从project-specific或shapefile-specific文件夹保存和加载 
        /// Using a project-level label file will create a new subdirectory with the project's name.
        /// </summary>
        public bool LabelsUseProjectLevel
        {
            get
            {
                return Program.appInfo.LabelsUseProjectLevel;
            }
            set
            {
                Program.appInfo.LabelsUseProjectLevel = value;
                Program.frmMain.SetModified(true);
            }
        }

        /// <summary>
        /// 在给定的层中引发重新加载field values
        /// 如果层（shapefile、labels）的handle是无效的，任何事件不会发生
        /// </summary>
        /// <param name="LayerHandle"></param>
        public void LabelsRelabel(int LayerHandle)
        {
            if (Program.frmMain.Layers.IsValidHandle(LayerHandle) && (Program.frmMain.m_Layers[LayerHandle].LayerType == Interfaces.eLayerType.LineShapefile || Program.frmMain.Layers[LayerHandle].LayerType == Interfaces.eLayerType.PointShapefile || Program.frmMain.Layers[LayerHandle].LayerType == Interfaces.eLayerType.PolygonShapefile))
            {
                Program.frmMain.DoLabelsRelabel(LayerHandle);
            }
        }

        /// <summary>
        /// 显示label编辑器
        /// 如果LayerHandle是无效的或没有shapefile，不会发生任何事件
        /// </summary>
        /// <param name="LayerHandle"></param>
        public void LabelsEdit(int LayerHandle)
        {
            if (Program.frmMain.Layers.IsValidHandle(LayerHandle) && (Program.frmMain.m_Layers[LayerHandle].LayerType == Interfaces.eLayerType.LineShapefile || Program.frmMain.Layers[LayerHandle].LayerType == Interfaces.eLayerType.PointShapefile || Program.frmMain.Layers[LayerHandle].LayerType == Interfaces.eLayerType.PolygonShapefile))
            {
                Program.frmMain.DoLabelsEdit(LayerHandle);
            }
        }

        /// <summary>
        /// 是否保持选择。默认为false
        /// 是，那么之前的选择在没有选择新的shape之前不会清空。
        /// 否，那么所有的选择将清空
        /// </summary>
        public bool SelectionPersistence
        {
            get
            {
                return m_SelectionPersistence;
            }
            set
            {
                m_SelectionPersistence = value;
            }
        }

        /// <summary>
        /// 在地图中可容忍选择的公差
        /// </summary>
        public double SelectionTolerance
        {
            get
            {
                return m_SelectionTolerance;
            }
            set
            {
                m_SelectionTolerance = value;
            }
        }

        /// <summary>
        /// 获取设置 选择模式：包含、交叉
        /// <list type="bullet">
        /// <item>Inclusion</item>
        /// <item>Intersection</item>
        /// </summary>
        public MapWinGIS.SelectMode SelectMethod
        {
            get
            {
                return m_Selectmethod;
            }
            set
            {
                m_Selectmethod = value;
            }
        }

        /// <summary>
        /// 获取设置被选择的shape的颜色
        /// </summary>
        public System.Drawing.Color SelectColor
        {
            get
            {
                return ColorScheme.IntToColor(m_SelectColor);
            }
            set
            {
                m_SelectColor = ColorScheme.ColorToInt(value);

                for (int i = 0; i < Program.frmMain.MapMain.NumLayers; i++)
                {
                    int handle = Program.frmMain.MapMain.get_LayerHandle(i);
                    MapWinGIS.Shapefile sf = Program.frmMain.MapMain.get_Shapefile(handle);
                    if (sf != null)
                    {
                        sf.SelectionColor = (uint)m_SelectColor;
                    }
                }
            }
        }

        /// <summary>
        /// 与地图相关的提示语
        /// </summary>
        public string Tag
        {
            get
            {
                return Program.frmMain.MapMain.Key;
            }
            set
            {
                Program.frmMain.MapMain.Key = value;
            }
        }

        /// <summary>
        /// 当CursorMode是cmUserDefined时，获取设置鼠标句柄
        /// </summary>
        public int UserCursorHandle
        {
            get
            {
                return Program.frmMain.MapMain.UDCursorHandle;
            }
            set
            {
                Program.frmMain.MapMain.UDCursorHandle = value;
            }
        }

        /// <summary>
        /// 当在地图上用鼠标Zoom时，设置默认Zoom比例
        /// </summary>
        public double ZoomPercent
        {
            get
            {
                return Program.frmMain.MapMain.ZoomPercent;
            }
            set
            {
                Program.frmMain.MapMain.ZoomPercent = value;
            }
        }

        /// <summary>
        /// S在指定的点上选择shape. 误差在:View.SelectionTolerance中设置
        /// </summary>
        /// <param name="ScreenX">相对于屏幕的x坐标</param>
        /// <param name="ScreenY">相对于屏幕的y坐标</param>
        /// <param name="ClearOldSelection">是否清空所有之前选择的shape</param>
        public MapWinGIS.Interfaces.SelectInfo Select(int ScreenX, int ScreenY, bool ClearOldSelection)
        {
            return this.SelectShapesByPoint(ScreenX, ScreenY, !ClearOldSelection);
        }

        /// <summary>
        /// 选择用户用鼠标指定的矩形内的shapes
        /// </summary>
        /// <param name="ScreenBounds">相对于屏幕的矩形的坐标点</param>
        /// <param name="ClearOldSelection">是否清空所有之前选择的shape</param>
        public MapWinGIS.Interfaces.SelectInfo Select(System.Drawing.Rectangle ScreenBounds, bool ClearOldSelection)
        {
            return this.SelectShapesByRectangle(ScreenBounds.Left, ScreenBounds.Right, ScreenBounds.Top, ScreenBounds.Bottom, !ClearOldSelection);
        }

        /// <summary>获取设置shapes的绘制方法</summary>
        public MapWinGIS.tkShapeDrawingMethod ShapeDrawingMethod
        {
            get
            {
                return Program.frmMain.MapMain.ShapeDrawingMethod;
            }
            set
            {
                Program.frmMain.MapMain.ShapeDrawingMethod = value;
            }
        }

        /// <summary>获取map control的高度,即MapMain在宿主中的高</summary>
        /// <remarks>Added by Paul Meems on May 26 2010</remarks>
        public int MapHeight
        {
            get { return Program.frmMain.MapMain.Height; }
        }

        /// <summary>获取map control的宽度，即MapMain在宿主中的宽</summary>
        public int MapWidth 
        {
            get { return Program.frmMain.MapMain.Width; }
        }

        /// <summary>获取设置当前视图的scale</summary>
        public double Scale
        {
            get
            {
                Program.frmMain.MapMain.MapUnits = GetMapUnits(Program.frmMain.m_Project.MapUnits);
                return Program.frmMain.MapMain.CurrentScale;
            }
            set
            {
                Program.frmMain.MapMain.CurrentScale = value;
            }
        }

        /// <summary>获取地图是否被锁</summary>
        public bool IsMapLocked
        {
            get
            {
                if (Program.frmMain.MapMain.IsLocked == MapWinGIS.tkLockMode.lmLock)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否能用CanUseImageGrouping属性
        /// 提高Image加载速度
        /// </summary>
        public bool CanUseImageGrouping
        {
            get
            {
                return Program.frmMain.MapMain.CanUseImageGrouping;
            }
            set
            {
                Program.frmMain.MapMain.CanUseImageGrouping = value;
            }
        }

        /// <summary>
        /// 在指定的层上更新选择的shapes
        /// 别的层上的选择将保持
        /// </summary>
        /// <param name="sf">要更新的Shapefile</param>
        /// <param name="shpIndices">所有相关的Shape的索引集合</param>
        /// <param name="mode">选择操作</param>
        /// <returns></returns>
        public MapWinGIS.Interfaces.SelectInfo UpdateSelection(int layerHandle, ref int[] shpIndices, Interfaces.SelectionOperation mode)
        {
            //清空原来存储的
            m_selection = null;

            Interfaces.Layer layer = Program.frmMain.m_Layers[layerHandle];
            if (layer == null)
            {
                return null;
            }
            MapWinGIS.Shapefile sf = layer.GetObject() as MapWinGIS.Shapefile;
            if (sf == null)
            {
                return null;
            }

            if (mode == Interfaces.SelectionOperation.SelectNew)
            {
                sf.SelectNone();
            }

            if (shpIndices != null && shpIndices.Length > 0)
            {
                int i;
                int shpIndicesLen = shpIndices.Length;
                if (mode == Interfaces.SelectionOperation.SelectNew)
                {
                    for (i = 0; i < shpIndicesLen; i++)
                    {
                        sf.ShapeSelected[shpIndices[i]] = true;
                    }
                }
                else if (mode == Interfaces.SelectionOperation.SelectAdd)
                {
                    for (i = 0; i < shpIndicesLen; i++)
                    {
                        sf.ShapeSelected[shpIndices[i]] = true;
                    }
                }
                else if (mode == Interfaces.SelectionOperation.SelectExclude)
                {
                    for (i = 0; i < shpIndicesLen; i++)
                    {
                        sf.ShapeSelected[shpIndices[i]] = false;
                    }
                }
                else if (mode == Interfaces.SelectionOperation.SelectInvert)
                {
                    for (i = 0; i < shpIndicesLen; i++)
                    {
                        sf.ShapeSelected[shpIndices[i]] = !sf.ShapeSelected[shpIndices[i]];
                    }
                }
            }

            Program.frmMain.UpdateButtons();

            bool handled = false;
            Program.frmMain.FireLayerSelectionChanged(layerHandle, ref handled);

            return layer.SelectedShapes;
        }

        /// <summary>
        /// 返回最大可见范围
        /// 所有可见的关联的层的范围
        /// </summary>
        public MapWinGIS.Extents MaxVisibleExtents
        {
            get
            {
                MapWinGIS.Extents tExts = new MapWinGIS.Extents();
                bool bFoundVisibleLayer = false;
                double maxX = 0, maxY = 0, minX = 0, minY = 0; 
                int i;
                double dx, dy;

                int numLyr = Program.frmMain.MapMain.NumLayers;
                for (i = 0; i < numLyr; i++)
                {
                    int lyrHandle = Program.frmMain.MapMain.get_LayerHandle(i);
                    if (Program.frmMain.MapMain.get_LayerVisible(lyrHandle))
                    {
                        tExts = Program.frmMain.m_Layers[lyrHandle].Extents;
                        if (bFoundVisibleLayer == false)
                        {
                            maxX = tExts.xMax;
                            minX = tExts.xMin;
                            maxY = tExts.yMax;
                            minY = tExts.yMin;
                            bFoundVisibleLayer = true;
                        }
                        else
                        {
                            if (tExts.xMax > maxX)
                            {
                                maxX = tExts.xMax;
                            }
                            if (tExts.yMax > maxY)
                            {
                                maxY = tExts.yMax;
                            }
                            if (tExts.xMin < minX)
                            {
                                minX = tExts.xMin;
                            }
                            if (tExts.yMin < minY)
                            {
                                minY = tExts.yMin;
                            }
                        }
                    }
                }

                dx = maxX - minX;
                dx = dx * Program.frmMain.MapMain.ExtentPad;
                maxX = maxX + dx;
                minX = minX - dx;

                dy = maxY - minY;
                dy = dy * Program.frmMain.MapMain.ExtentPad;
                maxY = maxY + dy;
                minY = minY - dy;

                tExts = new MapWinGIS.Extents();
                tExts.SetBounds(minX, minY, 0, maxX, maxY, 0);
                return tExts;
            }
        }

        /// <summary>
        /// 重绘map和legend
        /// </summary>
        public void ForceFullRedraw()
        {
            // 重绘map
            bool locked = this.IsMapLocked;
            while (this.IsMapLocked)
            {
                this.UnlockMap();
            }
            this.Redraw();
            Application.DoEvents();
            if (locked)
            {
                this.LockMap();
            }

            // 重绘 legend
            locked = this.IsMapLocked;
            while (this.LegendControl.Locked)
            {
                this.LegendControl.Unlock();
            }
            this.LegendControl.Refresh();
            Application.DoEvents();
            if (locked)
            {
                this.LegendControl.Lock();
            }
        }

        /// <summary>
        /// 在legend和toolbox中转换，显示legend
        /// </summary>
        public void ShowLegend()
        {
            if (Program.frmMain.m_legendTabControl != null && Program.frmMain.m_legendTabControl.TabCount > 1)
            {
                Program.frmMain.m_legendTabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 在legend和toolbox中转换，显示toolbox
        /// </summary>
        public void ShowToolbox()
        {
            if (Program.frmMain.m_legendTabControl != null && Program.frmMain.m_legendTabControl.TabCount > 1)
            {
                Program.frmMain.m_legendTabControl.SelectedIndex = 1;
            }
        }

        #endregion 51

        /// <summary>
        /// 根据给定的点搜索shape
        /// </summary>
        /// <param name="screenX">屏幕x坐标</param>
        /// <param name="screenY">屏幕y坐标</param>
        /// <param name="ctrlDown">Ctrl按钮是否按下</param>
        /// <returns></returns>
        internal MapWinGIS.Interfaces.SelectInfo SelectShapesByPoint(int screenX, int screenY, bool ctrlDown = false)
        {
            if (Program.frmMain.Legend.SelectedLayer == -1)
            {
                return null;
            }

            double x = 0, y = 0;
            PixelToProj(screenX, screenY, ref x, ref y);

            MapWinGIS.Extents bounds = new MapWinGIS.Extents();
            bounds.SetBounds(x, y, 0, x, y, 0);

            Interfaces.eLayerType type = Program.frmMain.m_Layers[Program.frmMain.Layers.CurrentLayer].LayerType;
            if (type == Interfaces.eLayerType.Grid || type == Interfaces.eLayerType.Image || type == Interfaces.eLayerType.Invalid)
            {
                return null;
            }

            object obj = Program.frmMain.MapMain.get_GetObject(Program.frmMain.Layers.CurrentLayer);
            MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)obj;

            //计算容差
            double tolerance = m_SelectionTolerance;
            if (tolerance == 0) //容差
            {
                if (((sf.ShapefileType == ShpfileType.SHP_POLYGON) || (sf.ShapefileType == ShpfileType.SHP_POLYGONM)) || (sf.ShapefileType == ShpfileType.SHP_POLYGONZ))
                {
                    tolerance = 0.0D;
                }
                else if (((sf.ShapefileType == ShpfileType.SHP_POINT) || (sf.ShapefileType == ShpfileType.SHP_POINTM)) || (sf.ShapefileType == ShpfileType.SHP_POINTZ))
                {

                    double x1 = 0, y1 = 0;
                    int size = (int)(sf.DefaultDrawingOptions.PointSize / 2);

                    PixelToProj(screenX + size, screenY + size, ref x1, ref y1);
                    tolerance = System.Math.Sqrt(Math.Pow((x - x1), 2) + Math.Pow((y - y1), 2));
                }
                else
                {
                    double x1 = 0, y1 = 0;
                    PixelToProj(screenX + 5, screenY + 5, ref x1, ref y1);

                    tolerance = System.Convert.ToDouble(Math.Sqrt(Math.Pow((x - x1), 2) + Math.Pow((y - y1), 2)));
                }
            }

            if (ctrlDown) //选择模式
            {
                m_SelectionOperation = Interfaces.SelectionOperation.SelectAdd;
            }
            else
            {
                m_SelectionOperation = Interfaces.SelectionOperation.SelectNew;
            }

            //处理选择的shape
            return PerformSelection(sf, bounds, tolerance);
        }

        /// <summary>
        /// 根据在屏幕中划定的范围搜索shapes
        /// </summary>
        /// <param name="screenLeft">相对屏幕的左坐标点</param>
        /// <param name="screenRight">相对屏幕的右坐标点</param>
        /// <param name="screenTop">相对屏幕的顶部坐标点</param>
        /// <param name="screenBottom">相对屏幕的底部坐标点</param>
        /// <param name="ctrlDown"></param>
        /// <returns></returns>
        internal MapWinGIS.Interfaces.SelectInfo SelectShapesByRectangle(int screenLeft, int screenRight, int screenTop, int screenBottom, bool ctrlDown = false)
        {
            if (Program.frmMain.Legend.SelectedLayer == -1)
            {
                return null;
            }

            Interfaces.eLayerType type = Program.frmMain.Layers[Program.frmMain.Layers.CurrentLayer].LayerType;
            if (type == Interfaces.eLayerType.Grid || type == Interfaces.eLayerType.Image || type == Interfaces.eLayerType.Invalid)
            {
                return null;
            }

            MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)(Program.frmMain.MapMain.get_GetObject(Program.frmMain.Layers.CurrentLayer));

            double geoL = 0, geoR = 0, geoT = 0, geoB = 0; 

            Program.frmMain.MapMain.PixelToProj(screenLeft, screenTop, ref geoL, ref geoT);
            Program.frmMain.MapMain.PixelToProj(screenRight, screenBottom, ref geoR, ref geoB);

            MapWinGIS.Extents bounds = new MapWinGIS.Extents();
            bounds.SetBounds(geoL, geoB, 0, geoR, geoT, 0);

            if (ctrlDown)
            {
                m_SelectionOperation = Interfaces.SelectionOperation.SelectAdd;
            }
            else
            {
                m_SelectionOperation = Interfaces.SelectionOperation.SelectNew;
            }

            return PerformSelection(sf, bounds, 0.0D);
        }

        /// <summary>
        /// 根据给定的shapefile、范围、容差来将选择的shape存入shapeInfo集合中
        /// </summary>
        internal MapWinGIS.Interfaces.SelectInfo PerformSelection(MapWinGIS.Shapefile sf, MapWinGIS.Extents bounds, double tolerance)
        {
            SelectInfo m_SelectedShapes = new SelectInfo(this.LegendControl.SelectedLayer);

            if (m_SelectedShapes == null)
            {
                m_SelectedShapes = new MainProgram.SelectInfo(this.LegendControl.SelectedLayer);
            }
            m_SelectedShapes.ClearSelectedShapesTemp();

            object arr = null;
            System.Array results = null;
            if (sf.SelectShapes(bounds, tolerance, m_Selectmethod, ref arr))
            {
                results = (System.Array)arr;
            }

            if (m_SelectionOperation == Interfaces.SelectionOperation.SelectNew)
            {
                sf.SelectNone();
            }

             int i;            
            //设置选择的shape
            if (results != null && results.Length > 0)
            {
                int len = results.Length;

                if (m_SelectionOperation == Interfaces.SelectionOperation.SelectNew)
                {
                    for (i = 0; i < len; i++)
                    {
                        sf.ShapeSelected[((int)results.GetValue(i))] = true;
                    }
                }
                else if (m_SelectionOperation == Interfaces.SelectionOperation.SelectAdd)
                {
                    for (i = 0; i < len; i++)
                    {
                        sf.ShapeSelected[((int)results.GetValue(i))] = true;
                    }
                }
                else if (m_SelectionOperation == Interfaces.SelectionOperation.SelectExclude)
                {
                    for (i = 0; i < len; i++)
                    {
                        sf.ShapeSelected[((int)results.GetValue(i))] = false;
                    }
                }
                else if (m_SelectionOperation == Interfaces.SelectionOperation.SelectInvert)
                {
                    for (i = 0; i < len; i++)
                    {
                        sf.ShapeSelected[((int)results.GetValue(i))] = !sf.ShapeSelected[((int)results.GetValue(i))];
                    }
                }
            }
            else
            {
                if (m_SelectionOperation == Interfaces.SelectionOperation.SelectNew)
                {
                    sf.SelectNone();
                }
            }

            // 将选择的shape添加到shapeInfo集合中
            for (i = 0; i < sf.NumShapes ; i++)
            {
                if (sf.ShapeSelected[i])
                {
                    SelectedShape shape = new SelectedShape();
                    shape.Add(i);
                    m_SelectedShapes.AddSelectedShape(shape);
                }
            }

            m_selection = null;

            this.Redraw();
            return m_SelectedShapes;
        }

        /// <summary>
        /// 获取地图的单位
        /// </summary>
        private MapWinGIS.tkUnitsOfMeasure GetMapUnits(string projectMapUnits)
        {
            switch (projectMapUnits.ToLower())
            {
                case "lat/long":
                    return MapWinGIS.tkUnitsOfMeasure.umDecimalDegrees;
                case "meters":
                    return MapWinGIS.tkUnitsOfMeasure.umMeters;
                case "centimeters":
                    return MapWinGIS.tkUnitsOfMeasure.umCentimeters;
                case "feet":
                    return MapWinGIS.tkUnitsOfMeasure.umFeets;
                case "inches":
                    return MapWinGIS.tkUnitsOfMeasure.umInches;
                case "kilometers":
                    return MapWinGIS.tkUnitsOfMeasure.umKilometers;
                case "miles":
                    return MapWinGIS.tkUnitsOfMeasure.umMiles;
                case "millimeters":
                    return MapWinGIS.tkUnitsOfMeasure.umMiliMeters;
                case "yards":
                    return MapWinGIS.tkUnitsOfMeasure.umYards;
                case "us-ft":
                    return MapWinGIS.tkUnitsOfMeasure.umFeets;
                default:
                    return MapWinGIS.tkUnitsOfMeasure.umMeters;
            }
        }
        
    } //56
}
