namespace MapWinGIS.Controls.Tiles
{
    partial class TilesForm
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.btnRunCaching = new System.Windows.Forms.Button();
            this.chkServer = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnChangeDiskLocation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDiskCachePath = new System.Windows.Forms.TextBox();
            this.cacheSizeControl2 = new MapWinGIS.Controls.Tiles.CacheSizeControl();
            this.cacheSizeControl1 = new MapWinGIS.Controls.Tiles.CacheSizeControl();
            this.tabControl1 = new MapWinGIS.Controls.Tiles.FixedTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(265, 434);
            this.treeView1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(159, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 66);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTestConnection.Location = new System.Drawing.Point(159, 366);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(95, 35);
            this.btnTestConnection.TabIndex = 16;
            this.btnTestConnection.Text = "Test connection...";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(19, 392);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(80, 17);
            this.chkVisible.TabIndex = 15;
            this.chkVisible.Text = "Tiles visible";
            this.chkVisible.UseVisualStyleBackColor = true;
            
            // 
            // btnRunCaching
            // 
            this.btnRunCaching.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRunCaching.Location = new System.Drawing.Point(159, 407);
            this.btnRunCaching.Name = "btnRunCaching";
            this.btnRunCaching.Size = new System.Drawing.Size(95, 25);
            this.btnRunCaching.TabIndex = 14;
            this.btnRunCaching.Text = "Run caching...";
            this.btnRunCaching.UseVisualStyleBackColor = true;
            // 
            // chkServer
            // 
            this.chkServer.AutoSize = true;
            this.chkServer.Location = new System.Drawing.Point(19, 366);
            this.chkServer.Name = "chkServer";
            this.chkServer.Size = new System.Drawing.Size(129, 17);
            this.chkServer.TabIndex = 13;
            this.chkServer.Text = "Download form server";
            this.chkServer.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnChangeDiskLocation);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtDiskCachePath);
            this.panel2.Location = new System.Drawing.Point(10, 264);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 96);
            this.panel2.TabIndex = 12;
            // 
            // btnChangeDiskLocation
            // 
            this.btnChangeDiskLocation.Image = global::MapWinGIS.Controls.Properties.Resources.folder_open;
            this.btnChangeDiskLocation.Location = new System.Drawing.Point(154, 57);
            this.btnChangeDiskLocation.Name = "btnChangeDiskLocation";
            this.btnChangeDiskLocation.Size = new System.Drawing.Size(81, 25);
            this.btnChangeDiskLocation.TabIndex = 11;
            this.btnChangeDiskLocation.Text = "Change...";
            this.btnChangeDiskLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChangeDiskLocation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChangeDiskLocation.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Disk cache location:";
            // 
            // txtDiskCachePath
            // 
            this.txtDiskCachePath.Location = new System.Drawing.Point(8, 31);
            this.txtDiskCachePath.Name = "txtDiskCachePath";
            this.txtDiskCachePath.ReadOnly = true;
            this.txtDiskCachePath.Size = new System.Drawing.Size(227, 20);
            this.txtDiskCachePath.TabIndex = 9;
            // 
            // cacheSizeControl2
            // 
            this.cacheSizeControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cacheSizeControl2.Location = new System.Drawing.Point(10, 137);
            this.cacheSizeControl2.Name = "cacheSizeControl2";
            this.cacheSizeControl2.Size = new System.Drawing.Size(250, 121);
            this.cacheSizeControl2.TabIndex = 8;
            // 
            // cacheSizeControl1
            // 
            this.cacheSizeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cacheSizeControl1.Location = new System.Drawing.Point(10, 8);
            this.cacheSizeControl1.Name = "cacheSizeControl1";
            this.cacheSizeControl1.Size = new System.Drawing.Size(250, 123);
            this.cacheSizeControl1.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(298, 448);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(271, 440);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Providers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnTestConnection);
            this.tabPage2.Controls.Add(this.cacheSizeControl2);
            this.tabPage2.Controls.Add(this.chkVisible);
            this.tabPage2.Controls.Add(this.cacheSizeControl1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.btnRunCaching);
            this.tabPage2.Controls.Add(this.chkServer);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(271, 440);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(298, 448);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TilesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Tiles setup";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button1;
        private CacheSizeControl cacheSizeControl1;
        private CacheSizeControl cacheSizeControl2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDiskCachePath;
        private System.Windows.Forms.Button btnChangeDiskLocation;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkServer;
        private System.Windows.Forms.Button btnRunCaching;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.CheckBox chkVisible;
        private FixedTabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}