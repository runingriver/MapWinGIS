/****************************************************************************
 * 文件名:clsIdentifiedLayers.cs （F）
 * 描  述:提供插件获取被选择的图层相关信息，通过图层句柄可以获得该图层上选择的shape
 *        信息。还可以获取选择的图层的数量。
 * **************************************************************************/

using System.Collections.Generic;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 存储所有图层中被标识的shape对象(IdentifiedShapes)
    /// </summary>
    public class IdentifiedLayers : MapWinGIS.Interfaces.IdentifiedLayers
    {
        /// <summary>
        /// key-图层handle， value-标识的shape(MainProgram.IdentifiedShapes)
        /// </summary>
        Dictionary<int, MainProgram.IdentifiedShapes> m_Layers = new Dictionary<int, IdentifiedShapes>();

        /// <summary>
        /// 返回从Identify方法调用的并存在相关信息的图层的数量
        /// </summary>
        public int Count
        {
            get { return m_Layers.Count; }
        }

        /// <summary>
        /// 返回一个IdentifiedShapes对象
        /// </summary>
        public Interfaces.IdentifiedShapes this[int LayerHandle]
        {
            get
            {
                if (m_Layers.ContainsKey(LayerHandle))
                {
                    return (Interfaces.IdentifiedShapes)m_Layers[LayerHandle];
                }
                return null;
            }
        }

        internal void Add(MainProgram.IdentifiedShapes item, int hLyr)
        {
            if (item == null)
            {
                return;
            }
            if (m_Layers.ContainsKey(hLyr))  //已经添加过该shape，更新存储在该shape中的信息。
            {
                MainProgram.IdentifiedShapes t;
                t = m_Layers[hLyr];
                int i;
                for (i = 0; i < item.Count; i++)
                {
                    t.Add(item[i]);
                }
            }
            else //未添加，添加
            {
                m_Layers.Add(hLyr, item);
            }
        }

        internal void Remove(int layerHandle)
        {
            if (m_Layers.ContainsKey(layerHandle))
            {
                m_Layers.Remove(layerHandle);
            }
        }
    }
}
