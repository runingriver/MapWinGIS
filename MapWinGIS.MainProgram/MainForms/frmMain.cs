/****************************************************************************
 * 文件名:frmMain.cs
 * 描  述:
 * **************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MapWinGIS.Interfaces;
using Microsoft.VisualBasic;

namespace MapWinGIS.MainProgram
{
    public partial class MapWinForm : Form, MapWinGIS.Interfaces.IMapWin
    {
        #region ----------------声明变量---------------
        public System.Resources.ResourceManager resources =
            new System.Resources.ResourceManager("MapWinGIS.MainProgram.GlobalResource", System.Reflection.Assembly.GetExecutingAssembly());

        internal WeifenLuo.WinFormsUI.Docking.DockPanel dckPanel;
        internal MWDockPanel legendPanel;
        internal MWDockPanel previewPanel;
        internal MWDockPanel mapPanel;

        /// <summary>
        /// legend中的图层和工具箱选项卡
        /// </summary>
        internal TabControl m_legendTabControl = null;

        internal MainProgram.PluginTracker m_PluginManager;  //插件管理
        internal MainProgram.Menus m_Menu;
        internal MainProgram.Layers m_Layers;
        internal MainProgram.View m_View;
        internal MainProgram.PreviewMap m_PreviewMap;
        internal MainProgram.LegendPanel m_LegendPanel;
        internal MainProgram.StatusBar m_StatusBar;
        internal MainProgram.Project m_Project;
        internal MainProgram.Toolbar m_Toolbar;
        internal MainProgram.Reports m_Reports;
        internal MainProgram.UIPanel m_UIPanel;
        internal MainProgram.LabelClass m_Labels;      
        internal MainProgram.LegendEditorForm m_legendEditor;
        internal MainProgram.UserInteraction m_UserInteraction;
        internal MainProgram.DynamicVisibilityClass m_AutoVis;
        
        internal MapWinGIS.Controls.GisToolbox.GisToolbox m_GisToolbox = null;

        internal bool m_HasBeenSaved;
        internal ArrayList m_Extents;//存储每次图层改变的extent值
        internal int m_CurrentExtent; //存储在m_Extents当前图层的extent的索引。
        private double m_lastScale; //最近一次的extent
        internal bool m_IsManualExtentsChange;//手动改变了Extent,即通过前进后退改变的。
        internal string CustomWindowTitle = "";

        internal bool m_HandleFileDrop;

        internal int m_GroupHandle; //用作legend事件

        private System.Windows.Forms.Timer MapToolTipTimer;
        /// <summary>
        /// 近期项目的前缀"mnuRecentProjects_"
        /// </summary>
        private const string RecentProjectPrefix = "mnuRecentProjects_";
        /// <summary>
        /// 查看书签的前缀"mnuBookmarkedView_"
        /// </summary>
        private const string BookmarkedViewPrefix = "mnuBookmarkedView_";
        private bool m_MapToolTipsAtLeastOneLayer = false;

        private int m_startX, m_startY;
        private int oldX, oldY;

        //避免搜索多个图层
        public double m_CancelledPromptToBrowse = 0;

        internal bool Title_ShowFullProjectPath = false;
        //浮动刻度条
        internal bool m_FloatingScalebar_Enabled = false;
        internal PictureBox m_FloatingScalebar_PictureBox = null;
        internal string m_FloatingScalebar_ContextMenu_SelectedPosition = "LowerRight";
        internal string m_FloatingScalebar_ContextMenu_SelectedUnit = "";
        internal System.Drawing.Color m_FloatingScalebar_ContextMenu_ForeColor;
        internal System.Drawing.Color m_FloatingScalebar_ContextMenu_BackColor;
        internal ContextMenu m_FloatingScalebar_ContextMenu; //后续可以改为ContextMenuStrip
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_UL;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_UR;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_LL;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_LR;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_FC;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_BC;
        internal System.Windows.Forms.MenuItem m_FloatingScalebar_ContextMenu_CU;

        /// <summary>
        /// 存储shapefile的填充方式
        /// key-layerHandle，value-Interfaces.ShapefileFillStippleScheme
        /// </summary>
        public Dictionary<int, Interfaces.ShapefileFillStippleScheme> m_FillStippleSchemes;
        #endregion

        #region ----------------初始化-----------------

        public MapWinForm()
        {
            InitializeComponent();
            //Logo放这里，可以减少执行文件大小
            this.Icon = MainProgram.Properties.Resources.MapWinGIS;
            m_PluginManager = new PluginTracker();

            InitializeMapsAndLegends();

            m_Project = new MainProgram.Project();
            m_Layers = new MainProgram.Layers();
            m_View = new MainProgram.View();
            m_Menu = new MainProgram.Menus();
            m_LegendPanel = new MainProgram.LegendPanel();
            m_Toolbar = new MainProgram.Toolbar();
            m_HasBeenSaved = true;
            m_Extents = new ArrayList();

            m_StatusBar = new MainProgram.StatusBar();
            this.Controls.Add(m_StatusBar.StatusBar1);//将状态条添加进来
            this.MapMain.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(m_StatusBar.HandleMapMouseMove);
            this.MapMain.ExtentsChanged += new System.EventHandler(m_StatusBar.HandleExtentsChanged);
            this.m_Project.ProjectionChanged += new Interfaces.ProjectionChangedDelegate(m_StatusBar.HandleProjectionChanged);

            
            m_PreviewMap = new PreviewMap();
            m_Reports = new MainProgram.Reports();
            m_UIPanel = new MainProgram.UIPanel();
            PreviewMap.LocatorBoxColor = System.Drawing.Color.Red;
            m_Labels = new LabelClass();
            m_AutoVis = new DynamicVisibilityClass();
            m_UserInteraction = new UserInteraction();
            m_CurrentExtent = -1;
            m_IsManualExtentsChange = false;

            m_HandleFileDrop = true;
            m_GroupHandle = -1;

            MapToolTipTimer = new System.Windows.Forms.Timer();
            MapToolTipTimer.Interval = 1000;
            this.MapToolTipTimer.Tick += new EventHandler(this.MapToolTipTimer_Tick);

            m_FloatingScalebar_ContextMenu_ForeColor = System.Drawing.Color.Black;
            m_FloatingScalebar_ContextMenu_BackColor = System.Drawing.Color.White;

            m_FillStippleSchemes = new Dictionary<int, Interfaces.ShapefileFillStippleScheme>();
        }

        /// <summary>
        /// 初始化docking panels
        /// </summary>
        public void InitializeMapsAndLegends()
        {
            this.Legend = new LegendControl.Legend();
            this.Legend.Map = (MapWinGIS.Map)(MapMain.GetOcx());
            this.Legend.BackColor = System.Drawing.Color.White;
            this.Legend.Dock = DockStyle.Fill;
            this.Legend.Location = new System.Drawing.Point(0, 50);
            this.Legend.Name = resources.GetString("LegendName_Text");
            this.Legend.SelectedColor = System.Drawing.Color.FromArgb((byte)(240), (byte)(240), (byte)(240));
            this.Legend.SelectedLayer = -1;
            this.Legend.GroupMouseDown +=new LegendControl.GroupMouseDown(Legend_GroupMouseDown);
            this.Legend.LayerMouseDown +=new LegendControl.LayerMouseDown(Legend_LayerMouseDown);
            this.Legend.LegendClick +=new LegendControl.LegendClick(Legend_LegendClick);
            this.Legend.GroupDoubleClick +=new LegendControl.GroupDoubleClick(Legend_GroupDoubleClick);
            this.Legend.LayerColorboxClicked +=new LegendControl.LayerColorboxClicked(Legend_LayerColorboxClicked);
            this.Legend.LayerCategoryClicked +=new LegendControl.LayerCategoryClicked(Legend_LayerCategoryClicked);
            this.Legend.GroupRemoved +=new LegendControl.GroupRemoved(Legend_GroupRemoved);

            dckPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            dckPanel.Parent = panel1;
            dckPanel.Dock = DockStyle.Fill;
            dckPanel.BringToFront();
            dckPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow; //DockingSdi

            //将MapWinGISDock.config配置文件的默认位置改为..\Config\MapWinGISDock.config
            string dockConfigFile = System.IO.Path.Combine(XmlProjectFile.GetApplicationDataDir(), "MapWinGISDock.config");

            //如果找不到或不存在配置文件，尝试重新存储默认dock配置到指定目录中
            if (!System.IO.File.Exists(dockConfigFile))
            {
                System.IO.BinaryReader sr = new System.IO.BinaryReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MapWinGIS.MainProgram.Resources.MapWinGISDockDefault.config"), System.Text.Encoding.GetEncoding("UTF-16"));
                System.IO.BinaryWriter sw = new System.IO.BinaryWriter(new System.IO.FileStream(dockConfigFile, System.IO.FileMode.Create), System.Text.Encoding.GetEncoding("UTF-16"));
                char[] buf = new char[1025]; //字符串结尾默认有'\0',所以1025
                int n = 1;
                while (n > 0)
                {
                    n = sr.Read(buf, 0, 512);
                    sw.Write(buf, 0, n);
                }
                sw.Close();
                sr.Close();
            }

            //是否需要读取dock windows的默认配置
            bool needDefaultConfig = true;

            if (System.IO.File.Exists(dockConfigFile))
            {

                try
                {
                    dckPanel.LoadFromXml(dockConfigFile, new WeifenLuo.WinFormsUI.Docking.DeserializeDockContent(BuildDockContent));
                    needDefaultConfig = false;//成功读取配置，下面就不需要读取
                }
                catch
                {
                    MapWinGIS.Utility.Logger.Dbg(dockConfigFile + " 文件毁坏，请使用默认配置");
                }
            }

            if (needDefaultConfig)
            {
                try
                {
                    CreateLegendPanel();
                    CreatePreviewPanel();
                    CreateMapPanel();
                }
                catch (Exception e)
                {
                    MapWinGIS.Utility.Logger.Dbg("dock windows设置错误: " + e.Message);
                }
            }       
        }

        /// <summary>
        /// 创建DockContent窗体
        /// </summary>
        private WeifenLuo.WinFormsUI.Docking.IDockContent BuildDockContent(string persistString)
        {
            switch (persistString)
            {
                case "mwDockPanel_Legend":
                    return CreateLegendPanel();
                case "mwDockPanel_Preview Map":
                    return CreatePreviewPanel();
                case "mwDockPanel_Map View":
                    return CreateMapPanel();
                case "MapWinGIS.LegendEditorForm":
                    return null; //不是直接显示的，不能再这里创建
                default:
                    return null;
            }
        }

        public WeifenLuo.WinFormsUI.Docking.DockContent CreateLegendPanel()
        {

            m_legendTabControl = new TabControl();
            m_legendTabControl.TabPages.Add(new TabPage());
            m_legendTabControl.TabPages.Add(new TabPage());
            m_legendTabControl.TabPages[0].Text = "图层";
            m_legendTabControl.TabPages[1].Text = "工具箱";
            m_legendTabControl.Dock = DockStyle.Fill;

            // legend
            Legend.Dock = DockStyle.Fill;
            m_legendTabControl.TabPages[0].Controls.Add(Legend);//将legend放入第一个（图层）选项卡

            // ------gis toolbox 涉及到MapWindow.Controls，故注释---------
            //m_GisToolbox = new MapWindow.Controls.GisToolbox.GisToolbox();
            //m_GisToolbox.Dock = DockStyle.Fill;

            //m_GisToolbox.SplitterDistance = m_GisToolbox.Height * 0.8;
            //m_legendTabControl.TabPages[1].Controls.Add(m_GisToolbox);

            // 创建一个legend panel
            legendPanel = new MWDockPanel("Legend");
            legendPanel.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;//停靠方式
            legendPanel.Controls.Add(m_legendTabControl);
            
            //添加图标
            legendPanel.Icon = MapWinGIS.MainProgram.GlobalResource.formlegend;
            legendPanel.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DockedPanelClosing);
            return legendPanel;
        }

        public WeifenLuo.WinFormsUI.Docking.DockContent CreateMapPanel()
        {
            mapPanel = new MWDockPanel("Map View");
            mapPanel.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;//指示窗体显示的初始位置
            mapPanel.Controls.Add(MapMain);

            ((System.ComponentModel.ISupportInitialize)MapMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapMain).EndInit();

            MapMain.CursorMode = tkCursorMode.cmNone;
            MapMain.DoubleBuffer = true;
            MapMain.SendMouseDown = true;
            MapMain.SendMouseMove = true;
            MapMain.SendMouseUp = true;
            MapMain.SendSelectBoxDrag = false;
            MapMain.SendSelectBoxFinal = true;

            MapMain.ShowVersionNumber = false;

            MapMain.ShowRedrawTime = false;

            Legend.Map = (MapWinGIS.Map)MapMain.GetOcx();

            mapPanel.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DockedPanelClosing);
            return mapPanel;
        }

        public WeifenLuo.WinFormsUI.Docking.DockContent CreatePreviewPanel()
        {

            previewPanel = new MWDockPanel("Preview Map");
            previewPanel.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide;
            previewPanel.Controls.Add(MapPreview);

            ((System.ComponentModel.ISupportInitialize)(this.MapPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPreview)).EndInit();
            //初始化preview map设置，并强制重绘
            MapPreview.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            MapPreview.MapCursor = MapWinGIS.tkCursor.crsrArrow;
            MapPreview.SendMouseDown = true;
            MapPreview.SendMouseMove = true;
            MapPreview.SendMouseUp = true;
            MapPreview.SendSelectBoxDrag = false;
            MapPreview.SendSelectBoxFinal = false;
            MapPreview.MouseWheelSpeed = 1.0;
            MapPreview.DoubleBuffer = true;

            MapPreview.MapResizeBehavior = MapWinGIS.tkResizeBehavior.rbClassic;
            previewPanel.Icon = new System.Drawing.Icon(this.GetType().Assembly.GetManifestResourceStream("MapWinGIS.MainProgram.Resources.MapPanel.ico"));
            //previewPanel.Icon = MapWinGIS.MainProgram.GlobalResource.MapPanel;
            MapPreview.Dock = DockStyle.Fill;

            //隐藏
            previewPanel.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DockedPanelClosing);
            return previewPanel;
        }

        private void DockedPanelClosing(object sender, FormClosingEventArgs e)
        {
            MWDockPanel mwdpSender = (MWDockPanel)sender;
            // Added try catch, if the menu has been deleted by a plug-in you get an error
            try
            {
                MapWinGIS.Utility.Logger.Dbg("sender.text: " + mwdpSender.Text);

                if (mwdpSender.Text == "Map View")
                {
                    e.Cancel = true;
                    MessageBox.Show("请不要关闭主窗口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //不允许关闭地图（MapMain）.
                }

                if (mwdpSender.Text == "Legend" || mwdpSender.Text == "Preview Map")
                {
                    // 不关闭，隐藏
                    MWDockPanel w = mwdpSender;
                    w.Hide();
                    e.Cancel = true;

                    if (mwdpSender.Text == "Legend")
                    {
                        m_Menu["mnuLegendVisible"].Checked = false;
                    }
                    else if (mwdpSender.Text == "Preview Map")
                    {
                        m_Menu["mnuPreviewVisible"].Checked = false;
                    }
                }
            }
            catch
            {
            }
        }

        public void UpdateLegendPanel(bool legendVisible)//显示隐藏Legend，并同步菜单上的显示
        {  
            if (legendVisible)
            {
                legendPanel.Show(dckPanel);
            }
            else
            {
                legendPanel.Hide();
            }
            if (m_Menu["mnuLegendVisible"] != null)
            {
                m_Menu["mnuLegendVisible"].Checked = legendVisible;
            }
        }

        public void UpdatePreviewPanel(bool previewVisible)//显示隐藏MapPreview，并同步菜单上的显示
        {
            if (previewPanel != null)
            {
                if (previewVisible)
                {
                    previewPanel.Show(dckPanel);
                }
                else
                {
                    previewPanel.Hide();
                }
                if (m_Menu["mnuPreviewVisible"] != null)
                {
                    m_Menu["mnuPreviewVisible"].Checked = previewVisible;
                }
            }
            else
            {
                if (m_Menu["mnuPreviewVisible"] != null)
                {
                    m_Menu["mnuPreviewVisible"].Checked = false;
                }
            }
        }

        /// <summary>
        /// 加载保存工具条设置（location，position，是否显示文本）
        /// </summary>
        internal void LoadToolStripSettings(ToolStripContainer toolStripContainer)//加载toolstrip设置
        {

            // 加载toolstrip设置
            ToolStripManager.LoadSettings(this);

            //存储所有我们需要加载的toolstrip
            IList<ToolStrip> toolstrips = new List<ToolStrip>();

            // 获取ToolStripManager的设置的同时也获取自定义设置
            foreach (Control panel in toolStripContainer.Controls)//迭代toolStripContainer中的元素
            {
                //找到toolstripPanel
                if (panel is ToolStripContentPanel)
                {
                    // 跳过，迭代下一个
                    continue;
                }

                if (!(panel is ToolStripPanel))
                {
                    // 跳过，迭代下一个
                    continue;
                }

                ToolStripPanel myToolstripPanel = panel as ToolStripPanel;
                if (myToolstripPanel == null)//panel不是ToolStripPanel
                {
                    continue;
                }

                if (!myToolstripPanel.HasChildren)//toolstripPanel没有子控件
                {
                    continue;
                }

                foreach (Control myControl in myToolstripPanel.Controls)//迭代ToolStripPanel中的toolstrip控件
                {
                    if (!(myControl is ToolStrip))
                    {
                        //不是容器，迭代下一个
                        continue;
                    }

                    ToolStrip myToolstrip = myControl as ToolStrip;
                    if (myToolstrip == null)//不是ToolStrip类型
                    {
                        continue;
                    }
                    if (myToolstrip.Name == "MenuStrip1")//是菜单
                    {
                        continue;
                    }

                    ToolStripSettings toolStripSettings = new ToolStripSettings(myToolstrip.Name);
                    if (toolStripSettings.Location.X == -1)//不存在用户配置文件
                    {
                        //设置存在于TopToolStripPanel中的toolstrips到一个默认的位置
                        SetToolStripsManually(toolStripContainer);
                        return;
                    }

                    SetDisplayStyle(myToolstrip, toolStripSettings.DisplayStyle);

                    //由于ToolStripPanelName可能被重置为系统值。所以，toolstrips可能不是存储在正确的panel中，修正
                    string toolStripPanelName = toolStripSettings.ToolStripPanelName;
                    if (toolStripPanelName.EndsWith(".Top"))
                    {
                        if (!(toolStripContainer.TopToolStripPanel.Controls.Contains(myToolstrip)))
                        {
                            toolStripContainer.TopToolStripPanel.Join(myToolstrip);
                        }
                    }
                    if (toolStripPanelName.EndsWith(".Bottom"))
                    {
                        if (!(toolStripContainer.BottomToolStripPanel.Controls.Contains(myToolstrip)))
                        {
                            toolStripContainer.BottomToolStripPanel.Join(myToolstrip);
                        }
                    }
                    if (toolStripPanelName.EndsWith(".Left"))
                    {
                        if (!(toolStripContainer.LeftToolStripPanel.Controls.Contains(myToolstrip)))
                        {
                            toolStripContainer.LeftToolStripPanel.Join(myToolstrip);
                        }
                    }
                    if (toolStripPanelName.EndsWith(".Right"))
                    {
                        if (!(toolStripContainer.RightToolStripPanel.Controls.Contains(myToolstrip)))
                        {
                            toolStripContainer.RightToolStripPanel.Join(myToolstrip);
                        }
                    }


                    //重新设置toolstrip的位置
                    myToolstrip.Location = toolStripSettings.Location;

                    // 将toolstrip添加到IList泛型集合中，一起设置位置并防止toolstrip随即摆放在toolstripPanel中
                    if (toolStripSettings.ToolStripPanelName.Contains(".Top"))
                    {
                        toolstrips.Add(myToolstrip);
                    }
                }
            }

            // 重新设置toolstrip的位置，防止随机摆放
            RelocateToolStrips(toolstrips, toolStripContainer);

        }

        private void SetDisplayStyle(ToolStrip myToolstrip, string displayStyle)//设置按钮显示样式，ImageAndText或Image
        {
            if (string.IsNullOrEmpty(displayStyle))
            {
                //转为默认样式
                displayStyle = "ImageAndText";
            }

            // 将所有的toolstrips的样式设置为指定样式
            int count = myToolstrip.Items.Count;
            for (int i = 0; i < count; i++)
            {
                myToolstrip.Tag = displayStyle;
                myToolstrip.Items[i].DisplayStyle = (displayStyle == "ImageAndText" ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image);
            }
        }

        /// <summary>
        /// 先清除再加载，重新将存在的toolstrip加载到TopToolStripPanel
        /// </summary>
        private void SetToolStripsManually(ToolStripContainer toolStripContainer)
        {

            //移除程序本身的toolstrips。不能通过Controls.Clear，因为存在插件toolstrip
            toolStripContainer.TopToolStripPanel.Controls.Remove(this.tlbMain);
            toolStripContainer.TopToolStripPanel.Controls.Remove(this.tlbLayers);
            toolStripContainer.TopToolStripPanel.Controls.Remove(this.tlbStandard);
            toolStripContainer.TopToolStripPanel.Controls.Remove(this.tlbZoom);

            //将插件的toolstrips保存起来
            Stack<Control> pluginsToolstrips = new Stack<Control>();
            foreach (Control ctrl in toolStripContainer.TopToolStripPanel.Controls)
            {
                pluginsToolstrips.Push(ctrl);
            }
            // 移除其他的toolstrips:
            toolStripContainer.TopToolStripPanel.Controls.Clear();

            // 第一行加载程序自身的toolstrips
            toolStripContainer.TopToolStripPanel.Join(this.tlbMain, 0);
            toolStripContainer.TopToolStripPanel.Join(this.tlbLayers, 0);
            toolStripContainer.TopToolStripPanel.Join(this.tlbStandard, 0);

            //第二行，加载插件（plug-ins）的toolstrips
            while (pluginsToolstrips.Count > 0)
            {
                toolStripContainer.TopToolStripPanel.Join((ToolStrip)(pluginsToolstrips.Pop()), 1);
            }

            //通过最后加载，确保zoom toolstrip在第二行的第一个
            toolStripContainer.TopToolStripPanel.Join(this.tlbZoom, 1);
        }

        private void RelocateToolStrips(IList<ToolStrip> toolStrips, ToolStripContainer toolStripContainer)
        {

            System.Collections.Generic.IDictionary<ToolStrip, int> toolstripsDic = new Dictionary<ToolStrip, int>();

            //1.先保存这些toolstrip，采用rowId确保它们正确摆放
            foreach (ToolStrip myToolstrip in toolStrips)
            {
                //设置rowId为最后一行（Rows是从0开始的）
                int rowID = toolStripContainer.TopToolStripPanel.Rows.Length - 1;

                ToolStripSettings toolStripSettings = new ToolStripSettings(myToolstrip.Name);//保存toolstrip设置对象

                // 获取RowId
                int length = toolStripContainer.TopToolStripPanel.Rows.Length;
                for (int i = 0; i < length; i++)
                {
                    if (toolStripContainer.TopToolStripPanel.Rows[i].Bounds.Y >= toolStripSettings.Location.Y)
                    {
                        rowID = i;
                        break;
                    }
                }

                Debug.WriteLine(string.Format("{0} 在第 {1}行", myToolstrip.Name, rowID));
                toolstripsDic.Add(myToolstrip, rowID);
            }

            //2.移除toptoolstripPanel中的toolstrip
            foreach (ToolStrip myToolstrip in toolStrips)
            {
                toolStripContainer.TopToolStripPanel.Controls.Remove(myToolstrip);
            }

            int firstValue = 0;
            int currentRowID = 0;
            System.Collections.Generic.IDictionary<ToolStrip, int> tmp = new Dictionary<ToolStrip, int>();
            // 3.rowId排序
            foreach (KeyValuePair<ToolStrip, int> pair in toolstripsDic.OrderBy(m => m.Value))
            {
                ToolStripSettings toolStripSettings = new ToolStripSettings(pair.Key.Name);
                //pair.value就是 rowID:
                if (firstValue == pair.Value)//第一排
                {
                    tmp.Add(pair.Key, toolStripSettings.Location.X);
                    Debug.WriteLine(string.Format("添加到tmp的是: {0} X 坐标是：{1}", pair.Key.Name, toolStripSettings.Location.X));
                }
                else//不是第一排
                {
                    foreach (KeyValuePair<ToolStrip, int> keyValue in tmp.OrderByDescending(m => m.Value))
                    {
                        //添加到最前面
                        toolStripContainer.TopToolStripPanel.Join(keyValue.Key, currentRowID);
                        Debug.WriteLine(string.Format("添加 {0} 到第{1}行", keyValue.Key.Name, currentRowID));
                    }

                    currentRowID++;
                    tmp.Clear();
                    tmp.Add(pair.Key, toolStripSettings.Location.X);
                    Debug.WriteLine(string.Format("添加到tmp的是 : {0}  X 坐标是：{1}", pair.Key.Name, toolStripSettings.Location.X));
                    firstValue = pair.Value;
                }
            }

            //4.添加最后一排
            if (tmp.Count > 0)
            {
                foreach (KeyValuePair<ToolStrip, int> keyValue in tmp.OrderByDescending(m => m.Value))
                {
                    toolStripContainer.TopToolStripPanel.Join(keyValue.Key, currentRowID);
                    Debug.WriteLine(string.Format("将{0} 添加到第 {1} 行", keyValue.Key.Name, currentRowID));
                }
                tmp.Clear();
            }
            //重新设置toolstrip的显示样式，确保正确
            foreach (ToolStrip myToolstrip in toolStrips)
            {
                ToolStripSettings toolStripSettings = new ToolStripSettings(myToolstrip.Name);

                SetDisplayStyle(myToolstrip, toolStripSettings.DisplayStyle);
            }

        }
        /// <summary>
        /// 保存Toolbar设置
        /// </summary>
        private void SaveToolStripSettings(ToolStripContainer toolStripContainer) //保存toolstrip设置
        {
            ToolStripManager.SaveSettings(this);

            //保存按钮的显示样式
            foreach (Control panel in toolStripContainer.Controls)
            {
                if (panel is ToolStripContentPanel) //跳过
                {
                    continue;
                }

                if (!(panel is ToolStripPanel))//不是toolstripPanel，也跳过
                {
                    continue;
                }

                ToolStripPanel myToolstripPanel = panel as ToolStripPanel;
                if (myToolstripPanel == null)
                {
                    continue;
                }

                if (!myToolstripPanel.HasChildren)
                {
                    continue;
                }

                foreach (Control myControl in myToolstripPanel.Controls)
                {
                    if (!(myControl is ToolStrip))
                    {
                        continue;
                    }

                    string displayStyle = "ImageAndText";
                    ToolStrip myToolstrip = myControl as ToolStrip;
                    if (myToolstrip == null)
                    {
                        continue;
                    }
                    if (myToolstrip.Name == "MenuStrip1")//MenuStrip也是Toolstrip类型
                    {
                        continue;
                    }

                    if (myToolstrip.Tag != null)
                    {
                        displayStyle = myToolstrip.Tag.ToString();
                    }

                    ToolStripSettings toolStripSettings = new ToolStripSettings(myToolstrip.Name);
                    toolStripSettings.DisplayStyle = displayStyle;

                    toolStripSettings.Save();
                }
            }
        }

        public void CreateMenus() //创建菜单
        {
            //创建主菜单
            this.SetUpMenus();

            //如果存在本地帮助文件，就显示本地帮助菜单项。
            //m_Menu["mnuOfflineDocs"].Visible = System.IO.File.Exists(Program.appInfo.HelpFilePath);

            // 更新按钮
            UpdateButtons();

            // 保存所有菜单和工具条上的按钮
            XmlProjectFile.SaveMainToolbarButtons();

            //默认鼠标在地图上的操作
            //tbbZoomIn.Checked = true;
        }

        public void SetUpMenus() //创建主菜单
        {
            object Nil = null; //为了指定方法

            //创建 文件（File）菜单
            m_Menu.AddMenu("mnuFile", Nil, resources.GetString("mnuProject_Text"));
            m_Menu.AddMenu("mnuNew", "mnuFile", resources.GetObject("project"), resources.GetString("mnuNew_Text"));//新建
            m_Menu.AddMenu("mnuFileBreak0", "mnuFile", Nil, "-");//分界线
            m_Menu.AddMenu("mnuOpen", "mnuFile", resources.GetObject("open"), resources.GetString("mnuOpen_Text"));//打开
            m_Menu.AddMenu("mnuOpenProjectIntoGroup", "mnuFile", resources.GetObject("group_open"), resources.GetString("mnuOpenProjectIntoGroup_Text"));//新组打开
            m_Menu.AddMenu("mnuFileBreak1", "mnuFile", Nil, "-");//分界线
            m_Menu.AddMenu("mnuSave", "mnuFile", resources.GetObject("saveNew"), resources.GetString("mnuSave_Text"));//保存
            m_Menu.AddMenu("mnuSaveAs", "mnuFile", resources.GetObject("save_as"), resources.GetString("mnuSaveAs_Text"));//另存为
            m_Menu.AddMenu("mnuFileBreak2", "mnuFile", Nil, "-");//分界线
            m_Menu.AddMenu("mnuPrint", "mnuFile", resources.GetObject("print"), resources.GetString("mnuPrint_Text"));//打印
            m_Menu.AddMenu("mnuFileBreak3", "mnuFile", Nil, "-");//分界线
            m_Menu.AddMenu("mnuRecentProjects", "mnuFile", resources.GetObject("recent_maps"), resources.GetString("mnuRecentProjects_Text"));//近期项目
            m_Menu.AddMenu("mnuExport", "mnuFile", resources.GetObject("map_export"), resources.GetString("mnuExport_Text"));//图像输出
            //HQ图像没有
            m_Menu.AddMenu("mnuSaveMapImage", "mnuExport", resources.GetObject("export_map"), resources.GetString("mnuSaveMapImage_Text"));//地图
            m_Menu.AddMenu("mnuSaveGeorefMapImage", "mnuExport", resources.GetObject("export_georeferenced"), resources.GetString("mnuSaveGeorefMapImage_Text"));//地理引用的地图
            m_Menu.AddMenu("mnuSaveScaleBar", "mnuExport", resources.GetObject("scale_bar"), resources.GetString("mnuSaveScaleBar_Text"));//范围条
            m_Menu.AddMenu("mnuSaveNorthArrow", "mnuExport", resources.GetObject("northArrow"), resources.GetString("mnuSaveNorthArrow_Text"));//方向针
            m_Menu.AddMenu("mnuSaveLegend", "mnuExport", resources.GetObject("legend"), resources.GetString("mnuSaveLegend_Text"));
            m_Menu.AddMenu("mnuFileBreak4", "mnuFile", Nil, "-");//分界线

            m_Menu.AddMenu("mnuProjectSettings", "mnuFile", resources.GetObject("project_settings"), resources.GetString("mnuProjectSettings_Text"));//项目设置
            m_Menu.AddMenu("mnuFileBreak5", "mnuFile", Nil, "-");//分界线
            m_Menu.AddMenu("mnuExit", "mnuFile", resources.GetObject("quit"), resources.GetString("mnuExit_Text"));//退出

            // 创建图层（Layer）菜单
            m_Menu.AddMenu("mnuLayer", Nil, resources.GetString("mnuLayer_Text"));//图层
            m_Menu.AddMenu("mnuAddLayer", "mnuLayer", resources.GetObject("layer_add"), resources.GetString("mnuAddLayer_Text"));
            m_Menu.AddMenu("mnuAddECWPLayer", "mnuLayer", resources.GetObject("layer_add_ecwp"), resources.GetString("mnuAddECWPLayer_Text")); //OK
            m_Menu.AddMenu("mnuRemoveLayer", "mnuLayer", resources.GetObject("layer_remove"), resources.GetString("mnuRemoveLayer_Text"));
            m_Menu.AddMenu("mnuClearLayer", "mnuLayer", resources.GetObject("remove_all_layers"), resources.GetString("mnuClearLayer_Text"));
            m_Menu.AddMenu("mnuLayerBreak1", "mnuLayer", Nil, "-");//分界线
            m_Menu.AddMenu("mnuLayerLabels", "mnuLayer", resources.GetObject("label_properties"), resources.GetString("mnuLayerLabels_Text")); //OK
            m_Menu.AddMenu("mnuLayerCharts", "mnuLayer", resources.GetObject("charts_properties"), resources.GetString("mnuLayerCharts_Text"));//图表
            m_Menu.AddMenu("mnuLayerAttributeTable", "mnuLayer", resources.GetObject("table_editor"), resources.GetString("mnuLayerAttributeTable_Text"));//属性数据表
            m_Menu.AddMenu("mnuLayerCategories", "mnuLayer", resources.GetObject("layer_categories"), resources.GetString("mnuLayerCategories_Text"));
            m_Menu.AddMenu("mnuOptionsManager", "mnuLayer", resources.GetObject("layer_symbology"), resources.GetString("mnuOptionsManager_Text"));
            m_Menu.AddMenu("mnuLayerRelabel", "mnuLayer", resources.GetObject("label_reload"), resources.GetString("mnuLayerRelabel_Text"));
            m_Menu.AddMenu("mnuLayerBreak2", "mnuLayer", Nil, "-");//分界线
            m_Menu.AddMenu("mnuQueryLayer", "mnuLayer", resources.GetObject("layer_query"), resources.GetString("mnuQueryLayer_Text"));//创建查询
            m_Menu.AddMenu("mnuClearSelectedShapes", "mnuLayer", resources.GetObject("deselect"), resources.GetString("mnuClearSelectedShapes_Text")).Enabled = false;//清空当前层选择的shaps
            m_Menu.AddMenu("mnuLayerBreak3", "mnuLayer", Nil, "-");//分界线
            m_Menu.AddMenu("mnuLayerProperties", "mnuLayer", resources.GetObject("layer_properties"), resources.GetString("mnuLayerProperties_Text"));//图层属性

            // 视图菜单
            m_Menu.AddMenu("mnuView", Nil, resources.GetString("mnuView_Text"));
            //legend和preview窗口
            m_Menu.AddMenu("mnuRestoreMenu", "mnuView", resources.GetObject("panels"), resources.GetString("mnuPanels_Text"));
            m_Menu.AddMenu("mnuLegendVisible", "mnuRestoreMenu", Nil, resources.GetString("mnuShowLegend_Text")).Checked = legendPanel == null ? false : true;
            m_Menu.AddMenu("mnuPreviewVisible", "mnuRestoreMenu", Nil, resources.GetString("mnuShowPreviewMap_Text")).Checked = previewPanel == null ? false : true;
            //新添加语言选择菜单
            m_Menu.AddMenu("mnuLanguage", "mnuView", resources.GetObject("language"), resources.GetString("mnuLanguages_Text"));
            m_Menu.AddMenu("mnuEnglish", "mnuLanguage", Nil, resources.GetString("mnuEnglish_Text"));
            m_Menu.AddMenu("mnuChinese", "mnuLanguage", Nil, resources.GetString("mnuChinese_Text"));

            m_Menu.AddMenu("mnuViewBreak2", "mnuView", Nil, "-");//分界线
            //范围条
            m_Menu.AddMenu("mnuSetScale", "mnuView", resources.GetObject("set_map_scale"), resources.GetString("mnuSetScale_Text"));
            m_Menu.AddMenu("mnuShowScaleBar", "mnuView", resources.GetObject("scale_bar_add"), resources.GetString("mnuShowScaleBar_Text"));
            // 复制
            m_Menu.AddMenu("mnuViewBreak5", "mnuView", Nil, "-");//分界线
            m_Menu.AddMenu("mnuCopy", "mnuView", resources.GetObject("imgCopy"), resources.GetString("mnuCopy_Text"));
            m_Menu.AddMenu("mnuCopyMap", "mnuCopy", resources.GetObject("export_map"), resources.GetString("mnuCopyMap_Text"));
            m_Menu.AddMenu("mnuCopyLegend", "mnuCopy", resources.GetObject("legend"), resources.GetString("mnuCopyLegend_Text"));//图符
            m_Menu.AddMenu("mnuCopyScaleBar", "mnuCopy", resources.GetObject("scale_bar"), resources.GetString("mnuCopyScaleBar_Text"));
            m_Menu.AddMenu("mnuCopyNorthArrow", "mnuCopy", resources.GetObject("northArrow"), resources.GetString("mnuCopyNorthArrow_Text"));
            m_Menu.AddMenu("mnuViewBreak3", "mnuView", Nil, "-");//分界线
            //Zoom
            m_Menu.AddMenu("mnuZoomIn", "mnuView", resources.GetObject("zoom_inNew"), resources.GetString("mnuZoomIn_Text"));
            m_Menu.AddMenu("mnuZoomOut", "mnuView", resources.GetObject("zoom_outNew"), resources.GetString("mnuZoomOut_Text"));
            m_Menu.AddMenu("mnuZoomToFullExtents", "mnuView", resources.GetObject("zoom_extentNew"), resources.GetString("mnuZoomToFullExtents_Text"));
            m_Menu.AddMenu("mnuZoomToPreviewExtents", "mnuView", resources.GetObject("imgMapExtents"), resources.GetString("mnuZoomToPreviewExtents_Text")).Enabled = false;
            m_Menu.AddMenu("mnuViewBreak4", "mnuView", Nil, "-");//分界线
            //前进后退
            m_Menu.AddMenu("mnuPreviousZoom", "mnuView", resources.GetObject("zoom_lastNew"), resources.GetString("mnuPreviousZoom_Text"));
            m_Menu.AddMenu("mnuNextZoom", "mnuView", resources.GetObject("zoom_nextNew"), resources.GetString("mnuNextZoom_Text"));
            m_Menu.AddMenu("mnuViewBreak6", "mnuView", Nil, "-");//分界线
            m_Menu.AddMenu("mnuClearAllSelection", "mnuView", resources.GetObject("deselect_all"), resources.GetString("mnuClearAllSelection_Text"));
            m_Menu.AddMenu("mnuPreviewSep", "mnuView", Nil, "-");//分界线
            //菜单栏上的预览地图（PreviewMap）菜单
            m_Menu.AddMenu("mnuPreview", "mnuView", resources.GetObject("imgMapPreview"), resources.GetString("mnuPreview_Text"));
            m_Menu.AddMenu("mnuUpdatePreviewFull", "mnuPreview", resources.GetObject("imgMapExtents"), resources.GetString("mnuUpdatePreviewFull_Text"));
            m_Menu.AddMenu("mnuUpdatePreviewCurr", "mnuPreview", resources.GetObject("imgMapScale"), resources.GetString("mnuUpdatePreviewCurr_Text"));
            m_Menu.AddMenu("mnuClearPreview", "mnuPreview", resources.GetObject("deselect_all"), resources.GetString("mnuClearPreview_Text"));

            //书签菜单
            m_Menu.AddMenu("mnuBookmarks", Nil, resources.GetString("mnuBookmarks_Text"));
            m_Menu.AddMenu("mnuBookmarkAdd", "mnuBookmarks", resources.GetObject("imgBookmarkAdd"), resources.GetString("mnuBookmarkAdd_Text"));
            m_Menu.AddMenu("mnuBookmarkView", "mnuBookmarks", resources.GetObject("bookmarks_view"), resources.GetString("mnuBookmarkView_Text"));
            m_Menu.AddMenu("mnuBookmarksBreak1", "mnuBookmarks", Nil, "-");//分界线
            m_Menu.AddMenu("mnuBookmarksManager", "mnuBookmarks", resources.GetObject("bookmark_manager"), resources.GetString("mnuBookmarksManager_Text"));

            //插件菜单
            m_Menu.AddMenu("mnuPlugins", Nil, resources.GetString("mnuPlugins_Text"));
            m_Menu.AddMenu("mnuEditPlugins", "mnuPlugins", resources.GetObject("imgPluginEdit"), resources.GetString("mnuEditPlugins_Text"));
            m_Menu.AddMenu("mnuScript", "mnuPlugins", resources.GetObject("imgScripts"), resources.GetString("mnuScript_Text"));
            m_Menu.AddMenu("mnuPluginsBreak1", "mnuPlugins", Nil, "-");

            //帮助菜单
            m_Menu.AddMenu("mnuHelp", Nil, resources.GetString("mnuHelp_Text"));
            //m_Menu.AddMenu("mnuContents", "mnuHelp", Nil, resources.GetString("mnuContents_Text")).Visible=false;

            //下面条目显示或隐藏着
            m_Menu.AddMenu("mnuTutorials", "mnuHelp", resources.GetObject("tutorials"), resources.GetString("mnuTutorials_Text"));//新手指导
            m_Menu.AddMenu("mnuOnlineDocs", "mnuHelp", resources.GetObject("documentation"), resources.GetString("mnuOnlineDocs_Text"));
            m_Menu.AddMenu("mnuOfflineDocs", "mnuHelp", resources.GetObject("imgHelp"), resources.GetString("mnuOfflineDocs_Text")).Visible = true;
            m_Menu.AddMenu("mnuBugReport", "mnuHelp", resources.GetObject("mantis"), resources.GetString("mnuBugReport_Text"));
            m_Menu.AddMenu("mnuPluginUpLoading", "mnuHelp", resources.GetObject("paypal"), resources.GetString("mnuPluginUpLoading_Text"));//上传插件
            m_Menu.AddMenu("mnuCheckForUpdates", "mnuHelp", resources.GetObject("imgUpdate"), resources.GetString("mnuCheckUpdates_Text"));
            m_Menu.AddMenu("mnuHelpBreak1", "mnuHelp", Nil, "-");//分界线
            m_Menu.AddMenu("mnuShortcuts", "mnuHelp", resources.GetObject("imgKeyboard"), resources.GetString("mnuShortcuts_Text"));
            m_Menu.AddMenu("mnuHelpBreak2", "mnuHelp", Nil, "-");//分界线
            m_Menu.AddMenu("mnuWelcomeScreen", "mnuHelp", resources.GetObject("welcome"), resources.GetString("mnuWelcomeScreen_Text"));
            m_Menu.AddMenu("mnuAboutMapWindow", "mnuHelp", resources.GetObject("about"), resources.GetString("mnuAboutMapWindow_Text"));

            //浮动范围条的右键菜单
            m_FloatingScalebar_ContextMenu = new ContextMenu();
            m_FloatingScalebar_ContextMenu_UL = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_UpperLeft_Text"), new System.EventHandler(FloatingScalebar_UpperLeft_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_UL);
            m_FloatingScalebar_ContextMenu_UR = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_UpperRight_Text"), new System.EventHandler(FloatingScalebar_UpperRight_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_UR);
            m_FloatingScalebar_ContextMenu_LL = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_LowerLeft_Text"), new System.EventHandler(FloatingScalebar_LowerLeft_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_LL);
            m_FloatingScalebar_ContextMenu_LR = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_LowerRight_Text"), new System.EventHandler(FloatingScalebar_LowerRight_Click));
            m_FloatingScalebar_ContextMenu_LR.Checked = true;
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_LR);
            m_FloatingScalebar_ContextMenu.MenuItems.Add("-");
            m_FloatingScalebar_ContextMenu_FC = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_ChooseForecolor_Text"), new System.EventHandler(FloatingScalebar_ChooseForecolor_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_FC);
            m_FloatingScalebar_ContextMenu_BC = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_ChooseBackcolor_Text"), new System.EventHandler(FloatingScalebar_ChooseBackcolor_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_BC);
            m_FloatingScalebar_ContextMenu_CU = new System.Windows.Forms.MenuItem(resources.GetString("sbContextMenu_ChangeUnits_Text"), new System.EventHandler(FloatingScalebar_ChangeUnits_Click));
            m_FloatingScalebar_ContextMenu.MenuItems.Add(m_FloatingScalebar_ContextMenu_CU);
        }

        #endregion

        #region  ---------------接口实现----------------

        /// <summary>
        /// 刷新宿主程序地图的显示
        /// </summary>
        public new void Refresh()
        {
            for (int i = 0; i <= Program.frmMain.m_Layers.NumLayers - 1; i++)
            {
                if (!(Program.frmMain.m_Layers[Program.frmMain.m_Layers.GetHandle(i)].FillStippleScheme == null))
                {
                    Program.frmMain.m_Layers[Program.frmMain.m_Layers.GetHandle(i)].HatchingRecalculate();
                }
            }
            Program.frmMain.MapMain.Redraw();

        }

        /// <summary>
        /// 获取最后一条错误信息. 
        /// 任何时候都能设置这个错误信息
        /// </summary>
        public string LastError
        {
            get
            {
                string tStr;

                if (Program.g_error == null)
                {
                    return "";
                }

                tStr = System.String.Copy(Program.g_error);
                Program.g_error = "";
                return tStr;
            }
        }

        /// <summary>
        /// 返回层对象，管理
        /// </summary>
        public MapWinGIS.Interfaces.Layers Layers
        {
            get { return this.m_Layers; }
        }

        /// <summary>
        /// 返回视图（View）对象，处理地图视图
        /// </summary>
        public MapWinGIS.Interfaces.View View
        {
            get { return this.m_View; }
        }

        /// <summary>
        /// 返回菜单对象，管理菜单
        /// </summary>
        public MapWinGIS.Interfaces.Menus Menus
        {
            get { return this.m_Menu; }
        }

        /// <summary>
        /// 返回插件对象，管理插件
        /// </summary>
        public MapWinGIS.Interfaces.Plugins Plugins
        {
            get { return this.m_PluginManager; }
        }

        /// <summary>
        /// 返回地图预览(PreviewMap)对象，管理预览地图
        /// 注意PreviewMap和MapPreview
        /// </summary>
        public MapWinGIS.Interfaces.PreviewMap PreviewMap
        {
            get { return this.m_PreviewMap; }
        }

        /// <summary>
        /// 返回legendPanel对象，管理legend panel
        /// </summary>
        public MapWinGIS.Interfaces.LegendPanel LegendPanel
        {
            get { return this.m_LegendPanel; }
        }

        /// <summary>
        /// 返回状态栏对象，管理状态栏
        /// </summary>
        public MapWinGIS.Interfaces.StatusBar StatusBar
        {
            get { return this.m_StatusBar; }
        }

        /// <summary>
        /// 返回工具条，管理工具条
        /// </summary>
        public MapWinGIS.Interfaces.Toolbar Toolbar
        {
            get { return this.m_Toolbar; }
        }

        /// <summary>
        /// 为方法和属性提供入口，得到一个报告
        /// Provides access to report generation methods and properties. 
        /// </summary>
        public MapWinGIS.Interfaces.Reports Reports
        {
            get { return this.m_Reports; }
        }

        /// <summary>
        /// 为项目和配置文件，提供管理
        /// Provides control over project and configuration files.
        /// </summary>
        public MapWinGIS.Interfaces.Project Project
        {
            get { return this.m_Project; }
        }

        /// <summary>
        /// 为App ，提供控制
        /// Provides control over application-level settings like the app name.
        /// </summary>
        public MapWinGIS.Interfaces.AppInfo ApplicationInfo
        {
            get { return Program.appInfo; }
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="ex">抛出的异常</param>
        public void ShowErrorDialog(System.Exception ex)
        {
            Program.ShowError(ex, "zhongdy@qq.com");
        }

        /// <summary>
        /// 显示错误对话框，并发送到指定的地址
        /// </summary>
        /// <param name="ex">抛出的异常</param>
        /// <param name="SendEmailTo">要发送的地址</param>
        public void ShowErrorDialog(System.Exception ex, string sendEmailTo)
        {
            Program.ShowError(ex, sendEmailTo);
        }

        /// <summary>
        /// 显示在App名字后面的，对话框标题，覆盖默认项目名标题
        /// Sets the dialog title to be displayed after the "AppInfo" name for the main window.
        /// </summary>
        public void SetCustomWindowTitle(string newTitleText)
        {
            CustomWindowTitle = newTitleText;
            SetModified(false); //强制重写标题
        }

        /// <summary>
        /// 显示默认项目名标题 清除自定义窗体标题
        /// Returns dialog title for the main window to the default "project name" title.
        /// </summary>
        public void ClearCustomWindowTitle()
        {
            CustomWindowTitle = "";
        }

        /// <summary>
        /// 指定是否用绝对文件路径
        /// Specify whether the full project path should be specified rather than just filename, in title bar for main window.
        /// </summary>
        public bool DisplayFullProjectPath
        {
            set
            {
                Title_ShowFullProjectPath = value;
                SetModified(false); //强制重写标题
            }
        }

        /// <summary>
        /// 未实现 需要MapWindow.Controls.Projections 
        /// 让用户选择一个投影，并且返回PROJ4格式的代替这个投影
        /// 指定对话框的标题和一个可选择的的默认投影
        /// </summary>
        /// <param name="dialogCaption">The text to be displayed on the dialog, e.g. "Please select a projection."</param>
        /// <param name="defaultProjection">The PROJ4 projection string of the projection to default to, "" for none.</param>
        /// <returns></returns>
        public string GetProjectionFromUser(string dialogCaption, string defaultProjection)
        {
            return null;
        }

        /// <summary>
        /// 在宿主程序的右下角为用户提供panel
        /// </summary>
        public MapWinGIS.Interfaces.UIPanel UIPanel
        {
            get { return this.m_UIPanel; }
        }

        /// <summary>
        /// 用户交互，促使用输入内容
        /// </summary>
        public MapWinGIS.Interfaces.UserInteraction UserInteraction
        {
            get { return this.m_UserInteraction; }
        }

        /// <summary>
        /// MapWinGIS用户控件的高级操作
        /// Returns the underlying MapWinGIS activex control for advanced operations.
        /// </summary>
        public object GetOCX
        {
            get { return this.MapMain; }
        }

        public void RefreshDynamicVisibility()
        {
            m_AutoVis.TestLayerZoomExtents();
        }

        /// <summary>
        /// 未实现，需要MapWindow.Controls.GisToolbox
        /// 返回指向工具盒的对象
        /// </summary>
        public MapWinGIS.Interfaces.IGisToolBox GisToolbox
        {
            get
            {
                return null;
                //return m_GisToolbox;
            }
        }

        /// <summary>
        /// 未实现 MapWindow.Controls.Projections.ProjectionDatabase
        /// 获取投影数据库
        /// </summary>
        public MapWinGIS.Interfaces.IProjectionDatabase ProjectionDatabase
        {
            get { return null; }
        }

        /// <summary>
        /// 与层关联的自定义对象从项目加载时触发的事件
        /// Event raised when state of custom object associated with layer is read from project
        /// </summary>
        public event CustomObjectLoadedDelegate CustomObjectLoaded;

        internal void FireCustomObjectLoaded(int layerHandle, string key, string state, ref bool handled)
        {
            if (CustomObjectLoaded != null)
            {
                CustomObjectLoaded(layerHandle, key, state, ref handled);
            }
        }

        /// <summary>
        /// shapefile层对象的选择发生改变时触发的事件
        /// </summary>
        public event LayerSelectionChangedDelegate LayerSelectionChanged;

        internal void FireLayerSelectionChanged(int layerHandle, ref bool handled)
        {
            if (LayerSelectionChanged != null)
            {
                LayerSelectionChanged(layerHandle, ref handled);
            }
        }

        /// <summary>
        /// 当一个项目有MainProgram加载完毕后触发的事件
        /// </summary>
        public event ProjectLoadedDelegate ProjectLoaded;

        internal void FireProjectLoaded(string projectName, bool errors)
        {
            if (ProjectLoaded != null)
            {
                ProjectLoaded(projectName, errors);
            }
        }

        #endregion

        #region ---------------MainWinForm事件---------
        private void MapWinForm_Shown(object sender, EventArgs e)
        {
            this.UpdateButtons();
        }
        //窗体被激活,将焦点给MapMain
        private void MapWinForm_Activated(object sender, EventArgs e)
        {
            MapMain.Focus();
        }
        //在调整宿主窗体大小时发生，更新scale bar
        private void MapWinForm_Resize(object sender, EventArgs e)
        {
            UpdateFloatingScalebar();
        }
        //关闭窗体时发生（需要手动添加）,保存设置、卸载插件
        void MapWinForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Program.projInfo.SaveConfig(true, true);

            SaveToolStripSettings(this.StripDocker);

            MapWinGIS.Utility.Logger.Dbg("文件对话框的默认目录,AppInfo.DefaultDir: " + Program.appInfo.DefaultDir);

            if (!m_HasBeenSaved || Program.projInfo.Modified)
            {
                if (PromptToSaveProject() == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
            }

            if (m_legendEditor != null)
            {
                m_legendEditor.Close();
            }

            Program.g_SyncPluginMenuDefer = true;
            m_PluginManager.UnloadAll(); // 卸载外部插件
            m_PluginManager.UnloadApplicationPlugins();//卸载程序插件
            this.DialogResult = DialogResult.OK;

        }
       //快捷键
        private bool HandleShortcutKeys(Keys e)
        {
            //采用ProcessCmdKey（处理命令键）方式
            Hashtable keycodes = new Hashtable();
            Hashtable keystates = new Hashtable();
            keycodes.Add("control", 0x11);
            keycodes.Add("shift", 0x10);
            keycodes.Add("ks", 0x53);
            keycodes.Add("ko", 0x4F);
            keycodes.Add("kc", 0x43);
            keycodes.Add("kp", 0x50);
            keycodes.Add("ki", 0x49);
            keycodes.Add("kh", 0x48);
            keycodes.Add("kr", 0x52);
            keycodes.Add("kz", 0x5A);
            keycodes.Add("ky", 0x59);
            keycodes.Add("km", 0x4D);
            keycodes.Add("kprint", 0x2A);
            keycodes.Add("kf4", 0x73);
            keycodes.Add("khome", 0x24);
            keycodes.Add("kinsert", 0x2D);
            keycodes.Add("kdelete", 0x2E);
            keycodes.Add("kpageup", 0x21);
            keycodes.Add("kpagedown", 0x22);
            keycodes.Add("kleftarrow", 0x25);
            keycodes.Add("kuparrow", 0x26);
            keycodes.Add("krightarrow", 0x27);
            keycodes.Add("kdownarrow", 0x28);
            keycodes.Add("kplus", 0xBB);
            keycodes.Add("kminus", 0xBD);
            keycodes.Add("kspacebar", 0x20);
            keycodes.Add("kenter", 0xD);
            keycodes.Add("kbackspace", 0x8);
            //将有效按键添加到keystates中
            foreach (DictionaryEntry item in keycodes)
            {
                if (WinApi32.GetAsyncKeyState((int)item.Value) > 0)
                {
                    keystates.Add(item.Key, true);
                } 
            }
            //keystates不能为空
            if (0 == keystates.Count)
            {
                return false;
            }
            //处理快捷键事件
            if (keystates.Contains("ks") && keystates.Contains("control"))
            {
                DoSave();
                return true;
            }
            else if (keystates.Contains("kbackspace"))
            {
                mnuZoomPrevious.Checked = true;
                HandleZoomButtonClick("tbbZoomPrevious");
                return true;
            }
            else if (keystates.Contains("km") && keystates.Contains("control")) //Move移动地图
            {
                HandleZoomButtonClick("tbbPan");
            }
            else if (keystates.Contains("ko") && keystates.Contains("control"))
            {
                HandleZoomButtonClick("tbbZoomOut");
            }
            else if (keystates.Contains("ki") && keystates.Contains("control"))
            {
                HandleZoomButtonClick("tbbZoomIn");
            }
            else if (keystates.Contains("kr") && keystates.Contains("control"))
            {
                HandleZoomButtonClick("tbbZoomExtent");
            }
            else if (keystates.Contains("kz") && keystates.Contains("control"))
            {
                HandleZoomButtonClick("tbbZoomPrevious");
            }
            else if (keystates.Contains("ky") && keystates.Contains("control"))
            {
                HandleZoomButtonClick("tbbZoomNext");
            }
            else if (keystates.Contains("ki") && keystates.Contains("shift") && keystates.Contains("control"))
            {
                HandleButtonClick("Identify"); //shape的属性框
            }
            else if (keystates.Contains("kh") && keystates.Contains("control"))
            {
                HandleButtonClick("tbbSelect");
            }
            else if (keystates.Contains("kenter") && keystates.Contains("control"))
            {
                if (Legend.SelectedLayer == -1)
                {
                    if (m_legendEditor == null)
                    {
                        m_legendEditor = LegendEditorForm.CreateAndShowLYR();
                    }
                    else
                    {
                        m_legendEditor.LoadProperties((int)Handle, true);
                    }
                }
            }
            else if (keystates.Contains("kspacebar") && keystates.Contains("control"))
            {
                Legend.Layers.ItemByHandle(Legend.SelectedLayer).Visible = !(Legend.Layers.ItemByHandle(Legend.SelectedLayer).Visible);
            }
            else if (keystates.Contains("kuparrow") && keystates.Contains("control"))
            {
                ArrayList ar = new ArrayList();
                int z, g_Count = Legend.Groups.Count;
                for (z = 0; z < g_Count; z++)//将所有层的handle添加到ar中
                {
                    int zz, l_Count = Legend.Groups[z].LayerCount;
                    for (zz = 0; zz < l_Count; zz++)
                    {
                        ar.Add(((LegendControl.Group)Legend.Groups[z])[zz].Handle);//存储层的handle，注意要强转获得索引器
                    }
                }

                int ary_Count = ar.Count;
                for (z = 0; z < ary_Count; z++) //设置当前选择的层
                {
                    if (Legend.SelectedLayer == (int)ar[z] && z + 1 < ary_Count)
                    {
                        Legend.SelectedLayer = (int)ar[z + 1];
                        break;
                    }
                }

            }
            else if (keystates.Contains("kdownarrow") && keystates.Contains("control"))
            {
                ArrayList ar = new ArrayList();
                int z, g_Count = Legend.Groups.Count;
                for (z = 0; z < g_Count; z++)//将所有层的handle添加到ar中
                {
                    int zz, l_Count = Legend.Groups[z].LayerCount;
                    for (zz = 0; zz < l_Count; zz++)//存储层的handle，注意要强转获得索引器
                    {
                        ar.Add(((LegendControl.Group)Legend.Groups[z])[zz].Handle);
                    }
                }

                int ary_Count = ar.Count;
                for (z = 0; z < ary_Count; z++)//设置当前选择的层
                {
                    if (Legend.SelectedLayer == (int)ar[z] && z - 1 > -1)
                    {
                        Legend.SelectedLayer = (int)ar[z - 1];
                        break;
                    }
                }
            }
            else if (keystates.Contains("ko") && keystates.Contains("shift") && keystates.Contains("control"))
            {
                DoOpen();
                return true;
            }
            else if (keystates.Contains("kc") && keystates.Contains("control"))
            {
                DoCopyMap();
                return true;
            }
            else if (keystates.Contains("kp") && keystates.Contains("control"))
            {
                DoPrint();
                return true;
            }
            else if (keystates.Contains("kprint"))
            {
                DoPrint();
                return true;
            }
            else if (keystates.Contains("kf4") && keystates.Contains("control"))
            {
                DoClose();
                return true;
            }
            else if (keystates.Contains("khome") && keystates.Contains("control"))
            {
                if (Legend != null)
                {
                    if (Legend.SelectedLayer != -1)
                    {
                        DoZoomToLayer();
                    }
                }
                return true;
            }
            else if (keystates.Contains("kdelete"))
            {
                if (Legend != null)
                {
                    if (Legend.SelectedLayer != -1)
                    {
                        if (MapWinGIS.Utility.Logger.Message(resources.GetString("msgHandleShortcutKeys_Text"), Program.appInfo.ApplicationName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DoRemoveLayer();
                        }
                    }
                }
                return true;
            }
            else if (keystates.Contains("kinsert"))
            {
                DoAddLayer();
                return true;
            }
            else if (keystates.Contains("khome"))
            {
                DoZoomToFullExtents();
                return true;
            }
            else if (keystates.Contains("kpageup"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double ydiff = (exts.yMax - exts.yMin) / 2.0;
                    exts.SetBounds(exts.xMin, exts.yMin + ydiff, exts.zMin, exts.xMax, exts.yMax + ydiff, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("kpagedown"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double ydiff = (exts.yMax - exts.yMin) / 2.0;
                    exts.SetBounds(exts.xMin, exts.yMin - ydiff, exts.zMin, exts.xMax, exts.yMax - ydiff, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("kuparrow"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double ydiff = (exts.yMax - exts.yMin) / 4.0;
                    exts.SetBounds(exts.xMin, exts.yMin + ydiff, exts.zMin, exts.xMax, exts.yMax + ydiff, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("kdownarrow"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double ydiff = (exts.yMax - exts.yMin) / 4.0;
                    exts.SetBounds(exts.xMin, exts.yMin - ydiff, exts.zMin, exts.xMax, exts.yMax - ydiff, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("kleftarrow"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double xdiff = (exts.xMax - exts.xMin) / 4.0;
                    exts.SetBounds(exts.xMin - xdiff, exts.yMin, exts.zMin, exts.xMax - xdiff, exts.yMax, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("krightarrow"))
            {
                MapWinGIS.Extents exts = (MapWinGIS.Extents)MapMain.Extents;
                if (exts.xMin != 0 && exts.xMax != 0 && exts.yMin != 0 && exts.yMax != 0)
                {
                    double xdiff = (exts.xMax - exts.xMin) / 4.0;
                    exts.SetBounds(exts.xMin + xdiff, exts.yMin, exts.zMin, exts.xMax + xdiff, exts.yMax, exts.zMax);
                    MapMain.Extents = exts;
                }
                return true;
            }
            else if (keystates.Contains("kplus"))
            {
                MapMain.ZoomIn(0.25);
                return true;
            }
            else if (keystates.Contains("kminus"))
            {
                MapMain.ZoomOut(0.25);
                return true;
            }

            return false;
        }
        /// <summary>
        /// 重写ProcessCmdKey，可以获取方向键
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //首先确保，有焦点
            if (MapMain.Focused || StripDocker.Focused || MapPreview.Focused || Legend.Focused)
            {
                if (msg.Msg == 0x100) //WM_KEYDOWN类型（一个按键按住不放）的消息 0x00 =256
                {
                    if (!HandleShortcutKeys(keyData))//执行快捷键操作，若不是自定义快捷键，则调用系统的ProcessCmdKey
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                    return true;
                }
                else //其他的可能响应该消息
                {
                    return base.ProcessCmdKey(ref msg, keyData);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region ----------------Legend事件--------------

        internal bool LoadRenderingOptions(int handle, string filename = "", bool PluginCall = false)
        {
            if (m_Layers[handle].LayerType == eLayerType.LineShapefile || m_Layers[handle].LayerType == eLayerType.PointShapefile || m_Layers[handle].LayerType == eLayerType.PolygonShapefile)
            {
                if (filename == "")
                {
                    MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)(MapMain.get_GetObject(handle));
                    if (File.Exists(Path.ChangeExtension(sf.Filename, ".mwsr")))
                    {

                        XmlDocument doc = new XmlDocument();
                        doc.Load(Path.ChangeExtension(sf.Filename, ".mwsr"));
                        if (doc.GetElementsByTagName("SFRendering").Count != 0 && doc.GetElementsByTagName("SFRendering")[0].ChildNodes.Count != 0)
                        {
                            Program.projInfo.LoadLayerProperties(doc.GetElementsByTagName("SFRendering")[0].ChildNodes[0], handle, PluginCall);
                            SetModified(true);
                            return true;
                        }

                    }
                }
                else
                {
                    if (File.Exists(filename))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(filename);
                        if (doc.GetElementsByTagName("SFRendering").Count != 0 && doc.GetElementsByTagName("SFRendering")[0].ChildNodes.Count != 0)
                        {
                            Program.projInfo.LoadLayerProperties(doc.GetElementsByTagName("SFRendering")[0].ChildNodes[0], handle, PluginCall);
                            SetModified(true);
                            return true;
                        }
                    }
                }
            }
            else if (m_Layers[handle].LayerType == eLayerType.Grid)
            {
                if (filename == "")
                {
                    if (File.Exists(Path.ChangeExtension(m_Layers[handle].FileName, ".mwsr")))
                    {
                        filename = Path.ChangeExtension(m_Layers[handle].FileName, ".mwsr");
                    }
                    else
                    {
                        try
                        {
                            if (MapMain.get_GetObject(handle) != null)
                            {
                                filename = Path.ChangeExtension(((MapWinGIS.ImageClass)(MapMain.get_GetObject(handle))).Filename, ".mwsr");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                }
                if (System.IO.File.Exists(filename))
                {
                    m_Layers[handle].ColoringScheme = ColoringSchemeTools.ImportScheme(m_Layers[handle], filename);
                }
            }
            return false;
        }
        
        internal bool SaveShapeLayerProps(int handle, string filename = "")
        {
            if (m_Layers[handle].LayerType == eLayerType.LineShapefile || m_Layers[handle].LayerType == eLayerType.PointShapefile || m_Layers[handle].LayerType == eLayerType.PolygonShapefile)
            {
            //    Dim doc As New Xml.XmlDocument
            //Dim outfn As String = filename
            //Dim node As Xml.XmlNode = doc.CreateElement("SFRendering")
            //ProjInfo.AddLayerElement(doc, Layers(handle), node)
            //doc.AppendChild(node)
                try
                {
                //    If outfn = "" Then outfn = System.IO.Path.ChangeExtension(CType(MapMain.get_GetObject(handle), MapWinGIS.Shapefile).Filename, ".mwsr")
                //doc.Save(outfn)
                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                //Try
                //    If System.IO.File.Exists(outfn) Then
                //        Dim fi As New System.IO.FileInfo(outfn)
                //        'Note -- parenthesis in line below are critical (will always return true without)
                //        If (fi.Attributes And System.IO.FileAttributes.ReadOnly) = System.IO.FileAttributes.ReadOnly Then
                //            errmsg = "File is read-only: " + outfn
                //        End If
                //    ElseIf System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(outfn)) Then
                //        Dim fi As New System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(outfn))
                //        'Note -- parenthesis in line below are critical (will always return true without)
                //        If (fi.Attributes And System.IO.FileAttributes.ReadOnly) = System.IO.FileAttributes.ReadOnly Then
                //            errmsg = "Directory is read-only: " + System.IO.Path.GetDirectoryName(outfn)
                //        End If
                //    End If
                //Catch
                //End Try
                    MapWinGIS.Utility.Logger.Dbg("无法保存 shapefile 的属性 (.mwsr文件): " + error);
                }
                return true;
            }
            else if (m_Layers[handle].LayerType == eLayerType.Grid)
            {
                try
                {
                    string outfn = filename;
                    if (outfn == "")
                    {
                        outfn = Path.ChangeExtension(m_Layers[handle].FileName, ".mwsr");
                    }
                    ColoringSchemeTools.ExportScheme(m_Layers[handle], outfn);
                }
                catch { }
            }
            return false;
        }

        private void Legend_GroupMouseDown(int handle, MouseButtons button)
        {
            if (m_PluginManager.LegendMouseDown(handle, (int)button, Interfaces.ClickLocation.Group) == false)
            {
                if (button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_GroupHandle = handle;
                    ShowLegendMenu(Interfaces.ClickLocation.Group);
                }
            }
        }

        private void Legend_LayerMouseDown(int handle, MouseButtons button)
        {
            Legend.SelectedLayer = handle;
            if (m_PluginManager.LegendMouseDown(handle, (int)button, ClickLocation.Layer) == false)
            {
                if (button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_GroupHandle = -1;
                    ShowLegendMenu(Interfaces.ClickLocation.Layer);
                }
            }

        }

        private void Legend_LegendClick(MouseButtons button, System.Drawing.Point location)
        {
            if (m_PluginManager.LegendMouseDown(-1, (int)button, ClickLocation.None) == false)
            {
                if (button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_GroupHandle = -1;
                    ShowLegendMenu(Interfaces.ClickLocation.None);
                }
            }
        }

        private void Legend_GroupDoubleClick(int handle)
        {
            if (m_PluginManager.LegendDoubleClick(handle, ClickLocation.Group) == false)
            {
                if (m_legendEditor == null)
                {
                    m_legendEditor = LegendEditorForm.CreateAndShowGRP(handle);
                }
                else
                {
                    m_legendEditor.LoadProperties(handle, false);
                }
            }
        }

        private void Legend_LayerColorboxClicked(int handle)
        {
            if (MapMain.ShapeDrawingMethod == tkShapeDrawingMethod.dmNewSymbology)
            {
                m_PluginManager.BroadcastMessage("LAYER_EDIT_SYMBOLOGY" + handle.ToString());
            }
        }

        private void Legend_LayerCategoryClicked(int handle, int category)
        {
            if (MapMain.ShapeDrawingMethod == tkShapeDrawingMethod.dmNewSymbology)
            {
                m_PluginManager.BroadcastMessage("LAYER_EDIT_SYMBOLOGY" + handle.ToString() + "!" + category.ToString());
            }
        }

        private void Legend_GroupRemoved(int handle)
        {
            if (m_legendEditor != null && m_legendEditor.m_groupHandle > -1)
            {
                if (m_legendEditor.m_groupHandle == handle)
                {
                    m_legendEditor.LoadProperties(-1, true);
                }
            }
        }

        private void ShowLegendMenu(Interfaces.ClickLocation location)
        {
            System.Drawing.Point pnt = new System.Drawing.Point();
            bool mnuShapefileVisible = location == ClickLocation.Layer;
            
            mnuTableEditorLaunch.Visible = mnuShapefileVisible; //属性表编辑器
            mnuLabelSetup.Visible = mnuShapefileVisible; //图层标签设置
            mnuChartsSetup.Visible = mnuShapefileVisible; //图层Chart样式设置          
            mnuRelabel.Visible = mnuShapefileVisible; //重载图层标签
            mnuSaveAsLayerFile.Visible = mnuShapefileVisible; //symbology 管理
            mnuLegendShapefileCategories.Visible = mnuShapefileVisible; //Shapefile设置
            mnuLegend.Items["mnuClearLayers"].Enabled = Legend.Layers.Count > 0;

            if (location == ClickLocation.Group)  //2-删除组、层。4-缩放至组、图层
            {
                mnuLegend.Items["mnuRemoveLayerOrGroup"].Enabled = true;

                if (Legend.Groups.ItemByHandle(m_GroupHandle).LayerCount > 0)
                {
                    mnuLegend.Items["mnuZoomToLayerOrGroup"].Enabled = true;
                }
                else
                {
                    mnuLegend.Items["mnuZoomToLayerOrGroup"].Enabled = false;
                }

                mnuLegend.Items["mnuRemoveLayerOrGroup"].Text = resources.GetString("mnuRemoveGroup_Text");
                mnuLegend.Items["mnuZoomToLayerOrGroup"].Text = resources.GetString("mnuZoomToGroup_Text");
            }
            else if (location == ClickLocation.Layer)
            {
                mnuLegend.Items["mnuRemoveLayerOrGroup"].Enabled = true;
                mnuLegend.Items["mnuZoomToLayerOrGroup"].Enabled = true;
                mnuLegend.Items["mnuRemoveLayerOrGroup"].Text = resources.GetString("mnuRemoveLayer_Text");
                mnuLegend.Items["mnuZoomToLayerOrGroup"].Text = resources.GetString("mnuZoomToLayer_Text");

                bool isShapefile;
                isShapefile = (m_Layers[Legend.SelectedLayer].LayerType == eLayerType.LineShapefile || m_Layers[Legend.SelectedLayer].LayerType == eLayerType.PointShapefile || m_Layers[Legend.SelectedLayer].LayerType == eLayerType.PolygonShapefile);

                mnuTableEditorLaunch.Visible = isShapefile;
                mnuLabelSetup.Visible = isShapefile;
                mnuChartsSetup.Visible = isShapefile;
                mnuRelabel.Visible = (isShapefile && MapMain.ShapeDrawingMethod != tkShapeDrawingMethod.dmNewSymbology);
                mnuLegendShapefileCategories.Visible = isShapefile;
            }
            else
            {
                mnuLegend.Items["mnuRemoveLayerOrGroup"].Enabled = false;
                mnuLegend.Items["mnuZoomToLayerOrGroup"].Enabled = false;
            }

            pnt.X = Legend.PointToClient(MousePosition).X;
            pnt.Y= Legend.PointToClient(MousePosition).Y;

            mnuLegend.Show(Legend, pnt);
            //mnuLegend.Show(MousePosition);

        }

        private void mnuLegend_ItemClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripItem legendMenu = sender as System.Windows.Forms.ToolStripItem;
            if (legendMenu != null)
            {
                string itemName = legendMenu.Name;
                switch (itemName)
                {
                    case "mnuAddGroup": //添加组
                        DoAddGroup();break;
                    case "mnuAddLayer":  //添加图层
                        DoAddLayer();break;
                    case "mnuRemoveLayerOrGroup":  // 移除图层或组
                        DoRemoveLayerOrGroup();
                        break;
                    case "mnuClearLayers": // 清空图层
                        DoClearLayers();
                        break;
                    case "mnuZoomToLayerOrGroup": // 缩放到层或组

                        break;
                    case "mnuRelabel": //重载标签

                        break;
                    case "mnuLabelSetup": //标签设置

                        break;
                    case "mnuChartsSetup": // chart设置

                        break;
                    case "mnuSeeMetadata": // 查看元数据

                        break;
                    case "mnuLegendShapefileCategories": // shapefile 设置

                        break;
                    case "mnuTableEditorLaunch": // 新命名一个数据表

                        break;
                    case "mnuSaveAsLayerFile": //编辑符号库

                        break;
                    case "mnuExpandGroups": //展开组

                        break;
                    case "mnuExpandAll": //展开所有组

                        break;
                    case "mnuCollapseGroups": //折叠组

                        break;
                    case "mnuCollapseAll": // 折叠所有组

                        break;
                    case "mnuProperties": //图层或组的属性

                        break;
                    default: break;
                }
            }
        }

        internal void DoAddGroup()
        {
            Legend.Groups.Add();
            SetModified(true);
        }
        internal void DoRemoveLayerOrGroup()
        {
            if (mnuRemoveLayerOrGroup.Text == resources.GetString("mnuRemoveGroup_Text"))
            {
                DoRemoveGroup();
            }
            else
            {
                DoRemoveLayer();
            }
        }
        internal void DoRemoveGroup()
        { }
        #endregion

        #region --------------MapPreview事件------------
        //MapPreview大小改变发生的事件
        private void MapPreview_SizeChanged(object sender, EventArgs e)
        {
            if (m_PreviewMap != null)
            {
                m_PreviewMap.UpdateLocatorBox();
            }
        }
        //MapPreview鼠标按下事件
        private void MapPreview_MouseDownEvent(object sender, AxMapWinGIS._DMapEvents_MouseDownEvent e)
        { 
            if (e.button == 2)//鼠标右键，在MapPreview上点击
            {
                //重复show可能是必须的，尤其当unlocked的时候,若右键出现问题放开下面语句试试
                //PreviewMapContextMenuStrip.Show(this, MapPreview.PointToClient(MousePosition));
                PreviewMapContextMenuStrip.Show(MapPreview, MapPreview.PointToClient(MousePosition));
            }
            else
            {
                //确定是否在拖动范围框
                if (InBox(m_PreviewMap.g_ExtentsRect, e.x, e.y))
                {
                    m_PreviewMap.g_Dragging = true;
                    oldX = e.x;
                    oldY = e.y;
                    m_startX = e.x;
                    m_startY = e.y;
                }
                else
                {
                    m_PreviewMap.g_Dragging = false;
                }
            }


        }
        
        private void MapPreview_MouseUpEvent(object sender, AxMapWinGIS._DMapEvents_MouseUpEvent e)
        {           
            //添加鼠标按键判断，不允许除左键以外的行为
            if (e.button == 1)
            {
                //停止拖拽
                MapWinGIS.Extents newExts = new MapWinGIS.Extents();
                double xMin = -1, xMax = -1, yMin = -1, yMax = -1;
                if (m_PreviewMap.g_Dragging)
                {
                    //PixelToProj：转换像素坐标为项目地图的坐标
                    MapPreview.PixelToProj(m_PreviewMap.g_ExtentsRect.Left, m_PreviewMap.g_ExtentsRect.Top, ref xMin, ref yMax); //矩形左上角
                    MapPreview.PixelToProj(m_PreviewMap.g_ExtentsRect.Right, m_PreviewMap.g_ExtentsRect.Bottom, ref xMax, ref yMin);//矩形右上角
                    newExts.SetBounds(xMin, yMin, 0, xMax, yMax, 0);
                    //设置鼠标样式
                    MapPreview.MapCursor = MapWinGIS.tkCursor.crsrSizeAll;
                    //同步到MapMain中的地图
                    MapMain.Extents = newExts;
                    newExts = null;
                    MapMain.Focus();
                }
                else //不是拖拽
                {
                    //当点击的点超出红色矩形框的范围，则以改点为中点移动红色矩形框
                    int curCenterX, curCenterY; //当前范围框的中点
                    curCenterX = (m_PreviewMap.g_ExtentsRect.Right + m_PreviewMap.g_ExtentsRect.Left) / 2;
                    curCenterY = (m_PreviewMap.g_ExtentsRect.Bottom + m_PreviewMap.g_ExtentsRect.Top) / 2;
                    m_PreviewMap.g_ExtentsRect.Offset(e.x - curCenterX, e.y - curCenterY);//设置矩形框的位置

                    MapPreview.PixelToProj(m_PreviewMap.g_ExtentsRect.Left, m_PreviewMap.g_ExtentsRect.Top, ref xMin, ref yMax);
                    MapPreview.PixelToProj(m_PreviewMap.g_ExtentsRect.Right, m_PreviewMap.g_ExtentsRect.Bottom, ref xMax, ref yMin);
                    newExts.SetBounds(xMin, yMin, 0, xMax, yMax, 0);
                    //设置鼠标样式
                    MapPreview.MapCursor = MapWinGIS.tkCursor.crsrSizeAll;
                    // 同步到MapMain中的地图
                    MapMain.Extents = newExts;
                    newExts = null;
                    MapMain.Focus();
                }
                
            }
            m_PreviewMap.g_Dragging = false;
        }

        private void MapPreview_MouseMoveEvent(object sender, AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        {
            //移动红色矩形框
            if (e.button == 1)
            {
                if (m_PreviewMap.g_Dragging == true)
                {
                    m_PreviewMap.g_ExtentsRect.Offset(e.x - oldX, e.y - oldY);
                    m_PreviewMap.DrawBox(m_PreviewMap.g_ExtentsRect);
                    oldX = e.x;
                    oldY = e.y;
                }
                else
                {
                    if (InBox(m_PreviewMap.g_ExtentsRect, e.x, e.y))
                    {
                        MapPreview.MapCursor = tkCursor.crsrSizeAll;
                    }
                    else
                    {
                        MapPreview.MapCursor = tkCursor.crsrArrow;
                    }
                }
            }
            else
            {
                m_PreviewMap.g_Dragging = false;
            }
        }

        //右键全图视图事件
        private void mnuPreviewExtents_Click(object sender, EventArgs e)
        {
            DoUpdatePreview(true);
        }
        //右键当前视图事件
        private void mnuPreviewCurrent_Click(object sender, EventArgs e)
        {
            DoUpdatePreview();
        }
        //右键清除预览事件
        private void mnuPreviewClear_Click(object sender, EventArgs e)
        {
            DoClearPreview();
        }

        #endregion

        #region -------------MapMain事件----------------
        // *地图上的事件，都已经在ocx中注册好了。不需在宿主中重复注册
        private void MapToolTipTimer_Tick(object sender, EventArgs e)
        {
        }
        //右键菜单、广播插件(、计算面积？)
        private void MapMain_MouseDownEvent(object sender, AxMapWinGIS._DMapEvents_MouseDownEvent e)
        {
            if (e.button == 2)
            {
                MapMain.CursorMode = tkCursorMode.cmNone; //置none防止右键时继续进行地图的放大缩小。
                //mnuZoom.Show(this, MapMain.PointToClient(MousePosition));
                mnuZoom.Show(MapMain, MapMain.PointToClient(MousePosition));
            }
            SetModified(true);

            if (Program.appInfo.AreaMeasuringCurrently)
            {
                //处理面积测量
                MapWinGIS.Point currPoint = new MapWinGIS.Point();
                double locx = -1, locy = -1;

                //获取实际位置，并存储改点到点列表中
                m_View.PixelToProj(e.x, e.y, ref locx, ref locy);
                currPoint.x = locx;
                currPoint.y = locy;
                Program.appInfo.AreaMeasuringlstDrawPoints.Add(currPoint);

                if (m_View.CursorMode == tkCursorMode.cmNone)
                {
                    //如果用户点击的是右键停止计算面积并显示相关消息
                    if (e.button != 2)
                    {
                        m_View.Draw.DrawPoint(locx, locy, 3, System.Drawing.Color.Red);
                        if (Program.appInfo.AreaMeasuringLastStartPtX == -1)
                        {
                            Program.appInfo.AreaMeasuringLastStartPtX = System.Windows.Forms.Control.MousePosition.X;
                            Program.appInfo.AreaMeasuringLastStartPtY = System.Windows.Forms.Control.MousePosition.Y;
                            Program.appInfo.AreaMeasuringStartPtX = Program.appInfo.AreaMeasuringLastStartPtX;
                            Program.appInfo.AreaMeasuringStartPtY = Program.appInfo.AreaMeasuringLastStartPtY;
                            Program.appInfo.AreaMeasuringEraseLast = false;
                        }
                        else
                        {
                            //颠倒该点为开始位置
                            ControlPaint.DrawReversibleLine(new System.Drawing.Point((int)Program.appInfo.AreaMeasuringStartPtX, (int)Program.appInfo.AreaMeasuringStartPtY),
                                new System.Drawing.Point((int)Program.appInfo.AreaMeasuringLastEndX, (int)Program.appInfo.AreaMeasuringLastEndY),
                                Program.appInfo.AreaMeasuringmycolor);

                            Program.appInfo.AreaMeasuringReversibleDrawn.Add(Program.appInfo.AreaMeasuringLastStartPtX);
                            Program.appInfo.AreaMeasuringReversibleDrawn.Add(Program.appInfo.AreaMeasuringLastStartPtY);
                            Program.appInfo.AreaMeasuringReversibleDrawn.Add(Control.MousePosition.X);
                            Program.appInfo.AreaMeasuringReversibleDrawn.Add(Control.MousePosition.Y);

                            Program.appInfo.AreaMeasuringLastStartPtX = Control.MousePosition.X;
                            Program.appInfo.AreaMeasuringLastStartPtY = Control.MousePosition.Y;
                            Program.appInfo.AreaMeasuringEraseLast = false;

                        }

                    }
                }
            }
            else
            {
                m_PluginManager.MapMouseDown(e.button, e.shift, e.x, e.y);
            }
        }
        
        //处理"选择shape" 更新Preview的显示，将事件广播给插件。
        private void MapMain_MouseUpEvent(object sender, AxMapWinGIS._DMapEvents_MouseUpEvent e)
        {
            MainProgram.SelectInfo seleInfo;
            bool ctrlDown;

            if (MapMain.NumLayers > 0)
            {
                if (MapMain.CursorMode == tkCursorMode.cmPan || MapMain.CursorMode == tkCursorMode.cmZoomIn || MapMain.CursorMode == tkCursorMode.cmZoomOut)
                {
                    m_PreviewMap.UpdateLocatorBox();
                }
                else if (MapMain.CursorMode == tkCursorMode.cmSelection && e.button == 1)
                {
                    if (m_PluginManager.MapMouseUp(e.button, e.shift, e.x, e.y) == false)
                    {
                        if (m_Layers[Legend.SelectedLayer].LayerType != eLayerType.Image && m_Layers[Legend.SelectedLayer].LayerType != eLayerType.Grid)
                        {
                            if (e.shift == 2 || e.shift == 3)
                            {
                                ctrlDown = true;
                            }
                            else
                            {
                                ctrlDown = false;
                            }
                            seleInfo = (MainProgram.SelectInfo)m_View.SelectShapesByPoint((int)e.x, (int)e.y, ctrlDown);
                            m_PluginManager.ShapesSelected(Legend.SelectedLayer, seleInfo);
                            UpdateButtons();
                        }
                    }
                    return;
                }
            }
            m_PluginManager.MapMouseUp(e.button, e.shift, e.x, e.y);
        }

        private void MapMain_MouseMoveEvent(object sender, AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        { }

        private void MapMain_ExtentsChanged(object sender, EventArgs e)
        { 
            if (m_Extents == null || MapMain.NumLayers == 0)
            {
                return;
            }
            MapWinGIS.Utility.Logger.Dbg(string.Format("地图范围改变开始，当前extent：{0}. extent次数：{1}", m_CurrentExtent.ToString(), m_Extents.Count.ToString()));

            m_PreviewMap.UpdateLocatorBox();
            UpdateFloatingScalebar();

            double scale = this.MapMain.CurrentScale;
            if (m_IsManualExtentsChange == true)
            {
                m_IsManualExtentsChange = false; //重置
            }
            else
            {
                if (scale == m_lastScale) //防止将两次同样scale的extent加到数组中。即防止zoomMax多次点击造成内存浪费。
                {
                    return;
                }
                FlushForwardHistory();               
                m_Extents.Add(MapMain.Extents);
                m_CurrentExtent = m_Extents.Count - 1;
            }
            UpdateButtons();

            if (scale != m_lastScale)
            {
                for (int i = 0; i < MapMain.NumLayers; i++)
                {
                    int handle = MapMain.get_LayerHandle(i);
                    if (MapMain.get_LayerDynamicVisibility(handle))
                    {
                        double min = MapMain.get_LayerMinVisibleScale(handle);
                        double max = MapMain.get_LayerMaxVisibleScale(handle);
                        bool visibleNow = (scale >= min && scale <= max);
                        bool visibleBefore = (m_lastScale >= min && m_lastScale <= max);

                        if (visibleBefore != visibleNow)
                        {
                            Legend.Refresh();
                       }
                    }
                }
 
            }
            m_lastScale = scale;

            m_PluginManager.MapExtentsChanged();

            MapWinGIS.Utility.Logger.Dbg(String.Format("地图范围改变完成，当前extent：{0}. extent次数：{1}", m_CurrentExtent.ToString(), m_Extents.Count.ToString()));
        }

        private void MapMain_FileDropped(object sender, AxMapWinGIS._DMapEvents_FileDroppedEvent e)
        { }

        private void MapMain_SelectBoxFinal(object sender, AxMapWinGIS._DMapEvents_SelectBoxFinalEvent e)
        { }

        public bool MapTooltipsAtLeastOneLayer
        {
            get
            {
                return this.m_MapToolTipsAtLeastOneLayer;
            }
            set
            {
                this.m_MapToolTipsAtLeastOneLayer = value;
                MapToolTipTimer.Enabled = value;
                if (value)
                {
                    MapToolTipTimer.Start();
                }
                else
                {
                    MapToolTipTimer.Stop();
                }
            }
        }

        public void UpdateMapToolTipsAtLeastOneLayer()
        {
            MapTooltipsAtLeastOneLayer = false;
            try
            {
                for (int i = 0; i < Legend.Layers.Count; i++)
                {
                    if (Legend.Layers[i].MapTooltipsEnabled && (Legend.Layers[i].Type == eLayerType.LineShapefile || Legend.Layers[i].Type == eLayerType.PointShapefile || Legend.Layers[i].Type == eLayerType.PolygonShapefile))
                    {
                        MapTooltipsAtLeastOneLayer = true;
                        return; 
                    }
                }
            }
            catch
            {
            }
        }


        #endregion

        #region ---------------Toolbar功能---------------
        /// <summary>
        /// 根据map的状态，设置默认按钮为Enable或Disable
        /// </summary>
        internal void UpdateButtons()
        {
            //若TopToolStripPanel被插件移除，则不更新
            if (false == StripDocker.TopToolStripPanel.Visible)
            {
                return;
            }

            //更新主菜单状态
            bool isLayerSelected = this.Legend.SelectedLayer > -1;
            bool isShapefileLayer = false; 
            MapWinGIS.Interfaces.Layer layer = Layers[this.Legend.SelectedLayer];
            if (layer != null)
            {
                if (layer.LayerType == eLayerType.LineShapefile || layer.LayerType == eLayerType.PointShapefile || layer.LayerType == eLayerType.PolygonShapefile)
                {
                    isShapefileLayer = true;
                }
            }

            //设置button为Enable或disabled
            int numLayers = MapMain.NumLayers;
            bool previewMapExtentsIsValid = PreviewMapExtentsValid();

            bool exitLayers = numLayers > 0;
            //工具条按钮，tlbMain:
            tbbSelect.Enabled = isShapefileLayer;//选择shape
            tbbPan.Enabled = exitLayers; //移动 

            //工具条按钮，tlbzoom
            tbbZoomExtent.Enabled = exitLayers;//复位
            tbbZoomIn.Enabled = exitLayers; //放大
            tbbZoomOut.Enabled = exitLayers;//缩小
            tbbZoomLayer.Enabled = exitLayers;//移动到当前图层
            tbbZoomNext.Enabled = (exitLayers) && (m_CurrentExtent < m_Extents.Count - 1) && (m_Extents.Count > 0);//前进         
            tbbZoomPrevious.Enabled = (exitLayers) && (m_Extents.Count > 0) && (m_CurrentExtent > 1);//后退
            tbbZoomSelected.Enabled = (exitLayers) && (m_View.SelectedShapes.NumSelected > 0);//选择

            mnuZoomPrevious.Enabled = tbbZoomPrevious.Enabled;
            mnuZoomNext.Enabled = tbbZoomNext.Enabled;
            //MapMain上的右键菜单,设置没有图层时不显示的项
            toolStripSeparator3.Visible = exitLayers;
            mnuZoomLayer.Visible = exitLayers;
            mnuSelect.Visible = exitLayers;
            mnuZoomPreviewMap.Visible = exitLayers;
            mnuZoomShape.Visible = exitLayers;

            if (m_Menu["mnuZoomToPreviewExtents"] != null) //同步PreviewMap菜单
            {
                this.mnuZoomPreviewMap.Enabled = (exitLayers && previewMapExtentsIsValid);
            }
            if (Legend != null && Legend.SelectedLayer != -1)//legend存在且有选择的图层
            {
                    eLayerType lt;
                    if (m_Layers == null || m_Layers[Legend.SelectedLayer] == null)
                    {
                        lt = eLayerType.Invalid;
                    }
                    else
                    {
                        lt = m_Layers[Legend.SelectedLayer].LayerType;
                    }
                    mnuZoomShape.Enabled = (lt == eLayerType.LineShapefile || lt == eLayerType.PointShapefile || lt == eLayerType.PolygonShapefile);
            }

            //工具条按钮,tlbStandard
            tbbSave.Enabled = (!m_HasBeenSaved) || Program.projInfo.Modified;
            tbbPrint.Enabled = exitLayers;

            //工具条按钮,tlbLayers
            tbbRemoveLayer.Enabled = exitLayers;
            tbbClearLayers.Enabled = exitLayers;
            tbbSymbologyManager.Enabled = isShapefileLayer;
            tbbLayerProperties.Enabled = exitLayers;

            //根据鼠标样式，更新Zoom按钮的选择状态
            MapWinGIS.tkCursorMode value = MapMain.CursorMode;
            tbbSelect.Checked = (value == MapWinGIS.tkCursorMode.cmSelection);
            tbbPan.Checked = (value == MapWinGIS.tkCursorMode.cmPan);
            tbbZoomIn.Checked = (value == MapWinGIS.tkCursorMode.cmZoomIn);
            tbbZoomOut.Checked = (value == MapWinGIS.tkCursorMode.cmZoomOut);

            //'图层'菜单中的子项显示状态
            Program.frmMain.Menus["mnuOptionsManager"].Enabled = isLayerSelected;
            //this.m_Menu["mnuOptionsManager"].Enabled = isLayerSelected;
            //Program.frmMain.m_Menu["mnuOptionsManager"].Enabled = isLayerSelected;
            Program.frmMain.Menus["mnuLayerProperties"].Enabled = isLayerSelected;
            Program.frmMain.Menus["mnuLayerLabels"].Enabled = isShapefileLayer;
            Program.frmMain.Menus["mnuLayerRelabel"].Enabled = isShapefileLayer;
            Program.frmMain.Menus["mnuLayerCharts"].Enabled = isShapefileLayer;
            Program.frmMain.Menus["mnuLayerAttributeTable"].Enabled = isShapefileLayer;
            Program.frmMain.Menus["mnuLayerCategories"].Enabled = isShapefileLayer;
            Program.frmMain.Menus["mnuQueryLayer"].Enabled = isShapefileLayer;

            //若需要，此处可以添加同步菜单栏的菜单项的Enabled视觉样式

            //tbbZoomSelected按钮
            bool selection = false;
            MapWinGIS.Shapefile sf = this.MapMain.get_Shapefile(this.Legend.SelectedLayer);
            if (sf != null)
            {
                selection = sf.NumSelected > 0;
                tbbZoomSelected.Enabled = selection;
                if (m_View.SelectedShapes != null && m_View.SelectedShapes.NumSelected > sf.NumSelected)
                {
                    m_View.SelectedShapes.ClearSelectedShapes();
                }
            }
            //清空当前层选择的Shapes
            Program.frmMain.Menus["mnuClearSelectedShapes"].Enabled = selection;
            tbbDeSelectLayer.Enabled = selection;//反选按钮

            selection = false;
            int numlayers = this.MapMain.NumLayers;
            for (int i = 0; i < numlayers; i++)//图层中有没有选择的shapes
            {
                sf = this.MapMain.get_Shapefile(this.MapMain.get_LayerHandle(i));
                if (sf != null)
                {
                    if (sf.NumSelected > 0)
                    {
                        selection = true;
                        break;
                    }
                }
            }
            Program.frmMain.Menus["mnuClearAllSelection"].Enabled = selection;//清空所有选择的显示

            //从TableEditor添加的toolbar items
            MapWinGIS.Interfaces.ToolbarButton btn = this.m_Toolbar.ButtonItem("TableEditorButton");
            if (btn != null)
            {
                btn.Enabled = isShapefileLayer;
            }
            //从symbology plug-in添加的toolbar items
            btn = this.m_Toolbar.ButtonItem("Categories");
            if (btn != null)
            {
                btn.Enabled = isShapefileLayer;
            }
            btn = this.m_Toolbar.ButtonItem("Label Mover");
            if (btn != null)
            {
                btn.Enabled = isShapefileLayer;
            }
            btn = this.m_Toolbar.ButtonItem("Query");
            if (btn != null)
            {
                btn.Enabled = isShapefileLayer;
            }

            if (tbbSelect.Checked || tbbPan.CanSelect || tbbZoomIn.Checked || tbbZoomOut.Checked)
            {
                MapMain.MapCursor = MapWinGIS.tkCursor.crsrMapDefault;
            }

            this.StatusBar.Refresh();
            MapWinGIS.Utility.Logger.Dbg("UpdateButtons()，更新按钮完成！");
        }
        
        private void DoZoomMax()
        {
            MapMain.ZoomToMaxVisibleExtents();
            MapMain.CursorMode = tkCursorMode.cmNone;
            UpdateButtons();
        }

        private void DoZoomPrevious()
        {
            if (m_Extents.Count > 1 && m_CurrentExtent > 0)
            {
                m_IsManualExtentsChange = true;
                m_CurrentExtent -= 1;
                //MapWinGIS.Utility.Logger.Dbg("In DoZoomPrevious. New CurrentExtent: " + m_CurrentExtent.ToString());
                MapMain.Extents = m_Extents[m_CurrentExtent];                
            }
        }

        private void DoZoomNext()
        { 
            if (m_CurrentExtent < m_Extents.Count - 1)
            {
                m_CurrentExtent += 1;
                m_IsManualExtentsChange = true;
                MapMain.Extents = m_Extents[m_CurrentExtent];
            }
            UpdateButtons();
        }

        private void DoZoomToLayer() 
        {
            MapMain.ZoomToLayer(Legend.SelectedLayer);
            SetModified(true);
        }

        //显示/隐藏toolbar上的文本标签
        private void ContextToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)(e.ClickedItem.GetCurrentParent());
            ToolStrip toolStrip = contextMenuStrip.SourceControl as ToolStrip;
            if (toolStrip == null)
            {
                return;
            }

            if (toolStrip.Tag == null)
            {
                // 设置默认值
                toolStrip.Tag = "ImageAndText";
            }

            // 设置值
            ToolStripItemDisplayStyle displayStyle;
            if (toolStrip.Tag.ToString() == "Image")
            {
                toolStrip.Tag = "ImageAndText";
                displayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
            else
            {
                toolStrip.Tag = "Image";
                displayStyle = ToolStripItemDisplayStyle.Image;
            }

            // 设置每个按钮的显示样式
            for (int i = 0; i < toolStrip.Items.Count; i++)
            {
                toolStrip.Items[i].DisplayStyle = displayStyle;
            }
        }

        //"新建、打开、保存、、、"工具条点击事件
        private void tlbStandard_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)e.ClickedItem;
            string btnName = btn.Name;
            if (btnName.Trim() == "" && btn.Tag is string)
            {
                btnName = btn.Tag.ToString();
            }
            HandleStandardButtonClick(btnName, btn);

        }
        /// <summary>
        /// 处理standard toolbar上的点击事件
        /// </summary>
        internal void HandleStandardButtonClick(string btnName, ToolStripButton btn)
        {
            bool handled = m_PluginManager.ItemClicked(btnName);
            if (!handled)//若插件没有处理
            {
                switch (btnName)
                {
                    case "tbbPrint": DoPrint(); break;
                    case "tbbSave": DoSave(); break;
                    case "tbbNew": DoNew(); break;
                    case "tbbOpen": DoOpen(); break;
                    case "tbbProjectSettings": DoProjectSettings(); break;
                }
            }

            UpdateButtons();
        }

        //"添加、移除···"工具条点击事件
        private void tlbLayers_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)e.ClickedItem;
            string btnName = btn.Name;
            if (btnName.Trim() == "" && btn.Tag is string)
            {
                btnName = btn.Tag.ToString();
            }
            HandleLayersButtonClick(btnName, btn);
        }
        /// <summary>
        /// 处理Layers toolbar上的点击事件
        /// </summary>
        internal void HandleLayersButtonClick(string btnName, ToolStripButton btn)
        {
            if (Program.appInfo.MeasuringCurrently && btnName != "tbbMeasure")
            {
                Program.appInfo.MeasuringStop();
            }
            if (Program.appInfo.AreaMeasuringCurrently && btnName != "tbbMeasureArea")
            {
                Program.appInfo.AreaMeasuringStop();
            }

            switch (btnName)
            {
                case "tbbAddLayer": DoAddLayer(); break;
                case "tbbRemoveLayer": DoRemoveLayer(); break;
                case "tbbClearLayers": DoClearLayers(); break;
                case "tbbSymbologyManager": DoSymbologyManager(); break;
                case "tbbLayerProperties": ShowLayerProperties(Legend.SelectedLayer); break;
                default: m_PluginManager.ItemClicked(btnName); break;// 送给插件处理
            }

            UpdateButtons();
 
        }

        //选择、取消选择 工具条点击事件
        public void tlbMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)e.ClickedItem;
            string btnName = btn.Name;
            if (btnName.Trim() == "" && btn.Tag is string)
            {
                btnName = btn.Tag.ToString();
            }
            HandleButtonClick(btnName, btn);
        }
        /// <summary>
        /// 处理tlb_Main上的点击事件
        /// </summary>
        public void HandleButtonClick(string btnName, System.Windows.Forms.ToolStripItem btn = null)
        {
            bool handled;
            switch (btnName)
            {
                case "tbbAddRemove"://隐藏指定按钮，处理按钮事件
                    if (btn != null)
                    {
                        ((System.Windows.Forms.ToolStripDropDownButton)btn).HideDropDown();
                    }

                    if (mnuBtnAdd.Checked)
                    {
                        DoAddLayer();
                    }
                    else if (mnuBtnRemove.Checked) //移除当前layer
                    {
                        DoRemoveLayer();
                    }
                    else if (mnuBtnClear.Checked)
                    {
                        DoClearLayers();//清空所有layer
                    }
                    break;
                case "tbbSelect"://鼠标呈选择状态，通知插件
                    MapMain.CursorMode = MapWinGIS.tkCursorMode.cmSelection;
                    UpdateButtons();
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
                case "tbbDeSelectLayer"://清空选择域，通知插件
                    DoClearLayerSelection();
                    UpdateButtons();
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
                default:
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
            }

            UpdateButtons();
        }

        //"移动、放大、缩小、、"工具条点击事件
        private void tlbZoom_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)e.ClickedItem;
            string btnName = btn.Name;
            if (btnName.Trim() == "" && btn.Tag is string)
            {
                btnName = btn.Tag.ToString();
            }
            HandleZoomButtonClick(btnName);
        }

        /// <summary>
        /// 处理在ZoomToolbar上按钮的点击的事件
        /// </summary>
        internal void HandleZoomButtonClick(string btnName)
        {
            bool handled;
            switch (btnName)
            {
                case "tbbPan":
                    MapMain.CursorMode = tkCursorMode.cmPan;
                    UpdateButtons();
                    handled = m_PluginManager.ItemClicked(btnName); //通知插件，该点击事件发生
                    break;
                case "tbbZoomIn":
                    DoZoomIn();
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
                case "tbbZoomOut":
                    DoZoomOut();
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
                case "tbbZoomExtent":
                    DoZoomMax();
                    break;
                case "tbbZoomSelected": //聚焦shape
                    DoZoomSelected();
                    break;
                case "tbbZoomPrevious":
                    DoZoomPrevious();
                    break;
                case "tbbZoomNext":
                    DoZoomNext();
                    break;
                case "tbbZoomLayer":
                    DoZoomToLayer();
                    break;
                default:
                    handled = m_PluginManager.ItemClicked(btnName);
                    break;
            }
            //当请求发送给插件后，确保插件没有修改地图上的鼠标状态
            UpdateButtons();
        }

        /// <summary>
        /// 当用户在MainToolbar上点击自定义的combo box时触发的事件
        /// </summary>
        public void CustomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripItem cbClicked = (System.Windows.Forms.ToolStripItem)sender;
            m_PluginManager.ItemClicked(cbClicked.Name);
        }

        /// <summary>
        /// 移除状态栏中以psStartText开头的Text的panel
        /// </summary>
        /// <returns>-1没有删除，0-index表示删除panel的索引</returns>
        internal int GetOrRemovePanel(string psStartText, bool pbRemove = false)
        {
            int panel_index = -1;
            int i;
            int len= psStartText.Length;

            for (i = m_StatusBar.NumPanels - 1; i >= 0; i--)
            {
                if (m_StatusBar[i].Text.Substring(0, len) == psStartText)//eg：是计算面积panel
                {
                    panel_index = i;
                    if (pbRemove == true)
                    {
                        m_StatusBar.RemovePanel(i);
                    }
                }
            }
            return panel_index;
        }
        
        #endregion

        #region ---------------Menus功能-----------------
        private void menuStrip1_LocationChanged(object sender, EventArgs e)//不允许移动菜单栏
        {
            if (this.menuStrip1.Location.X == 0 && this.menuStrip1.Location.Y == 0)
            {
                return;
            }
            //不允许移动菜单栏
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        }

        public double ExtentsToScale(MapWinGIS.Extents ext)
        {
            return m_View.Scale;
            //Return MapWinGeoProc.ScaleTools.calcScale(ext, Project.MapUnits, MapMain.Width, MapMain.Height)
        }

        /// <summary>
        /// 确保所有插件都加载到插件菜单中
        /// </summary>
        internal void SynchPluginMenu()
        {
            //从Plugins菜单中清空Plug-ins列表，然后刷新它们
            if (!Program.g_SyncPluginMenuDefer)
            {
                System.Windows.Forms.ToolStripMenuItem parentMenu;
                Interfaces.MenuItem childMenu;
                string menuKey;
                //不允许插件把“插件”菜单移除
                if (!m_Menu.m_MenuTable.ContainsKey(m_Menu.MenuTableKey("mnuPlugins")))
                {
                    return;
                }

                parentMenu = (System.Windows.Forms.ToolStripMenuItem)m_Menu.m_MenuTable["mnuPlugins"];
                Dictionary<string, PluginInfo> all_PluginList = m_PluginManager.m_PluginList;//存储外部插件
                string[] names = new string[all_PluginList.Count];
                string[] keys = new string[all_PluginList.Count];
                int i = 0;

                foreach (PluginInfo pInfo in all_PluginList.Values)
                {
                    names[i] = pInfo.Name;
                    keys[i] = pInfo.Key;
                    i += 1;
                }
                //排序
                Array.Sort(names, keys);
                //使用排好序的数组中的元素，将其添加到插件菜单的后面
                int len = names.Length;
                for (i = 0; i < len; i++)
                {
                    menuKey = "plugin_" + keys[i];
                    //通过将自己名字命名为"Subcategory::Plugin Name"方式来实现，插件指定自己属于某个插件的子菜单项
                    //根据插件的状态(enabled/disenabled)，实现动态图标加载
                    try
                    {
                        string workingName = names[i];
                        string subCat = "";
                        string lastMenu = "mnuPlugins";
                        object oPicture;
                        bool bPluginState;

                        while (workingName.IndexOf("::") >= 0)
                        {
                            //此处修改 eg： Aa::Bb::Cc 则父菜单就叫Aa Bb
                            //原语句：subCat = "subcat_" + workingName.Substring(0, workingName.IndexOf("::"));
                            subCat = workingName.Substring(0, workingName.IndexOf("::"));
                            oPicture = MapWinGIS.MainProgram.GlobalResource.imgPluginSub;
                            if (!m_Menu.Contains(subCat))//m_MenuTable中不存在，则添加
                            {
                                m_Menu.AddMenu(subCat, lastMenu, oPicture, workingName.Substring(0, workingName.IndexOf("::")));
                            }
                            lastMenu = subCat;
                            workingName = workingName.Substring(workingName.IndexOf("::") + 2);
                        }

                        bPluginState = m_PluginManager.PluginIsLoaded(keys[i]);
                        //if (bPluginState) { MapWinGIS.Utility.Logger.Dbg(string.Format("Plugin {0} is loaded", keys[i])); }
                        oPicture = bPluginState == true ? MapWinGIS.MainProgram.GlobalResource.imgPlugin : MapWinGIS.MainProgram.GlobalResource.imgPluginDisabled;
                        if (subCat != "")//有父菜单
                        {
                            childMenu = m_Menu.AddMenu(menuKey, subCat, oPicture, workingName);
                        }
                        else//没有父菜单
                        {
                            childMenu = m_Menu.AddMenu(menuKey, "mnuPlugins", oPicture, names[i]);
                        }
                        childMenu.Checked = bPluginState;
                        childMenu.Picture = oPicture;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    MapWinGIS.Utility.Logger.Dbg("同步完成");
                    //MapWinGIS.Utility.Logger.Dbg("Ensuring Help menu is last...");
                    m_Menu.EnsureHelpItemLast();
                }//for

            }//if

        }

        internal void CustomMenu_Click(object sender, EventArgs e)//当鼠标点击了菜单中某项的时候触发
        {
            System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
            HandleClickedMenu(item.Name);
        }

        /// <summary>
        /// 处理所有菜单项的点击事件
        /// </summary>
        public void HandleClickedMenu(string menuName)
        {
            //首先看是不是点击的是插件菜单项
            if (menuName.StartsWith("plugin_") == true)
            {
                DoPluginNameClick(menuName.Substring(7));
                return;
            }

            //将点击事件发送给所有插件
            if (!(m_PluginManager.ItemClicked(menuName)))
            {
                switch (menuName)
                {
                    //帮助菜单放在最前面，可以当它移除添加后仍然起作用
                    case "mnuTutorials":
                        System.Diagnostics.Process.Start("http://www.baidu.com");
                        break;
                    case "mnuOnlineDocs":
                        System.Diagnostics.Process.Start("http://www.baidu.com");
                        break;
                    case "mnuOfflineDocs":
                        System.Diagnostics.Process.Start(Program.binFolder + @"\Help\MapWinGIS.chm");
                        break;
                    case "mnuBugReport":
                        System.Diagnostics.Process.Start("http://www.baidu.com");
                        break;
                    case "mnuPluginUpLoading":
                        System.Diagnostics.Process.Start("http://www.baidu.com");
                        break;
                    case "mnuCheckForUpdates": DoCheckForUpdates(); break;
                    case "mnuShortcuts": DoShortcuts(); break;
                    case "mnuWelcomeScreen": DoWelcomeScreen(); break;
                    case "mnuAboutMapWindow": DoAboutMapWindow(); break;
                    //文件（File）菜单
                    case "mnuNew": DoNew(); break;
                    case "mnuOpen": DoOpen(); break;
                    case "mnuOpenProjectIntoGroup": DoOpenIntoCurrent(); break;
                    case "mnuSave": DoSave(); break;
                    case "mnuSaveAs": DoSaveAs(); break;
                    case "mnuPrint": DoPrint(); break;
                    case "mnuProjectSettings": DoProjectSettings(); break;
                    case "mnuExit": DoExit(); break;
                    //二级，输出图像
                    case "mnuSaveMapImage": DoSaveMapImage(); break;
                    case "mnuSaveGeorefMapImage": DoSaveGeoreferenced(); break;
                    case "mnuSaveScaleBar": DoSaveScaleBar(); break;
                    case "mnuSaveNorthArrow": DoSaveNorthArrow(); break;
                    case "mnuSaveLegend": DoSaveLegend(); break;
                    //图层菜单
                    case "mnuAddLayer": DoAddLayer(); break;
                    case "mnuAddECWPLayer": DoAddECWPLayer(); break;
                    case "mnuRemoveLayer": DoRemoveLayer(); break;
                    case "mnuClearLayer": DoClearLayers(); break;
                    case "mnuLayerLabels": DoLabelsEdit(Legend.SelectedLayer); break;
                    case "mnuLayerCharts": DoChartsEdit(Legend.SelectedLayer); break;
                    case "mnuLayerAttributeTable": m_PluginManager.BroadcastMessage("TableEditorStart"); break;
                    case "mnuLayerCategories": DoEditCategories(); break;
                    case "mnuOptionsManager": DoSymbologyManager(); break;
                    case "mnuLayerRelabel": DoLabelsRelabel(Legend.SelectedLayer); break;
                    case "mnuQueryLayer": DoQueryShapefile(); break;
                    case "mnuClearSelectedShapes": DoClearLayerSelection(); break;
                    case "mnuLayerProperties": ShowLayerProperties(Legend.SelectedLayer); break;
                    //视图菜单
                    case "mnuLegendVisible": UpdateLegendPanel(!(m_Menu["mnuLegendVisible"].Checked)); break;
                    case "mnuPreviewVisible": UpdatePreviewPanel(!(m_Menu["mnuPreviewVisible"].Checked)); break;

                    case "mnuEnglish": ChangeCulture(Thread.CurrentThread.CurrentUICulture.Name, "en"); break; //英语
                    case "mnuChinese": ChangeCulture(Thread.CurrentThread.CurrentUICulture.Name); break; //默认语言

                    case "mnuSetScale": DoSetScale(); break;
                    case "mnuShowScaleBar": DoShowScaleBar(); break;

                    case "mnuCopyMap": DoCopyMap(); break;
                    case "mnuCopyLegend": DoCopyLegend(); break;
                    case "mnuCopyScaleBar": DoCopyScaleBar(); break;
                    case "mnuCopyNorthArrow": DoCopyNorthArrow(); break;

                    case "mnuZoomIn": DoZoomIn(); break;
                    case "mnuZoomOut": DoZoomOut(); break;
                    case "mnuZoomToFullExtents": DoZoomToFullExtents(); break;
                    case "mnuZoomToPreviewExtents": DoZoomToPreviewExtents(); break;

                    case "mnuPreviousZoom": DoPreviousZoom(); break;
                    case "mnuNextZoom": DoNextZoom(); break;
                    case "mnuClearAllSelection": DoClearAllSelection(); break;

                    case "mnuUpdatePreviewFull": DoUpdatePreview(true); break;
                    case "mnuUpdatePreviewCurr": DoUpdatePreview(); break;
                    case "mnuClearPreview": DoClearPreview(); break;
                    //书签菜单
                    case "mnuBookmarkAdd": DoBookmarkAdd(); break;
                    case "mnuBookmarksManager": DoBookmarksManager(); break;
                    //插件
                    case "mnuEditPlugins": DoEditPlugins(); break;
                    case "mnuScript": DoScript(); break;
                    default:
                        DoDefaultMenuClik(menuName);//处理“查看书签”和“近期项目”
                        break;
                }

            }

        }

        /// <summary>
        /// 预览地图框体是否有效
        /// </summary>
        /// <returns></returns>
        public bool PreviewMapExtentsValid()
        {
            //检查PreviewPanel是否已经创建
            if (Program.frmMain.previewPanel == null)
            {
                return false; //此处由true改为false
            }
            MapWinGIS.Extents ext = (MapWinGIS.Extents)(MapPreview.Extents);

            if ((ext.xMax == 0.5 && ext.xMin == -0.5) || (ext.yMin == -0.5 && ext.yMax == 0.5))
            {
                return false;
            }

            return true;

        }

        private void mnuZoomItems_Click(object sender, EventArgs e)//右键Zoom菜单子项点击事件
        {
            if (MapMain.NumLayers == 0) //若没有图层，右键任何点击都视为无效
            {
                return;
            }
            System.Windows.Forms.ToolStripItem mnu_Item = (System.Windows.Forms.ToolStripItem)sender;
            //放大、缩小、移动、选择shape等均视为交互，故提示选中。
            //全图、同步Preview、聚焦选择的shape、切换图层、前进、后退等均视为命令，不提示选中
            mnuZoomIn.Checked = mnu_Item.Name == "mnuZoomIn" ? true : false;
            mnuZoomOut.Checked = mnu_Item.Name == "mnuZoomOut" ? true : false;
            mnuZoomPan.Checked = mnu_Item.Name == "mnuZoomPan" ? true : false;
            mnuSelect.Checked = mnu_Item.Name == "mnuSelect" ? true : false; //选择shape

            if (mnuZoomIn.Checked)
            {
                DoZoomIn();
            }
            else if (mnuZoomOut.Checked)
            {
                DoZoomOut();
            }
            else if (mnuZoomPan.Checked)
            {
                MapMain.CursorMode = tkCursorMode.cmPan;
                UpdateButtons();
                m_PluginManager.ItemClicked("tbbPan");
            }
            else if (mnuSelect.Checked)
            {
                MapMain.CursorMode = tkCursorMode.cmSelection;
                UpdateButtons();
                m_PluginManager.ItemClicked("tbbSelect");
            }
            else if (mnu_Item.Name == "mnuZoomPrevious")
            {
                DoZoomPrevious();
            }
            else if (mnu_Item.Name == "mnuZoomNext")
            {
                DoZoomNext();
            }
            else if (mnu_Item.Name == "mnuZoomMax")
            {
                DoZoomMax();
            }
            else if (mnu_Item.Name == "mnuZoomLayer")
            {
                DoZoomToLayer();
            }
            else if (mnu_Item.Name == "mnuZoomShape") //聚焦选择的shape
            {
                DoZoomShape();
            }
            else if (mnu_Item.Name == "mnuZoomPreviewMap")
            {
                DoZoomToPreviewExtents();
            }
        }

        //添加"近期项目"下的子菜单
        public void BuildRecentProjectsMenu()
        {           
            ArrayList keysToRemove = new ArrayList();
            //查找RecentProject menu items 并移除
            foreach (string key in m_Menu.m_MenuTable.Keys)
            {
                if (key.StartsWith(RecentProjectPrefix))
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (string key in keysToRemove)
            {
                m_Menu.Remove(key);
            }
            //添加所有当前的"近期项目"到'近期项目'菜单项中
            string fileName;
            string mnu_Key;
            int re_Count = Program.projInfo.RecentProjects.Count;
            for (int i = 0; i < re_Count; i++)
            {
                fileName = Convert.ToString(Program.projInfo.RecentProjects[i]).Trim();
                if (fileName != "" && fileName != ".mwgprj")//为什么这样判断?
                {
                    mnu_Key = RecentProjectPrefix + fileName.Replace(" ", "");
                    m_Menu.AddMenu(mnu_Key, "mnuRecentProjects", (object)null, Path.GetFileNameWithoutExtension(fileName));
                }
            }
        }

        //添加“查看书签”下的子菜单
        public void BuildBookmarkedViewsMenu()
        {
            ArrayList keysToRemove = new ArrayList();
            string parentMenuKey = "mnuBookmarkedViews"; //父菜单的Name
            System.Windows.Forms.ToolStripMenuItem parentToolStripMenuItem = null;
            //获取父菜单对象
            parentToolStripMenuItem = (System.Windows.Forms.ToolStripMenuItem)m_Menu.m_MenuTable[m_Menu.MenuTableKey(parentMenuKey)];
            if (parentToolStripMenuItem == null)//没有获取到
            {
                return;
            }

            //移除以“mnuBookmarkedView_”开头的书签菜单项。先记录，再移除
            foreach (string key in m_Menu.m_MenuTable.Keys)
            {
                if (key.StartsWith(BookmarkedViewPrefix))
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (string key in keysToRemove)
            {
                m_Menu.m_MenuTable.Remove(key);
            }

            //移除"查看书签"下的子菜单项
            parentToolStripMenuItem.DropDownItems.Clear();

            //然后重新从Program.projInfo.BookmarkedViews中添加"查看书签"下的子菜单项
            int bm_Count = Program.projInfo.BookmarkedViews.Count;
            for (int i = 0; i < bm_Count; i++)
            {
                if ((((XmlProjectFile.BookmarkedView)Program.projInfo.BookmarkedViews[i]).Name != ""))
                {
                    m_Menu.AddMenu(BookmarkedViewPrefix + i.ToString(), parentMenuKey, (object)null, ((XmlProjectFile.BookmarkedView)Program.projInfo.BookmarkedViews[i]).Name.Trim());
                }
            }
        }

        private void DoClearPreview()
        {
            ClearPreview();
            SetModified(true);
        }

        /// <summary>
        /// 更新preview map的视图
        /// </summary>
        /// <param name="FullExtents">是更新全图true，还是更新当前视图false</param>
        private void DoUpdatePreview(bool FullExtents = false)//更新Preview Map
        {
            m_PreviewMap.GetPictureFromMap(FullExtents);
            SetModified(true);
        }

        /// <summary>
        /// 处理保存的"书签"和"近期项目"的动态菜单点击事件
        /// </summary>
        private void DoDefaultMenuClik(string menuName)
        {
            if (menuName.StartsWith(BookmarkedViewPrefix))
            {
                //加载书签保存的视图位置
                string sViewNumber = menuName.Replace(BookmarkedViewPrefix, ""); //mnuBookmarkedView_1、2···
                int iViewNumber = -1;
                if (int.TryParse(sViewNumber, out iViewNumber) && iViewNumber != -1)
                {
                    //此处先搁置
                    MapMain.Extents = ((XmlProjectFile.BookmarkedView)(Program.projInfo.BookmarkedViews[iViewNumber])).Exts;
                    SetModified(true);
                }
                else
                {
                    MessageBox.Show("无法识别的书签！", "书签无效", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                }

            }
            else if (menuName.StartsWith(RecentProjectPrefix)) //mnuRecentProjects_1、2···
            {
                //加载最近项目
                if (!m_HasBeenSaved || Program.projInfo.Modified)
                {
                    if (PromptToSaveProject() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                //将最近的项目加载到Legend和MapMain中
                if (!m_Project.Load(menuName.Substring(RecentProjectPrefix.Length).Replace("{32}", " ")))
                {
                    MessageBox.Show("不能加载" + menuName.Substring(RecentProjectPrefix.Length), "近期项目", MessageBoxButtons.OK);
                }
            }
        }

        private void DoPluginNameClick(string pluginKey)
        {
            if (m_PluginManager.PluginIsLoaded(pluginKey))//若加载，卸载
            {
                m_PluginManager.StopPlugin(pluginKey);
                m_Menu["plugin_" + pluginKey].Checked = false;
                m_Menu["plugin_" + pluginKey].Picture = MapWinGIS.MainProgram.GlobalResource.imgPluginDisabled;
            }
            else //没有加载，加载
            {
                m_PluginManager.StartPlugin(pluginKey);
                m_Menu["plugin_" + pluginKey].Checked = true;
                m_Menu["plugin_" + pluginKey].Picture = MapWinGIS.MainProgram.GlobalResource.imgPlugin;
            }
            //插件存储在project中，所以插件改变需要设置Modified
            SetModified(true);
        }
        
        private void DoCheckForUpdates()
        {
            // 检查更新，是通过调用CheckUpdate.exe并将该版本参数出递给该exe到服务器进行检测
            // 略
            string myVersion = App.VersionString;
            MessageBox.Show("当前版本：" + myVersion + " 无需更新!", "检查更新", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DoShortcuts()
        {
            string strMessage;
            strMessage = resources.GetString("msgShortcutsTitle_Text") + "\r\n\r\n" +
                resources.GetString("msgShortcutsDel_Text") + "\r\n" +
                resources.GetString("msgShortcutsIns_Text") + "\r\n\r\n" +
                resources.GetString("msgShortcutsCtrlS_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlO_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlC_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlP_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlI_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlH_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlR_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlZ_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlY_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlM_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlF4_Text") + "\r\n\r\n" +
                resources.GetString("msgShortcutsHome_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlHome_Text") + "\r\n" +
                resources.GetString("msgShortcutsPlus_Text") + "\r\n" +
                resources.GetString("msgShortcutsMinus_Text") + "\r\n\r\n" +
                resources.GetString("msgShortcutsPageUp_Text") + "\r\n" +
                resources.GetString("msgShortcutsPageDown_Text") + "\r\n" +
                resources.GetString("msgShortcutsArrowUp_Text") + "\r\n" +
                resources.GetString("msgShortcutsArrowDown_Text") + "\r\n" +
                resources.GetString("msgShortcutsArrowLeft_Text") + "\r\n" +
                resources.GetString("msgShortcutsArrowRight_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlShiftI_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlShiftO_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlSpace_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlArrows_Text") + "\r\n" +
                resources.GetString("msgShortcutsCtrlEnter_Text") + "\r\n" +
                resources.GetString("msgShortcutsBackspace_Text");

            new NonModalMessageBox(strMessage, "快捷键", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MsgBox msgBox = new MsgBox(strMessage);
            //msgBox.Show();
            //new NonModalMessageBoxVB(strMessage, Microsoft.VisualBasic.MsgBoxStyle.Information, "快捷键");
        }

        private void DoWelcomeScreen()
        {
            if (Program.appInfo.WelcomePlugin != null && Program.appInfo.WelcomePlugin != "" && Program.appInfo.WelcomePlugin != "WelcomeScreen")
            {
                Program.frmMain.Plugins.BroadcastMessage("WELCOME_SCREEN");
            }
            else
            {
                WelcomeScreen welcomescreen = new WelcomeScreen();
                welcomescreen.ShowDialog(this);
            }
        }

        private void DoAboutMapWindow()
        {
            frmAbout about = new frmAbout();
            about.ShowDialog(this);
        }

        private void DoNew()
        { }

        private void DoOpen() 
        {
            OpenFileDialog cdlOpen = new OpenFileDialog();

            MapWinGIS.Grid gr = new MapWinGIS.Grid();
            MapWinGIS.Image im = new MapWinGIS.Image();
            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            string layerFilters = gr.CdlgFilter + "|" + im.CdlgFilter + "|" + sf.CdlgFilter;
            gr = null;
            im = null;
            sf = null;
            cdlOpen.Filter = "MapWinGIS Project (*.mwprj)|*mwprj" + "|" + layerFilters;

            //若打开之前存在项目，提示是否保存
            if (!m_HasBeenSaved || Program.projInfo.Modified)
            {
                if (PromptToSaveProject() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
            }
            //打开新的项目
            if (Directory.Exists(Program.appInfo.DefaultDir))
            {
                cdlOpen.InitialDirectory = Program.appInfo.DefaultDir;
            }
            cdlOpen.ShowDialog();

            if (File.Exists(cdlOpen.FileName))
            {
                //保存最近打开的文件路径
                Program.appInfo.DefaultDir = Path.GetDirectoryName(cdlOpen.FileName);
                if (Path.GetExtension(cdlOpen.FileName) == ".mwprj")
                {
                    //先清空当前项目
                    DoNew();
                    Program.projInfo.ProjectFileName = cdlOpen.FileName;
                    if (Program.projInfo.LoadProject(cdlOpen.FileName))
                    {
                        MapWinGIS.Utility.Logger.Message("打开项目文件出错", "项目文件错误报告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                    m_HasBeenSaved = true;
                    Program.projInfo.ProjectFileName = cdlOpen.FileName;
                    SetModified(false);
                }
                else
                {
                    //选择的是层而不是项目文件
                    if (m_Layers.AddLayer(objectOrFilename: cdlOpen.FileName, layerName: Path.GetFileNameWithoutExtension(cdlOpen.FileName), layerVisible: MapWinGIS.MainProgram.Layers.GetDefaultLayerVis(), positionFromSelected: true) != null)
                    {
                        SetModified(true);
                    }
                }
            }

        }

        internal void DoSave() { }

        private void DoSaveAs() { }

        private void DoPrint() { }
        
        private void DoProjectSettings() 
        {
            ProjectSettings settingForm = new ProjectSettings();
            settingForm.ShowDialog();
        }

        private void DoExit()
        {
            this.Close();
        }

        internal void DoClose()//关闭当前项目
        {
            if (!m_HasBeenSaved || Program.projInfo.Modified)
            {
                if (PromptToSaveProject() == DialogResult.Cancel)
                {
                    return;
                }
            }

            Program.projInfo.ProjectFileName = "";//项目文件名，空
            m_Layers.Clear();//图层，空
            Legend.Groups.Clear();//组，空
            Program.projInfo.BookmarkedViews.Clear();//书签视图，空
            BuildBookmarkedViewsMenu();//创建书签菜单
            ClearPreview();//清空预览地图
            m_AutoVis = new DynamicVisibilityClass();
            ResetViewState();//重置窗口
            m_HasBeenSaved = true;//保存
            SetModified(false);
        }

        internal void ResetViewState(bool leaveFloatingScalebar = false)//重置界面状态
        {
           
        }

        private void DoSaveMapImage() { }
        private void DoSaveGeoreferenced() { }
        private void DoSaveScaleBar() { }
        private void DoSaveNorthArrow() { }
        private void DoSaveLegend() { }
        private void ShowLayerProperties(int handle)
        {
        }
        
        //清空当前一个层上所有选择的shape
        private void DoClearLayerSelection() 
        {
            Interfaces.Layer layer = m_Layers[Legend.SelectedLayer];
            if (layer != null)
            {
                layer.ClearSelection();
                bool handled = false;
                m_View.Redraw();
                //UpdateButtons(); //我认为，此处不需要UpdateButton
                FireLayerSelectionChanged(Legend.SelectedLayer, ref handled);
            }
        }

        //清空所有图层上选择的shape，并通知插件
        private void DoClearAllSelection()
        {
            m_View.ClearSelectedShapes();
            for (int i = 0; i < m_Layers.NumLayers; i++)
            {
                bool handled = false;
                FireLayerSelectionChanged(m_Layers.GetHandle(i), ref handled);
            }

        }

        //广播给插件，建立查询shapefile窗体
        private void DoQueryShapefile()
        {
            if (MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                MapWinGIS.MainProgram.Layer lyr = (MapWinGIS.MainProgram.Layer)m_Layers[Legend.SelectedLayer];
                if (lyr.LayerType == eLayerType.LineShapefile || lyr.LayerType == eLayerType.PointShapefile || lyr.LayerType == eLayerType.PolygonShapefile)
                {
                    if (lyr != null)
                    {
                        m_PluginManager.BroadcastMessage("QUERY_SHAPEFILE" + lyr.Handle.ToString());
                        UpdateButtons();
                    }
                }
            }
        }
        
        public void DoLabelsRelabel(int handle)
        {
        }
        private void DoSymbologyManager()
        {
        }
        //编辑shapefile categories
        private void DoEditCategories()
        {
            if (MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                Layer lyr = (MapWinGIS.MainProgram.Layer)m_Layers[Legend.SelectedLayer];
                if (lyr.LayerType == eLayerType.LineShapefile || lyr.LayerType == eLayerType.PointShapefile || lyr.LayerType == eLayerType.PolygonShapefile)
                {
                    if (lyr != null)
                    {
                        m_PluginManager.BroadcastMessage("SHAPEFILE_CATEGORIES_EDIT" + lyr.Handle.ToString());
                    }
                }
            }
        }

        public void DoChartsEdit(int handle)
        {
        }

        public void DoLabelsEdit(int handle)
        {
        }

        //移除所有图层
        private void DoClearLayers()
        {
            if (MapWinGIS.Utility.Logger.Message("确定要移除所有图层吗？", "移除所有图层", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                m_Layers.Clear();
                Legend.Layers.Clear();
                SetModified(false);
            }
        }

        //移除当前图层
        private void DoRemoveLayer()
        {
            int curHandle = Legend.SelectedLayer;
            if (curHandle != -1)
            {
                m_Layers.Remove(curHandle);
                Legend.Refresh();
                SetModified(true);
            }
        }

        private void DoAddECWPLayer()
        {
        }

        //添加一个图层到地图上
        private void DoAddLayer()
        {
            if (m_Layers.AddLayer(objectOrFilename: "", layerVisible: MainProgram.Layers.GetDefaultLayerVis(), positionFromSelected: true) != null)
            {
                SetModified(true);
            }
        }

        private void DoScript()
        {
            if (Program.scripts == null || Program.scripts.IsDisposed)
            {
                Program.scripts = new frmScript();
                Program.scripts.Owner = this;
            }

            Program.scripts.Icon = this.Icon;
            Program.scripts.Show();
        }

        private void DoEditPlugins()
        {
            m_PluginManager.ShowPluginDialog();
        }

        private void DoBookmarksManager()
        {
            BookmarkManager bookmarkManagerDialog = new BookmarkManager(Program.projInfo.BookmarkedViews);

            bookmarkManagerDialog.ShowDialog();
            SetModified(bookmarkManagerDialog.IsModified);
            BuildBookmarkedViewsMenu();

        }

        private void DoBookmarkAdd()
        {
            BookmarkAddNew addBMDialog = null;
            string newName = string.Empty;
            MapWinGIS.Extents newExtents = null;

            newName = "书签 " + (Program.projInfo.BookmarkedViews.Count + 1).ToString();
            if (MapMain.Extents != null)
            {
                addBMDialog = new BookmarkAddNew(newName, (MapWinGIS.Extents)(MapMain.Extents));
                if (addBMDialog.ShowDialog() == DialogResult.OK)//显示对话框并检查是否确定添加
                {
                    newName = addBMDialog.BookmarkName;
                    newExtents = addBMDialog.BookmarkExtents;

                    if (!(newName.Trim() == ""))
                    {
                        Program.projInfo.BookmarkedViews.Add(new XmlProjectFile.BookmarkedView(newName, newExtents));
                        SetModified(true);
                        BuildBookmarkedViewsMenu();
                    }
                }
            }
        }

        private void DoNextZoom()
        {
            //这里的前进后退需要与toolbar不同
            //具体功能后期再加。
            this.DoZoomNext();
        }

        private void DoPreviousZoom()
        {
            //这里的前进后退需要与toolbar不同
            //具体功能后期再加。
            this.DoZoomPrevious();
        }

        private void DoZoomToPreviewExtents()
        {
            if (previewPanel == null)
            {
                return;
            }
            if (PreviewMapExtentsValid())
            {
                if (Program.appInfo.MeasuringCurrently)
                {
                    Program.appInfo.MeasuringStop();
                }
                MapMain.Extents = MapPreview.Extents;
                //更新浮动比例尺
                m_FloatingScalebar_Enabled = Program.appInfo.ShowFloatingScalebar;
                UpdateFloatingScalebar();
            }
            else
            {
                MapWinGIS.Utility.Logger.Message("预览地图尚未设置，请先显示预览地图", "PreviewMap 没有初始化");
            }
        }

        private void DoZoomToFullExtents()
        {
            MapMain.ZoomToMaxExtents();
            SetModified(true);
            if (Program.appInfo.MeasuringCurrently)
            {
                Program.appInfo.MeasuringStop();
            }
        }

        private void DoZoomOut()
        {
            MapMain.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut;
            UpdateButtons();
        }

        private void DoZoomIn()
        {
            MapMain.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn;
            UpdateButtons();
        }

        private void DoCopyNorthArrow()
        {
        }

        private void DoCopyScaleBar()
        {
        }

        private void DoCopyLegend()
        {
        }

        private void DoCopyMap()
        {
        }

        private void DoShowScaleBar()
        {
            SetModified(true);
            m_FloatingScalebar_Enabled = !m_FloatingScalebar_Enabled;
            Program.appInfo.ShowFloatingScalebar = m_FloatingScalebar_Enabled;
            UpdateFloatingScalebar();
        }

        internal void DoSetScale()
        {
            if (MapMain.NumLayers == 0)
            {
                MapWinGIS.Utility.Logger.Msg("请在设置比例之前先向地图添加数据。", "请先添加数据", MessageBoxIcon.Information);
                return;
            }

            SetScale getscale = new SetScale(GetCurrentScale());
            if (getscale.ShowDialog() == DialogResult.OK)
            {
                //---------注释，需要SetScale类实现完成-------------
                //if (getscale.cboPredefinedScales.Text.StartsWith("["))
                //{
                //    SetScale(getscale.txtNewScale.Text);
                //}
                //else
                //{
                //    SetScale(getscale.cboPredefinedScales.Text);
                //}
            }
        }

        //聚焦到当前选择的shape.右键菜单，菜单栏
        private void DoZoomShape()
        {
            if (m_View.SelectedShapes[0] == null)
            {
                return;
            }
            double maxX, maxY, minX, minY;
            double dx, dy;

            int i;
            MapWinGIS.Extents tExts;
            maxX = m_View.SelectedShapes[0].Extents.xMax;
            minX = m_View.SelectedShapes[0].Extents.xMin;
            maxY = m_View.SelectedShapes[0].Extents.yMax;
            minY = m_View.SelectedShapes[0].Extents.yMin;

            for (i = 0; i < m_View.SelectedShapes.NumSelected; i++)
            {
                if (m_View.SelectedShapes[i].Extents.xMax > maxX)
                {
                    maxX = m_View.SelectedShapes[i].Extents.xMax;
                }
                if (m_View.SelectedShapes[i].Extents.yMax > maxY)
                {
                    maxY = m_View.SelectedShapes[i].Extents.yMax;
                }
                if (m_View.SelectedShapes[i].Extents.xMin < minX)
                {
                    minX = m_View.SelectedShapes[i].Extents.xMin;
                }
                if (m_View.SelectedShapes[i].Extents.yMin < minY)
                {
                    minY = m_View.SelectedShapes[i].Extents.yMin;
                }
            }

            // Pad extents now
            dx = maxX - minX;
            dx = dx * m_View.ExtentPad;
            maxX = maxX + dx;
            minX = minX - dx;

            dy = maxY - minY;
            dy = dy * m_View.ExtentPad;
            maxY = maxY + dy;
            minY = minY - dy;

            tExts = new MapWinGIS.Extents();
            tExts.SetBounds(minX, minY, 0, maxX, maxY, 0);
            MapMain.Extents = tExts;
            tExts = null;
        }

        private void DoZoomSelected() //toolbar上的聚焦shape
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (m_View.SelectedShapes != null && m_View.SelectedShapes.NumSelected > 0)
                {
                    double maxX, maxY, minX, minY, dx, dy;
                    MapWinGIS.Extents tExts;

                    int i;
                    maxX = View.SelectedShapes[0].Extents.xMax;
                    minX = View.SelectedShapes[0].Extents.xMin;
                    maxY = View.SelectedShapes[0].Extents.yMax;
                    minY = View.SelectedShapes[0].Extents.yMin;
                    for (i = 0; i < m_View.SelectedShapes.NumSelected; i++)
                    {
                        if (m_Layers[m_Layers.CurrentLayer].Visible == false)
                        {
                            m_Layers[m_Layers.CurrentLayer].Visible = true;
                        }
                        if (m_View.SelectedShapes[i].Extents.xMax > maxX)
                        {
                            maxX = m_View.SelectedShapes[i].Extents.xMax;
                        }
                        if (m_View.SelectedShapes[i].Extents.yMax > maxY)
                        {
                            maxY = m_View.SelectedShapes[i].Extents.yMax;
                        }
                        if (m_View.SelectedShapes[i].Extents.xMin < minX)
                        {
                            minX = m_View.SelectedShapes[i].Extents.xMin;
                        }
                        if (m_View.SelectedShapes[i].Extents.yMin < minY)
                        {
                            minY = m_View.SelectedShapes[i].Extents.yMin;
                        }
                    }

                    // Pad extents now
                    dx = maxX - minX;
                    dx = dx / 8;
                    if (dx == 0)
                    {
                        dx = 1;
                    }
                    maxX = maxX + dx;
                    minX = minX - dx;

                    dy = maxY - minY;
                    dy = dy / 8;
                    if (dy == 0)
                    {
                        dy = 1;
                    }
                    maxY = maxY + dy;
                    minY = minY - dy;

                    tExts = new MapWinGIS.Extents();
                    if (View.SelectedShapes.NumSelected == 1 && m_Layers[m_Layers.CurrentLayer].LayerType == Interfaces.eLayerType.PointShapefile)
                    {
                        MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)(m_Layers[m_Layers.CurrentLayer].GetObject());
                        //Use shape extents
                        double xpad = (1 / 100) * (sf.Extents.xMax - sf.Extents.xMin);
                        double ypad = (1 / 100) * (sf.Extents.yMax - sf.Extents.yMin);
                        tExts.SetBounds(minX + xpad, minY - ypad, 0, maxX - xpad, maxY + ypad, 0);
                    }
                    else
                    {
                        tExts.SetBounds(minX, minY, 0, maxX, maxY, 0);
                    }
                    m_View.Extents = tExts;
                    tExts = null;
                }
            }
            catch (Exception e)
            {
                MapWinGIS.Utility.Logger.Dbg("聚焦shape错误: " + e.ToString());
                return;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void ChangeCulture(string sourcesCulture, string destinationCulture = null)
        {
            //改变语言显示，其实就是只要改变UI的区域性，线程的区域性可以保持不变。
            if (sourcesCulture == destinationCulture || destinationCulture == Thread.CurrentThread.CurrentUICulture.Name)
            {
                return;
            }

            if (destinationCulture == null || destinationCulture == Thread.CurrentThread.CurrentCulture.Name || destinationCulture == "InvariantCulture")
            {
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                Program.appInfo.OverrideSystemLocale = true;
                Program.appInfo.Locale = Thread.CurrentThread.CurrentCulture.Name;              
                //m_PluginManager.m_dlg.Dispose();
                //m_PluginManager.m_dlg = new PluginsForm();
                ApplyResources();
                AlterMenuText();
            }
            else if (destinationCulture == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                Program.appInfo.OverrideSystemLocale = true;
                Program.appInfo.Locale = destinationCulture;
                ApplyResources();
                //这里不采用移除菜单，再添加的方式。而是直接采用修改Text属性。
                AlterMenuText("en");
            }
            else
            {
                Program.messageBox.Display("语言转换出现错误!", "错误");
            }

            //this.Update();
            //Application.DoEvents();
        }

        private void ApplyResources()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapWinForm));
            resources.ApplyResources(this.ContextToolStrip, "ContextToolStrip");
            resources.ApplyResources(this.ToggleTextLabelsToolStripMenuItem, "ToggleTextLabelsToolStripMenuItem");
            resources.ApplyResources(this.tbbNew, "tbbNew");
            resources.ApplyResources(this.tbbOpen, "tbbOpen");
            resources.ApplyResources(this.tbbSave, "tbbSave");
            resources.ApplyResources(this.tbbPrint, "tbbPrint");
            resources.ApplyResources(this.tbbProjectSettings, "tbbProjectSettings");
            resources.ApplyResources(this.tbbAddLayer, "tbbAddLayer");
            resources.ApplyResources(this.tbbRemoveLayer, "tbbRemoveLayer");
            resources.ApplyResources(this.tbbClearLayers, "tbbClearLayers");
            resources.ApplyResources(this.tbbSymbologyManager, "tbbSymbologyManager");
            resources.ApplyResources(this.tbbLayerProperties, "tbbLayerProperties");
            resources.ApplyResources(this.tbbSelect, "tbbSelect");
            resources.ApplyResources(this.tbbDeSelectLayer, "tbbDeSelectLayer");
            resources.ApplyResources(this.tbbPan, "tbbPan");
            resources.ApplyResources(this.tbbZoomIn, "tbbZoomIn");
            resources.ApplyResources(this.tbbZoomOut, "tbbZoomOut");
            resources.ApplyResources(this.tbbZoomExtent, "tbbZoomExtent");
            resources.ApplyResources(this.tbbZoomSelected, "tbbZoomSelected");
            resources.ApplyResources(this.tbbZoomPrevious, "tbbZoomPrevious");
            resources.ApplyResources(this.tbbZoomNext, "tbbZoomNext");
            resources.ApplyResources(this.tbbZoomLayer, "tbbZoomLayer");
            resources.ApplyResources(this.mnuAddGroup, "mnuAddGroup");
            resources.ApplyResources(this.mnuAddLayer, "mnuAddLayer");
            resources.ApplyResources(this.mnuRemoveLayerOrGroup, "mnuRemoveLayerOrGroup");
            resources.ApplyResources(this.mnuClearLayers, "mnuClearLayers");
            resources.ApplyResources(this.mnuZoomToLayerOrGroup, "mnuZoomToLayerOrGroup");
            resources.ApplyResources(this.mnuRelabel, "mnuRelabel");
            resources.ApplyResources(this.mnuLabelSetup, "mnuLabelSetup");
            resources.ApplyResources(this.mnuChartsSetup, "mnuChartsSetup");
            resources.ApplyResources(this.mnuSeeMetadata, "mnuSeeMetadata");
            resources.ApplyResources(this.mnuLegendShapefileCategories, "mnuLegendShapefileCategories");
            resources.ApplyResources(this.mnuTableEditorLaunch, "mnuTableEditorLaunch");
            resources.ApplyResources(this.mnuSaveAsLayerFile, "mnuSaveAsLayerFile");
            resources.ApplyResources(this.mnuExpandGroups, "mnuExpandGroups");
            resources.ApplyResources(this.mnuExpandAll, "mnuExpandAll");
            resources.ApplyResources(this.mnuCollapseGroups, "mnuCollapseGroups");
            resources.ApplyResources(this.mnuCollapseAll, "mnuCollapseAll");
            resources.ApplyResources(this.mnuProperties, "mnuProperties");
            resources.ApplyResources(this.mnuBtnAdd, "mnuBtnAdd");
            resources.ApplyResources(this.mnuBtnRemove, "mnuBtnRemove");
            resources.ApplyResources(this.mnuBtnClear, "mnuBtnClear");
            resources.ApplyResources(this.mnuZoomIn, "mnuZoomIn");
            resources.ApplyResources(this.mnuZoomOut, "mnuZoomOut");
            resources.ApplyResources(this.mnuZoomPan, "mnuZoomPan");
            resources.ApplyResources(this.mnuZoomMax, "mnuZoomMax");
            resources.ApplyResources(this.mnuZoomPrevious, "mnuZoomPrevious");
            resources.ApplyResources(this.mnuZoomNext, "mnuZoomNext");
            resources.ApplyResources(this.mnuSelect, "mnuSelect");
            resources.ApplyResources(this.mnuZoomShape, "mnuZoomShape");
            resources.ApplyResources(this.mnuZoomPreviewMap, "mnuZoomPreviewMap");
            resources.ApplyResources(this.mnuZoomLayer, "mnuZoomLayer");
            resources.ApplyResources(this.mnuPreviewExtents, "mnuPreviewExtents");
            resources.ApplyResources(this.mnuPreviewCurrent, "mnuPreviewCurrent");
            resources.ApplyResources(this.mnuPreviewClear, "mnuPreviewClear");
            resources.ApplyResources(this, "$this");
        }

        private void AlterMenuText(string culture = null)
        {
            System.Windows.Forms.ToolStripMenuItem parent;
            System.Windows.Forms.ToolStripMenuItem subParent;
            //先修改宿主内部创建的菜单Text属性。
            //文件
            menuStrip1.Items["mnuFile"].Text = resources.GetString("mnuProject_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuFile"];
            parent.DropDownItems["mnuNew"].Text = resources.GetString("mnuNew_Text");
            parent.DropDownItems["mnuOpen"].Text = resources.GetString("mnuOpen_Text");
            parent.DropDownItems["mnuOpenProjectIntoGroup"].Text = resources.GetString("mnuOpenProjectIntoGroup_Text");
            parent.DropDownItems["mnuSave"].Text = resources.GetString("mnuSave_Text");
            parent.DropDownItems["mnuSaveAs"].Text = resources.GetString("mnuSaveAs_Text");
            parent.DropDownItems["mnuPrint"].Text = resources.GetString("mnuPrint_Text");
            parent.DropDownItems["mnuRecentProjects"].Text = resources.GetString("mnuRecentProjects_Text");
            parent.DropDownItems["mnuExport"].Text = resources.GetString("mnuExport_Text");
            
            subParent = (System.Windows.Forms.ToolStripMenuItem)parent.DropDownItems["mnuExport"];
            subParent.DropDownItems["mnuSaveMapImage"].Text = resources.GetString("mnuSaveMapImage_Text");
            subParent.DropDownItems["mnuSaveGeorefMapImage"].Text = resources.GetString("mnuSaveGeorefMapImage_Text");
            subParent.DropDownItems["mnuSaveScaleBar"].Text = resources.GetString("mnuSaveScaleBar_Text");
            subParent.DropDownItems["mnuSaveNorthArrow"].Text = resources.GetString("mnuSaveNorthArrow_Text");
            subParent.DropDownItems["mnuSaveLegend"].Text = resources.GetString("mnuSaveLegend_Text");

            parent.DropDownItems["mnuProjectSettings"].Text = resources.GetString("mnuProjectSettings_Text");
            parent.DropDownItems["mnuExit"].Text = resources.GetString("mnuExit_Text");
            //图层
            menuStrip1.Items["mnuLayer"].Text = resources.GetString("mnuLayer_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuLayer"];
            parent.DropDownItems["mnuAddLayer"].Text = resources.GetString("mnuAddLayer_Text");
            parent.DropDownItems["mnuAddECWPLayer"].Text = resources.GetString("mnuAddECWPLayer_Text");
            parent.DropDownItems["mnuRemoveLayer"].Text = resources.GetString("mnuRemoveLayer_Text");
            parent.DropDownItems["mnuClearLayer"].Text = resources.GetString("mnuClearLayer_Text");
            parent.DropDownItems["mnuLayerLabels"].Text = resources.GetString("mnuLayerLabels_Text");
            parent.DropDownItems["mnuLayerCharts"].Text = resources.GetString("mnuLayerCharts_Text");
            parent.DropDownItems["mnuLayerAttributeTable"].Text = resources.GetString("mnuLayerAttributeTable_Text");
            parent.DropDownItems["mnuLayerCategories"].Text = resources.GetString("mnuLayerCategories_Text");
            parent.DropDownItems["mnuOptionsManager"].Text = resources.GetString("mnuOptionsManager_Text");
            parent.DropDownItems["mnuLayerRelabel"].Text = resources.GetString("mnuLayerRelabel_Text");
            parent.DropDownItems["mnuQueryLayer"].Text = resources.GetString("mnuQueryLayer_Text");
            parent.DropDownItems["mnuClearSelectedShapes"].Text = resources.GetString("mnuClearSelectedShapes_Text");
            parent.DropDownItems["mnuLayerProperties"].Text = resources.GetString("mnuLayerProperties_Text");
            //视图
            menuStrip1.Items["mnuView"].Text = resources.GetString("mnuView_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuView"];
            parent.DropDownItems["mnuRestoreMenu"].Text = resources.GetString("mnuPanels_Text");

            subParent = (System.Windows.Forms.ToolStripMenuItem)parent.DropDownItems["mnuRestoreMenu"];
            subParent.DropDownItems["mnuLegendVisible"].Text = resources.GetString("mnuShowLegend_Text");
            subParent.DropDownItems["mnuPreviewVisible"].Text = resources.GetString("mnuShowPreviewMap_Text");

            parent.DropDownItems["mnuLanguage"].Text = resources.GetString("mnuLanguages_Text");
            parent.DropDownItems["mnuSetScale"].Text = resources.GetString("mnuSetScale_Text");
            parent.DropDownItems["mnuShowScaleBar"].Text = resources.GetString("mnuShowScaleBar_Text");
            parent.DropDownItems["mnuCopy"].Text = resources.GetString("mnuCopy_Text");

            subParent = (System.Windows.Forms.ToolStripMenuItem)parent.DropDownItems["mnuCopy"];
            subParent.DropDownItems["mnuCopyMap"].Text = resources.GetString("mnuCopyMap_Text");
            subParent.DropDownItems["mnuCopyLegend"].Text = resources.GetString("mnuCopyLegend_Text");
            subParent.DropDownItems["mnuCopyScaleBar"].Text = resources.GetString("mnuCopyScaleBar_Text");
            subParent.DropDownItems["mnuCopyNorthArrow"].Text = resources.GetString("mnuCopyNorthArrow_Text");

            parent.DropDownItems["mnuZoomIn"].Text = resources.GetString("mnuZoomIn_Text");
            parent.DropDownItems["mnuZoomOut"].Text = resources.GetString("mnuZoomOut_Text");
            parent.DropDownItems["mnuZoomToFullExtents"].Text = resources.GetString("mnuZoomToFullExtents_Text");
            parent.DropDownItems["mnuZoomToPreviewExtents"].Text = resources.GetString("mnuZoomToPreviewExtents_Text");
            parent.DropDownItems["mnuPreviousZoom"].Text = resources.GetString("mnuPreviousZoom_Text");
            parent.DropDownItems["mnuNextZoom"].Text = resources.GetString("mnuNextZoom_Text");
            parent.DropDownItems["mnuClearAllSelection"].Text = resources.GetString("mnuClearAllSelection_Text");
            parent.DropDownItems["mnuPreview"].Text = resources.GetString("mnuPreview_Text");

            subParent = (System.Windows.Forms.ToolStripMenuItem)parent.DropDownItems["mnuPreview"];
            subParent.DropDownItems["mnuUpdatePreviewFull"].Text = resources.GetString("mnuUpdatePreviewFull_Text");
            subParent.DropDownItems["mnuUpdatePreviewCurr"].Text = resources.GetString("mnuUpdatePreviewCurr_Text");
            subParent.DropDownItems["mnuClearPreview"].Text = resources.GetString("mnuClearPreview_Text");
            //书签
            menuStrip1.Items["mnuBookmarks"].Text = resources.GetString("mnuBookmarks_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuBookmarks"];
            parent.DropDownItems["mnuBookmarkAdd"].Text = resources.GetString("mnuBookmarkAdd_Text");
            parent.DropDownItems["mnuBookmarkView"].Text = resources.GetString("mnuBookmarkView_Text");
            parent.DropDownItems["mnuBookmarksManager"].Text = resources.GetString("mnuBookmarksManager_Text");
            //插件
            menuStrip1.Items["mnuPlugins"].Text = resources.GetString("mnuPlugins_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuPlugins"];
            parent.DropDownItems["mnuEditPlugins"].Text = resources.GetString("mnuEditPlugins_Text");
            parent.DropDownItems["mnuScript"].Text = resources.GetString("mnuScript_Text");
            //帮助
            menuStrip1.Items["mnuHelp"].Text = resources.GetString("mnuHelp_Text");
            parent = (System.Windows.Forms.ToolStripMenuItem)menuStrip1.Items["mnuHelp"];
            parent.DropDownItems["mnuTutorials"].Text = resources.GetString("mnuTutorials_Text");
            parent.DropDownItems["mnuOnlineDocs"].Text = resources.GetString("mnuOnlineDocs_Text");
            parent.DropDownItems["mnuOfflineDocs"].Text = resources.GetString("mnuOfflineDocs_Text");
            parent.DropDownItems["mnuBugReport"].Text = resources.GetString("mnuBugReport_Text");
            parent.DropDownItems["mnuPluginUpLoading"].Text = resources.GetString("mnuPluginUpLoading_Text");
            parent.DropDownItems["mnuCheckForUpdates"].Text = resources.GetString("mnuCheckUpdates_Text");
            parent.DropDownItems["mnuShortcuts"].Text = resources.GetString("mnuShortcuts_Text");
            parent.DropDownItems["mnuWelcomeScreen"].Text = resources.GetString("mnuWelcomeScreen_Text");
            parent.DropDownItems["mnuAboutMapWindow"].Text = resources.GetString("mnuAboutMapWindow_Text");

            //修改插件中的Text属性。若要修改插件中的显示，首先需要插件提供不同语言的Text值，并且很麻烦。
            //实现思路：
            //首先说好，通过m_PluginManager.Message(string)发送的消息检测是否包含特定语言信息。
            //比如，约定好：string msg以Language:[区域性代号] 格式传送(其中的区域性代号可以不要，只需确定是Language即可)，说明需要修改区域性,并且置handled为false。
            //然后由插件开发者实现将插件创建的菜单和Toolbar移除，然后重新加载。以指定的语言实现和展示。
            //在AppPluginSample示例中有详细实现。
            if (culture != null && culture != "")
            {
                m_PluginManager.Message("Language:" + culture);
            }
            else //使用默认语言
            {
                m_PluginManager.Message("Language:zh-CN");
            }
        }

        #endregion

        #region----------------浮动比例尺----------------

        public void UpdateFloatingScalebar()
        {
            //显示刻度条菜单项不存在，则不显示刻度条
            if (m_Menu == null || m_Menu["mnuShowScaleBar"] == null)
            {
                return;
            }

            m_Menu["mnuShowScaleBar"].Checked = m_FloatingScalebar_Enabled;

            //若没有投影，则刻度条显示没有意义
            if (m_Project.ProjectProjection == string.Empty)
            {
                //m_FloatingScalebar_Enabled = false;
            }
            //若刻度条不显示，则回收其资源
            if (!m_FloatingScalebar_Enabled)
            {
                if (m_FloatingScalebar_PictureBox != null && mapPanel.Contains(m_FloatingScalebar_PictureBox))
                {
                    mapPanel.Controls.Remove(m_FloatingScalebar_PictureBox);
                    //m_FloatingScalebar_PictureBox.Image.Dispose(); //等图片有内容了放开
                    m_FloatingScalebar_PictureBox.Image = null;
                    m_FloatingScalebar_PictureBox.Dispose();
                    m_FloatingScalebar_PictureBox = null;
                }
            }
            else
            {
                //显示刻度条的前提是窗体不是最小化,否则刻度条绘制自己的时候，每个像素的距离将会是无穷大导致范围溢出
                if (this.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                if (m_FloatingScalebar_PictureBox == null)
                {
                    m_FloatingScalebar_PictureBox = new PictureBox();
                    m_FloatingScalebar_PictureBox.MouseClick +=new MouseEventHandler(m_FloatingScalebar_PictureBox_MouseClick);
                }
                //将刻度条添加到窗体中
                if (mapPanel != null && !mapPanel.IsDisposed)
                {
                    if (!mapPanel.Controls.Contains(m_FloatingScalebar_PictureBox))
                    {
                        mapPanel.Controls.Add(m_FloatingScalebar_PictureBox);
                    }
                }

                m_FloatingScalebar_PictureBox.Visible = false;

                ScaleBarUtils sb = new ScaleBarUtils();
                //默认地图单位:米
                UnitOfMeasure mapunit = UnitOfMeasure.Meters;

                if (m_Project.MapUnits != "")
                {
                    //注释，该方法尚未实现
                    //mapunit = MapWinGIS.GeoProcess.UnitConverter.StringToUOM(m_Project.MapUnits);
                }

                //设置刻度条的单位
                UnitOfMeasure ScaleUnit = mapunit;

                //ScaleUnit = MapWinGIS.GeoProcess.UnitConverter.StringToUOM(Program.projInfo.ShowStatusBarCoords_Alternate);

                if (m_FloatingScalebar_ContextMenu_SelectedUnit != "")
                {
                    //ScaleUnit = MapWinGIS.GeoProcess.UnitConverter.StringToUOM(m_FloatingScalebar_ContextMenu_SelectedUnit);
                }

                //不允许显示DecimalDegrees作为单位
                if (ScaleUnit == UnitOfMeasure.DecimalDegrees)
                {
                    ScaleUnit = UnitOfMeasure.Kilometers;
                }

                m_FloatingScalebar_PictureBox.BorderStyle = BorderStyle.FixedSingle;
                m_FloatingScalebar_PictureBox.Image = sb.GenerateScaleBar(((MapWinGIS.Extents)MapMain.Extents), mapunit, ScaleUnit, 300, m_FloatingScalebar_ContextMenu_BackColor, m_FloatingScalebar_ContextMenu_ForeColor);
                m_FloatingScalebar_PictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                if (m_FloatingScalebar_ContextMenu_SelectedPosition == "UpperLeft")
                {
                    m_FloatingScalebar_PictureBox.Location = new System.Drawing.Point(0, 0);
                }
                else if (m_FloatingScalebar_ContextMenu_SelectedPosition == "UpperRight")
                {
                    m_FloatingScalebar_PictureBox.Location = new System.Drawing.Point(MapMain.Width - m_FloatingScalebar_PictureBox.Width, 0);
                }
                else if (m_FloatingScalebar_ContextMenu_SelectedPosition == "LowerLeft")
                {
                    m_FloatingScalebar_PictureBox.Location = new System.Drawing.Point(0, MapMain.Height - m_FloatingScalebar_PictureBox.Height);
                }
                else if (m_FloatingScalebar_ContextMenu_SelectedPosition == "LowerRight")
                {
                    m_FloatingScalebar_PictureBox.Location = new System.Drawing.Point(MapMain.Width - m_FloatingScalebar_PictureBox.Width, MapMain.Height - m_FloatingScalebar_PictureBox.Height);
                }
                else
                {
                    m_FloatingScalebar_PictureBox.Location = new System.Drawing.Point(MapMain.Width - m_FloatingScalebar_PictureBox.Width, MapMain.Height - m_FloatingScalebar_PictureBox.Height);
                }

                m_FloatingScalebar_PictureBox.BringToFront();
                m_FloatingScalebar_PictureBox.Visible = true;
            }
        }

        private void m_FloatingScalebar_PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //若位置显示不正确。
                //m_FloatingScalebar_ContextMenu.Show(m_FloatingScalebar_PictureBox, m_FloatingScalebar_PictureBox.PointToClient(MousePosition));
                m_FloatingScalebar_ContextMenu.Show(this, this.PointToClient(Cursor.Position));
            }
            return;
        }
        //左上
        private void FloatingScalebar_UpperLeft_Click(object sender, EventArgs e)
        {
            m_FloatingScalebar_ContextMenu_SelectedPosition = "UpperLeft";
            m_FloatingScalebar_ContextMenu_UL.Checked = true;
            m_FloatingScalebar_ContextMenu_UR.Checked = false;
            m_FloatingScalebar_ContextMenu_LL.Checked = false;
            m_FloatingScalebar_ContextMenu_LR.Checked = false;

            SetModified(true);
            UpdateFloatingScalebar();
        }
        //右上
        private void FloatingScalebar_UpperRight_Click(object sender, EventArgs e)
        {
            m_FloatingScalebar_ContextMenu_SelectedPosition = "UpperRight";
            m_FloatingScalebar_ContextMenu_UL.Checked = false;
            m_FloatingScalebar_ContextMenu_UR.Checked = true;
            m_FloatingScalebar_ContextMenu_LL.Checked = false;
            m_FloatingScalebar_ContextMenu_LR.Checked = false;

            SetModified(true);
            UpdateFloatingScalebar();
        }
        //左下
        private void FloatingScalebar_LowerLeft_Click(object sender, EventArgs e)
        {
            m_FloatingScalebar_ContextMenu_SelectedPosition = "LowerLeft";
            m_FloatingScalebar_ContextMenu_UL.Checked = false;
            m_FloatingScalebar_ContextMenu_UR.Checked = false;
            m_FloatingScalebar_ContextMenu_LL.Checked = true;
            m_FloatingScalebar_ContextMenu_LR.Checked = false;

            SetModified(true);
            UpdateFloatingScalebar();
        }
        //右下
        private void FloatingScalebar_LowerRight_Click(object sender, EventArgs e)
        {
            m_FloatingScalebar_ContextMenu_SelectedPosition = "LowerRight";
            m_FloatingScalebar_ContextMenu_UL.Checked = false;
            m_FloatingScalebar_ContextMenu_UR.Checked = false;
            m_FloatingScalebar_ContextMenu_LL.Checked = false;
            m_FloatingScalebar_ContextMenu_LR.Checked = true;

            SetModified(true);
            UpdateFloatingScalebar();
        }
        //字体颜色
        private void FloatingScalebar_ChooseForecolor_Click(object sender, EventArgs e)
        {
            ColorPickerSingle picker = new ColorPickerSingle();
            if (picker.ShowDialog() == DialogResult.OK)
            {
                //m_FloatingScalebar_ContextMenu_ForeColor = picker.btnStartColor.BackColor;
                SetModified(true);
                UpdateFloatingScalebar();
            }
        }
        //背景颜色
        private void FloatingScalebar_ChooseBackcolor_Click(object sender, EventArgs e)
        {
            ColorPickerSingle picker = new ColorPickerSingle();
            if (picker.ShowDialog() == DialogResult.OK)
            {
                //m_FloatingScalebar_ContextMenu_BackColor = picker.btnStartColor.BackColor;
                SetModified(true);
                UpdateFloatingScalebar();
            }
        }
        //单位选择
        private void FloatingScalebar_ChangeUnits_Click(object sender, EventArgs e)
        {
            ChooseDisplayUnits chooseUnits = new ChooseDisplayUnits();
            if (chooseUnits.ShowDialog() == DialogResult.OK)
            {
                //m_FloatingScalebar_ContextMenu_SelectedUnit = chooseUnits.list.Items(chooseUnits.list.SelectedIndex);
                SetModified(true);
                UpdateFloatingScalebar();
            }
        }


        #endregion

        #region ---------------公共方法-------------------
        /// <summary>
        /// 设置Modified（指示项目是否修改），重写标题，显示版本，更新刻度条
        /// </summary>
        /// <param name="Status"></param>
        public void SetModified(bool status)
        {
            //ProjectFileName和layers为空，则阻止设置Modified
            if (Program.projInfo != null && (Program.projInfo.ProjectFileName == null || Program.projInfo.ProjectFileName.Trim() == "") && MapMain.NumLayers == 0)
            {
                status = false;
            }
            //同步Modifie  ----难道地图一加载就等于修改了吗？----------
            if (Program.projInfo.Modified != status)
            {
                Program.projInfo.Modified = status;
                UpdateButtons();
            }
            MapMain.ShowVersionNumber = Program.appInfo.ShowMapWinGISVersion;
            MapMain.ShowRedrawTime = Program.appInfo.ShowRedrawSpeed;
            //更新浮动比例尺
            m_FloatingScalebar_Enabled = Program.appInfo.ShowFloatingScalebar;
            UpdateFloatingScalebar();

            // added avoid collision property
            for (int i = 0; i < m_Layers.Count(); i++)
            {
                Interfaces.Layer layer = m_Layers[m_Layers.GetHandle(i)]; //fixed the error at removing the layer
            }
            //  added Auto Create Spatial Index property
            if (m_Layers.NumLayers > 0)
            {
                MapWinGIS.Shapefile sf = m_Layers[m_Layers.CurrentLayer].GetObject() as MapWinGIS.Shapefile;           
                MapMain.Redraw();
            }

            //更新窗体的标题显示
            if (Program.projInfo.ProjectFileName == "" && Program.projInfo.ProjectFileName.Length < 1) //主标题+自定义标题+是否修改标识(*)
            {
                this.Text = Program.appInfo.ApplicationName + " " + (CustomWindowTitle == "" ? "" : " - ") + CustomWindowTitle + (status ? "*" : "");
            }
            else //主标题+自定义标题+项目路径+是否修改标识(*)
            {
                this.Text = Program.appInfo.ApplicationName + " " +
                    (CustomWindowTitle == "" ? "" : " - ") + CustomWindowTitle + " - " +(this.Title_ShowFullProjectPath ? Program.projInfo.ProjectFileName : (Path.GetFileNameWithoutExtension(Program.projInfo.ProjectFileName))) + (status ? "*" : "");
            }

        }

        internal void ClearPreview()
        {
            if (!(previewPanel.IsDisposed))
            {
                Program.frmMain.MapPreview.ClearDrawings();
                Program.frmMain.MapPreview.RemoveAllLayers();
                UpdateButtons();
            }
        }

        public void DoOpenIntoCurrent(string fileName = "") { }


        /// <summary>
        /// 提示保存项目消息框
        /// </summary>
        internal DialogResult PromptToSaveProject()
        {
            SaveFileDialog cdlSave = new SaveFileDialog();
            DialogResult result;

            cdlSave.Filter = "MapWindow Project (*.mwprj)|*.mwprj";
            if (System.IO.Path.GetFileNameWithoutExtension(Program.projInfo.ProjectFileName) == "")
            {
                result = Program.messageBox.Display(resources.GetString("msgSaveProject1_Text") + resources.GetString("msgSaveProject2_Text"), Program.appInfo.ApplicationName, Utility.MessageButton.YesNoCancel);
                //result = MessageBox.Show(resources.GetString("msgSaveProject1_Text") + resources.GetString("msgSaveProject2_Text"), Program.appInfo.ApplicationName, MessageBoxButtons.YesNoCancel);
            }
            else
            {
                result = Program.messageBox.Display(resources.GetString("msgSaveProject1.text") + System.IO.Path.GetFileNameWithoutExtension(Program.projInfo.ProjectFileName) + "?", Program.appInfo.ApplicationName, Utility.MessageButton.YesNoCancel);
                //result = MessageBox.Show(resources.GetString("msgSaveProject1.text") + System.IO.Path.GetFileNameWithoutExtension(Program.projInfo.ProjectFileName) + "?", Program.appInfo.ApplicationName, MessageBoxButtons.YesNoCancel);
            }

            switch (result)
            {
                case DialogResult.Yes:
                    if (m_HasBeenSaved == true && Program.projInfo.ProjectFileName.Length < 1 && Program.projInfo.ProjectFileName == null)
                    {
                        Program.projInfo.SaveProject();
                        m_HasBeenSaved = true;
                        SetModified(false);
                    }
                    else
                    {
                        cdlSave.InitialDirectory = Program.appInfo.DefaultDir;
                        if (cdlSave.ShowDialog() == DialogResult.Cancel)
                        {
                            return DialogResult.Cancel;
                        }

                        if (System.IO.Path.GetExtension(cdlSave.FileName) != ".mwprj")
                        {
                            cdlSave.FileName += ".mwprj";
                        }
                        Program.projInfo.ProjectFileName = cdlSave.FileName;
                        Program.projInfo.SaveProject();
                        m_HasBeenSaved = true;
                        Program.projInfo.ProjectFileName = cdlSave.FileName;
                        SetModified(false);

                    }
                    return DialogResult.Yes;
                case DialogResult.Cancel:
                    return DialogResult.Cancel;
                case DialogResult.No:
                    SetModified(false);
                    return DialogResult.No;
                default:
                    return DialogResult.Cancel;
            }
            
        }

        /// <summary>
        /// 获取鼠标相对于屏幕的坐标点
        /// </summary>
        public static System.Drawing.Point GetCursorLocation()
        {
            WinApi32.POINTAPI pnt = new WinApi32.POINTAPI();
            WinApi32.GetCursorPos(ref pnt);
            return new System.Drawing.Point(pnt.x, pnt.y);
        }

        /// <summary>
        /// 检查给定的点是否在窗体内部
        /// </summary>
        private bool InMyFormBounds(System.Drawing.Point pt)
        {
            if (pt.X < this.Location.X + this.Width && pt.X > this.Location.X && pt.Y < this.Location.Y + this.Height && pt.Y > this.Location.Y)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算给点的x,y坐标是否在矩形框内
        /// </summary>
        private bool InBox(Rectangle rect, double x, double y)
        {
            if (x >= rect.Left && x <= rect.Right && y <= rect.Bottom && y >= rect.Top)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 格式化根据"Measure distance"or "Measure Area" 函数计算出来的值。
        /// 该值显示在Statutsbar上
        /// </summary>
        internal string FormatDistance(double dist)
        {
            int decimals = Program.projInfo.StatusBarAlternateCoordsNumDecimals;
            bool useCommas = Program.projInfo.StatusBarAlternateCoordsUseCommas;

            string nf;

            if (useCommas == true)
            {
                nf = "N" + decimals.ToString();
            }
            else
            {
                nf = "F" + decimals.ToString();
            }

            return dist.ToString(nf, CultureInfo.InvariantCulture);

        }

        /// <summary>
        /// 获取当前地图的比例
        /// </summary>
        public string GetCurrentScale()
        {
            return m_View.Scale.ToString(CultureInfo.InvariantCulture);
        }

        private void SetScale(string newScale)
        {
            if (!Information.IsNumeric(newScale))
            {
                return;
            }
            m_View.Scale = Double.Parse(newScale, CultureInfo.InvariantCulture);
        }

        public MapWinGIS.Extents ScaleToExtents(double scale, MapWinGIS.Extents ext)
        {
            MapWinGIS.Point pt = new MapWinGIS.Point();
            pt.x = (ext.xMin + ext.xMax) / 2;
            pt.y = (ext.yMin + ext.yMax) / 2;

            MapWinGIS.Extents newExtents = new MapWinGIS.Extents();
            if (scale == 0 || Project.MapUnits == "")
            {
                newExtents.SetBounds(pt.x, pt.y, 0, pt.x, pt.y, 0);
            }
            else
            {
                //此处注释，ScaleTools.ExtentFromScale尚未编写
                //newExtents = MapWinGIS.GeoProcess.ScaleTools.ExtentFromScale(Convert.ToInt32(scale), pt, Project.MapUnits, MapMain.Width, MapMain.Height);
            }
            return newExtents;
        }


        /// <summary>
        /// 移除m_Extents中索引m_CurrentExtent之后的记录
        /// 当我前进几次后，又改变了extent，这时就需要将前进的那几次extent去掉，存储刚刚改变的extent。
        /// </summary>
        private void FlushForwardHistory()
        {
            int i;
            MapWinGIS.Utility.Logger.Dbg("在FlushForwardHistory方法中，当前的extent:" + m_CurrentExtent.ToString());
            if (m_Extents.Count > 0)
            {
                if (m_CurrentExtent < m_Extents.Count - 1)
                {
                    for (i = m_Extents.Count - 1; i > m_CurrentExtent; i--)
                    {
                        m_Extents.RemoveAt(i);
                    }
                }
                else
                {
                    m_CurrentExtent = m_Extents.Count - 1;
                }
            }

        }





        #endregion



    }

    #region enum和StringPairSorter
    internal class StringPairSorter : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            //空
            return 1;
        }
    }

    public enum GeoTIFFAndImgBehavior
    {
        LoadAsImage = 0,
        LoadAsGrid = 1,
        Automatic = 2
    }

    public enum ESRIBehavior
    {
        LoadAsImage = 0,
        LoadAsGrid = 1
    }

    public enum MouseWheelZoomDir
    {
        WheelUpZoomsIn = 0,
        WheelUpZoomsOut = 1,
        NoAction = 2
    }
    #endregion
}
