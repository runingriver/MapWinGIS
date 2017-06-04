
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
    #endregion

    public partial class CacheUsageForm : Form
    {
#if OCX_VERSION49
        #region Declarations
        private const int CMN_NAME = 0;
        private const int CMN_STATUS = 1;
        private const int CMN_CLEAR = 2;
        private const int CMN_SCALES = 3;
        private Tiles tiles = null;
        tkCacheType cacheType;
        #endregion

        /// <summary>
        /// Creates a new instance of the CacheUsageForm
        /// </summary>
        public CacheUsageForm(Tiles tiles, tkCacheType cacheType)
        {
            InitializeComponent();


            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);    
            this.dgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgv_CellPainting);
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);

            this.tiles = tiles;
            this.cacheType = cacheType;

            DataGridViewButtonColumn cmn = (DataGridViewButtonColumn)dgv.Columns[CMN_CLEAR];
            cmn.Text = "Clear";
            cmn.DefaultCellStyle.Padding = new Padding(2);
            cmn.UseColumnTextForButtonValue = true;

            cmn = (DataGridViewButtonColumn)dgv.Columns[CMN_SCALES];
            cmn.Text = "Scales";
            cmn.DefaultCellStyle.Padding = new Padding(2);
            cmn.UseColumnTextForButtonValue = true;

            this.FillGrid();
        }

        /// <summary>
        /// Fills the grid
        /// </summary>
        private void FillGrid()
        {
            dgv.Rows.Clear();
            string[] names = Enum.GetNames(typeof(tkTileProvider));
            tkTileProvider[] values = (tkTileProvider[])Enum.GetValues(typeof(tkTileProvider));

            for (int i = 0; i < names.Length; i++)
            {
                if (values[i] != tkTileProvider.ProviderNone)
                {
                    if (tiles.get_CacheSize2(this.cacheType, values[i], -1) == 0)
                        continue;

                    int row = dgv.Rows.Add();
                    dgv[CMN_NAME, row].Value = names[i];
                    dgv.Rows[row].Tag = values[i];
                    dgv.Rows[row].Height = 30;
                }
            }
        }

        /// <summary>
        /// Draws the status bar for every provider
        /// </summary>
        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == CMN_STATUS && e.RowIndex != -1)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background);
                e.Paint(e.CellBounds, DataGridViewPaintParts.Border);

                tkTileProvider provider = (tkTileProvider)dgv.Rows[e.RowIndex].Tag;
                
                // bounds for status 
                Rectangle rect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                rect.Y += 7;
                rect.X += 10;
                rect.Height -= 14;
                rect.Width -= 20;
                int width = rect.Width;

                double size = tiles.get_CacheSize2(this.cacheType, provider, -1);
                double maxCacheSize = tiles.get_MaxCacheSize(this.cacheType);

                // particular status
                double ratio = size / maxCacheSize;
                if (ratio < 1.0)
                    rect.Width = (int)((double)rect.Width * ratio);

                Brush brush = new SolidBrush(Color.LightGreen);
                e.Graphics.FillRectangle(brush, rect);

                // bounds (initial)
                rect.Width = width;
                rect.X -= 1;
                e.Graphics.DrawRectangle(new Pen(Color.Gray), rect);

                // drawing text
                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(size.ToString("0.0") + " MB", dgv.Font, 
                                      new SolidBrush(Color.Black), 
                                      new PointF(rect.X + rect.Width/2, rect.Y + rect.Height/2), format);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the click over the cell
        /// </summary>
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            
            tkTileProvider provider = (tkTileProvider)dgv.Rows[e.RowIndex].Tag;

            switch (e.ColumnIndex)
            {
                case CMN_CLEAR:
                    if (MessageBox.Show(String.Format("Do you want to clear the cache for the: {0}?", provider.ToString()),
                                        Application.ProductName,
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.tiles.ClearCache2(this.cacheType, provider, 0, 100);
                        dgv.Invalidate();
                    }
                    break;
                case CMN_SCALES:
                    break;
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Do you want to clear the cache for all providers?",
                                        Application.ProductName,
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.tiles.ClearCache(this.cacheType);
                dgv.Invalidate();
                this.DialogResult = DialogResult.OK;
            }

        }
#endif

    }
}
