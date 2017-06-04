using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapWinGIS.Interfaces;


namespace MapWinGIS.LegendControl
{
    #region 定义事件的委托
    /// <summary>
    /// 双击（DoubleClick）层对象
    /// </summary>
    public delegate void TileLayerDoubleClick(int X, int Y);

    /// <summary>
    /// 双击（DoubleClick）层对象
    /// </summary>
    public delegate void LayerDoubleClick(int Handle);

    /// <summary>
    /// 鼠标按钮在某个层上点击（MouseDown）
    /// </summary>
    public delegate void LayerMouseDown(int Handle, MouseButtons button);

    /// <summary>
    /// 鼠标按钮在某个层上松开（MouseUp）
    /// </summary>
    public delegate void LayerMouseUp(int Handle, MouseButtons button);

    /// <summary>
    /// 新的层被选择
    /// </summary>
    public delegate void LayerSelected(int Handle);

    /// <summary>
    /// 新的层被添加
    /// </summary>
    public delegate void LayerAdded(int Handle);

    /// <summary>
    /// 层被移除
    /// </summary>
    public delegate void LayerRemoved(int Handle);

    /// <summary>
    /// 组（Group）被双击
    /// </summary>
    public delegate void GroupDoubleClick(int Handle);

    /// <summary>
    /// 在组（Group）上鼠标按钮按下（MouseDown）
    /// </summary>
    public delegate void GroupMouseDown(int Handle, MouseButtons button);

    /// <summary>
    /// 在组（Group）上鼠标按钮松开（MouseUp）
    /// </summary>
    public delegate void GroupMouseUp(int Handle, MouseButtons button);

    /// <summary>
    /// 鼠标点击在Legend上（既不在组标签上，也不在层标签上）
    /// </summary>
    public delegate void LegendClick(MouseButtons button, System.Drawing.Point Location);

    /// <summary>
    /// 层的位置发生改变
    /// </summary>
    public delegate void LayerPositionChanged(int Handle);

    /// <summary>
    /// 层的可见性发生改变
    /// </summary>
    public delegate void LayerVisibleChanged(int Handle, bool NewState, ref bool Cancel);

    /// <summary>
    /// 组（Group）及其包含的所有的层（Layer）位置发生改变
    /// </summary>
    public delegate void GroupPositionChanged(int Handle);

    /// <summary>
    /// 一个组被添加
    /// </summary>
    public delegate void GroupAdded(int Handle);

    /// <summary>
    /// 某个组被移除
    /// </summary>
    public delegate void GroupRemoved(int Handle);

    /// <summary>
    /// 组的展开属性发生改变
    /// </summary>
    public delegate void GroupExpandedChanged(int Handle, bool Expanded);

    /// <summary>
    /// 组的checkbox被点击
    /// </summary>
    public delegate void GroupCheckboxClicked(int Handle, VisibleStateEnum State);

    /// <summary>
    ///  自定义ExpansionBox颜色
    /// </summary>
    /// <param name="Handle">要被绘制的展开的层的句柄</param>
    /// <param name="Bounds">允许绘制的边界</param>
    /// <param name="G">graphics对象</param>
    /// <param name="Handled">如果要给该层的expansion box着色，设置true</param>
    public delegate void ExpansionBoxCustomRenderer(int Handle, System.Drawing.Rectangle Bounds, ref System.Drawing.Graphics G, ref bool Handled);

    /// <summary>
    /// 指示ExpansionBoxCustomRenderFunction设置的高度
    /// </summary>
    /// <param name="Handle">要着色的层的handle/</param>
    /// <param name="CurrentWidth">要着色的区域的宽度</param>
    /// <param name="HeightToDraw">根据宽度设置高度</param>
    /// <param name="Handled"></param>
    public delegate void ExpansionBoxCustomHeight(int Handle, int CurrentWidth, ref int HeightToDraw, ref bool Handled);

    /// <summary>
    /// 层的layerCheckbox被点击
    /// </summary>
    public delegate void LayerCheckboxClicked(int Handle, bool NewState);

    /// <summary>
    /// 层的LayerColorbox被点击
    /// </summary>
    public delegate void LayerColorboxClicked(int Handle);

    /// <summary>
    /// 其中一个shapefiles的某个Category被点击，colorbox
    /// </summary>
    /// <param name="Handle">层的句柄</param>
    /// <param name="Category">Category的索引</param>
    public delegate void LayerCategoryClicked(int Handle, int Category);

    /// <summary>
    /// shapefiles的chart Icon被点击
    /// </summary>
    public delegate void LayerChartClicked(int Handle);

    /// <summary>
    /// shapefiles中的某个chart field被点击
    /// </summary>
    public delegate void LayerChartFieldClicked(int Handle, int FieldIndex);

    /// <summary>
    /// LayerLabel被点击
    /// </summary>
    public delegate void LayerLabelClicked(int Handle);

    /// <summary>
    /// 层的指定标签被点击
    /// </summary>
    /// <param name="Handle">层的句柄</param>
    /// <param name="Category">标签目录的索引</param>
    public delegate void LayerLabelCategoryClicked(int Handle, int Category);

    /// <summary>
    /// 层的labels icon被点击
    /// </summary>
    public delegate void LayerLabelsEventArguments(int Handle);

    #endregion //28

    /// <summary>
    /// Legend整体概述
    /// </summary>
    public partial class Legend : System.Windows.Forms.UserControl
    {
        #region 成员变量

        private const int INVALID_GROUP = -1;

        private Groups m_GroupManager;
        private Layers m_LayerManager;
        /// <summary>
        /// 用于legend与MapWinGIS.Map交互
        /// </summary>
        protected internal MapWinGIS.Map m_Map;

        private System.Drawing.Color m_SelectedColor;
        private System.Drawing.Color m_BoxLineColor;

        private System.Drawing.Image m_BackBuffer;
        private System.Drawing.Image m_MidBuffer;
        private System.Drawing.Graphics m_FrontBuffer;
        private bool m_showGroupFolders;

        /// <summary>
        /// 所有组（gourps）列表
        /// </summary>
        protected internal ArrayList m_AllGroups = new ArrayList();

        /// <summary>
        /// 存储组的位置（index）集合
        /// </summary>
        protected internal ArrayList m_GroupPositions = new ArrayList();

        private System.Drawing.Graphics m_Draw = null;
        private DragInfo m_DragInfo = new DragInfo();
        private uint m_LockCount;
        private int m_SelectedLayerHandle;
        private int m_SelectedGroupHandle;
        private System.Drawing.Font m_Font;
        private System.Drawing.Font m_BoldFont;
        private bool m_painting = false;

        private const int cGridIcon = 0;
        private const int cImageIcon = 1;
        private const int cCheckedBoxIcon = 2;
        private const int cUnCheckedBoxIcon = 3;
        private const int cCheckedBoxGrayIcon = 4;
        private const int cUnCheckedBoxGrayIcon = 5;
        private const int cActiveLabelIcon = 6;
        private const int cDimmedLabelIcon = 7;

        private bool m_ShowLabels = false;
        private Group tileGroup = null;
        public bool DisplayTilesGroup { get; set; }

        #endregion

        #region 事件声明 
        //ExpansionBoxCustomRenderer、ExpansionBoxCustomHeight这两个委托没有声明事件

        /// <summary>
        /// 层的双击事件
        /// </summary>
        public event TileLayerDoubleClick TileLayerDoubleClick;

        /// <summary>
        /// 层的双击事件
        /// </summary>
        public event LayerDoubleClick LayerDoubleClick;

        /// <summary>
        /// 层上的鼠标点击事件
        /// </summary>
        public event LayerMouseDown LayerMouseDown;

        /// <summary>
        /// 层上的鼠标松开（mouse up）事件
        /// </summary>
        public event LayerMouseUp LayerMouseUp;

        /// <summary>
        /// 组上的鼠标的双击事件
        /// </summary>
        public event GroupDoubleClick GroupDoubleClick;

        /// <summary>
        /// 组上的鼠标按下事件
        /// </summary>
        public event GroupMouseDown GroupMouseDown;

        /// <summary>
        /// 组上的鼠标松开事件
        /// </summary>
        public event GroupMouseUp GroupMouseUp;

        /// <summary>
        /// legend点击事件
        /// </summary>
        public event LegendClick LegendClick;

        /// <summary>
        /// 选择的层改变事件
        /// </summary>
        public event LayerSelected LayerSelected;

        /// <summary>
        /// 新的层被添加事件
        /// </summary>
        public event LayerAdded LayerAdded;

        /// <summary>
        /// 层被移除事件
        /// </summary>
        public event LayerRemoved LayerRemoved;

        /// <summary>
        /// 层的位置改变事件
        /// </summary>
        public event LayerPositionChanged LayerPositionChanged;

        /// <summary>
        /// 组的位置改变事件
        /// </summary>
        public event GroupPositionChanged GroupPositionChanged;

        /// <summary>
        /// 组被添加事件
        /// </summary>
        public event GroupAdded GroupAdded;

        /// <summary>
        /// 组被移除事件
        /// </summary>
        public event GroupRemoved GroupRemoved;

        /// <summary>
        /// 层的可见性发生改变事件
        /// </summary>
        public event LayerVisibleChanged LayerVisibleChanged;

        /// <summary>
        /// 组的checkbox被点击事件
        /// </summary>
        public event GroupCheckboxClicked GroupCheckboxClicked;

        /// <summary>
        /// 组的Expanded属性发生改变事件
        /// </summary>
        public event GroupExpandedChanged GroupExpandedChanged;

        /// <summary>
        /// 层的checkbox被点击事件
        /// </summary>
        public event LayerCheckboxClicked LayerCheckboxClicked;

        /// <summary>
        /// 层的colorbox被点击事件
        /// </summary>
        public event LayerColorboxClicked LayerColorboxClicked;

        /// <summary>
        /// shapefile被点击事件
        /// </summary>
        public event LayerCategoryClicked LayerCategoryClicked;

        /// <summary>
        /// 层的chart icon被点击事件
        /// </summary>
        public event LayerChartClicked LayerChartClicked;

        /// <summary>
        /// 层的chart fields被点击事件
        /// </summary>
        public event LayerChartFieldClicked LayerChartFieldClicked;

        /// <summary>
        /// 层的label preview被点击事件
        /// </summary>
        public event LayerLabelClicked LayerLabelClicked;

        /// <summary>
        /// Fired when label preview for particular label category is clicked
        /// </summary>
        public event LayerLabelCategoryClicked LayerLabelCategoryClicked;

        /// <summary>
        /// 层的labels icon被点击事件
        /// </summary>
        public event LayerLabelsEventArguments LayerLabelsClicked;

        /***************************将事件发送给监听者*********************************/
        /// <summary>
        /// 触发层的可见性发生改变事件
        /// </summary>
        protected internal void FireLayerVisibleChanged(int handle, bool newState, ref bool cancel)
        {
            if (LayerVisibleChanged != null)
                LayerVisibleChanged(handle, newState, ref cancel);
        }
        /// <summary>
        /// 触发组的位置发生改变事件
        /// </summary>
        protected internal void FireGroupPositionChanged(int handle)
        {
            if (GroupPositionChanged != null)
                GroupPositionChanged(handle);
        }
        /// <summary>
        /// 触发组被添加事件
        /// </summary>
        protected internal void FireGroupAdded(int Handle)
        {
            if (GroupAdded != null)
                GroupAdded(Handle);
        }
        /// <summary>
        /// 触发组被移除事件
        /// </summary>
        protected internal void FireGroupRemoved(int Handle)
        {
            if (GroupRemoved != null)
                GroupRemoved(Handle);
        }
        /// <summary>
        /// 触发层的位置改变事件
        /// </summary>
        protected internal void FireLayerPositionChanged(int Handle)
        {
            if (LayerPositionChanged != null)
                LayerPositionChanged(Handle);
        }
        /// <summary>
        /// 触发选择的层发生改变事件
        /// </summary>
        protected internal void FireLayerSelected(int Handle)
        {
            if (LayerSelected != null)
                LayerSelected(Handle);
        }
        /// <summary>
        /// 触发新的层被添加事件
        /// </summary>
        protected internal void FireLayerAdded(int Handle)
        {
            if (LayerAdded != null)
                LayerAdded(Handle);
        }
        /// <summary>
        /// 触发层被移除事件
        /// </summary>
        protected internal void FireLayerRemoved(int Handle)
        {
            if (LayerRemoved != null)
                LayerRemoved(Handle);
        }
        /// <summary>
        /// 触发legend点击事件
        /// </summary>
        protected internal void FireLegendClick(MouseButtons button, System.Drawing.Point Location)
        {
            if (LegendClick != null)
                LegendClick(button, Location);
        }
        /// <summary>
        /// 触发在组上鼠标松开事件
        /// </summary>
        protected internal void FireGroupMouseUp(int Handle, MouseButtons button)
        {
            if (GroupMouseUp != null)
                GroupMouseUp(Handle, button);
        }
        /// <summary>
        /// 触发组上mousedown事件
        /// </summary>
        protected internal void FireGroupMouseDown(int Handle, MouseButtons button)
        {
            if (GroupMouseDown != null)
                GroupMouseDown(Handle, button);
        }
        /// <summary>
        /// 触发组上双击事件
        /// </summary>
        protected internal void FireGroupDoubleClick(int Handle)
        {
            if (GroupDoubleClick != null)
                GroupDoubleClick(Handle);
        }
        /// <summary>
        /// 触发层上mouseup事件
        /// </summary>
        protected internal void FireLayerMouseUp(int Handle, MouseButtons button)
        {
            if (LayerMouseUp != null)
                LayerMouseUp(Handle, button);
        }
        /// <summary>
        /// 触发层上鼠标点击事件
        /// </summary>
        protected internal void FireLayerMouseDown(int Handle, MouseButtons button)
        {
            if (LayerMouseDown != null)
                LayerMouseDown(Handle, button);
        }
        /// <summary>
        /// 触发层的双击事件
        /// </summary>
        protected internal void FireTileLayerDoubleClick(int X, int Y)
        {
            if (TileLayerDoubleClick != null)
                TileLayerDoubleClick(X, Y);
        }
        /// <summary>
        /// 触发层的双击事件
        /// </summary>
        protected internal void FireLayerDoubleClick(int Handle)
        {
            if (LayerDoubleClick != null)
                LayerDoubleClick(Handle);
        }
        /// <summary>
        /// 触发组的Checkbox被点击事件
        /// </summary>
        protected internal void FireGroupCheckboxClicked(int Handle, VisibleStateEnum State)
        {
            if (null != GroupCheckboxClicked)
                GroupCheckboxClicked(Handle, State);
        }
        /// <summary>
        /// 触发组的Expanded属性改变事件
        /// </summary>
        protected internal void FireGroupExpandedChanged(int Handle, bool Expanded)
        {
            if (null != GroupExpandedChanged)
                GroupExpandedChanged(Handle, Expanded);
        }
        /// <summary>
        /// 触发层的checkbox被点击事件
        /// </summary>
        protected internal void FireLayerCheckboxClicked(int Handle, bool NewState)
        {
            if (null != LayerCheckboxClicked)
                LayerCheckboxClicked(Handle, NewState);
        }
        /// <summary>
        /// 触发层的colorbox被点击事件
        /// </summary>
        protected internal void FireLayerColorboxClicked(int Handle)
        {
            if (LayerColorboxClicked != null)
                LayerColorboxClicked(Handle);
        }
        /// <summary>
        /// 触发shapefile被点击事件
        /// </summary>
        protected internal void FireLayerCategoryClicked(int Handle, int Category)
        {
            if (LayerCategoryClicked != null)
                LayerCategoryClicked(Handle, Category);
        }
        /// <summary>
        /// 触发层的chart Iocn被点击事件
        /// </summary>
        protected internal void FireLayerChartClicked(int Handle)
        {
            if (LayerChartClicked != null)
                LayerChartClicked(Handle);
        }
        /// <summary>
        /// 触发层的chart fields被点击事件
        /// </summary>
        protected internal void FireLayerChartFieldClicked(int Handle, int Field)
        {
            if (LayerChartFieldClicked != null)
                LayerChartFieldClicked(Handle, Field);
        }
        /// <summary>
        /// 触发层的label preview 被点击事件
        /// </summary>
        protected internal void FireLayerLabelClicked(int Handle)
        {
            if (LayerLabelClicked != null)
                LayerLabelClicked(Handle);
        }
        /// <summary>
        /// 触发LayerLabelCategoryClicked
        /// </summary>
        protected internal void FireLayerLabelCategoryClicked(int Handle, int Category)
        {
            if (LayerLabelCategoryClicked != null)
                LayerLabelCategoryClicked(Handle, Category);
        }
        /// <summary>
        /// 触发层的label icon被点击事件
        /// </summary>
        protected internal void FireLayerLabelsClicked(int Handle)
        {
            if (LayerLabelsClicked != null)
                this.LayerLabelsClicked(Handle);
        }
        /***************************26**********************************/

        #endregion 26

        public Legend()
        {
            InitializeComponent();

            m_GroupManager = new Groups(this);
            m_LayerManager = new Layers(this);

            m_LockCount = 0;
            m_SelectedLayerHandle = -1;
            m_SelectedGroupHandle = -1;
            m_Font = new Font("Arial", 8);
            m_BoldFont = new Font("Arial", 8, System.Drawing.FontStyle.Bold);
            m_SelectedColor = Color.FromArgb(255, 240, 240, 240);
            m_BoxLineColor = Color.Gray;
            m_showGroupFolders = true;
            
            DisplayTilesGroup = false;
        }

        /// <summary>
        /// 在legend上获取和设置layer的可见性
        /// </summary>
        public bool ShowLabels
        {
            get { return m_ShowLabels; }
            set { m_ShowLabels = value; }
        }

        /// <summary>
        /// 获取操作组的方法
        /// </summary>
        public Groups Groups
        {
            get
            {
                return m_GroupManager;
            }
        }

        /// <summary>
        /// 获取操作层的方法（无需考虑Groups）
        /// </summary>
        public Layers Layers
        {
            get
            {
                return m_LayerManager;
            }
        }

        /// <summary>
        /// 添加一个新组到组列表中
        /// </summary>
        protected internal int AddGroup(string Name)
        {
            return AddGroup(Name, -1);
        }

        /// <summary>
        /// 添加一个新组到组列表中
        /// </summary>
        /// <param name="Name">显示在legend的上Caption</param>
        /// <param name="Position">组添加的位置（从0开始）</param>
        /// <returns>组的句柄，-1失败</returns>
        protected internal int AddGroup(string Name, int Position)
        {
            Group grp = CreateGroup(Name, Position);
            if (grp == null)
            {
                Globals.LastError = "创建Group实例失败";
                return INVALID_GROUP;
            }

            Redraw();

            FireGroupAdded(grp.Handle);

            return grp.Handle;
        }

        /// <summary>
        /// 获取设置在legend中选择层的背景色
        /// </summary>
        public System.Drawing.Color SelectedColor
        {
            get
            {
                return m_SelectedColor;
            }
            set
            {
                m_SelectedColor = value;
            }
        }

        /// <summary>
        /// 获取和设置是否显示组的Folders
        /// </summary>
        public bool ShowGroupFolders
        {
            get
            {
                return m_showGroupFolders;
            }
            set
            {
                m_showGroupFolders = value;
            }
        }

