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
    public partial class ColorPicker : Form
    {
        private MapWinGIS.ShapefileColorBreak m_SFBreak;
        private MapWinGIS.GridColorBreak m_GrdBreak;

        public ColorPicker(MapWinGIS.ShapefileColorBreak colorBreak)
        {
            InitializeComponent();

            btnStartColor.BackColor = ColorScheme.UIntToColor(colorBreak.StartColor);
            btnEndColor.BackColor = ColorScheme.UIntToColor(colorBreak.EndColor);

            m_GrdBreak = null;
            m_SFBreak = colorBreak;
        }

        public ColorPicker(MapWinGIS.GridColorBreak colorBreak)
        {
            InitializeComponent();

            btnStartColor.BackColor = ColorScheme.UIntToColor(colorBreak.LowColor);
            btnEndColor.BackColor = ColorScheme.UIntToColor(colorBreak.HighColor);

            m_SFBreak = null;
            m_GrdBreak = colorBreak;
        }


        public ColorPicker(System.Drawing.Color startc, System.Drawing.Color endc)
        {
            InitializeComponent();

            m_GrdBreak = null;
            m_SFBreak = null;

            btnStartColor.BackColor = startc;
            btnEndColor.BackColor = endc;

            UpdatePreview();

        }

        private void UpdatePreview()
        {
            System.Drawing.Drawing2D.LinearGradientBrush br = new System.Drawing.Drawing2D.LinearGradientBrush(pnlPreview.ClientRectangle, btnStartColor.BackColor, btnEndColor.BackColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            Graphics g = pnlPreview.CreateGraphics();
            g.FillRectangle(br, pnlPreview.ClientRectangle);
            g.Dispose();
            br.Dispose();

        }

        private void ColorPicker_Load(object sender, EventArgs e)
        {
            //System.Drawing.KnownColor
            foreach (string s in "Black,DimGray,Gray,DarkGray,Silver,LightGray,Gainsboro,WhiteSmoke,White,RosyBrown,IndianRed,Brown,Firebrick,LightCoral,Maroon,DarkRed,Red,Snow,MistyRose,Salmon,Tomato,DarkSalmon,Coral,OrangeRed,LightSalmon,Sienna,SeaShell,Chocalate,SaddleBrown,SandyBrown,PeachPuff,Peru,Linen,Bisque,DarkOrange,BurlyWood,Tan,AntiqueWhite,NavajoWhite,BlanchedAlmond,PapayaWhip,Mocassin,Orange,Wheat,OldLace,FloralWhite,DarkGoldenrod,Cornsilk,Gold,Khaki,LemonChiffon,PaleGoldenrod,DarkKhaki,Beige,LightGoldenrod,Olive,Yellow,LightYellow,Ivory,OliveDrab,YellowGreen,DarkOliveGreen,GreenYellow,Chartreuse,LawnGreen,DarkSeaGreen,ForestGreen,LimeGreen,PaleGreen,DarkGreen,Green,Lime,Honeydew,SeaGreen,MediumSeaGreen,SpringGreen,MintCream,MediumSpringGreen,MediumAquaMarine,YellowAquaMarine,Turquoise,LightSeaGreen,MediumTurquoise,DarkSlateGray,PaleTurquoise,Teal,DarkCyan,Aqua,Cyan,LightCyan,Azure,DarkTurquoise,CadetBlue,PowderBlue,LightBlue,DeepSkyBlue,SkyBlue,LightSkyBlue,SteelBlue,AliceBlue,DodgerBlue,SlateGray,LightSlateGray,LightSteelBlue,CornflowerBlue,RoyalBlue,MidnightBlue,Lavender,Navy,DarkBlue,MediumBlue,Blue,GhostWhite,SlateBlue,DarkSlateBlue,MediumSlateBlue,MediumPurple,BlueViolet,Indigo,DarkOrchid,DarkViolet,MediumOrchid,Thistle,Plum,Violet,Purple,DarkMagenta,Magenta,Fuchsia,Orchid,MediumVioletRed,DeepPink,HotPink,LavenderBlush,PaleVioletRed,Crimson,Pink,LightPink".Split(','))
            {
                cmbStart.Items.Add(s);
                cmbEnd.Items.Add(s);
            }

            cmbStart.Sorted = true;
            cmbEnd.Sorted = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (m_GrdBreak != null)
            {
                m_GrdBreak.LowColor = ColorScheme.ColorToUInt(btnStartColor.BackColor);
                m_GrdBreak.HighColor = ColorScheme.ColorToUInt(btnEndColor.BackColor);
            }
            else if (m_SFBreak != null)
            {
                m_SFBreak.StartColor = ColorScheme.ColorToUInt(btnStartColor.BackColor);
                m_SFBreak.EndColor = ColorScheme.ColorToUInt(btnEndColor.BackColor);
            }
            this.Hide();
        }

        private void btnEndColor_Click(object sender, EventArgs e)
        {

        }

        private void btnStartColor_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            btnEndColor_Click(btnEndColor, new EventArgs());
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            btnStartColor_Click(btnStartColor, new EventArgs());
        }

        private void pnlPreview_Paint(object sender, PaintEventArgs e)
        {
            UpdatePreview();
        }

        private void cmbStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnStartColor.BackColor = System.Drawing.Color.FromName(cmbStart.Items[cmbStart.SelectedIndex].ToString());
            UpdatePreview();
        }

        private void cmbEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEndColor.BackColor = System.Drawing.Color.FromName(cmbEnd.Items[cmbEnd.SelectedIndex].ToString());
            UpdatePreview();
        }



    }
}
