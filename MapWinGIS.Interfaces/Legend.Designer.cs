namespace MapWinGIS.LegendControl
{
    partial class Legend
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Legend));
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.Icons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Location = new System.Drawing.Point(120, 44);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 232);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // Icons
            // 
            this.Icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Icons.ImageStream")));
            this.Icons.TransparentColor = System.Drawing.Color.Transparent;
            this.Icons.Images.SetKeyName(0, "");
            this.Icons.Images.SetKeyName(1, "");
            this.Icons.Images.SetKeyName(2, "");
            this.Icons.Images.SetKeyName(3, "");
            this.Icons.Images.SetKeyName(4, "");
            this.Icons.Images.SetKeyName(5, "");
            this.Icons.Images.SetKeyName(6, "tag_blue.png");
            this.Icons.Images.SetKeyName(7, "tag_gray.png");
            // 
            // Legend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.vScrollBar);
            this.Name = "Legend";
            this.Size = new System.Drawing.Size(156, 336);
            this.DoubleClick += new System.EventHandler(this.Legend_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Legend_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Legend_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Legend_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.ImageList Icons;
    }
}
