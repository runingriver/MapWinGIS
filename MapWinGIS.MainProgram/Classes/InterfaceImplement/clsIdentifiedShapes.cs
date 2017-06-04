/*******************************************************************************
 * 文件名:clsIdentifiedShapes.cs （F）
 * 描  述:提供插件获取图层中选择的shape的数量和索引，通过索引我们可以获得shape的属性
 * ******************************************************************************/

using System.Collections;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 将一个图层中所有标识的shape存储起来
    /// </summary>
    public class IdentifiedShapes : MapWinGIS.Interfaces.IdentifiedShapes
    {
        /// <summary>
        /// 存储shape在图层中的编号（索引）
        /// </summary>
        private ArrayList m_Shapes = new ArrayList();

        /// <summary>
        /// 被选中的shapes的数量
        /// </summary>
        public int Count 
        {
            get { return m_Shapes.Count; }
        }

        /// <summary>
        /// 根据存储在ArrayList索引返回shape的索引
        /// </summary>
        public int this[int index] 
        {
            get
            {
                if (index < 0 || index >= m_Shapes.Count)
                {
                    return -1;
                }
                return (int)m_Shapes[index];
            }
        }

        internal void Remove(int index)
        {
            if (index < 0 || index >= m_Shapes.Count)
            {
                return;
            }
            m_Shapes.RemoveAt(index);
        }

        internal void Add(int item)
        {
            m_Shapes.Add(item);
        }

    }
}
