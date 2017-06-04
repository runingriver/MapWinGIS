
namespace MapWinGIS.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Data.Common;
    
    public partial class frmImportShapefile : Form
    {
        // Reference to MapWinGIS
        MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        #region Constructor
        /// <summary>
        /// Creates a new instance of the frmImportShapefile dialog
        /// </summary>
        public frmImportShapefile(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();

            if (mapWin == null)
                throw new ArgumentException("No refernce to MapWinGIS was passed.");

            m_mapWin = mapWin;

            // TODO: fill the list of tables
            
        }
        #endregion

        #region Selection
        /// <summary>
        /// Tests database connection
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            DbUtilities.TestConnection(this.txtFilename.Text, false);
        }

        /// <summary>
        /// Toogles the selected state for each shapefile
        /// </summary>
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                this.listView1.Items[i].Checked = this.chkSelectAll.Checked;
            }
        }

        /// <summary>
        /// Chooses the database
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select database to export shapefiles to:";
            dialog.Filter = "All supported formats|*.db;*.db3;*.mdb|" +
                            "SQLite databases|*.db;*.db3|" +
                            "MS Access databases|*.mdb;*.accdb";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txtFilename.Text = dialog.FileName;

                //switch (System.IO.Path.GetExtension(this.txtFilename.Text).ToLower())
                //{
                //    case ".db":
                //    case ".db3":
                //        lblFormat.Text = "Format: SQLite";
                //        break;
                //    case ".mdb":
                //    case ".accdb":
                //        lblFormat.Text = "Foramt: MS Access";
                //        break;
                //    default:
                //        lblFormat.Text = "";
                //        break;
                //}
            }
            dialog.Dispose();

            this.RefreshTables();
        }

        /// <summary>
        /// Refresh tables list in the selected database
        /// </summary>
        private void RefreshTables()
        {
            this.listView1.Items.Clear();

            IDataProvider provider = DbUtilities.GetDataProvider(this.txtFilename.Text);
            if (provider != null)
            {
                DbConnection connection = provider.CreateConnection(this.txtFilename.Text);
                connection.Open();
                try
                {
                    List<TableInfo> list = DbUtilities.GetTableList(connection);
                    foreach (TableInfo tbl in list)
                    {
                        this.listView1.Items.Add(tbl.Name);
                    }
                }
                finally 
                {
                    connection.Close();
                }
            }

        }
        #endregion

        #region Import tables
        /// <summary>
        /// Exports the selected layers
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            IDataProvider provider = DbUtilities.GetDataProvider(this.txtFilename.Text);
            if (provider == null)
                return;

            IEnumerable<ListViewItem> items = this.listView1.Items.Cast<ListViewItem>().Where(item => item.Checked);
            if (items.Count() == 0)
            {
                MessageBox.Show("No layers are selected.", m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ShapefileDataClient client = new ShapefileDataClient(provider, this.txtFilename.Text, InitStringType.DatabaseName);
                client.Callback = m_mapWin.Layers as MapWinGIS.ICallback;
                if (client.OpenConnection())
                {
                    int percent, count, i;
                    percent = count = i = 0;

                    foreach (ListViewItem item in items)
                    {
                        if (items.Count() > 1)
                        {
                            int newPercent = Convert.ToInt32((double)i / (double)(items.Count() - 1) * 100.0);
                            if (newPercent != percent)
                            {
                                (m_mapWin.Layers as MapWinGIS.ICallback).Progress("", newPercent, "Importing shapefiles...");
                                percent = newPercent;
                            }
                        }

                        MapWinGIS.Shapefile sf = client.LoadShapefile(item.Text, CommandType.TableDirect);
                        if (sf != null)
                        {
                            m_mapWin.Layers.StartAddingSession();
                            m_mapWin.Layers.Add(ref sf);
                            m_mapWin.Layers.StopAddingSession();
                            count++;
                            // TODO: log errors
                        }
                        i++;
                    }

                    // cleaning progress
                    if (items.Count() > 1)
                        (m_mapWin.Layers as MapWinGIS.ICallback).Progress("", 100, "");

                    MessageBox.Show("Shapefiles imported: " + count.ToString(), m_mapWin.ApplicationInfo.ApplicationName,
                                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        /// <summary>
        /// Temporary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumns_Click(object sender, EventArgs e)
        {
            IDataProvider provider = DbUtilities.GetDataProvider(this.txtFilename.Text);
            if (provider != null)
            {
                DbConnection connection = provider.CreateConnection(this.txtFilename.Text);
                connection.Open();
                try
                {
                    System.Diagnostics.Debug.Print("List of tables:");
                    List<string> list = DbUtilities.GetTablesByField(connection, "Geometry", "blob");
                    foreach (string s in list)
                    {
                        System.Diagnostics.Debug.Print(s);
                    }
                    
                    //if (listView1.SelectedIndices.Count > 0)
                    //{
                    //    bool exists = DbUtilities.ColumnExists(connection, listView1.SelectedItems[0].Text, "Geometry");
                    //    MessageBox.Show("Geometry field exists: " +  exists.ToString());
                    //}
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
