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
    public partial class MsgBox : Form
    {
        public MsgBox(string msg)
        {
            InitializeComponent();
            this.Icon = Program.frmMain.Icon;
            this.label2.Text = "MapWinGIS 快捷键提示：";
            this.label1.Text = msg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
