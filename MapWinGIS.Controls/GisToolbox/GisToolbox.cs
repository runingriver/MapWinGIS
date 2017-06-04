/***************************************************************
 * 文件名：GisToolbox.cs
 * 描  述：Gis工具箱类管理Gis工具控件；
 *         初始化控件绘制，包括上部树形视图面板和下部描述面板；
 *         Gis工具盒容器承载工具集合Groups，工具集合下有多个Gis工具tool；
 *         创建Gis工具箱：
 *         （1）需要创建一个工具箱GisToolbox新实例m_GisToolbox用于存放工具和工具集合
 *         （2）在工具箱的基础上创建工具m_tool和工具集合m_group
 *              m_GisToolbox.CreateTool(string,string);
 *              m_GisToolbox.CreateGroup(string,string);
 *         （3）将工具添加到工具集合中
 *              m_group.Tools.Add(m_tool);
 *         （4）将工具集合添加到工具箱中
 *              m_GisToolbox.Groups.Add(m_group);
 *          程序在加载插件时，已经创建好的工具箱就被创建到主程序
 * **************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace MapWinGIS.Controls.GisToolbox
{

    /// <summary>
    /// GISToolbox 控件
    /// </summary>
    public partial class GisToolbox : SplitContainer, IGisToolBox 
    {
        ///icon 索引
        internal const int ICON_FOLDER = 0;//文件夹
        internal const int ICON_FOLDER_OPEN = 1;//打开的文件夹
        internal const int ICON_TOOL = 2;//工具

        //树形视图
        TreeView m_tree = null;

        //text box显示描述
        RichTextBox m_textbox = null;

        #region 初始化
        /// <summary>
        /// 创建一个GIS Toolbox类的实例
        /// </summary>
        public GisToolbox()
        {
            this.BorderStyle = BorderStyle.FixedSingle;  //将控件边框样式设置为单行边框

            m_tree = new TreeView();//创建TreeView的新实例
            m_tree.BorderStyle = System.Windows.Forms.BorderStyle.None;//TreeView的边框样式为无边框
            m_tree.Dock = System.Windows.Forms.DockStyle.Fill;//TreeView中停靠方式为填充
            this.Panel1.Controls.Add(m_tree);//将TreeView控件添加到GISToolbox控件集合的上部面板(由于Orientation为Horizontal)

            m_textbox = new RichTextBox();//创建RichTextBox的新实例
            m_textbox.BorderStyle = System.Windows.Forms.BorderStyle.None;//RichTextBox的边框样式为无边框
            m_textbox.Dock = System.Windows.Forms.DockStyle.Fill;//RichTextBox中停靠方式为填充
            m_textbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;//RichTextBox控件不显示滚动条
            m_textbox.BackColor = Color.FromKnownColor(KnownColor.Control);//RichTextBox控件的背景色为三维元素的系统定义表面颜色
            m_textbox.ReadOnly = true;//RichTextBox控件的文本为只读
            m_textbox.Text = "No tool is selected.";//RichTextBox的当前文本
            this.Panel2.Controls.Add(m_textbox);//将RichTextBox控件添加到GISToolbox控件集合的下部面板(由于Orientation为Horizontal）

            this.Panel1MinSize = 0;//拆分器离Panel1的上边缘的最小距离
            this.Panel2MinSize = 0;//拆分器离Panel2的下边缘的最小距离
            this.Orientation = Orientation.Horizontal;//SplitContainer面板为水平方向
            this.SplitterDistance = Convert.ToInt32((double)this.Height * 0.9);//拆分器离SplitContainer的上边缘的最小距离

            this.InitImageList();//初始化图像列表
            
            //添加事件句柄
            m_tree.BeforeExpand += m_GeoprocessingTree_BeforeExpand;//打开文件夹监听
            m_tree.BeforeCollapse += m_GeoprocessingTree_BeforeCollapse;//关闭文件夹监听
            m_tree.AfterSelect += m_GeoprocessingTree_AfterSelect;//工具被选择后的监听
            m_tree.NodeMouseDoubleClick += m_tree_NodeMouseDoubleClick;//鼠标双击的监听
            this.ToolSelected += new ToolSelectedDelegate(GisToolbox_ToolSelected);
            this.GroupSelected += new GroupSelectedDelegate(GisToolbox_GroupSelected);
        }

        /// <summary>
        /// 显示组描述
        /// </summary>
        void GisToolbox_GroupSelected(IGisToolboxGroup group, ref bool handled)
        {
            this.m_textbox.Clear();//清除所有文本
            this.m_textbox.Text = group.Name + Environment.NewLine + Environment.NewLine + group.Description;//文本内容为组的工具名和组的工具描述
            this.m_textbox.Select(0, group.Name.Length);//选择文本框中的文本框的范围为组名的长度
            this.m_textbox.SelectionFont = new Font(this.Font, FontStyle.Bold);//文本字体
            handled = true;
        }

        /// <summary>
        /// 显示工具描述
        /// </summary>
        void GisToolbox_ToolSelected(IGisTool tool, ref bool handled)
        {
            this.m_textbox.Clear();//清除所有文本
            this.m_textbox.Text = tool.Name + Environment.NewLine + Environment.NewLine + tool.Description;//文本内容为组的工具名和组的工具描述
            if (tool.Name.Length > 0)
            {
                this.m_textbox.Select(0, tool.Name.Length);//选择文本框中的文本框的范围为工具名的长度
                this.m_textbox.SelectionFont = new Font(this.Font, FontStyle.Bold);//文本字体
            }
            handled = true;
        }
 
        /// <summary>
        /// 初始化图像列表
        /// </summary>
        private void InitImageList()
        {
            ImageList imageList = new ImageList();//创建ImageList的新实例
            imageList.ColorDepth = ColorDepth.Depth32Bit;//图像列表的颜色深度为32位图像

            Bitmap bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.folder, new Size(16, 16));
            imageList.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.folder_open, new Size(16, 16));
            imageList.Images.Add(bmp);

            bmp = new Bitmap(MapWinGIS.Controls.Properties.Resources.tool, new Size(16, 16));
            imageList.Images.Add(bmp);

            m_tree.ImageList = imageList;
        }
        
        #endregion

        #region IGISToolBox的成员

        /// <summary>
        /// 当用户点击工具运行时解除
        /// </summary>
        public event ToolSelectedDelegate ToolClicked;

        /// <summary>
        /// 将事件传递给所有监听者
        /// </summary>
        private void FireToolClicked(IGisTool tool, ref bool handled)
        {
            if (this.ToolClicked != null)
            {
                this.ToolClicked(tool, ref handled);
            }
        }

        /// <summary>
        /// 当工具被选择时解除
        /// </summary>
        public event ToolSelectedDelegate ToolSelected;

        /// <summary>
        /// 将事件传递给所有监听者
        /// </summary>
        private void FireToolSelected(IGisTool tool, ref bool handled)
        {
            if(this.ToolSelected != null)
            {
                this.ToolSelected(tool, ref handled);
            }
        }

        /// <summary>
        /// 当组被选择时解除
        /// </summary>
        public event GroupSelectedDelegate GroupSelected;

        /// <summary>
        /// 将事件传递给所有监听者
        /// </summary>
        private void FireGroupSelected(IGisToolboxGroup group, ref bool handled)
        {
            if (this.GroupSelected != null)
            {
                this.GroupSelected(group, ref handled);
            }
        }

        /// <summary>
        /// 返回位于工具箱中第一级别的组的清单
        /// 这份清单是从每个节点调用动态聚集，所以最好不要反复调用
        /// </summary>
        public IGisToolboxGroups Groups
        {
            get 
            {
                return new GisToolboxGroups(m_tree.Nodes);
            }
        }

        /// <summary>
        /// 返回工具箱中的所有工具的清单
        /// </summary>
        public IEnumerable<IGisTool> Tools
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 创建一个Gistool类的新实例
        /// </summary>
        public IGisTool CreateTool(string name, string key)
        {
            GisTool tool = new GisTool(name, key, "");
            return tool;
        }

        /// <summary>
        /// 创建一个GisToolboxGroup类的新实例
        /// </summary>
        public IGisToolboxGroup CreateGroup(string name, string description)
        {
            return new GisToolboxGroup(name, description);
        }

        /// <summary>
        /// 扩展所有组到指定级别
        /// </summary>
        public void ExpandGroups(int level)
        {
            expandGroups(this.Groups, level);
        }

        /// <summary>
        /// 所有子组递归扩展到指定级别
        /// </summary>
        private void expandGroups(IGisToolboxGroups groups, int level)
        {
            foreach (IGisToolboxGroup group in this.Groups)
            {
                group.Expanded = true;
                level--;
                if (level > 0)
                    expandGroups(group.SubGroups, level);
            }
        }
        #endregion

        #region 树形视图事件
        /// <summary>
        /// 设置文件夹的关闭状态
        /// </summary>
        private void m_GeoprocessingTree_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            if ((e.Node != null))
            {
                if (e.Node.ImageIndex == ICON_FOLDER_OPEN)//树节点处于未选定状态时所显示图像的图像列表索引值为1
                {
                    e.Node.ImageIndex = ICON_FOLDER;//树节点处于未选定状态时所显示图像的图像列表索引值为0
                    e.Node.SelectedImageIndex = ICON_FOLDER;//树节点处于选定状态时所显示图像的图像列表索引值为0
                }
            }
        }

        /// <summary>
        /// 设置文件夹的打开状态
        /// </summary>
        private void m_GeoprocessingTree_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            if ((e.Node != null))
            {
                if (e.Node.ImageIndex == ICON_FOLDER)//树节点处于未选定状态时所显示图像的图像列表索引值为0
                {
                    e.Node.ImageIndex = ICON_FOLDER_OPEN;//树节点处于未选定状态时所显示图像的图像列表索引值为1
                    e.Node.SelectedImageIndex = ICON_FOLDER_OPEN;//树节点处于选定状态时所显示图像的图像列表索引值为1
                }
            }
        }

        /// <summary>
        /// 解除事件，设置选择模式为普通模式相同的icons
        /// </summary>
        private void m_GeoprocessingTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        { 
            if((e.Node != null))
            {
                e.Node.SelectedImageIndex = e.Node.ImageIndex;
                if((e.Node.Tag != null))
                {
                    bool handled = false;
                    if (e.Node.ImageIndex == ICON_TOOL)//树节点处于未选定状态时所显示图像的图像列表索引值为2
                    {
                        IGisTool tool = e.Node.Tag as IGisTool;
                        if (tool != null)
                            FireToolSelected(tool, ref handled);
                    }
                    else
                    {
                        //假定它是个文件夹
                        IGisToolboxGroup group = e.Node.Tag as IGisToolboxGroup;
                        if (group != null)
                            FireGroupSelected(group, ref handled);
                    }
                }
            }
        }

        /// <summary>
        /// 生成双击击事件插件工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node != null && e.Node.Tag != null)
            {
                if (e.Node.ImageIndex == ICON_TOOL)//树节点处于未选定状态时所显示图像的图像列表索引值为2
                {
                    bool handled = false;
                    IGisTool tool = e.Node.Tag as IGisTool;
                    if (tool != null)
                    {
                        FireToolClicked(tool, ref handled);
                    }
                }
            }
        }
        #endregion
    }

}
