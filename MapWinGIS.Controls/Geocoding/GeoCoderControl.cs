using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapWinGIS;
using System.Threading;

namespace MapWinGIS.Controls.Geocoding
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class GeoCoderControl : UserControl
    {
        private AxMapWinGIS.AxMap map; 
        
        /// <summary>
        /// Creates a new instance of the GeoCoderControl
        /// </summary>
        public GeoCoderControl()
        {
            InitializeComponent();
            this.txtFind.KeyPress += new KeyPressEventHandler(txtFind_KeyPress);
            this.txtLat.KeyPress += new KeyPressEventHandler(txtLat_KeyPress);
            this.txtLong.KeyPress += new KeyPressEventHandler(txtLat_KeyPress);
        }

        /// <summary>
        /// Assign axmap control to geocoder
        /// </summary>
        public AxMapWinGIS.AxMap AxMap 
        { 
            set 
            {  
                #if OCX_VERSION49
                this.map = value; 
                if (map != null)
                {
                    map.ExtentsChanged += delegate(object sender, EventArgs e)
                    {
                        this.txtLat.Text = "";
                        this.txtLong.Text = "";
                        Extents ext = map.GeographicExtents;
                        if (ext != null)
                        {
                            this.txtLat.Text = ((ext.yMax + ext.yMin) / 2.0).ToString("0.0000000");
                            this.txtLong.Text = ((ext.xMax + ext.xMin) / 2.0).ToString("0.0000000");
                        }
                    };
                }
                #endif
            } 
        }

        /// <summary>
        /// Updates position on map
        /// </summary>
        private void txtLat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                this.txtFind.Text = "";
                this.listBox1.DataSource = null;
                double lat, lng;
                try
                {
                    double.TryParse(txtLat.Text, out lat);
                    double.TryParse(txtLong.Text, out lng);

                    List<Placemark> list = new List<Placemark>();
                    GeoCoderStatusCode status = GeoCoder.GetPlacemarks(new Point2D(lat, lng), out list);

                    if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                    {
                        this.listBox1.DataSource = list;
                    }

                    this.UpdateMapPosition(lat, lng);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid format");
                }
            }
        }

        /// <summary>
        /// Runs geocoding task based upon user input
        /// </summary>
        private void txtFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                this.listBox1.DataSource = null;
                List<Point2D> points = new List<Point2D>();
                GeoCoderStatusCode status = GeoCoder.GetPoints(txtFind.Text, out points);
                if (status == GeoCoderStatusCode.G_GEO_SUCCESS && points.Count > 0)
                {
                    this.txtLat.Text = points[0].Lat.ToString();
                    this.txtLong.Text = points[0].Lng.ToString();
                    this.UpdateMapPosition(points[0].Lat, points[0].Lng);
                    
                    if (this.chkShowAll.Checked)
                    {
                        ParameterizedThreadStart start = ShowAll;
                        Thread thread = new Thread(start);
                        thread.Priority = ThreadPriority.BelowNormal;
                        thread.IsBackground = true;
                        thread.Start(points);
                    }
                    else
                    {
                        this.listBox1.DataSource = points;
                    }
                }
            }
        }

        public void ShowAll(object obj)
        {
            List<Point2D> points = obj as List<Point2D>;
            if (points == null)
                return;

            GeoCoderStatusCode status =  GeoCoderStatusCode.Unknow;
            List<Placemark> list = new List<Placemark>();
            foreach (Point2D pnt in points)
            {
                Placemark place = GeoCoder.GetPlacemark(pnt, out status);
                place.Location = pnt;
                list.Add(place);
            }
            this.listBox1.DataSource = list;
        }

        /// <summary>
        /// Updates position 
        /// </summary>
        private void UpdateMapPosition(double lat, double lng)
        {
            #if OCX_VERSION49
            if (this.map != null)
            {
                Extents ext = map.GeographicExtents;
                if (ext != null)
                {
                    double dx = (ext.xMax - ext.xMin) / 2.0;
                    double dy = (ext.yMax - ext.yMin) / 2.0;
                    ext.SetBounds(lng - dx, lat - dy, 0.0, lng + dx, lat + dy, 0.0);
                    map.SetGeographicExtents(ext);
                }
            }
            #endif
        }

        /// <summary>
        /// Zooms to location
        /// </summary>
        private void listBox1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            #if OCX_VERSION49    
            if (listBox1.SelectedItem != null)
            {
                if (listBox1.SelectedItem is Point2D)
                {
                    Point2D pnt = listBox1.SelectedItem as Point2D;
                    this.UpdateMapPosition(pnt.Lat, pnt.Lng);
                }
                else if (listBox1.SelectedItem is Placemark)
                {
                    Placemark place = listBox1.SelectedItem as Placemark;
                    if (place.Location != null)
                    {
                        this.UpdateMapPosition(place.Location.Lat, place.Location.Lng);
                    }
                }
            }
            #endif
        }

    }
}
