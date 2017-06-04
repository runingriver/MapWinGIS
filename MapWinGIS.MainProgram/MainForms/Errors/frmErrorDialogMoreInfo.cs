using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public partial class frmErrorDialogMoreInfo : Form
    {
        public frmErrorDialogMoreInfo()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtFullText.Text);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFullText_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void frmErrorDialogMoreInfo_Load(object sender, EventArgs e)
        {
            this.Top = Owner.Top;
            this.Left = Owner.Left;
            txtFullText.SelectionLength = 0;
        }
    }
}
