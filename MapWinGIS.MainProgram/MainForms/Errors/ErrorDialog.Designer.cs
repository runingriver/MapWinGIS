namespace MapWinGIS.MainProgram
{
    partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.btnSend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.chkNoReport = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEMail = new System.Windows.Forms.TextBox();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblErr = new System.Windows.Forms.Label();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblAltLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkNoReport
            // 
            resources.ApplyResources(this.chkNoReport, "chkNoReport");
            this.chkNoReport.Name = "chkNoReport";
            this.chkNoReport.UseVisualStyleBackColor = true;
            this.chkNoReport.CheckedChanged += new System.EventHandler(this.chkNoReport_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtEMail
            // 
            resources.ApplyResources(this.txtEMail, "txtEMail");
            this.txtEMail.Name = "txtEMail";
            // 
            // txtComments
            // 
            this.txtComments.AcceptsReturn = true;
            resources.ApplyResources(this.txtComments, "txtComments");
            this.txtComments.Name = "txtComments";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lblErr
            // 
            resources.ApplyResources(this.lblErr, "lblErr");
            this.lblErr.Name = "lblErr";
            // 
            // lblComments
            // 
            resources.ApplyResources(this.lblComments, "lblComments");
            this.lblComments.Name = "lblComments";
            // 
            // lblAltLink
            // 
            resources.ApplyResources(this.lblAltLink, "lblAltLink");
            this.lblAltLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblAltLink.Name = "lblAltLink";
            this.lblAltLink.TabStop = true;
            this.lblAltLink.UseCompatibleTextRendering = true;
            this.lblAltLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblAltLink_LinkClicked);
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.button1;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lblAltLink);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.lblErr);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.txtEMail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkNoReport);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnSend;
        internal System.Windows.Forms.Button button1;
        internal System.Windows.Forms.CheckBox chkNoReport;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtEMail;
        internal System.Windows.Forms.TextBox txtComments;
        internal System.Windows.Forms.LinkLabel linkLabel1;
        internal System.Windows.Forms.Label lblErr;
        internal System.Windows.Forms.Label lblComments;
        internal System.Windows.Forms.LinkLabel lblAltLink;
    }
}