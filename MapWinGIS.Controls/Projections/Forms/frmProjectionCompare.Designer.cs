namespace MapWinGIS.Controls.Projections
{
    partial class frmProjectionCompare
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
            this.chkWkt = new System.Windows.Forms.CheckBox();
            this.btnProject = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnLayer = new System.Windows.Forms.Button();
            this.txtLayer = new System.Windows.Forms.TextBox();
            this.txtProject = new System.Windows.Forms.TextBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.lblProject = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkWkt
            // 
            this.chkWkt.AutoSize = true;
            this.chkWkt.Location = new System.Drawing.Point(14, 244);
            this.chkWkt.Name = "chkWkt";
            this.chkWkt.Size = new System.Drawing.Size(84, 16);
            this.chkWkt.TabIndex = 31;
            this.chkWkt.Text = "WKT string";
            this.chkWkt.UseVisualStyleBackColor = true;
            this.chkWkt.Visible = false;
            // 
            // btnProject
            // 
            this.btnProject.Location = new System.Drawing.Point(365, 16);
            this.btnProject.Name = "btnProject";
            this.btnProject.Size = new System.Drawing.Size(89, 22);
            this.btnProject.TabIndex = 30;
            this.btnProject.Text = "详细...";
            this.btnProject.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(365, 239);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(89, 24);
            this.btnOk.TabIndex = 29;
            this.btnOk.Text = "关闭";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnLayer
            // 
            this.btnLayer.Location = new System.Drawing.Point(365, 133);
            this.btnLayer.Name = "btnLayer";
            this.btnLayer.Size = new System.Drawing.Size(89, 22);
            this.btnLayer.TabIndex = 28;
            this.btnLayer.Text = "详细...";
            this.btnLayer.UseVisualStyleBackColor = true;
            // 
            // txtLayer
            // 
            this.txtLayer.Location = new System.Drawing.Point(13, 162);
            this.txtLayer.Multiline = true;
            this.txtLayer.Name = "txtLayer";
            this.txtLayer.Size = new System.Drawing.Size(442, 71);
            this.txtLayer.TabIndex = 27;
            // 
            // txtProject
            // 
            this.txtProject.Location = new System.Drawing.Point(12, 45);
            this.txtProject.Multiline = true;
            this.txtProject.Name = "txtProject";
            this.txtProject.Size = new System.Drawing.Size(442, 71);
            this.txtProject.TabIndex = 26;
            // 
            // lblLayer
            // 
            this.lblLayer.Location = new System.Drawing.Point(10, 139);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(349, 17);
            this.lblLayer.TabIndex = 25;
            this.lblLayer.Text = "图层:";
            // 
            // lblProject
            // 
            this.lblProject.Location = new System.Drawing.Point(10, 22);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(349, 17);
            this.lblProject.TabIndex = 24;
            this.lblProject.Text = "项目:";
            // 
            // frmProjectionCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 279);
            this.Controls.Add(this.chkWkt);
            this.Controls.Add(this.btnProject);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnLayer);
            this.Controls.Add(this.txtLayer);
            this.Controls.Add(this.txtProject);
            this.Controls.Add(this.lblLayer);
            this.Controls.Add(this.lblProject);
            this.Name = "frmProjectionCompare";
            this.Text = "Projection mismatch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkWkt;
        private System.Windows.Forms.Button btnProject;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnLayer;
        private System.Windows.Forms.TextBox txtLayer;
        private System.Windows.Forms.TextBox txtProject;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Label lblProject;

    }
}