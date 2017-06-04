
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
    using System.Diagnostics;
    #endregion


    public partial class CachingProgressForm : Form, MapWinGIS.ICallback, MapWinGIS.IStopExecution
    {
        #if OCX_VERSION49
        PrefetcherForm parent = null;
        private int count = 0;
        
        /// <summary>
        /// Creates a new instance of the progress form
        /// </summary>
        public CachingProgressForm(PrefetcherForm parent)
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.CachingProgressForm_Shown);
            this.parent = parent;
            if (parent == null)
                throw new ArgumentNullException("Reference to the parent form wasn't passed");
       }

        private void CachingProgressForm_Shown(object sender, EventArgs e)
        {
            parent.MoveFirst();
            DoCaching();
        }

        /// <summary>
        /// Runs caching for a single zoom level
        /// </summary>
        /// <param name="row">The row in the listview to process</param>
        private void DoCaching()
        {
            if (!parent.HasRow())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
           
            if (parent.RowChecked())
            {
                double xMin, xMax, yMin, yMax, zMin, zMax;
                parent.extents.GetBounds(out xMin, out yMin, out zMin, out xMax, out yMax, out zMax);
                int zoom = parent.GetRowZoom();
                this.Text = string.Format("Caching: <zoom = {0}>", zoom);
                this.progressBar1.Maximum = parent.tiles.Prefetch(yMin, yMax, xMin, xMax, zoom, parent.SelectedProvider, this);
                if (this.progressBar1.Maximum == 0)
                {
                    parent.MoveToNextRow();
                    this.DoCaching();
                }
            }
            else
            {
                parent.MoveToNextRow();
                this.DoCaching();
            }
        }
#endif
        #region ICallback interface
        /// <summary>
        /// Processes errors which took place in Tiles class
        /// </summary>
        public void Error(string KeyOfSender, string ErrorMsg)
        {
            Debug.WriteLine("MapWinGIS error: " + ErrorMsg);
        }

        /// <summary>
        /// Displays progress information
        /// </summary>
        public void Progress(string KeyOfSender, int Percent, string Message)
        {
            if (this.InvokeRequired)
            {
                ProgressDelegate d = new ProgressDelegate(ShowStatus);
                this.BeginInvoke(d, new object[] { Percent });
            }
            else
            {
                ShowStatus(Percent);
            }
        }

        private delegate void ProgressDelegate(int Percent);
        /// <summary>
        /// Updates progress bar
        /// </summary>
        /// <param name="Percent"></param>
        private void ShowStatus(int Percent)
        {
#if OCX_VERSION49
            if (Percent == -1)
            {
                this.lblStatus.Text = "Finished";
                parent.SetRowStatus("Ok");
                parent.MoveToNextRow();
                DoCaching();
            }
            else if (Percent == -2)
            {
                MessageBox.Show("Aborted by user");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                this.lblStatus.Text = "Loaded: " + Percent.ToString();
                this.progressBar1.Value = Math.Min(Percent, this.progressBar1.Maximum);
                Application.DoEvents();
            }
#endif
        }

        #endregion

        #region Stop execution
        private bool stopped = false;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            stopped = true;
        }

        public bool StopFunction()
        {
            return stopped;
        }
        #endregion

    }

}
