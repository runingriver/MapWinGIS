
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
    
    public partial class frmEnterProjection : Form
    {
        // Projections already present in the list
        IEnumerable<string> m_existingList = null;

        // the base coordinate system
        CoordinateSystem m_coordinateSystem = null;

        // reference to projection database
        ProjectionDatabase m_database = null;
        
        /// <summary>
        /// Creates a new instance of the frmEnterProjection class
        /// </summary>
        public frmEnterProjection(CoordinateSystem coordSystem, IEnumerable<string> list, ProjectionDatabase database)
        {
            InitializeComponent();

            m_existingList = list;
            m_coordinateSystem = coordSystem;
            m_database = database;
        }

        /// <summary>
        /// Analyzes user input. Closes the dialog if needed.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            string MSG_INVALID_PROJECTION = "Invalid projection";
            
            string text = this.textBox1.Text.Trim();
            MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
            if (!proj.ImportFromProj4(text))
            {
                if (!proj.ImportFromWKT(text))
                {
                    MessageBox.Show("No valid proj4 or WKT string was entered.", MSG_INVALID_PROJECTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            
            MapWinGIS.GeoProjection projBase= new MapWinGIS.GeoProjection();
            if (!projBase.ImportFromEPSG(m_coordinateSystem.Code))
            {
                MessageBox.Show("Failed to initialize the base projection.", MSG_INVALID_PROJECTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (projBase.ExportToProj4() == text || projBase.ExportToWKT() == text)
            {
                MessageBox.Show("The dialect string is the same as base string", MSG_INVALID_PROJECTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // do we have this string already?
            if (m_existingList.Contains(text))
            {
                MessageBox.Show("The entered string is already present in the list.", MSG_INVALID_PROJECTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // do we have this string as a base one?
            IEnumerable<CoordinateSystem> list = m_database.CoordinateSystems.Where(s => s.proj4 == text);
            if (list.Count() > 0)
            {
                // no sense try to save it, base strings are processed first on loading all the same
                MessageBox.Show("Current string is aready bound to another EPSG code as the base one: " + 
                                list.First().Name + "(" + list.First().Code.ToString() + ")", MSG_INVALID_PROJECTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // is this really a dialect; user will be allowed to save as dialect CS with different parameters, 
            // as sometimes they differ insignificantly because of the rounding
            if (!proj.get_IsSameExt(projBase, m_coordinateSystem.Extents, 5))
            {
                if (MessageBox.Show("The base projection and its dialect have different transformation parameters." +
                                    "This can lead to incorrect disaply of data." + Environment.NewLine +
                                    "Do you want to save the dialect all the same?", "Projection mismatch",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            // TODO: check whether this dialect is used for some other EPSG code
            this.DialogResult = DialogResult.OK;
        }
    }
}
