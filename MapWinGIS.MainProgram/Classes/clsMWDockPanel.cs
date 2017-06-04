using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.MainProgram
{
    public class MWDockPanel : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        private string m_Name;

        public MWDockPanel(string Name)
        {

            m_Name = Name;
            this.Text = Name;
        }

        protected override string GetPersistString()
        {
            return "mwDockPanel_" + m_Name;
        }
    }
}
