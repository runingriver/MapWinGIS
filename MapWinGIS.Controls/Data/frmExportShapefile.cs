// frmExportShapefile
// Author: Sergei Leschinski
// Created: 09 sep 2011

namespace MapWinGIS.Controls.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using MapWinGIS.Interfaces;
    using MapWinGIS.Data;
    using System.IO;
    using System.Data.Common;
    
    /// <summary>
    /// Provides GUI to export shapefiles to the database
    /// </summary>
    public partial class frmExportShapefile : Form
    {
        #region Declrations
        
        // Reference to MapWinGIS
        MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the frmExportShapefile class
        /// </summary>
        public frmExportShapefile(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            InitializeComponent();

            if (mapWin == null)
                throw new ArgumentException("No refernce to MapWinGIS was passed");

            m_mapWin = mapWin;

            IEnumerable<MapWinGIS.Interfaces.Layer> layers =
                m_mapWin.Layers.Where(layer => layer.LayerType == eLayerType.LineShapefile || 
                                              layer.LayerType == eLayerType.PointShapefile || 
                                              layer.LayerType == eLayerType.PolygonShapefile);
            foreach (Layer layer in layers)
            {
                ListViewItem item = this.listView1.Items.Add(layer.Name);
                item.Tag = layer;
            }
        }
        #endregion

        #region Selection
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

                //switch (Path.GetExtension(this.txtFilename.Text).ToLower())
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
        }

        /// <summary>
        /// Tests connection to the selected database
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
        #endregion

        #region Exporting
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
                                (m_mapWin.Layers as MapWinGIS.ICallback).Progress("", newPercent, "Exporting shapes...");
                                percent = newPercent;
                            }
                        }
                        
                        MapWinGIS.Interfaces.Layer layer = item.Tag as MapWinGIS.Interfaces.Layer;
                        MapWinGIS.Shapefile sf = layer.GetObject() as MapWinGIS.Shapefile;

                        if (client.SaveShapefile(sf, layer.Name, chkOverwrite.Checked))
                        {
                            count++;
                            // TODO: log errors
                        }
                        i++;
                    }
                    
                    // cleaning progress
                    if (items.Count() > 1)
                        (m_mapWin.Layers as MapWinGIS.ICallback).Progress("", 100, "");

                    MessageBox.Show("Shapefiles exported: " + count.ToString(), m_mapWin.ApplicationInfo.ApplicationName,
                                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion
    }
}
