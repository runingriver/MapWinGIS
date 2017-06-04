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
    public partial class ProjectSettings : Form
    {
        public ProjectSettings()
        {
            InitializeComponent();
            this.Icon = Program.frmMain.Icon;
        }

        private void ProjectSettings_Load(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = new PrjSetGrid();
            this.propertyGrid2.SelectedObject = new AppSetGrid();
            Program.LoadFormPosition(this);
        }

        private void ProjectSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.SaveFormPosition(this);
        }
    }
}
