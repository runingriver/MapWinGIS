
namespace MapWinGIS.Controls.Tiles
{
    #region usings
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

    public partial class ChooseExtents : Form
    {
        /// <summary>
        /// Creates a new instance of ChooseExtents form
        /// </summary>
        public ChooseExtents(Extents ext)
        {
            InitializeComponent();
            if (ext != null)
            {
                double xMin, xMax, yMin, yMax, zMin, zMax;
                ext.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);
                txtMinLat.Text = yMin.ToString();
                txtMaxLat.Text = yMax.ToString();
                txtMinLng.Text = xMin.ToString();
                txtMaxLng.Text = xMax.ToString();
            }
        }

        /// <summary>
        /// Validates the extents
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.CheckData())
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// Checks the input by user
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            try
            {
                double yMin = double.Parse(txtMinLat.Text);
                double yMax = double.Parse(txtMaxLat.Text);
                double xMin = double.Parse(txtMinLng.Text);
                double xMax = double.Parse(txtMaxLng.Text);

                if (xMin <= xMax && yMin <= yMax)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Minimum value should be less than maximum");
                }
            }
            catch (Exception) 
            { 
                MessageBox.Show("Invalid format of number");
            }
            return false;
        }

        /// <summary>
        /// Retrieves extents
        /// </summary>
        /// <returns></returns>
        public Extents GetExtents()
        {
            try
            {
                double yMin = double.Parse(txtMinLat.Text);
                double yMax = double.Parse(txtMaxLat.Text);
                double xMin = double.Parse(txtMinLng.Text);
                double xMax = double.Parse(txtMaxLng.Text);
                Extents ext = new Extents();
                ext.SetBounds(xMin, yMin, 0.0, xMax, yMax, 0.0);
                return ext;
            }
            catch(Exception) {}
            return null;
        }
    }
}
