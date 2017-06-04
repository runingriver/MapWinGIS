using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public partial class BookmarkManager : Form
    {
        private bool m_IsModified;
        public BookmarkManager(ArrayList currentBookmarks)
        {
            InitializeComponent();
        }


        public bool IsModified
        {
            get
            {
                return m_IsModified;
            }
        }








    }
}
