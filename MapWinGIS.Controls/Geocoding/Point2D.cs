using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.Controls.Geocoding
{
    /// <summary>
    /// Double precision point
    /// </summary>
    public class Point2D
    {
        public double X;
        public double Y;

        public double Lat { get { return Y; } }
        public double Lng { get { return X; } }

        public Point2D(double Lat, double Lng)
        {
            Y = Lat;
            X = Lng;
        }

        public override string ToString()
        {
            return String.Format("({0}; {1})", Y, X);
        }

        public string ToShortString()
        {
            return String.Format("({0,7};{1,8})", Y.ToString("0.000"), X.ToString("0.000"));
        }
    }
}
