
namespace MapWinGIS.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Collections;
    
    /// <summary>
    /// Shows list of options to choose from
    /// </summary>
    internal partial class OptionsChooser : Form
    {
        internal OptionsChooser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
                this.DialogResult = DialogResult.OK;
        }

        internal ListBox ListBox
        {
            get { return this.listBox1;}
        }

        internal Label Label
        {
            get { return this.lblMessage; }
        }
    }

    /// <summary>
    /// A class to hold static methods
    /// </summary>
    public class Dialogs
    {
        /// <summary>
        /// Shows list of options and allows user to choose one of them by double-click or ok button.
        /// </summary>
        /// <param name="list">The list of options as strings</param>
        /// <param name="selectedIndex">The initial selection</param>
        /// <returns>Seleted index or -1 if cancel was pressed</returns>
        public static int ChooseOptions(ArrayList list, int selectedIndex, string message, string caption)
        {
            if (list.Count == 0)
                throw new Exception("List of options must not be empty");

            OptionsChooser form = new OptionsChooser();
            form.Text = caption;
            form.Label.Text = message;
            form.ListBox.DataSource = list;
            if (selectedIndex > 0 && selectedIndex < form.ListBox.Items.Count)
                form.ListBox.SelectedIndex = selectedIndex;

            int index = (form.ShowDialog() == DialogResult.OK) ? form.ListBox.SelectedIndex : -1;
            form.Dispose();
            return index;
        }

        /// <summary>
        /// Shows list of options for projection mismatch situations
        /// </summary>
        /// <param name="dontShow">User has marked 'don't show checkbox', so no need to show this dialog</param>
        //public static int ChooseProjectionOptions(ArrayList list, int selectedIndex, string message, string caption, out bool dontShow)
        //{
        //    if (list.Count == 0)
        //        throw new Exception("List of options must not be empty");

        //    OptionsChooser form = new OptionsChooser();
        //    form.chkUseAnswerLater.Visible = true;
        //    form.chkUseAnswerLater.Text = "Don't show this dialog";
        //    form.Text = caption;
        //    form.Label.Text = message;
        //    form.btnCancel.Visible = false;
        //    form.btnOk.Left = form.btnCancel.Left;
            
        //    form.ListBox.DataSource = list;
        //    if (selectedIndex > 0 && selectedIndex < form.ListBox.Items.Count)
        //        form.ListBox.SelectedIndex = selectedIndex;
        //    else if (form.ListBox.Items.Count > 0)
        //        form.ListBox.SelectedIndex = 0;

        //    int index = (form.ShowDialog() == DialogResult.OK) ? form.ListBox.SelectedIndex : -1;
        //    dontShow = form.chkUseAnswerLater.Checked;

        //    form.Dispose();
        //    return index;
        //}
    }
}
