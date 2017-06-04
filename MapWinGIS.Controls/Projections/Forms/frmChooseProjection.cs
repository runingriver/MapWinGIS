
namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    
    /// <summary>
    /// Simple GUI fro choosing projection
    /// </summary>
    public partial class frmChooseProjection : Form
    {
        /// <summary>
        /// Creates a new instance of frmProjectionChooser class
        /// It's assumed that database isn't read yet
        /// </summary>
        public frmChooseProjection(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();
            if (projectionTreeView1.InitializeByExePath(Application.ExecutablePath, mapWin))
            {
                this.projectionTreeView1.RefreshList();
            }
        }
        
        /// <summary>
        /// Creates a new instance of frmProjectionChooser class. 
        /// It's assumed that database is read already.
        /// </summary>
        public frmChooseProjection(ProjectionDatabase database, MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();
            if (projectionTreeView1.Initialize(database, mapWin))
            {
                this.projectionTreeView1.RefreshList();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (projectionTreeView1.SelectedCoordinateSystem != null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("No projection is selected", "Select projection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
