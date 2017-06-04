using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// legend中的一个组对象
    /// </summary>
    public class Group :MapWinGIS.Interfaces.Group
    {
        #region 成员变量

        private string m_Caption;

        /// <summary>
        /// 该字符串使开发者可以用来获取该组的信息
        /// </summary>
        public string Tag;

        private object m_Icon;
        private bool m_Expanded;
        private int m_Height;
        private Legend m_Legend;

        //public Legend Legend 
        //{
        //    get { return this.m_Legend; }
        //}

        /// <summary>
        /// 该组的句柄
        /// </summary>
        protected internal int m_Handle;

        /// <summary>
        /// 该组的顶点位置
        /// </summary>
        protected internal int Top;

        /// <summary>
        /// 包含在该组中所有图层的列表
        /// </summary>
        protected internal ArrayList m_Layers;

        private VisibleStateEnum m_VisibleState;

        protected internal bool m_StateLocked;
        #endregion

        public Group(Legend leg)
        {
            m_Legend = leg;
            m_Layers = new ArrayList();
            Expanded = true;
            VisibleState = VisibleStateEnum.vsALL_VISIBLE;
            m_Handle = -1;
            Icon = null;
            m_StateLocked = false;
        }

        /// <summary>
        /// 回收资源，回收legend、arraylist、object等资源
        /// </summary>
        ~Group()
        {
            m_Legend = null;
            m_Layers.Clear();
            m_Layers = null;
            m_Icon = null;
        }

        /// <summary>
        /// 获取设置显示在legend上组的文本
        /// Gets or sets the Text that appears in the legend for this group
        /// </summary>
        public string Text
        {
            get
            {
                return m_Caption;
            }
            set
            {
                m_Caption = value;
                m_Legend.Redraw();
            }
        }

        /// <summary>
        /// 获取和设置显示在该组下面的图标
        /// 设置该值null，则移除该图标
        /// </summary>
        public object Icon
        {
            get
            {
                return m_Icon;
            }
            set
            {
                if (Globals.IsSupportedPicture(value))
                {
                    m_Icon = value;
                    m_Legend.Redraw();
                }
                else
                {
                    throw new System.Exception("Legend 错误: 无效的组图标类型");
                }
            }
        }

        /// <summary>
        /// 获取在该组中层的数量
        /// Gets the number of layers within this group
        /// </summary>
        public int LayerCount
        {
            get
            {
                return m_Layers.Count;
            }
        }

        /// <summary>
        /// 索引，根据层的位置得到层（Layer）对象
        /// </summary>
        public LegendControl.Layer this[int LayerPosition]
        {
            get
            {
                if (LayerPosition >= 0 && LayerPosition < this.m_Layers.Count)
                    return (Layer)m_Layers[LayerPosition];

                Globals.LastError = "该组中的层位置无效";
                return null;
            }
        }

        /// <summary>
        /// 获取该组的句柄（handle），该标志是唯一的
        /// </summary>
        public int Handle
        {
            get
            {
                return m_Handle;
            }
        }

        /// <summary>
        /// 通过句柄在该组中查找一个层对象
        /// </summary>
        /// <param name="Handle">要查找的层的句柄</param>
        protected internal LegendControl.Layer LayerByHandle(int Handle)
        {
            int count = m_Layers.Count;
            Layer lyr = null;
            for (int i = 0; i < count; i++)
            {
                lyr = (Layer)m_Layers[i];
                if (lyr.Handle == Handle)
                    return lyr;
            }
            return null;
        }

        /// <summary>
        /// 获取在该组中层的位置（index）
        /// </summary>
        protected internal int LayerPositionInGroup(int Handle)
        {
            int count = m_Layers.Count;
            Layer lyr = null;
            for (int i = 0; i < count; i++)
            {
                lyr = (Layer)m_Layers[i];
                if (lyr.Handle == Handle)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 根据该层在该组中的位置获取指定层的句柄
        /// </summary>
        public int LayerHandle(int PositionInGroup)
        {
            if (PositionInGroup >= 0 && PositionInGroup < m_Layers.Count)
                return ((Layer)m_Layers[PositionInGroup]).Handle;

            Globals.LastError = "该组中的层位置无效";
            return -1;
        }

        /// <summary>
        /// 获取和设置该组是否是展开的
        /// 这将显示或隐藏在该组中的层
        /// </summary>
        public bool Expanded
        {
            get
            {
                return m_Expanded;
            }
            set
            {
                if (value != m_Expanded)
                {
                    m_Expanded = value;
                    RecalcHeight();
                    m_Legend.Redraw();
                }
            }
        }

        /// <summary>
        /// 获取绘制该组的高度
        /// </summary>
        protected internal int Height
        {
            get
            {
                RecalcHeight();
                return m_Height;
            }
        }

        /// <summary>
        /// 获取该组展开状态的高度
        /// </summary>
        protected internal int ExpandedHeight
        {
            get
            {
                int NumLayers = m_Layers.Count;
                //初始化高度为该组中每个条目绝对高度
                int Retval = Constants.ITEM_HEIGHT;
                Layer lyr;

                //根据层的数量利用绝对高度获取总高度
                for (int i = 0; i < NumLayers; i++)
                {
                    lyr = (Layer)m_Layers[i];
                    Retval += lyr.CalcHeight(true);
                }

                return Retval;
            }
        }

        /// <summary>
        /// 重新计算该组的高度
        /// </summary>
        protected internal void RecalcHeight()
        {
            int NumLayers = m_Layers.Count;
            //初始化高度为该组中每个条目绝对高度
            m_Height = Constants.ITEM_HEIGHT;
            Layer lyr;

            if (m_Expanded == true)
            {
                //根据层的数量利用绝对高度获取总高度
                for (int i = 0; i < NumLayers; i++)
                {
                    lyr = (Layer)m_Layers[i];
                    if (!lyr.HideFromLegend)
                        m_Height += lyr.Height;
                }
            }
            else
            {
                m_Height = Constants.ITEM_HEIGHT;
            }
        }

        /// <summary>
        /// 获取和设置该组中所有层的可见性
        /// </summary>
        public bool LayersVisible
        {
            get
            {
                if (VisibleState == VisibleStateEnum.vsALL_HIDDEN)
                    return false;
                else
                    return true;
            }
            set
            {
                if (value == true)
                    VisibleState = VisibleStateEnum.vsALL_VISIBLE;
                else
                    VisibleState = VisibleStateEnum.vsALL_HIDDEN;
            }
        }

        /// <summary>
        /// 获取和设置该组的可见性状态
        /// 不能设置某些可见，某些不可见
        /// </summary>
        protected internal VisibleStateEnum VisibleState
        {
            get
            {
                return m_VisibleState;
            }
            set
            {
                if (value == VisibleStateEnum.vsPARTIAL_VISIBLE)
                {
                    throw new System.Exception("无效的属性设置: vsPARTIAL_VISIBLE");
                }

                m_VisibleState = value;
                UpdateLayerVisibility();
            }
        }

        /// <summary>
        /// 获取和设置锁定状态的属性，这将阻止用户改变该可见性状态
        /// </summary>
        public bool StateLocked
        {
            get
            {
                return m_StateLocked;
            }
            set
            {
                m_StateLocked = value;
            }
        }

        /// <summary>
        /// 更新层的可见性
        /// </summary>
        private void UpdateLayerVisibility()
        {
            int NumLayers = m_Layers.Count;
            Layer lyr = null;
            bool visible = false;
            if (m_VisibleState == VisibleStateEnum.vsALL_VISIBLE)
                visible = true;

            for (int i = 0; i < NumLayers; i++)
            {
                lyr = (Layer)m_Layers[i];
                bool oldState = m_Legend.m_Map.get_LayerVisible(lyr.Handle);

                m_Legend.m_Map.set_LayerVisible(lyr.Handle, visible);

                if (oldState != visible)
                {
                    bool cancel = false;
                    m_Legend.FireLayerVisibleChanged(lyr.Handle, visible, ref cancel);
                    if (cancel == true)
                        lyr.Visible = !(visible);
                }
            }
        }

        /// <summary>
        /// 根据在该组中的每个层的可见性，跟心该组的可见性状态
        /// </summary>
        protected internal void UpdateGroupVisibility()
        {
            int NumVisible = 0;
            int NumLayers = m_Layers.Count;
            Layer lyr = null;
            for (int i = 0; i < NumLayers; i++)
            {
                lyr = (Layer)m_Layers[i];
                if (m_Legend.m_Map.get_LayerVisible(lyr.Handle) == true)
                    NumVisible++;
            }

            if (NumVisible == NumLayers)//所有层可见
                m_VisibleState = VisibleStateEnum.vsALL_VISIBLE;
            else if (NumVisible == 0)
                m_VisibleState = VisibleStateEnum.vsALL_HIDDEN;
            else
                m_VisibleState = VisibleStateEnum.vsPARTIAL_VISIBLE;
        }

        /// <summary>
        /// 返回该组的闪照图像
        /// </summary>
        /// <param name="imgWidth">返回的图像的宽度（pix），高度根据该组中层的个数</param>
        /// <returns>该组及其子层（若展开了）一个Bitmap对象</returns>
        public System.Drawing.Bitmap Snapshot(int imgWidth)
        {

            Bitmap bmp = null;
            Rectangle rect;

            System.Drawing.Graphics g;

            bmp = new Bitmap(imgWidth, this.ExpandedHeight);
            g = Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.White);

            rect = new Rectangle(0, 0, imgWidth, this.ExpandedHeight);

            m_Legend.DrawGroup(g, this, rect, true);

            return bmp;
        }
    }
}
