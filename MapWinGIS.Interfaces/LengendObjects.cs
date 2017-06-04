using System.Drawing;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// 枚举，组的可见性
    /// </summary>
    public enum VisibleStateEnum
    {
        /// <summary>
        /// 所有的层均可见
        /// </summary>
        vsALL_VISIBLE = 0,

        /// <summary>
        /// 所有的层（Layer）均隐藏
        /// </summary>
        vsALL_HIDDEN = 1,

        /// <summary>
        /// 某些层可见，某些不可见
        /// </summary>
        vsPARTIAL_VISIBLE = 2,
    }

    /// <summary>
    /// 颜色信息，包括Start和End颜色，标题，是否透明
    /// </summary>
    internal struct ColorInfo
    {
        #region 公共成员变量

        public Color StartColor;
        public Color EndColor;
        public string Caption;
        public bool IsTransparent;

        #endregion

        public ColorInfo(Color start, Color end, string pCaption, bool transparent)
        {
            StartColor = start;
            EndColor = end;
            Caption = pCaption;
            IsTransparent = transparent;
        }

        public ColorInfo(Color start, Color end, string pCaption)
        {
            StartColor = start;
            EndColor = end;
            Caption = pCaption;
            IsTransparent = false;

        }
    }

    /// <summary>
    /// 组（group）和层（layer）的拖拽信息
    /// </summary>
    internal class DragInfo
    {
        #region 公共成员变量
        public bool Dragging;
        public bool MouseDown;
        public bool LegendLocked;
        public int DragLayerIndex;//拖拽的层
        public int DragGroupIndex;//拖拽的组
        public int TargetGroupIndex;//要拖拽的目标组位置
        public int TargetLayerIndex;//要拖拽的目标层位置
        public int StartY;
        #endregion

        public DragInfo()
        {
            Reset();
        }
        public void Reset()
        {
            Dragging = false;
            MouseDown = false;
            StartY = 0;
            LegendLocked = false;
            DragLayerIndex = -1;
            DragGroupIndex = -1;
            TargetGroupIndex = -1;
            TargetLayerIndex = -1;
        }

        public bool DraggingLayer
        {
            get
            {
                if (DragLayerIndex != -1)
                    return true;
                else
                    return false;
            }
        }
        public void StartGroupDrag(int MouseY, int GroupIndex)
        {
            MouseDown = true;
            DragGroupIndex = GroupIndex;
            DragLayerIndex = Constants.INVALID_INDEX;
            StartY = MouseY;
        }

        public void StartLayerDrag(int MouseY, int GroupIndex, int LayerIndex)
        {
            MouseDown = true;
            DragGroupIndex = GroupIndex;
            DragLayerIndex = LayerIndex;
            StartY = MouseY;

        }


    }

}