        /// <summary>
        /// 从组列表中移除一个组
        /// </summary>
        protected internal bool RemoveGroup(int Handle)
        {
            Group grp = null;
            bool LayerInGroupWasSelected = false;

            if (IsValidGroup(Handle) == true)
            {
                int index = (int)m_GroupPositions[Handle];
                grp = (Group)m_AllGroups[index];

                //同步移除该组中的所有层
                while (grp.m_Layers.Count > 0)
                {
                    int lyr = ((Layer)grp.m_Layers[0]).Handle;
                    LayerInGroupWasSelected = LayerInGroupWasSelected || (m_SelectedLayerHandle == lyr);
                    RemoveLayer(lyr);
                    GC.Collect();
                }

                m_AllGroups.RemoveAt(index);
                UpdateGroupPositions();

                if (LayerInGroupWasSelected)
                {
                    m_SelectedLayerHandle = (m_Map.NumLayers > 0 ? m_Map.get_LayerHandle(m_Map.NumLayers - 1) : -1);

                    FireLayerSelected(m_SelectedLayerHandle);
                }

                Redraw();

                FireGroupRemoved(Handle);
            }
            else
            {
                Globals.LastError = "组将句柄无效";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从层列表中移除一个层
        /// </summary>
        protected internal bool RemoveLayer(int LayerHandle)
        {
            int GroupIndex,
                LayerIndex;
            Layer lyr = FindLayerByHandle(LayerHandle, out GroupIndex, out LayerIndex);
            if (lyr == null)
                return false;

            Group grp = (Group)m_AllGroups[GroupIndex];
            grp.m_Layers.RemoveAt(LayerIndex);

            m_Map.RemoveLayer(LayerHandle);

            if (LayerHandle == m_SelectedLayerHandle)
            {
                m_SelectedLayerHandle = m_Map.get_LayerHandle(m_Map.NumLayers - 1);

                FireLayerSelected(m_SelectedLayerHandle);
            }

            grp.RecalcHeight();
            Redraw();

            //Christian Degrassi 2010-02-25: This fixes issue 1622
            FireLayerRemoved(LayerHandle);

            return true;
        }

        /// <summary>
        /// 更新组的位置
        /// </summary>
        private void UpdateGroupPositions()
        {
            int grpCount = m_AllGroups.Count;
            int HandleCount = m_GroupPositions.Count;
            int i;

            //重置所有位置
            for (i = 0; i < HandleCount; i++)
                m_GroupPositions[i] = INVALID_GROUP;

            //为存在的组设置有效的组位置
            for (i = 0; i < grpCount; i++)
                m_GroupPositions[((Group)m_AllGroups[i]).Handle] = i;
        }

        /// <summary>
        /// 创建一个新的组
        /// </summary>
        private Group CreateGroup(string caption, int Position)
        {
            Group grp = new Group(this);
            if (caption.Length < 1)
                grp.Text = "新建组";
            else
                grp.Text = caption;

            grp.m_Handle = m_GroupPositions.Count;
            m_GroupPositions.Add(INVALID_GROUP);

            if (Position < 0 || Position >= m_AllGroups.Count)
            {
                //放置在顶部
                m_GroupPositions[grp.Handle] = m_AllGroups.Count;
                m_AllGroups.Add(grp);
            }
            else
            {
                //放置在指定position位置
                m_GroupPositions[grp.Handle] = Position;
                m_AllGroups.Insert(Position, grp);

                UpdateGroupPositions();
            }
            return grp;
        }

        /// <summary>
        /// 绘制一个矩形盒
        /// </summary>
        private void DrawBox(Graphics DrawTool, Rectangle rect, Color LineColor)
        {
            Pen pen;
            pen = new Pen(LineColor);

            DrawTool.DrawRectangle(pen, rect);
            pen = null;
        }

        /// <summary>
        /// 绘制一个矩形盒
        /// </summary>
        private void DrawBox(Graphics DrawTool, Rectangle rect, Color LineColor, Color BackColor)
        {
            Pen pen;
            pen = new Pen(BackColor);
            DrawTool.FillRectangle(pen.Brush, rect);

            pen = new Pen(LineColor);

            DrawTool.DrawRectangle(pen, rect);
            pen = null;
        }

        /// <summary>
        /// 根据局部变量m_BackBuffer和m_FrontBuffer绘制图像
        /// </summary>
        private void SwapBuffers()
        {
            SwapBuffers(m_BackBuffer, m_FrontBuffer);
        }

        /// <summary>
        /// 根据参数BackBuffer和局部变量m_FrontBuffer绘制图像
        /// </summary>
        /// <param name="BackBuffer"></param>
        private void SwapBuffers(System.Drawing.Image BackBuffer)
        {
            SwapBuffers(BackBuffer, m_FrontBuffer);
        }

        /// <summary>
        /// 根据参数BackBuffer、FrontBuffer绘制图像
        /// </summary>
        private void SwapBuffers(System.Drawing.Image BackBuffer, System.Drawing.Graphics FrontBuffer)
        {
            try
            {
                // 随即选择的属性DpiX，以确保该FrontBuffer是有效的
                float k = FrontBuffer.DpiX;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                return;
            }

            FrontBuffer.DrawImage(BackBuffer, 0, 0);
            FrontBuffer.Flush(FlushIntention.Sync);
        }

        /// <summary>
        /// 根据参数BackBuffer、FrontBuffer绘制图像
        /// </summary>
        private void SwapBuffers(System.Drawing.Image BackBuffer, System.Drawing.Image FrontBuffer)
        {
            Graphics draw = Graphics.FromImage(FrontBuffer);
            draw.DrawImage(BackBuffer, 0, 0);
            draw.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
        }

        /// <summary>
        /// 根据给定的<c>DrawTool</c>对象绘制一个组
        /// </summary>
        protected internal void DrawGroup(Graphics DrawTool, Group grp, Rectangle bounds, bool IsSnapshot)
        {
            int CurLeft = Constants.GRP_INDENT,
                CurWidth = bounds.Width - Constants.GRP_INDENT - Constants.ITEM_RIGHT_PAD,
                CurTop = bounds.Top,
                CurHeight = Constants.ITEM_HEIGHT;

            Rectangle rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

            CurLeft = Constants.GRP_INDENT + Constants.EXPAND_BOX_LEFT_PAD;
            CurTop = bounds.Top;
            bool DrawCheck = false;
            //Color BoxBackColor = Color.White;
            bool DrawGrayCheckbox = false;
            int GroupHeight = grp.Height;
            grp.Top = bounds.Top;

            if (grp.VisibleState == VisibleStateEnum.vsALL_VISIBLE || grp.VisibleState == VisibleStateEnum.vsPARTIAL_VISIBLE)
                DrawCheck = true;

            //当该组中的层是选择状态，为该层绘制一个边界
            if (grp.Handle == m_SelectedGroupHandle && grp.Expanded == false && IsSnapshot == false)
            {
                rect = new Rectangle(Constants.GRP_INDENT, CurTop, bounds.Width - Constants.GRP_INDENT - Constants.ITEM_RIGHT_PAD, Constants.ITEM_HEIGHT);
                DrawBox(DrawTool, rect, m_BoxLineColor, m_SelectedColor);
            }

            //当该组中存在内容，绘制一个“+”-“展开式的盒子
            if (grp.m_Layers.Count > 0 && IsSnapshot == false)
                DrawExpansionBox(DrawTool, bounds.Top + Constants.EXPAND_BOX_TOP_PAD, Constants.GRP_INDENT + Constants.EXPAND_BOX_LEFT_PAD, grp.Expanded);

            if (grp.VisibleState == VisibleStateEnum.vsPARTIAL_VISIBLE)
                DrawGrayCheckbox = true;
            //BoxBackColor = Color.LightGray;

            if (IsSnapshot == false && grp.Expanded == true && grp.m_Layers.Count > 0 && Map.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                int endY = grp.Top + Constants.ITEM_HEIGHT;

                Pen BlackPen = new Pen(m_BoxLineColor);
                DrawTool.DrawLine(BlackPen, Constants.VERT_LINE_INDENT, bounds.Top + Constants.VERT_LINE_GRP_TOP_OFFSET, Constants.VERT_LINE_INDENT, endY);
            }

            CurLeft = Constants.GRP_INDENT;
            if (bounds.Width > 35 && IsSnapshot == false)
            {
                if (!grp.StateLocked)
                {
                    CurLeft = Constants.GRP_INDENT + Constants.CHECK_LEFT_PAD;
                    DrawCheckBox(DrawTool, bounds.Top + Constants.CHECK_TOP_PAD, CurLeft, DrawCheck, DrawGrayCheckbox);
                }
            }

            CurLeft = Constants.GRP_INDENT + Constants.TEXT_LEFT_PAD;

            if (grp.Icon != null && bounds.Width > 55)
            {
                //绘制icon
                DrawPicture(DrawTool, bounds.Right - Constants.ICON_RIGHT_PAD, CurTop + Constants.ICON_TOP_PAD, Constants.ICON_SIZE, Constants.ICON_SIZE, grp.Icon);

                //设置边界，放置文本和图标重叠
                if (IsSnapshot == true)
                    CurLeft = Constants.GRP_INDENT;
                else
                    CurLeft = Constants.GRP_INDENT + Constants.TEXT_LEFT_PAD;

                CurTop = bounds.Top + Constants.TEXT_TOP_PAD;
                CurWidth = bounds.Width - CurLeft - Constants.TEXT_RIGHT_PAD;
                CurHeight = Constants.TEXT_HEIGHT;
                rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
            }
            else
            {
                //Bitmap bmp = LegendControl.Properties.Resources.folder1;
                //DrawPicture(DrawTool, bounds.Right - Constants.ICON_RIGHT_PAD, CurTop + Constants.ICON_TOP_PAD, Constants.ICON_SIZE, Constants.ICON_SIZE, bmp);

                if (IsSnapshot == true)
                    CurLeft = Constants.GRP_INDENT;
                else
                    CurLeft = Constants.GRP_INDENT + Constants.TEXT_LEFT_PAD;

                CurTop = bounds.Top + Constants.TEXT_TOP_PAD;
                CurWidth = bounds.Width - CurLeft - Constants.TEXT_RIGHT_PAD_NO_ICON;
                CurHeight = Constants.TEXT_HEIGHT;
                rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
            }

            // 组图标
            if (Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology && m_showGroupFolders)
            {
                int size = 16;
                Bitmap bmp = grp.Expanded ? MapWinGIS.LegendControl.Properties.Resources.folder_open : MapWinGIS.LegendControl.Properties.Resources.folder;
                rect.Offset(0, -2);
                DrawPicture(DrawTool, rect.Left, rect.Top, size, size, bmp);

                rect = new Rectangle(rect.X + size + 3, rect.Y + 2, rect.Width - size, rect.Height);
            }

            // 组名称
            if (grp.Handle == m_SelectedGroupHandle && IsSnapshot == false)
                DrawText(DrawTool, grp.Text, rect, m_BoldFont);
            else
                DrawText(DrawTool, grp.Text, rect, m_Font);

            //set up the boundaries for drawing list items
            CurTop = bounds.Top + Constants.ITEM_HEIGHT;

            if (grp.Expanded || IsSnapshot == true)
            {
                int ItemCount;
                ItemCount = grp.m_Layers.Count;
                Layer lyr = null;
                int newLeft = bounds.X + Constants.LIST_ITEM_INDENT;
                int newWidth = bounds.Width - newLeft;
                rect = new Rectangle(newLeft, CurTop, newWidth, bounds.Height - CurTop);

                Pen pen = new Pen(this.m_BoxLineColor);

                //now draw each of the subitems
                for (int i = ItemCount - 1; i >= 0; i--)
                {
                    lyr = (Layer)grp.m_Layers[i];
                    if (!lyr.HideFromLegend)
                    {
                        //clipping
                        if (rect.Top + lyr.Height < this.ClientRectangle.Top && IsSnapshot == false)
                        {
                            //update the rectangle for the next layer
                            rect.Y += lyr.Height;
                            rect.Height -= lyr.Height;

                            //Skip drawing this layer and move onto the next one
                            continue;
                        }

                        DrawLayer(DrawTool, lyr, rect, IsSnapshot);

                        bool drawLines = (m_Map.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology);

                        if (IsSnapshot == false && drawLines == true)
                        {
                            //draw sub-item vertical line
                            if (i != 0 && !((Layer)grp.m_Layers[i - 1]).HideFromLegend)//not the last visible layer
                                DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, lyr.Top, Constants.VERT_LINE_INDENT, lyr.Top + lyr.Height + Constants.ITEM_PAD);
                            else//only draw down to box, not down to next item in list(since there is no next item)
                                DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, lyr.Top, Constants.VERT_LINE_INDENT, (int)(lyr.Top + .55 * Constants.ITEM_HEIGHT));

                            //draw Horizontal line over to the Vertical Sub-lyr line
                            CurTop = (int)(rect.Top + .5 * Constants.ITEM_HEIGHT);

                            if ((lyr.ColorLegend == null || lyr.ColorLegend.Count == 0)
                                && (lyr.PointImageScheme == null || lyr.PointImageScheme.NumberItems == 0))

                                // Color or image schemes do not exist with the layer

                                // if the layer is selected
                                if (lyr.Handle == m_SelectedLayerHandle)
                                {
                                    DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, CurTop, rect.Left + Constants.EXPAND_BOX_LEFT_PAD - 3, CurTop);
                                }
                                else
                                {
                                    // if the layer is not selected
                                    DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, CurTop, rect.Left + Constants.CHECK_LEFT_PAD, CurTop);
                                    //DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, CurTop, rect.Left + Constants.EXPAND_BOX_LEFT_PAD, CurTop);
                                }
                            //
                            else
                            {
                                // There is color or image scheme with the layer

                                // if the layer is selected
                                if (lyr.Handle == m_SelectedLayerHandle)
                                    DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, CurTop, rect.Left + Constants.EXPAND_BOX_LEFT_PAD - 3, CurTop);
                                // if the layer is not selected
                                else
                                    DrawTool.DrawLine(pen, Constants.VERT_LINE_INDENT, CurTop, rect.Left + Constants.EXPAND_BOX_LEFT_PAD, CurTop);
                            }

                            //set up the rectangle for the next layer
                            rect.Y += lyr.Height;
                            rect.Height -= lyr.Height;
                        }
                        else if (IsSnapshot == true)
                        {
                            rect.Y += lyr.CalcHeight(true);
                            rect.Height -= lyr.CalcHeight(true);
                        }
                        else
                        {
                            rect.Y += lyr.CalcHeight(lyr.Expanded);
                            rect.Height -= lyr.CalcHeight(lyr.Expanded);
                        }

                        if (rect.Top >= this.ClientRectangle.Bottom && IsSnapshot == false)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制指定颜色的文本
        /// </summary>
        private void DrawText(Graphics DrawTool, string text, Rectangle rect, Font font, Color penColor)
        {
            Pen pen = new Pen(penColor);
            DrawTool.DrawString(text, font, pen.Brush, rect);
        }

        /// <summary>
        /// 绘制普通（默认黑色）文本
        /// </summary>
        private void DrawText(Graphics DrawTool, string text, Rectangle rect, Font font)
        {
            DrawText(DrawTool, text, rect, font, Color.Black);
        }

        /// <summary>
        /// 获取和设置在legend中选择的layer
        /// </summary>
        public int SelectedLayer
        {
            get
            {
                if (m_LayerManager == null) return 0;
                return (m_LayerManager.Count == 0 ? -1 : m_SelectedLayerHandle);
            }

            set
            {
                int GroupIndex, LayerIndex;

                if (m_SelectedLayerHandle != value && FindLayerByHandle(value, out GroupIndex, out LayerIndex) != null)
                {
                    //只有在层的handle有效并且选择的layer改变时重绘
                    m_SelectedLayerHandle = value;
                    m_SelectedGroupHandle = ((Group)m_AllGroups[GroupIndex]).Handle;

                    FireLayerSelected(value);

                    Redraw();
                }
            }
        }

        /// <summary>
        /// 在组的顶部添加一个layer
        /// </summary>
        protected internal int AddLayer(object newLayer, bool Visible, int TargetGroupHandle)
        {
            return AddLayer(newLayer, Visible, TargetGroupHandle, true);
        }

        /// <summary>
        /// 添加一个层（layer）到地图上。可以选择将它放置在当前选择layer的上面，也可以放置在所有layer的上面
        /// </summary>
        /// <param name="newLayer">要添加的层（必须符合层类型）</param>
        /// <param name="Visible">添加的层是否可见</param>
        /// <param name="PlaceAboveCurrentlySelected">是将该层放置在当前选择的层的上面，还是所有层的上面</param>
        /// <returns>新添加层的Handle, -1 添加失败</returns>
        protected internal int AddLayer(object newLayer, bool Visible, bool PlaceAboveCurrentlySelected)
        {
            int MapLayerHandle = AddLayer(newLayer, Visible);

            if (!PlaceAboveCurrentlySelected) return MapLayerHandle;

            if (SelectedLayer != -1 && PlaceAboveCurrentlySelected)
            {
                int addPos = 0;
                int addGrp = 0;
                addPos = Layers.PositionInGroup(SelectedLayer) + 1;
                addGrp = Layers.GroupOf(SelectedLayer);
                Layers.MoveLayer(MapLayerHandle, addGrp, addPos);
                return MapLayerHandle;
            }

            return MapLayerHandle;
        }

        /// <summary>
        /// 在组的顶部添加一个layer
        /// </summary>
        /// <param name="newLayer">要添加的层（必须符合层类型）</param>
        /// <param name="Visible">添加的层是否可见</param>
        /// <param name="TargetGroupHandle">要添加layer进去的组的句柄</param>
        /// <param name="LegendVisible">该layer是否在legend中可见</param>
        /// <returns>层的Handle, -1 失败</returns>
        protected internal int AddLayer(object newLayer, bool Visible, int TargetGroupHandle, bool LegendVisible)
        {
            Group grp = null;

            if (m_Map == null)
                throw new System.Exception("MapWinGIS.Map 没有设置. 请在添加图层前设置Map的属性");

            m_Map.LockWindow(MapWinGIS.tkLockMode.lmLock);
            int MapLayerHandle = m_Map.AddLayer(newLayer, Visible);
            //newLayer = new Object();

            if (MapLayerHandle < 0)
            {
                m_Map.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                return MapLayerHandle;
            }

            if (m_AllGroups.Count == 0 || IsValidGroup(TargetGroupHandle) == false)
            {
                //we have to create or find a group to put this layer into

                if (m_AllGroups.Count == 0)
                {
                    grp = CreateGroup("数据图层", -1);
                    m_GroupPositions[grp.Handle] = m_AllGroups.Count - 1;

                    FireGroupAdded(grp.Handle);
                }
                else
                {
                    grp = (Group)m_AllGroups[m_AllGroups.Count - 1];
                }
            }
            else
            {
                grp = (Group)m_AllGroups[(int)m_GroupPositions[TargetGroupHandle]];
            }

            Layer lyr = CreateLayer(MapLayerHandle, newLayer);
            lyr.HideFromLegend = !LegendVisible;

            grp.m_Layers.Add(lyr);
            grp.RecalcHeight();

            grp.UpdateGroupVisibility();

            // 不显示预览
            MapWinGIS.Shapefile sf = (newLayer as MapWinGIS.Shapefile);


            if (LegendVisible)
            {
                m_SelectedGroupHandle = grp.Handle;
                m_SelectedLayerHandle = MapLayerHandle;

                FireLayerSelected(MapLayerHandle);
            }

            m_Map.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
            Redraw();

            FireLayerAdded(MapLayerHandle);

            return MapLayerHandle;
        }

        /// <summary>
        /// 在组的顶部添加一个layer
        /// </summary>
        /// <param name="newLayer">要添加的层（必须符合层类型）</param>
        /// <param name="Visible">层是否可见</param>
        /// <returns>层的Handle, -1 失败</returns>
        protected internal int AddLayer(object newLayer, bool Visible)
        {
            return AddLayer(newLayer, Visible, -1, true);
        }

        /// <summary>
        /// 创建一个新的层
        /// </summary>
        private Layer CreateLayer(int Handle, object newLayer)
        {
            Layer lyr = new Layer(this);
            lyr.m_Handle = Handle;
            lyr.Type = GetLayerType(newLayer);
            if (lyr.Type == eLayerType.Image)
            {
                lyr.HasTransparency = HasTransparency(newLayer);
            }
            return lyr;
        }

        /// <summary>
        /// 获取legend中所有layers的闪照
        /// </summary>
        public System.Drawing.Bitmap Snapshot()
        {
            return Snapshot(false, 200);
        }

        /// <summary>
        /// 获取legend中所有layers的闪照
        /// </summary>
        /// <param name="imgWidth">想要闪照的宽度（pix）</param>
        public System.Drawing.Bitmap Snapshot(int imgWidth)
        {
            return Snapshot(false, imgWidth);
        }

        /// <summary>
        /// 获取legend中所有layers的闪照
        /// </summary>
        /// <param name="VisibleLayersOnly">是否只对可见的layer闪照</param>
        public System.Drawing.Bitmap Snapshot(bool VisibleLayersOnly)
        {
            return Snapshot(VisibleLayersOnly, 200);
        }

        /// <summary>
        /// 获取legend中所有layers的指定字体的闪照
        /// </summary>
        /// <param name="VisibleLayersOnly">是否只对可见的layer闪照</param>
        /// <param name="useFont">legend字体用哪种font</param>
        public System.Drawing.Bitmap Snapshot(bool VisibleLayersOnly, Font useFont)
        {
            Font o = m_Font;
            m_Font = useFont;
            Bitmap b = Snapshot(VisibleLayersOnly, 200);
            m_Font = o;
            return b;
        }

        /// <summary>
        /// 获取legend中所有layers的指定字体和宽度的闪照
        /// </summary>
        /// <param name="VisibleLayersOnly">是否只对可见的layer闪照</param>
        /// <param name="imgWidth">图像的宽度</param>
        /// <param name="useFont">legend文本字体</param>
        public System.Drawing.Bitmap Snapshot(bool VisibleLayersOnly, int imgWidth, Font useFont)
        {
            Font o = m_Font;
            m_Font = useFont;
            Bitmap b = Snapshot(VisibleLayersOnly, imgWidth);
            m_Font = o;
            return b;
        }

        /// <summary>
        /// 获取legend中所有layers的指定字体、宽度和前景色的闪照
        /// </summary>
        /// <param name="VisibleLayersOnly">是否只对可见的layer闪照</param>
        /// <param name="imgWidth">图像的宽度</param>
        /// <param name="useFont">legend文本字体</param>
        /// <param name="foreColor">控件的前景色</param>
        public System.Drawing.Bitmap Snapshot(bool VisibleLayersOnly, int imgWidth, Font useFont, Color foreColor)
        {
            System.Drawing.Color fore = this.ForeColor;
            this.ForeColor = foreColor;

            Font o = m_Font;
            m_Font = useFont;

            Bitmap b = null;
            try
            {
                this.Lock();
                b = Snapshot(VisibleLayersOnly, imgWidth, useFont);

            }
            finally
            {
                this.ForeColor = fore;
                m_Font = o;
                this.Unlock();
            }
            return b;
        }

        /// <summary>
        /// 获取legend中所有layers的指定宽度的闪照
        /// </summary>
        /// <param name="VisibleLayersOnly">是否只对可见的layer闪照</param>
        /// <param name="imgWidth">图像的宽度</param>
        public System.Drawing.Bitmap Snapshot(bool VisibleLayersOnly, int imgWidth)
        {
            int imgHeight = 0;// = CalcTotalDrawHeight(true);
            Bitmap bmp = null;// = new Bitmap(imgWidth,imgHeight);
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            int GrpCount, i;
            Layer lyr;
            int LyrHeight;

            bool expandedHeight = false;

            try
            {
                System.Drawing.Graphics g;

                if (VisibleLayersOnly == true)
                {
                    ArrayList VisibleLayers = new ArrayList();

                    //figure out how big the img needs to be in height
                    for (i = m_LayerManager.Count - 1; i >= 0; i--)
                    {
                        lyr = m_LayerManager[i];
                        if (lyr.Visible && !lyr.HideFromLegend)
                        {
                            imgHeight += lyr.CalcHeight(expandedHeight) - 1;
                            VisibleLayers.Add(lyr);
                        }
                    }

                    imgHeight += Constants.ITEM_PAD;

                    bmp = new Bitmap(imgWidth, imgHeight, this.CreateGraphics());
                    g = Graphics.FromImage(bmp);
                    g.Clear(this.BackColor);

                    if (VisibleLayers.Count > 0)
                    {	//set up the boundaries for the first layer
                        LyrHeight = ((Layer)VisibleLayers[0]).CalcHeight(expandedHeight);
                        rect = new Rectangle(2, 2, imgWidth - 4, LyrHeight - 1);
                    }

                    for (i = 0; i < VisibleLayers.Count; i++)
                    {
                        lyr = (Layer)VisibleLayers[i];

                        this.DrawLayer(g, lyr, rect, true);

                        LyrHeight = lyr.CalcHeight(expandedHeight);

                        rect.Y += LyrHeight - 1;
                        rect.Height = LyrHeight;
                    }
                }
                else
                {//draw all layers
                    GrpCount = m_GroupManager.Count;
                    Group grp;
                    int LyrCount;

                    imgHeight = 0;

                    //figure out how tall the image is going to need to be
                    for (i = GrpCount - 1; i >= 0; i--)
                    {
                        grp = (Group)m_GroupManager[i];
                        LyrCount = grp.m_Layers.Count;
                        for (int j = LyrCount - 1; j >= 0; j--)
                        {
                            lyr = (Layer)grp.m_Layers[j];
                            if (!lyr.HideFromLegend)
                            {
                                imgHeight += lyr.CalcHeight(expandedHeight) - 1;
                            }
                        }
                    }

                    imgHeight += Constants.ITEM_PAD;

                    //create a new bitmap of the right size, then create a graphics object from the bitmap
                    bmp = new Bitmap(imgWidth, imgHeight, this.CreateGraphics());
                    g = Graphics.FromImage(bmp);
                    g.Clear(this.BackColor);

                    rect = new Rectangle(2, 2, imgWidth - 4, imgHeight - 1);

                    //now draw the snapshot
                    for (i = GrpCount - 1; i >= 0; i--)
                    {
                        grp = (Group)m_GroupManager[i];
                        LyrCount = grp.m_Layers.Count;
                        for (int j = LyrCount - 1; j >= 0; j--)
                        {
                            lyr = (Layer)grp.m_Layers[j];
                            if (!lyr.HideFromLegend)
                            {
                                this.DrawLayer(g, lyr, rect, true);

                                LyrHeight = lyr.CalcHeight(expandedHeight);

                                rect.Y += LyrHeight - 1;
                                rect.Height = LyrHeight;
                            }
                        }
                    }
                }

                return bmp;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("错误: Legend.Snaphot. " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取指定layer的闪照
        /// </summary>
        /// <param name="LayerHandle">请求layer的handle</param>
        protected internal System.Drawing.Bitmap LayerSnapshot(int LayerHandle)
        {
            return LayerSnapshot(LayerHandle, 200);
        }

        /// <summary>
        /// 获取指定layer的闪照
        /// </summary>
        /// <param name="LayerHandle">请求layer的handle</param>
        /// <param name="imgWidth">图像的宽度</param>
        protected internal Bitmap LayerSnapshot(int LayerHandle, int imgWidth)
        {
            if (Layers.IsValidHandle(LayerHandle) == false)
                return null;

            Layer lyr = Layers.ItemByHandle(LayerHandle);

            Bitmap bmp;
            Graphics g;
            int LyrHeight = lyr.CalcHeight(true);
            bmp = new Bitmap(imgWidth, LyrHeight);
            g = Graphics.FromImage(bmp);

            Rectangle rect = new Rectangle(0, 0, imgWidth - 1, LyrHeight - 1);
            DrawLayer(g, lyr, rect, true);

            return bmp;
        }

        /// <summary>
        /// 是否使用了Transparency属性
        /// </summary>
        internal bool HasTransparency(object newLayer)
        {
            try
            {
                MapWinGIS.Image img = newLayer as MapWinGIS.Image;
                if (img != null)
                {
                    if (img.UseTransparencyColor == true)
                        return true;
                }
            }
            catch (System.Exception ex)
            {
                string str = ex.ToString();
            }

            return false;
        }

        /// <summary>
        /// 获取层的类型
        /// </summary>
        /// <param name="newLayer">要读取的层类型的层对象</param>
        /// <returns></returns>
        private eLayerType GetLayerType(object newLayer)
        {
            if (newLayer != null)
            {
                if (newLayer is MapWinGIS.Image)
                {
                    return eLayerType.Image;
                }
                else if (newLayer is MapWinGIS.Shapefile)
                {
                    MapWinGIS.Shapefile shpfile = (MapWinGIS.Shapefile)newLayer;
                    if (shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POINT
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTM
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POINTZ
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINT
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTM
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_MULTIPOINTZ)
                        return eLayerType.PointShapefile;
                    else if (shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINE
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEM
                        || shpfile.ShapefileType == MapWinGIS.ShpfileType.SHP_POLYLINEZ)
                        return eLayerType.LineShapefile;
                    else
                        return eLayerType.PolygonShapefile;
                }
            }
            return eLayerType.Invalid;
        }

        /// <summary>
        /// 获取和设置MapWinGIS.Map与legend控件联系
        /// 这个属性必须在操作layers前设置
        /// </summary>
        public MapWinGIS.Map Map
        {
            get { return m_Map; }
            set
            {
                m_Map = value;

                //添加tiles组，只有的那个Map的引用被设置后才能添加
                if (this.tileGroup == null && m_Map != null)
                {
                    tileGroup = new Group(this);
                    tileGroup.Text = "Tiles瓦片";
                    Layer l = this.CreateLayer(-1, null);
                    l.Type = eLayerType.Tiles;
                    tileGroup.m_Layers.Add(l);
                }
            }
        }

        /// <summary>
        /// 指定Handle的组是否存在
        /// </summary>
        protected internal bool IsValidGroup(int Handle)
        {
            if (Handle >= 0 && Handle < m_GroupPositions.Count)
            {
                if ((int)m_GroupPositions[Handle] >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 通过Handle查找一个层
        /// </summary>
        /// <param name="Handle">给定的Handle</param>
        /// <param name="GroupIndex">该层所属的组的句柄</param>
        /// <param name="LayerIndex">在对应组中该层的句柄（与给定的相同）</param>
        /// <returns>返回一个层对象，null没找到</returns>
        protected internal Layer FindLayerByHandle(int Handle, out int GroupIndex, out int LayerIndex)
        {
            GroupIndex = -1;
            LayerIndex = -1;

            int GroupCount, ItemCount;
            GroupCount = m_AllGroups.Count;
            Group grp;
            Layer lyr;

            for (int i = 0; i < GroupCount; i++)
            {
                grp = (Group)m_AllGroups[i];
                ItemCount = grp.m_Layers.Count;
                for (int j = 0; j < ItemCount; j++)
                {
                    lyr = (Layer)grp.m_Layers[j];
                    if (lyr.Handle == Handle)
                    {
                        GroupIndex = i;
                        LayerIndex = j;
                        return lyr;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通过handle查找一个层
        /// </summary>
        protected internal Layer FindLayerByHandle(int Handle)
        {
            int GroupIndex, LayerIndex;
            return FindLayerByHandle(Handle, out GroupIndex, out LayerIndex);
        }

        /// <summary>
        /// 绘制checkbox
        /// </summary>
        /// <param name="DrawTool">绘图对象</param>
        /// <param name="ItemTop"></param>
        /// <param name="ItemLeft"></param>
        /// <param name="DrawCheck">是否绘制成选择状态</param>
        /// <param name="DrawGrayBackground">是否绘制灰色背景</param>
        private void DrawCheckBox(Graphics DrawTool, int ItemTop, int ItemLeft, bool DrawCheck, bool DrawGrayBackground)
        {
            System.Drawing.Image icon;
            int index = 0;
            if (DrawCheck == true)
            {
                if (DrawGrayBackground == true)
                    index = cCheckedBoxGrayIcon;
                else
                    index = cCheckedBoxIcon;
            }
            else
            {
                if (DrawGrayBackground == true)
                    index = cUnCheckedBoxGrayIcon;
                else
                    index = cUnCheckedBoxIcon;
            }
            icon = Icons.Images[index];
            DrawPicture(DrawTool, ItemLeft, ItemTop, Constants.CHECK_BOX_SIZE, Constants.CHECK_BOX_SIZE, icon);
        }

        /// <summary>
        /// 在legend中绘制一张图片，图片可以是image和icon类型
        /// </summary>
        private void DrawPicture(Graphics DrawTool, int PicLeft, int PicTop, int PicWidth, int PicHeight, object picture)
        {
            if (picture == null) return;

            System.Drawing.Drawing2D.SmoothingMode oldSM = DrawTool.SmoothingMode;
            DrawTool.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Rectangle rect = new Rectangle(PicLeft, PicTop, PicWidth, PicHeight);

            Icon icon = null;
            if (picture is Icon)
            {
                icon = (Icon)picture;
            }

            if (icon != null)
            {
                DrawTool.DrawIcon(icon, rect);
            }
            else
            {
                //尝试转换换成image类型
                System.Drawing.Image img = null;
                try { img = (System.Drawing.Image)picture; }
                catch (System.InvalidCastException) { }

                if (img != null)
                {
                    DrawTool.DrawImage(img, rect);
                }
                else
                {
                    MapWinGIS.Image mwImg = null;
                    try { mwImg = (MapWinGIS.Image)picture; }
                    catch (System.InvalidCastException) { }
                    if (mwImg != null)
                    {
                        try
                        {
                            img = System.Drawing.Image.FromHbitmap(new System.IntPtr(mwImg.Picture.Handle));
                            DrawTool.DrawImage(img, rect);
                        }
                        catch (Exception) { }
                    }
                    mwImg = null;
                }
            }

            DrawTool.SmoothingMode = oldSM;
        }

        /// <summary>
        /// 绘制展开标识框“+”“-”
        /// </summary>
        private void DrawExpansionBox(Graphics DrawTool, int ItemTop, int ItemLeft, bool Expanded)
        {
            int MidX, MidY;
            Rectangle rect;

            Pen pen = new Pen(m_BoxLineColor, 1);

            rect = new Rectangle(ItemLeft, ItemTop, Constants.EXPAND_BOX_SIZE, Constants.EXPAND_BOX_SIZE);

            //绘制边框
            DrawBox(DrawTool, rect, m_BoxLineColor, Color.White);

            MidX = (int)(rect.Left + .5 * (rect.Width));
            MidY = (int)(rect.Top + .5 * (rect.Height));

            if (Expanded == false)
            {
                //绘制一个“+”及垂直部分
                DrawTool.DrawLine(pen, MidX, ItemTop + 2, MidX, ItemTop + Constants.EXPAND_BOX_SIZE - 2);

                //绘制水平部分
                DrawTool.DrawLine(pen, ItemLeft + 2, MidY, ItemLeft + Constants.EXPAND_BOX_SIZE - 2, MidY);
            }
            else
            {
                //绘制“-”
                DrawTool.DrawLine(pen, ItemLeft + 2, MidY, ItemLeft + Constants.EXPAND_BOX_SIZE - 2, MidY);
            }
        }

        /// <summary>
        /// 锁定legend，阻止legend重绘
        /// 当操作legend时，确保已解锁
        /// </summary>
        public void Lock()
        {
            m_LockCount++;
        }

        /// <summary>
        /// 解锁legend
        /// </summary>
        public void Unlock()
        {
            if (m_LockCount > 0)
                m_LockCount--;
            if (m_LockCount == 0)
                Redraw();
        }

        /// <summary>
        /// 提供一个高质量的闪照
        /// </summary>
        /// <param name="LayerHandle">层的handle</param>
        /// <param name="Width">图像的宽度</param>
        /// <param name="Columns">Number of desired columns.</param>
        /// <param name="FontFamily">使用的字体</param>
        /// <param name="MinFontSize">最小字号</param>
        /// <param name="MaxFontSize">最大字号</param>
        /// <param name="UnderlineLayerTitles">layer标题是否使用下划线</param>
        /// <param name="BoldLayerTitles">layer标题是否加粗</param>
        /// <returns></returns>
        public System.Drawing.Bitmap SnapshotHQ(int LayerHandle, int Width, int Columns, string FontFamily, int MinFontSize, int MaxFontSize, bool UnderlineLayerTitles, bool BoldLayerTitles)
        {
            int CurLeft = 0,
                CurTop = 0;

            Layer lyr = m_LayerManager.ItemByHandle(LayerHandle);
            Bitmap b = new Bitmap(Width, 1000);

            //创建image的大小
            string TestString = m_Map.get_LayerName(lyr.Handle);
            object o = m_Map.GetColorScheme(lyr.Handle);
            if (o != null && o is MapWinGIS.GridColorScheme)
            {
                MapWinGIS.GridColorScheme t = (MapWinGIS.GridColorScheme)o;
                for (int i = 0; i < t.NumBreaks; i++)
                {
                    if (t.get_Break(i).Caption.Length > TestString.Length) TestString = t.get_Break(i).Caption;
                }
            }
            else if (o != null && o is MapWinGIS.ShapefileColorScheme)
            {
                MapWinGIS.ShapefileColorScheme t = (MapWinGIS.ShapefileColorScheme)o;
                for (int i = 0; i < t.NumBreaks(); i++)
                {
                    if (t.get_ColorBreak(i).Caption.Length > TestString.Length) TestString = t.get_ColorBreak(i).Caption;
                }
            }

            Graphics g = Graphics.FromImage(b);
            int currSize = MinFontSize;
            SizeF estSize = g.MeasureString(TestString.ToUpper(), new Font(FontFamily, currSize));
            // Multicolumn sizing mode - font must fit in single column - if Cols > 1
            // If cols == 1, width/1 = width
            while (estSize.Width < ((Width / Columns) * .95)) // slight tolerance
            {
                if (currSize + 1 > MaxFontSize) break;
                currSize += 1;
                estSize = g.MeasureString(TestString, new Font(FontFamily, currSize));
            }

            Font f = new Font(FontFamily, currSize);
            int LineSpacing = (int)(estSize.Height * 0.25); // 1/4行高
            int boxSize = (int)g.MeasureString("X", f).Width;

            //重建指定大小的image
            int itemMultiplier = (Math.Max(1, lyr.ColorLegend.Count) / Columns);
            // Add in either the height of the breaks + spacig, or spacing + the one box for the color
            int newHeight = (int)(estSize.Height + (lyr.ColorLegend.Count > 1 ? (estSize.Height * 1.25 * (lyr.ColorLegend.Count)) : estSize.Height * 1.25));
            // Do we have multiple columns and enough legend items for it to matter?
            if (Columns > 1 && lyr.ColorLegend.Count > 1)
            {
                // Subtract off some height to account for multi column mode
                // Done with three temporary ints to make this more readible
                int MaxPerCol = (int)Math.Ceiling((double)lyr.ColorLegend.Count / Columns);
                int SubtractPerExtraItem = (int)(estSize.Height * 1.25);
                int ExcessItemsPerCol = lyr.ColorLegend.Count - MaxPerCol;
                newHeight -= SubtractPerExtraItem * ExcessItemsPerCol;
            }
            estSize = new SizeF(Width, newHeight);
            g.Dispose();
            b.Dispose();
            b = new Bitmap((int)estSize.Width, (int)estSize.Height);
            g = Graphics.FromImage(b);

            // Layer Title
            g.DrawString(m_Map.get_LayerName(lyr.Handle), new Font(f.FontFamily, f.Size, (BoldLayerTitles ? FontStyle.Bold : FontStyle.Regular) | (UnderlineLayerTitles ? FontStyle.Underline : FontStyle.Regular)), Brushes.Black, CurLeft, CurTop);
            CurTop += (int)g.MeasureString(m_Map.get_LayerName(lyr.Handle), f).Height + LineSpacing;

            int ColumnCurTop = CurTop; // Layer name covers all columns of the legend
            int ItemsPerColumn = (lyr.ColorLegend.Count > 0 ? ((int)Math.Floor((lyr.ColorLegend.Count / Columns) + 0.5)) : lyr.ColorLegend.Count);

            int LastColumnUsed = 1;

            // Draw a single symbol if there are no breaks
            if (lyr.ColorLegend.Count == 0)
            {
                DrawHQLayerSymbol(g, lyr, CurLeft + 4, CurTop, boxSize);
            }
            else
            {
                // Otherwise, draw the breaks into the columns
                long NumBreaks = lyr.ColorLegend.Count;
                int CurrentColumn = 1;

                CurLeft = LineSpacing;

                for (int p = 0; p < NumBreaks; p++)
                {
                    ColorInfo ci = (ColorInfo)lyr.ColorLegend[p];
                    if (ci.IsTransparent == false)
                    {
                        if (lyr.Type == eLayerType.Grid || lyr.Type == eLayerType.Image)
                        {
                            DrawColorPatch(g, ci.StartColor, ci.EndColor, CurTop, CurLeft, boxSize, boxSize, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), false);
                        }
                        else
                        {
                            System.Drawing.Color sc = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.StartColor);
                            System.Drawing.Color ec = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.EndColor);
                            DrawColorPatch(g, sc, ec, CurTop, CurLeft, boxSize, boxSize, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), (m_Map.get_ShapeLayerLineWidth(lyr.Handle) != 0));
                        }
                    }
                    else
                    {
                        DrawTransparentPatch(g, CurTop, CurLeft, boxSize, boxSize, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), (m_Map.get_ShapeLayerLineWidth(lyr.Handle) != 0));
                    }

                    // New - Trimmed to column width:
                    string ActualText = ci.Caption;
                    while (g.MeasureString(ActualText, f).Width >= (Width / Columns) - LineSpacing - boxSize)
                    {
                        ActualText = ActualText.Remove(ActualText.Length - 1, 1);
                    }
                    g.DrawString(ActualText, f, Brushes.Black, CurLeft + boxSize, CurTop);

                    LastColumnUsed = CurrentColumn;

                    CurTop += boxSize + LineSpacing;
                    if (p % ItemsPerColumn == 0 && p != 0) // Last one in this column
                    {
                        CurrentColumn += 1;
                        CurTop = ColumnCurTop;
                        CurLeft = ((Width / Columns) * (CurrentColumn - 1)) + LineSpacing;
                    }
                }
            }

            g.Dispose();

            if (LastColumnUsed < Columns)
            {
                // Image will be bigger than it needs to be. Shrink
                // it by columns - lastcolumnused
                System.Drawing.Bitmap newB = new Bitmap(b.Width - ((Width / Columns) * (Columns - LastColumnUsed)), b.Height);
                Graphics newG = Graphics.FromImage(newB);
                newG.DrawImageUnscaled(b, 1, 1);
                newG.Dispose();
                return newB;
            }

            return b;
        }

        public void DrawHQLayerSymbolBreaks(Graphics g, int LayerHandle, int category, int CurTop, int CurLeft, int Width, int Height)
        {
            Layer lyr = m_LayerManager.ItemByHandle(LayerHandle);
            ColorInfo ci = new ColorInfo();
            if (lyr.ColorLegend.Count >= 1)
                ci = (ColorInfo)lyr.ColorLegend[category];
            else
            {
                ci.StartColor = Globals.UintToColor(m_Map.get_ShapeLayerFillColor(lyr.Handle));
                ci.EndColor = Globals.UintToColor(m_Map.get_ShapeLayerLineColor(lyr.Handle));
            }
            if (lyr.Type == eLayerType.Grid || lyr.Type == eLayerType.Image)
            {
                DrawColorPatch(g, ci.StartColor, ci.EndColor, CurTop, CurLeft, Width, Height, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), false);
            }
            else
            {
                System.Drawing.Color sc = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.StartColor);
                System.Drawing.Color ec = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.EndColor);
                DrawLayerSymbolHQ(g, lyr, CurLeft, CurTop, Width, sc, ec);
            }
        }

        /// <summary>
        /// 绘制一个layer类型的symbol
        /// </summary>
        private void DrawHQLayerSymbol(Graphics g, Layer lyr, int Left, int Top, int Dimension)
        {
            //draw Layer Type symbol
            if (lyr.Icon == null)
            {
                uint Fill = 0,
                    Line = 0;

                if (lyr.Type == eLayerType.LineShapefile)
                    Fill = m_Map.get_ShapeLayerLineColor(lyr.Handle);
                else if (lyr.Type == eLayerType.PointShapefile)
                    Fill = m_Map.get_ShapeLayerPointColor(lyr.Handle);
                else if (lyr.Type == eLayerType.PolygonShapefile)
                {
                    Fill = m_Map.get_ShapeLayerFillColor(lyr.Handle);
                    Line = m_Map.get_ShapeLayerLineColor(lyr.Handle);
                }

                Color FillColor = Globals.UintToColor(Fill);
                Color LineColor = Globals.UintToColor(Line);
                DrawLayerSymbolHQ(g, lyr, Left, Top, Dimension, FillColor, LineColor);
            }
            else
            {
                DrawPicture(g, Left, Top, Dimension, Dimension, lyr.Icon);
            }
        }

        /// <summary>
        /// 绘制一个层
        /// </summary>
        /// <param name="DrawTool">绘图对象</param>
        /// <param name="lyr">要绘制的layer对象</param>
        /// <param name="bounds">可绘制区域的边界</param>
        /// <param name="IsSnapshot">是否采用Snapshot绘制方式</param>
        protected internal void DrawLayer(Graphics DrawTool, Layer lyr, Rectangle bounds, bool IsSnapshot)
        {
            int CurLeft,
                CurTop,
                CurWidth,
                CurHeight;
            Rectangle rect;

            lyr.m_smallIconWasDrawn = false;
            lyr.Top = bounds.Top;
            lyr.Elements.Clear();

            CurLeft = bounds.Left;
            CurTop = bounds.Top;
            CurWidth = bounds.Width - Constants.ITEM_RIGHT_PAD;
            CurHeight = lyr.Height;
            rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

            // ------------------------------------------------------
            //  绘制背景（当图层时选择状态时需要绘制背景）
            // ------------------------------------------------------
            if (IsSnapshot == false)
            {
                CurLeft = bounds.Left;
                CurTop = bounds.Top;
                CurWidth = bounds.Width - Constants.ITEM_RIGHT_PAD;
                CurHeight = lyr.Height;
                rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                if (lyr.Handle == m_SelectedLayerHandle && bounds.Width > 25)
                {
                    // selects the title only
                    if (m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                        rect.Height = Constants.ITEM_HEIGHT;
                    else
                        rect.Height = CurHeight;

                    if (CurTop + rect.Height > 0 || CurTop < this.ClientRectangle.Height)
                    {
                        DrawBox(DrawTool, rect, m_BoxLineColor, m_SelectedColor);
                    }
                }
            }
            else //采用snapshot绘制
            {
                CurLeft = bounds.Left;
                CurTop = bounds.Top;
                CurWidth = bounds.Width - 1;
                CurHeight = lyr.CalcHeight(true) - 1;
                rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                DrawBox(DrawTool, rect, m_BoxLineColor, System.Drawing.Color.White);
                // MessageBox.Show("IsSnapshot");
            }

            // -------------------------------------------------------
            //  绘制checkbox
            // -------------------------------------------------------
            if (bounds.Width > 55 && IsSnapshot == false)
            {
                CurTop = bounds.Top + Constants.CHECK_TOP_PAD;
                CurLeft = bounds.Left + Constants.CHECK_LEFT_PAD;

                bool visible = true;
                if (m_Map.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                {
                    visible = m_Map.get_LayerVisible(lyr.Handle);
                }
                else
                {
                    if (lyr.UseDynamicVisibility)
                        visible = (m_Map.CurrentScale >= lyr.MinVisibleScale) && (m_Map.CurrentScale <= lyr.MaxVisibleScale);
                    visible = visible && m_Map.get_LayerVisible(lyr.Handle);

                }

                DrawCheckBox(DrawTool, CurTop, CurLeft, visible, lyr.UseDynamicVisibility); // 如果layer是动态可见模式，则绘制一个灰色背景.
            }

            // ----------------------------------------------------------
            //    绘制文本
            // ----------------------------------------------------------
            SizeF textSize = new SizeF(0.0f, 0.0f);
            if (bounds.Width > 60)
            {
                string text = m_Map.get_LayerName(lyr.Handle);

                textSize = DrawTool.MeasureString(text, m_Font);

                if (IsSnapshot == true)
                    CurLeft = bounds.Left + Constants.CHECK_LEFT_PAD;
                else
                    CurLeft = bounds.Left + Constants.TEXT_LEFT_PAD;

                CurTop = bounds.Top + Constants.TEXT_TOP_PAD;
                //CurWidth = bounds.Width - CurLeft - Constants.TEXT_RIGHT_PAD;
                CurWidth = bounds.Width - Constants.TEXT_RIGHT_PAD - 27;
                CurHeight = Constants.TEXT_HEIGHT;

                rect = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
                DrawText(DrawTool, text, rect, m_Font, this.ForeColor);

                LayerElement el = new LayerElement(LayerElementType.Name, rect, text);
                lyr.Elements.Add(el);
            }

            // -------------------------------------------------------------
            //   绘制 layer icon
            // -------------------------------------------------------------
            if (m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                if (bounds.Width > 60 && bounds.Right - CurLeft - 41 > textSize.Width)  // -5 (offset)
                {
                    int top = bounds.Top + Constants.ICON_TOP_PAD;
                    int left = bounds.Right - 36;
                    System.Drawing.Image icon;

                    if (lyr.Icon != null)
                    {
                        this.DrawPicture(DrawTool, left, CurTop, Constants.ICON_SIZE, Constants.ICON_SIZE, lyr.Icon);
                    }
                    else if (lyr.Type == eLayerType.Image)
                    {
                        icon = Icons.Images[cImageIcon];
                        this.DrawPicture(DrawTool, left, top, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                    }
                    else if (lyr.Type == eLayerType.Grid)
                    {
                        icon = Icons.Images[cGridIcon];
                        this.DrawPicture(DrawTool, left, top, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                    }
                    else
                    {
                        // 当层处于折叠状态时，绘制shapefile符号的预览
                        if (!lyr.Expanded)
                        {
                            lyr.m_smallIconWasDrawn = true;
                            Rectangle iconBounds = new Rectangle(left, top, Constants.ICON_SIZE, Constants.ICON_SIZE);

                            // drawing category symbol
                            IntPtr hdc = DrawTool.GetHdc();
                            Color clr = (lyr.Handle == m_SelectedLayerHandle && bounds.Width > 25) ? m_SelectedColor : this.BackColor;
                            uint backColor = Convert.ToUInt32(ColorTranslator.ToOle(clr));

                            MapWinGIS.Shapefile sf = m_Map.get_GetObject(lyr.Handle) as MapWinGIS.Shapefile;

                            if (lyr.Type == eLayerType.PointShapefile)
                                sf.DefaultDrawingOptions.DrawPoint(hdc, left, top, Constants.ICON_SIZE, Constants.ICON_SIZE, backColor);
                            else if (lyr.Type == eLayerType.LineShapefile)
                                sf.DefaultDrawingOptions.DrawLine(hdc, left, top, Constants.ICON_SIZE - 1, Constants.ICON_SIZE - 1, false, Constants.ICON_SIZE, Constants.ICON_SIZE, backColor);
                            else if (lyr.Type == eLayerType.PolygonShapefile)
                                sf.DefaultDrawingOptions.DrawRectangle(hdc, left, top, Constants.ICON_SIZE - 1, Constants.ICON_SIZE - 1, false, Constants.ICON_SIZE, Constants.ICON_SIZE, backColor);

                            DrawTool.ReleaseHdc(hdc);
                        }
                    }
                }

                if (bounds.Width > 60 && bounds.Right - CurLeft - 62 > textSize.Width)
                {
                    MapWinGIS.Shapefile sf = m_Map.get_GetObject(lyr.Handle) as MapWinGIS.Shapefile;
                    if (sf != null)
                    {
                        int top = bounds.Top + Constants.ICON_TOP_PAD;
                        int left = bounds.Right - 56;

                        //Image icon = null;
                        double scale = m_Map.CurrentScale;
                        bool labelsVisible = sf.Labels.Count > 0 && sf.Labels.Visible && sf.Labels.Expression.Trim() != "";
                        labelsVisible &= scale >= sf.Labels.MinVisibleScale && scale <= sf.Labels.MaxVisibleScale;
                        System.Drawing.Image icon = labelsVisible ? Icons.Images[cActiveLabelIcon] : Icons.Images[cDimmedLabelIcon];
                        this.DrawPicture(DrawTool, left, top, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                    }
                }
            }
            else
            {
                if (bounds.Width > 60)
                {
                    //draw Layer Type symbol
                    if (lyr.Icon == null)
                    {
                        uint Fill = 0,
                            Line = 0;

                        if (lyr.Type == eLayerType.LineShapefile)
                            Fill = m_Map.get_ShapeLayerLineColor(lyr.Handle);
                        else if (lyr.Type == eLayerType.PointShapefile)
                            Fill = m_Map.get_ShapeLayerPointColor(lyr.Handle);
                        else if (lyr.Type == eLayerType.PolygonShapefile)
                        {
                            Fill = m_Map.get_ShapeLayerFillColor(lyr.Handle);
                            Line = m_Map.get_ShapeLayerLineColor(lyr.Handle);
                        }

                        CurTop = bounds.Top + Constants.ICON_TOP_PAD;
                        CurLeft = bounds.Right - Constants.ICON_RIGHT_PAD;

                        Color FillColor = Globals.UintToColor(Fill);
                        Color LineColor = Globals.UintToColor(Line);
                        DrawLayerSymbol(DrawTool, lyr.Type, CurTop, CurLeft, FillColor, LineColor, lyr.Handle);
                    }
                    else
                    {
                        CurTop = bounds.Top + Constants.ICON_TOP_PAD;
                        CurLeft = bounds.Right - Constants.ICON_RIGHT_PAD;
                        DrawPicture(DrawTool, CurLeft, CurTop, Constants.ICON_SIZE, Constants.ICON_SIZE, lyr.Icon);
                    }
                }
            }

            // -------------------------------------------------------------
            //   为shapefile绘制categories和expansion box
            // -------------------------------------------------------------
            if (m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology &&
                                                    (lyr.Type == eLayerType.PointShapefile ||
                                                    lyr.Type == eLayerType.LineShapefile ||
                                                    lyr.Type == eLayerType.PolygonShapefile))
            {
                if (bounds.Width > 17 && IsSnapshot == false)
                {
                    rect = new Rectangle(bounds.Left, bounds.Top, bounds.Width - Constants.ITEM_RIGHT_PAD, bounds.Height);
                    DrawExpansionBox(DrawTool, rect.Top + Constants.EXPAND_BOX_TOP_PAD, rect.Left + Constants.EXPAND_BOX_LEFT_PAD, lyr.Expanded);
                }

                // drawing shapefile
                DrawShapefileCategories(DrawTool, lyr, bounds, IsSnapshot);      // drawing of categories for the new symbology

                // drawing image
                //MapWinGIS.Image img = m_Map.get_GetObject(lyr.Handle) as MapWinGIS.Image;
            }
            else
            {
                // ----------------------------------------------------------
                //   绘制扩展盒及其子条目
                // ----------------------------------------------------------
                bool Handled = false;
                if (lyr.Expanded && lyr.ExpansionBoxCustomRenderFunction != null)
                    lyr.ExpansionBoxCustomRenderFunction(lyr.Handle, new Rectangle(bounds.Left + Constants.CHECK_LEFT_PAD, lyr.Top + Constants.ITEM_HEIGHT + Constants.EXPAND_BOX_TOP_PAD, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT - Constants.EXPAND_BOX_LEFT_PAD, bounds.Height - lyr.Top), ref DrawTool, ref Handled);

                // Here, draw the + or - sign according to based on  layer.expanded property
                if (lyr.ExpansionBoxForceAllowed || lyr.ColorLegend.Count > 0 ||
                   (lyr.HatchingScheme != null && lyr.HatchingScheme.NumHatches() > 0) ||
                   (lyr.PointImageScheme != null && lyr.PointImageScheme.NumberItems > 0))
                {
                    if (bounds.Width > 17 && IsSnapshot == false)
                    {
                        //SetRect(&LocalBounds, bounds.left + LIST_ITEM_INDENT,Top,bounds.right-ITEM_PAD,Top+lyr.Height);
                        rect = new Rectangle(bounds.Left, bounds.Top, bounds.Width - Constants.ITEM_RIGHT_PAD, bounds.Height);
                        DrawExpansionBox(DrawTool, rect.Top + Constants.EXPAND_BOX_TOP_PAD, rect.Left + Constants.EXPAND_BOX_LEFT_PAD, lyr.Expanded);
                    }
                }

                // -------------------------------------------------------------
                //   绘制一个旧样式的legend
                // -------------------------------------------------------------
                int p = 0;
                if (!Handled && (lyr.ColorLegend.Count > 0 || lyr.Icon != null || (lyr.PointImageScheme != null && lyr.PointImageScheme.NumberItems > 0)))
                {
                    if ((IsSnapshot == true || lyr.Expanded == true) && bounds.Width > 47)
                    {
                        //figure out if we can clip any of the breaks at the top
                        if (bounds.Top + Constants.ITEM_HEIGHT < this.ClientRectangle.Top && IsSnapshot == false)
                        {
                            int Difference = this.ClientRectangle.Top - (bounds.Top + Constants.ITEM_HEIGHT);
                            p = Difference / Constants.CS_ITEM_HEIGHT;
                        }

                        // Draw legends for point icon symbology
                        if (lyr.PointImageScheme != null && lyr.PointImageScheme.NumberItems > 0)
                        {
                            // Header Text
                            if (lyr.PointImageFieldCaption != "")
                            {
                                rect = new Rectangle(bounds.Left + 7, bounds.Top + Constants.ITEM_HEIGHT + p * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                                DrawText(DrawTool, lyr.PointImageFieldCaption, rect, m_Font, Color.Black);
                            }

                            rect = new Rectangle(Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT, rect.Width - Constants.CS_PATCH_LEFT_INDENT, lyr.CalcHeight(IsSnapshot));

                            ArrayList ItemNames = new ArrayList();
                            ArrayList ItemIndex = new ArrayList();
                            foreach (DictionaryEntry de in lyr.PointImageScheme.Items)
                            {
                                ItemNames.Add(de.Key);
                                ItemIndex.Add(de.Value);
                            }

                            for (; p < lyr.PointImageScheme.NumberItems; p++)
                            {
                                int offset = p + (lyr.PointImageFieldCaption != "" ? 1 : 0);
                                object img = (MapWinGIS.Image)m_Map.get_UDPointImageListItem(lyr.m_Handle, Convert.ToInt32(ItemIndex[p]));

                                DrawPicture(DrawTool, bounds.Left + Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, 14, 14, img);
                                rect = new Rectangle(bounds.Left + Constants.CS_TEXT_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                                DrawText(DrawTool, (string)ItemNames[p], rect, m_Font, Color.Black);

                                if (rect.Top >= this.ClientRectangle.Bottom && IsSnapshot == false)
                                    break;
                            }
                        }
                        else // If there is color legend but not point icon symbolgies.
                        {
                            // Header Text
                            if (lyr.ColorSchemeFieldCaption != "")
                            {
                                rect = new Rectangle(bounds.Left + 7, bounds.Top + Constants.ITEM_HEIGHT + p * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);

                                if (lyr.StippleSchemeFieldCaption != "")
                                {
                                    DrawText(DrawTool, "Fill: " + lyr.ColorSchemeFieldCaption, rect, m_Font, Color.Black);
                                }
                                else
                                {
                                    DrawText(DrawTool, lyr.ColorSchemeFieldCaption, rect, m_Font, Color.Black);
                                }
                            }
                            rect = new Rectangle(Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT, rect.Width - Constants.CS_PATCH_LEFT_INDENT, lyr.CalcHeight(IsSnapshot));

                            long NumBreaks = lyr.ColorLegend.Count;

                            for (; p < NumBreaks; p++)
                            {
                                int offset = p + (lyr.ColorSchemeFieldCaption != "" ? 1 : 0);
                                ColorInfo ci = (ColorInfo)lyr.ColorLegend[p];
                                if (ci.IsTransparent == false)
                                {
                                    if (lyr.Type == eLayerType.Grid || lyr.Type == eLayerType.Image)
                                    {
                                        DrawColorPatch(DrawTool, ci.StartColor, ci.EndColor, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Left + Constants.CS_PATCH_LEFT_INDENT, Constants.CS_PATCH_HEIGHT, Constants.CS_PATCH_WIDTH, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), true);
                                    }
                                    else
                                    {
                                        System.Drawing.Color sc = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.StartColor);
                                        System.Drawing.Color ec = System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), ci.EndColor);
                                        DrawColorPatch(DrawTool, sc, ec, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Left + Constants.CS_PATCH_LEFT_INDENT, Constants.CS_PATCH_HEIGHT, Constants.CS_PATCH_WIDTH, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), (m_Map.get_ShapeLayerLineWidth(lyr.Handle) != 0), lyr.Type);
                                    }
                                }
                                else
                                {
                                    DrawTransparentPatch(DrawTool, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Left + Constants.CS_PATCH_LEFT_INDENT, Constants.CS_PATCH_HEIGHT, Constants.CS_PATCH_WIDTH, System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))), (m_Map.get_ShapeLayerLineWidth(lyr.Handle) != 0));
                                }

                                rect = new Rectangle(bounds.Left + Constants.CS_TEXT_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                                DrawText(DrawTool, ci.Caption, rect, m_Font, Color.Black);

                                if (rect.Top >= this.ClientRectangle.Bottom && IsSnapshot == false)
                                    break;
                            }
                        }
                    }
                }

                if (!Handled && lyr.HatchingScheme != null && lyr.HatchingScheme.NumHatches() > 0)
                {
                    if ((IsSnapshot == true || lyr.Expanded == true) && bounds.Width > 47)
                    {
                        // Consider space for the caption above us
                        int offset = p + (lyr.ColorSchemeFieldCaption != "" ? 1 : 0);

                        // Header Text
                        if (lyr.StippleSchemeFieldCaption != "")
                        {
                            rect = new Rectangle(bounds.Left + 7, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);

                            if (lyr.ColorSchemeFieldCaption != "")
                            {
                                DrawText(DrawTool, "Hatch: " + lyr.StippleSchemeFieldCaption, rect, m_Font, Color.Black);
                            }
                            else
                            {
                                DrawText(DrawTool, lyr.StippleSchemeFieldCaption, rect, m_Font, Color.Black);
                            }
                        }

                        rect = new Rectangle(Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT, rect.Width - Constants.CS_PATCH_LEFT_INDENT, lyr.CalcHeight(IsSnapshot));
                        long NumBreaks = lyr.HatchingScheme.NumHatches();

                        IDictionaryEnumerator i = (IDictionaryEnumerator)lyr.HatchingScheme.GetHatchesEnumerator();
                        while (i.MoveNext())
                        {
                            // Consider our caption plus the one above us
                            offset = p + (lyr.ColorSchemeFieldCaption != "" ? 1 : 0) + (lyr.StippleSchemeFieldCaption != "" ? 1 : 0);

                            Rectangle fillRect = new Rectangle(bounds.Left + Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, Constants.CS_PATCH_WIDTH, Constants.CS_PATCH_HEIGHT);
                            Pen pen = new Pen(System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(m_Map.get_ShapeLayerLineColor(lyr.Handle))));

                            //fill the rectangle with a gradient fill
                            ShapefileFillStippleBreak b = (ShapefileFillStippleBreak)i.Value;
                            System.Drawing.Brush brush;

                            // If brush color is close to white, it is invisible in legend
                            // because the background color of the box is transparent (thus white).
                            // Do something to prevent it.
                            if (GetHatchStyle(b.Hatch) == System.Drawing.Drawing2D.HatchStyle.Max)
                            {
                                brush = new SolidBrush(Color.Transparent);
                            }
                            else
                            {
                                if (b.LineColor.GetBrightness() > 0.9)
                                {
                                    brush = new System.Drawing.Drawing2D.HatchBrush(GetHatchStyle(b.Hatch), b.LineColor, Color.LightGray);
                                }
                                else
                                {
                                    brush = new System.Drawing.Drawing2D.HatchBrush(GetHatchStyle(b.Hatch), b.LineColor, Color.Transparent);
                                }
                            }

                            DrawTool.FillRectangle(brush, fillRect);
                            DrawTool.DrawRectangle(pen, bounds.Left + Constants.CS_PATCH_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, Constants.CS_PATCH_WIDTH, Constants.CS_PATCH_HEIGHT);

                            rect = new Rectangle(bounds.Left + Constants.CS_TEXT_LEFT_INDENT, bounds.Top + Constants.ITEM_HEIGHT + offset * Constants.CS_ITEM_HEIGHT, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                            DrawText(DrawTool, b.Value, rect, m_Font, Color.Black);

                            p++;

                            if (rect.Top >= this.ClientRectangle.Bottom && IsSnapshot == false)
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制shapefile类型的layer的categories
        /// </summary>
        private void DrawShapefileCategories(Graphics DrawTool, Layer layer, Rectangle bounds, bool IsSnapshot)
        {
            if (layer.Type != eLayerType.PointShapefile &&
                layer.Type != eLayerType.LineShapefile &&
                layer.Type != eLayerType.PolygonShapefile) return;

            if ((/*!IsSnapshot &&*/ !layer.Expanded) || bounds.Width <= 47)
                return;

            MapWinGIS.Shapefile sf = m_Map.get_GetObject(layer.Handle) as MapWinGIS.Shapefile;

            if (sf == null)
                return;

            int maxWidth = Constants.ICON_WIDTH; ;
            if (layer.Type == eLayerType.PointShapefile)
                maxWidth = layer.Get_MaxIconWidth(sf);

            int top = bounds.Top + Constants.ITEM_HEIGHT + 2;

            // temp
            int val1;
            int height;
            val1 = (layer.Get_CategoryHeight(sf.DefaultDrawingOptions) + 2);  // default symbology

            height = val1;

            if (top + height > this.ClientRectangle.Top)
            {
                DrawShapefileCategory(DrawTool, sf.DefaultDrawingOptions, layer, bounds, top, "", maxWidth, -1);
            }

            top += height;
            // end temp

            Rectangle rect = new Rectangle();
            if (sf.Categories.Count > 0)
            {
                // categories caption
                string caption = sf.Categories.Caption;
                if (caption == string.Empty)
                    caption = "Categories";
                int left = bounds.Left + Constants.TEXT_LEFT_PAD;
                if (!(top + Constants.TEXT_HEIGHT < 0))
                {
                    rect = new Rectangle(left, top, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                    DrawText(DrawTool, caption, rect, m_Font, this.ForeColor);
                }
                top += Constants.CS_ITEM_HEIGHT + 2;

                //figure out if we can clip any of the categories at the top
                int i = 0;
                MapWinGIS.ShapefileCategories categories = sf.Categories;
                int numCategories = sf.Categories.Count;
                if (top < this.ClientRectangle.Top && IsSnapshot == false)
                {
                    while (i < numCategories)
                    {
                        // for point categories height can be different
                        top += layer.Get_CategoryHeight(categories.get_Item(i).DrawingOptions);

                        if (top < this.ClientRectangle.Top)
                        {
                            i++;
                        }
                        else
                        {
                            top -= layer.Get_CategoryHeight(categories.get_Item(i).DrawingOptions);  // this category should be drawn
                            break;
                        }
                    }
                }

                // we shall draw symbology first and text second
                // symbology is drawn from ocx, so it's better to draw all categories at once
                // avoiding additional GetHDC calls
                IntPtr hdc = DrawTool.GetHdc();
                int topTemp = top;
                int startIndex = i;
                for (; i < categories.Count; i++)
                {
                    MapWinGIS.ShapefileCategory cat = categories.get_Item(i);
                    MapWinGIS.ShapeDrawingOptions options = cat.DrawingOptions;

                    DrawShapefileCategorySymbology(DrawTool, options, layer, bounds, topTemp, maxWidth, i, hdc);

                    topTemp += layer.Get_CategoryHeight(options);
                    if (topTemp >= this.ClientRectangle.Bottom && IsSnapshot == false)     // stop drawing in case there are not visible
                        break;
                }
                DrawTool.ReleaseHdc(hdc);

                // now when hdc is released, GDI+ can be used for the text
                i = startIndex;
                for (; i < categories.Count; i++)
                {
                    MapWinGIS.ShapefileCategory cat = categories.get_Item(i);
                    MapWinGIS.ShapeDrawingOptions options = cat.DrawingOptions;

                    DrawShapefileCategoryText(DrawTool, options, layer, bounds, top, cat.Name, maxWidth, i);

                    top += layer.Get_CategoryHeight(options);
                    if (top >= this.ClientRectangle.Bottom && IsSnapshot == false)     // stop drawing in case there are not visible
                        break;
                }
            }

            // charts
            if (sf.Charts.Count > 0 && sf.Charts.NumFields > 0 && sf.Charts.Visible)
            {
                // charts caption
                string caption = sf.Charts.Caption;
                if (caption == string.Empty)
                    caption = "Charts";

                int left = bounds.Left + Constants.TEXT_LEFT_PAD;
                rect = new Rectangle(left, top, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                DrawText(DrawTool, caption, rect, m_Font, this.ForeColor);
                top += Constants.CS_ITEM_HEIGHT + 2;

                // storing bounds
                LayerElement el = new LayerElement(LayerElementType.Charts, rect);
                layer.Elements.Add(el);

                // preview
                IntPtr hdc = DrawTool.GetHdc();
                uint backColor = Convert.ToUInt32(ColorTranslator.ToOle(this.BackColor));

                left = bounds.Left + Constants.TEXT_LEFT_PAD;
                sf.Charts.DrawChart(hdc, left, top, true, backColor);
                top += sf.Charts.IconHeight + 2;
                DrawTool.ReleaseHdc(hdc);

                // storing bounds
                el = new LayerElement(LayerElementType.ChartField, rect);
                layer.Elements.Add(el);

                // fields
                Color color = ColorTranslator.FromOle(Convert.ToInt32(sf.Charts.LineColor));
                Pen pen = new Pen(color);

                for (int i = 0; i < sf.Charts.NumFields; i++)
                {
                    rect = new Rectangle(left, top, Constants.ICON_WIDTH, Constants.ICON_HEIGHT);
                    color = ColorTranslator.FromOle(Convert.ToInt32(sf.Charts.get_Field(i).Color));
                    SolidBrush brush = new SolidBrush(color);
                    DrawTool.FillRectangle(brush, rect);
                    DrawTool.DrawRectangle(pen, rect);

                    // storing bounds
                    el = new LayerElement(LayerElementType.ChartField, rect, i);
                    layer.Elements.Add(el);

                    rect = new Rectangle(left + Constants.ICON_WIDTH + 5, top, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
                    string name = sf.Charts.get_Field(i).Name;
                    DrawText(DrawTool, name, rect, m_Font, Color.Black);

                    // storing bounds
                    el = new LayerElement(LayerElementType.ChartFieldName, rect, name, i);
                    layer.Elements.Add(el);

                    top += (Constants.CS_ITEM_HEIGHT + 2);
                }
            }
        }

        /// <summary>
        /// 在指定位置绘制shapefile category
        /// </summary>
        /// <param name="options">用于绘制的选项</param>
        /// <param name="type">Shapefile的类型</param>
        private void DrawShapefileCategory(Graphics DrawTool, MapWinGIS.ShapeDrawingOptions options, LegendControl.Layer layer,
                                           Rectangle bounds, int top, string name, int maxWidth, int index)
        {
            int categoryHeight = layer.Get_CategoryHeight(options);
            int categoryWidth = layer.Get_CategoryWidth(options);

            // drawing category symbol
            IntPtr hdc = DrawTool.GetHdc();
            uint backColor = Convert.ToUInt32(ColorTranslator.ToOle(this.BackColor));

            int left = bounds.Left + Constants.TEXT_LEFT_PAD;
            if (categoryWidth != Constants.ICON_WIDTH)
            {
                left -= ((categoryWidth - Constants.ICON_WIDTH) / 2);
            }

            if (layer.Type == eLayerType.PointShapefile)
                options.DrawPoint(hdc, left, top, categoryWidth + 1, categoryHeight + 1, backColor);
            else if (layer.Type == eLayerType.LineShapefile)
                options.DrawLine(hdc, left, top, categoryWidth - 1, Constants.ICON_HEIGHT - 1, false, categoryWidth, categoryHeight, backColor);
            else if (layer.Type == eLayerType.PolygonShapefile)
                options.DrawRectangle(hdc, left, top, categoryWidth - 1, Constants.ICON_HEIGHT - 1, false, categoryWidth, categoryHeight, backColor);

            DrawTool.ReleaseHdc(hdc);

            if (categoryHeight > Constants.CS_ITEM_HEIGHT)
                top += (categoryHeight - Constants.CS_ITEM_HEIGHT) / 2;

            // drawing category name
            left = (bounds.Left + Constants.TEXT_LEFT_PAD + Constants.ICON_WIDTH / 2 + maxWidth / 2 + 5);

            Rectangle rect = new Rectangle(left, top, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
            DrawText(DrawTool, name, rect, m_Font, Color.Black);
        }

        /// <summary>
        /// 绘制shapefile category. It's asumed here that GetHDC and ReleaseHDC calls are made by caller
        /// </summary>
        private void DrawShapefileCategorySymbology(Graphics DrawTool, MapWinGIS.ShapeDrawingOptions options, LegendControl.Layer layer,
                                           Rectangle bounds, int top, int maxWidth, int index, IntPtr hdc)
        {
            int categoryHeight = layer.Get_CategoryHeight(options);
            int categoryWidth = layer.Get_CategoryWidth(options);

            uint backColor = Convert.ToUInt32(ColorTranslator.ToOle(this.BackColor));

            int left = bounds.Left + Constants.TEXT_LEFT_PAD;
            if (categoryWidth != Constants.ICON_WIDTH)
            {
                left -= ((categoryWidth - Constants.ICON_WIDTH) / 2);
            }

            if (layer.Type == eLayerType.PointShapefile)
                options.DrawPoint(hdc, left, top, categoryWidth + 1, categoryHeight + 1, backColor);
            else if (layer.Type == eLayerType.LineShapefile)
                options.DrawLine(hdc, left, top, categoryWidth - 1, Constants.ICON_HEIGHT - 1, false, categoryWidth, categoryHeight, backColor);
            else if (layer.Type == eLayerType.PolygonShapefile)
                options.DrawRectangle(hdc, left, top, categoryWidth - 1, Constants.ICON_HEIGHT - 1, false, categoryWidth, categoryHeight, backColor);

            if (categoryHeight > Constants.CS_ITEM_HEIGHT)
                top += (categoryHeight - Constants.CS_ITEM_HEIGHT) / 2;
        }

        /// <summary>
        /// 绘制shapefile category的文本
        /// </summary>
        private void DrawShapefileCategoryText(Graphics DrawTool, MapWinGIS.ShapeDrawingOptions options, LegendControl.Layer layer,
                                           Rectangle bounds, int top, string name, int maxWidth, int index)
        {
            int categoryHeight = layer.Get_CategoryHeight(options);
            if (categoryHeight > Constants.CS_ITEM_HEIGHT)
                top += (categoryHeight - Constants.CS_ITEM_HEIGHT) / 2;

            // drawing category name
            int left = (bounds.Left + Constants.TEXT_LEFT_PAD + Constants.ICON_WIDTH / 2 + maxWidth / 2 + 5);

            Rectangle rect = new Rectangle(left, top, bounds.Width - Constants.TEXT_RIGHT_PAD_NO_ICON - Constants.CS_TEXT_LEFT_INDENT, Constants.TEXT_HEIGHT);
            DrawText(DrawTool, name, rect, m_Font, this.ForeColor);
        }

        private System.Drawing.Drawing2D.HatchStyle GetHatchStyle(MapWinGIS.tkFillStipple stip)
        {
            switch (stip)
            {
                case MapWinGIS.tkFillStipple.fsDiagonalDownLeft:
                    return System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal;
                case MapWinGIS.tkFillStipple.fsDiagonalDownRight:
                    return System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal;
                case MapWinGIS.tkFillStipple.fsHorizontalBars:
                    return System.Drawing.Drawing2D.HatchStyle.DarkHorizontal;
                case MapWinGIS.tkFillStipple.fsPolkaDot:
                    return System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond;
                case MapWinGIS.tkFillStipple.fsVerticalBars:
                    return System.Drawing.Drawing2D.HatchStyle.DarkVertical;
                default:
                    return System.Drawing.Drawing2D.HatchStyle.Max;
            }
        }

        /// <summary>
        /// 为新的shapefile位置icon
        /// </summary>
        private void DrawLayerSymbolNew(Graphics DrawTool, Layer layer, int TopPos, int LeftPos)
        {
            System.Drawing.Drawing2D.SmoothingMode OldSmoothingMode;
            OldSmoothingMode = DrawTool.SmoothingMode;

            try
            {
                DrawTool.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Image icon;

                switch (layer.Type)
                {
                    case eLayerType.Grid:
                        icon = Icons.Images[cGridIcon];
                        DrawPicture(DrawTool, LeftPos, TopPos, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                        break;
                    case eLayerType.Image:
                        icon = Icons.Images[cImageIcon];
                        DrawPicture(DrawTool, LeftPos, TopPos, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                        break;
                    default:
                        MapWinGIS.Shapefile sf = m_Map.get_GetObject(layer.Handle) as MapWinGIS.Shapefile;
                        if (sf == null)
                        {
                            MessageBox.Show("错误: shapefile not set");
                            return;
                        }

                        IntPtr hdc = DrawTool.GetHdc();

                        uint backColor = Convert.ToUInt32(ColorTranslator.ToOle(this.BackColor));

                        if (layer.Type == eLayerType.PointShapefile)
                            sf.DefaultDrawingOptions.DrawPoint(hdc, LeftPos, TopPos, Constants.ICON_WIDTH, Constants.ICON_HEIGHT, backColor);
                        else if (layer.Type == eLayerType.LineShapefile)
                            sf.DefaultDrawingOptions.DrawLine(hdc, LeftPos, TopPos, Constants.ICON_WIDTH - 1, Constants.ICON_SIZE - 1, false, Constants.ICON_WIDTH, Constants.ICON_HEIGHT, backColor);
                        else if (layer.Type == eLayerType.PolygonShapefile)
                            sf.DefaultDrawingOptions.DrawRectangle(hdc, LeftPos, TopPos, Constants.ICON_WIDTH - 1, Constants.ICON_SIZE - 1, false, Constants.ICON_WIDTH, Constants.ICON_HEIGHT, backColor);

                        DrawTool.ReleaseHdc(hdc);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                string temp = ex.ToString();
            }

            DrawTool.SmoothingMode = OldSmoothingMode;
        }

        /// <summary>
        /// 为旧的shapefile绘制icon
        /// </summary>
        private void DrawLayerSymbol(Graphics DrawTool, eLayerType LayerType, int TopPos, int LeftPos, Color FillColor, Color OutlineColor, int LayerHandle)
        {
            System.Drawing.Drawing2D.SmoothingMode OldSmoothingMode;
            OldSmoothingMode = DrawTool.SmoothingMode;

            try
            {
                DrawTool.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Image icon;
                Pen pen;

                switch (LayerType)
                {
                    case eLayerType.Grid:
                        icon = Icons.Images[cGridIcon];
                        DrawPicture(DrawTool, LeftPos, TopPos, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                        break;
                    case eLayerType.Image:
                        icon = Icons.Images[cImageIcon];
                        DrawPicture(DrawTool, LeftPos, TopPos, Constants.ICON_SIZE, Constants.ICON_SIZE, icon);
                        break;
                    case eLayerType.LineShapefile:
                        pen = new Pen(FillColor, 2);

                        OldSmoothingMode = DrawTool.SmoothingMode;
                        DrawTool.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        DrawTool.DrawLine(pen, LeftPos, TopPos + 8, LeftPos + 4, TopPos + 3);
                        DrawTool.DrawLine(pen, LeftPos + 4, TopPos + 3, LeftPos + 9, TopPos + 10);
                        DrawTool.DrawLine(pen, LeftPos + 9, TopPos + 10, LeftPos + 13, TopPos + 4);

                        DrawTool.SmoothingMode = OldSmoothingMode;

                        break;
                    case eLayerType.PointShapefile:
                        pen = new Pen(FillColor);

                        float offset;
                        MapWinGIS.tkPointType pntType;
                        pntType = m_Map.get_ShapeLayerPointType(LayerHandle);
                        System.Drawing.Point[] pnts;

                        switch (pntType)
                        {
                            case MapWinGIS.tkPointType.ptCircle:
                                offset = (float)(.2 * Constants.ICON_SIZE);
                                DrawTool.FillEllipse(pen.Brush, (float)LeftPos + offset, TopPos + offset, 7, 7);
                                break;
                            case MapWinGIS.tkPointType.ptDiamond:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[4];

                                //top
                                pnts[0] = new System.Drawing.Point((int)(LeftPos + offset), TopPos + 2);
                                //left
                                pnts[1] = new System.Drawing.Point(LeftPos + 2, (int)(TopPos + offset));
                                //bottom
                                pnts[2] = new System.Drawing.Point((int)(LeftPos + offset), TopPos + Constants.ICON_SIZE - 3);
                                //right
                                pnts[3] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, (int)(TopPos + offset));

                                DrawTool.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptSquare:
                                offset = (float)(.25 * Constants.ICON_SIZE);
                                DrawTool.FillRectangle(pen.Brush, (int)(LeftPos + offset), (int)(TopPos + offset), 6, 6);
                                break;
                            case MapWinGIS.tkPointType.ptTriangleDown:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[3];

                                //bottom middle
                                pnts[0] = new System.Drawing.Point((int)(LeftPos + offset), TopPos + Constants.ICON_SIZE - 3);
                                //top right
                                pnts[1] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, TopPos + 3);
                                //top left
                                pnts[2] = new System.Drawing.Point((int)(LeftPos + 3), TopPos + 3);

                                DrawTool.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleLeft:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[3];

                                //left middle
                                pnts[0] = new System.Drawing.Point(LeftPos + 3, (int)(TopPos + offset));
                                //top right
                                pnts[1] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, TopPos + 3);
                                //bottom right
                                pnts[2] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, TopPos + Constants.ICON_SIZE - 3);

                                DrawTool.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleRight:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[3];

                                //right middle
                                pnts[0] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, (int)(TopPos + offset));
                                //top left
                                pnts[1] = new System.Drawing.Point(LeftPos + 3, TopPos + 3);
                                //bottom left
                                pnts[2] = new System.Drawing.Point(LeftPos + 3, TopPos + Constants.ICON_SIZE - 3);

                                DrawTool.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleUp:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[3];

                                //top middle
                                pnts[0] = new System.Drawing.Point((int)(LeftPos + offset), TopPos + 3);
                                //bottom right
                                pnts[1] = new System.Drawing.Point(LeftPos + Constants.ICON_SIZE - 3, TopPos + Constants.ICON_SIZE - 3);
                                //bottom left
                                pnts[2] = new System.Drawing.Point((int)(LeftPos + 3), TopPos + Constants.ICON_SIZE - 3);

                                DrawTool.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptUserDefined:
                                object img = m_Map.get_UDPointType(LayerHandle);
                                DrawPicture(DrawTool, LeftPos, TopPos, Constants.ICON_SIZE, Constants.ICON_SIZE, img);
                                img = null;
                                break;
                        }
                        break;
                    case eLayerType.PolygonShapefile:
                        bool DrawFill = m_Map.get_ShapeLayerDrawFill(LayerHandle);
                        MapWinGIS.tkFillStipple fillStyle = m_Map.get_ShapeLayerFillStipple(LayerHandle);

                        Pen FillPen;
                        Pen OutlinePen = new Pen(OutlineColor, 2);

                        Rectangle rect = new Rectangle(LeftPos + 1, TopPos + 1, Constants.ICON_SIZE - 2, Constants.ICON_SIZE - 2);

                        if (DrawFill == true)
                        {
                            switch (fillStyle)
                            {
                                case MapWinGIS.tkFillStipple.fsNone:
                                    // Chris Michaelis 7/5/2007 - Changed this coloring to take into account transparency "paleness"
                                    FillPen = new Pen(System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(LayerHandle) * 255), FillColor));
                                    break;
                                case MapWinGIS.tkFillStipple.fsDiagonalDownLeft:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsDiagonalDownRight:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsHorizontalBars:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightHorizontal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsPolkaDot:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsVerticalBars:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightVertical, Color.Black, Color.White));
                                    break;
                                default:
                                    // Chris Michaelis 7/5/2007 - Changed this coloring to take into account transparency "paleness"
                                    FillPen = new Pen(System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(LayerHandle) * 255), FillColor));
                                    break;
                            }

                            DrawTool.FillRectangle(FillPen.Brush, rect);
                        }

                        if (m_Map.get_ShapeLayerLineWidth(LayerHandle) > 0) DrawTool.DrawRectangle(OutlinePen, rect);

                        break;
                }
            }
            catch (System.Exception ex)
            {
                string temp = ex.ToString();
            }

            DrawTool.SmoothingMode = OldSmoothingMode;
        }

        private void DrawLayerSymbolHQ(Graphics g, Layer lyr, int Left, int Top, int Dimension, Color FillColor, Color OutlineColor)
        {
            System.Drawing.Drawing2D.SmoothingMode OldSmoothingMode;
            OldSmoothingMode = g.SmoothingMode;

            try
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Image icon;
                Pen pen;

                switch (lyr.Type)
                {
                    case eLayerType.Grid:
                        icon = Icons.Images[cGridIcon];
                        DrawPicture(g, Left, Top, Dimension, Dimension, icon);
                        break;
                    case eLayerType.Image:
                        icon = Icons.Images[cImageIcon];
                        DrawPicture(g, Left, Top, Dimension, Dimension, icon);
                        break;
                    case eLayerType.LineShapefile:
                        pen = new Pen(FillColor, 2);

                        OldSmoothingMode = g.SmoothingMode;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        g.DrawLine(pen, Left, Top + (Dimension / 2), Left + (Dimension / 3), Top);
                        g.DrawLine(pen, Left + (Dimension / 3), Top, (int)(Left + (Dimension * (2.0 / 3.0))), Top + Dimension);
                        g.DrawLine(pen, (int)(Left + (Dimension * (2.0 / 3.0))), Top + Dimension, Left + Dimension, Top + (Dimension / 2));

                        g.SmoothingMode = OldSmoothingMode;

                        break;
                    case eLayerType.PointShapefile:
                        pen = new Pen(FillColor);

                        float offset;
                        MapWinGIS.tkPointType pntType;
                        pntType = m_Map.get_ShapeLayerPointType(lyr.Handle);
                        System.Drawing.Point[] pnts;

                        switch (pntType)
                        {
                            case MapWinGIS.tkPointType.ptCircle:
                                g.FillEllipse(pen.Brush, Left, Top, Dimension, Dimension);
                                break;

                            case MapWinGIS.tkPointType.ptDiamond:
                                pnts = new System.Drawing.Point[4];

                                //top
                                pnts[0] = new System.Drawing.Point(Left, Top + (Dimension / 2));
                                //left
                                pnts[1] = new System.Drawing.Point(Left + (Dimension / 2), Top + Dimension);
                                //bottom
                                pnts[2] = new System.Drawing.Point(Left + Dimension, Top + (Dimension / 2));
                                //right
                                pnts[3] = new System.Drawing.Point(Left + (Dimension / 2), Top);

                                g.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptSquare:
                                g.FillRectangle(pen.Brush, Left, Top, Left + Dimension, Top + Dimension);
                                break;
                            case MapWinGIS.tkPointType.ptTriangleDown:
                                offset = (float)(.5 * Constants.ICON_SIZE);
                                pnts = new System.Drawing.Point[3];

                                pnts[0] = new System.Drawing.Point(Left, Top);
                                pnts[1] = new System.Drawing.Point(Left + (Dimension / 2), Top + Dimension);
                                pnts[2] = new System.Drawing.Point(Left + Dimension, Top);

                                g.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleLeft:
                                pnts = new System.Drawing.Point[3];

                                pnts[0] = new System.Drawing.Point(Left + Dimension, Top);
                                pnts[1] = new System.Drawing.Point(Left, Top + (Dimension / 2));
                                pnts[2] = new System.Drawing.Point(Left + Dimension, Top + Dimension);

                                g.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleRight:
                                pnts = new System.Drawing.Point[3];

                                pnts[0] = new System.Drawing.Point(Left, Top);
                                pnts[1] = new System.Drawing.Point(Left + Dimension, Top + (Dimension / 2));
                                pnts[2] = new System.Drawing.Point(Left, Top + Dimension);

                                g.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptTriangleUp:
                                pnts = new System.Drawing.Point[3];

                                pnts[0] = new System.Drawing.Point(Left, Top + Dimension);
                                pnts[1] = new System.Drawing.Point(Left + (Dimension / 2), Top);
                                pnts[2] = new System.Drawing.Point(Left + Dimension, Top + Dimension);

                                g.FillPolygon(pen.Brush, pnts);
                                pnts = null;
                                break;
                            case MapWinGIS.tkPointType.ptUserDefined:
                                object img = m_Map.get_UDPointType(lyr.Handle);
                                DrawPicture(g, Left, Top, Dimension, Dimension, img);
                                img = null;
                                break;
                        }
                        break;
                    case eLayerType.PolygonShapefile:
                        bool DrawFill = m_Map.get_ShapeLayerDrawFill(lyr.Handle);
                        MapWinGIS.tkFillStipple fillStyle = m_Map.get_ShapeLayerFillStipple(lyr.Handle);

                        Pen FillPen;
                        Pen OutlinePen = new Pen(OutlineColor, 2);
                        Rectangle rect = new Rectangle(Left, Top, Dimension, Dimension);

                        if (DrawFill == true)
                        {
                            switch (fillStyle)
                            {
                                case MapWinGIS.tkFillStipple.fsNone:
                                    FillPen = new Pen(System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), FillColor));
                                    break;
                                case MapWinGIS.tkFillStipple.fsDiagonalDownLeft:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsDiagonalDownRight:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsHorizontalBars:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightHorizontal, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsPolkaDot:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard, Color.Black, Color.White));
                                    break;
                                case MapWinGIS.tkFillStipple.fsVerticalBars:
                                    FillPen = new Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightVertical, Color.Black, Color.White));
                                    break;
                                default:
                                    // Chris Michaelis 7/5/2007 - Changed this coloring to take into account transparency "paleness"
                                    FillPen = new Pen(System.Drawing.Color.FromArgb((int)(m_Map.get_ShapeLayerFillTransparency(lyr.Handle) * 255), FillColor));
                                    break;
                            }

                            g.FillRectangle(FillPen.Brush, rect);
                        }

                        if (m_Map.get_ShapeLayerLineWidth(lyr.Handle) > 0) g.DrawRectangle(OutlinePen, rect);

                        break;
                }
            }
            catch (System.Exception ex)
            {
                string temp = ex.ToString();
            }

