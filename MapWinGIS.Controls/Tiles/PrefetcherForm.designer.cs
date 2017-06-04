namespace MapWinGIS.Controls.Tiles
{
    partial class PrefetcherForm
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
         if(disposing && (components != null))
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
          this.listView1 = new System.Windows.Forms.ListView();
          this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
          this.Ok = new System.Windows.Forms.Button();
          this.btnCancel = new System.Windows.Forms.Button();
          this.label3 = new System.Windows.Forms.Label();
          this.chkSelectAll = new System.Windows.Forms.CheckBox();
          this.treeView1 = new System.Windows.Forms.TreeView();
          this.txtExtents = new System.Windows.Forms.TextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.txtDatabase = new System.Windows.Forms.TextBox();
          this.btnChangeDiskLocation = new System.Windows.Forms.Button();
          this.btnChoose = new System.Windows.Forms.Button();
          this.lblStatus = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // listView1
          // 
          this.listView1.CheckBoxes = true;
          this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader2});
          this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.listView1.FullRowSelect = true;
          this.listView1.GridLines = true;
          this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
          this.listView1.HideSelection = false;
          this.listView1.Location = new System.Drawing.Point(254, 105);
          this.listView1.Name = "listView1";
          this.listView1.Size = new System.Drawing.Size(409, 301);
          this.listView1.TabIndex = 3;
          this.listView1.UseCompatibleStateImageBehavior = false;
          this.listView1.View = System.Windows.Forms.View.Details;
          // 
          // columnHeader1
          // 
          this.columnHeader1.Text = "";
          this.columnHeader1.Width = 40;
          // 
          // columnHeader5
          // 
          this.columnHeader5.Text = "Scale";
          this.columnHeader5.Width = 50;
          // 
          // columnHeader6
          // 
          this.columnHeader6.Text = "Tiles count";
          this.columnHeader6.Width = 80;
          // 
          // columnHeader3
          // 
          this.columnHeader3.Text = "Loaded";
          // 
          // columnHeader7
          // 
          this.columnHeader7.Text = "Size, MB";
          this.columnHeader7.Width = 70;
          // 
          // columnHeader2
          // 
          this.columnHeader2.Text = "Status";
          this.columnHeader2.Width = 70;
          // 
          // Ok
          // 
          this.Ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.Ok.Location = new System.Drawing.Point(481, 412);
          this.Ok.Name = "Ok";
          this.Ok.Size = new System.Drawing.Size(88, 28);
          this.Ok.TabIndex = 4;
          this.Ok.Text = "Start";
          this.Ok.UseVisualStyleBackColor = true;
          // 
          // btnCancel
          // 
          this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.btnCancel.Location = new System.Drawing.Point(575, 412);
          this.btnCancel.Name = "btnCancel";
          this.btnCancel.Size = new System.Drawing.Size(88, 28);
          this.btnCancel.TabIndex = 5;
          this.btnCancel.Text = "Close";
          this.btnCancel.UseVisualStyleBackColor = true;
          
          // 
          // label3
          // 
          this.label3.AutoSize = true;
          this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.label3.Location = new System.Drawing.Point(254, 7);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(58, 17);
          this.label3.TabIndex = 8;
          this.label3.Text = "Extents:";
          // 
          // chkSelectAll
          // 
          this.chkSelectAll.AutoSize = true;
          this.chkSelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.chkSelectAll.Location = new System.Drawing.Point(254, 412);
          this.chkSelectAll.Name = "chkSelectAll";
          this.chkSelectAll.Size = new System.Drawing.Size(98, 17);
          this.chkSelectAll.TabIndex = 11;
          this.chkSelectAll.Text = "Select all/none";
          this.chkSelectAll.UseVisualStyleBackColor = true;
          // 
          // treeView1
          // 
          this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.treeView1.HideSelection = false;
          this.treeView1.Location = new System.Drawing.Point(11, 7);
          this.treeView1.Name = "treeView1";
          this.treeView1.Size = new System.Drawing.Size(237, 399);
          this.treeView1.TabIndex = 12;
          // 
          // txtExtents
          // 
          this.txtExtents.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.txtExtents.Location = new System.Drawing.Point(257, 27);
          this.txtExtents.Name = "txtExtents";
          this.txtExtents.ReadOnly = true;
          this.txtExtents.Size = new System.Drawing.Size(319, 23);
          this.txtExtents.TabIndex = 13;
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.label1.Location = new System.Drawing.Point(254, 54);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(73, 17);
          this.label1.TabIndex = 15;
          this.label1.Text = "Database:";
          // 
          // txtDatabase
          // 
          this.txtDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.txtDatabase.Location = new System.Drawing.Point(257, 74);
          this.txtDatabase.Name = "txtDatabase";
          this.txtDatabase.ReadOnly = true;
          this.txtDatabase.Size = new System.Drawing.Size(319, 23);
          this.txtDatabase.TabIndex = 16;
          // 
          // btnChangeDiskLocation
          // 
          this.btnChangeDiskLocation.Image = global::MapWinGIS.Controls.Properties.Resources.folder_open;
          this.btnChangeDiskLocation.Location = new System.Drawing.Point(582, 74);
          this.btnChangeDiskLocation.Name = "btnChangeDiskLocation";
          this.btnChangeDiskLocation.Size = new System.Drawing.Size(81, 25);
          this.btnChangeDiskLocation.TabIndex = 18;
          this.btnChangeDiskLocation.Text = "Choose";
          this.btnChangeDiskLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
          this.btnChangeDiskLocation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
          this.btnChangeDiskLocation.UseVisualStyleBackColor = true;
          // 
          // btnChoose
          // 
          this.btnChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
          this.btnChoose.Image = global::MapWinGIS.Controls.Properties.Resources.globe;
          this.btnChoose.Location = new System.Drawing.Point(582, 24);
          this.btnChoose.Name = "btnChoose";
          this.btnChoose.Size = new System.Drawing.Size(81, 26);
          this.btnChoose.TabIndex = 14;
          this.btnChoose.Text = "Choose";
          this.btnChoose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
          this.btnChoose.UseVisualStyleBackColor = true;
          // 
          // lblStatus
          // 
          this.lblStatus.AutoSize = true;
          this.lblStatus.Location = new System.Drawing.Point(17, 417);
          this.lblStatus.Name = "lblStatus";
          this.lblStatus.Size = new System.Drawing.Size(0, 13);
          this.lblStatus.TabIndex = 19;
          // 
          // PrefetcherForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackColor = System.Drawing.SystemColors.Control;
          this.ClientSize = new System.Drawing.Size(673, 447);
          this.Controls.Add(this.lblStatus);
          this.Controls.Add(this.btnChangeDiskLocation);
          this.Controls.Add(this.txtDatabase);
          this.Controls.Add(this.label1);
          this.Controls.Add(this.btnChoose);
          this.Controls.Add(this.txtExtents);
          this.Controls.Add(this.treeView1);
          this.Controls.Add(this.chkSelectAll);
          this.Controls.Add(this.label3);
          this.Controls.Add(this.btnCancel);
          this.Controls.Add(this.Ok);
          this.Controls.Add(this.listView1);
          this.DoubleBuffered = true;
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
          this.KeyPreview = true;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "PrefetcherForm";
          this.Padding = new System.Windows.Forms.Padding(4);
          this.ShowIcon = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
          this.Text = "Tiles prefetcher";
          this.ResumeLayout(false);
          this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ListView listView1;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Button Ok;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ColumnHeader columnHeader5;
      private System.Windows.Forms.ColumnHeader columnHeader6;
      private System.Windows.Forms.ColumnHeader columnHeader7;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.CheckBox chkSelectAll;
      private System.Windows.Forms.TreeView treeView1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.TextBox txtExtents;
      private System.Windows.Forms.Button btnChoose;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtDatabase;
      private System.Windows.Forms.Button btnChangeDiskLocation;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.ColumnHeader columnHeader3;
   }
}