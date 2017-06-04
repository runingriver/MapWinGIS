/****************************************************************************
 * 文件名:clsShapes.cs (F)
 * 描  述:提供获取图层上shape的信息。包括可以迭代图层上的每一个shape，获取某个图层
 *        上的shape数量，通过shapeindex或foreach获取一个shape对象。
 * **************************************************************************/

using System.Collections;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 提供获取一个图层上的shape对象
    /// </summary>
    public class Shapes : MapWinGIS.Interfaces.Shapes, IEnumerable
    {
        //-----------------实现枚举--------------------------
        internal class ShapeEnumerator : IEnumerator
        {
            Interfaces.Shapes m_Collection;
            int m_Index = -1;

            public ShapeEnumerator(Interfaces.Shapes shp)
            {
                m_Collection = shp;
                m_Index = -1;
            }
            public object Current
            {
                get { return m_Collection[m_Index]; }
            }
            public bool MoveNext()
            {
                m_Index++;
                if (m_Index > m_Collection.NumShapes)
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
            return new ShapeEnumerator(this);
        }

        //-----------------变量定义，构造函数-----------------
        private int m_LayerHandle;

        public Shapes()
        {
            m_LayerHandle = -1;
        }

        //--------------接口实现-----------------------------
        /// <summary>
        /// 在shapefile中的shape的数量
        /// </summary>
        public int NumShapes 
        {
            get
            {
                MapWinGIS.Shapefile sf;
                if (m_LayerHandle != -1)
                {
                    sf = (MapWinGIS.Shapefile)Program.frmMain.MapMain.get_GetObject(m_LayerHandle);
                    if (sf != null)
                    {
                        return sf.NumShapes;
                    }
                    else
                    {
                        Program.g_error = "NumShapes: 无法从地图获取shapefile对象.";
                        return -1;
                    }
                }
                else
                {
                    Program.g_error = "NumShapes: 在objec中设置了无效的handle";
                    return -1;
                }
            }
        }

        /// <summary>
        /// 索引，返回一个shape，通过在shapefile中的索引
        /// </summary>
        public Interfaces.Shape this[int Index] 
        {
            get
            {
                if (Index >= 0 && Index < NumShapes)
                {
                    return (Interfaces.Shape)(new Shape(m_LayerHandle, Index));
                }
                else
                {
                    Program.g_error = "Shapes: 无效的索引.";
                    return null;
                }
            }
        }

        //-------------获取或设置图层句柄----------------------
        internal int LayerHandle
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
    }
}
