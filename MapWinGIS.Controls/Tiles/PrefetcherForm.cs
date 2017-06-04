namespace MapWinGIS.Controls.Tiles
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using MapWinGIS;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;
    #endregion

    /// <summary>
    /// form helping to prefetch tiles on local db
    /// </summary>
    public partial class PrefetcherForm : Form
    {
#if OCX_VERSION49
        internal MapWinGIS.Tiles tiles = null;
        internal Extents extents = null;
        private int row = -1;

        #region Contsructor
        /// <summary>
        /// Creates a new instance of the tile prefetcher
        /// </summary>
        public PrefetcherForm(Extents ext, int provider, string dbFilename)
        {
            InitializeComponent();

            this.btnChangeDiskLocation.Click += new System.EventHandler(this.btnChangeDiskLocation_Click);
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged_1);
            this.Ok.Click += new System.EventHandler(this.Ok_Click_1);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrefetcherForm_FormClosing);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);

            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            //this.chkVisible.CheckedChanged += new System.EventHandler(this.chkVisible_CheckedChanged);

            this.tiles = new MapWinGIS.Tiles();
            tiles.AutodetectProxy();
            this.SetExtents(ext);
            Thread.Sleep(300);

            TileSettings.Read(tiles);
            TileSettings.FillProviderTree(this.treeView1, provider);

            if (dbFilename != "") {
                this.tiles.DiskCacheFilename = dbFilename;
            }
            this.txtDatabase.Text = tiles.DiskCacheFilename;

            try
            {
                if (this.treeView1.SelectedNode == null)
                    this.treeView1.SelectedNode = this.treeView1.Nodes[0].Nodes[0].Nodes[0];
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Displayes extents at the textbox
        /// </summary>
        /// <param name="ext"></param>
        private void SetExtents(Extents ext)
        {
            if (ext != null)
            {            
                double xMin, xMax, yMin, yMax, zMin, zMax;
                ext.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);

                this.txtExtents.Text = string.Format("Lat: {0} to {1}; Lng: {2} to {3}",
                                                        yMin.ToString("0.000"),
                                                        yMax.ToString("0.000"),
                                                        xMin.ToString("0.000"),
                                                        xMax.ToString("0.000"));
                this.extents = ext;
            }
        }

        /// <summary>
        /// Fills tiles info by scale for a given provider
        /// </summary>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.FillListView();
            this.UpdateTileCount();
        }

        /// <summary>
        /// Fill listview with available scale for the provider
        /// </summary>
        private void FillListView()
        {
            this.listView1.Items.Clear();

            int provider = this.SelectedProvider;
            if (provider != -1)
            {
                int index = tiles.Providers.get_IndexByProviderId(provider);
                int minZoom = tiles.Providers.get_minZoom(index);
                int maxZoom = tiles.Providers.get_maxZoom(index);

                if (this.extents != null)
                {
                    int number = 0;
                    for (int zoom = (int)minZoom; zoom <= maxZoom; zoom++)
                    {
                        Extents ext = tiles.GetTilesIndices(this.extents, zoom, provider);
                        if (ext != null)
                        {
                            double xMin, xMax, yMin, yMax, zMin, zMax;
                            ext.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);

                            int count = (int)((xMax - xMin + 1) * (yMax - yMin + 1));

                            if (count > 1)
                            {
                                ListViewItem item = this.listView1.Items.Add("");
                                item.Checked = false;
                                item.SubItems.Add(zoom.ToString());
                                item.SubItems.Add(count.ToString());
                                item.SubItems.Add("");      // number of tiles already in the database
                                item.SubItems.Add((count * 0.02).ToString("0.00"));
                                item.SubItems.Add("");
                            }
                            number = count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates number of tiles that already present in the database
        /// </summary>
        private void UpdateTileCount()
        {
            if (extents != null)
            {
                int provider = this.SelectedProvider;

                if (!this.MoveFirst())
                    return;

                do
                {
                    int zoom = this.GetRowZoom();
                    Extents ext = this.tiles.GetTilesIndices(this.extents, zoom, provider);

                    if (ext != null)
                    {
                        double xMin, xMax, yMin, yMax, zMin, zMax;
                        ext.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);
                        int count = tiles.get_DiskCacheCount(this.SelectedProvider, zoom, (int)xMin, (int)xMax, (int)yMin, (int)yMax);
                        this.SetRowCount(count);
                    }
                    else
                        this.SetRowCount(0);
                } while (this.MoveToNextRow());
            }
        }
        #endregion

        #region Start prefetching
        
        /// <summary>
        /// Starts the loading routine
        /// </summary>
        private void Ok_Click_1(object sender, EventArgs e)
        {
            int provider = this.SelectedProvider;
            CachingProgressForm form = new CachingProgressForm(this);
            this.tiles.GlobalCallback = form;
            form.ShowDialog(this);
            form.Dispose();
            this.UpdateTileCount();
        }

        internal bool RowChecked()
        {
            return this.listView1.Items[row].Checked;
        }
        internal int GetRowZoom()
        {
            return Int32.Parse(this.listView1.Items[row].SubItems[1].Text);
        }
        
        internal bool MoveToNextRow()
        {
            row++;
            return this.HasRow();
        }
        internal bool MoveFirst()
        {
            row = 0;
            return this.listView1.Items.Count > 0;
        }
        internal bool HasRow()
        {
            return row >= 0 && row < this.listView1.Items.Count;
        }

        private delegate void SetTextDelegate(string s);
        internal void SetRowStatus(string s)
        {
            if (this.InvokeRequired)
            {
                SetTextDelegate del = new SetTextDelegate(SetRowStatus);
                this.Invoke(del, new object[] { s });
            }
            else
                this.listView1.Items[row].SubItems[5].Text = s;
        }
        internal void SetRowCount(int count)
        {
            if (this.InvokeRequired)
            {
                SetTextDelegate del = new SetTextDelegate(SetRowStatus);
                this.Invoke(del, new object[] { count });
            }
            else
                this.listView1.Items[row].SubItems[3].Text = count.ToString();
        }
       
        #endregion

       /// <summary>
       /// Gets a selected provider
       /// </summary>
       public int SelectedProvider
       {
           get
           {
               if (this.treeView1.SelectedNode != null && this.treeView1.SelectedNode.Tag != null)
               {
                   return (int)this.treeView1.SelectedNode.Tag;
               }
               else 
               {
                   return -1;
               }                   
           }
       }

        /// <summary>
        /// Saves the state of the tree
        /// </summary>
        private void PrefetcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TileSettings.SaveTreeState(this.treeView1);
        }

        /// <summary>
        /// Toggles on/off all the checkboxes
        /// </summary>
        private void chkSelectAll_CheckedChanged_1(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                this.listView1.Items[i].Checked = this.chkSelectAll.Checked;
            }
        }

        /// <summary>
        /// Display dialog to type the extents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoose_Click(object sender, EventArgs e)
        {
            ChooseExtents form = new ChooseExtents(this.extents);
            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                this.SetExtents(form.GetExtents());
            }
            form.Dispose();
        }

        /// <summary>
        /// Sets the new location of the disk cache
        /// </summary>
        private void btnChangeDiskLocation_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "SQLite database (*.db3)|*.db3";
            string filename = tiles.DiskCacheFilename;
            dialog.InitialDirectory = Path.GetDirectoryName(tiles.DiskCacheFilename);
            dialog.OverwritePrompt = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (filename.ToLower() != dialog.FileName.ToLower())
                {
                    this.tiles.DiskCacheFilename = dialog.FileName;
                    this.txtDatabase.Text = dialog.FileName;
                }
            }
            dialog.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
#endif
    }
}
