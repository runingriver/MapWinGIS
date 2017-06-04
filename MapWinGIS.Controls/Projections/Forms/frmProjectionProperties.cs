// ----------------------------------------------------------------------------
// MapWinGIS.Controls.Projections: 
// Author: Sergei Leschinski
// ----------------------------------------------------------------------------

namespace MapWinGIS.Controls.Projections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS;
    using System.Collections;

    /// <summary>
    /// Displays dialog with projection properties. Allows modification of the projection dialogs.
    /// </summary>
    public partial class frmProjectionProperties : Form
    {
        #region Declarations
        /// <summary>
        /// Underlying geoprojection object
        /// </summary>
        private GeoProjection m_proj = null;

        /// <summary>
        /// Reference to treeview for searching by EPSG code, currently not needed
        /// </summary>
        private ProjectionDatabase m_database = null;

        // At least one dialect was either added or removed
        private bool m_dialectsChanged = false;

        // well-known coordinate system (in case one was passed)
        private CoordinateSystem m_coordinateSystem = null;

        // max index in the list
        private int m_index = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of ProjectionViewer class
        /// </summary>
        public frmProjectionProperties(CoordinateSystem projection, ProjectionDatabase database)
        {
            InitializeComponent();

            m_database = database;
            m_coordinateSystem = projection;

            listView1.MouseDoubleClick += delegate(object sender, MouseEventArgs e)
            {
                this.EditProjection();
            };

            listView1.SelectedIndexChanged += new EventHandler(listView1_SelectedIndexChanged);

            btnEdit.Click += delegate(object sender, EventArgs e)
            {
                this.EditProjection();
            };
            
            this.projectionMap1.LoadStateFromExeName(Application.ExecutablePath);
            this.projectionMap1.ShowVersionNumber = false;

            this.ShowProjection(projection);
        }
        
        /// <summary>
        /// Creates a new instance of ProjectionViewer class
        /// </summary>
        public frmProjectionProperties(MapWinGIS.GeoProjection projection)
        {
            InitializeComponent();
            this.projectionMap1.Visible = false;
            this.linkLabel1.Visible = false;

            // dialects available for EPSG codes
            this.tabControl1.TabPages.RemoveAt(3);

            this.ShowProjection(projection);
        }
        #endregion

        #region Show projection
        /// <summary>
        /// Shows string for the selected projection
        /// </summary>
        void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = listView1.SelectedItems.Count > 0;
            if (listView1.SelectedItems.Count > 0)
            {
                this.txtDialect.Text = listView1.SelectedItems[0].SubItems[2].Text;
            }
            else
            {
                this.txtDialect.Text = "";
            }
        }
        
        /// <summary>
        /// Shows information about selected projection
        /// </summary>
        /// <param name="projection"></param>
        public void ShowProjection(CoordinateSystem projection)
        {
            if (projection == null)
                throw new NullReferenceException("Geoprojection wasn't passed");

            txtName.Text = projection.Name;
            txtCode.Text = projection.Code.ToString();

            m_proj = new MapWinGIS.GeoProjection();
            if (!m_proj.ImportFromEPSG(projection.Code))
            {
                // usupported projection
            }
            else
            {
                projectionTextBox1.ShowProjection(m_proj.ExportToWKT());

                projectionMap1.DrawCoordinateSystem(projection);
                projectionMap1.ZoomToCoordinateSystem(projection);

                txtProj4.Text = m_proj.ExportToProj4();

                txtAreaName.Text = projection.AreaName;
                txtRemarks.Text = projection.Remarks;
                txtScope.Text = projection.Scope;
            }

            // showing dialects
            if (m_coordinateSystem != null)
            {
                m_database.ReadDialects(m_coordinateSystem);

                for (int i = 0; i < m_coordinateSystem.Dialects.Count; i++)
                {
                    string s = m_coordinateSystem.Dialects[i];
                    ListViewItem item = this.listView1.Items.Add(i.ToString());
                    this.UpdateDialectString(item, s);
                }
                m_index = m_coordinateSystem.Dialects.Count;

                if (listView1.Items.Count > 0)
                    listView1.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Shows information about unrecognized projection
        /// </summary>
        public void ShowProjection(MapWinGIS.GeoProjection projection)
        {
            if (projection == null)
                throw new NullReferenceException("Geoprojection wasn't passed");

            m_proj = projection;

            txtName.Text = projection.Name == "" ? "None" : projection.Name;
            txtCode.Text = "None";

            if (!projection.IsEmpty)
            {
                projectionTextBox1.ShowProjection(m_proj.ExportToWKT());
                txtProj4.Text = m_proj.ExportToProj4();

                txtAreaName.Text = "Not defined";
                txtScope.Text = "Not defined";
                txtRemarks.Text = "Unrecognized projection";
            }
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Shows map control on the second tab only
        /// </summary>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            projectionMap1.Visible = tabControl1.SelectedIndex == 1;
        }

        /// <summary>
        /// Updates size of tab pages. They tend to forget updting without it.
        /// </summary>
        private void tabControl1_SizeChanged(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                page.Invalidate();
            }
            Application.DoEvents();
        }

        private void ProjectionViewer_SizeChanged(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                page.Invalidate();
            }
            tabControl1.Invalidate();
            Application.DoEvents();
        }

        /// <summary>
        /// Copies WRT string to clipboard
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            projectionTextBox1.SelectAll();
            projectionTextBox1.Copy();
            projectionTextBox1.SelectionLength = 0;
        }

        /// <summary>
        /// Shows spatialreference.org page for the projection
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string link = "http://spatialreference.org/ref/epsg/" + txtCode.Text + "/";
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked." + Environment.NewLine + ex.Message);
            }
        }
        #endregion

        #region Dialects
        /// <summary>
        /// Adds a dialect formulation to the list
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            IEnumerable<string> list = ((IEnumerable)listView1.Items).Cast<ListViewItem>().Select(item => item.SubItems[2].Text);
            frmEnterProjection form = new frmEnterProjection(m_coordinateSystem, list, m_database);
            if (form.ShowDialog() == DialogResult.OK )
            {
                ListViewItem item = this.listView1.Items.Add((m_index++).ToString());
                this.UpdateDialectString(item, form.textBox1.Text);
                item.Selected = true;
                m_dialectsChanged = true;
            }
        }

        /// <summary>
        /// Removes a dialect string from the list
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 )
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
                m_dialectsChanged = true;
            }
        }

        /// <summary>
        /// Saving dialects if needed
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (m_coordinateSystem != null && m_dialectsChanged)
            {
                m_coordinateSystem.Dialects.Clear();
                foreach (ListViewItem item in this.listView1.Items)
                {
                    m_coordinateSystem.Dialects.Add(item.SubItems[2].Text);
                }
                m_database.SaveDialects(m_coordinateSystem);
            }
        }

        /// <summary>
        /// Displays a single projection string
        /// </summary>
        /// <param name="item">Listview item</param>
        /// <param name="projection">String to display</param>
        private void UpdateDialectString(ListViewItem item, string projection)
        {
            MapWinGIS.GeoProjection projTest = new GeoProjection();
            string projType = projTest.ImportFromProj4(projection) ? "proj4" : "WKT";
            projTest = null;

            item.SubItems.Add(projType);
            item.SubItems.Add(projection);
        }

        /// <summary>
        /// Edits projection string. Returns true if editing took place
        /// </summary>
        private void EditProjection()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string text = listView1.SelectedItems[0].SubItems[2].Text;

                // building the list of available dialects
                List<string> list = new List<string>();
                foreach (ListViewItem item in listView1.Items)
                {
                    if (!item.Selected)
                        list.Add(item.SubItems[2].Text);
                }
                
                frmEnterProjection form = new frmEnterProjection(m_coordinateSystem, list, m_database);
                form.textBox1.Text = text;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    listView1.SelectedItems[0].SubItems[2].Text = form.textBox1.Text;
                }
            }
        }
        #endregion
    }
}
