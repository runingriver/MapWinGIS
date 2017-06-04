namespace MapWinGIS.MainProgram
{
    partial class StatusBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StatusBar1 = new System.Windows.Forms.StatusStrip();
            this.StatusBarPanelProjection = new System.Windows.Forms.ToolStripSplitButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnChoose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAbsenceBehavior = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbsenceAssign = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbsenceIgnore = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbsenceSkip = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMismatchBehavior = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMismatchIgnore = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMismatchProjectOld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMismatchSkip = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowWarnings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowReport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusPlaceHolder = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusPlaceHolder2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusUnits = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusAlternativeUnits = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusTooltip = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarPanelScale = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarPanelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusBar1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBar1
            // 
            this.StatusBar1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBarPanelProjection,
            this.StatusPlaceHolder,
            this.StatusPlaceHolder2,
            this.StatusUnits,
            this.StatusAlternativeUnits,
            this.StatusTooltip,
            this.StatusBarPanelScale,
            this.StatusBarPanelStatus,
            this.ProgressBar1});
            this.StatusBar1.Location = new System.Drawing.Point(0, 250);
            this.StatusBar1.Name = "StatusBar1";
            this.StatusBar1.Size = new System.Drawing.Size(629, 22);
            this.StatusBar1.TabIndex = 5;
            this.StatusBar1.Text = "StatusStrip1";
            this.StatusBar1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StatusBar1_ItemClicked);
            // 
            // StatusBarPanelProjection
            // 
            this.StatusBarPanelProjection.DropDown = this.contextMenuStrip1;
            this.StatusBarPanelProjection.Image = global::MapWinGIS.MainProgram.GlobalResource.imgMap;
            this.StatusBarPanelProjection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StatusBarPanelProjection.Name = "StatusBarPanelProjection";
            this.StatusBarPanelProjection.Size = new System.Drawing.Size(85, 20);
            this.StatusBarPanelProjection.Text = " 未定义 ";
            this.StatusBarPanelProjection.ToolTipText = "坐标系统和投影";
            this.StatusBarPanelProjection.ButtonClick += new System.EventHandler(this.StatusBarPanelProjection_ButtonClick);
            this.StatusBarPanelProjection.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StatusBarPanelProjection_DropDownItemClicked);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnChoose,
            this.toolStripSeparator1,
            this.btnAbsenceBehavior,
            this.btnMismatchBehavior,
            this.btnShowWarnings,
            this.btnShowReport,
            this.toolStripSeparator2,
            this.btnProperties});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.OwnerItem = this.StatusBarPanelProjection;
            this.contextMenuStrip1.Size = new System.Drawing.Size(143, 148);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // btnChoose
            // 
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(142, 22);
            this.btnChoose.Text = "选择投影";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // btnAbsenceBehavior
            // 
            this.btnAbsenceBehavior.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAbsenceAssign,
            this.btnAbsenceIgnore,
            this.btnAbsenceSkip});
            this.btnAbsenceBehavior.Name = "btnAbsenceBehavior";
            this.btnAbsenceBehavior.Size = new System.Drawing.Size(142, 22);
            this.btnAbsenceBehavior.Text = "缺省行为";
            this.btnAbsenceBehavior.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnAbsenceBehavior_DropDownItemClicked);
            // 
            // btnAbsenceAssign
            // 
            this.btnAbsenceAssign.Name = "btnAbsenceAssign";
            this.btnAbsenceAssign.Size = new System.Drawing.Size(142, 22);
            this.btnAbsenceAssign.Text = "从项目中指派";
            // 
            // btnAbsenceIgnore
            // 
            this.btnAbsenceIgnore.Name = "btnAbsenceIgnore";
            this.btnAbsenceIgnore.Size = new System.Drawing.Size(142, 22);
            this.btnAbsenceIgnore.Text = "忽视";
            // 
            // btnAbsenceSkip
            // 
            this.btnAbsenceSkip.Name = "btnAbsenceSkip";
            this.btnAbsenceSkip.Size = new System.Drawing.Size(142, 22);
            this.btnAbsenceSkip.Text = "跳过";
            // 
            // btnMismatchBehavior
            // 
            this.btnMismatchBehavior.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMismatchIgnore,
            this.btnMismatchProjectOld,
            this.btnMismatchSkip});
            this.btnMismatchBehavior.Name = "btnMismatchBehavior";
            this.btnMismatchBehavior.Size = new System.Drawing.Size(142, 22);
            this.btnMismatchBehavior.Text = "不匹配行为";
            this.btnMismatchBehavior.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnMismatchBehavior_DropDownItemClicked);
            // 
            // btnMismatchIgnore
            // 
            this.btnMismatchIgnore.Name = "btnMismatchIgnore";
            this.btnMismatchIgnore.Size = new System.Drawing.Size(94, 22);
            this.btnMismatchIgnore.Text = "忽视";
            // 
            // btnMismatchProjectOld
            // 
            this.btnMismatchProjectOld.Name = "btnMismatchProjectOld";
            this.btnMismatchProjectOld.Size = new System.Drawing.Size(94, 22);
            this.btnMismatchProjectOld.Text = "重投";
            // 
            // btnMismatchSkip
            // 
            this.btnMismatchSkip.Name = "btnMismatchSkip";
            this.btnMismatchSkip.Size = new System.Drawing.Size(94, 22);
            this.btnMismatchSkip.Text = "跳过";
            // 
            // btnShowWarnings
            // 
            this.btnShowWarnings.Name = "btnShowWarnings";
            this.btnShowWarnings.Size = new System.Drawing.Size(142, 22);
            this.btnShowWarnings.Text = "显示警告";
            // 
            // btnShowReport
            // 
            this.btnShowReport.Name = "btnShowReport";
            this.btnShowReport.Size = new System.Drawing.Size(142, 22);
            this.btnShowReport.Text = "显示加载报告";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
            // 
            // btnProperties
            // 
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(142, 22);
            this.btnProperties.Text = "属性";
            // 
            // StatusPlaceHolder
            // 
            this.StatusPlaceHolder.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusPlaceHolder.Name = "StatusPlaceHolder";
            this.StatusPlaceHolder.Size = new System.Drawing.Size(4, 17);
            // 
            // StatusPlaceHolder2
            // 
            this.StatusPlaceHolder2.Name = "StatusPlaceHolder2";
            this.StatusPlaceHolder2.Size = new System.Drawing.Size(0, 17);
            // 
            // StatusUnits
            // 
            this.StatusUnits.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusUnits.Name = "StatusUnits";
            this.StatusUnits.Size = new System.Drawing.Size(4, 17);
            // 
            // StatusAlternativeUnits
            // 
            this.StatusAlternativeUnits.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatusAlternativeUnits.Name = "StatusAlternativeUnits";
            this.StatusAlternativeUnits.Size = new System.Drawing.Size(4, 17);
            // 
            // StatusTooltip
            // 
            this.StatusTooltip.Name = "StatusTooltip";
            this.StatusTooltip.Size = new System.Drawing.Size(245, 17);
            this.StatusTooltip.Spring = true;
            // 
            // StatusBarPanelScale
            // 
            this.StatusBarPanelScale.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.StatusBarPanelScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusBarPanelScale.Name = "StatusBarPanelScale";
            this.StatusBarPanelScale.Size = new System.Drawing.Size(27, 17);
            this.StatusBarPanelScale.Text = "1:1";
            this.StatusBarPanelScale.ToolTipText = "双击显示比例";
            // 
            // StatusBarPanelStatus
            // 
            this.StatusBarPanelStatus.Name = "StatusBarPanelStatus";
            this.StatusBarPanelStatus.Size = new System.Drawing.Size(245, 17);
            this.StatusBarPanelStatus.Spring = true;
            this.StatusBarPanelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.AutoSize = false;
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(120, 16);
            this.ProgressBar1.Step = 1;
            this.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar1.Visible = false;
            // 
            // StatusBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(629, 272);
            this.Controls.Add(this.StatusBar1);
            this.Name = "StatusBar";
            this.StatusBar1.ResumeLayout(false);
            this.StatusBar1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.StatusStrip StatusBar1;
        internal System.Windows.Forms.ToolStripSplitButton StatusBarPanelProjection;
        internal System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem btnChoose;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem btnAbsenceBehavior;
        internal System.Windows.Forms.ToolStripMenuItem btnAbsenceAssign;
        internal System.Windows.Forms.ToolStripMenuItem btnAbsenceIgnore;
        internal System.Windows.Forms.ToolStripMenuItem btnAbsenceSkip;
        internal System.Windows.Forms.ToolStripMenuItem btnMismatchBehavior;
        internal System.Windows.Forms.ToolStripMenuItem btnMismatchIgnore;
        internal System.Windows.Forms.ToolStripMenuItem btnMismatchProjectOld;
        internal System.Windows.Forms.ToolStripMenuItem btnMismatchSkip;
        internal System.Windows.Forms.ToolStripMenuItem btnShowWarnings;
        internal System.Windows.Forms.ToolStripMenuItem btnShowReport;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem btnProperties;
        internal System.Windows.Forms.ToolStripStatusLabel StatusPlaceHolder;
        internal System.Windows.Forms.ToolStripStatusLabel StatusPlaceHolder2;
        internal System.Windows.Forms.ToolStripStatusLabel StatusUnits;
        internal System.Windows.Forms.ToolStripStatusLabel StatusAlternativeUnits;
        internal System.Windows.Forms.ToolStripStatusLabel StatusTooltip;
        internal System.Windows.Forms.ToolStripStatusLabel StatusBarPanelScale;
        internal System.Windows.Forms.ToolStripStatusLabel StatusBarPanelStatus;
        internal System.Windows.Forms.ToolStripProgressBar ProgressBar1;
    }
}