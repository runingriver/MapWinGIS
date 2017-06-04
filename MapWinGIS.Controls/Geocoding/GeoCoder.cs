// This code is originally part of GMap.NET project: http://greatmaps.codeplex.com/

namespace MapWinGIS.Controls.Geocoding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Xml;
    using System.Diagnostics;
    using System.Net;
    using System.IO;

    public class GeoCoder
    {
        #region Declarations
        private static string LanguageStr = "en";
        private static string Server = "google.com";

        private static string UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
        private static string APIKey = @"ABQIAAAAWaQgWiEBF3lW97ifKnAczhRAzBk5Igf8Z5n2W3hNnMT0j2TikxTLtVIGU7hCLLHMAuAMt-BO5UrEWA";
        private static readonly string requestAccept = "*/*";
        private static int TimeoutMs = 22 * 1000;
        #endregion

        #region Public API
        /// <summary>
        /// Gets complete list of points for a given keyword
        /// </summary>
        public static GeoCoderStatusCode GetPoints(string keywords, out List<Point2D> pointList)
        {
            return GetLatLngFromGeocoderUrl(MakeGeocoderUrl(keywords, LanguageStr), out pointList);
        }

        /// <summary>
        /// Returns the first point for the given keywords
        /// </summary>
        public static Point2D GetPoint(string keywords, out GeoCoderStatusCode status)
        {
            List<Point2D> pointList;
            status = GetPoints(keywords, out pointList);
            return pointList != null && pointList.Count > 0 ? pointList[0] : null;
        }

        /// <summary>
        /// Returns list of names (placemarks) for the given point
        /// </summary>
        public static GeoCoderStatusCode GetPlacemarks(Point2D location, out List<Placemark> placemarkList)
        {
            return GetPlacemarkFromReverseGeocoderUrl(MakeReverseGeocoderUrl(location, LanguageStr), out placemarkList);
        }

        /// <summary>
        /// Returns the first name for a given point
        /// </summary>
        public static Placemark GetPlacemark(Point2D location, out GeoCoderStatusCode status)
        {
            List<Placemark> pointList;
            status = GetPlacemarks(location, out pointList);
            return pointList != null && pointList.Count > 0 ? pointList[0] : null;
        }
        #endregion

        #region URL
        private static readonly string ReverseGeocoderUrlFormat = "http://maps.{4}/maps/geo?hl={0}&ll={1},{2}&output=xml&key={3}";
        private static readonly string GeocoderUrlFormat = "http://maps.{3}/maps/geo?q={0}&hl={1}&output=kml&key={2}";

        private static string MakeGeocoderUrl(string keywords, string language)
        {
            return string.Format(GeocoderUrlFormat, keywords.Replace(' ', '+'), language, APIKey, Server);
        }

        private static string MakeReverseGeocoderUrl(Point2D pt, string language)
        {
            return string.Format(CultureInfo.InvariantCulture, ReverseGeocoderUrlFormat, language, pt.Lat, pt.Lng, APIKey, Server);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Gets list of points by sending HTTP request with keywords
        /// </summary>
        private static GeoCoderStatusCode GetLatLngFromGeocoderUrl(string url, out List<Point2D> pointList)
        {
            var status = GeoCoderStatusCode.Unknow;
            pointList = null;

            try
            {
                string urlEnd = url.Substring(url.IndexOf("geo?q="));

                string geo = GetContentUsingHttp(url);

                if (!string.IsNullOrEmpty(geo))
                {
                    if (geo.StartsWith("200"))
                    {
                        // true : 200,4,56.1451640,22.0681787
                        // false: 602,0,0,0
                        string[] values = geo.Split(',');
                        if (values.Length == 4)
                        {
                            status = (GeoCoderStatusCode)int.Parse(values[0]);
                            if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                            {
                                double lat = double.Parse(values[2], CultureInfo.InvariantCulture);
                                double lng = double.Parse(values[3], CultureInfo.InvariantCulture);

                                pointList = new List<Point2D>();
                                pointList.Add(new Point2D(lat, lng));
                            }
                        }
                    }
                    else if (geo.StartsWith("<?xml"))
                    {
                        #region -- kml response --
                        //<?xml version="1.0" encoding="UTF-8" ?>
                        //<kml xmlns="http://earth.google.com/kml/2.0">
                        //<Response>
                        //  <name>Lithuania, Vilnius</name>
                        //  <Status>
                        //    <code>200</code>
                        //    <request>geocode</request>
                        //  </Status>
                        //  <Placemark id="p1">
                        //    <address>Vilnius, Lithuania</address>
                        //    <AddressDetails Accuracy="4" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0">
                        //      <Country>
                        //      <CountryNameCode>LT</CountryNameCode>
                        //      <CountryName>Lithuania</CountryName>

                        //      <SubAdministrativeArea>
                        //         <SubAdministrativeAreaName>Vilnius Region</SubAdministrativeAreaName>
                        //         <Locality>
                        //            <LocalityName>Vilnius</LocalityName>
                        //         </Locality>
                        //     </SubAdministrativeArea>

                        //     </Country>
                        //     </AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="54.8616279" south="54.4663633" east="25.4839269" west="24.9688846" />
                        //    </ExtendedData>
                        //    <Point><coordinates>25.2800243,54.6893865,0</coordinates></Point>
                        //  </Placemark>
                        //</Response>
                        //</kml> 
                        #endregion

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(geo);

                        XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
                        nsMgr.AddNamespace("sm", string.Format("http://earth.{0}/kml/2.0", Server));
                        nsMgr.AddNamespace("sn", "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0");

                        XmlNode nn = doc.SelectSingleNode("//sm:Status/sm:code", nsMgr);
                        if (nn != null)
                        {
                            status = (GeoCoderStatusCode)int.Parse(nn.InnerText);
                            if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                            {
                                pointList = new List<Point2D>();

                                XmlNodeList l = doc.SelectNodes("/sm:kml/sm:Response/sm:Placemark", nsMgr);
                                if (l != null)
                                {
                                    foreach (XmlNode n in l)
                                    {
                                        nn = n.SelectSingleNode("sm:Point/sm:coordinates", nsMgr);
                                        if (nn != null)
                                        {
                                            string[] values = nn.InnerText.Split(',');
                                            if (values.Length >= 2)
                                            {
                                                double lat = double.Parse(values[1], CultureInfo.InvariantCulture);
                                                double lng = double.Parse(values[0], CultureInfo.InvariantCulture);

                                                pointList.Add(new Point2D(lat, lng));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetLatLngFromGeocoderUrl: " + ex);
            }

            return status;
        }

        /// <summary>
        /// Gets list of names by sending HTTP request with location
        /// </summary>
        private static GeoCoderStatusCode GetPlacemarkFromReverseGeocoderUrl(string url, out List<Placemark> placemarkList)
        {
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;
            placemarkList = null;

            try
            {
                string urlEnd = url.Substring(url.IndexOf("geo?hl="));

                string reverse = reverse = GetContentUsingHttp(url);

                if (!string.IsNullOrEmpty(reverse))
                {
                    if (reverse.StartsWith("200"))
                    {
                        string acc = reverse.Substring(0, reverse.IndexOf('\"'));
                        var ret = new Placemark(reverse.Substring(reverse.IndexOf('\"')));
                        ret.Accuracy = int.Parse(acc.Split(',').GetValue(1) as string);
                        placemarkList = new List<Placemark>();
                        placemarkList.Add(ret);
                        status = GeoCoderStatusCode.G_GEO_SUCCESS;
                    }
                    else if (reverse.StartsWith("<?xml"))
                    {
                        #region -- kml version --
                        //<?xml version="1.0" encoding="UTF-8" ?>
                        //<kml xmlns="http://earth.server.com/kml/2.0">
                        // <Response>
                        //  <name>55.023322,24.668408</name>
                        //  <Status>
                        //    <code>200</code>
                        //    <request>geocode</request>
                        //  </Status>

                        //  <Placemark id="p1">
                        //    <address>4313, Širvintos 19023, Lithuania</address>
                        //    <AddressDetails Accuracy="6" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"><Country><CountryNameCode>LT</CountryNameCode><CountryName>Lithuania</CountryName><SubAdministrativeArea><SubAdministrativeAreaName>Vilnius Region</SubAdministrativeAreaName><Locality><LocalityName>Širvintos</LocalityName><Thoroughfare><ThoroughfareName>4313</ThoroughfareName></Thoroughfare><PostalCode><PostalCodeNumber>19023</PostalCodeNumber></PostalCode></Locality></SubAdministrativeArea></Country></AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="55.0270661" south="55.0207709" east="24.6711965" west="24.6573382" />
                        //    </ExtendedData>
                        //    <Point><coordinates>24.6642677,55.0239187,0</coordinates></Point>
                        //  </Placemark>

                        //  <Placemark id="p2">
                        //    <address>Širvintos 19023, Lithuania</address>
                        //    <AddressDetails Accuracy="5" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"><Country><CountryNameCode>LT</CountryNameCode><CountryName>Lithuania</CountryName><SubAdministrativeArea><SubAdministrativeAreaName>Vilnius Region</SubAdministrativeAreaName><Locality><LocalityName>Širvintos</LocalityName><PostalCode><PostalCodeNumber>19023</PostalCodeNumber></PostalCode></Locality></SubAdministrativeArea></Country></AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="55.1109513" south="54.9867479" east="24.7563286" west="24.5854650" />
                        //    </ExtendedData>
                        //    <Point><coordinates>24.6778290,55.0561428,0</coordinates></Point>
                        //  </Placemark>

                        //  <Placemark id="p3">
                        //    <address>Širvintos, Lithuania</address>
                        //    <AddressDetails Accuracy="4" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"><Country><CountryNameCode>LT</CountryNameCode><CountryName>Lithuania</CountryName><SubAdministrativeArea><SubAdministrativeAreaName>Vilnius Region</SubAdministrativeAreaName><Locality><LocalityName>Širvintos</LocalityName></Locality></SubAdministrativeArea></Country></AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="55.1597127" south="54.8595715" east="25.2358124" west="24.5536348" />
                        //    </ExtendedData>
                        //    <Point><coordinates>24.9447696,55.0482439,0</coordinates></Point>
                        //  </Placemark>

                        //  <Placemark id="p4">
                        //    <address>Vilnius Region, Lithuania</address>
                        //    <AddressDetails Accuracy="3" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"><Country><CountryNameCode>LT</CountryNameCode><CountryName>Lithuania</CountryName><SubAdministrativeArea><SubAdministrativeAreaName>Vilnius Region</SubAdministrativeAreaName></SubAdministrativeArea></Country></AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="55.5177330" south="54.1276791" east="26.7590747" west="24.3866334" />
                        //    </ExtendedData>
                        //    <Point><coordinates>25.2182138,54.8086502,0</coordinates></Point>
                        //  </Placemark>

                        //  <Placemark id="p5">
                        //    <address>Lithuania</address>
                        //    <AddressDetails Accuracy="1" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"><Country><CountryNameCode>LT</CountryNameCode><CountryName>Lithuania</CountryName></Country></AddressDetails>
                        //    <ExtendedData>
                        //      <LatLonBox north="56.4503174" south="53.8986720" east="26.8356500" west="20.9310000" />
                        //    </ExtendedData>
                        //    <Point><coordinates>23.8812750,55.1694380,0</coordinates></Point>
                        //  </Placemark>
                        //</Response>
                        //</kml> 
                        #endregion

                        #region KML
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(reverse);

                        XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
                        nsMgr.AddNamespace("sm", string.Format("http://earth.{0}/kml/2.0", Server));
                        nsMgr.AddNamespace("sn", "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0");

                        var codeNode = doc.SelectSingleNode("//sm:Status/sm:code", nsMgr);
                        if (codeNode != null)
                        {
                            status = (GeoCoderStatusCode)int.Parse(codeNode.InnerText);
                            if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                            {
                                placemarkList = new List<Placemark>();
                                XmlNodeList l = doc.SelectNodes("/sm:kml/sm:Response/sm:Placemark", nsMgr);
                                if (l != null)
                                {
                                    foreach (XmlNode n in l)
                                    {
                                        XmlNode nnd, nnl, nn;
                                        {
                                            nn = n.SelectSingleNode("sm:address", nsMgr);
                                            if (nn != null)
                                            {
                                                var ret = new Placemark(nn.InnerText);

                                                nnd = n.SelectSingleNode("sn:AddressDetails", nsMgr);
                                                if (nnd != null)
                                                {
                                                    nn = nnd.SelectSingleNode("@Accuracy", nsMgr);
                                                    if (nn != null)
                                                    {
                                                        ret.Accuracy = int.Parse(nn.InnerText);
                                                    }

                                                    nn = nnd.SelectSingleNode("sn:Country/sn:CountryNameCode", nsMgr);
                                                    if (nn != null)
                                                    {
                                                        ret.CountryNameCode = nn.InnerText;
                                                    }

                                                    nn = nnd.SelectSingleNode("sn:Country/sn:CountryName", nsMgr);
                                                    if (nn != null)
                                                    {
                                                        ret.CountryName = nn.InnerText;
                                                    }

                                                    nn = nnd.SelectSingleNode("descendant::sn:AdministrativeArea/sn:AdministrativeAreaName", nsMgr);
                                                    if (nn != null)
                                                    {
                                                        ret.AdministrativeAreaName = nn.InnerText;
                                                    }

                                                    nn = nnd.SelectSingleNode("descendant::sn:SubAdministrativeArea/sn:SubAdministrativeAreaName", nsMgr);
                                                    if (nn != null)
                                                    {
                                                        ret.SubAdministrativeAreaName = nn.InnerText;
                                                    }

                                                    // Locality or DependentLocality tag ?
                                                    nnl = nnd.SelectSingleNode("descendant::sn:Locality", nsMgr) ?? nnd.SelectSingleNode("descendant::sn:DependentLocality", nsMgr);
                                                    if (nnl != null)
                                                    {
                                                        nn = nnl.SelectSingleNode(string.Format("sn:{0}Name", nnl.Name), nsMgr);
                                                        if (nn != null)
                                                        {
                                                            ret.LocalityName = nn.InnerText;
                                                        }

                                                        nn = nnl.SelectSingleNode("sn:Thoroughfare/sn:ThoroughfareName", nsMgr);
                                                        if (nn != null)
                                                        {
                                                            ret.ThoroughfareName = nn.InnerText;
                                                        }

                                                        nn = nnl.SelectSingleNode("sn:PostalCode/sn:PostalCodeNumber", nsMgr);
                                                        if (nn != null)
                                                        {
                                                            ret.PostalCodeNumber = nn.InnerText;
                                                        }
                                                    }
                                                }
                                                placemarkList.Add(ret);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                placemarkList = null;
                Debug.WriteLine("GetPlacemarkReverseGeocoderUrl: " + ex.ToString());
            }

            return status;
        }

        /// <summary>
        /// HTTP request-response routine
        /// </summary>
        /// <param name="url">Url to request</param>
        /// <returns>Http response string</returns>
        private static string GetContentUsingHttp(string url)
        {
            string ret = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //if (WebProxy != null)
            //{
            //    request.Proxy = WebProxy;
            //}

            request.UserAgent = UserAgent;
            request.Timeout = TimeoutMs;
            request.ReadWriteTimeout = TimeoutMs * 6;
            request.Accept = requestAccept;
            //request.Referer = RefererUrl;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader read = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            ret = read.ReadToEnd();
                        }
                    }
                }
                response.Close();
            }

            return ret;
        }
        #endregion
    }
}
