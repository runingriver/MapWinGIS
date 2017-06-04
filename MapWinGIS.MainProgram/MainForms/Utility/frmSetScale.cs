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
    public partial class SetScale : Form
    {
        public SetScale(string currentScale)
        {
            InitializeComponent();
            this.Icon = MainProgram.Properties.Resources.MapWinGIS;
        }
    }
}
