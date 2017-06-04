/****************************************************************************
 * 文件名:clsSelectInfo.cs (F)
 * 描  述:提供操作选择的shape的方法。MainProgram.SelectedShape是操作对象， 
 *        一个集合对应一个层,一个层对应多个shape。
 * **************************************************************************/

using System;
using System.Collections;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 一个存储SelectedShpe的集合，并提供选择的shape的操作
    /// </summary>
    public class SelectInfo : MapWinGIS.Interfaces.SelectInfo,IEnumerable
    {

        internal class SelectedShapeEnumerator : IEnumerator
        {
            private MainProgram.SelectInfo m_Collection;
            private int m_Index = -1;

            public SelectedShapeEnumerator(MainProgram.SelectInfo inp)
            {
                m_Collection = inp;
                m_Index = -1;
            }

            public object Current
            {
                get { return m_Collection[m_Index]; }
            }
            public bool MoveNext()
            {
                m_Index++;
                if (m_Index >= m_Collection.NumSelected)
                {
                    return false;
                }
                return true;
            }
            public void Reset()
            {
                m_Index = -1;
            }

        }

        public IEnumerator GetEnumerator()
        {
            return new SelectedShapeEnumerator(this);
        }

        private int m_LayerHandle;
        private int m_NumSelected;
        private MapWinGIS.Extents m_SelectBounds;
        private MainProgram.SelectedShape[] m_Shapes;

        public SelectInfo(int layerHandle)
        {
            m_SelectBounds = new MapWinGIS.Extents();
            m_LayerHandle = layerHandle;
        }

        //-------------------------------接口实现-------------------------------
        
        /// <summary>
        /// 添加一个已选择的shape到指定的存储选择shape的集合中
        /// </summary>
        /// <param name="newShape">The <c>SelectedShape</c> object to add.</param>
        public void AddSelectedShape(Interfaces.SelectedShape newShape)
        {
            if (m_LayerHandle == -1)
            {
                m_LayerHandle = Program.frmMain.Legend.SelectedLayer;
            }

            if (newShape == null)
            {
                Program.g_error = "AddSelectedShape: 参数变量没有设置";

            }
            else
            {
                Array.Resize(ref m_Shapes, m_NumSelected + 1);
                m_Shapes[m_NumSelected] = (MainProgram.SelectedShape)newShape;
                m_NumSelected++;
            }
        }

        /// <summary>
        /// 根据提供的shape的索引，添加到存储以选择shape的集合中
        /// </summary>
        /// <param name="ShapeIndex">要添加的shape的索引.</param>
        /// <param name="SelectColor">不用的参数.</param>
        public void AddByIndex(int ShapeIndex, System.Drawing.Color SelectColor)
        {
            MainProgram.SelectedShape newShp = new SelectedShape();
            if (Program.frmMain.MapMain.ShapeDrawingMethod != tkShapeDrawingMethod.dmNewSymbology)
            {
                if (Program.frmMain.MapMain.get_ShapeVisible(Program.frmMain.Legend.SelectedLayer, ShapeIndex) != false)
                {
                    newShp.Add(ShapeIndex, SelectColor);
                    AddSelectedShape(newShp);
                }
            }
            else
            {
                newShp.Add(ShapeIndex, SelectColor);
                AddSelectedShape(newShp);
            }
            newShp = null;
        }

        /// <summary>
        /// 清空所有已选择的shape的列表
        /// </summary>
        public void ClearSelectedShapes()
        {
            if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                bool isLocked = false;
                try
                {
                    MainProgram.SelectedShape oneShp;
                    int i;
                    int tLyrHandle;

                    tLyrHandle = m_LayerHandle;

                    if (Program.frmMain.MapMain == null)
                    {
                        for (i = m_NumSelected - 1; i >= 0; i--)
                        {
                            m_Shapes[i] = null;
                        }
                        m_Shapes = null;
                        m_SelectBounds = null;
                        return;
                    }

                    if (Program.frmMain.MapMain.IsLocked == MapWinGIS.tkLockMode.lmUnlock)
                    {
                        Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                        isLocked = true;
                    }

                    for (i = m_NumSelected - 1; i >= 0; i--)
                    {
                        oneShp = m_Shapes[i];
                        if (oneShp != null)
                        {
                            Program.frmMain.MapMain.set_ShapePointColor(tLyrHandle, oneShp.ShapeIndex, oneShp.OriginalColor);
                            Program.frmMain.MapMain.set_ShapeLineColor(tLyrHandle, oneShp.ShapeIndex, oneShp.OriginalOutlineColor);
                            Program.frmMain.MapMain.set_ShapeFillColor(tLyrHandle, oneShp.ShapeIndex, oneShp.OriginalColor);
                            Program.frmMain.MapMain.set_ShapeDrawFill(tLyrHandle, oneShp.ShapeIndex, oneShp.OriginalDrawFill);

                            if (Program.projInfo.TransparentSelection)
                            {
                                Program.frmMain.MapMain.set_ShapeFillTransparency(tLyrHandle, oneShp.ShapeIndex, oneShp.OriginalTransparency);
                            }
                        }

                        m_NumSelected--;
                        oneShp = null;

                        m_Shapes[i] = null;
                    }

                    m_Shapes = null;
                    m_SelectBounds = null;
                    Program.frmMain.UpdateButtons();
                    m_LayerHandle = -1;

                }
                catch (Exception ex)
                {
                    Program.g_error = ex.Message;
                    Program.ShowError(ex);
                }
                if (isLocked)
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
            }
            else //不是dmNewSymbology绘制方式
            {
                int handle;
                for (int i = 0; i < Program.frmMain.Layers.NumLayers; i++)
                {
                    handle =Program.frmMain.MapMain.get_LayerHandle(i);
                    MapWinGIS.Shapefile sf;
                    Interfaces.Layer layer = Program.frmMain.Layers[handle];
                    if (layer != null)
                    {
                        if (layer.LayerType == Interfaces.eLayerType.LineShapefile || layer.LayerType == Interfaces.eLayerType.PointShapefile || layer.LayerType == Interfaces.eLayerType.PolygonShapefile)
                        {
                            sf = (MapWinGIS.Shapefile)layer.GetObject();
                            if (sf != null)
                            {
                                sf.SelectNone();
                            }
                        }
                    }
                }

                m_Shapes = null;
                m_LayerHandle = -1;
                m_NumSelected = 0;
                Program.frmMain.UpdateButtons();
                Program.frmMain.MapMain.Redraw();
            }
        }

        /// <summary>
        /// 从集合中移除指定索引的shape，通过shape存储在集合中的索引删除
        /// </summary>
        /// <param name="ListIndex">要移除的shape的集中对应的索引</param>
        public void RemoveSelectedShape(int ListIndex)
        {
            int i;
            MainProgram.SelectedShape tShp ;
            int mapIndex ;
            int mapHandle;

            if (Program.frmMain.MapMain == null)
            {
                return;
            }

            mapHandle = m_LayerHandle;

            if (ListIndex >= 0 && ListIndex < m_NumSelected)
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);

                try
                {
                    tShp = m_Shapes[ListIndex];
                    mapIndex = tShp.ShapeIndex;

                    if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                    {
                        Program.frmMain.MapMain.set_ShapePointColor(mapHandle, mapIndex, tShp.OriginalColor);
                        Program.frmMain.MapMain.set_ShapeLineColor(mapHandle, mapIndex, tShp.OriginalOutlineColor);
                        Program.frmMain.MapMain.set_ShapeFillColor(mapHandle, mapIndex, tShp.OriginalColor);
                        Program.frmMain.MapMain.set_ShapeDrawFill(mapHandle, mapIndex, tShp.OriginalDrawFill);

                        if (Program.projInfo.TransparentSelection)
                        {
                            Program.frmMain.MapMain.set_ShapeFillTransparency(mapHandle, mapIndex, tShp.OriginalTransparency);
                        }
                    }
                    else
                    {
                        MainProgram.Layer layer = (MainProgram.Layer)Program.frmMain.Layers[m_LayerHandle];
                        if (layer != null)
                        {
                            MapWinGIS.Shapefile sf;
                            sf = (MapWinGIS.Shapefile)(layer.GetObject());
                            if (sf != null)
                            {
                                tShp = m_Shapes[ListIndex];
                                mapIndex = tShp.ShapeIndex;
                                sf.ShapeSelected[mapIndex] = false;
                            }
                        }
                    }
                    tShp = null;

                    for (i = ListIndex; i < m_NumSelected - 1; i++) //移除并整理数组
                    {
                        m_Shapes[i] = m_Shapes[i + 1];
                    }

                    m_Shapes[m_NumSelected - 1] = null;
                    m_NumSelected--;

                    if (m_NumSelected == 0)
                    {
                        m_Shapes = null;
                    }
                    else
                    {
                        Array.Resize(ref m_Shapes, m_NumSelected);
                    }
                }
                finally
                {
                    Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }

            }
            else
            {
                Program.g_error = "RemoveSelectedShape:  无效的索引"; 
            }
        }

        /// <summary>
        /// 从集合中移除指定的shape，通过shape在地图中的实际索引移除
        /// </summary>
        /// <param name="ShapeIndex">shape的索引</param>
        public void RemoveByShapeIndex(int ShapeIndex)
        {
            int i;
            int j;
            MainProgram.SelectedShape tShp;

            if (Program.frmMain.MapMain == null)
            {
                return;
            }

            for (i = 0; i < m_Shapes.Length; i++)
            {
                if (m_Shapes[i].ShapeIndex == ShapeIndex) //找到该shapIndex的对象
                {
                    tShp = m_Shapes[i];

                    if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                    {
                        Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
                        try
                        {
                            Program.frmMain.MapMain.set_ShapePointColor(m_LayerHandle, ShapeIndex, tShp.OriginalColor);
                            Program.frmMain.MapMain.set_ShapeLineColor(m_LayerHandle, ShapeIndex, tShp.OriginalOutlineColor);
                            Program.frmMain.MapMain.set_ShapeFillColor(m_LayerHandle, ShapeIndex, tShp.OriginalColor);
                            Program.frmMain.MapMain.set_ShapeDrawFill(m_LayerHandle, ShapeIndex, tShp.OriginalDrawFill);

                            if (Program.projInfo.TransparentSelection)
                            {
                                Program.frmMain.MapMain.set_ShapeFillTransparency(m_LayerHandle, ShapeIndex, tShp.OriginalTransparency);
                            }
                        }
                        finally
                        {
                            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                        }
                    }
                    else
                    {
                        // 新版中ocx会自己选择合适的颜色
                        MainProgram.Layer layer = (MainProgram.Layer)Program.frmMain.Layers[m_LayerHandle];
                        if (layer != null)
                        {
                            MapWinGIS.Shapefile sf;
                            sf = (MapWinGIS.Shapefile)(layer.GetObject());
                            if (sf != null)
                            {
                                sf.ShapeSelected[ShapeIndex] = false;
                            }
                        }
                    }

                    for (j = i; j <= m_NumSelected - 2; j++)
                    {
                        m_Shapes[j] = m_Shapes[j + 1];
                    }

                    m_Shapes[m_NumSelected - 1] = null;
                    m_NumSelected--;

                    if (m_NumSelected == 0)
                    {
                        m_Shapes = null;
                    }
                    else
                    {
                        Array.Resize(ref m_Shapes, m_NumSelected);
                    }

                    tShp = null;
                    return; //所有的操作完成，直接返回
                }
            }
        }

        /// <summary>
        /// 返回选择的层的layerHandle
        /// </summary>
        public int LayerHandle 
        {
            get { return m_LayerHandle; }
        }

        /// <summary>
        /// 当前选择的shape的数量
        /// </summary>
        public int NumSelected 
        {
            get { return m_NumSelected; }
        }

        /// <summary>
        /// 返回所有选择shape的全图.
        /// 即以最大范围的方式显示出所有选择的shape，并返回该范围
        /// </summary>
        public MapWinGIS.Extents SelectBounds 
        {
            get 
            {
                //计算当前选择的shape的总范围
                MapWinGIS.Extents newBounds = new MapWinGIS.Extents();
                int k;
                double curmaxX, curminX, curmaxY, curminY, curmaxZ, curminZ;
                MapWinGIS.Extents shpExts;

                if (m_NumSelected > 0)
                {
                    shpExts = m_Shapes[0].Extents;
                    curmaxX = shpExts.xMax;
                    curminX = shpExts.xMin;
                    curmaxY = shpExts.yMax;
                    curminY = shpExts.yMin;
                    curmaxZ = shpExts.zMax;
                    curminZ = shpExts.zMin;

                }
                else
                {
                    return null;
                }

                for (k = 1; k < m_NumSelected; k++)
                {
                    shpExts = null;
                    shpExts = m_Shapes[k].Extents;
                    if (shpExts.xMax > curmaxX)
                    {
                        curmaxX = shpExts.xMax;
                    }
                    if (shpExts.xMin < curminX)
                    {
                        curminX = shpExts.xMin;
                    }
                    if (shpExts.yMax > curmaxY)
                    {
                        curmaxY = shpExts.yMax;
                    }
                    if (shpExts.yMin < curminY)
                    {
                        curminY = shpExts.yMin;
                    }
                    if (shpExts.zMax > curmaxZ)
                    {
                        curmaxZ = shpExts.zMax;
                    }
                    if (shpExts.zMin < curminZ)
                    {
                        curminZ = shpExts.zMin;
                    }
                }

                newBounds.SetBounds(curminX, curminY, curminZ, curmaxX, curmaxY, curmaxZ);
                return newBounds;
            }
        }

        /// <summary>
        /// 索引，获得SelectedShape
        /// </summary>
        public Interfaces.SelectedShape this[int Index] 
        {
            get
            {
                if (m_NumSelected == 0)
                {
                    return null;

                }
                else if (Index >= 0 & Index < m_NumSelected)
                {
                    return m_Shapes[Index];

                }
                else
                {
                    Program.g_error = "SelectedShape:  无效的索引";
                    return null;
                }
            }
        }
        //---------------------------------------------------------------
        
        /// <summary>
        /// 清空所有选择的shape
        /// </summary>
        public void ClearSelectedShapesTemp()
        {
            m_Shapes = null;
            m_NumSelected = 0;
        }
    }
}
