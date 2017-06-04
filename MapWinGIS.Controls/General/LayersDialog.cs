// ----------------------------------------------------------------------------
// MapWinGIS.Controls.LayersControl: 
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

    /// <summary>
    /// Provides functionality to choose layers from project or from open file dialog
    /// </summary>
    public partial class LayersDialog : Form
    {
        // refence to MapWinGIS
        MapWinGIS.Interfaces.IMapWin m_mapWin = null;
        
        /// <summary>
        /// Creates a new instance of LayersDialog class, with no selected layers passed
        /// </summary>
        /// <param name="mapWin">Reference to MapWinGIS 4</param>
        /// <param name="layerTypes">Layer types to be included in the list</param>
        public LayersDialog(MapWinGIS.Interfaces.IMapWin mapWin, MapWinGIS.Interfaces.eLayerType[] layerTypes): this(mapWin, layerTypes, null)
        {
        }

        /// <summary>
        /// Creates new instance of LayersDialog class
        /// </summary>
        /// <param name="mapWin">Reference to MapWinGIS 4</param>
        /// <param name="layerTypes">Layer types to be included in the list</param>
        /// <param name="selection">List of filesnames to be checked</param>
        public LayersDialog(MapWinGIS.Interfaces.IMapWin mapWin, MapWinGIS.Interfaces.eLayerType[] layerTypes, IEnumerable<string> selection)
        {
            InitializeComponent();
            if (mapWin == null)
                throw new NullReferenceException();
            
            m_mapWin = mapWin;

            this.listView1.Items.Clear();
            foreach(Layer layer in mapWin.Layers)
            {
                foreach (eLayerType type in layerTypes)
                {
                    if (layer.LayerType == type)
                    {
                        ListViewItem item = new ListViewItem(layer.Name);
                        item.Tag = layer;

                        if (selection != null && selection.Contains(layer.FileName))
                            item.Checked = true;

                        listView1.Items.Add(item);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Selects/deselects all layers
        /// </summary>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach(ListViewItem item in listView1.Items)
                item.Checked = checkBox1.Checked;
        }

        /// <summary>
        /// Gets the list of selected layers (null if no layers were selected)
        /// </summary>
        public IList<Layer> SelectedLayers
        {
            get 
            {
                IEnumerable<ListViewItem> items = listView1.Items.Cast<ListViewItem>().Where(item => item.Checked);
                if (items.Count() > 0)
                {
                    List<Layer> list = new List<Layer>();
                    foreach (ListViewItem item in items)
                    {
                        list.Add((Layer)item.Tag);
                    }
                    return list;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns list of non-selected layers
        /// </summary>
        public IList<Layer> NonSelectedLayers
        {
            get
            {
                IEnumerable<ListViewItem> items = listView1.Items.Cast<ListViewItem>().Where(item => !item.Checked);
                if (items.Count() > 0)
                {
                    List<Layer> list = new List<Layer>();
                    foreach (ListViewItem item in items)
                    {
                        list.Add((Layer)item.Tag);
                    }
                    return list;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Closes the dialog, returns list of layers
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            //if (listView1.Items.Cast<ListViewItem>().Where(item => item.Checked).Count() == 0)
            //{
            //    MessageBox.Show("No layers were selected", m_mapWin.ApplicationInfo.ApplicationName,
            //                     MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    this.DialogResult = DialogResult.None;
            //}
        }
    }
}
