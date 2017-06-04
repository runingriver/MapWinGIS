/****************************************************************************
 * 文件名:clsLayers.cs
 * 描  述: 提供操作地图上图层的方法。主要包括：添加图层、移动图层位置、清空图层等。
 *         OpenGrid和Grid功能未实现。
 * **************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MapWinGIS.Controls;

namespace MapWinGIS.MainProgram
{
    public class Layers : MapWinGIS.Interfaces.Layers, MapWinGIS.ICallback, IEnumerable<Interfaces.Layer>
    {
        #region 变量声明
        /// <summary>
        /// 指示是否是插件调用的
        /// </summary>
        private bool m_PluginCall = false;
        
        /// <summary>
        /// 存储当前MapMain中的Grid类型图层。
        /// </summary>
        internal Hashtable m_Grids = new Hashtable();

        // Projection mismatch tester, used during adding sesion
        private MapWinGIS.Controls.Projections.MismatchTester m_mismatchTester = null;

        /// <summary>
        /// 在添加图层时要添加添加的图层列表
        /// </summary>
        private List<MapWinGIS.Interfaces.Layer> m_newLayers = new List<MapWinGIS.Interfaces.Layer>();

        /// <summary>
        /// 在添加图层时要添加的图层数量
        /// </summary>
        private int m_initCount;

        static int addCnt = 0; //此变量用来存储已添加无图层名的图层的个数，为了为无名字的图层设置一个不同的名字
        #endregion

        #region 实现泛型（IEnumerator<Layer>）枚举器
        /// <summary>
        /// 实现泛型枚举器，提供在Layers上的foreach迭代
        /// </summary>
        private class LayerEnumerator : System.Collections.Generic.IEnumerator<MapWinGIS.Interfaces.Layer>
        {
            MapWinGIS.Interfaces.Layers m_layers;
            int m_index = -1;

            public LayerEnumerator(MapWinGIS.MainProgram.Layers layers) //改成Interfaces.Layers试试
            {
                this.m_layers = layers;
                this.m_index = -1;
            }

            public MapWinGIS.Interfaces.Layer Current //泛型枚举的Current
            {
                get
                {
                    return m_layers[m_layers.GetHandle(m_index)];
                }
            }

            object IEnumerator.Current //集合枚举 的Current ,不能加public
            {
                get
                {
                    return m_layers[m_layers.GetHandle(m_index)];
                }
            }

            public bool MoveNext()
            {
                m_index++;
                if (m_index >= m_layers.NumLayers)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void Reset()
            {
                m_index = -1;
            }

            #region 实现IDisposable
            private bool disposedValue = false; // 检查无用的方法
            //实现IDisposable
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                    }
                }
                this.disposedValue = true;
            }

            public void Dispose() //实现泛型集合的IDisposable
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }


            #endregion
        }
        #endregion

        //实现集合枚举
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LayerEnumerator(this);
        }

        //实现泛型枚举
        public IEnumerator<MapWinGIS.Interfaces.Layer> GetEnumerator()
        {
            return new LayerEnumerator(this);
        }


        #region Layers接口实现 Properties、Methods、Layers.Add、Grid utilities
        /// <summary>
        /// 从地图中移除所有的层
        /// </summary>
        public void Clear()
        {
            try
            {
                int i;
                for (i = 0; i < Program.frmMain.MapMain.NumLayers; i++)
                {
                    if (this[GetHandle(i)].LayerType == Interfaces.eLayerType.Grid)
                    {
                        try
                        {
                            if (this[GetHandle(i)].GetGridObject != null)
                            {
                                ((MapWinGIS.Grid)this[GetHandle(i)].GetGridObject).Close();
                            }
                        }
                        catch
                        {
                        }

                        // 也要关闭image类型对象
                        MapWinGIS.Image o;
                        o = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(this.GetHandle(i)));
                        if (o != null)
                        {
                            o.Close();
                        }
                        o = null;
                    }
                    else if (this[GetHandle(i)].LayerType == Interfaces.eLayerType.Image)
                    {
                        ((MapWinGIS.Image)(this[GetHandle(i)].GetObject())).Close();
                    }
                    else if (this[GetHandle(i)].LayerType == Interfaces.eLayerType.Invalid)
                    {
                    }
                    else
                    {
                        ((MapWinGIS.Shapefile)(this[GetHandle(i)].GetObject())).Close();
                    }
                }
                Program.frmMain.Legend.Layers.Clear();
                Program.frmMain.Legend.Groups.Clear();
                m_Grids.Clear();
                if (Program.frmMain.m_PluginManager != null)
                {
                    Program.frmMain.m_PluginManager.LayersCleared();
                }
                Program.frmMain.m_AutoVis.Clear();

                //没有图层后，就不需要有项目投影了
                Program.projInfo.ProjectProjection = "";

            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
            }
        }

        /// <summary>
        /// 将一个层移动到另外一个位置或者组中
        /// </summary>
        /// <param name="Handle">要移动层的Handle</param>
        /// <param name="NewPosition">目标组中的新位置.</param>
        /// <param name="TargetGroup">要将层移动到的组.</param>
        public void MoveLayer(int Handle, int NewPosition, int TargetGroup)
        { 
            if (TargetGroup == -1)
            {
                Program.frmMain.Legend.Layers.MoveLayerWithinGroup(Handle, NewPosition);
            }
            else
            {
                Program.frmMain.Legend.Layers.MoveLayer(Handle, TargetGroup, NewPosition);
            }
        }

        /// <summary>
        /// 从MainProgram中移除指定的层
        /// </summary>
        /// <param name="LayerHandle"></param>
        public void Remove(int LayerHandle)
        {
            if (LayerHandle >= 0 && Program.frmMain.MapMain.get_LayerPosition(LayerHandle) >= 0)
            {
                switch (this[LayerHandle].LayerType)
                {
                    case Interfaces.eLayerType.Grid:
                        try
                        {
                            ((MapWinGIS.Grid)(this[LayerHandle].GetGridObject)).Close();
                        }
                        catch { }
                        MapWinGIS.Image o = (MapWinGIS.Image)Program.frmMain.MapMain.get_GetObject(LayerHandle);
                        if (o != null)
                        {
                            o.Close();
                        }
                        o = null;
                        break;
                    case Interfaces.eLayerType.Image:
                        ((MapWinGIS.Image)(this[LayerHandle].GetObject())).Close();break;
                    case Interfaces.eLayerType.Invalid:
                        break;
                    default :
                        ((MapWinGIS.Shapefile)(this[LayerHandle].GetObject())).Close();break;
                }
                Program.frmMain.Legend.Layers.Remove(LayerHandle);
                Program.frmMain.m_PluginManager.LayerRemoved(LayerHandle);
                Program.frmMain.UpdateButtons();
                if (Program.frmMain.m_AutoVis[LayerHandle] != null)
                {
                    Program.frmMain.m_AutoVis.Remove(LayerHandle);
                }
            }
        }

        /// <summary>
        /// 如果指定层的handle属于一个有效层，则true
        /// </summary>
        /// <param name="LayerHandle">要检测的层的句柄(handle)</param>
        public bool IsValidHandle(int LayerHandle)
        {
            return Program.frmMain.Legend.Layers.IsValidHandle(LayerHandle);
        }

        /// <summary>
        /// 默认加载层，显示一个文件对话框
        /// </summary>
        public MapWinGIS.Interfaces.Layer[] Add()
        {
            m_PluginCall = true;
            Interfaces.Layer[] retval;
            object addedLayer = AddLayer(layerVisible: GetDefaultLayerVis());
            if (addedLayer is Interfaces.Layer[])
            {
                retval = (Interfaces.Layer[])addedLayer;
            }
            else
            {
                Interfaces.Layer[] newret = new Interfaces.Layer[1];
                newret[0] = (Interfaces.Layer)addedLayer;
                retval = newret;
            }
            m_PluginCall = false;
            return retval;
        }

        /// <summary>
        /// 从指定文件名中加载一个层
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(string Filename)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: Filename,layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 从指定的文件中，添加一个指定名字的层
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(string Filename, string LayerName)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: Filename, layerName: LayerName ,layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 从指定的文件中，加载一个指定名字和投影的层
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="layerName">The layer Name</param>
        /// <param name="geoProjection">The projection</param>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(string filename, string layerName, ref MapWinGIS.GeoProjection geoProjection)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: filename, layerName: layerName, layerVisible: GetDefaultLayerVis(), geoProjection: geoProjection)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Image类型的层到MainProgram中
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Image ImageObject)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ImageObject, layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个指定层名的Image类型层到MainProgram中
        /// </summary>
        /// <param name="ImageObject">Image类型的层对象</param>
        /// <param name="LayerName">加载的层的名字</param>
        /// <returns>Layer对象</returns>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Image ImageObject, string LayerName)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ImageObject, layerName: LayerName , layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Image类型的层到MainProgram中
        /// </summary>
        /// <param name="ImageObject">The image object</param>
        /// <param name="LayerName">Tha name of the layer</param>
        /// <param name="Visible">Visibility of the layer</param>
        /// <param name="TargetGroup">Add to which group, -1 means top group</param>
        /// <param name="LayerPosition">On what layer position, -1 means top position</param>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Image ImageObject, string LayerName, bool Visible, int TargetGroup, int LayerPosition)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ImageObject, layerName: LayerName, layerVisible: Visible, group: TargetGroup, layerPosition: LayerPosition)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个shapefile类型对象的图层到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ShapefileObject, layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个指定名字的shapefile类型对象的图层到MapMain中
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ShapefileObject, layerName: LayerName, layerVisible: GetDefaultLayerVis())[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个shapefile类型对象的图层到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName, int Color, int OutlineColor)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ShapefileObject, layerName: LayerName, layerVisible: GetDefaultLayerVis(), color: Color, outlineColor: OutlineColor)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个shapefile类型对象的图层到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName, int Color, int OutlineColor, int LineOrPointSize)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: ShapefileObject, layerName: LayerName, layerVisible: GetDefaultLayerVis(), color: Color, outlineColor: OutlineColor, lineOrPointSize: LineOrPointSize)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Grid图层对象到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Grid GridObject)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: GridObject)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Grid图层对象到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: GridObject, grdColorScheme: ColorScheme)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Grid图层对象到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, string LayerName)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: GridObject, layerName: LayerName)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个Grid图层对象到MapMain中
        /// </summary>
        public MapWinGIS.Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme, string LayerName)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: GridObject, layerName: LayerName, grdColorScheme: ColorScheme)[0];
            m_PluginCall = false;
            return addedLayer;
        }


        /// <summary>
        /// 获取或设置当前层的句柄
        /// </summary>
        public int CurrentLayer
        {
            get { return Program.frmMain.Legend.SelectedLayer; }
            set { Program.frmMain.Legend.SelectedLayer = value; }
        }

        /// <summary>
        /// 根据给定的图层的global(全局)位置，查找图层的句柄
        /// </summary>
        public int GetHandle(int GlobalPosition)
        {
            try
            {
                if (GlobalPosition < NumLayers && GlobalPosition >= 0)
                {
                    return Program.frmMain.MapMain.get_LayerHandle(GlobalPosition);
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
            }
            return -1;
        }

        /// <summary>
        /// 索引，根据LayerHandle获取指定的Layer
        /// </summary>
        public MapWinGIS.Interfaces.Layer this[int LayerHandle]
        {
            get
            {
                if (Program.frmMain.MapMain.NumLayers == 0)
                {
                    Program.g_error = "没有图层.";
                    return null;
                }

                if (!IsValidHandle(LayerHandle))
                {
                    Program.g_error = "无效的图层句柄（handle）" + LayerHandle.ToString() + "请求。";
                    return null;
                }
                MainProgram.Layer newlyr = new MainProgram.Layer();
                newlyr.Handle = LayerHandle;
                return newlyr;
            }
        }

        /// <summary>
        /// 获得所有已经加载到MainProgram中的层的数量，正在绘制的不算
        /// </summary>
        public int NumLayers
        {
            get { return Program.frmMain.MapMain.NumLayers; }
        }

        /// <summary>
        /// 用指定的GridColorScheme新建一个grid layer
        /// </summary>
        /// <param name="LayerHandle">Handle of the grid layer.</param>
        /// <param name="GridObject">Grid object corresponding to that layer handle.</param>
        /// <param name="ColorScheme">Coloring scheme to use when rebuilding.</param>
        /// <returns></returns>
        public bool RebuildGridLayer(int LayerHandle, MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme)
        {
            MapWinGIS.Image img = null;
            MapWinGIS.GridColorScheme NewScheme = null;
            string fileName;
            MapWinGIS.Image tmpImage;
            MapWinGIS.Utils gc = new MapWinGIS.Utils();
            bool oldUseTrans;
            uint oldTransColor;

            try
            {
                //确保在当前图层中的handle是有效的grid对象
                img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(LayerHandle));

                if (GridObject == null)
                {
                    Program.g_error = "RebuildGridLayer:GridObject参数是空'";
                    return false;
                }
                if (img == null)
                {
                    Program.g_error = Program.frmMain.MapMain.get_ErrorMsg(Program.frmMain.MapMain.LastErrorCode);
                    return false;
                }

                //确保GridColorScheme参数是有效的
                if (ColorScheme == null || ColorScheme.NumBreaks < 1)
                {
                    GenerateGridColorScheme(GridObject, NewScheme);
                }
                else
                {
                    NewScheme = ColorScheme;
                }

                //开始，创建新的image
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                try
                {
                    if (img.Filename.EndsWith(".tif") || img.Filename.EndsWith(".tiff") || img.Filename.EndsWith(".img"))
                    {
                        try
                        {
                            ((MapWinGIS.Image)(Program.frmMain.m_Layers[LayerHandle].GetObject()))._pushSchemetkRaster(ColorScheme);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                        }
                    }
                    else
                    {
                        fileName = img.Filename;
                        //确保filename是以bmp结尾，不去读取tif或bil格式的bitmap数据然后添加到tif或bil文件中
                        fileName = System.IO.Path.ChangeExtension(fileName, ".bmp");

                        oldUseTrans = img.UseTransparencyColor;
                        oldTransColor = img.TransparencyColor;
                        img.Close();
                        tmpImage = gc.GridToImage(GridObject, NewScheme);
                        tmpImage.Save(fileName, true, MapWinGIS.ImageType.BITMAP_FILE, this);
                        tmpImage.Close();
                        tmpImage = null;
                        img.Open(fileName, MapWinGIS.ImageType.BITMAP_FILE, false, this);
                        img.UseTransparencyColor = oldUseTrans;
                        img.TransparencyColor = oldTransColor;
                        img.TransparencyColor2 = oldTransColor;
                    }
                }
                finally
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
                Program.frmMain.View.Extents = Program.frmMain.View.Extents;
                Program.frmMain.MapMain.UpdateImage(LayerHandle);
                Program.frmMain.MapMain.Redraw();
                Application.DoEvents();

                //更新legend
                Program.frmMain.MapMain.SetImageLayerColorScheme(LayerHandle, NewScheme);
                Program.frmMain.MapMain.set_GridFileName(LayerHandle, GridObject.Filename);
                Program.frmMain.Legend.Layers.ItemByHandle(LayerHandle).Refresh();
                ColoringSchemeTools.ExportScheme(Program.frmMain.m_Layers[LayerHandle], Path.ChangeExtension(img.Filename, ".mwleg"));

                Progress("", 0, "");
                return true;
            }
            catch (Exception ex)
            {
                Program.g_error = ex.Message;
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取在legend中的组的属性
        /// </summary>
        public Interfaces.Groups Groups
        {
            get { return Program.frmMain.Legend.Groups; }
        }

        /// <summary>
        /// 通过filename添加一个层，名字指定，legend可见
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        public MapWinGIS.Interfaces.Layer Add(string Filename, string LayerName, bool VisibleInLegend)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: Filename, layerVisible: GetDefaultLayerVis(), legendVisible: VisibleInLegend)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 添加一个层到map中，层的位置是当前选择的层的上面，否则就是层列表的顶部
        /// </summary>
        /// <param name="Visible">添加的层是否可见</param>
        /// <param name="PlaceAboveCurrentlySelected">是将层放在当前选择的上面，还是顶部</param>
        /// <param name="Filename">The name of the file to add.</param>
        /// <param name="LayerName">层的名字，将显示在legend中</param>
        public MapWinGIS.Interfaces.Layer Add(string Filename, string LayerName, bool Visible, bool PlaceAboveCurrentlySelected)
        {
            m_PluginCall = true;
            Interfaces.Layer addedLayer = AddLayer(objectOrFilename: Filename, layerVisible: Visible, positionFromSelected: PlaceAboveCurrentlySelected)[0];
            m_PluginCall = false;
            return addedLayer;
        }

        /// <summary>
        /// 开始添加层对话框（Session）
        /// 在此期间，Projection MisMatch会检查，添加的层是否匹配。并且map,、legend 、buttons不会更新
        /// </summary>
        public void StartAddingSession()
        {
            m_mismatchTester = new MapWinGIS.Controls.Projections.MismatchTester(Program.frmMain);
            //设置鼠标，锁定地图和legend
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
            Program.frmMain.Legend.Lock();

            m_initCount = Program.frmMain.m_Layers.NumLayers;

            //若没有图层，确保没有存留的extent记录留在集合中
            if (Program.frmMain.MapMain.NumLayers == 0)
            {
                Program.frmMain.m_Extents.Clear();
                Program.frmMain.m_CurrentExtent = -1;
            }

        }

        /// <summary>
        /// 停止添加层对话框（Session）
        /// </summary>
        public void StopAddingSession()
        {
            this.StopAddingSession(m_initCount == 0 && Program.frmMain.m_Layers.NumLayers > 0);
        }

        /// <summary>
        /// 停止添加层对话框（Session）
        /// </summary>
        /// <param name="zoomToExtents">是否显示全图</param>
        public void StopAddingSession(bool zoomToExtents)
        {
            if (zoomToExtents)
            {
                Program.frmMain.MapMain.ZoomToMaxExtents();
            }
            if (m_newLayers.Count > 0)
            {
                Program.frmMain.m_PluginManager.LayersAdded(m_newLayers.ToArray());
            }
            //进度条完成，解锁mapmain和legend
            MapWinGIS.Utility.Logger.Progress(100, 100);
            Program.frmMain.MapMain.LockWindow(tkLockMode.lmUnlock);
            Program.frmMain.Legend.Unlock();
            Cursor.Current = Cursors.Default;
            Cursor.Show();
            Program.frmMain.Update();

            if (m_mismatchTester.FileCount > 0 && Program.frmMain.ApplicationInfo.ShowLoadingReport)
            {
                m_mismatchTester.ShowReport(Program.frmMain.m_Project.GeoProjection);
            }
            else
            {
                m_mismatchTester.HideProgress();
            }

            //清空
            m_initCount = 0;
            m_newLayers.Clear();
            m_mismatchTester = null;
        }

        #endregion 30

        #region MapWinGIS.ICallback接口实现
        /// <summary>
        /// 提供进度条，显示加载进度功能
        /// </summary>
        /// <param name="KeyOfSender">发送请求对象</param>
        /// <param name="Percent">进度比</param>
        /// <param name="Message">显示在状态栏的消息</param>
        public void Progress(string KeyOfSender, int Percent, string Message)
        { 
            try
            {
                if (m_mismatchTester == null)
                {
                    if (string.IsNullOrEmpty(Message))
                    {
                        MapWinGIS.Utility.Logger.Progress(Percent, 100);
                    }
                    else
                    {
                        MapWinGIS.Utility.Logger.Progress(Message, Percent, 100);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("进度条出现异常:" + ex.Message);
            }
        }

        public void Error(string KeyOfSender, string ErrorMsg)
        {
            Program.g_error = ErrorMsg; 
        }

        #endregion

        #region Add layer
        /// <summary>
        /// 添加一个图层
        /// </summary>
        internal Interfaces.Layer[] AddLayer(object objectOrFilename = null,
                                             string layerName = "",
                                             int group = -1,
                                             bool layerVisible = true,
                                             int color = -1,
                                             int outlineColor = -1,
                                             bool drawFill = true,
                                             float lineOrPointSize = 1,
                                             MapWinGIS.tkPointType pointType = 0,
                                             MapWinGIS.GridColorScheme grdColorScheme = null,
                                             bool legendVisible = true,
                                             bool positionFromSelected = false,
                                             int layerPosition = -1,
                                             bool loadXMLInfo = true,
                                             MapWinGIS.GeoProjection geoProjection = null)
        {
            Interfaces.Layer[] layers = null;
            //文件名为空，则打开选择文件对话框
            if (objectOrFilename == null || (objectOrFilename.GetType() == typeof(string) && Convert.ToString(objectOrFilename) == ""))
            {
                string[] filenames = ShowLayerDialog();
                if (filenames == null)
                {
                    return null;
                }
                layers = this.AddLayerFromFilename(filenames, layerName, group, layerVisible, grdColorScheme, legendVisible, positionFromSelected, layerPosition, loadXMLInfo);
            }
            else //给定了文件对象
            {
                if (objectOrFilename.GetType().ToString() == "System.String") //文件名存在则从指定的位置添加图层
                {
                    string[] filenames = new string[] { objectOrFilename.ToString() };
                    layers = this.AddLayerFromFilename(filenames, layerName, group, layerVisible, grdColorScheme, legendVisible, positionFromSelected, layerPosition, loadXMLInfo, geoProjection);
                }
                else //objectOrFilename存在但不是string，则打开添加图层对话框
                {
                    bool addingSession = m_mismatchTester != null;
                    try
                    {
                        if (!addingSession)
                        {
                            this.StartAddingSession();
                        }
                        MapWinGIS.Controls.LayerSource source = new MapWinGIS.Controls.LayerSource(objectOrFilename);
                        if (source.Type == LayerSourceType.Undefined)
                        {
                            MapWinGIS.Utility.Logger.Message("文件不存在: " + objectOrFilename, "MapWin.Layers.AddLayer");
                            return null;
                        }
                        // 投影检测，是tiles类型则关闭
                        bool titleLayer = objectOrFilename is MapWinGIS.Image && layerName.StartsWith("mwTile-");

                        if (!titleLayer)
                        {
                            MapWinGIS.Controls.LayerSource sourceTemp = null;
                            if (m_mismatchTester.TestLayer(source, out sourceTemp) == MapWinGIS.Controls.Projections.TestingResult.Substituted)
                            {
                                source = sourceTemp;
                            }
                        }

                        Interfaces.Layer layer = this.AddLayerCore(source.GetObject, layerName, group, layerVisible, grdColorScheme, legendVisible, positionFromSelected, layerPosition, loadXMLInfo);
                        if (layer != null)
                        {
                            m_newLayers.Add(layer);
                        }
                        layers = (Interfaces.Layer[])Array.CreateInstance(typeof(Interfaces.Layer), 1);
                        layers[0] = layer;
                    }
                    finally
                    {
                        //必须关掉对话框
                        if (!addingSession)
                        {
                            this.StopAddingSession();
                        }
                    }
                }
            }

            //属性应该应用在最新添加的图层上
            for (int i = 0; i < layers.Length; i++)
            {
                int handle = layers[i].Handle;
                Interfaces.Layer layer = Program.frmMain.m_Layers[handle];
                if (layer != null)
                {
                    layer.UseLabelCollision = Program.appInfo.ShowAvoidCollision;
                }
            }
            //设置图层的属性
            if (layers != null && layers.Length == 1)
            {
                this.SetLayerProperties(layers[0], color, outlineColor, drawFill, lineOrPointSize, pointType);
            }
            return layers;
        }

        /// <summary>
        /// 从指定文件路径添加一个或多个图层到地图上
        /// </summary>
        private Interfaces.Layer[] AddLayerFromFilename(string[] filenames,
                                                        string LayerName = "",
                                                        int Group =-1,
                                                        bool LayerVisible = true,
                                                        MapWinGIS.GridColorScheme GrdColorScheme = null,
                                                        bool LegendVisible = true,
                                                        bool PositionFromSelected = false,
                                                        int LayerPosition= -1,
                                                        bool LoadXMLInfo = true,
                                                        MapWinGIS.GeoProjection geoProjection = null)
        {
            if (filenames == null || filenames.Length == 0)
            {
                return null;
            }
            bool addingSession = m_mismatchTester != null;

            List<Interfaces.Layer> layers = new List<Interfaces.Layer>();

            try
            {
                if (!addingSession)
                {
                    this.StartAddingSession();
                }
                //遍历每一个图层文件
                for (int i = 0; i < filenames.Length; i++)
                {

                    MapWinGIS.Controls.Projections.TestingResult result = MapWinGIS.Controls.Projections.TestingResult.Ok;
                    string newName = filenames[i];

                    // 投影检测
                    if (geoProjection == null)
                    {
                        result = m_mismatchTester.TestLayer(filenames[i], out newName);
                        if (result != MapWinGIS.Controls.Projections.TestingResult.Substituted)
                        {
                            newName = filenames[i];
                        }
                    }

                    if ((result == MapWinGIS.Controls.Projections.TestingResult.Ok) || (result == MapWinGIS.Controls.Projections.TestingResult.Substituted))
                    {

                        Interfaces.Layer layer = AddLayerCore(newName, LayerName, Group, LayerVisible, GrdColorScheme, LegendVisible, PositionFromSelected, LayerPosition, LoadXMLInfo, true, geoProjection);

                        if (layer != null)
                        {
                            m_newLayers.Add(layer);
                            layers.Add(layer);
                        }

                        if (filenames.Length > 1)
                        {
                            MapWinGIS.Utility.Logger.Progress(i, filenames.Length);
                        }
                    }
                    else if (result == MapWinGIS.Controls.Projections.TestingResult.CancelOperation)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            finally
            {
                //必须关掉对话框
                if (!addingSession)
                {
                    this.StopAddingSession();
                }
            }
            return layers.ToArray();
        }
        
        /// <summary>
        /// 添加一个图层到地图上的核心方法。
        /// objectOrFilename可以是一个图层文件路径，也可以是shapefile, image or grid类型的文件
        /// </summary>
        private Interfaces.Layer AddLayerCore(object ObjectOrFilename = null,
                                              string LayerName = "",
                                              int Group = -1,
                                              bool LayerVisible = true,
                                              MapWinGIS.GridColorScheme GrdColorScheme = null,
                                              bool LegendVisible = true,
                                              bool PositionFromSelected = false,
                                              int LayerPosition = -1,
                                              bool LoadXMLInfo = true,
                                              bool BatchMode = false,
                                              MapWinGIS.GeoProjection geoProjection= null)
        {
            string lyrFilename = ""; // 包含完整路径的图层文件名
            string lyrName; // 图层文件名
            Interfaces.eLayerType lyrType = Interfaces.eLayerType.Invalid;

            object newObject = null;
            MapWinGIS.Grid newGrid = new MapWinGIS.Grid();
            Interfaces.Layer newLyr = null;

            int groupCount = Program.frmMain.Legend.Groups.Count;
            // 1.确定图层类型，并设置好相关属性。
            if (ObjectOrFilename != null && ObjectOrFilename.GetType().ToString() == "System.String") //是图层文件路径名
            {
                if (ObjectOrFilename.ToString().ToLower().StartsWith("ecwp://"))
                {
                    lyrName = ObjectOrFilename.ToString();
                }
                else
                {
                    if (!File.Exists(ObjectOrFilename.ToString()))
                    {
                        MapWinGIS.Utility.Logger.Message("文件不存在: " + ObjectOrFilename, "MapWinGIS.MainProgram.Layers.AddLayer");
                        return null;
                    }

                    if (LayerName == "")
                    {
                        lyrName = MapWinGIS.Utility.MiscUtils.GetBaseName(ObjectOrFilename.ToString());
                    }
                    else
                    {
                        lyrName = LayerName;
                    }
                }

                lyrFilename = ObjectOrFilename.ToString();

                //打开文件
                Program.frmMain.m_PluginManager.BroadcastMessage("Layer addition initiated, file name :" + lyrFilename);
                newObject = OpenDataFile(ref lyrFilename, ref newGrid, ref GrdColorScheme, ref lyrType, geoProjection);
                if (newObject == null)
                {
                    return null;
                }
            }
            else //是shapefile、image、grid类型文件
            {
                newObject = ObjectOrFilename;

                // 提取名字
                MapWinGIS.Controls.LayerSource obj = new MapWinGIS.Controls.LayerSource(newObject);
                if (obj.Type == MapWinGIS.Controls.LayerSourceType.Undefined)
                {
                    MapWinGIS.Utility.Logger.Message("添加的对象不支持", "MapWin.Layers.AddLayer");
                    return null;
                }
                lyrFilename = obj.Filename;

                if (LayerName != "")
                {
                    lyrName = LayerName;
                }
                else
                {
                    if (lyrFilename != "")
                    {
                        lyrName = MapWinGIS.Utility.MiscUtils.GetBaseName(lyrFilename);
                    }
                    else
                    {
                        //如果没有提取到名字，则生成一个
                        lyrName = "Layer " + addCnt.ToString();
                        addCnt++;
                    }
                }

                //为grids对象创建一个bitmap
                MapWinGIS.Grid gridObject = newObject as MapWinGIS.Grid;
                if (gridObject != null)
                {
                    MapWinGIS.Image image = this.OpenGridCore(lyrFilename, ref gridObject, ref GrdColorScheme); //以image的方式打开grid对象
                    if (image != null)
                    {
                        newGrid = gridObject;
                        lyrType = Interfaces.eLayerType.Grid;
                        newObject = image;
                    }
                    else
                    {
                        return null;
                    }
                }

            } 

            //2.将object对象添加到地图上
            int mapHandle;
            if (PositionFromSelected && Program.frmMain.Legend.SelectedLayer != -1)
            {
                //添加到当前选择的层的上面
                int addPos = Program.frmMain.Legend.Layers.PositionInGroup(Program.frmMain.m_Layers.CurrentLayer) + 1;
                int addGrp = Program.frmMain.Legend.Layers.GroupOf(Program.frmMain.m_Layers.CurrentLayer);

                mapHandle = Program.frmMain.Legend.Layers.Add(LegendVisible, newObject, LayerVisible);
                Program.frmMain.Legend.Layers.MoveLayer(mapHandle, addGrp, addPos);
            }
            else
            {
                mapHandle = Program.frmMain.Legend.Layers.Add(LegendVisible, newObject, LayerVisible);
            }

            if (!IsValidHandle(mapHandle))
            {
                MapWinGIS.Utility.Logger.Dbg("打开图层失败: " + ObjectOrFilename.ToString());
                return null;
            }

            Program.frmMain.SetModified(true); // 图层添加成功，更新显示

            //处理图层的位置
            if (LayerPosition > -1 && Group > -1)
            {
                Program.frmMain.Legend.Layers.MoveLayer(mapHandle, Group, LayerPosition);
            }

            // 3.若是grid类型图层，则保存grid对象：lyrTye保存的时候会被忽视，legend可以推测出该类型
            if (lyrType == Interfaces.eLayerType.Grid)
            {
                if (m_Grids.Contains(mapHandle))
                {
                    m_Grids.Remove(mapHandle);
                }
                m_Grids.Add(mapHandle, newGrid);
                Program.frmMain.Legend.Layers.ItemByHandle(mapHandle).Type = Interfaces.eLayerType.Grid;
            }

            // 4.从mwleg文件中导入颜色配置
            if (lyrType == Interfaces.eLayerType.Grid && GrdColorScheme == null && IsValidHandle(mapHandle))
            {
                string name = Path.ChangeExtension(lyrFilename, ".mwleg");
                if (File.Exists(name))
                {
                    object obj = ColoringSchemeTools.ImportScheme(Program.frmMain.Layers[mapHandle], name);
                    GrdColorScheme = (MapWinGIS.GridColorScheme)obj;
                }
            }

            //5.创建layer对象
            newLyr = new MapWinGIS.MainProgram.Layer();
            newLyr.Handle = mapHandle;
            newLyr.Name = lyrName;

            //6.从.mwsymb 或.mwsr文件中加载渲染(rendering)信息
            if (LoadXMLInfo)
            {
                if (Program.appInfo.SymbologyLoadingBehavior == Interfaces.SymbologyBehavior.RandomOptions)
                {
                    //不处理，所有的代码移植到ocx中去了
                }
                else if (Program.appInfo.SymbologyLoadingBehavior == Interfaces.SymbologyBehavior.DefaultOptions)
                {
                    if (newLyr.LayerType == Interfaces.eLayerType.LineShapefile || newLyr.LayerType == Interfaces.eLayerType.PointShapefile || newLyr.LayerType == Interfaces.eLayerType.PolygonShapefile)
                    {
                        //首先尝试加载.mwsymb类型文件 （.mwsymb文件将lbl和mwsr合并了）(.shp.mwsymb and .shp.view-deafult.mwsymb)
                        //在没有mwsymb类型文件之前，不能正确加载颜色设置。而在源程序中能。
                        string description = "";
                        bool res = Program.frmMain.MapMain.LoadLayerOptions(mapHandle, "", ref description); //该处加载了相应的图层颜色什么的设置

                        //如果不能加载mwsymb文件，则尝试加载旧文件格式的文件.mwsr、.lbl
                        if (!res)
                        {
                            // .mwsr
                            Program.frmMain.LoadRenderingOptions(newLyr.Handle, "", m_PluginCall);
                            // .lbl
                            MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)newObject;
                            string name = System.IO.Path.ChangeExtension(sf.Filename, ".lbl");
                            sf.Labels.LoadFromXML(name);
                        }

                    }
                    else if (newLyr.LayerType == Interfaces.eLayerType.Grid) 
                    {
                        // 首先尝试加载.mwsymb文件
                        string description = "";
                        bool res = Program.frmMain.MapMain.LoadLayerOptions(mapHandle, "", ref description);

                        // 无法加载，则尝试旧格式的文件
                        if (LayerName == "" && !res)
                        {
                            Program.frmMain.LoadRenderingOptions(newLyr.Handle, "", m_PluginCall);
                        }
                    }
                }
                else if (Program.appInfo.SymbologyLoadingBehavior == Interfaces.SymbologyBehavior.UserPrompting)
                {
                    Program.frmMain.m_PluginManager.BroadcastMessage("SYMBOLOGY_CHOOSE:" + mapHandle.ToString() + "!" + lyrFilename);

                    int position = Program.frmMain.MapMain.get_LayerPosition(newLyr.Handle);
                    if (position < 0)
                    {
                        //若不能获取图层位置，则清空所有组和层
                        if (groupCount == 0 && Program.frmMain.Legend.Groups.Count > 0)
                        {
                            Program.frmMain.Legend.Groups.Clear();
                        }
                        newLyr = null;
                        return null;
                    }
                }
            }

            //7.保存grids的渲染信息
            if (GrdColorScheme != null)
            {
                Program.frmMain.MapMain.SetImageLayerColorScheme(mapHandle, GrdColorScheme);
                Program.frmMain.MapMain.set_GridFileName(mapHandle, lyrFilename);

                MapWinGIS.Image img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(mapHandle));
                if (img != null)
                {
                    img.TransparencyColor = GrdColorScheme.NoDataColor;
                    img.TransparencyColor2 = GrdColorScheme.NoDataColor;
                    img.UseTransparencyColor = true;
                    img = null;
                }
                Program.frmMain.Legend.Layers.ItemByHandle(mapHandle).Refresh();

                string imgName = ((MapWinGIS.Image)newObject).Filename;
                ColoringSchemeTools.ExportScheme(Program.frmMain.Layers[mapHandle], Path.ChangeExtension(imgName, ".mwleg"));
            }

            //清理资源
            newObject = null;
            GC.Collect();
            return newLyr;
        }

        #endregion

        #region OpenDatafile
        /// <summary>
        /// 打开数据文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="newGrid"></param>
        /// <param name="GrdColorScheme"></param>
        /// <param name="layerType"></param>
        /// <param name="geoProjection"></param>
        /// <returns></returns>
        private object OpenDataFile(ref string filename, ref MapWinGIS.Grid newGrid, 
            ref MapWinGIS.GridColorScheme GrdColorScheme, ref MapWinGIS.Interfaces.eLayerType layerType, 
            MapWinGIS.GeoProjection geoProjection = null)
        {
            //从image中获取对话框，然后看扩展名是否在对话框标识的范围内
            string ExtName = MapWinGIS.Utility.MiscUtils.GetExtensionName(filename).ToLower();
            MapWinGIS.Image newImage = new MapWinGIS.Image();

            if (filename.ToLower().EndsWith(".mwsymb")) //添加图层文件 (.mwlyr)
            {
                // .shp .mwsymb
                string name = filename.Substring(0, filename.Length - 7);
                if (!File.Exists(name))
                {
                    int pos = name.LastIndexOf(".");
                    if (pos > 0)
                    {
                        name = name.Substring(0, pos);
                    }
                }

                if (File.Exists(name))
                {
                    string options = filename.Substring(name.Length); // 去掉文件名部分
                    options = options.Substring(0, options.Length - 7); // 去掉扩展名部分
                    if (options.Length > 0)
                    {
                        options = options.Substring(1);
                    }

                    // 递归调用
                    Interfaces.Layer layer = this.AddLayerCore(ObjectOrFilename: name, LoadXMLInfo: false);
                    if (layer != null)
                    {
                        string description = "";
                        Program.frmMain.MapMain.LoadLayerOptions(layer.Handle, options, ref description);
                    }

                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                    Program.frmMain.Legend.Refresh();
                    return null;
                }
                else
                {
                    MessageBox.Show("找不到指定的数据集加载: " + Environment.NewLine + name, Program.frmMain.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            if (filename.ToLower().IndexOf(".wmf") + 1 != 0) // 将wmf转换成bitmap
            {
                System.Drawing.Bitmap cvter = new System.Drawing.Bitmap(filename);
                filename = Program.GetMWGTempFile() + ".bmp";
                ExtName = "bmp";
                cvter.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
            }

            if (ExtName.ToLower().EndsWith("flt") || ExtName.ToLower().EndsWith("grd")) // 转换flt文件
            {
                string bgdequiv = System.IO.Path.ChangeExtension(filename, ".bgd");
                if (File.Exists(bgdequiv) && MapWinGIS.Utility.DataManagement.CheckFile2Newest(filename, bgdequiv))
                {
                    filename = bgdequiv;
                }
                else
                {
                    //转换它
                    SuperGrid cvg = new SuperGrid();
                    if (cvg.Open(filename))
                    {
                        //Opening it will cause the equiv. bgd to be written
                        cvg.Close();
                        filename = bgdequiv;
                    }
                }
            }

            // 处理aux文件
            try
            {
                System.IO.FileStream lyrFileStm = System.IO.File.OpenRead(filename);
                if (lyrFileStm != null)
                {
                    byte[] header = new byte[11];
                    lyrFileStm.Read(header, 0, 11);
                    lyrFileStm.Close();
                    System.Text.ASCIIEncoding headerEncoding = new System.Text.ASCIIEncoding();
                    string strheader = headerEncoding.GetString(header);
                    if (strheader == "EHFA_HEADER") //打开sta.adf文件
                    {
                        if (File.Exists(filename.Substring(0, filename.Length - 4) + "\\sta.adf"))
                        {
                            filename = filename.Substring(0, filename.Length - 4) + "\\sta.adf";
                        }
                    }
                }
            }
            catch (Exception)
            {
                //这只是一个小的测试，不显示错误
            }

             //打开不同类型的文件，若打不开，最后尝试用iamge打开一次
            if (filename.ToLower().IndexOf(".adf") + 1 != 0 || filename.ToLower().IndexOf(".asc") + 1 != 0) //打开 .adf 和 .asc 文件 (ESRI grids)
            {
                newImage = OpenEsriGrid(filename, ref newGrid, ref GrdColorScheme);
                if (newImage != null)
                {
                    layerType = Interfaces.eLayerType.Grid;
                    return newImage;
                }
                else
                {
                    // 后面将尝试用iamge打开
                }

            } //打开 tif, img, dem, flt, grd 文件
            else if (filename.ToLower().IndexOf(".tif") + 1 != 0 || filename.ToLower().IndexOf(".img") + 1 != 0 || filename.ToLower().IndexOf(".dem") + 1 != 0 || filename.ToLower().IndexOf(".flt") + 1 != 0 || filename.ToLower().IndexOf(".grd") + 1 != 0)
            {
                newImage = OpenTiffAsGridFile(ref filename, ref newGrid, ref GrdColorScheme);
                if (newImage != null)
                {
                    layerType = Interfaces.eLayerType.Grid;
                    return newImage;
                }
                else
                {
                    // 后面将尝试用iamge打开
                }

            }//打开 shapefiles
            else if (filename.ToLower().IndexOf(".shp") + 1 != 0)
            {

                MapWinGIS.Shapefile sf;
                sf = OpenShapefile(filename);

                // 跳过投影:
                if (geoProjection != null && sf.GeoProjection.IsEmpty)
                {
                    sf.GeoProjection = geoProjection;
                }
                return sf;
            }//打开 grids
            else if ((newGrid.CdlgFilter.ToLower()).IndexOf(ExtName) + 1 != 0 || ExtName.ToLower().EndsWith("flt") || ExtName.ToLower().EndsWith("dem") || ExtName.ToLower().EndsWith("grd"))
            {

                layerType = Interfaces.eLayerType.Grid;
                newImage = OpenGrid(filename, ref newGrid, ref GrdColorScheme);
                return newImage;
            }

            // 打开images
            MapWinGIS.Image img = new MapWinGIS.Image();
            if ((img.CdlgFilter.ToLower()).IndexOf(ExtName) + 1 != 0)
            {
                if (!img.Open(filename, MapWinGIS.ImageType.USE_FILE_EXTENSION, false, ((MapWinGIS.ICallback)this)))
                {
                    MapWinGIS.ImageType newExt = GetImageType(filename);
                    // 重新以image打开一次
                    if (!img.Open(filename, newExt, false, ((MapWinGIS.ICallback)this)))
                    {
                        MapWinGIS.Utility.Logger.Msg("打开文件失败: " + filename, "MapWin.Layers.AddLayer");
                        return null;
                    }
                    else
                    {
                        return img;
                    }
                }
                else
                {
                    return img;
                }
            }
            else
            {
                //不支持的格式
                MapWinGIS.Utility.Logger.Msg("文件格式不支持: " + ExtName, "MapWin.Layers.AddLayer");
                return null;
            }
        }

        /// <summary>
        /// 根据文件路径名，获取图片的格式
        /// </summary>
        private MapWinGIS.ImageType GetImageType(string filename)
        {
            MapWinGIS.ImageType imageType = ImageType.USE_FILE_EXTENSION;
            try
            {
                System.Drawing.Bitmap bitMap = new System.Drawing.Bitmap(filename);
                System.Drawing.Imaging.ImageFormat bmpFormat = bitMap.RawFormat;

                if (bmpFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                {
                    imageType = ImageType.GIF_FILE;
                }
                else if (bmpFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                {
                    imageType = ImageType.JPEG_FILE;
                }
                else if (bmpFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                {
                    imageType = ImageType.PNG_FILE;
                }
                else
                {
                    imageType = ImageType.TIFF_FILE;
                }
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Message("在获取图像格式时出错：" + ex.ToString());
            }
            return imageType;
        }

        #endregion

        #region Open grid
        /// <summary>
        /// 未实现
        /// </summary>
        private MapWinGIS.Image OpenEsriGrid(string filename, ref MapWinGIS.Grid grid, ref MapWinGIS.GridColorScheme GrdColorScheme)
        {
            return null;
        }

        /// <summary>
        /// 未实现
        /// </summary>
        private MapWinGIS.Image OpenTiffAsGridFile(ref string filename, ref MapWinGIS.Grid grid, ref MapWinGIS.GridColorScheme GrdColorScheme)
        {
            return null;
        }

        /// <summary>
        /// 未实现
        /// </summary>
        private MapWinGIS.Image OpenGrid(string filename, ref MapWinGIS.Grid grid, ref MapWinGIS.GridColorScheme GrdColorScheme)
        {
            return null;
        }

        /// <summary>
        /// 未实现
        /// </summary>
        private MapWinGIS.Image OpenGridCore(string filename, ref MapWinGIS.Grid grid, ref MapWinGIS.GridColorScheme GrdColorScheme)
        {
            return null;
        }

        /// <summary>
        /// 未实现
        /// </summary>
        private bool LoadingTIForIMGasGrid(string fn)
        {
            return false;
        }
        #endregion

        #region Open shapefile
        /// <summary>
        /// 打开shapefile文件
        /// </summary>
        private MapWinGIS.Shapefile OpenShapefile(string filename)
        {
           FileInfo fi = new FileInfo(filename);
           if (fi.Attributes == FileAttributes.ReadOnly) //检测文件是否为只读,
           {
               MapWinGIS.Utility.Logger.Message("shapefile文件为只读！");
               return null;
               //string LastPath = "";
               //YesNoToAll.DialogResult LastAnswer = YesNoToAll.DialogResult.Undefined;
               //if (LastAnswer == YesNoToAll.DialogResult.NoToAll)
               //{
               //    //不做任何事
               //}
               //else if (LastAnswer == YesNoToAll.DialogResult.YesToAll && LastPath != "")
               //{
               //    //只处理，不显示对话框
               //    MapWinGIS.GeoProcess.DataManagement.CopyShapefile(filename, LastPath + "\\" + System.IO.Path.GetFileName(filename));
               //    filename = LastPath + "\\" + System.IO.Path.GetFileName(filename);
               //}
               //else
               //{
               //    LastAnswer = YesNoToAll.ShowPrompt("警告: 您添加的层是只读的. 您希望在使用前将其复制到另外一个位置?", "只读图层 - 复制?");
               //    if (LastAnswer == YesNoToAll.DialogResult.Yes || LastAnswer == YesNoToAll.DialogResult.YesToAll)
               //    { 
               //        FolderBrowserDialog fb = new FolderBrowserDialog();
               //        fb.SelectedPath = Program.appInfo.DefaultDir;
               //        if (fb.ShowDialog() == DialogResult.OK)
               //        {
               //            //MapWinGIS.GeoProcess.DataManagement.CopyShapefile(filename, fb.SelectedPath + "\\" + System.IO.Path.GetFileName(filename));
               //            filename = fb.SelectedPath + "\\" + System.IO.Path.GetFileName(filename);
               //            LastPath = fb.SelectedPath;
               //        }
               //    }
               //}
           }

           //打开
           MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
           if (sf.Open(filename, ((MapWinGIS.ICallback)this)) == false) //此处打开了shapefile文件
           {
               MapWinGIS.Utility.Logger.Message("打开shapefile文件失败: " + sf.ErrorMsg[sf.LastErrorCode], "Layers.AddLayer");
           }

            //做基本的检测，包括：dbf是否存在，shape的数量是否和行数一直
           bool abort = false;
           TestShapefile(sf, ref abort);

           if (abort)
           {
               TryCloseObject((object)sf);
               MapWinGIS.Utility.Logger.Message("打开shapefile文件失败: ", "Layers.AddLayer");
               return null;
           }
           else
           {
               return sf;
           }
        }
        
        /// <summary>
        /// 设置图层的属性
        /// </summary>
        private void SetLayerProperties(MapWinGIS.Interfaces.Layer layer, int Color = -1, int OutlineColor = -1, bool DrawFill = true, float LineOrPointSize = 1, MapWinGIS.tkPointType PointType = 0)
        {
            if (Program.frmMain.MapMain.ShapeDrawingMethod != tkShapeDrawingMethod.dmNewSymbology)
            {
                layer.DrawFill = DrawFill;
                //颜色设置
                if (Color == -1)
                {
                    layer.Color = MakeRandomColor();
                }
                else
                {
                    layer.Color = ColorScheme.IntToColor(Color);
                }
                //轮廓线颜色
                if (OutlineColor == -1)
                {
                    layer.OutlineColor = MakeRandomColor();
                }
                else
                {
                    layer.OutlineColor = ColorScheme.IntToColor(OutlineColor);
                }
                // 点或线的大小
                if (layer.LayerType == Interfaces.eLayerType.PointShapefile)
                {
                    if (PointType == tkPointType.ptUserDefined)
                    {
                        layer.LineOrPointSize = LineOrPointSize;
                    }
                    else
                    {
                        if (LineOrPointSize == 1)
                        {
                            layer.LineOrPointSize = 3;
                        }
                        else
                        {
                            layer.LineOrPointSize = LineOrPointSize;
                        }
                    }
                }
                else
                {
                    layer.LineOrPointSize = LineOrPointSize;
                }

                // 点类型
                layer.PointType = PointType;
            }
        }

        /// <summary>
        /// 检测shapefile文件对应的dbf数据表是否存在，并且与shapefile的数量是否一直
        /// </summary>
        private void TestShapefile(MapWinGIS.Shapefile sf, ref bool abort)
        {
            abort = false;
            if (!File.Exists(Path.ChangeExtension(sf.Filename, ".dbf")))
            {
                if (MapWinGIS.Utility.Logger.Message("该shapefile没有数据表文件(.dbf) /r/n 你希望继续加载该文件吗？", "shapefile无数据表", MessageBoxButtons.YesNo, MessageBoxIcon.Information, DialogResult.Yes) == DialogResult.Yes)
                {
                    abort = true;
                }
                return;
            }
            MapWinGIS.Table tbl = new MapWinGIS.Table();
            tbl.Open(Path.ChangeExtension(sf.Filename, ".dbf"));
            if (sf.NumShapes != tbl.NumRows)
            {
                if (MapWinGIS.Utility.Logger.Message("该shapefile的数据表文件(.dbf)个数与shapefile文件个数不相等 /r/n 你希望继续加载该文件吗？", "shapefile无数据表", MessageBoxButtons.YesNo, MessageBoxIcon.Information, DialogResult.Yes) == DialogResult.Yes)
                {
                    abort = true;
                }
                tbl.Close();
                return;
            }
            else
            {
                tbl.Close();
            }
        }

        #endregion

        #region  Utilities
        /// <summary>
        /// 获取层的默认可见性
        /// </summary>
        public static bool GetDefaultLayerVis()
        {
            int grp = -1;
            if (Program.frmMain.Legend.SelectedLayer != -1)
            {
                grp = Program.frmMain.Legend.Layers.GroupOf(Program.frmMain.m_Layers.CurrentLayer);
            }
            else if (Program.frmMain.Legend.Groups.Count > 0)
            {
                grp = 0;
            }

            if (grp == -1)
            {
                return true;
            }
            LegendControl.Group oGrp = (LegendControl.Group)Program.frmMain.Legend.Groups.ItemByHandle(grp);
            if (oGrp == null)
            {
                return true;
            }
            return oGrp.LayersVisible;
        }

        /// <summary>
        /// 生成一个随机的颜色
        /// </summary>
        private System.Drawing.Color MakeRandomColor()
        {
            Random random = new Random();
            int r = random.Next();
            int g = random.Next();
            int b = random.Next();
            return System.Drawing.Color.FromArgb(255 * r, 255 * g, 255 * b);
        }

        /// <summary>
        /// 获取支持的格式字符串
        /// </summary>
        public string GetSupportedFormats()
        {
            GridUtils GridUtil = new GridUtils();
            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            MapWinGIS.Image img = new MapWinGIS.Image();

            string[] vArr = null;
            ArrayList allNames = new ArrayList();
            ArrayList allVals = new ArrayList();
            int i;

            //shapefile支持的格式
            vArr = sf.CdlgFilter.Split('|');
            for (i = 0; i < vArr.Length; i += 2)
            {
                if (vArr[i].Substring(0, "all supported".Length).ToLower() != "all supported" && !allVals.Contains(vArr[i + 1]) && !allNames.Contains(vArr[i]))
                {
                    allNames.Add(vArr[i]); // value
                    allVals.Add(vArr[i + 1]); // key
                }
            }

            //Grid支持的格式
            vArr = GridUtil.GridCdlgFilter().Split('|');
            GridUtil = null;
            for (i = 0; i < vArr.Length; i += 2)
            {
                if (vArr[i].Substring(0, "all supported".Length).ToLower() != "all supported" && !allVals.Contains(vArr[i + 1]) && !allNames.Contains(vArr[i]))
                {
                    allNames.Add(vArr[i]); // value
                    allVals.Add(vArr[i + 1]); // key
                }
            }

            //img支持的格式
            vArr = img.CdlgFilter.Split('|');
            for (i = 0; i < vArr.Length ; i += 2)
            {
                if (vArr[i].Substring(0, "all supported".Length).ToLower() != "all supported" && !allVals.Contains(vArr[i + 1]) && !allNames.Contains(vArr[i]))
                {
                    if (vArr[i].Substring(0, "MRSID".Length) != "MrSID") //Fix the dumplicate SID problem.
                    {
                        allNames.Add(vArr[i]);
                        allVals.Add(vArr[i + 1]);
                    }
                }
            }

            allNames.Add("Windows Metafile (*.wmf)");
            allVals.Add("*.WMF");

            object[] keys = null;
            keys = allVals.ToArray();

            string allExtensions = "";
            string allTypes = "";

            for (i = 0; i < keys.Length; i++)
            {
                string keyi = keys[i].ToString();
                if (allExtensions.Length == 0)
                {
                    if (keyi.Substring(keyi.Length - 1, 1) == ";")
                    {
                        allExtensions = keyi.Substring(0, keyi.Length - 1).Trim();
                    }
                    else
                    {
                        allExtensions = keyi.Trim();
                    }
                }
                else
                {
                    if (keyi.Substring(1) == ";")
                    {
                        allExtensions += ";" + keyi.Substring(0, keyi.Length - 1).Trim();
                    }
                    else
                    {
                        allExtensions += ";" + keyi.Trim();
                    }
                }


                if (allTypes.Length == 0)
                {
                    allTypes = allNames[allVals.IndexOf(keys[i])].ToString() + "|" + keyi.Trim();
                }
                else
                {
                    allTypes += "|" + allNames[allVals.IndexOf(keys[i])].ToString().Trim() + "|" + keyi.Trim();
                }
            }

            return "All supported formats|" + allExtensions + "|" + allTypes;
        }

        /// <summary>
        /// 关闭Grid、shapefile、Image等图层对象
        /// </summary>
        private void TryCloseObject(object newObject)
        {
            try
            {
                if (newObject != null)
                {
                    if (newObject is MapWinGIS.Grid)
                    {
                        ((MapWinGIS.Grid)newObject).Close();
                    }
                    else if (newObject is MapWinGIS.Shapefile)
                    {
                        ((MapWinGIS.Shapefile)newObject).Clone();
                    }
                    else if (newObject is MapWinGIS.Image)
                    {
                        ((MapWinGIS.Image)newObject).Close();
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 显示添加图层对话框
        /// </summary>
        private string[] ShowLayerDialog()
        {
            OpenFileDialog cdlOpen = new OpenFileDialog();

            if (Directory.Exists(Program.appInfo.DefaultDir))
            {
                cdlOpen.InitialDirectory = Program.appInfo.DefaultDir;
            }
            cdlOpen.FileName = "";
            cdlOpen.Title = "添加图层";
            cdlOpen.Filter = GetSupportedFormats();
            cdlOpen.CheckFileExists = true;
            cdlOpen.CheckPathExists = true;
            cdlOpen.Multiselect = true;
            cdlOpen.ShowReadOnly = false;

            string[] filenames = null;
            if (cdlOpen.ShowDialog() == DialogResult.Cancel)
            {
                return null;
            }
            else
            {
                filenames = cdlOpen.FileNames;
            }

            //保存默认路径
            if (File.Exists(cdlOpen.FileName))
            {
                string dir = Path.GetDirectoryName(cdlOpen.FileName);
                if (Directory.Exists(dir))
                {
                    Program.appInfo.DefaultDir = Path.GetDirectoryName(cdlOpen.FileName);
                }
            }

            cdlOpen.Dispose();
            Program.frmMain.Update();
            return filenames;
        }

        #endregion

        #region Grid 功能
        /// <summary>
        /// 未实现
        /// </summary>
        /// <param name="newGrid"></param>
        /// <param name="GrdColorScheme"></param>
        private void GenerateGridColorScheme(MapWinGIS.Grid newGrid, MapWinGIS.GridColorScheme GrdColorScheme)
        {
        }
        /// <summary>
        /// 未实现
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="newImage"></param>
        /// <param name="newGrid"></param>
        /// <param name="GrdColorScheme"></param>
        /// <param name="cb"></param>
        public void GetImageRep(string filename, ref MapWinGIS.Image newImage, MapWinGIS.Grid newGrid, MapWinGIS.GridColorScheme GrdColorScheme, MapWinGIS.ICallback cb)
        {
        }
        #endregion


    }
}
