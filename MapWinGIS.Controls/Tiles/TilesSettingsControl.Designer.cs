namespace MapWindow.Controls.Tiles
{
    partial class TilesSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkAddDiskCache = new System.Windows.Forms.CheckBox();
            this.chkAddRamCache = new System.Windows.Forms.CheckBox();
            this.chkUseDiskCache = new System.Windows.Forms.CheckBox();
            this.chkUseRamCache = new System.Windows.Forms.CheckBox();
            this.chkUseServer = new System.Windows.Forms.CheckBox();
            this.chkDisplayTiles = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Store tiles in:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(8, 200);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(167, 1);
            this.panel3.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Use tiles from:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(8, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(167, 1);
            this.panel2.TabIndex = 12;
            // 
            // chkAddDiskCache
            // 
            this.chkAddDiskCache.AutoSize = true;
            this.chkAddDiskCache.Location = new System.Drawing.Point(13, 240);
            this.chkAddDiskCache.Name = "chkAddDiskCache";
            this.chkAddDiskCache.Size = new System.Drawing.Size(80, 17);
            this.chkAddDiskCache.TabIndex = 19;
            this.chkAddDiskCache.Text = "Disk cache";
            this.chkAddDiskCache.UseVisualStyleBackColor = true;
            // 
            // chkAddRamCache
            // 
            this.chkAddRamCache.AutoSize = true;
            this.chkAddRamCache.Location = new System.Drawing.Point(13, 207);
            this.chkAddRamCache.Name = "chkAddRamCache";
            this.chkAddRamCache.Size = new System.Drawing.Size(81, 17);
            this.chkAddRamCache.TabIndex = 18;
            this.chkAddRamCache.Text = "Ram cache";
            this.chkAddRamCache.UseVisualStyleBackColor = true;
            // 
            // chkUseDiskCache
            // 
            this.chkUseDiskCache.AutoSize = true;
            this.chkUseDiskCache.Location = new System.Drawing.Point(13, 143);
            this.chkUseDiskCache.Name = "chkUseDiskCache";
            this.chkUseDiskCache.Size = new System.Drawing.Size(80, 17);
            this.chkUseDiskCache.TabIndex = 15;
            this.chkUseDiskCache.Text = "Disk cache";
            this.chkUseDiskCache.UseVisualStyleBackColor = true;
            // 
            // chkUseRamCache
            // 
            this.chkUseRamCache.AutoSize = true;
            this.chkUseRamCache.Location = new System.Drawing.Point(13, 109);
            this.chkUseRamCache.Name = "chkUseRamCache";
            this.chkUseRamCache.Size = new System.Drawing.Size(81, 17);
            this.chkUseRamCache.TabIndex = 14;
            this.chkUseRamCache.Text = "Ram cache";
            this.chkUseRamCache.UseVisualStyleBackColor = true;
            // 
            // chkUseServer
            // 
            this.chkUseServer.AutoSize = true;
            this.chkUseServer.Location = new System.Drawing.Point(13, 76);
            this.chkUseServer.Name = "chkUseServer";
            this.chkUseServer.Size = new System.Drawing.Size(57, 17);
            this.chkUseServer.TabIndex = 11;
            this.chkUseServer.Text = "Server";
            this.chkUseServer.UseVisualStyleBackColor = true;
            // 
            // chkDisplayTiles
            // 
            this.chkDisplayTiles.AutoSize = true;
            this.chkDisplayTiles.Location = new System.Drawing.Point(13, 13);
            this.chkDisplayTiles.Name = "chkDisplayTiles";
            this.chkDisplayTiles.Size = new System.Drawing.Size(81, 17);
            this.chkDisplayTiles.TabIndex = 10;
            this.chkDisplayTiles.Text = "Display tiles";
            this.chkDisplayTiles.UseVisualStyleBackColor = true;
            // 
            // TilesSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkAddDiskCache);
            this.Controls.Add(this.chkAddRamCache);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.chkUseDiskCache);
            this.Controls.Add(this.chkUseRamCache);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.chkUseServer);
            this.Controls.Add(this.chkDisplayTiles);
            this.Name = "TilesSettingsControl";
            this.Size = new System.Drawing.Size(184, 276);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAddDiskCache;
        private System.Windows.Forms.CheckBox chkAddRamCache;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chkUseDiskCache;
        private System.Windows.Forms.CheckBox chkUseRamCache;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkUseServer;
        private System.Windows.Forms.CheckBox chkDisplayTiles;
    }
}
