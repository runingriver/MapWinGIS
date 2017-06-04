using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace MapWinGIS.MainProgram
{
    partial class XmlProjectFile
    {
        /// <summary>
        /// 未实现
        /// </summary>
        private void AddShapeFileElement(XmlDocument m_Doc, Interfaces.Layer sfl, XmlNode parent)
        { }

        /// <summary>
        /// 未实现
        /// </summary>
        private void AddGridElement(XmlDocument m_Doc, Interfaces.Layer gridFileLayer, XmlNode parent)
        { }

        /// <summary>
        /// 从mwsr中取得每个shape的颜色设置，并应用到图层中
        /// </summary>
        private void LoadShpFileColoringScheme(XmlElement legend, int handle)
        {
            MapWinGIS.ShapefileColorScheme shpscheme = new MapWinGIS.ShapefileColorScheme();
            int numOfBreaks;
            MapWinGIS.ShapefileColorBreak _break;
            int i;

            try
            {
                //设置ShapefileColorScheme
                shpscheme.FieldIndex = int.Parse(legend.Attributes["FieldIndex"].InnerText);
                shpscheme.LayerHandle = handle;
                shpscheme.Key = legend.Attributes["Key"].InnerText;

                try
                {
                    Program.frmMain.Legend.Layers.ItemByHandle(handle).ColorSchemeFieldCaption = legend.Attributes["SchemeCaption"].InnerText;
                }
                catch
                {
                }

                //设置所有的breaks
                numOfBreaks = legend["ColorBreaks"].ChildNodes.Count;
                for (i = 0; i < numOfBreaks; i++)
                {
                    object with_1 = legend["ColorBreaks"].ChildNodes[i];
                    _break = new MapWinGIS.ShapefileColorBreak();
                    _break.Caption = legend["ColorBreaks"].ChildNodes[i].Attributes["Caption"].InnerText;
                    _break.StartColor = System.Convert.ToUInt32(legend["ColorBreaks"].ChildNodes[i].Attributes["StartColor"].InnerText);
                    _break.EndColor = System.Convert.ToUInt32(legend["ColorBreaks"].ChildNodes[i].Attributes["EndColor"].InnerText);
                    if (legend["ColorBreaks"].ChildNodes[i].Attributes["StartValue"].InnerText == "(null)")
                    {
                        _break.StartValue = null;
                    }
                    else
                    {
                        _break.StartValue = legend["ColorBreaks"].ChildNodes[i].Attributes["StartValue"].InnerText;
                    }

                    if (legend["ColorBreaks"].ChildNodes[i].Attributes["EndValue"].InnerText == "(null)")
                    {
                        _break.EndValue = null;
                    }
                    else
                    {
                        _break.EndValue = legend["ColorBreaks"].ChildNodes[i].Attributes["EndValue"].InnerText;
                    }

                    if (legend["ColorBreaks"].ChildNodes[i].Attributes["Visible"] != null && legend["ColorBreaks"].ChildNodes[i].Attributes["Visible"].InnerText != "")
                    {
                        _break.Visible = bool.Parse(legend["ColorBreaks"].ChildNodes[i].Attributes["Visible"].InnerText);
                    }

                    shpscheme.Add(_break);
                }

                if (numOfBreaks > 0)
                {
                    //set that layers scheme and redraw the legend
                    //设置Layer的颜色设置，并重回legend
                    Program.frmMain.Layers[handle].ColoringScheme = shpscheme;
                    Program.frmMain.Legend.Refresh();
                }

            }
            catch (System.Exception e)
            {
                m_ErrorMsg += "在LoadShpFileColoringScheme()方法中出错：" + e.Message + "\r\n";
                m_ErrorOccured = true;
            }
        }

        /// <summary>
        /// 序列化点图像
        /// 未实现
        /// </summary>
        public void SerializePointImageScheme(Interfaces.ShapefilePointImageScheme PointImgScheme, XmlDocument doc, XmlElement root)
        { }

        /// <summary>
        /// 反序列化点图像
        /// 从mwsr中获得点信息并翻译，应用到图层上。
        /// </summary>
        public void DeserializePointImageScheme(int newHandle, XmlElement root)
        {
            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
            try
            {
                MapWinGIS.Utility.ImageUtils imgUtil = new MapWinGIS.Utility.ImageUtils();

                bool found = false;
                Interfaces.ShapefilePointImageScheme csh = new Interfaces.ShapefilePointImageScheme(newHandle);

                Hashtable TranslationTable = new Hashtable();

                foreach (XmlElement xe1 in root)
                {
                    if (xe1.Name == "PointImageScheme")
                    {
                        found = true;
                        csh.FieldIndex = long.Parse(xe1.Attributes["FieldIndex"].InnerText);

                        foreach (XmlElement xe2 in xe1.ChildNodes)
                        {
                            if (xe2.Name == "ImageData")
                            {
                                foreach (XmlElement xe3 in xe2.ChildNodes)
                                {
                                    long origIndex = long.Parse(xe3.Attributes["ID"].InnerText);

                                    string imgtype;
                                    imgtype = xe3["Image"].Attributes["Type"].InnerText;
                                    System.Drawing.Image img = (System.Drawing.Image)(ConvertStringToImage(xe3["Image"].InnerText, imgtype));

                                    MapWinGIS.Image ico = new MapWinGIS.Image();
                                    ico.Picture = (stdole.IPictureDisp)imgUtil.ImageToIPictureDisp(img);
                                    if (ico != null)
                                    {
                                        ico.TransparencyColor = (uint)ico.Value[0, 0];
                                    }

                                    int newidx = System.Convert.ToInt32(Program.frmMain.MapMain.set_UDPointImageListAdd(newHandle, ico));
                                    TranslationTable.Add(origIndex, newidx);
                                    img = null;
                                }
                            }
                            else if (xe2.Name == "ItemData")
                            {
                                foreach (XmlElement xe3 in xe2.ChildNodes)
                                {
                                    if (xe3.Name == "Item")
                                    {
                                        string tag = xe3.Attributes["MatchValue"].InnerText;
                                        long imgIndex = long.Parse(xe3.Attributes["ImgIndex"].InnerText);
                                        long actualIndex = -1;
                                        if (TranslationTable.Contains(imgIndex))
                                        {
                                            actualIndex = System.Convert.ToInt64(TranslationTable[imgIndex]);
                                        }
                                        else
                                        {
                                            actualIndex = imgIndex;
                                        }

                                        if (actualIndex != -1 && tag != "")
                                        {
                                            csh.Items.Add(tag, actualIndex);
                                        }

                                        MapWinGIS.Shapefile sf;

                                        sf = (MapWinGIS.Shapefile)(Program.frmMain.m_Layers[newHandle].GetObject());
                                        if (sf == null)
                                        {
                                            Program.g_error = "获取Shapefile 对象失败。";
                                            return;
                                        }

                                        if (actualIndex != -1)
                                        {
                                            for (int j = 0; j < sf.NumShapes; j++)
                                            {
                                                if (sf.CellValue[(int)csh.FieldIndex, j].ToString() == tag)
                                                {
                                                    Program.frmMain.MapMain.set_ShapePointImageListID(newHandle, j, (int)actualIndex);
                                                    if (Program.frmMain.MapMain.get_ShapePointType(newHandle, j) != MapWinGIS.tkPointType.ptImageList)
                                                    {
                                                        Program.frmMain.MapMain.set_ShapePointType(newHandle, j, MapWinGIS.tkPointType.ptImageList);
                                                    }
                                                    Program.frmMain.MapMain.set_ShapePointSize(newHandle, j, 1);
                                                }
                                            }
                                        }

                                        sf = null;
                                    }
                                }
                            }
                            else if (xe2.Name == "ItemVisibility")
                            {
                                foreach (XmlElement xe3 in xe2.ChildNodes)
                                {
                                    if (xe3.Name == "Item")
                                    {
                                        string tag = xe3.Attributes["MatchValue"].InnerText;
                                        bool vis = bool.Parse(xe3.Attributes["Visible"].InnerText);
                                        csh.ItemVisibility.Add(tag, vis);

                                        MapWinGIS.Shapefile sf;

                                        sf = (MapWinGIS.Shapefile)(Program.frmMain.m_Layers[newHandle].GetObject());
                                        if (sf == null)
                                        {
                                            Program.g_error = "获取Shapefile 对象失败.";
                                            return;
                                        }

                                        for (int j = 0; j < sf.NumShapes; j++)
                                        {
                                            if (sf.CellValue[(int)csh.FieldIndex, j].ToString() == tag)
                                            {
                                                Program.frmMain.MapMain.set_ShapeVisible(newHandle, j, vis);
                                            }
                                        }

                                        sf = null;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                if (found)
                {
                    Program.frmMain.Legend.Layers.ItemByHandle(newHandle).PointImageScheme = csh;
                }

                TranslationTable.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("DEBUG: " + ex.ToString());
            }
            finally
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
            }
        }

        /// <summary>
        /// 反序列化填充方式
        /// 将mwsr中的信息翻译并应用到图层上
        /// </summary>
        public void DeserializeFillStippleScheme(int newHandle, XmlElement root)
        {
            Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);
            try
            {
                Interfaces.ShapefileFillStippleScheme csh = new Interfaces.ShapefileFillStippleScheme();
                csh.FieldHandle = -1;

                foreach (XmlElement xe in root)
                {
                    if (xe.Name == "FillStippleScheme")
                    {
                        csh.FieldHandle = long.Parse(xe.Attributes["FieldIndex"].InnerText);

                        try
                        {
                            Program.frmMain.Legend.Layers.ItemByHandle(newHandle).StippleSchemeFieldCaption = xe.Attributes["StippleCaption"].InnerText;
                        }
                        catch
                        {
                        }

                        foreach (XmlElement xe2 in xe.ChildNodes)
                        {
                            if (xe2.Name == "StippleBreak")
                            {
                                if (xe2.Attributes["Value"] != null && xe2.Attributes["Transparent"] != null && xe2.Attributes["LineColor"] != null && xe2.Attributes["Hatch"] != null)
                                {
                                    try
                                    {
                                        csh.AddHatch(xe2.Attributes["Value"].InnerText, bool.Parse(xe2.Attributes["Transparent"].InnerText), System.Drawing.Color.FromArgb(int.Parse(xe2.Attributes["LineColor"].InnerText)), StringToStipple(xe2.Attributes["Hatch"].InnerText));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }

                if (csh.FieldHandle > -1)
                {
                    Program.frmMain.m_Layers[newHandle].FillStippleScheme = csh;
                    Program.frmMain.m_Layers[newHandle].HatchingRecalculate();
                }
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Dbg("DEBUG: " + ex.ToString());
            }
            finally
            {
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
            }
        }

        /// <summary>
        /// 从mwsr中加载shape的属性列表
        /// </summary>
        private void LoadShapePropertiesList(XmlElement propList, int handle)
        {
            try
            {
                foreach (XmlElement sProp in propList.ChildNodes)
                {
                    int ix = int.Parse(sProp.GetAttribute("ShapeIndex"));
                    foreach (XmlAttribute xmla in sProp.Attributes)
                    {
                        if (xmla.Name == "ShapeIndex")
                        {
                            //已经处理了。
                        }
                        else if (xmla.Name == "DrawLine")
                        {
                            bool b = bool.Parse(sProp.GetAttribute("DrawLine"));
                            Program.frmMain.MapMain.set_ShapeDrawLine(handle, ix, b);
                        }
                        else if (xmla.Name == "LineColor")
                        {
                            System.Drawing.Color sc = System.Drawing.Color.FromArgb(int.Parse(sProp.GetAttribute("LineColor")));
                            Program.frmMain.MapMain.set_ShapeLineColor(handle, ix, (uint)System.Drawing.ColorTranslator.ToOle(sc));
                        }
                        else if (xmla.Name == "LineWidth")
                        {
                            int sL = int.Parse(sProp.GetAttribute("LineWidth"));
                            Program.frmMain.MapMain.set_ShapeLineWidth(handle, ix, sL);
                        }
                        else if (xmla.Name == "LineStyle")
                        {
                            int i = int.Parse(sProp.GetAttribute("LineStyle"));
                            MapWinGIS.tkLineStipple lineStipple = (MapWinGIS.tkLineStipple)i;
                            Program.frmMain.MapMain.set_ShapeLineStipple(handle, ix, lineStipple);
                        }
                        else if (xmla.Name == "DrawPoint")
                        {
                            bool b = bool.Parse((string)(sProp.GetAttribute("DrawPoint")));
                            Program.frmMain.MapMain.set_ShapeDrawPoint(handle, ix, b);
                        }
                        else if (xmla.Name == "PointColor")
                        {
                            System.Drawing.Color sc = System.Drawing.Color.FromArgb(int.Parse(sProp.GetAttribute("PointColor")));
                            Program.frmMain.MapMain.set_ShapePointColor(handle, ix, (uint)System.Drawing.ColorTranslator.ToOle(sc));
                        }
                        else if (xmla.Name == "PointSize")
                        {
                            int sP = int.Parse(sProp.GetAttribute("PointSize"));
                            Program.frmMain.MapMain.set_ShapePointSize(handle, ix, sP);
                        }
                        else if (xmla.Name == "DrawFill")
                        {
                            bool b = bool.Parse(sProp.GetAttribute("DrawFill"));
                            Program.frmMain.MapMain.set_ShapeDrawFill(handle, ix, b);
                        }
                        else if (xmla.Name == "FillColor")
                        {
                            System.Drawing.Color sc = System.Drawing.Color.FromArgb(int.Parse(sProp.GetAttribute("FillColor")));
                            Program.frmMain.MapMain.set_ShapeFillColor(handle, ix, (uint)System.Drawing.ColorTranslator.ToOle(sc));
                        }
                        else if (xmla.Name == "FillTransparency")
                        {
                            float s = float.Parse(sProp.GetAttribute("FillTransparency"));
                            Program.frmMain.MapMain.set_ShapeFillTransparency(handle, ix, s);
                        }
                        else if (xmla.Name == "FillStyle")
                        {
                            int i = int.Parse(sProp.GetAttribute("FillStyle"));
                            Program.frmMain.MapMain.set_ShapeFillStipple(handle, ix, (MapWinGIS.tkFillStipple)i);
                        }
                        else
                        {
                            
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                m_ErrorMsg += "在LoadShapePropertiesList()出现错误: " + e.Message + "\r\n";
                m_ErrorOccured = true;
            }
        }



    }
}
