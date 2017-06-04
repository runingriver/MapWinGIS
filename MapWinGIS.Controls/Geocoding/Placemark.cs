// This code is originally part of GMap.NET project: http://greatmaps.codeplex.com/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.Controls.Geocoding
{
    /// <summary>
    /// represents place info
    /// </summary>
    public class Placemark
    {
        string address;

        /// <summary>
        /// the address
        /// </summary>
        public string Address
        {
            get
            {
                return address;
            }
            internal set
            {
                address = value;
            }
        }

        public Point2D Location { get; set; }

        /// <summary>
        /// the accuracy of address
        /// </summary>
        public int Accuracy;

        // parsed values from address
        public string ThoroughfareName;
        public string LocalityName;
        public string PostalCodeNumber;
        public string CountryName;
        public string CountryNameCode;
        public string AdministrativeAreaName;
        public string SubAdministrativeAreaName;

        public Placemark(string address)
        {
            this.address = address;
            Location = null;
        }

        public override string ToString()
        {
            if (Location != null)
            {
                if (this.LocalityName != null && this.AdministrativeAreaName != null
                    && this.LocalityName.Trim().Length > 0 && this.AdministrativeAreaName.Trim().Length > 0)
                {
                    return string.Format("{0,18} {1}, {2}, {3}", Location.ToShortString(), this.LocalityName, this.AdministrativeAreaName, this.CountryName);
                }
                else
                {
                    return string.Format("{0,18} {1}", Location.ToShortString(), this.CountryName);
                }
            }
            else
            {
                return address;
            }
        }
    }
}
