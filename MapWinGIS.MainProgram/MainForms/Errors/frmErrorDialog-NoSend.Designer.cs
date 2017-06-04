namespace MapWinGIS.MainProgram
{
    partial class ErrorDialogNoSend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialogNoSend));
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblAltLink = new System.Windows.Forms.LinkLabel();
            this.lblErr = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnCopy, "btnCopy");
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
            // lblAltLink
            // 
            resources.ApplyResources(this.lblAltLink, "lblAltLink");
            this.lblAltLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblAltLink.Name = "lblAltLink";
            this.lblAltLink.TabStop = true;
            this.lblAltLink.UseCompatibleTextRendering = true;
            this.lblAltLink.UseMnemonic = false;
            this.lblAltLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblAltLink_LinkClicked);
            // 
            // lblErr
            // 
            resources.ApplyResources(this.lblErr, "lblErr");
            this.lblErr.Name = "lblErr";
            // 
            // txtComments
            // 
            resources.ApplyResources(this.txtComments, "txtComments");
            this.txtComments.Name = "txtComments";
            this.txtComments.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtComments_KeyDown);
            // 
            // ErrorDialogNoSend
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnSend;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.lblErr);
            this.Controls.Add(this.lblAltLink);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnCopy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialogNoSend";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.ErrorDialogNoSend_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnCopy;
        internal System.Windows.Forms.Button btnSend;
        internal System.Windows.Forms.LinkLabel lblAltLink;
        internal System.Windows.Forms.Label lblErr;
        internal System.Windows.Forms.TextBox txtComments;
    }
}