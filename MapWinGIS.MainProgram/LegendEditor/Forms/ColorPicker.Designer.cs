namespace MapWinGIS.MainProgram
{
    partial class ColorPicker
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblStartColor = new System.Windows.Forms.Label();
            this.cmbStart = new System.Windows.Forms.ComboBox();
            this.btnStartColor = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.lblEndColor = new System.Windows.Forms.Label();
            this.cmbEnd = new System.Windows.Forms.ComboBox();
            this.btnEndColor = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Controls.Add(this.lblStartColor);
            this.groupBox1.Controls.Add(this.cmbStart);
            this.groupBox1.Controls.Add(this.btnStartColor);
            this.groupBox1.Location = new System.Drawing.Point(14, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 85);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "开始色";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(65, 17);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(155, 12);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "从调色板选择";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // lblStartColor
            // 
            this.lblStartColor.Location = new System.Drawing.Point(65, 35);
            this.lblStartColor.Name = "lblStartColor";
            this.lblStartColor.Size = new System.Drawing.Size(165, 17);
            this.lblStartColor.TabIndex = 0;
            this.lblStartColor.Text = "选择颜色名:";
            this.lblStartColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbStart
            // 
            this.cmbStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStart.DropDownWidth = 175;
            this.cmbStart.FormattingEnabled = true;
            this.cmbStart.Location = new System.Drawing.Point(60, 55);
            this.cmbStart.Name = "cmbStart";
            this.cmbStart.Size = new System.Drawing.Size(175, 20);
            this.cmbStart.TabIndex = 5;
            this.cmbStart.SelectedIndexChanged += new System.EventHandler(this.cmbStart_SelectedIndexChanged);
            // 
            // btnStartColor
            // 
            this.btnStartColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStartColor.Location = new System.Drawing.Point(20, 35);
            this.btnStartColor.Name = "btnStartColor";
            this.btnStartColor.Size = new System.Drawing.Size(25, 25);
            this.btnStartColor.TabIndex = 2;
            this.btnStartColor.UseVisualStyleBackColor = true;
            this.btnStartColor.Click += new System.EventHandler(this.btnStartColor_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkLabel2);
            this.groupBox2.Controls.Add(this.lblEndColor);
            this.groupBox2.Controls.Add(this.cmbEnd);
            this.groupBox2.Controls.Add(this.btnEndColor);
            this.groupBox2.Location = new System.Drawing.Point(14, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 85);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "结束色";
            // 
            // linkLabel2
            // 
            this.linkLabel2.Location = new System.Drawing.Point(65, 17);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(155, 12);
            this.linkLabel2.TabIndex = 5;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "从调色板选择";
            this.linkLabel2.Click += new System.EventHandler(this.linkLabel2_Click);
            // 
            // lblEndColor
            // 
            this.lblEndColor.Location = new System.Drawing.Point(65, 35);
            this.lblEndColor.Name = "lblEndColor";
            this.lblEndColor.Size = new System.Drawing.Size(165, 17);
            this.lblEndColor.TabIndex = 0;
            this.lblEndColor.Text = "选择颜色名:";
            this.lblEndColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbEnd
            // 
            this.cmbEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEnd.FormattingEnabled = true;
            this.cmbEnd.Location = new System.Drawing.Point(60, 55);
            this.cmbEnd.Name = "cmbEnd";
            this.cmbEnd.Size = new System.Drawing.Size(175, 20);
            this.cmbEnd.TabIndex = 4;
            this.cmbEnd.SelectedIndexChanged += new System.EventHandler(this.cmbEnd_SelectedIndexChanged);
            // 
            // btnEndColor
            // 
            this.btnEndColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEndColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEndColor.Location = new System.Drawing.Point(20, 35);
            this.btnEndColor.Name = "btnEndColor";
            this.btnEndColor.Size = new System.Drawing.Size(25, 25);
            this.btnEndColor.TabIndex = 3;
            this.btnEndColor.UseVisualStyleBackColor = true;
            this.btnEndColor.Click += new System.EventHandler(this.btnEndColor_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pnlPreview);
            this.groupBox3.Location = new System.Drawing.Point(280, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(85, 176);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "预览";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(30, 20);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(35, 150);
            this.pnlPreview.TabIndex = 5;
            this.pnlPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPreview_Paint);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(185, 195);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(285, 195);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 25);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ColorPicker
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(372, 224);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择颜色...";
            this.Load += new System.EventHandler(this.ColorPicker_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        internal System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnOk;
        internal System.Windows.Forms.Button btnStartColor;
        internal System.Windows.Forms.Button btnEndColor;
        internal System.Windows.Forms.ComboBox cmbStart;
        internal System.Windows.Forms.ComboBox cmbEnd;
        internal System.Windows.Forms.Label lblStartColor;
        internal System.Windows.Forms.Label lblEndColor;
        internal System.Windows.Forms.LinkLabel linkLabel1;
        internal System.Windows.Forms.LinkLabel linkLabel2;
        internal System.Windows.Forms.Panel pnlPreview;
    }
}