using System;
using System.Drawing;
using System.Xml;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 实现Color到int，int到Color的转换
    /// 在interfaces的Globals中也有
    /// </summary>
    public class ColorScheme
    {
        public static void GetRGB(int color, out int r, out int g, out int b)
        {
            if (color < 0)
            {
                color = 0;
            }
            r = (int)(color & 0xFF);
            g = (int)(color & 0xFF00) / 256;	//shift right 8 bits
            b = (int)(color & 0xFF0000) / 65536; //shift right 16 bits
        }

        public static int HSLToRGB(int Hue, int Saturation, int Luminance)
        {
            int R = 0, G = 0, B = 0;
            int lMax, lMid, lMin;
            float q;

            lMax = (int)((Luminance * 255) / 100);
            lMin = (int)((100 - Saturation) * lMax / 100);
            q = (float)((lMax - lMin) / 60);

            if (Hue >= 0 && Hue <= 60)
            {
                lMid = (int)((Hue - 0) * q + lMin);
                R = lMax;
                G = lMid;
                B = lMin;
            }
            else if (Hue >= 60 && Hue <= 120)
            {
                lMid = (int)(-(Hue - 120) * q + lMin);
                R = lMid;
                G = lMax;
                B = lMin;
            }
            else if (Hue >= 120 && Hue <= 180)
            {
                lMid = (int)((Hue - 120) * q + lMin);
                R = lMin;
                G = lMax;
                B = lMid;
            }
            else if (Hue >= 180 && Hue <= 240)
            {
                lMid = (int)(-(Hue - 240) * q + lMin);
                R = lMin;
                G = lMid;
                B = lMax;
            }
            else if (Hue >= 240 && Hue <= 300)
            {
                lMid = (int)((Hue - 240) * q + lMin);
                R = lMid;
                G = lMin;
                B = lMax;
            }
            else if (Hue >= 300 && Hue <= 360)
            {
                lMid = (int)(-(Hue - 360) * q + lMin);
                R = lMax;
                G = lMin;
                B = lMid;
            }

            return B * 0x10000 + G * 0x100 + R;
        }

        public static int ColorToInt(Color c)
        {
            int retval = ((int)c.B) << 16;
            retval += ((int)c.G) << 8;
            return retval + ((int)c.R);
        }

        public static uint ColorToUInt(Color c)
        {
            return (uint)ColorToInt(c);
        }

        public static Color UIntToColor(UInt32 IntColor)
        {
            int r, g, b;
            GetRGB((int)(IntColor), out r, out g, out b);
            return Color.FromArgb(255, r, g, b);
        }

        public static Color IntToColor(int IntColor)
        {
            int r, g, b;
            GetRGB(IntColor, out r, out g, out b);
            return Color.FromArgb(255, r, g, b);
        }
    }

    public class ColoringSchemeTools
    {

        /// <summary>
        /// 从指定文件中导入颜色设置
        /// </summary>
        public static object ImportScheme(Interfaces.Layer lyr, string Filename)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root;

            if (lyr == null)
            {
                return null;
            }

            doc.Load(Filename);
            root = doc.DocumentElement;

            if (lyr.LayerType == Interfaces.eLayerType.LineShapefile || lyr.LayerType == Interfaces.eLayerType.PointShapefile || lyr.LayerType == Interfaces.eLayerType.PolygonShapefile)
            {
                try
                {
                    MapWinGIS.ShapefileColorScheme sch = new ShapefileColorScheme();
                    if (root.Attributes["SchemeType"].InnerText == "Shapefile")
                    {
                        if (ImportScheme(sch, (MapWinGIS.Shapefile)lyr.GetObject(), root["ShapefileColoringScheme"]))
                        {
                            return sch;
                        }
                    }
                    else
                    {
                        MapWinGIS.Utility.Logger.Message("文件中包含无效的颜色设置.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.Message;
                    return null;
                }
            }
            else //不是shapefile类型文件
            {
                try
                {
                    MapWinGIS.GridColorScheme sch = new MapWinGIS.GridColorScheme();
                    if (root.Attributes["SchemeType"].InnerText == "Grid")
                    {
                        try
                        {
                            if (root.Attributes["GridName"] != null)
                            {
                                lyr.Name = root.Attributes["GridName"].InnerText;
                            }
                            if (root.Attributes["GroupName"] != null)
                            {
                                string GroupName = root.Attributes["GroupName"].InnerText;
                                bool found = false; //是否在legend中找到Group匹配的名字
                                for (int i = 0; i < Program.frmMain.Legend.Groups.Count; i++)
                                {
                                    if (Program.frmMain.Legend.Groups[i].Text.ToLower().Trim() == GroupName.Trim().ToLower())
                                    {
                                        lyr.GroupHandle = Program.frmMain.Legend.Groups[i].Handle;
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    lyr.GroupHandle = Program.frmMain.Legend.Groups.Add(GroupName);
                                }
                            }
                        }
                        catch { }
                        if (ImportScheme(sch, root["GridColoringScheme"]))
                        {
                            return sch;
                        }
                    }
                    else //没有GridColorScheme设置
                    {
                        MapWinGIS.Utility.Logger.Message("文件中包含无效的颜色设置.");
                        return null;
                    }
                    
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    return null;
                }
            }
            return null;
        }

        private static bool ImportScheme(MapWinGIS.ShapefileColorScheme sch, MapWinGIS.Shapefile sf, XmlElement e)
        {
            string FldName;
            int i;
            MapWinGIS.ShapefileColorBreak brk;
            XmlNode n ;
            bool foundField = false;

            if (e == null)
            {
                return false;
            }

            FldName = e.Attributes["FieldName"].InnerText;
            for (i = 0; i < sf.NumFields; i++)
            {
                if (sf.Field[i].Name.ToLower() == FldName.ToLower())
                {
                    sch.FieldIndex = i;
                    foundField = true;
                    break;
                }
            }
            if (!foundField)
            {
                MapWinGIS.Utility.Logger.Message("无法找到 field \'" + FldName + "\'  无法导入coloring scheme.");
                return false;
            }
            sch.Key = e.Attributes["Key"].InnerText;

            for (i = 0; i < e.ChildNodes.Count; i++)
            {
                n = e.ChildNodes[i];
                brk = new MapWinGIS.ShapefileColorBreak();
                brk.Caption = n.Attributes["Caption"].InnerText;
                brk.StartColor = ColorScheme.ColorToUInt(Color.FromArgb(System.Convert.ToInt32(n.Attributes["StartColor"].InnerText)));
                brk.EndColor = ColorScheme.ColorToUInt(Color.FromArgb(System.Convert.ToInt32(n.Attributes["EndColor"].InnerText)));
                brk.StartValue = n.Attributes["StartValue"].InnerText;
                brk.EndValue = n.Attributes["EndValue"].InnerText;
                sch.Add(brk);
            }
            return true;
        }

        public static bool ImportScheme(MapWinGIS.GridColorScheme sch, XmlElement e)
        {
            int i;
            MapWinGIS.GridColorBreak brk;
            string t;
            double azimuth;
            double elevation;
            XmlNode n;

            if (e == null)
            {
                return false;
            }

            sch.Key = e.Attributes["Key"].InnerText;

            t = e.Attributes["AmbientIntensity"].InnerText;
            sch.AmbientIntensity = MapWinGIS.Utility.MiscUtils.ParseDouble(t, 0.7); 

            t = e.Attributes["LightSourceAzimuth"].InnerText;
            azimuth = MapWinGIS.Utility.MiscUtils.ParseDouble(t, 90.0); 

            t = e.Attributes["LightSourceElevation"].InnerText;
            elevation = MapWinGIS.Utility.MiscUtils.ParseDouble(t, 45.0); 
            sch.SetLightSource(azimuth, elevation);

            t = e.Attributes["LightSourceIntensity"].InnerText;
            sch.LightSourceIntensity = MapWinGIS.Utility.MiscUtils.ParseDouble(t, 0.7);

            for (i = 0; i <= e.ChildNodes.Count - 1; i++)
            {
                n = e.ChildNodes[i];
                brk = new MapWinGIS.GridColorBreak();
                brk.Caption = n.Attributes["Caption"].InnerText;
                brk.LowColor = ColorScheme.ColorToUInt(Color.FromArgb(System.Convert.ToInt32(n.Attributes["LowColor"].InnerText)));
                brk.HighColor = ColorScheme.ColorToUInt(Color.FromArgb(System.Convert.ToInt32(n.Attributes["HighColor"].InnerText)));
                brk.LowValue = MapWinGIS.Utility.MiscUtils.ParseDouble(n.Attributes["LowValue"].InnerText, 0.0); 
                brk.HighValue = MapWinGIS.Utility.MiscUtils.ParseDouble(n.Attributes["HighValue"].InnerText, 0.0); 
                brk.ColoringType = (MapWinGIS.ColoringType)Enum.Parse(typeof(MapWinGIS.ColoringType), n.Attributes["ColoringType"].InnerText);
                brk.GradientModel = (MapWinGIS.GradientModel)Enum.Parse(typeof(MapWinGIS.GradientModel), n.Attributes["GradientModel"].InnerText);
                sch.InsertBreak(brk);
            }
            return true;
        }


        /// <summary>
        /// 将图层颜色设置输出到一个指定路径
        /// </summary>
        public static bool ExportScheme(Interfaces.Layer lyr, string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement mainScheme;
            XmlElement root;
            XmlAttribute schemeType;
            root = doc.CreateElement("ColoringScheme");

            if (lyr == null)
            {
                return false;
            }

            if (lyr.LayerType == Interfaces.eLayerType.LineShapefile || lyr.LayerType == Interfaces.eLayerType.PointShapefile || lyr.LayerType == Interfaces.eLayerType.PolygonShapefile)
            {

                MapWinGIS.ShapefileColorScheme sch = (MapWinGIS.ShapefileColorScheme)lyr.ColoringScheme;
                MapWinGIS.Shapefile sf = (MapWinGIS.Shapefile)lyr.GetObject();
                XmlAttribute fldName;
                XmlAttribute key;

                if (sch == null || sch.NumBreaks() == 0)
                {
                    return false;
                }
                schemeType = doc.CreateAttribute("SchemeType");
                schemeType.InnerText = "Shapefile";
                root.Attributes.Append(schemeType);
                mainScheme = doc.CreateElement("ShapefileColoringScheme");
                fldName = doc.CreateAttribute("FieldName");
                key = doc.CreateAttribute("Key");
                fldName.InnerText = sf.Field[sch.FieldIndex].Name;
                key.InnerText = sch.Key;

                mainScheme.Attributes.Append(fldName);
                mainScheme.Attributes.Append(key);
                root.AppendChild(mainScheme);
                doc.AppendChild(root);
                if (ExportScheme(((MapWinGIS.ShapefileColorScheme)lyr.ColoringScheme), doc, mainScheme))
                {
                    doc.Save(path);
                    return true;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Message("导出 coloring scheme 失败.","错误");
                    return false;
                }

            }
            else if (lyr.LayerType == Interfaces.eLayerType.Grid)
            {
                MapWinGIS.GridColorScheme sch = (MapWinGIS.GridColorScheme)lyr.ColoringScheme;
                MapWinGIS.Grid grd = lyr.GetGridObject;
                XmlAttribute AmbientIntensity;
                XmlAttribute Key;
                XmlAttribute LightSourceAzimuth;
                XmlAttribute LightSourceElevation;
                XmlAttribute LightSourceIntensity;
                XmlAttribute NoDataColor;
                XmlAttribute GridName;
                XmlAttribute GroupName;
                XmlAttribute ImageLayerFillTransparency;
                XmlAttribute ImageUpsamplingMethod;
                XmlAttribute ImageDownsamplingMethod;

                if (sch == null || sch.NumBreaks == 0)
                {
                    return false;
                }
                GridName = doc.CreateAttribute("GridName");
                GroupName = doc.CreateAttribute("GroupName");
                schemeType = doc.CreateAttribute("SchemeType");
                schemeType.InnerText = "Grid";
                root.Attributes.Append(schemeType);

                AmbientIntensity = doc.CreateAttribute("AmbientIntensity");
                Key = doc.CreateAttribute("Key");
                LightSourceAzimuth = doc.CreateAttribute("LightSourceAzimuth");
                LightSourceElevation = doc.CreateAttribute("LightSourceElevation");
                LightSourceIntensity = doc.CreateAttribute("LightSourceIntensity");
                ImageLayerFillTransparency = doc.CreateAttribute("ImageLayerFillTransparency");
                ImageUpsamplingMethod = doc.CreateAttribute("ImageUpsamplingMethod");
                ImageDownsamplingMethod = doc.CreateAttribute("ImageDownsamplingMethod");

                NoDataColor = doc.CreateAttribute("NoDataColor");
                GridName.InnerText = lyr.Name;
                GroupName.InnerText = Program.frmMain.Legend.Groups.ItemByHandle(lyr.GroupHandle).Text;
                AmbientIntensity.InnerText = (sch.AmbientIntensity).ToString();
                Key.InnerText = sch.Key;
                LightSourceAzimuth.InnerText = (sch.LightSourceAzimuth).ToString();
                LightSourceElevation.InnerText = (sch.LightSourceElevation).ToString();
                LightSourceIntensity.InnerText = (sch.LightSourceIntensity).ToString();
                NoDataColor.InnerText = (ColorScheme.UIntToColor(sch.NoDataColor).ToArgb()).ToString();
                ImageLayerFillTransparency.InnerText = ((System.Convert.ToInt32(lyr.ImageLayerFillTransparency * 100)) / 100).ToString(); 

                MapWinGIS.Image img = new MapWinGIS.Image();
                img = (MapWinGIS.Image)(Program.frmMain.MapMain.get_GetObject(lyr.Handle));
                if (img.DownsamplingMode == MapWinGIS.tkInterpolationMode.imBicubic)
                {
                    ImageDownsamplingMethod.InnerText = "Bicubic";
                }
                else if (img.DownsamplingMode == MapWinGIS.tkInterpolationMode.imBilinear)
                {
                    ImageDownsamplingMethod.InnerText = "Bilinear";
                }
                else if (img.DownsamplingMode == MapWinGIS.tkInterpolationMode.imHighQualityBicubic)
                {
                    ImageDownsamplingMethod.InnerText = "HighQualityBicubic";
                }
                else if (img.DownsamplingMode == MapWinGIS.tkInterpolationMode.imHighQualityBilinear)
                {
                    ImageDownsamplingMethod.InnerText = "HighQualityBilinear";
                }
                else if (img.DownsamplingMode == MapWinGIS.tkInterpolationMode.imNone)
                {
                    ImageDownsamplingMethod.InnerText = "None";
                }
                else
                {
                    ImageDownsamplingMethod.InnerText = "None";
                }

                if (img.UpsamplingMode == MapWinGIS.tkInterpolationMode.imBicubic)
                {
                    ImageUpsamplingMethod.InnerText = "Bicubic";
                }
                else if (img.UpsamplingMode == MapWinGIS.tkInterpolationMode.imBilinear)
                {
                    ImageUpsamplingMethod.InnerText = "Bilinear";
                }
                else if (img.UpsamplingMode == MapWinGIS.tkInterpolationMode.imHighQualityBicubic)
                {
                    ImageUpsamplingMethod.InnerText = "HighQualityBicubic";
                }
                else if (img.UpsamplingMode == MapWinGIS.tkInterpolationMode.imHighQualityBilinear)
                {
                    ImageUpsamplingMethod.InnerText = "HighQualityBilinear";
                }
                else if (img.UpsamplingMode == MapWinGIS.tkInterpolationMode.imNone)
                {
                    ImageUpsamplingMethod.InnerText = "None";
                }
                else
                {
                    ImageUpsamplingMethod.InnerText = "None";
                }

                mainScheme = doc.CreateElement("GridColoringScheme");
                mainScheme.Attributes.Append(AmbientIntensity);
                mainScheme.Attributes.Append(Key);
                mainScheme.Attributes.Append(LightSourceAzimuth);
                mainScheme.Attributes.Append(LightSourceElevation);
                mainScheme.Attributes.Append(LightSourceIntensity);
                mainScheme.Attributes.Append(NoDataColor);
                mainScheme.Attributes.Append(ImageLayerFillTransparency);
                mainScheme.Attributes.Append(ImageUpsamplingMethod); 
                mainScheme.Attributes.Append(ImageDownsamplingMethod); 

                root.AppendChild(mainScheme);
                root.Attributes.Append(GridName);
                root.Attributes.Append(GroupName);
                doc.AppendChild(root);
                if (ExportScheme(((MapWinGIS.GridColorScheme)lyr.ColoringScheme), doc, mainScheme))
                {
                    doc.Save(path);
                    return true;
                }
                else
                {
                    MapWinGIS.Utility.Logger.Message("导出 coloring scheme 失败.", "错误");
                    return false;
                }
            }
            return false;
        }

        private static bool ExportScheme(MapWinGIS.ShapefileColorScheme Scheme, XmlDocument RootDoc, XmlElement Parent)
        {
            int i;
            XmlElement brk;
            XmlAttribute caption;
            XmlAttribute sValue;
            XmlAttribute eValue;
            XmlAttribute sColor;
            XmlAttribute eColor;
            MapWinGIS.ShapefileColorBreak curBrk;

            for (i = 0; i < Scheme.NumBreaks(); i++)
            {
                curBrk = Scheme.ColorBreak[i];
                brk = RootDoc.CreateElement("Break");
                caption = RootDoc.CreateAttribute("Caption");
                sValue = RootDoc.CreateAttribute("StartValue");
                eValue = RootDoc.CreateAttribute("EndValue");
                sColor = RootDoc.CreateAttribute("StartColor");
                eColor = RootDoc.CreateAttribute("EndColor");
                caption.InnerText = curBrk.Caption;
                sValue.InnerText = (curBrk.StartValue).ToString();
                eValue.InnerText = (curBrk.EndValue).ToString();
                sColor.InnerText = (ColorScheme.UIntToColor(curBrk.StartColor).ToArgb()).ToString();
                eColor.InnerText = (ColorScheme.UIntToColor(curBrk.EndColor).ToArgb()).ToString();
                brk.Attributes.Append(caption);
                brk.Attributes.Append(sValue);
                brk.Attributes.Append(eValue);
                brk.Attributes.Append(sColor);
                brk.Attributes.Append(eColor);
                Parent.AppendChild(brk);
                curBrk = null;
            }
            return true;
        }

        private static bool ExportScheme(MapWinGIS.GridColorScheme Scheme, XmlDocument RootDoc, XmlElement Parent)
        {
            int i;
            XmlElement brk;
            XmlAttribute caption;
            XmlAttribute sValue;
            XmlAttribute eValue;
            XmlAttribute sColor;
            XmlAttribute eColor;
            XmlAttribute coloringType;
            XmlAttribute gradientModel;
            MapWinGIS.GridColorBreak curBrk;

            if (Scheme == null || Scheme.NumBreaks == 0)
            {
                return false;
            }

            for (i = 0; i <= Scheme.NumBreaks - 1; i++)
            {
                curBrk = Scheme.Break[i];
                brk = RootDoc.CreateElement("Break");
                caption = RootDoc.CreateAttribute("Caption");
                sValue = RootDoc.CreateAttribute("LowValue");
                eValue = RootDoc.CreateAttribute("HighValue");
                sColor = RootDoc.CreateAttribute("LowColor");
                eColor = RootDoc.CreateAttribute("HighColor");
                coloringType = RootDoc.CreateAttribute("ColoringType");
                gradientModel = RootDoc.CreateAttribute("GradientModel");
                caption.InnerText = curBrk.Caption;
                sValue.InnerText = (curBrk.LowValue).ToString();
                eValue.InnerText = (curBrk.HighValue).ToString();
                sColor.InnerText = (ColorScheme.UIntToColor(curBrk.LowColor).ToArgb()).ToString();
                eColor.InnerText = (ColorScheme.UIntToColor(curBrk.HighColor).ToArgb()).ToString();
                coloringType.InnerText = (curBrk.ColoringType).ToString();
                gradientModel.InnerText = (curBrk.GradientModel).ToString();
                brk.Attributes.Append(caption);
                brk.Attributes.Append(sValue);
                brk.Attributes.Append(eValue);
                brk.Attributes.Append(sColor);
                brk.Attributes.Append(eColor);
                brk.Attributes.Append(coloringType);
                brk.Attributes.Append(gradientModel);
                Parent.AppendChild(brk);
                curBrk = null;
            }
            return true;
        }


    }
}
