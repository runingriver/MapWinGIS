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
    public partial class BookmarkAddNew : Form
    {
        MapWinGIS.Extents m_BookmarkExtents;
        string m_BookmarkName;

        public BookmarkAddNew(string newName, MapWinGIS.Extents newExtents)
        {
            InitializeComponent();
            if (newExtents != null)
            {
                m_BookmarkExtents = new MapWinGIS.Extents();
                m_BookmarkExtents.SetBounds(newExtents.xMin, newExtents.yMin, newExtents.zMin, newExtents.xMax, newExtents.yMax, newExtents.zMax);
            }

            m_BookmarkName = newName;
        }

        public MapWinGIS.Extents BookmarkExtents
        {
            get
            {
                return m_BookmarkExtents;
            }
        }

        public string BookmarkName
        {
            get
            {
                return m_BookmarkName;
            }
        }











    }
}
