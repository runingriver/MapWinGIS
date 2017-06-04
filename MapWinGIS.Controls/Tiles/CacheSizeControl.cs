
namespace MapWinGIS.Controls.Tiles
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS;
    #endregion


    public partial class CacheSizeControl : UserControl
    {
        #if OCX_VERSION49
        private tkCacheType cacheType;
        private Tiles tiles;

        /// <summary>
        /// Creates a new instance of the tile control class
        /// </summary>
        public CacheSizeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control
        /// </summary>
        /// <param name="tiles">The reference to map tiles</param>
        /// <param name="cacheType">The type of cache to deal with</param>
        public void Init(Tiles tiles, tkCacheType cacheType)
        {
            if (tiles == null)
            {
                throw new ArgumentNullException("Tiles reference wasn't passed");
            }

            if (cacheType != tkCacheType.Disk && cacheType != tkCacheType.RAM)
            {
                throw new ArgumentException("Tiles reference wasn't passed");
            }

            this.cacheType = cacheType;
            this.tiles = tiles;
            this.groupBox2.Text = cacheType == tkCacheType.Disk ? "Disk cache" : "RAM cache";
            this.lblName.Text = cacheType == tkCacheType.Disk ? "Disk cache" : "RAM cache";

            string[] sizes = new string[] { "5", "10", "25", "50", "100", "250", "500", "1000" };
            foreach (string s in sizes)
                this.comboBox1.Items.Add(s);

            chkUse.Checked = tiles.get_UseCache(cacheType);
            chkAdd.Checked = tiles.get_DoCaching(cacheType);

            this.UpdateState();
        }

        /// <summary>
        /// Updates the state of the form
        /// </summary>
        public void UpdateState()
        {
            double size = tiles.get_CacheSize(cacheType);    // in MB
            double maxSize = tiles.get_MaxCacheSize(cacheType);

            this.progressBar1.Maximum = (int)maxSize;
            if (size < maxSize)
            {
                this.progressBar1.Value = (int)size;
            }
            this.lblSize.Text = String.Format("{0} / {1} MB", size.ToString("0.0"), maxSize.ToString("0.0"));

            this.comboBox1.Text = maxSize.ToString();
            this.btnClear.Enabled = tiles.get_CacheSize(this.cacheType) > 0;
        }
        #endif

        /// <summary>
        /// Displays cache usage form
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            #if OCX_VERSION49
            CacheUsageForm form = new CacheUsageForm(this.tiles, this.cacheType);
            form.ShowDialog(this);
            form.Dispose();
            this.UpdateState();
            #endif
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            #if OCX_VERSION49
            this.tiles.set_MaxCacheSize(this.cacheType, Int32.Parse(comboBox1.Text));
            this.UpdateState();
            #endif
        }

        private void chkUse_CheckedChanged(object sender, EventArgs e)
        {
            #if OCX_VERSION49
            tiles.set_UseCache(cacheType, chkUse.Checked);
            #endif
        }

        private void chkAdd_CheckedChanged(object sender, EventArgs e)
        {
            #if OCX_VERSION49
            tiles.set_DoCaching(cacheType, chkAdd.Checked);
            #endif
        }

    }
}
