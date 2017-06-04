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
    public partial class ErrorDialogNoSend : Form
    {

        private Exception m_exception;

        public ErrorDialogNoSend(Exception ex):base()
        {
            //MyBase.New()
            
            InitializeComponent();

            m_exception = ex;
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void ErrorDialogNoSend_Load(object sender, EventArgs e)
        {
            txtComments.Text = Program.appInfo.ApplicationName  + " " + App.VersionString + " (" + System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToShortDateString() + ")" + m_exception.ToString() + MapWinGIS.Utility.MiscUtils.GetDebugInfo();
        }

        private void lblAltLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.baidu.com");
            }
            catch
            { }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtComments.Text);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
