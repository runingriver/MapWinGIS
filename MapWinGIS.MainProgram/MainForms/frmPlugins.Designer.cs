namespace MapWinGIS.MainProgram
{
    partial class PluginsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTurnAllOff = new System.Windows.Forms.Button();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnTurnAllOm = new System.Windows.Forms.Button();
            this.lstPlugins = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.lblPluginInfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btnTurnAllOff);
            this.groupBox1.Controls.Add(this.btnRefreshList);
            this.groupBox1.Controls.Add(this.btnTurnAllOm);
            this.groupBox1.Controls.Add(this.lstPlugins);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnTurnAllOff
            // 
            resources.ApplyResources(this.btnTurnAllOff, "btnTurnAllOff");
            this.btnTurnAllOff.Name = "btnTurnAllOff";
            this.btnTurnAllOff.UseVisualStyleBackColor = true;
            this.btnTurnAllOff.Click += new System.EventHandler(this.btnTurnAllOff_Click);
            // 
            // btnRefreshList
            // 
            resources.ApplyResources(this.btnRefreshList, "btnRefreshList");
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.UseVisualStyleBackColor = true;
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // btnTurnAllOm
            // 
            resources.ApplyResources(this.btnTurnAllOm, "btnTurnAllOm");
            this.btnTurnAllOm.Name = "btnTurnAllOm";
            this.btnTurnAllOm.UseVisualStyleBackColor = true;
            this.btnTurnAllOm.Click += new System.EventHandler(this.btnTurnAllOn_Click);
            // 
            // lstPlugins
            // 
            resources.ApplyResources(this.lstPlugins, "lstPlugins");
            this.lstPlugins.Name = "lstPlugins";
            this.lstPlugins.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstPlugins_ItemCheck);
            this.lstPlugins.SelectedIndexChanged += new System.EventHandler(this.lstPlugins_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chkRemember
            // 
            resources.ApplyResources(this.chkRemember, "chkRemember");
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            resources.ApplyResources(this.cmdCancel, "cmdCancel");
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdApply
            // 
            resources.ApplyResources(this.cmdApply, "cmdApply");
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // lblPluginInfo
            // 
            resources.ApplyResources(this.lblPluginInfo, "lblPluginInfo");
            this.lblPluginInfo.AllowDrop = true;
            this.lblPluginInfo.BackColor = System.Drawing.Color.White;
            this.lblPluginInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPluginInfo.Name = "lblPluginInfo";
            // 
            // PluginsForm
            // 
            this.AcceptButton = this.cmdApply;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.cmdCancel;
            this.Controls.Add(this.lblPluginInfo);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PluginsForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PluginsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.Button btnRefreshList;
        internal System.Windows.Forms.Button btnTurnAllOm;
        internal System.Windows.Forms.CheckedListBox lstPlugins;
        internal System.Windows.Forms.Button btnTurnAllOff;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.CheckBox chkRemember;
        internal System.Windows.Forms.Button cmdOK;
        internal System.Windows.Forms.Button cmdCancel;
        internal System.Windows.Forms.Button cmdApply;
        internal System.Windows.Forms.Label lblPluginInfo;
    }
}