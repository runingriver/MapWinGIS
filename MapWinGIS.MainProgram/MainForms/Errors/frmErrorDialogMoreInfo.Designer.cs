namespace MapWinGIS.MainProgram
{
    partial class frmErrorDialogMoreInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmErrorDialogMoreInfo));
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblErr = new System.Windows.Forms.Label();
            this.txtFullText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblErr
            // 
            resources.ApplyResources(this.lblErr, "lblErr");
            this.lblErr.Name = "lblErr";
            // 
            // txtFullText
            // 
            resources.ApplyResources(this.txtFullText, "txtFullText");
            this.txtFullText.Name = "txtFullText";
            this.txtFullText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFullText_KeyDown);
            // 
            // frmErrorDialogMoreInfo
            // 
            this.AcceptButton = this.btnSend;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSend;
            this.Controls.Add(this.txtFullText);
            this.Controls.Add(this.lblErr);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnCopy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmErrorDialogMoreInfo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.frmErrorDialogMoreInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnCopy;
        internal System.Windows.Forms.Button btnSend;
        internal System.Windows.Forms.Label lblErr;
        internal System.Windows.Forms.TextBox txtFullText;
    }
}