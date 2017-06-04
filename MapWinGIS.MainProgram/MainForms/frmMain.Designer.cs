namespace MapWinGIS.MainProgram
{
    partial class MapWinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapWinForm));
            this.StripDocker = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlbStandard = new MapWinGIS.ToolStripExtensions.ToolStripEx();
            this.ContextToolStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToggleTextLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbbNew = new System.Windows.Forms.ToolStripButton();
            this.tbbOpen = new System.Windows.Forms.ToolStripButton();
            this.tbbSave = new System.Windows.Forms.ToolStripButton();
            this.tbbPrint = new System.Windows.Forms.ToolStripButton();
            this.tbbProjectSettings = new System.Windows.Forms.ToolStripButton();
            this.tlbLayers = new MapWinGIS.ToolStripExtensions.ToolStripEx();
            this.tbbAddLayer = new System.Windows.Forms.ToolStripButton();
            this.tbbRemoveLayer = new System.Windows.Forms.ToolStripButton();
            this.tbbClearLayers = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbSymbologyManager = new System.Windows.Forms.ToolStripButton();
            this.tbbLayerProperties = new System.Windows.Forms.ToolStripButton();
            this.tlbMain = new MapWinGIS.ToolStripExtensions.ToolStripEx();
            this.tbbSelect = new System.Windows.Forms.ToolStripButton();
            this.tbbDeSelectLayer = new System.Windows.Forms.ToolStripButton();
            this.tlbZoom = new MapWinGIS.ToolStripExtensions.ToolStripEx();
            this.tbbPan = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomIn = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomOut = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomExtent = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomSelected = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomPrevious = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomNext = new System.Windows.Forms.ToolStripButton();
            this.tbbZoomLayer = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new MapWinGIS.ToolStripExtensions.MenuStripEx();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mnuLegend = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveLayerOrGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearLayers = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomToLayerOrGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuBreak1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRelabel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLabelSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChartsSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeeMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLegendShapefileCategories = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTableEditorLaunch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAsLayerFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExpandGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCollapseGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuBreak2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLayerButton = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuBtnAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBtnRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBtnClear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomPan = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomMax = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuZoomPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomShape = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomPreviewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.MapPreview = new AxMapWinGIS.AxMap();
            this.MapMain = new AxMapWinGIS.AxMap();
            this.PreviewMapContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuPreviewExtents = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPreviewCurrent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPreviewClear = new System.Windows.Forms.ToolStripMenuItem();
            this.StripDocker.ContentPanel.SuspendLayout();
            this.StripDocker.TopToolStripPanel.SuspendLayout();
            this.StripDocker.SuspendLayout();
            this.tlbStandard.SuspendLayout();
            this.ContextToolStrip.SuspendLayout();
            this.tlbLayers.SuspendLayout();
            this.tlbMain.SuspendLayout();
            this.tlbZoom.SuspendLayout();
            this.mnuLegend.SuspendLayout();
            this.mnuLayerButton.SuspendLayout();
            this.mnuZoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapMain)).BeginInit();
            this.PreviewMapContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // StripDocker
            // 
            // 
            // StripDocker.ContentPanel
            // 
            this.StripDocker.ContentPanel.Controls.Add(this.panel1);
            resources.ApplyResources(this.StripDocker.ContentPanel, "StripDocker.ContentPanel");
            resources.ApplyResources(this.StripDocker, "StripDocker");
            this.StripDocker.Name = "StripDocker";
            // 
            // StripDocker.TopToolStripPanel
            // 
            resources.ApplyResources(this.StripDocker.TopToolStripPanel, "StripDocker.TopToolStripPanel");
            this.StripDocker.TopToolStripPanel.Controls.Add(this.tlbStandard);
            this.StripDocker.TopToolStripPanel.Controls.Add(this.tlbLayers);
            this.StripDocker.TopToolStripPanel.Controls.Add(this.tlbMain);
            this.StripDocker.TopToolStripPanel.Controls.Add(this.tlbZoom);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tlbStandard
            // 
            this.tlbStandard.AllowItemReorder = true;
            this.tlbStandard.AllowMerge = false;
            this.tlbStandard.ClickThrough = true;
            this.tlbStandard.ContextMenuStrip = this.ContextToolStrip;
            resources.ApplyResources(this.tlbStandard, "tlbStandard");
            this.tlbStandard.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlbStandard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbNew,
            this.tbbOpen,
            this.tbbSave,
            this.tbbPrint,
            this.tbbProjectSettings});
            this.tlbStandard.Name = "tlbStandard";
            this.tlbStandard.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.tlbStandard.SuppressHighlighting = true;
            this.tlbStandard.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tlbStandard_ItemClicked);
            // 
            // ContextToolStrip
            // 
            this.ContextToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToggleTextLabelsToolStripMenuItem});
            this.ContextToolStrip.Name = "ContextToolStrip";
            resources.ApplyResources(this.ContextToolStrip, "ContextToolStrip");
            this.ContextToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ContextToolStrip_ItemClicked);
            // 
            // ToggleTextLabelsToolStripMenuItem
            // 
            this.ToggleTextLabelsToolStripMenuItem.Image = global::MapWinGIS.MainProgram.GlobalResource.imgClose;
            this.ToggleTextLabelsToolStripMenuItem.Name = "ToggleTextLabelsToolStripMenuItem";
            resources.ApplyResources(this.ToggleTextLabelsToolStripMenuItem, "ToggleTextLabelsToolStripMenuItem");
            // 
            // tbbNew
            // 
            resources.ApplyResources(this.tbbNew, "tbbNew");
            this.tbbNew.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbNew.Image = global::MapWinGIS.MainProgram.GlobalResource.project;
            this.tbbNew.Name = "tbbNew";
            // 
            // tbbOpen
            // 
            this.tbbOpen.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbOpen.Image = global::MapWinGIS.MainProgram.GlobalResource.open;
            resources.ApplyResources(this.tbbOpen, "tbbOpen");
            this.tbbOpen.Name = "tbbOpen";
            // 
            // tbbSave
            // 
            this.tbbSave.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbSave.Image = global::MapWinGIS.MainProgram.GlobalResource.saveNew;
            resources.ApplyResources(this.tbbSave, "tbbSave");
            this.tbbSave.Name = "tbbSave";
            // 
            // tbbPrint
            // 
            this.tbbPrint.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbPrint.Image = global::MapWinGIS.MainProgram.GlobalResource.print;
            resources.ApplyResources(this.tbbPrint, "tbbPrint");
            this.tbbPrint.Name = "tbbPrint";
            // 
            // tbbProjectSettings
            // 
            this.tbbProjectSettings.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbProjectSettings.Image = global::MapWinGIS.MainProgram.GlobalResource.project_settings;
            resources.ApplyResources(this.tbbProjectSettings, "tbbProjectSettings");
            this.tbbProjectSettings.Name = "tbbProjectSettings";
            // 
            // tlbLayers
            // 
            this.tlbLayers.AllowItemReorder = true;
            this.tlbLayers.ClickThrough = true;
            this.tlbLayers.ContextMenuStrip = this.ContextToolStrip;
            resources.ApplyResources(this.tlbLayers, "tlbLayers");
            this.tlbLayers.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlbLayers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbAddLayer,
            this.tbbRemoveLayer,
            this.tbbClearLayers,
            this.toolStripSeparator1,
            this.tbbSymbologyManager,
            this.tbbLayerProperties});
            this.tlbLayers.Name = "tlbLayers";
            this.tlbLayers.SuppressHighlighting = true;
            this.tlbLayers.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tlbLayers_ItemClicked);
            // 
            // tbbAddLayer
            // 
            this.tbbAddLayer.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbAddLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_add;
            resources.ApplyResources(this.tbbAddLayer, "tbbAddLayer");
            this.tbbAddLayer.Name = "tbbAddLayer";
            // 
            // tbbRemoveLayer
            // 
            this.tbbRemoveLayer.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbRemoveLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_remove;
            resources.ApplyResources(this.tbbRemoveLayer, "tbbRemoveLayer");
            this.tbbRemoveLayer.Name = "tbbRemoveLayer";
            // 
            // tbbClearLayers
            // 
            this.tbbClearLayers.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbClearLayers.Image = global::MapWinGIS.MainProgram.GlobalResource.remove_all_layers;
            resources.ApplyResources(this.tbbClearLayers, "tbbClearLayers");
            this.tbbClearLayers.Name = "tbbClearLayers";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbbSymbologyManager
            // 
            this.tbbSymbologyManager.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbSymbologyManager.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_symbology;
            resources.ApplyResources(this.tbbSymbologyManager, "tbbSymbologyManager");
            this.tbbSymbologyManager.Name = "tbbSymbologyManager";
            // 
            // tbbLayerProperties
            // 
            this.tbbLayerProperties.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbLayerProperties.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_properties;
            resources.ApplyResources(this.tbbLayerProperties, "tbbLayerProperties");
            this.tbbLayerProperties.Name = "tbbLayerProperties";
            // 
            // tlbMain
            // 
            this.tlbMain.AllowItemReorder = true;
            this.tlbMain.ClickThrough = true;
            this.tlbMain.ContextMenuStrip = this.ContextToolStrip;
            resources.ApplyResources(this.tlbMain, "tlbMain");
            this.tlbMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbSelect,
            this.tbbDeSelectLayer});
            this.tlbMain.Name = "tlbMain";
            this.tlbMain.SuppressHighlighting = true;
            this.tlbMain.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tlbMain_ItemClicked);
            // 
            // tbbSelect
            // 
            this.tbbSelect.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbSelect.Image = global::MapWinGIS.MainProgram.GlobalResource.selectNew;
            resources.ApplyResources(this.tbbSelect, "tbbSelect");
            this.tbbSelect.Name = "tbbSelect";
            // 
            // tbbDeSelectLayer
            // 
            this.tbbDeSelectLayer.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbDeSelectLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.deselect;
            resources.ApplyResources(this.tbbDeSelectLayer, "tbbDeSelectLayer");
            this.tbbDeSelectLayer.Name = "tbbDeSelectLayer";
            // 
            // tlbZoom
            // 
            this.tlbZoom.AllowItemReorder = true;
            this.tlbZoom.ClickThrough = true;
            this.tlbZoom.ContextMenuStrip = this.ContextToolStrip;
            resources.ApplyResources(this.tlbZoom, "tlbZoom");
            this.tlbZoom.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlbZoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbPan,
            this.tbbZoomIn,
            this.tbbZoomOut,
            this.tbbZoomExtent,
            this.tbbZoomSelected,
            this.tbbZoomPrevious,
            this.tbbZoomNext,
            this.tbbZoomLayer});
            this.tlbZoom.Name = "tlbZoom";
            this.tlbZoom.SuppressHighlighting = true;
            this.tlbZoom.Tag = "";
            this.tlbZoom.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tlbZoom_ItemClicked);
            // 
            // tbbPan
            // 
            this.tbbPan.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbPan.Image = global::MapWinGIS.MainProgram.GlobalResource.pan;
            resources.ApplyResources(this.tbbPan, "tbbPan");
            this.tbbPan.Name = "tbbPan";
            // 
            // tbbZoomIn
            // 
            this.tbbZoomIn.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomIn.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_inNew;
            resources.ApplyResources(this.tbbZoomIn, "tbbZoomIn");
            this.tbbZoomIn.Name = "tbbZoomIn";
            // 
            // tbbZoomOut
            // 
            this.tbbZoomOut.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomOut.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_outNew;
            resources.ApplyResources(this.tbbZoomOut, "tbbZoomOut");
            this.tbbZoomOut.Name = "tbbZoomOut";
            // 
            // tbbZoomExtent
            // 
            this.tbbZoomExtent.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomExtent.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_extentNew;
            resources.ApplyResources(this.tbbZoomExtent, "tbbZoomExtent");
            this.tbbZoomExtent.Name = "tbbZoomExtent";
            // 
            // tbbZoomSelected
            // 
            this.tbbZoomSelected.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomSelected.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_selectionNew;
            resources.ApplyResources(this.tbbZoomSelected, "tbbZoomSelected");
            this.tbbZoomSelected.Name = "tbbZoomSelected";
            // 
            // tbbZoomPrevious
            // 
            this.tbbZoomPrevious.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomPrevious.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_lastNew;
            resources.ApplyResources(this.tbbZoomPrevious, "tbbZoomPrevious");
            this.tbbZoomPrevious.Name = "tbbZoomPrevious";
            // 
            // tbbZoomNext
            // 
            this.tbbZoomNext.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomNext.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_nextNew;
            resources.ApplyResources(this.tbbZoomNext, "tbbZoomNext");
            this.tbbZoomNext.Name = "tbbZoomNext";
            // 
            // tbbZoomLayer
            // 
            this.tbbZoomLayer.ForeColor = System.Drawing.Color.DarkBlue;
            this.tbbZoomLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_layerNew;
            resources.ApplyResources(this.tbbZoomLayer, "tbbZoomLayer");
            this.tbbZoomLayer.Name = "tbbZoomLayer";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ClickThrough = true;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.SuppressHighlighting = true;
            this.menuStrip1.LocationChanged += new System.EventHandler(this.menuStrip1_LocationChanged);
            // 
            // mnuLegend
            // 
            this.mnuLegend.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddGroup,
            this.mnuAddLayer,
            this.mnuRemoveLayerOrGroup,
            this.mnuClearLayers,
            this.mnuZoomToLayerOrGroup,
            this.toolStripMenuBreak1,
            this.mnuRelabel,
            this.mnuLabelSetup,
            this.mnuChartsSetup,
            this.mnuSeeMetadata,
            this.mnuLegendShapefileCategories,
            this.mnuTableEditorLaunch,
            this.mnuSaveAsLayerFile,
            this.ToolStripSeparator2,
            this.mnuExpandGroups,
            this.mnuExpandAll,
            this.mnuCollapseGroups,
            this.mnuCollapseAll,
            this.toolStripMenuBreak2,
            this.mnuProperties});
            this.mnuLegend.Name = "contextMenuStrip1";
            resources.ApplyResources(this.mnuLegend, "mnuLegend");
            // 
            // mnuAddGroup
            // 
            this.mnuAddGroup.Image = global::MapWinGIS.MainProgram.GlobalResource.imgGroupAdd;
            this.mnuAddGroup.Name = "mnuAddGroup";
            resources.ApplyResources(this.mnuAddGroup, "mnuAddGroup");
            this.mnuAddGroup.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuAddLayer
            // 
            this.mnuAddLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_add;
            this.mnuAddLayer.Name = "mnuAddLayer";
            resources.ApplyResources(this.mnuAddLayer, "mnuAddLayer");
            this.mnuAddLayer.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuRemoveLayerOrGroup
            // 
            this.mnuRemoveLayerOrGroup.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_remove;
            this.mnuRemoveLayerOrGroup.Name = "mnuRemoveLayerOrGroup";
            resources.ApplyResources(this.mnuRemoveLayerOrGroup, "mnuRemoveLayerOrGroup");
            this.mnuRemoveLayerOrGroup.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuClearLayers
            // 
            this.mnuClearLayers.Image = global::MapWinGIS.MainProgram.GlobalResource.remove_all_layers;
            this.mnuClearLayers.Name = "mnuClearLayers";
            resources.ApplyResources(this.mnuClearLayers, "mnuClearLayers");
            this.mnuClearLayers.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuZoomToLayerOrGroup
            // 
            this.mnuZoomToLayerOrGroup.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_layerNew;
            this.mnuZoomToLayerOrGroup.Name = "mnuZoomToLayerOrGroup";
            resources.ApplyResources(this.mnuZoomToLayerOrGroup, "mnuZoomToLayerOrGroup");
            this.mnuZoomToLayerOrGroup.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // toolStripMenuBreak1
            // 
            this.toolStripMenuBreak1.Name = "toolStripMenuBreak1";
            resources.ApplyResources(this.toolStripMenuBreak1, "toolStripMenuBreak1");
            // 
            // mnuRelabel
            // 
            this.mnuRelabel.Image = global::MapWinGIS.MainProgram.GlobalResource.label_reload;
            this.mnuRelabel.Name = "mnuRelabel";
            resources.ApplyResources(this.mnuRelabel, "mnuRelabel");
            this.mnuRelabel.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuLabelSetup
            // 
            this.mnuLabelSetup.Image = global::MapWinGIS.MainProgram.GlobalResource.label_properties;
            this.mnuLabelSetup.Name = "mnuLabelSetup";
            resources.ApplyResources(this.mnuLabelSetup, "mnuLabelSetup");
            this.mnuLabelSetup.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuChartsSetup
            // 
            this.mnuChartsSetup.Image = global::MapWinGIS.MainProgram.GlobalResource.charts_properties;
            this.mnuChartsSetup.Name = "mnuChartsSetup";
            resources.ApplyResources(this.mnuChartsSetup, "mnuChartsSetup");
            this.mnuChartsSetup.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuSeeMetadata
            // 
            this.mnuSeeMetadata.Image = global::MapWinGIS.MainProgram.GlobalResource.imgMetadata;
            this.mnuSeeMetadata.Name = "mnuSeeMetadata";
            resources.ApplyResources(this.mnuSeeMetadata, "mnuSeeMetadata");
            this.mnuSeeMetadata.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuLegendShapefileCategories
            // 
            this.mnuLegendShapefileCategories.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_categories;
            this.mnuLegendShapefileCategories.Name = "mnuLegendShapefileCategories";
            resources.ApplyResources(this.mnuLegendShapefileCategories, "mnuLegendShapefileCategories");
            this.mnuLegendShapefileCategories.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuTableEditorLaunch
            // 
            this.mnuTableEditorLaunch.Image = global::MapWinGIS.MainProgram.GlobalResource.table_editor;
            this.mnuTableEditorLaunch.Name = "mnuTableEditorLaunch";
            resources.ApplyResources(this.mnuTableEditorLaunch, "mnuTableEditorLaunch");
            this.mnuTableEditorLaunch.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuSaveAsLayerFile
            // 
            this.mnuSaveAsLayerFile.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_symbology;
            this.mnuSaveAsLayerFile.Name = "mnuSaveAsLayerFile";
            resources.ApplyResources(this.mnuSaveAsLayerFile, "mnuSaveAsLayerFile");
            this.mnuSaveAsLayerFile.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            resources.ApplyResources(this.ToolStripSeparator2, "ToolStripSeparator2");
            // 
            // mnuExpandGroups
            // 
            this.mnuExpandGroups.Image = global::MapWinGIS.MainProgram.GlobalResource.group_expand;
            this.mnuExpandGroups.Name = "mnuExpandGroups";
            resources.ApplyResources(this.mnuExpandGroups, "mnuExpandGroups");
            this.mnuExpandGroups.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuExpandAll
            // 
            this.mnuExpandAll.Image = global::MapWinGIS.MainProgram.GlobalResource.imgExpandAll;
            this.mnuExpandAll.Name = "mnuExpandAll";
            resources.ApplyResources(this.mnuExpandAll, "mnuExpandAll");
            this.mnuExpandAll.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuCollapseGroups
            // 
            this.mnuCollapseGroups.Image = global::MapWinGIS.MainProgram.GlobalResource.group_collapse;
            this.mnuCollapseGroups.Name = "mnuCollapseGroups";
            resources.ApplyResources(this.mnuCollapseGroups, "mnuCollapseGroups");
            this.mnuCollapseGroups.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuCollapseAll
            // 
            this.mnuCollapseAll.Image = global::MapWinGIS.MainProgram.GlobalResource.imgCollapseAll;
            this.mnuCollapseAll.Name = "mnuCollapseAll";
            resources.ApplyResources(this.mnuCollapseAll, "mnuCollapseAll");
            this.mnuCollapseAll.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // toolStripMenuBreak2
            // 
            this.toolStripMenuBreak2.Name = "toolStripMenuBreak2";
            resources.ApplyResources(this.toolStripMenuBreak2, "toolStripMenuBreak2");
            // 
            // mnuProperties
            // 
            this.mnuProperties.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_properties;
            this.mnuProperties.Name = "mnuProperties";
            resources.ApplyResources(this.mnuProperties, "mnuProperties");
            this.mnuProperties.Click += new System.EventHandler(this.mnuLegend_ItemClicked);
            // 
            // mnuLayerButton
            // 
            this.mnuLayerButton.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBtnAdd,
            this.mnuBtnRemove,
            this.mnuBtnClear});
            this.mnuLayerButton.Name = "mnuLayerButtom";
            resources.ApplyResources(this.mnuLayerButton, "mnuLayerButton");
            // 
            // mnuBtnAdd
            // 
            this.mnuBtnAdd.Checked = true;
            this.mnuBtnAdd.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.mnuBtnAdd, "mnuBtnAdd");
            this.mnuBtnAdd.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_add;
            this.mnuBtnAdd.Name = "mnuBtnAdd";
            // 
            // mnuBtnRemove
            // 
            resources.ApplyResources(this.mnuBtnRemove, "mnuBtnRemove");
            this.mnuBtnRemove.Image = global::MapWinGIS.MainProgram.GlobalResource.layer_remove;
            this.mnuBtnRemove.Name = "mnuBtnRemove";
            // 
            // mnuBtnClear
            // 
            resources.ApplyResources(this.mnuBtnClear, "mnuBtnClear");
            this.mnuBtnClear.Image = global::MapWinGIS.MainProgram.GlobalResource.remove_all_layers;
            this.mnuBtnClear.Name = "mnuBtnClear";
            // 
            // mnuZoom
            // 
            this.mnuZoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuZoomIn,
            this.mnuZoomOut,
            this.mnuZoomPan,
            this.mnuZoomMax,
            this.toolStripSeparator4,
            this.mnuZoomPrevious,
            this.mnuZoomNext,
            this.toolStripSeparator3,
            this.mnuSelect,
            this.mnuZoomShape,
            this.mnuZoomPreviewMap,
            this.mnuZoomLayer});
            this.mnuZoom.Name = "mnuZoom";
            resources.ApplyResources(this.mnuZoom, "mnuZoom");
            // 
            // mnuZoomIn
            // 
            this.mnuZoomIn.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_inNew;
            this.mnuZoomIn.Name = "mnuZoomIn";
            resources.ApplyResources(this.mnuZoomIn, "mnuZoomIn");
            this.mnuZoomIn.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomOut
            // 
            this.mnuZoomOut.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_outNew;
            this.mnuZoomOut.Name = "mnuZoomOut";
            resources.ApplyResources(this.mnuZoomOut, "mnuZoomOut");
            this.mnuZoomOut.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomPan
            // 
            this.mnuZoomPan.Image = global::MapWinGIS.MainProgram.GlobalResource.pan;
            this.mnuZoomPan.Name = "mnuZoomPan";
            resources.ApplyResources(this.mnuZoomPan, "mnuZoomPan");
            this.mnuZoomPan.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomMax
            // 
            this.mnuZoomMax.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_extentNew;
            this.mnuZoomMax.Name = "mnuZoomMax";
            resources.ApplyResources(this.mnuZoomMax, "mnuZoomMax");
            this.mnuZoomMax.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // mnuZoomPrevious
            // 
            this.mnuZoomPrevious.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_lastNew;
            this.mnuZoomPrevious.Name = "mnuZoomPrevious";
            resources.ApplyResources(this.mnuZoomPrevious, "mnuZoomPrevious");
            this.mnuZoomPrevious.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomNext
            // 
            this.mnuZoomNext.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_nextNew;
            this.mnuZoomNext.Name = "mnuZoomNext";
            resources.ApplyResources(this.mnuZoomNext, "mnuZoomNext");
            this.mnuZoomNext.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // mnuSelect
            // 
            this.mnuSelect.Image = global::MapWinGIS.MainProgram.GlobalResource.selectNew;
            this.mnuSelect.Name = "mnuSelect";
            resources.ApplyResources(this.mnuSelect, "mnuSelect");
            this.mnuSelect.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomShape
            // 
            this.mnuZoomShape.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_selectionNew;
            this.mnuZoomShape.Name = "mnuZoomShape";
            resources.ApplyResources(this.mnuZoomShape, "mnuZoomShape");
            this.mnuZoomShape.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomPreviewMap
            // 
            this.mnuZoomPreviewMap.Image = global::MapWinGIS.MainProgram.GlobalResource.imgMapPreview;
            this.mnuZoomPreviewMap.Name = "mnuZoomPreviewMap";
            resources.ApplyResources(this.mnuZoomPreviewMap, "mnuZoomPreviewMap");
            this.mnuZoomPreviewMap.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // mnuZoomLayer
            // 
            this.mnuZoomLayer.Image = global::MapWinGIS.MainProgram.GlobalResource.zoom_layerNew;
            this.mnuZoomLayer.Name = "mnuZoomLayer";
            resources.ApplyResources(this.mnuZoomLayer, "mnuZoomLayer");
            this.mnuZoomLayer.Click += new System.EventHandler(this.mnuZoomItems_Click);
            // 
            // MapPreview
            // 
            resources.ApplyResources(this.MapPreview, "MapPreview");
            this.MapPreview.Name = "MapPreview";
            this.MapPreview.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("MapPreview.OcxState")));
            this.MapPreview.MouseDownEvent += new AxMapWinGIS._DMapEvents_MouseDownEventHandler(this.MapPreview_MouseDownEvent);
            this.MapPreview.MouseUpEvent += new AxMapWinGIS._DMapEvents_MouseUpEventHandler(this.MapPreview_MouseUpEvent);
            this.MapPreview.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(this.MapPreview_MouseMoveEvent);
            this.MapPreview.SizeChanged += new System.EventHandler(this.MapPreview_SizeChanged);
            // 
            // MapMain
            // 
            resources.ApplyResources(this.MapMain, "MapMain");
            this.MapMain.Name = "MapMain";
            this.MapMain.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("MapMain.OcxState")));
            this.MapMain.MouseDownEvent += new AxMapWinGIS._DMapEvents_MouseDownEventHandler(this.MapMain_MouseDownEvent);
            this.MapMain.MouseUpEvent += new AxMapWinGIS._DMapEvents_MouseUpEventHandler(this.MapMain_MouseUpEvent);
            this.MapMain.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(this.MapMain_MouseMoveEvent);
            this.MapMain.FileDropped += new AxMapWinGIS._DMapEvents_FileDroppedEventHandler(this.MapMain_FileDropped);
            this.MapMain.SelectBoxFinal += new AxMapWinGIS._DMapEvents_SelectBoxFinalEventHandler(this.MapMain_SelectBoxFinal);
            this.MapMain.ExtentsChanged += new System.EventHandler(this.MapMain_ExtentsChanged);
            // 
            // PreviewMapContextMenuStrip
            // 
            this.PreviewMapContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPreviewExtents,
            this.mnuPreviewCurrent,
            this.mnuPreviewClear});
            this.PreviewMapContextMenuStrip.Name = "PreviewMapMenu";
            this.PreviewMapContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            resources.ApplyResources(this.PreviewMapContextMenuStrip, "PreviewMapContextMenuStrip");
            // 
            // mnuPreviewExtents
            // 
            this.mnuPreviewExtents.Image = global::MapWinGIS.MainProgram.GlobalResource.imgMapExtents;
            this.mnuPreviewExtents.Name = "mnuPreviewExtents";
            resources.ApplyResources(this.mnuPreviewExtents, "mnuPreviewExtents");
            this.mnuPreviewExtents.Click += new System.EventHandler(this.mnuPreviewExtents_Click);
            // 
            // mnuPreviewCurrent
            // 
            this.mnuPreviewCurrent.Image = global::MapWinGIS.MainProgram.GlobalResource.imgMapScale;
            this.mnuPreviewCurrent.Name = "mnuPreviewCurrent";
            resources.ApplyResources(this.mnuPreviewCurrent, "mnuPreviewCurrent");
            this.mnuPreviewCurrent.Click += new System.EventHandler(this.mnuPreviewCurrent_Click);
            // 
            // mnuPreviewClear
            // 
            this.mnuPreviewClear.Image = global::MapWinGIS.MainProgram.GlobalResource.imgDelete;
            this.mnuPreviewClear.Name = "mnuPreviewClear";
            resources.ApplyResources(this.mnuPreviewClear, "mnuPreviewClear");
            this.mnuPreviewClear.Click += new System.EventHandler(this.mnuPreviewClear_Click);
            // 
            // MapWinForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.StripDocker);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MapWinForm";
            this.Activated += new System.EventHandler(this.MapWinForm_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MapWinForm_Closing);
            this.Shown += new System.EventHandler(this.MapWinForm_Shown);
            this.Resize += new System.EventHandler(this.MapWinForm_Resize);
            this.StripDocker.ContentPanel.ResumeLayout(false);
            this.StripDocker.TopToolStripPanel.ResumeLayout(false);
            this.StripDocker.TopToolStripPanel.PerformLayout();
            this.StripDocker.ResumeLayout(false);
            this.StripDocker.PerformLayout();
            this.tlbStandard.ResumeLayout(false);
            this.tlbStandard.PerformLayout();
            this.ContextToolStrip.ResumeLayout(false);
            this.tlbLayers.ResumeLayout(false);
            this.tlbLayers.PerformLayout();
            this.tlbMain.ResumeLayout(false);
            this.tlbMain.PerformLayout();
            this.tlbZoom.ResumeLayout(false);
            this.tlbZoom.PerformLayout();
            this.mnuLegend.ResumeLayout(false);
            this.mnuLayerButton.ResumeLayout(false);
            this.mnuZoom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MapPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapMain)).EndInit();
            this.PreviewMapContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolStripContainer StripDocker;
        internal System.Windows.Forms.ToolTip toolTip;
        internal MapWinGIS.ToolStripExtensions.ToolStripEx tlbStandard;
        internal System.Windows.Forms.ContextMenuStrip ContextToolStrip;
        internal System.Windows.Forms.ToolStripMenuItem ToggleTextLabelsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripButton tbbNew;
        internal System.Windows.Forms.ToolStripButton tbbOpen;
        internal System.Windows.Forms.ToolStripButton tbbSave;
        internal System.Windows.Forms.ToolStripButton tbbPrint;
        internal System.Windows.Forms.ToolStripButton tbbProjectSettings;
        internal MapWinGIS.ToolStripExtensions.ToolStripEx tlbLayers;
        internal MapWinGIS.ToolStripExtensions.MenuStripEx menuStrip1;
        internal System.Windows.Forms.ToolStripButton tbbAddLayer;
        internal System.Windows.Forms.ToolStripButton tbbRemoveLayer;
        internal System.Windows.Forms.ToolStripButton tbbClearLayers;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripButton tbbSymbologyManager;
        internal System.Windows.Forms.ToolStripButton tbbLayerProperties;
        internal MapWinGIS.ToolStripExtensions.ToolStripEx tlbMain;
        internal System.Windows.Forms.ToolStripButton tbbSelect;
        internal System.Windows.Forms.ToolStripButton tbbDeSelectLayer;
        internal MapWinGIS.ToolStripExtensions.ToolStripEx tlbZoom;
        internal System.Windows.Forms.ToolStripButton tbbPan;
        internal System.Windows.Forms.ToolStripButton tbbZoomIn;
        internal System.Windows.Forms.ToolStripButton tbbZoomOut;
        internal System.Windows.Forms.ToolStripButton tbbZoomExtent;
        internal System.Windows.Forms.ToolStripButton tbbZoomSelected;
        internal System.Windows.Forms.ToolStripButton tbbZoomPrevious;
        internal System.Windows.Forms.ToolStripButton tbbZoomNext;
        internal System.Windows.Forms.ToolStripButton tbbZoomLayer;
        internal System.Windows.Forms.ContextMenuStrip mnuLegend;
        internal System.Windows.Forms.ToolStripMenuItem mnuAddGroup;
        internal System.Windows.Forms.ToolStripMenuItem mnuAddLayer;
        internal System.Windows.Forms.ToolStripMenuItem mnuRemoveLayerOrGroup;
        internal System.Windows.Forms.ToolStripMenuItem mnuClearLayers;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomToLayerOrGroup;
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuBreak1;
        internal System.Windows.Forms.ToolStripMenuItem mnuRelabel;
        internal System.Windows.Forms.ToolStripMenuItem mnuLabelSetup;
        internal System.Windows.Forms.ToolStripMenuItem mnuChartsSetup;
        internal System.Windows.Forms.ToolStripMenuItem mnuSeeMetadata;
        internal System.Windows.Forms.ToolStripMenuItem mnuLegendShapefileCategories;
        internal System.Windows.Forms.ToolStripMenuItem mnuTableEditorLaunch;
        internal System.Windows.Forms.ToolStripMenuItem mnuSaveAsLayerFile;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        internal System.Windows.Forms.ToolStripSeparator toolStripMenuBreak2;
        internal System.Windows.Forms.ToolStripMenuItem mnuExpandAll;
        internal System.Windows.Forms.ToolStripMenuItem mnuExpandGroups;
        internal System.Windows.Forms.ToolStripMenuItem mnuCollapseGroups;
        internal System.Windows.Forms.ToolStripMenuItem mnuCollapseAll;
        internal System.Windows.Forms.ToolStripMenuItem mnuProperties;
        internal System.Windows.Forms.ContextMenuStrip mnuLayerButton;
        internal System.Windows.Forms.ToolStripMenuItem mnuBtnAdd;
        internal System.Windows.Forms.ToolStripMenuItem mnuBtnRemove;
        internal System.Windows.Forms.ToolStripMenuItem mnuBtnClear;
        internal System.Windows.Forms.ContextMenuStrip mnuZoom;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomPrevious;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomNext;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomPreviewMap;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomMax;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomLayer;
        internal System.Windows.Forms.ToolStripMenuItem mnuSelect;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomShape;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomIn;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomOut;
        internal System.Windows.Forms.ToolStripMenuItem mnuZoomPan;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        internal System.Windows.Forms.ContextMenuStrip PreviewMapContextMenuStrip;
        internal System.Windows.Forms.ToolStripMenuItem mnuPreviewExtents;
        internal System.Windows.Forms.ToolStripMenuItem mnuPreviewCurrent;
        internal System.Windows.Forms.ToolStripMenuItem mnuPreviewClear;
        internal System.Windows.Forms.Panel panel1;
        internal AxMapWinGIS.AxMap MapPreview;
        internal AxMapWinGIS.AxMap MapMain;
        internal LegendControl.Legend Legend;

    }
}