            g.SmoothingMode = OldSmoothingMode;
        }

        /// <summary>
        /// 重置控件的大小
        /// </summary>
        protected override void OnResize(System.EventArgs e)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                m_BackBuffer = new Bitmap(this.Width, this.Height);
                m_Draw = Graphics.FromImage(m_BackBuffer);
                vScrollBar.Top = 0;
                vScrollBar.Height = this.Height;
                vScrollBar.Left = this.Width - vScrollBar.Width;
            }
            this.Invalidate();
        }

        /// <summary>
        /// 根据legend（0,0）上的坐标查找组，以顶部（top）和左边（left）为基准
        /// </summary>
        /// <param name="point">在legend中点击的坐标</param>
        /// <param name="InCheckbox">指示组是否可见，即的checkbox是否点击</param>
        /// <param name="InExpandbox">指示扩展盒是否点击</param>
        /// <returns>返回被点击的组，若返回null则表示无group</returns>
        public Group FindClickedGroup(System.Drawing.Point point, out bool InCheckbox, out bool InExpandbox)
        {
            //finds the group that was clicked, i.e. heading of group, not subitems
            InExpandbox = false;
            InCheckbox = false;

            int GroupCount = m_AllGroups.Count;
            Group grp = null;

            int CurLeft = 0,
                CurTop = 0,
                CurWidth = 0,
                CurHeight = 0;
            Rectangle bounds;

            GroupCount = m_AllGroups.Count;

            for (int i = 0; i <= GroupCount; i++)
            {
                grp = (i == GroupCount) ? this.tileGroup : (Group)m_AllGroups[i];
                if (grp == null)
                    continue;

                //set group header bounds
                CurLeft = Constants.GRP_INDENT;
                CurWidth = this.Width - Constants.GRP_INDENT - Constants.ITEM_RIGHT_PAD;
                CurTop = grp.Top;
                CurHeight = Constants.ITEM_HEIGHT;
                bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                if (bounds.Contains(point) == true)
                {
                    //we are in the group heading
                    //now check to see if the click was in the expansion box
                    //+- a little to make the hot spot a little more precise
                    CurLeft = Constants.GRP_INDENT + Constants.EXPAND_BOX_LEFT_PAD + 1;
                    CurWidth = Constants.EXPAND_BOX_SIZE - 1;
                    CurTop = grp.Top + Constants.EXPAND_BOX_TOP_PAD + 1;
                    CurHeight = Constants.EXPAND_BOX_SIZE - 1;
                    bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                    if (bounds.Contains(point) == true)
                    {
                        //we are in the bounds for the expansion box
                        //but only if there is an expansion box visible
                        if ((int)(grp.m_Layers.Count) > 0)
                            InExpandbox = true;
                    }
                    else
                    {
                        //now check to see if in the check box
                        CurLeft = Constants.GRP_INDENT + Constants.CHECK_LEFT_PAD + 1;
                        CurWidth = Constants.CHECK_BOX_SIZE - 1;
                        CurTop = grp.Top + Constants.CHECK_TOP_PAD + 1;
                        CurHeight = Constants.CHECK_BOX_SIZE - 1;
                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
                        if (bounds.Contains(point) == true)
                            InCheckbox = true;
                    }
                    return grp;
                }
            }
            return null;    //if we get to this point, no group item found
        }

        /// <summary>
        /// 根据坐标查找层，以顶部（top）和左边（left）为基准
        /// </summary>
        /// <param name="point">在legend中点击的坐标</param>
        /// <param name="InCheckBox">指示层是否可见，即的checkbox是否点击</param>
        /// <param name="InExpansionBox">指示扩展盒是否点击</param>
        public Layer FindClickedLayer(System.Drawing.Point point, out bool InCheckBox, out bool InExpansionBox)
        {
            ClickedElement element = new ClickedElement();
            Layer lyr = FindClickedLayer(point, ref element);
            InCheckBox = element.CheckBox;
            InExpansionBox = element.ExpansionBox;
            return lyr;
        }

        /// <summary>
        /// 根据坐标查找层，以顶部（top）和左边（left）为基准
        /// </summary>
        /// <param name="element">在legend上点击的元素</param>
        /// <returns></returns>
        public Layer FindClickedLayer(System.Drawing.Point point, ref ClickedElement Element)
        {
            int GroupCount = m_AllGroups.Count;
            int LayerCount;

            Element.Nullify();

            Layer lyr = null;
            Group grp = null;

            int CurLeft, CurTop, CurWidth, CurHeight;
            CurLeft = CurTop = CurWidth = CurHeight = 0;
            Rectangle bounds;

            for (int i = 0; i <= GroupCount; i++)
            {
                grp = i == GroupCount ? this.tileGroup : (Group)m_AllGroups[i];
                if (grp == null || grp.Expanded == false)
                    continue;

                LayerCount = grp.m_Layers.Count;

                for (int j = 0; j < LayerCount; j++)
                {
                    lyr = (Layer)grp.m_Layers[j];

                    //see if we are inside the current Layer
                    CurLeft = Constants.LIST_ITEM_INDENT;
                    CurTop = lyr.Top;
                    CurWidth = this.Width - CurLeft - Constants.ITEM_RIGHT_PAD;
                    CurHeight = lyr.Height;
                    bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                    if (bounds.Contains(point))
                    {
                        //we are inside the Layer boundaries,
                        //but we need to narrow down the search
                        Element.GroupIndex = i;

                        //check to see if in the check box
                        CurLeft = Constants.LIST_ITEM_INDENT + Constants.CHECK_LEFT_PAD + 1;
                        CurTop = lyr.Top + Constants.CHECK_TOP_PAD + 1;
                        CurWidth = Constants.CHECK_BOX_SIZE - 1;
                        CurHeight = Constants.CHECK_BOX_SIZE - 1;
                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                        if (bounds.Contains(point))
                        {
                            //we are in the check box
                            Element.CheckBox = true;
                            return lyr;
                        }
                        else
                        {
                            //check to see if we are in the expansion box for this item
                            CurLeft = Constants.LIST_ITEM_INDENT + Constants.EXPAND_BOX_LEFT_PAD + 1;
                            CurTop = lyr.Top + Constants.EXPAND_BOX_TOP_PAD + 1;
                            CurWidth = Constants.EXPAND_BOX_SIZE;
                            CurHeight = Constants.EXPAND_BOX_SIZE;
                            bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                            if (m_Map.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology ||
                               (lyr.Type == eLayerType.Image || lyr.Type == eLayerType.Grid || lyr.Type == eLayerType.Tiles))
                            {
                                if (bounds.Contains(point) == true && (lyr.ColorLegend.Count > 0 || lyr.ExpansionBoxForceAllowed ||
                                                                       (lyr.HatchingScheme != null && lyr.HatchingScheme.NumHatches() > 0) ||
                                                                       (lyr.PointImageScheme != null && lyr.PointImageScheme.NumberItems > 0)))
                                {
                                    //We are in the Expansion box
                                    Element.ExpansionBox = true;
                                    return lyr;
                                }
                                else
                                {
                                    //we aren't in the checkbox or the expansion box
                                    return lyr;
                                }
                            }
                            else if (m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                            {

                                if (bounds.Contains(point))
                                {
                                    //We are in the Expansion box
                                    Element.ExpansionBox = true;
                                    return lyr;
                                }
                                else
                                {
                                    if (!lyr.Expanded &&
                                         (lyr.Type == eLayerType.LineShapefile ||
                                         lyr.Type == eLayerType.PointShapefile ||
                                         lyr.Type == eLayerType.PolygonShapefile) && lyr.m_smallIconWasDrawn)
                                    {
                                        // layer icon (to the right from the caption)
                                        CurHeight = Constants.ICON_SIZE;
                                        CurWidth = Constants.ICON_SIZE;
                                        CurTop = lyr.Top + Constants.ICON_TOP_PAD;
                                        CurLeft = this.Width - 36;
                                        if (this.vScrollBar.Visible)
                                            CurLeft -= this.vScrollBar.Width;

                                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
                                        if (bounds.Contains(point))
                                        {
                                            Element.ColorBox = true;
                                            return lyr;
                                        }
                                    }

                                    // layer icon (to the right from the caption)
                                    if (lyr.Type == eLayerType.LineShapefile ||
                                        lyr.Type == eLayerType.PointShapefile ||
                                        lyr.Type == eLayerType.PolygonShapefile)
                                    {
                                        CurHeight = Constants.ICON_SIZE;
                                        CurWidth = Constants.ICON_SIZE;
                                        CurTop = lyr.Top + Constants.ICON_TOP_PAD;
                                        CurLeft = this.Width - 56;
                                        if (this.vScrollBar.Visible)
                                            CurLeft -= this.vScrollBar.Width;

                                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);
                                        if (bounds.Contains(point))
                                        {
                                            Element.LabelsIcon = true;
                                            return lyr;
                                        }
                                    }

                                    // check to see if we are in the default color box
                                    MapWinGIS.Shapefile sf = m_Map.get_GetObject(lyr.Handle) as MapWinGIS.Shapefile;

                                    CurHeight = lyr.Get_CategoryHeight(sf.DefaultDrawingOptions);
                                    CurWidth = lyr.Get_CategoryWidth(sf.DefaultDrawingOptions);
                                    CurTop = lyr.Top + Constants.ITEM_HEIGHT + 2;
                                    CurLeft = Constants.LIST_ITEM_INDENT + Constants.TEXT_LEFT_PAD;
                                    if (CurWidth != Constants.ICON_WIDTH)
                                    {
                                        CurLeft -= ((CurWidth - Constants.ICON_WIDTH) / 2);
                                    }
                                    bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                                    if (bounds.Contains(point))
                                    {
                                        Element.ColorBox = true;
                                        return lyr;
                                    }
                                    else
                                    {
                                        // check to see if we are in the label
                                        CurHeight = lyr.Get_CategoryHeight(sf.DefaultDrawingOptions);
                                        CurWidth = lyr.Get_CategoryWidth(sf.DefaultDrawingOptions);
                                        CurTop = lyr.Top + Constants.ITEM_HEIGHT + 2;
                                        int maxWidth = lyr.Get_MaxIconWidth(sf);
                                        CurLeft = bounds.Left + Constants.TEXT_LEFT_PAD + maxWidth + 5;
                                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                                        if (bounds.Contains(point))
                                        {
                                            Element.Label = true;
                                            return lyr;
                                        }
                                        else
                                        {
                                            // categories
                                            CurLeft = Constants.LIST_ITEM_INDENT + Constants.TEXT_LEFT_PAD;
                                            CurTop = lyr.Top + Constants.ITEM_HEIGHT + 2;   // name
                                            CurTop += CurHeight + 2;                        // default symbology

                                            if (sf.Categories.Count > 0)
                                            {
                                                CurTop += Constants.CS_ITEM_HEIGHT + 2;         // categories caption

                                                for (int cat = 0; cat < sf.Categories.Count; cat++)
                                                {
                                                    MapWinGIS.ShapeDrawingOptions options = sf.Categories.get_Item(cat).DrawingOptions;
                                                    CurWidth = lyr.Get_CategoryWidth(options);
                                                    CurHeight = lyr.Get_CategoryHeight(options);
                                                    bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                                                    CurTop += CurHeight;

                                                    if (bounds.Contains(point))
                                                    {
                                                        Element.ColorBox = true;
                                                        Element.CategoryIndex = cat;
                                                        return lyr;
                                                    }
                                                }
                                            }

                                            if (sf.Charts.NumFields > 0 && sf.Charts.Count > 0)
                                            {
                                                CurTop += Constants.CS_ITEM_HEIGHT + 2;         // charts caption
                                                CurWidth = sf.Charts.IconWidth;
                                                CurHeight = sf.Charts.IconHeight;
                                                bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                                                if (bounds.Contains(point))
                                                {
                                                    Element.Charts = true;
                                                    return lyr;
                                                }
                                                else
                                                {
                                                    CurTop += (CurHeight + 2);
                                                    CurHeight = Constants.ICON_HEIGHT;
                                                    CurWidth = Constants.ICON_WIDTH;

                                                    for (int fld = 0; fld < sf.Charts.NumFields; fld++)
                                                    {
                                                        bounds = new Rectangle(CurLeft, CurTop, CurWidth, CurHeight);

                                                        if (bounds.Contains(point))
                                                        {
                                                            Element.Charts = true;
                                                            Element.ChartFieldIndex = fld;
                                                            //MessageBox.Show("Field selected: " + fld.ToString());
                                                            return lyr;
                                                        }

                                                        CurTop += (Constants.CS_ITEM_HEIGHT + 2);
                                                    }
                                                }
                                            }

                                            // nothing was hit
                                            return lyr;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
       
        /// <summary>
        /// 获取legend是否是锁定状态
        /// </summary>
        public bool Locked
        {
            get
            {
                if (m_LockCount > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 获取legend中的组的数量
        /// </summary>
        protected internal int NumGroups
        {
            get
            {
                return this.m_AllGroups.Count;
            }
        }

        /// <summary>
        /// 如果legend没有锁定，重绘该legend
        /// </summary>
        protected internal void Redraw()
        {
            if (Locked == false)
            {
                //Application.DoEvents();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 如果legend没有锁定，重绘该legend
        /// </summary>
        public void FullRedraw()
        {
            if (Locked == false)
            {
                //Application.DoEvents();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 清空所有层
        /// </summary>
        protected internal void ClearLayers()
        {
            Group grp = null;
            int GrpCount = m_AllGroups.Count;

            for (int i = 0; i < GrpCount; i++)
            {
                grp = (Group)m_AllGroups[i];
                grp.m_Layers.Clear();
            }
            m_Map.RemoveAllLayers();

            Redraw();

            FireLayerSelected(-1);
        }

        /// <summary>
        /// 清空所有组
        /// </summary>
        protected internal void ClearGroups()
        {
            ClearLayers();
            m_Map.RemoveAllLayers();
            m_AllGroups.Clear();

            m_GroupPositions.Clear();

            Redraw();
        }

        /// <summary>
        /// 计算总的绘制高度
        /// </summary>
        private int CalcTotalDrawHeight(bool UseExpandedHeight)
        {
            int i = 0,
                count = m_AllGroups.Count,
                retval = 0;

            if (UseExpandedHeight == true)
            {
                for (i = 0; i < count; i++)
                {
                    ((Group)Groups[i]).RecalcHeight();
                    retval += ((Group)Groups[i]).ExpandedHeight;
                }
            }
            else
            {
                for (i = 0; i < count; i++)
                {
                    ((Group)Groups[i]).RecalcHeight();
                    retval += ((Group)Groups[i]).Height + Constants.ITEM_PAD;
                }
            }

            if (this.tileGroup != null && DisplayTilesGroup)
            {
                this.tileGroup.RecalcHeight();
                retval += this.tileGroup.Height + Constants.ITEM_PAD;
            }

            return retval;
        }

        /// <summary>
        /// 重新计算组和层的顶点
        /// </summary>
        private void RecalcItemPositions()
        {
            //this function calculates the top of each group and layer.
            //this is important because the click events use the stored top as
            //the way of figuring out if the item was clicked
            //and if the checkbox or expansion box was clicked

            int TotalHeight = CalcTotalDrawHeight(false);
            Group grp;
            Layer lyr;
            int CurTop = 0;

            if (vScrollBar.Visible == true)
                CurTop = -vScrollBar.Value;

            for (int i = m_AllGroups.Count - 1; i >= 0; i--)
            {
                grp = (Group)m_AllGroups[i];
                grp.Top = CurTop;
                if (grp.Expanded)
                {
                    CurTop += Constants.ITEM_HEIGHT;
                    for (int j = grp.m_Layers.Count - 1; j >= 0; j--)
                    {
                        lyr = (Layer)grp.m_Layers[j];
                        if (!lyr.HideFromLegend)
                        {
                            lyr.Top = CurTop;

                            CurTop += lyr.Height;
                        }
                    }
                    CurTop += Constants.ITEM_PAD;
                }
                else
                    CurTop += grp.Height + Constants.ITEM_PAD;
            }
        }

        private void DrawNextFrame()
        {
            if (Locked == false)
            {
                int TotalHeight = this.CalcTotalDrawHeight(false);
                Rectangle rect;
                if (TotalHeight > this.Height)
                {
                    vScrollBar.Minimum = 0;
                    vScrollBar.SmallChange = Constants.ITEM_HEIGHT;
                    vScrollBar.LargeChange = this.Height;
                    vScrollBar.Maximum = TotalHeight;

                    if (vScrollBar.Visible == false)
                    {
                        vScrollBar.Value = 0;
                        vScrollBar.Visible = true;
                    }

                    this.RecalcItemPositions();
                    rect = new Rectangle(0, -vScrollBar.Value, this.Width - vScrollBar.Width, TotalHeight);
                }
                else
                {
                    vScrollBar.Visible = false;
                    rect = new Rectangle(0, 0, this.Width, this.Height);
                }

                m_Draw.Clear(Color.White);

                int NumGroups = m_AllGroups.Count;
                Group grp = null;

                for (int i = NumGroups - 1; i >= 0; i--)
                {
                    grp = (Group)m_AllGroups[i];
                    if (rect.Top + grp.Height >= this.ClientRectangle.Top)
                    {
                        this.DrawGroup(m_Draw, grp, rect, false);
                    }

                    rect.Y += grp.Height + Constants.ITEM_PAD;

                    if (rect.Top >= this.ClientRectangle.Bottom)
                        break;
                }

                // drawing tiles
                if (DisplayTilesGroup && tileGroup != null && rect.Top < this.ClientRectangle.Bottom)
                {
                    this.DrawGroup(m_Draw, this.tileGroup, rect, false);
                }
            }
            this.SwapBuffers();
        }

        /// <summary>
        /// 重绘该控件
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // we don't want to paint when when statusbar visibility changed
            if (m_painting)
                return;

            m_FrontBuffer = e.Graphics;

            //m_FrontBuffer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            //m_FrontBuffer.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            DrawNextFrame();
        }

        /// <summary>
        /// 重绘该控件（legend）的背景
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            //do nothing, this helps us avoid flicker

            //if we don't override this function, then
            //the system will clear that background before
            //we draw, causing a flicker when resizing
        }

        private void HandleLeftMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void HandleRightMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Group grp = null;
            Layer lyr = null;

            System.Drawing.Point pnt = new System.Drawing.Point(e.X, e.Y);

            bool InCheckBox = false,
                InExpandBox = false;
            grp = FindClickedGroup(pnt, out InCheckBox, out InExpandBox);
            if (grp != null)
            {
                if (InCheckBox == false && InExpandBox == false)
                {
                    FireGroupMouseDown(grp.Handle, MouseButtons.Right);
                }
                return;
            }

            ClickedElement element = new ClickedElement();
            lyr = FindClickedLayer(pnt, ref element);
            if (lyr != null)
            {
                if (element.CheckBox == false && element.ExpansionBox == false)
                {
                    FireLayerMouseDown(lyr.Handle, MouseButtons.Right);
                }
                return;
            }
            FireLegendClick(MouseButtons.Right, pnt);
        }

        private void HandleRightMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Group grp = null;
            Layer lyr = null;

            System.Drawing.Point pnt = new System.Drawing.Point(e.X, e.Y);

            bool InCheckBox = false, InExpandBox = false;
            grp = FindClickedGroup(pnt, out InCheckBox, out InExpandBox);
            if (grp != null)
            {
                if (InCheckBox == false && InExpandBox == false)
                {
                    FireGroupMouseUp(grp.Handle, MouseButtons.Right);
                }
                return;
            }

            ClickedElement element = new ClickedElement();

            lyr = FindClickedLayer(pnt, ref element);
            if (lyr != null)
            {
                if (element.CheckBox == false && element.ExpansionBox == false)
                {
                    FireLayerMouseUp(lyr.Handle, MouseButtons.Right);
                }
                return;
            }
        }   

        private void HandleLeftMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Capture = false;
            System.Drawing.Point pnt = new System.Drawing.Point(e.X, e.Y);

            Layer lyr = null;
            Group grp = null;
            Group TargetGroup = null;

            m_DragInfo.MouseDown = false;

            if (m_DragInfo.Dragging == true)
            {
                if (m_DragInfo.LegendLocked == true)
                {
                    m_DragInfo.LegendLocked = false;
                    Unlock();//unlock the legend
                }

                m_MidBuffer = null;

                if (m_DragInfo.DraggingLayer)
                {
                    if (m_DragInfo.TargetGroupIndex != Constants.INVALID_INDEX)
                    {
                        TargetGroup = (Group)m_GroupManager[m_DragInfo.TargetGroupIndex];
                        grp = (Group)m_AllGroups[m_DragInfo.DragGroupIndex];

                        int OldPos = 0,
                            NewPos = 0,
                            LayerHandle = -1,
                            temp = 0;

                        LayerHandle = grp.LayerHandle(m_DragInfo.DragLayerIndex);

                        if (TargetGroup.Handle == grp.Handle)
                        {
                            //movement within the same group

                            FindLayerByHandle(LayerHandle, out temp, out OldPos);

                            //we may have to adjust the new position if moving up in the group
                            //because the way we are using TargetLayerIndex is marking things differently
                            //than the moveLayer function expects it
                            if (OldPos < m_DragInfo.TargetLayerIndex)
                                NewPos = m_DragInfo.TargetLayerIndex - 1;
                            else
                                NewPos = m_DragInfo.TargetLayerIndex;
                        }
                        else
                        {
                            //movement from one group to another group
                            NewPos = m_DragInfo.TargetLayerIndex;
                        }

                        MoveLayer(TargetGroup.Handle, LayerHandle, NewPos);
                    }
                }
                else
                {//we are dragging a group
                    if (IsValidIndex(m_AllGroups, m_DragInfo.DragGroupIndex) == false)
                    {
                        m_DragInfo.Reset();
                        return;
                    }

                    int grpHandle = ((Group)m_AllGroups[m_DragInfo.DragGroupIndex]).Handle;

                    //adjust the target group index because we are setting TargetGroupIndex
                    //differently than the MoveGroup Function expects it
                    if (m_DragInfo.DragGroupIndex < m_DragInfo.TargetGroupIndex)
                        m_DragInfo.TargetGroupIndex -= 1;

                    MoveGroup(grpHandle, m_DragInfo.TargetGroupIndex);
                }

                m_DragInfo.Reset();
                Redraw();
            }

            //are we completing a mouseup on a group?
            bool InCheck = false;
            bool InExpansion = false;
            grp = FindClickedGroup(pnt, out InCheck, out InExpansion);
            if (grp != null && InCheck == false && (InExpansion == false || grp.m_Layers.Count == 0))
            {
                FireGroupMouseUp(grp.Handle, MouseButtons.Left);
                return;
            }

            InCheck = false;
            InExpansion = false;
            //now figure out if we are completing a mouseup on a layer
            lyr = FindClickedLayer(pnt, out InCheck, out InExpansion);
            if (lyr != null && InCheck == false)
            {
                if (InExpansion == false)
                {
                    FireLayerMouseUp(lyr.Handle, MouseButtons.Left);
                    return;
                }
                else
                {
                    if (lyr.ColorLegend.Count == 0)
                    {
                        //if the expansion box is hidden, then fire a mouse up
                        FireLayerMouseUp(lyr.Handle, MouseButtons.Left);
                        return;
                    }
                }
            }

            //if no other mouseup event is send, then send the LegendMouseUp
            FireLegendClick(MouseButtons.Left, pnt);
        }

        private bool IsValidIndex(ArrayList list, int index)
        {
            if (index >= list.Count)
                return false;
            if (index < 0)
                return false;

            return true;
        }

        private void UpdateMapLayerPositions()
        {
            int GrpCount = m_AllGroups.Count;
            int LyrCount;
            Layer lyr = null;
            Group grp = null;
            int lyrPosition;

            m_Map.LockWindow(MapWinGIS.tkLockMode.lmLock);
            for (int i = GrpCount - 1; i >= 0; i--)
            {
                grp = (Group)m_AllGroups[i];
                LyrCount = grp.m_Layers.Count;
                for (int j = LyrCount - 1; j >= 0; j--)
                {
                    lyr = (Layer)grp.m_Layers[j];
                    lyrPosition = m_Map.get_LayerPosition(lyr.Handle);
                    m_Map.MoveLayerBottom(lyrPosition);
                }
            }
            m_Map.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
        }

        private void Legend_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    HandleLeftMouseDown(sender, e);
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    HandleRightMouseDown(sender, e);
                    break;
            }
        }

        private void Legend_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void Legend_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_DragInfo.MouseDown == true && Math.Abs(m_DragInfo.StartY - e.Y) > 10)
            {
                m_DragInfo.Dragging = true;
                if (m_DragInfo.LegendLocked == false)
                {
                    Lock();//lock the Legend
                    m_DragInfo.LegendLocked = true;
                }
            }

            if (m_DragInfo.Dragging == true)
            {
                FindDropLocation(e.Y);
                DrawDragLine(m_DragInfo.TargetGroupIndex, m_DragInfo.TargetLayerIndex);
            }

        }

        private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            VScrollBar sbar = (VScrollBar)sender;
            sbar.Value = e.NewValue;
            Redraw();
        }

        private void Legend_DoubleClick(object sender, System.EventArgs e)
        {
            Group grp = null;
            Layer lyr = null;

            System.Drawing.Point pnt = Globals.GetCursorLocation();
            pnt = this.PointToClient(pnt);

            bool InCheckBox = false, InExpandBox = false;

            grp = FindClickedGroup(pnt, out InCheckBox, out InExpandBox);
            if (grp != null)
            {
                if (InCheckBox == false && InExpandBox == false)
                {
                    FireGroupDoubleClick(grp.Handle);
                }
                return;
            }

            ClickedElement element = new ClickedElement();

            lyr = FindClickedLayer(pnt, ref element);
            if (lyr != null)
            {
                if (lyr.Type == eLayerType.Tiles)
                {
                    this.FireTileLayerDoubleClick(pnt.X, pnt.Y);
                }
                else if (element.CheckBox == false && element.ExpansionBox == false)
                {
                    FireLayerDoubleClick(lyr.Handle);
                }
                return;
            }
        }

        private void DrawDragLine(int GrpIndex, int LyrIndex)
        {
            int DrawY = 0;
            Group grp = null;

            if (m_DragInfo.Dragging)
            {
                if (IsValidIndex(m_AllGroups, GrpIndex) == true)
                    grp = (Group)m_AllGroups[GrpIndex];

                if (m_DragInfo.DraggingLayer)
                {
                    if (grp == null)
                        return; //don't draw anything

                    int LayerCount = grp.m_Layers.Count;

                    if (LyrIndex < 0 && LayerCount > 0)
                    {//the item goes at the bottom of the list
                        DrawY = ((Layer)grp.m_Layers[0]).Top + ((Layer)grp.m_Layers[0]).Height;
                    }
                    if (LayerCount > LyrIndex && LyrIndex >= 0)
                    {
                        int ItemTop = ((Layer)grp.m_Layers[LyrIndex]).Top;
                        DrawY = ItemTop + ((Layer)grp.m_Layers[LyrIndex]).Height;
                    }
                    else
                    {//the layer is to be placed at the top of the list
                        DrawY = grp.Top + Constants.ITEM_HEIGHT;
                    }
                }
                else
                {//we are dragging a group
                    if (GrpIndex < 0 || GrpIndex >= (int)m_AllGroups.Count)
                    {//the mouse is either above the top layer or below the bottom layer
                        if (GrpIndex < 0)
                            DrawY = ((Group)m_AllGroups[0]).Top + ((Group)m_AllGroups[0]).Height;
                        else
                            DrawY = ((Group)m_AllGroups[m_AllGroups.Count - 1]).Top;
                    }
                    else
                    {
                        //if(grp.Expanded == true)
                        DrawY = grp.Top + grp.Height;//CalcGroupHeight(grp);
                        //else
                        //	DrawY = grp.Top + grp.Height;//CalcGroupHeight(grp);
                    }
                }

                m_FrontBuffer = this.CreateGraphics();
                if (m_MidBuffer == null)
                    m_MidBuffer = new Bitmap(m_BackBuffer.Width, m_BackBuffer.Height, m_Draw);

                Graphics LocalDraw = Graphics.FromImage(m_MidBuffer);
                SwapBuffers(m_BackBuffer, LocalDraw);

                Pen pen = (Pen)Pens.Gray.Clone();
                pen.Width = 3;

                //draw a horizontal line
                LocalDraw.DrawLine(pen, Constants.ITEM_PAD, DrawY, this.Width - Constants.ITEM_RIGHT_PAD, DrawY);

                //draw the left vertical line
                LocalDraw.DrawLine(pen, Constants.ITEM_PAD, DrawY - 3, Constants.ITEM_PAD, DrawY + 3);

                //draw the right vertical line
                LocalDraw.DrawLine(pen, this.Width - Constants.ITEM_RIGHT_PAD, DrawY - 3, this.Width - Constants.ITEM_RIGHT_PAD, DrawY + 3);

                SwapBuffers(m_MidBuffer, m_FrontBuffer);
            }
        }

        private void FindDropLocation(int YPosition)
        {
            m_DragInfo.TargetGroupIndex = Constants.INVALID_INDEX;
            m_DragInfo.TargetLayerIndex = Constants.INVALID_INDEX;

            int grpCount, itemCount;
            Group grp = null;
            Group TopGroup = null,
                BottomGroup = null,
                TempGroup = null;
            Layer lyr = null;
            int grpHeight;

            grpCount = m_AllGroups.Count;

            if (grpCount < 1)
                return;

            TopGroup = (Group)m_AllGroups[grpCount - 1];
            BottomGroup = (Group)m_AllGroups[0];

            if (m_DragInfo.DraggingLayer == true)
            {
                if (YPosition >= (BottomGroup.Top + BottomGroup.Height))
                {//the mouse is below the bottom layer, mark for drop at bottom
                    m_DragInfo.TargetGroupIndex = 0;
                    m_DragInfo.TargetLayerIndex = 0;

                    return;
                }
                else if (YPosition <= TopGroup.Top)
                {//the mouse is above the top layer, mark for drop at top
                    m_DragInfo.TargetGroupIndex = grpCount - 1;
                    m_DragInfo.TargetLayerIndex = TopGroup.m_Layers.Count;

                    return;
                }

                //not the bottom or the top, so we must search for the correct one
                for (int i = grpCount - 1; i >= 0; i--)
                {
                    grp = (Group)m_AllGroups[i];

                    grpHeight = grp.Height;

                    //can we drop it at the top of the group?
                    //if(YPosition <= grp.Top && YPosition < grp.Top+Constants.ITEM_HEIGHT)
                    if (YPosition < grp.Top + Constants.ITEM_HEIGHT)
                    {
                        m_DragInfo.TargetLayerIndex = grp.m_Layers.Count;
                        m_DragInfo.TargetGroupIndex = i;
                        return;
                    }
                    else
                    {
                        itemCount = grp.m_Layers.Count;

                        if (itemCount == 0)
                        {
                            //if(YPosition > grp.Top && YPosition <= grp.Top + grpHeight)
                            if (YPosition > grp.Top && YPosition <= grp.Top + Constants.ITEM_HEIGHT)
                            {
                                m_DragInfo.TargetGroupIndex = i;
                                m_DragInfo.TargetLayerIndex = Constants.INVALID_INDEX;
                                return;
                            }
                        }
                        else if (grp.Expanded == true)
                        {
                            for (int j = itemCount - 1; j >= 0; j--)
                            {
                                lyr = (Layer)grp.m_Layers[j];
                                if (YPosition <= (lyr.Top + lyr.Height))
                                {
                                    //drop before this item
                                    m_DragInfo.TargetGroupIndex = i;
                                    m_DragInfo.TargetLayerIndex = j;
                                    return;
                                }
                                if (j == 0)
                                {
                                    //if this item is the bottom one, check to see if the item can be
                                    //dropped after this item
                                    if (i > 0)//if the group is not the bottom group
                                    {
                                        TempGroup = (Group)m_AllGroups[i - 1];
                                        if (YPosition <= TempGroup.Top && YPosition > lyr.Top + lyr.Height)
                                        {
                                            m_DragInfo.TargetGroupIndex = i;
                                            m_DragInfo.TargetLayerIndex = 0;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (YPosition > lyr.Top + lyr.Height)
                                        {
                                            m_DragInfo.TargetGroupIndex = 0;
                                            m_DragInfo.TargetLayerIndex = 0;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {//the group is not expanded
                            if (YPosition > grp.Top && YPosition < grp.Top + grpHeight)
                            {
                                m_DragInfo.TargetGroupIndex = i;
                                m_DragInfo.TargetLayerIndex = grp.m_Layers.Count;//put the item at the top
                            }
                        }
                    }
                }
            }
            else
            {//we are dragging a group
                if (YPosition > (BottomGroup.Top + BottomGroup.Height))
                {//the mouse is below the bottom layer, mark for drop at bottom
                    m_DragInfo.TargetGroupIndex = Constants.INVALID_INDEX;
                    m_DragInfo.TargetLayerIndex = Constants.INVALID_INDEX;

                    return;
                }
                else if (YPosition <= TopGroup.Top)
                {//the mouse is above the top Group, mark for drop at top
                    m_DragInfo.TargetGroupIndex = grpCount;
                    m_DragInfo.TargetLayerIndex = Constants.INVALID_INDEX;
                    return;
                }

                //we have to compare against all groups because we aren't at the top or bottom
                for (int i = grpCount - 1; i >= 0; i--)
                {
                    grp = (Group)m_AllGroups[i];

                    if (YPosition < grp.Top + grp.Height)
                    {
                        m_DragInfo.TargetGroupIndex = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 处理层位置改变
        /// </summary>
        /// <param name="Lyr">被移动的层</param>
        /// <param name="SourceGroup">源组</param>
        /// <param name="DestinationGroup">目标组（可以和源组相同）</param>
        /// <param name="TargetPosition">目标位置</param>
        /// <returns></returns>
        private void ChangeLayerPosition(int CurrentPositionInGroup, Group SourceGroup, int TargetPositionInGroup, Group DestinationGroup)
        {
            Layer Lyr = null;

            if (CurrentPositionInGroup < 0 || CurrentPositionInGroup >= SourceGroup.m_Layers.Count)
                throw new Exception("无效层索引");

            Lyr = (Layer)SourceGroup.m_Layers[CurrentPositionInGroup];
            SourceGroup.m_Layers.Remove(Lyr);

            if (TargetPositionInGroup >= DestinationGroup.m_Layers.Count)
                DestinationGroup.m_Layers.Add(Lyr);
            else if (TargetPositionInGroup <= 0)
                DestinationGroup.m_Layers.Insert(0, Lyr);
            else
                DestinationGroup.m_Layers.Insert(TargetPositionInGroup, Lyr);

            SourceGroup.RecalcHeight();
            SourceGroup.UpdateGroupVisibility();

            if (SourceGroup.Handle != DestinationGroup.Handle)
            {
                DestinationGroup.RecalcHeight();
                DestinationGroup.UpdateGroupVisibility();

                m_SelectedGroupHandle = DestinationGroup.Handle;
            }
        }

        /// <summary>
        /// 将一个层移动到新的位置
        /// </summary>
        /// <param name="TargetGroupHandle">接受层移进去的组的句柄</param>
        /// <param name="LayerHandle">要移动层的句柄</param>
        /// <param name="NewPos">在目标层中层放置的位置（以0开始）</param>
        /// <returns>True 层位置改变, False 其他</returns>
        protected internal bool MoveLayer(int TargetGroupHandle, int LayerHandle, int TargetPositionInGroup)
        {
            Group SourceGroup = null;
            Group DestinationGroup = null;
            //Layer Lyr = null;

            int SourceGroupIndex = 0;
            int CurrentPositionInGroup = 0;
            int OldMapPos;
            int NewMapPos;

            bool result = false;

            try
            {
                if (!m_LayerManager.IsValidHandle(LayerHandle))
                    throw new Exception("无效 Handle (LayerHandle)");

                if (!IsValidGroup(TargetGroupHandle))
                    throw new Exception("无效 Handle (TargetGroupHandle)");

                FindLayerByHandle(LayerHandle, out SourceGroupIndex, out CurrentPositionInGroup);

                SourceGroup = (Group)Groups[SourceGroupIndex];
                DestinationGroup = (Group)m_GroupManager.ItemByHandle(TargetGroupHandle);

                if (CurrentPositionInGroup != TargetPositionInGroup || SourceGroup.Handle != DestinationGroup.Handle)
                {
                    OldMapPos = m_Map.get_LayerPosition(LayerHandle);

                    ChangeLayerPosition(CurrentPositionInGroup, SourceGroup, TargetPositionInGroup, DestinationGroup);
                    UpdateMapLayerPositions();

                    NewMapPos = m_Map.get_LayerPosition(LayerHandle);

                    int CurHandle;
                    int CurPos;
                    int EndPos;

                    CurPos = Math.Min(OldMapPos, NewMapPos);
                    EndPos = Math.Max(OldMapPos, NewMapPos);

                    while (CurPos <= EndPos)
                    {
                        CurHandle = m_Map.get_LayerHandle(CurPos);
                        FireLayerPositionChanged(CurHandle);
                        CurPos += 1;
                    }

                    Redraw();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Globals.LastError = ex.ToString();
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 移动组到一个新的位置
        /// </summary>
        /// <param name="GroupHandle">移动组的句柄</param>
        /// <param name="NewPos">要移动的位置（以0开始）</param>
        /// <returns>True 成功, False 其他</returns>
        protected internal bool MoveGroup(int GroupHandle, int NewPos)
        {
            if (IsValidGroup(GroupHandle))
            {
                int OldPos = (int)m_GroupPositions[GroupHandle];

                if (OldPos == NewPos)
                    return true;

                Group grp = (Group)m_GroupManager.ItemByHandle(GroupHandle);

                if (NewPos < 0)
                {
                    NewPos = 0;
                }

                if (NewPos >= NumGroups)
                {
                    m_AllGroups.RemoveAt(OldPos);
                    m_AllGroups.Add(grp);
                }
                else
                {
                    m_AllGroups.RemoveAt(OldPos);
                    m_AllGroups.Insert(NewPos, grp);
                }

                if (grp.m_Layers.Count > 0)
                {//now we have to move the layers around
                    UpdateMapLayerPositions();
                }

                UpdateGroupPositions();
                Redraw();

                FireGroupPositionChanged(grp.Handle);
                return true;
            }
            else
            {
                Globals.LastError = "Invalid Group Handle";
                return false;
            }
        }

        private void DrawTransparentPatch(Graphics DrawTool, int TopPos, int LeftPos, int BoxHeight, int BoxWidth, Color OutlineColor, bool DrawOutline)
        {
            Rectangle rect = new Rectangle(LeftPos, TopPos, BoxWidth, BoxHeight);
            Pen pen = new Pen(OutlineColor);

            //fill the rectangle with a diagonal hatch
            System.Drawing.Brush brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, m_BoxLineColor, Color.White);
            DrawTool.FillRectangle(brush, rect);

            if (DrawOutline) DrawTool.DrawRectangle(pen, LeftPos, TopPos, BoxWidth, BoxHeight);
        }

        private void DrawColorPatch(Graphics DrawTool, Color StartColor, Color EndColor, int TopPos, int LeftPos, int BoxHeight, int BoxWidth, Color OutlineColor, bool DrawOutline)
        {
            DrawColorPatch(DrawTool, StartColor, EndColor, TopPos, LeftPos, BoxHeight, BoxWidth, OutlineColor, DrawOutline, eLayerType.Invalid);
        }

        private void DrawColorPatch(Graphics DrawTool, Color StartColor, Color EndColor, int TopPos, int LeftPos, int BoxHeight, int BoxWidth, Color OutlineColor, bool DrawOutline, eLayerType LayerType)
        {
            // Note - LayerType == invalid when we don't care :)

            if (LayerType == eLayerType.LineShapefile)
            {
                if (StartColor.A == 0) StartColor = Color.FromArgb(255, StartColor);
                Pen pen = new Pen(StartColor, 2);

                System.Drawing.Drawing2D.SmoothingMode OldSmoothingMode = DrawTool.SmoothingMode;
                DrawTool.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                DrawTool.DrawLine(pen, LeftPos, TopPos + 8, LeftPos + 4, TopPos + 3);
                DrawTool.DrawLine(pen, LeftPos + 4, TopPos + 3, LeftPos + 9, TopPos + 10);
                DrawTool.DrawLine(pen, LeftPos + 9, TopPos + 10, LeftPos + 13, TopPos + 4);

                DrawTool.SmoothingMode = OldSmoothingMode;
            }
            else
            {
                Rectangle rect = new Rectangle(LeftPos, TopPos, BoxWidth, BoxHeight);
                Pen pen = new Pen(OutlineColor);

                //fill the rectangle with a gradient fill
                System.Drawing.Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, StartColor, EndColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                DrawTool.FillRectangle(brush, rect);

                if (DrawOutline) DrawTool.DrawRectangle(pen, LeftPos, TopPos, BoxWidth, BoxHeight);
            }
        }

        /// <summary>
        /// 处理鼠标滚轮事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (vScrollBar.Visible == true)
            {
                int StepSize;
                int MaxSize = vScrollBar.Maximum - this.Height;

                StepSize = vScrollBar.SmallChange;
                if (e.Delta >= 0)
                    StepSize *= -1;

                if (vScrollBar.Value + StepSize < 0)
                {
                    vScrollBar.Value = 0;
                }
                else if (vScrollBar.Value + StepSize > MaxSize)
                {
                    vScrollBar.Value = MaxSize + 1;
                }
                else
                {
                    vScrollBar.Value += StepSize;
                }
                Redraw();
            }
        }



    }


    /// <summary>
    /// legend上点击的元素
    /// </summary>
    public class ClickedElement
    {
        public bool ColorBox = false;
        public bool CheckBox = false;
        public bool ExpansionBox = false;
        public bool Charts = false;
        public bool Label = false;
        public bool LabelsIcon = false;
        public int ChartFieldIndex = -1;
        public int CategoryIndex = -1;
        public int GroupIndex = -1;

        public void Nullify()
        {
            ColorBox = false;
            CheckBox = false;
            ExpansionBox = false;
            Charts = false;
            Label = false;
            ChartFieldIndex = -1;
            CategoryIndex = -1;
            GroupIndex = -1;
        }
    }

    /// <summary>
    /// 呈现在legend上层的元素
    /// </summary>
    internal enum LayerElementType
    {
        None = 0,
        Name = 1,
        Symbol = 2,
        Label = 3,
        CategoriesCaption = 4,
        CategoryName = 5,
        ChartsCaption = 7,
        Charts = 8,
        ChartField = 9,
        ChartFieldName = 10,
    }

    /// <summary>
    /// 层的元素, 包括位置大小有关的元素
    /// </summary>
    internal class LayerElement
    {
        internal LayerElementType ElementType = LayerElementType.None;
        internal int Index = -1;                // 目录或区域的所索引
        internal string Text = string.Empty;    // 名字或标题

        //点击文本的区域
        internal int Top = 0;
        internal int Left = 0;
        internal int Width = 0;
        internal int Height = 0;

        internal LayerElement(LayerElementType type, int top, int left, int width, int height)
        {
            ElementType = type;
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }

        internal LayerElement(LayerElementType type, Rectangle rect, string text, int index)
        {
            ElementType = type;
            Top = rect.Top;
            Left = rect.Left;
            Width = rect.Width;
            Height = rect.Height;
            Index = index;
        }

        internal LayerElement(LayerElementType type, Rectangle rect) : this(type, rect, string.Empty, -1) { }

        internal LayerElement(LayerElementType type, Rectangle rect, int index) : this(type, rect, string.Empty, index) { }

        internal LayerElement(LayerElementType type, Rectangle rect, string text) : this(type, rect, text, -1) { }
    }

}
