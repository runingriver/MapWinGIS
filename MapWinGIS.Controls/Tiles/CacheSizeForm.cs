
namespace MapWindow.Controls.Tiles
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
    #endregion

    public partial class CacheSizeForm : Form
    {
        /// <summary>
        /// Creates a new instance of the CacheSizeForm
        /// </summary>
        public CacheSizeForm(MapWinGIS.Tiles tiles)
        {
            InitializeComponent();
            //this.cacheSizeControl1.Init(tiles, MapWinGIS.tkCacheType.RAM);
        }
    }
}
