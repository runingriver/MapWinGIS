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
    public partial class YesNoToAll : Form
    {
        public YesNoToAll()
        {
            InitializeComponent();
        }

        public new enum DialogResult
        {
            Yes,
            No,
            YesToAll,
            NoToAll,
            Undefined
        }

        private DialogResult result = DialogResult.Undefined;

        public static DialogResult ShowPrompt(string Message, string Title)
        {
            YesNoToAll f = new YesNoToAll();
            f.label1.Text = Message;
            f.Text = Title;
            f.ShowDialog();
            return f.result;
        }

        private void frmYesNoToAll_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (result == DialogResult.Undefined)
            {
                e.Cancel = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            result = DialogResult.Yes;
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            result = DialogResult.No;
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            result = DialogResult.YesToAll;
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            result = DialogResult.NoToAll;
            this.Close();
        }

        private void YesNoToAll_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (result == DialogResult.Undefined)
            {
                e.Cancel = true;
            }
        }
    }
}
