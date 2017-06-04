using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MapWinGIS
{
    internal class Globals
    {
        public static string LastError;

        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void GetCursorPos(ref POINTAPI lpPoint);

        public static System.Drawing.Point GetCursorLocation()
        {
            POINTAPI pnt = new POINTAPI();
            GetCursorPos(ref pnt);
            return new System.Drawing.Point(pnt.x, pnt.y);
        }

        public static bool IsSupportedPicture(object picture)
        {
            if (picture == null)
                return true;

            System.Type picType = picture.GetType();
            if (typeof(Icon) == picType)
                return true;
            if (typeof(Image) == picType)
                return true;
            if (typeof(Bitmap) == picType)
                return true;

            return false;
        }

        public static Color UintToColor(uint val)
        {
            int r, g, b;

            GetRGB((int)val, out r, out g, out b);

            return Color.FromArgb(255, r, g, b);
        }

        public static void GetRGB(int Color, out int r, out int g, out int b)
        {
            if (Color < 0)
                Color = 0;

            r = (int)(Color & 0xFF);
            g = (int)(Color & 0xFF00) / 256;	//shift right 8 bits
            b = (int)(Color & 0xFF0000) / 65536; //shift right 16 bits
        }

        public static int ColorToInt(Color c)
        {
            int retval = ((int)c.B) << 16;
            retval += ((int)c.G) << 8;
            return retval + ((int)c.R);
        }

        public static Color HSLtoColor(float Hue, float Sat, float Lum)
        {

            double r = 0, g = 0, b = 0;

            double temp1, temp2;

            if (Lum == 0)
            {

                r = g = b = 0;

            }

            else
            {

                if (Sat == 0)
                {

                    r = g = b = Lum;

                }

                else
                {

                    temp2 = ((Lum <= 0.5) ? Lum * (1.0 + Sat) : Lum + Sat - (Lum * Sat));

                    temp1 = 2.0 * Lum - temp2;



                    double[] t3 = new double[] { Hue + 1.0 / 3.0, Hue, Hue - 1.0 / 3.0 };

                    double[] clr = new double[] { 0, 0, 0 };

                    for (int i = 0; i < 3; i++)
                    {

                        if (t3[i] < 0)

                            t3[i] += 1.0;

                        if (t3[i] > 1)

                            t3[i] -= 1.0;

                        if (6.0 * t3[i] < 1.0)

                            clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;

                        else if (2.0 * t3[i] < 1.0)

                            clr[i] = temp2;

                        else if (3.0 * t3[i] < 2.0)

                            clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);

                        else

                            clr[i] = temp1;

                    }

                    r = clr[0];

                    g = clr[1];

                    b = clr[2];

                }

            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        public static void GetHSL(Color c, out float Hue, out float Sat, out float Lum)
        {
            Hue = c.GetHue() / 360f;
            Sat = c.GetSaturation();
            Lum = c.GetBrightness();
        }
    }

    internal class Constants
    {
        public static int ITEM_HEIGHT = 18;
        public static int ITEM_PAD = 4;
        public static int ITEM_RIGHT_PAD = 5;
        //  TEXT 
        public static int TEXT_HEIGHT = 14;
        public static int TEXT_TOP_PAD = 3;
        public static int TEXT_LEFT_PAD = 30;
        public static int TEXT_RIGHT_PAD = 25;
        public static int TEXT_RIGHT_PAD_NO_ICON = 8;
        //  CHECK BOX
        public static int CHECK_TOP_PAD = 4;
        public static int CHECK_LEFT_PAD = 15;
        public static int CHECK_BOX_SIZE = 12;
        //  EXPANSION BOX
        public static int EXPAND_BOX_TOP_PAD = 5;
        public static int EXPAND_BOX_LEFT_PAD = 3;
        public static int EXPAND_BOX_SIZE = 8;
        //  GROUP
        public static int GRP_INDENT = 3;//缩排
        //	LIST ITEMS
        public static int LIST_ITEM_INDENT = 18;
        public static int ICON_RIGHT_PAD = 25;
        public static int ICON_TOP_PAD = 3;
        public static int ICON_SIZE = 13;

        //从组到子条目的关联行
        public static int VERT_LINE_INDENT = (GRP_INDENT + 7);
        public static int VERT_LINE_GRP_TOP_OFFSET = 14;
        //	颜色配置常量
        public static int CS_ITEM_HEIGHT = 14;
        public static int CS_TOP_PAD = 1;
        public static int CS_PATCH_WIDTH = 15;
        public static int CS_PATCH_HEIGHT = 12;
        public static int CS_PATCH_LEFT_INDENT = (CHECK_LEFT_PAD);
        public static int CS_TEXT_LEFT_INDENT = (CS_PATCH_LEFT_INDENT + CS_PATCH_WIDTH + 3);
        public static int CS_TEXT_TOP_PAD = 3;
        //	SCROLLBAR
        public static int SCROLL_WIDTH = 15;

        //Group
        public static int INVALID_INDEX = -1;

        // 新的符号参数的常量
        public static int ICON_WIDTH = 24;
        public static int ICON_HEIGHT = 13;

        //*******************************************************
        //VB相关常量
        //*******************************************************
        public static int VB_SHIFT_BUTTON = 1;
        public static int VB_LEFT_BUTTON = 1;
        public static int VB_RIGHT_BUTTON = 2;
    }
}
