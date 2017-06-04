// ----------------------------------------------------------------------------
// MapWinGIS.Controls.Projections: Data grid view with some commonly used options
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.General
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS;



    /// <summary>
    /// Data grid view with some commonly used options
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    [ToolboxBitmap(typeof(DataGridView))]
    public class DataGridViewMW : DataGridView
    {
        /// shapedrawing options associated with rows
        private List<DrawingOptions> m_shapeOptions = new List<DrawingOptions>();

        // the index of column to be treated as shapefile drawing one
        private int m_shapeDrawingColumn = -1;

        #region Initialization
        /// <summary>
        /// Creates new instance of DataGridViewMW class
        /// </summary>
        public DataGridViewMW()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Designer generated code
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewMW
            // 
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DataGridViewMW_CellFormatting);
            this.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridViewMW_RowsAdded);
            this.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.DataGridViewMW_CellPainting);
            this.CurrentCellChanged += new System.EventHandler(this.DataGridViewMW_CurrentCellChanged);
            this.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.DataGridViewMW_RowsRemoved);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Common Behavior
        /// <summary>
        /// Draws the focus rectangle
        /// </summary>
        private void DataGridViewMW_CellPainting(object sender, System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            if (this.CurrentCell == null) return;
            if (e.ColumnIndex == this.CurrentCell.ColumnIndex && e.RowIndex == this.CurrentCell.RowIndex)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                using (Pen p = new Pen(Color.Black, 4))
                {
                    Rectangle rect = e.CellBounds;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Committing changes of the checkbox state immediately, CellValueChanged event won't be triggered otherwise
        /// </summary>
        private void DataGridViewMW_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.CurrentCell == null)
                return;

            int index = this.CurrentCell.ColumnIndex;
            DataGridViewCheckBoxColumn cmn = this.Columns[index] as DataGridViewCheckBoxColumn;
            if (cmn != null)
            {
                if (this.IsCurrentCellDirty)
                {
                    this.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
        #endregion

        #region Shapefile Drawing
        /// <summary>
        /// Returns drawing options associated with specified row
        /// </summary>
        /// <returns>Reference to the instance of ShapeDrawingOptions class or null on failure</returns>
        public DrawingOptions get_ShapefileDrawingOptions(int rowIndex)
        {
            if (!this.CheckSynchronization())
                return null;

            if (rowIndex >= 0 && rowIndex < m_shapeOptions.Count)
            {
                return m_shapeOptions[rowIndex];
            }

            return null;
        }

        /// <summary>
        /// Sets drawing options for particular row in data grid view
        /// </summary>
        /// <param name="rowIndex">Row index to set options for</param>
        /// <param name="options">Set of options</param>
        public bool set_ShapefileDrawingOptions(int rowIndex, DrawingOptions options)
        {
            if (!this.CheckSynchronization())
                return false;

            if (rowIndex >= 0 && rowIndex < m_shapeOptions.Count)
            {
                if (options.Options != null)    // to avoid additional checks later
                {
                    m_shapeOptions[rowIndex] = options;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks synchronization between shapefile darwing options and rows
        /// </summary>
        /// <returns></returns>
        private bool CheckSynchronization()
        {
            bool val = m_shapeOptions.Count == this.Rows.Count;
            if (!val)
                System.Diagnostics.Debug.Print("Broken syncronization inside custom data grid view");
            return val;
        }

        /// <summary>
        /// Gets or sets th index if column to treat as shapefile drawing column
        /// This column should have DataGridViewImageColumn type set in client code
        /// </summary>
        public int ShapefileDrawingColumn
        {
            get { return m_shapeDrawingColumn; }
            set { m_shapeDrawingColumn = value; }
        }

        /// <summary>
        /// Updating the size of custom list
        /// </summary>
        private void DataGridViewMW_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
                m_shapeOptions.Insert(e.RowIndex, null);
        }

        /// <summary>
        /// Updating the size of custom list
        /// </summary>
        private void DataGridViewMW_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
                m_shapeOptions.RemoveAt(e.RowIndex);
        }

        /// <summary>
        /// Draws shapefile preview in shapefile drawing column
        /// </summary>
        private void DataGridViewMW_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != m_shapeDrawingColumn) return;

            if (!this.CheckSynchronization())
                return;

            System.Drawing.Image img = e.Value as System.Drawing.Image;
            if (img != null)
            {
                DrawingOptions val = m_shapeOptions[e.RowIndex];
                if (val != null && val.Options != null)
                {
                    Graphics g = Graphics.FromImage(img);
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    uint color = Convert.ToUInt32(ColorTranslator.ToOle(this.BackgroundColor));

                    IntPtr hdc = g.GetHdc();

                    if (val.Type == ShpfileType.SHP_POLYGON)
                    {
                        val.Options.DrawRectangle(hdc, 0, 0, img.Width - 1, img.Height - 1, true, img.Width, img.Height, color);
                    }
                    else if (val.Type == ShpfileType.SHP_POLYLINE)
                    {
                        val.Options.DrawLine(hdc, 0, 0, img.Width - 1, img.Height - 1, true, img.Width, img.Height, color);
                    }
                    else if (val.Type == ShpfileType.SHP_POINT)
                    {
                        val.Options.DrawPoint(hdc, 0.0f, 0.0f, img.Width, img.Height, color);
                    }

                    g.ReleaseHdc(hdc);
                    g.Dispose();
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility strucutre to hold drawing options with type
    /// </summary>
    public class DrawingOptions
    {
        internal ShapeDrawingOptions Options;
        internal MapWinGIS.ShpfileType Type;

        public DrawingOptions(ShapeDrawingOptions options, ShpfileType type)
        {
            Options = options;
            Type = type;
        }
    }
}
