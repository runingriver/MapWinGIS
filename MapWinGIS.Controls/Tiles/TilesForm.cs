
namespace MapWinGIS.Controls.Tiles
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS;
    using System.Diagnostics;
    using System.IO;
    #endregion

    public partial class TilesForm : Form
    {
#if OCX_VERSION49
        private Tiles tiles;
        private MapWinGIS.Interfaces.IMapWin mapWin;
        private string state = string.Empty;
        private static int tabIndex = 0;

        /// <summary>
        /// Creates a new instance of the ChooseProviderForm
        /// </summary>
        public TilesForm(MapWinGIS.Interfaces.IMapWin mapWin, Tiles tiles)
        {
            InitializeComponent();

            this.btnChangeDiskLocation.Click += new System.EventHandler(this.btnChangeDiskLocation_Click);
            this.btnRunCaching.Click += new System.EventHandler(this.btnRunCaching_Click);
            this.chkServer.CheckedChanged += new System.EventHandler(this.chkServer_CheckedChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TilesForm_FormClosing);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);

            if (mapWin == null)
                throw new ArgumentNullException("Reference to MapWinGIS wasn't passed");
            
            this.tiles = tiles;
            this.mapWin = mapWin;
            TileSettings.FillProviderTree(this.treeView1, tiles.ProviderId);

            this.state = tiles.Serialize();

            this.cacheSizeControl1.Init(tiles, tkCacheType.RAM);
            this.cacheSizeControl2.Init(tiles, tkCacheType.Disk);

            string s = tiles.DiskCacheFilename;
            this.txtDiskCachePath.Text = s;

            this.chkServer.Checked = tiles.UseServer;
            this.chkVisible.Checked = tiles.Visible;

            if (tabIndex >= 0 && tabIndex < tabControl1.TabPages.Count)
                this.tabControl1.SelectedIndex = tabIndex;
        }

        /// <summary>
        /// Fills the tree with providers. 
        /// </summary>
        /// <remarks>Currently unused. See TilesSettings class.</remarks>
        private void FillTree()
        {
            this.treeView1.HideSelection = false;
            string[] names = Enum.GetNames(typeof (tkTileProvider));
            tkTileProvider[] values = (tkTileProvider[])Enum.GetValues(typeof (tkTileProvider));

            this.treeView1.Nodes.Clear();
            TreeNode parent = new TreeNode("Providers");
            this.treeView1.Nodes.Add(parent);

            List<tkTileProvider> tempList = new List<tkTileProvider>() { tkTileProvider.ProviderNone };
            string[] groups = new string[] {"OpenStreetMap", "Bing", "Google", "Ovi", "Yandex", "Yahoo", "Other" };
            foreach(string s in groups)
            {
                IEnumerable<tkTileProvider> list = null;
                if (s == "Other")
                {
                    list = values.Except(tempList);
                    if (list.Count() == 0)
                        continue;
                }
                else if (s == "OpenStreetMap")
                {
                    list = new List<tkTileProvider>()
                    {
                        tkTileProvider.OpenStreetMap,
                        tkTileProvider.OpenCycleMap,
                        tkTileProvider.OpenTransportMap
                    };
                    tempList.AddRange(list);
                }
                else
                {
                    list = values.Where(item => item.ToString().StartsWith(s));
                    tempList.AddRange(list);
                }
                
                TreeNode group = new TreeNode(s);
                group.Expand();
                parent.Nodes.Add(group);
                foreach (tkTileProvider val in list)
                {
                    TreeNode node = new TreeNode() { Text = val.ToString(), Tag = val };
                    group.Nodes.Add(node);
                    if (val == this.tiles.Provider)
                        this.treeView1.SelectedNode = node;
                }
            }
        }

        /// <summary>
        /// Selecting another provider
        /// </summary>
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                int id = (int)e.Node.Tag;
                this.tiles.ProviderId = id;
            }
            this.DialogResult = DialogResult.OK;
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
                    tiles.DiskCacheFilename = dialog.FileName;
                    this.cacheSizeControl2.UpdateState();
                    this.txtDiskCachePath.Text = dialog.FileName;
                    mapWin.Project.Modified = true;
                }
            }
            dialog.Dispose();
        }

        /// <summary>
        /// Saves state of the tree and updates the state of the project
        /// </summary>
        private void TilesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tabIndex = this.tabControl1.SelectedIndex;
            TileSettings.SaveTreeState(this.treeView1);
            if (this.state != this.tiles.Serialize())
            {
                mapWin.Project.Modified = true;
            }
        }

        private void chkServer_CheckedChanged(object sender, EventArgs e)
        {
            tiles.UseServer = this.chkServer.Checked;
        }

        /// <summary>
        /// Runs caching
        /// </summary>
        private void btnRunCaching_Click(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + @"\TilesPrefetcher.exe";
            if (!File.Exists(filename))
            {
                MessageBox.Show("File is missing: " + filename);
            }
            else
            {
                AxMapWinGIS.AxMap map = mapWin.GetOCX as AxMapWinGIS.AxMap;
                Extents ext =  map.GeographicExtents;
                if (ext == null)
                {
                    MessageBox.Show("Geographic extents of the map aren't set.");
                }
                else
                {
                    double xMin, xMax, yMin, yMax, zMin, zMax;
                    ext.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);
                    String s = String.Format("{0} {1} {2} {3} {4} {5}", yMin, yMax, xMin, xMax, tiles.ProviderId, tiles.DiskCacheFilename);
                    Debug.Print(s);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = filename;
                    startInfo.Arguments = s;
                    Process.Start(startInfo);
                }
            }
        }

        /// <summary>
        /// Changes the visibility of tiles
        /// </summary>
        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            tiles.Visible = this.chkVisible.Checked;
            mapWin.StatusBar.Refresh();
        }

        /// <summary>
        /// Checks internet connection via MapWinGIS
        /// </summary>
        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            #if OCX_VERSION49
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    MessageBox.Show("Connection exists: " + tiles.CheckConnection("http://www.google.com"), "MapWinGIS 4", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) {}
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            #endif
        }
#endif
    }
}
