namespace MapWinGIS.Controls.Projections
{
    partial class frmProjectionMismatch
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnLayer = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.chkUseAnswerLater = new System.Windows.Forms.CheckBox();
            this.chkShowMismatchWarning = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MapWinGIS.Controls.Properties.Resources.projection_mismatch;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(52, 13);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(164, 32);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "图层投影是不同于项目的。选择如何处理它的方法：";
            // 
            // btnLayer
            // 
            this.btnLayer.Location = new System.Drawing.Point(244, 15);
            this.btnLayer.Name = "btnLayer";
            this.btnLayer.Size = new System.Drawing.Size(75, 23);
            this.btnLayer.TabIndex = 10;
            this.btnLayer.Text = "详细...";
            this.btnLayer.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Items.AddRange(new object[] {
            "Ignore",
            "Reproject in new file",
            "Reproject inplace",
            "Skip file"});
            this.listBox1.Location = new System.Drawing.Point(13, 63);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(316, 72);
            this.listBox1.TabIndex = 1;
            // 
            // chkUseAnswerLater
            // 
            this.chkUseAnswerLater.AutoSize = true;
            this.chkUseAnswerLater.Checked = true;
            this.chkUseAnswerLater.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseAnswerLater.Location = new System.Drawing.Point(13, 161);
            this.chkUseAnswerLater.Name = "chkUseAnswerLater";
            this.chkUseAnswerLater.Size = new System.Drawing.Size(180, 16);
            this.chkUseAnswerLater.TabIndex = 4;
            this.chkUseAnswerLater.Text = "处理相同的其他的不匹配投影";
            this.chkUseAnswerLater.UseVisualStyleBackColor = true;
            // 
            // chkShowMismatchWarning
            // 
            this.chkShowMismatchWarning.AutoSize = true;
            this.chkShowMismatchWarning.Location = new System.Drawing.Point(13, 184);
            this.chkShowMismatchWarning.Name = "chkShowMismatchWarning";
            this.chkShowMismatchWarning.Size = new System.Drawing.Size(120, 16);
            this.chkShowMismatchWarning.TabIndex = 6;
            this.chkShowMismatchWarning.Text = "不再显示该对话框";
            this.chkShowMismatchWarning.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(244, 154);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(244, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmProjectionMismatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 217);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkShowMismatchWarning);
            this.Controls.Add(this.chkUseAnswerLater);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnLayer);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pictureBox1);
            this.Name = "frmProjectionMismatch";
            this.Text = "Projection mismatch";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnLayer;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox chkUseAnswerLater;
        internal System.Windows.Forms.CheckBox chkShowMismatchWarning;
        internal System.Windows.Forms.Button btnOk;
        internal System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}