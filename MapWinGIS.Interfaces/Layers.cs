using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// 层的管理类，包含操作Legend中的层基本方法
    /// </summary>
    public class Layers
    {
        private readonly Legend legend;

        #region 构造函数
        public Layers(Legend leg)
        {
            this.legend = leg;
        }

        #endregion

        #region 公共属性

        /// <summary>
        ///  获取所有层的数量
        /// </summary>
        public int Count
        {
            get
            {
                if (this.legend == null)
                {
                    return 0;
                }

                return this.legend.m_Map == null ? 0 : this.legend.m_Map.NumLayers;
            }
        }

        #endregion

        #region 索引器

        /// <summary>
        ///  索引器，通过层的列表迭代
        /// </summary>
        /// <param name = "position">索引从0开始</param>
        /// <returns>The layer</returns>
        public LegendControl.Layer this[int position]
        {
            get
            {
                if (position >= 0 && position < this.Count)
                {
                    var handle = this.legend.m_Map.get_LayerHandle(position);
                    return this.legend.FindLayerByHandle(handle);
                }

                Globals.LastError = "层位置无效";
                return null;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 在组的顶部添加一个层
        /// </summary>
        public int Add(object newLayer, bool visible)
        {
            return this.legend.AddLayer(newLayer, visible);
        }

        /// <summary>
        /// 往地图上添加一个图层,位置在当前选择的层的上方或者在层列表的顶部
        /// </summary>
        public int Add(object newLayer, bool visible, bool placeAboveCurrentlySelected)
        {
            return this.legend.AddLayer(newLayer, visible, placeAboveCurrentlySelected);
        }

        /// <summary>
        /// 在组的顶部添加一个图层
        /// </summary>
        /// <returns>返回图层的句柄，-1添加失败</returns>
        public int Add(object newLayer, bool visible, int targetGroupHandle)
        {
            return this.legend.AddLayer(newLayer, visible, targetGroupHandle);
        }

        /// <summary>
        /// 在组的顶部添加一个图层
        /// </summary>
        /// <param name="legendVisible">层在lengend上是否可见</param>
        /// <param name="newLayer">符合层类型的对象</param>
        /// <param name="mapVisible">所添加的层是否在地图上显示</param>
        public int Add(bool legendVisible, object newLayer, bool mapVisible)
        {
            return this.legend.AddLayer(newLayer, mapVisible, -1, legendVisible);
        }

        /// <summary>
        /// 清空所有的层
        /// </summary>
        public void Clear()
        {
            this.legend.ClearLayers();
        }

        /// <summary>
        /// 销毁所有的层
        /// </summary>
        public void CollapseAll()
        {
            this.legend.Lock();
            int i;

            var count = this.Count;
            for (i = 0; i < count; i++)
            {
                this[i].Expanded = false;
            }

            this.legend.Unlock();
        }

        /// <summary>
        /// 展开所有的层
        /// </summary>
        public void ExpandAll()
        {
            this.legend.Lock();
            int i;

            var count = this.Count;
            for (i = 0; i < count; i++)
            {
                this[i].Expanded = true;//会导致legend重绘
            }

            this.legend.Unlock();
        }

        /// <summary>
        /// 获取包含在组中指定层的句柄
        /// </summary>
        public int GroupOf(int layerHandle)
        {
            int layerIndex, groupIndex;
            var lyr = this.legend.FindLayerByHandle(layerHandle, out groupIndex, out layerIndex);

            if (lyr != null)
            {
                var grp = (Group)this.legend.m_AllGroups[groupIndex];
                return grp.Handle;
            }

            Globals.LastError = "无效层句柄";
            return -1;
        }

        /// <summary>
        /// 检查指定的组是否存在于组的列表中
        /// </summary>
        /// <param name="handle">组的句柄</param>
        public bool IsValidHandle(int handle)
        {
            return this.legend.m_Map.get_LayerPosition(handle) >= 0;
        }

        /// <summary>
        /// 通过句柄获取一个层，不用知道层是属于那一个组
        /// Get a Layer by the handle to the layer (without knowing what group the layer is in)
        /// </summary>
        public Layer ItemByHandle(int handle)
        {
            return this.legend.FindLayerByHandle(handle);
        }

        /// <summary>
        /// 将一个层移动到指定组的指定位置
        /// </summary>
        public bool MoveLayer(int layerHandle, int targetGroupHandle, int positionInGroup)
        {
            return this.legend.MoveLayer(targetGroupHandle, layerHandle, positionInGroup);
        }

        /// <summary>
        /// 在该组中将一个层移动到一个新的位置
        /// </summary>
        public bool MoveLayerWithinGroup(int lyrHandle, int newPosition)
        {
            int groupIndex, layerIndex;

            if (this.legend.FindLayerByHandle(lyrHandle, out groupIndex, out layerIndex) != null)
            {
                var grp = (Group)this.legend.m_AllGroups[groupIndex];
                return this.legend.MoveLayer(grp.Handle, lyrHandle, newPosition);
            }

            Globals.LastError = "无效层句柄";
            return false;
        }

        /// <summary>
        /// 在组内获取指定层的位置（index）
        /// </summary>
        public int PositionInGroup(int layerHandle)
        {
            int layerIndex, groupIndex;
            var lyr = this.legend.FindLayerByHandle(layerHandle, out groupIndex, out layerIndex);

            if (lyr != null)
            {
                return layerIndex;
            }

            Globals.LastError = "无效层句柄";
            return -1;
        }

        /// <summary>
        /// 移除一个层
        /// </summary>
        public bool Remove(int layerHandle)
        {
            return this.legend.RemoveLayer(layerHandle);
        }

        #endregion
    }
}
