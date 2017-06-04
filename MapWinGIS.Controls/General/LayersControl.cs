// ----------------------------------------------------------------------------
// MapWinGIS.Controls.SelectLayerDialog: 
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.General
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS.Interfaces;
    using MapWinGIS.Controls.Projections;

    /// <summary>
    /// A dialog for layer selection  (eight from disk or from project)
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    [ToolboxBitmap(typeof(OpenFileDialog))]
    public partial class LayersControl : UserControl
    {
        /// <summary>
        /// The type of the control (addtional information that is to be displayed)
        /// </summary>
        public enum CustomType
        {
            Default = 0,
            Projection = 1,
        }
        
        #region Declarations
        // Reference to MapWinGIS
        MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        // whether it's allowed to select multiple files
        bool m_multiselect = false;

        // list of selected names
        IEnumerable<string> m_filenames = null;

        // data grid view columns
        private const int CMN_CHECK = 0;
        private const int CMN_NAME = 1;
        private const int CMN_CUSTOM = 2;

        // The type of the control (addtional info to be displayed)
        private CustomType m_controlType = CustomType.Default;
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets multiselect property for Open file dialog
        /// </summary>
        public bool Multiselect
        {
            get { return m_multiselect; }
            set { m_multiselect = value; }
        }

        /// <summary>
        /// Gets index of the custom column
        /// </summary>
        public int CustomColumnIndex
        {
            get { return CMN_CUSTOM; }
        }

        /// <summary>
        /// Gets or sets the type of the control
        /// </summary>
        public CustomType ControlType
        {
            get { return m_controlType; }
            set 
            {
                dgv.Columns[CMN_CUSTOM].Visible = (value != CustomType.Default);
                m_controlType = value;
                if (m_controlType == CustomType.Projection)
                    dgv.Columns[CMN_CUSTOM].HeaderText = "Projection";
            }
        }

        /// <summary>
        /// Returns the list of selected filenames
        /// </summary>
        public IEnumerable<string> Filenames
        {
            get 
            {
                IEnumerable<DataGridViewRow> rows = dgv.Rows.Cast<DataGridViewRow>().Where(row => (bool)row.Cells[CMN_CHECK].Value);
                return rows.Select(row => (string)row.Tag);
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of SelectLayerDilaog dialog
        /// </summary>
        public LayersControl()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Initializes the control with the instance of MapWinGIS class
        /// </summary>
        public void Initialize(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            m_mapWin = mapWin;
            m_multiselect = true;
            m_controlType = CustomType.Default;
            this.LayerAdded += new LayerAddedDelegate(LayersControl_LayerAdded);
        }

        #region Adding files
        /// <summary>
        /// Opens files from the folder
        /// </summary>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = sf.CdlgFilter;
            dlg.Multiselect = m_multiselect;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_filenames = dlg.FileNames;
                FillGrid();
            }
        }

        /// <summary>
        /// Adds project layers from dialog
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            MapWinGIS.Interfaces.eLayerType[] types = { eLayerType.LineShapefile, 
                                                        eLayerType.PointShapefile, 
                                                        eLayerType.PolygonShapefile };
           
            IEnumerable<string> existingNames = from DataGridViewRow row in dgv.Rows select (string)row.Tag;

            LayersDialog dlg = new LayersDialog(m_mapWin, types, existingNames);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // first we remove non selected layers if any
                if (dlg.NonSelectedLayers != null)
                {
                    List<DataGridViewRow> deleteList = new List<DataGridViewRow>();
                    IEnumerable<string> layers = dlg.NonSelectedLayers.Select(l => l.FileName);
                    
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (layers.Contains((string)row.Tag))
                            deleteList.Add(row);
                    }

                    foreach (DataGridViewRow row in deleteList)
                        dgv.Rows.Remove(row);
                }
                
               // the we add any selected ones if there are not here:
                if (dlg.SelectedLayers != null)
                {
                    IEnumerable<string>  layers = dlg.SelectedLayers.Select(l => l.FileName).Where(l => !existingNames.Contains(l));
                    foreach (string filename in layers)
                    {
                        int index = dgv.Rows.Add();
                        dgv[CMN_CHECK, index].Value = true;
                        dgv[CMN_NAME, index].Value = System.IO.Path.GetFileName(filename);
                        dgv.Rows[index].Tag = filename;
                        this.FireLayerAdded(filename, this.dgv, index);
                    }
                }

               this.AutoResizeColumns();
            }
            dlg.Dispose();
        }

        /// <summary>
        /// Fills the list with the filenames
        /// </summary>
        private void FillGrid()
        {
            if (m_filenames == null)
                return;
            
            // prevents duplicated names
            IEnumerable<string> existingNames = from DataGridViewRow row in dgv.Rows select (string)row.Tag;
            IEnumerable<string> duplicates = m_filenames.Where(name => existingNames.Contains(name));

            // notifiing the user about duplicates
            if (duplicates.Count() > 0)
            {
                string s = "";
                foreach (string name in duplicates)
                    s += Environment.NewLine + name;

                MessageBox.Show("Some of the layers are already present in the list:" + s, m_mapWin.ApplicationInfo.ApplicationName, 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            foreach (string filename in m_filenames)
            {
                if (existingNames.Contains(filename))
                    continue;

                int index = dgv.Rows.Add();
                dgv[CMN_CHECK, index].Value = true;
                dgv[CMN_NAME, index].Value = System.IO.Path.GetFileName(filename);
                dgv.Rows[index].Tag = filename;
                this.FireLayerAdded(filename, this.dgv, index);
            }
            m_filenames = null;

            this.AutoResizeColumns();
        }

        /// <summary>
        /// Autosizes the width of colums
        /// </summary>
        private void AutoResizeColumns()
        {
            dgv.AutoResizeColumn(CMN_NAME, DataGridViewAutoSizeColumnMode.AllCells);
            dgv.AutoResizeColumn(CMN_CUSTOM, DataGridViewAutoSizeColumnMode.AllCells);
        }

        /// <summary>
        /// A delegate for LayerAdded event
        /// </summary>
        /// <param name="filename">Filename of the datasource</param>
        /// <param name="rowIndex">Row index</param>
        public delegate void LayerAddedDelegate(string filename, DataGridView dgv, int rowIndex);

        /// <summary>
        /// A delegate for layer removed event. No paramters so far, the goal is just to be able to update the buttons on the client form
        /// </summary>
        public delegate void LayerRemovedDelegate();

        /// <summary>
        /// Event fired when a layer is added to the control
        /// </summary>
        public event LayerAddedDelegate LayerAdded;

        /// <summary>
        /// Event which occurs wien layer is removed from control
        /// </summary>
        public event LayerRemovedDelegate LayerRemoved;

        /// <summary>
        /// Sends LayerAdded event to all listeners
        /// </summary>
        private void FireLayerAdded(string filename, DataGridView dgv,  int rowIndex)
        {
            if (LayerAdded != null)
                LayerAdded(filename, dgv, rowIndex);
        }

        /// <summary>
        /// Sends LayerAdded event to all listeners
        /// </summary>
        private void FireLayerRemoved()
        {
            if (LayerRemoved != null)
                LayerRemoved();
        }

        /// <summary>
        /// Fills custom information
        /// </summary>
        void LayersControl_LayerAdded(string filename, DataGridView dgv, int rowIndex)
        {
            this.UpdateProjection(filename, dgv, rowIndex);
        }
        #endregion

        #region Update projection
        /// <summary>
        /// Update projection column for a specified layers
        /// </summary>
        private void UpdateProjection(string filename, DataGridView dgv, int rowIndex)
        {
            if (this.ControlType == CustomType.Projection)
            {
                string name = "" ;
                MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
                if (sf.Open(filename, null))
                {
                    name = sf.GeoProjection.Name;
                    ProjectionDatabase db = m_mapWin.ProjectionDatabase as ProjectionDatabase;
                    if (db != null)
                    {
                        CoordinateSystem cs = db.GetCoordinateSystem(sf.GeoProjection, ProjectionSearchType.UseDialects);
                        if (cs != null)
                            name = cs.Name;
                    }

                    dgv[this.CustomColumnIndex, rowIndex].Value = name;
                    sf.Close();
                }
                sf = null;

                if (name == "")
                {
                    dgv[this.CustomColumnIndex, rowIndex].Value = "Undefined";
                }
            }
        }

        /// <summary>
        /// Update projection column for all layers
        /// </summary>
        public void UpdateProjections()
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                this.UpdateProjection(dgv.Rows[i].Tag as string, this.dgv, i);
            }
        }
        #endregion

        /// <summary>
        /// Clears all the layers from the control
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
            this.FireLayerRemoved();
            this.AutoResizeColumns();
        }

        /// <summary>
        /// Removes the selected file
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell != null)
            {
                dgv.Rows.RemoveAt(dgv.CurrentCell.RowIndex);
                this.FireLayerRemoved();
                this.AutoResizeColumns();
            }
        }

        /// <summary>
        /// Gets the name of the selected file in the control ot "" if no file is selected
        /// </summary>
        public string SelectedFilename
        {
            get
            {
                if (dgv.CurrentCell != null)
                {
                    return dgv.Rows[dgv.CurrentCell.RowIndex].Tag.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
