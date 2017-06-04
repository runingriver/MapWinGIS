/****************************************************************************
 * 文件名:clsXmlProjectFile.cs
 * 描  述:
 * **************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Xml;
using System.Drawing;
using System.Diagnostics;

namespace MapWinGIS.MainProgram
{
    partial class XmlProjectFile
    {
        private System.Resources.ResourceManager resources =
            new System.Resources.ResourceManager("MapWinGIS.MainProgram.GlobalResource", System.Reflection.Assembly.GetExecutingAssembly());

        //私有变量
        private XmlDocument p_Doc= new XmlDocument();
        private bool m_ErrorOccured;
        private string m_ErrorMsg = "产生一个错误:" + "\r\r";
        private bool m_ForcedModified = false;
        // 防止多次加载图层属性。
        private bool LoopPrevention = false;
        internal int g_ViewBackColor;
        internal XmlElement g_ColorPalettes;

        //公共变量
        /// <summary>
        /// 包含完整路径的配置文件名
        /// </summary>
        public string ConfigFileName = "";
        /// <summary>
        /// 包含完整路径的项目文件名
        /// </summary>
        public string ProjectFileName = "";
        /// <summary>
        /// 配置文件是否加载
        /// false-没加载，true-已加载
        /// </summary>
        public bool ConfigLoaded;
        /// <summary>
        /// 项目是否被修改
        /// false-未修改，true-已修改
        /// </summary>
        public bool Modified;
        /// <summary>
        /// false- 显示ErrorDialo，true - 显示frmErrorDialog-NoSend
        /// </summary>
        public bool NoPromptToSendErrors = false;
        /// <summary>
        /// 存储的是一个BookmarkedVew对象
        /// </summary>
        public ArrayList BookmarkedViews = new ArrayList();
        /// <summary>
        /// 存储Menu和toolbar上的全部对象(菜单项和工具条按钮)
        /// </summary>
        public static Hashtable m_MainToolbarButtons = new Hashtable(); //该集合尚未使用。
        /// <summary>
        /// 存储近期项目的路径，包含文件名的完整路径
        /// </summary>
        public ArrayList RecentProjects = new ArrayList();

        public string m_MapUnits; //Meters, Feet, etc
        public bool ShowStatusBarCoords_Projected = true; //默认true
        public string ShowStatusBarCoords_Alternate = "(None)";
        public int StatusBarCoordsNumDecimals = 3;
        public int StatusBarAlternateCoordsNumDecimals = 3;
        public bool StatusBarCoordsUseCommas = true;
        public bool StatusBarAlternateCoordsUseCommas = true;
        public bool SaveShapeSettings = false; 
        public bool TransparentSelection = true;
        public Color ProjectBackColor = Color.White; //地图的背景色
        public bool UseDefaultBackColor = true; 
        
        public MapWinGIS.GeoProjection GeoProjection = new MapWinGIS.GeoProjection();

        //项目投影输入输出格式
        public string ProjectProjection
        {
            get
            {
                return GeoProjection.ExportToProj4();
            }
            set
            {
                if (!GeoProjection.ImportFromProj4(value))
                {
                    GeoProjection.ImportFromWKT(value);
                }
            }
        }
        public string ProjectProjectionWKT
        {
            get
            {
                return GeoProjection.ExportToWKT();
            }
            set
            {
                GeoProjection.ImportFromWKT(value);
            }
        }

        #region 书签（BookMark）类

        public class BookmarkedView
        {
            public string v_Name;
            public MapWinGIS.Extents v_Exts;

            public BookmarkedView(string bookmarkName, MapWinGIS.Extents bookmarkExts)
            {
                v_Name = bookmarkName;
                v_Exts = bookmarkExts;
            }

            public BookmarkedView(string bookmarkName, double xMin, double yMin, double xMax, double yMax)
            {
                v_Name = bookmarkName;
                v_Exts = new MapWinGIS.Extents();
                v_Exts.SetBounds(xMin, yMin, 0, xMax, yMax, 0);
            }

            /// <summary>
            /// 书签名
            /// </summary>
            public string Name
            {
                get
                {
                    return v_Name;
                }
                set
                {
                    v_Name = value;
                }
            }

            /// <summary>
            /// 书签（记录图层在MapMain中的信息）
            /// </summary>
            public MapWinGIS.Extents Exts
            {
                get
                {
                    return v_Exts;
                }
                set
                {
                    v_Exts = value;
                }
            }

            /// <summary>
            /// 自定义字符串序列
            /// </summary>
            public override string ToString()
            {
                return (Name + " (" + (Exts.xMin.ToString() + ", " + Exts.xMax.ToString() + "), (" + Exts.yMin.ToString() + ", " + Exts.yMax.ToString() + ")"));
            }
        }

        #endregion
        
        #region 配置文件路径
        /// <summary>
        /// 默认配置文件的完整路径，也就是保存在程序执行目录bin下的配置文件
        /// eg:D:\MapWinGIS\MapWinGIS\bin\default.mwcfg
        /// </summary>
        public string DefaultConfigFile
        {
            get
            {
                return System.IO.Path.Combine(Program.binFolder, "default.mwgcfg");
            }
        }
        /// <summary>
        /// 用户配置文件的完整路径，也就是存储在应用程序目录下的保存用户设置的文件
        /// 一般用户配置文件是放置..\..\Application Data\MapWinGIS\下面
        /// eg;D:\MapWinGIS\MapWinGIS\bin\Config\MapWinGIS.mwcfg
        /// </summary>
        public string UserConfigFile
        {
            get
            {
                return System.IO.Path.Combine(GetApplicationDataDir(), "MapWinGIS.mwgcfg");
            }
        }

        /// <summary>
        /// 应用程序数据保存的目录路径
        /// 注：保存配置文件到..\Config\目录中，若没有该目录就创建一个
        /// 一般用户配置文件是放置..\..\Application Data\MapWinGIS\下面
        /// 目前将配置文件保存在可执行文件目录的子目录下(..\bin\Config\)
        /// </summary>
        public static string GetApplicationDataDir()
        {
            //eg;D:\MapWinGIS\MapWinGIS\MapWinGIS.exe-13 = ;D:\MapWinGIS\MapWinGIS\
            string appDataDir = Application.StartupPath + "\\Config";
            
            try
            {
                if (!System.IO.Directory.Exists(appDataDir))
                {
                    MapWinGIS.Utility.Logger.Dbg("创建 Application Data 目录: " + appDataDir);
                    System.IO.Directory.CreateDirectory(appDataDir);
                }
            }
            catch (System.IO.IOException e)
            {
                MapWinGIS.Utility.Logger.Dbg("保存配置 - MapWinGIS的Application Data 目录: 异常: " + e.ToString());
            }
            catch
            {
            }
            return appDataDir;
        }
        #endregion

        #region 保存项目文件
        /// <summary>
        /// 未实现
        /// </summary>
        public bool SaveProject()
        {
            if (Program.frmMain.MapMain.ShapeDrawingMethod == tkShapeDrawingMethod.dmNewSymbology)
            {
                return SaveProjectNew();
            }
            if (ProjectFileName.Length == 0)
            {
                return false;
            }
            XmlElement node;
            string ver;
            XmlAttribute configPath;
            try
            {
                ver = App.VersionString;
                //在".mwgprj"项目文件中添加如下内容
                p_Doc = new XmlDocument();
                string prjName = Program.frmMain.Text.Replace("\'", "");
                string type = "projectfile." + (Program.frmMain.MapMain.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology ? 1 : 0).ToString();
                p_Doc.LoadXml("<MapwinGIS name='" + System.Web.HttpUtility.UrlEncode(prjName) + "' type='" + type + "' version='" + System.Web.HttpUtility.UrlEncode(ver) + "'></MapwinGIS>");
                node = p_Doc.DocumentElement;

                //添加配置文件路径
                configPath = p_Doc.CreateAttribute("ConfigurationPath");
                configPath.InnerText = GetRelativePath(ConfigFileName, ProjectFileName);
                node.Attributes.Append(configPath);
                //添加投影
                XmlAttribute projection = p_Doc.CreateAttribute("ProjectProjection");
                projection.InnerText = ProjectProjection;
                node.Attributes.Append(projection);
                XmlAttribute projWKT = p_Doc.CreateAttribute("ProjectProjectionWKT");
                projWKT.InnerText = ProjectProjectionWKT;
                node.Attributes.Append(projWKT);
                //添加地图单位
                XmlAttribute mapUint = p_Doc.CreateAttribute("MapUnits");
                mapUint.InnerText = Program.frmMain.m_Project.MapUnits;
                node.Attributes.Append(mapUint);
                //添加状态栏的信息
                XmlAttribute xStatusBarAlternateCoordsNumDecimals = p_Doc.CreateAttribute("StatusBarAlternateCoordsNumDecimals");
                xStatusBarAlternateCoordsNumDecimals.InnerText = StatusBarAlternateCoordsNumDecimals.ToString();
                node.Attributes.Append(xStatusBarAlternateCoordsNumDecimals);
                XmlAttribute xStatusBarCoordsNumDecimals = p_Doc.CreateAttribute("StatusBarCoordsNumDecimals");
                xStatusBarCoordsNumDecimals.InnerText = StatusBarCoordsNumDecimals.ToString();
                node.Attributes.Append(xStatusBarCoordsNumDecimals);
                XmlAttribute xStatusBarAlternateCoordsUseCommas = p_Doc.CreateAttribute("StatusBarAlternateCoordsUseCommas");
                xStatusBarAlternateCoordsUseCommas.InnerText = StatusBarAlternateCoordsUseCommas.ToString();
                node.Attributes.Append(xStatusBarAlternateCoordsUseCommas);
                XmlAttribute xStatusBarCoordsUseCommas = p_Doc.CreateAttribute("StatusBarCoordsUseCommas");
                xStatusBarCoordsUseCommas.InnerText = StatusBarCoordsUseCommas.ToString();
                node.Attributes.Append(xStatusBarCoordsUseCommas);
                //添加浮动比例尺信息
                //是否显示ScaleBar
                XmlAttribute ShowFloatingScaleBar = p_Doc.CreateAttribute("ShowFloatingScaleBar");
                ShowFloatingScaleBar.InnerText = Convert.ToString(Program.frmMain.m_FloatingScalebar_Enabled);
                node.Attributes.Append(ShowFloatingScaleBar);
                //ScaleBar的位置
                XmlAttribute FloatingScaleBarPosition = p_Doc.CreateAttribute("FloatingScaleBarPosition");
                FloatingScaleBarPosition.InnerText = Program.frmMain.m_FloatingScalebar_ContextMenu_SelectedPosition;
                node.Attributes.Append(FloatingScaleBarPosition);
                //ScaleBar的单位
                XmlAttribute FloatingScaleBarUnit = p_Doc.CreateAttribute("FloatingScaleBarUnit");
                FloatingScaleBarUnit.InnerText = Program.frmMain.m_FloatingScalebar_ContextMenu_SelectedUnit;
                node.Attributes.Append(FloatingScaleBarUnit);
                //ScaleBar字体
                System.Drawing.Color fColor = Program.frmMain.m_FloatingScalebar_ContextMenu_ForeColor;
                XmlAttribute FloatingScaleBarForecolor = p_Doc.CreateAttribute("FloatingScaleBarForecolor");
                FloatingScaleBarForecolor.InnerText = Convert.ToString(fColor.ToArgb());
                node.Attributes.Append(FloatingScaleBarForecolor);
                //ScaleBar背景色
                System.Drawing.Color bColor = Program.frmMain.m_FloatingScalebar_ContextMenu_BackColor;
                XmlAttribute FloatingScaleBarBackcolor = p_Doc.CreateAttribute("FloatingScaleBarBackcolor");
                FloatingScaleBarBackcolor.InnerText = Convert.ToString(bColor.ToArgb());
                node.Attributes.Append(FloatingScaleBarBackcolor);

                //添加地图的MapResizeBehavior属性
                XmlAttribute resizebehavior = p_Doc.CreateAttribute("MapResizeBehavior");
                resizebehavior.InnerText = ((short)Program.frmMain.MapMain.MapResizeBehavior).ToString();
                node.Attributes.Append(resizebehavior);

                //添加是否在状态栏显示多个坐标系
                XmlAttribute coord_projected = p_Doc.CreateAttribute("ShowStatusBarCoords_Projected");
                coord_projected.InnerText = ShowStatusBarCoords_Projected.ToString();
                node.Attributes.Append(coord_projected);
                XmlAttribute coord_alternate = p_Doc.CreateAttribute("ShowStatusBarCoords_Alternate");
                coord_alternate.InnerText = ShowStatusBarCoords_Alternate;
                node.Attributes.Append(coord_alternate);

                //添加保存Shape设置
                XmlAttribute saveshapesettinfgsbehavior = p_Doc.CreateAttribute("SaveShapeSettings");
                saveshapesettinfgsbehavior.InnerText = this.SaveShapeSettings.ToString();
                node.Attributes.Append(saveshapesettinfgsbehavior);

                //添加地图背景色
                XmlAttribute backColor_useDefault = p_Doc.CreateAttribute("ViewBackColor_UseDefault");
                backColor_useDefault.InnerText = UseDefaultBackColor.ToString();
                node.Attributes.Append(backColor_useDefault);
                XmlAttribute backColor = p_Doc.CreateAttribute("ViewBackColor");
                backColor.InnerText = ColorScheme.ColorToInt(ProjectBackColor).ToString();
                node.Attributes.Append(backColor);

                //将本项目添加到近期项目中
                AddToRecentProjects(ProjectFileName);

                //添加该项目保存时存在的外部插件
                AddPluginsElement(p_Doc, node, false);

                //添加内部插件
                AddApplicationPluginsElement(p_Doc, node, false);

                //添加地图的范围
                AddExtentsElement(p_Doc, node, (MapWinGIS.Extents)Program.frmMain.MapMain.Extents);

                //添加图层
                AddLayers(p_Doc, node);

                //添加书签
                AddBookmarks(p_Doc, node);

                //添加预览地图的属性
                AddPreViewMapElement(p_Doc, node);

                //保存项目文件
                MapWinGIS.Utility.Logger.Dbg("保存项目到: " + ProjectFileName);
                try
                {
                    p_Doc.Save(ProjectFileName);
                    Program.frmMain.SetModified(false);
                    return true;
                }
                catch (System.UnauthorizedAccessException) //无权访问或I/O错误
                {
                    bool ro = false;
                    if (File.Exists(ProjectFileName))
                    {
                        FileInfo fileInfo = new FileInfo(ProjectFileName);
                        if (fileInfo.IsReadOnly)
                        {
                            ro = true;
                        }
                    }
                    if (ro) //没有写权限
                    {
                        string msg = string.Format(resources.GetString("msgProjectReadOnly_Text"), ProjectFileName);
                        MapWinGIS.Utility.Logger.Msg(msg, resources.GetString("msgProjectReadOnly_Title"), MessageBoxIcon.Exclamation);
                    }
                    else //权限不够
                    {
                        string msg = string.Format(resources.GetString("msgProjectInsufficientAccess_Text"), ProjectFileName);
                        MapWinGIS.Utility.Logger.Msg(msg, resources.GetString("msgProjectInsufficientAccess_Title"), MessageBoxIcon.Exclamation);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// 将该项目添加到"近期项目"
        /// </summary>
        private void AddToRecentProjects(string projectFileName)
        {
            try
            {
                if (!Program.frmMain.m_Menu.m_MenuTable.Contains("mnuRecentProjects"))
                {
                    return;
                }

                //移除所有与该项目同名的近期项目
                string newNameLower = projectFileName.ToLower();
                int iRecent = RecentProjects.Count;
                while (iRecent > 0)
                {
                    if (RecentProjects[iRecent].ToString().ToLower() == newNameLower)
                    {
                        RecentProjects.RemoveAt(iRecent);
                    }
                    iRecent--;
                }

                //将该项目添加到第一个
                RecentProjects.Insert(0, projectFileName);

                //确保近期项目菜单不超过10个
                if (RecentProjects.Count > 10)
                {
                    RecentProjects.RemoveAt(RecentProjects.Count - 1);
                }
                Program.frmMain.BuildRecentProjectsMenu();
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
            }
        }

        /// <summary>
        /// 将地图的Extents添加到项目文件中
        /// </summary>
        private void AddExtentsElement(XmlDocument m_Doc, XmlElement node, MapWinGIS.Extents Exts)
        {
            XmlElement extents = m_Doc.CreateElement("Extents");
            XmlAttribute xMax = m_Doc.CreateAttribute("xMax");
            XmlAttribute yMax = m_Doc.CreateAttribute("yMax");
            XmlAttribute xMin = m_Doc.CreateAttribute("xMin");
            XmlAttribute yMin = m_Doc.CreateAttribute("yMin");

            xMax.InnerText = Exts.xMax.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yMax.InnerText = Exts.yMax.ToString(System.Globalization.CultureInfo.InvariantCulture);
            xMin.InnerText = Exts.xMin.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yMin.InnerText = Exts.yMin.ToString(System.Globalization.CultureInfo.InvariantCulture);

            extents.Attributes.Append(xMax);
            extents.Attributes.Append(yMax);
            extents.Attributes.Append(xMin);
            extents.Attributes.Append(yMin);

            node.AppendChild(extents);
        }

        /// <summary>
        /// 将Legend中的组(Group)和层(LegendControl.Layer)的信息添加到项目配置文件
        /// </summary>
        private void AddLayers(XmlDocument m_Doc, XmlElement parent)
        {
            //添加当前图层的信息到项目文件
            XmlElement Groups = m_Doc.CreateElement("Groups");
            XmlElement Layers;
            XmlElement Group;

            XmlAttribute Name;
            XmlAttribute Expanded;
            XmlAttribute Position;
            XmlAttribute LayerPos = m_Doc.CreateAttribute("Position");
            int NumGroups;
            int numLayers;
            int lHandle;
            int g;
            int l;

            //添加所有的组及下面的图层
            NumGroups = Program.frmMain.Legend.Groups.Count;
            for (g = 0; g < NumGroups; g++)
            {
                //保存组的信息
                Group = m_Doc.CreateElement("Group");
                Name = m_Doc.CreateAttribute("Name");
                Expanded = m_Doc.CreateAttribute("Expanded");
                Position = m_Doc.CreateAttribute("Position");

                Name.InnerText = Program.frmMain.Legend.Groups[g].Text;
                Expanded.InnerText = Program.frmMain.Legend.Groups[g].Expanded.ToString();
                Position.InnerText = g.ToString();
                Group.Attributes.Append(Name);
                Group.Attributes.Append(Expanded);
                Group.Attributes.Append(Position);
                SaveImage(m_Doc, Program.frmMain.Legend.Groups[g].Icon, Group);

                //组下面的图层
                numLayers = Program.frmMain.Legend.Groups[g].LayerCount;
                if (numLayers > 0)
                {
                    Layers = m_Doc.CreateElement("Layers");
                    for (l = 0; l < numLayers; l++)
                    {
                        if (!((LegendControl.Layer)(((LegendControl.Groups)Program.frmMain.Legend.Groups[g])[l])).SkipOverDuringSave)
                        {
                            lHandle = ((LegendControl.Layer)(((LegendControl.Groups)Program.frmMain.Legend.Groups[g])[l])).Handle;

                            if (Program.frmMain.MapMain.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
                            {
                                AddLayerElement(m_Doc, Program.frmMain.Layers[lHandle], Layers);
                            }
                            else
                            {
                                XmlLayerFile xmlLayerFile = new XmlLayerFile();
                                XmlElement xelLayer = xmlLayerFile.Layer2XML(Program.frmMain.Layers[lHandle], m_Doc, ProjectFileName);
                                Layers.AppendChild(xelLayer);
                            }
                        }
                    }
                    Group.AppendChild(Layers);
                }
                Groups.AppendChild(Group);
            }
            parent.AppendChild(Groups);
        }

        /// <summary>
        /// 添加图层(Interfaces.Layer)信息到对应节点
        /// </summary>
        internal void AddLayerElement(XmlDocument m_doc, Interfaces.Layer mapWinLayer, XmlNode parent)
        {
            XmlElement layer = m_doc.CreateElement("Layer");
            XmlAttribute name = m_doc.CreateAttribute("Name");
            XmlAttribute groupname = m_doc.CreateAttribute("GroupName");
            XmlAttribute type = m_doc.CreateAttribute("type");
            XmlAttribute path = m_doc.CreateAttribute("Path");
            XmlAttribute tag = m_doc.CreateAttribute("Tag");
            XmlAttribute legPic = m_doc.CreateAttribute("LegendPicture");
            XmlAttribute visible = m_doc.CreateAttribute("visible");
            XmlAttribute labelsVisible = m_doc.CreateAttribute("LabelsVisible");
            XmlAttribute expanded = m_doc.CreateAttribute("Expanded");

            //设置属性
            name.InnerText = mapWinLayer.Name;
            groupname.InnerText = Program.frmMain.m_Layers.Groups.ItemByHandle(mapWinLayer.GroupHandle).Text;
            type.InnerText = ((int)mapWinLayer.LayerType).ToString();
            tag.InnerText = mapWinLayer.Tag;
            visible.InnerText = mapWinLayer.Visible.ToString();
            labelsVisible.InnerText = mapWinLayer.LabelsVisible.ToString();
            expanded.InnerText = mapWinLayer.Expanded.ToString();
            SaveImage(m_doc, mapWinLayer.Icon, layer);

            //存储层文件的路径
            string fileName = mapWinLayer.FileName;
            if (fileName.Length != 0)
            {
                path.InnerText = GetRelativePath(fileName, ProjectFileName);
            }
            else
            {
                path.InnerText = GetRelativePath(mapWinLayer.FileName, ProjectFileName);
            }

            //将属性添加到节点
            layer.Attributes.Append(name);
            layer.Attributes.Append(groupname);
            layer.Attributes.Append(type);
            layer.Attributes.Append(path);
            layer.Attributes.Append(tag);
            layer.Attributes.Append(legPic);
            layer.Attributes.Append(visible);
            layer.Attributes.Append(labelsVisible);
            layer.Attributes.Append(expanded);

            if (mapWinLayer.GetObject() is MapWinGIS.IShapefile)
            {
                //如果是Shapefile，则添加shapefile节点
                AddShapeFileElement(m_doc, mapWinLayer, layer);
            }
            else if ((mapWinLayer.GetObject()) is MapWinGIS.IImage || mapWinLayer.GetObject() is MapWinGIS.Grid)
            {
                //添加grid节点
                AddGridElement(m_doc, mapWinLayer, layer);
            }

            //添加DynamicVisibility选项
            AddDynamicVisibility(m_doc, mapWinLayer, layer);

            parent.AppendChild(layer);
        }

        /// <summary>
        /// 添加动态可见性节点到项目文件的指定节点下面
        /// </summary>
        private void AddDynamicVisibility(XmlDocument m_Doc, Interfaces.Layer mapWinLayer, XmlNode parent)
        {
            XmlElement dynamicVisibility = m_Doc.CreateElement("DynamicVisibility");
            XmlAttribute useDynamicVisibility = m_Doc.CreateAttribute("UseDynamicVisibility");
            XmlAttribute scale = m_Doc.CreateAttribute("Scale");

            //设置xml元素属性值
            if (!(Program.frmMain.m_AutoVis[mapWinLayer.Handle] == null))
            {
                useDynamicVisibility.InnerText = Program.frmMain.m_AutoVis[mapWinLayer.Handle].UseDynamicExtents.ToString();
                scale.InnerText = Program.frmMain.m_AutoVis[mapWinLayer.Handle].DynamicScale.ToString();
            }
            else
            {
                useDynamicVisibility.InnerText = "false";
                scale.InnerText = "0";
            }

            dynamicVisibility.Attributes.Append(useDynamicVisibility);
            dynamicVisibility.Attributes.Append(scale);

            parent.AppendChild(dynamicVisibility);
        }

        /// <summary>
        /// 将该项目的书签添加到项目文件,书签是保存地图的Extents的
        /// </summary>
        private void AddBookmarks(XmlDocument m_Doc, XmlElement parent)
        {
            XmlElement bookmarksElem = m_Doc.CreateElement("Bookmarks");

            for (int i = 0; i < BookmarkedViews.Count; i++)
            {
                XmlElement bm = m_Doc.CreateElement("Bookmark");
                XmlAttribute attr = m_Doc.CreateAttribute("Name");
                attr.InnerText = ((BookmarkedView)BookmarkedViews[i]).Name;
                bm.Attributes.Append(attr);
                AddExtentsElement(m_Doc, bm, ((BookmarkedView)BookmarkedViews[i]).Exts);

                bookmarksElem.AppendChild(bm);
            }

            parent.AppendChild(bookmarksElem);
        }

        /// <summary>
        /// 添加当前项目预览地图的信息到项目文件
        /// </summary>
        private void AddPreViewMapElement(XmlDocument m_Doc, XmlElement parent)
        {
            XmlElement prevMap = m_Doc.CreateElement("PreviewMap");
            XmlAttribute visible = m_Doc.CreateAttribute("visible");
            XmlAttribute dx = m_Doc.CreateAttribute("dx");
            XmlAttribute dy = m_Doc.CreateAttribute("dy");
            XmlAttribute xllcenter = m_Doc.CreateAttribute("xllcenter");
            XmlAttribute yllcenter = m_Doc.CreateAttribute("yllcenter");

            if (Program.frmMain.MapPreview.NumLayers > 0)
            {
                MapWinGIS.Image img = (MapWinGIS.Image)(Program.frmMain.MapPreview.get_GetObject(0));
                dx.InnerText = img.dX.ToString();
                dy.InnerText = img.dY.ToString();
                xllcenter.InnerText = img.XllCenter.ToString();
                yllcenter.InnerText = img.YllCenter.ToString();
            }
            else
            {
                dx.InnerText = "0";
                dy.InnerText = "0";
                xllcenter.InnerText = "0";
                yllcenter.InnerText = "0";
            }

            prevMap.Attributes.Append(dx);
            prevMap.Attributes.Append(dy);
            prevMap.Attributes.Append(xllcenter);
            prevMap.Attributes.Append(yllcenter);

            SaveImage(m_Doc, Program.frmMain.m_PreviewMap.Picture, prevMap);

            parent.AppendChild(prevMap);
        }

        #endregion

        #region 加载项目文件 

        public bool LoadProject(string fileName, bool layersOnly = false, string layersIntoGroup = "")
        {
            return false;
        }

        private bool LoadMW4Settings(XmlElement node, bool LayersOnly)
        { return false; }

        private void LoadExtents(XmlElement ext)
        { }

        private bool LoadPreviewMap(XmlElement previewMap)
        { return false; }

        public void RestorePreviewMap(ref MapWinGIS.Image image)
        { }

        private void LoadBookmarks(XmlElement view)
        { }

        private void LoadGroups(XmlElement groups, string projectVersion, bool layersOnly = false, string layersIntoGroup = "")
        { }

        /// <summary>
        /// 加载图层属性(mwsr文件)，旧式的项目加载方法。
        /// </summary>
        internal void LoadLayerProperties(XmlNode layer, int existingLayerHandle = -1, bool pluginCall = false)
        {
            if (LoopPrevention) return;
            try
            {
                string filePath = layer.Attributes["Path"].InnerText;
                string name = layer.Attributes["Name"].InnerText;
                string groupname = "";
                try
                {
                    if (layer.Attributes["GroupName"] != null)
                    {
                        groupname = layer.Attributes["GroupName"].InnerText;
                    }
                }
                catch { }

                bool layerVisible = System.Convert.ToBoolean(layer.Attributes["Visible"].InnerText);
                int type = System.Convert.ToInt32(layer.Attributes["Type"].InnerText);
                bool expanded = System.Convert.ToBoolean(layer.Attributes["Expanded"].InnerText);
                string tag = layer.Attributes["Tag"].InnerText;
                bool labelsVisible = System.Convert.ToBoolean(layer.Attributes["LabelsVisible"].InnerText);
                int handle = -1;
                string imageType;
                //MapWinGIS.GridColorScheme gridScheme = null;

                //显示进度条
                MapWinGIS.Utility.Logger.Status("正在加载 " + name + "...");

                if (type == (int)Interfaces.eLayerType.LineShapefile || type == (int)Interfaces.eLayerType.PointShapefile || type == (int)Interfaces.eLayerType.PolygonShapefile)
                { 
                    //添加shapefile类型的图层属性
                    int color = Convert.ToInt32(layer["ShapeFileProperties"].Attributes["Color"].InnerText);
                    int outlineColor = Convert.ToInt32(layer["ShapeFileProperties"].Attributes["OutLineColor"].InnerText);
                    bool drawFill = Convert.ToBoolean(layer["ShapeFileProperties"].Attributes["DrawFill"].InnerText);
                    float lineOrPointSize = Convert.ToSingle(layer["ShapeFileProperties"].Attributes["LineOrPointSize"].InnerText);
                    MapWinGIS.tkPointType pointType = (MapWinGIS.tkPointType)Enum.Parse(typeof(MapWinGIS.tkPointType), layer["ShapeFileProperties"].Attributes["PointType"].InnerText);
                    MapWinGIS.tkLineStipple lineStipple = (MapWinGIS.tkLineStipple)Enum.Parse(typeof(MapWinGIS.tkLineStipple), layer["ShapeFileProperties"].Attributes["LineStipple"].InnerText);
                    MapWinGIS.tkFillStipple fillStipple = (MapWinGIS.tkFillStipple)Enum.Parse(typeof(MapWinGIS.tkFillStipple), layer["ShapeFileProperties"].Attributes["FillStipple"].InnerText);

                    System.Drawing.Color fillStippleLineColor = System.Drawing.Color.Black;
                    if (layer["ShapeFileProperties"].Attributes["FillStippleLineColor"] != null)
                    {
                        fillStippleLineColor = System.Drawing.Color.FromArgb(int.Parse(layer["ShapeFileProperties"].Attributes["FillStippleLineColor"].InnerText));
                    }
                    bool fillStippleTransparent = true;
                    if (layer["ShapeFileProperties"].Attributes["FillStippleTransparent"] != null)
                    {
                        fillStippleTransparent = bool.Parse(layer["ShapeFileProperties"].Attributes["FillStippleTransparent"].InnerText);
                    }

                    float transPercent = 1;
                    try
                    {
                        transPercent = System.Convert.ToSingle(layer["ShapeFileProperties"].Attributes["TransparencyPercent"].InnerText);
                    }
                    catch { }

                    MapWinGIS.Image userPointType = new MapWinGIS.Image();
                    if (layer["ShapeFileProperties"]["CustomPointType"]["Image"].InnerText != null && layer["ShapeFileProperties"]["CustomPointType"]["Image"].Attributes["Type"].InnerText != null)
                    {
                        userPointType.Picture = (stdole.IPictureDisp)(ConvertStringToImage(layer["ShapeFileProperties"]["CustomPointType"]["Image"].InnerText, layer["ShapeFileProperties"]["CustomPointType"]["Image"].Attributes["Type"].InnerText));
                    }
                    if (userPointType != null)
                    {
                        if (layer["ShapeFileProperties"].Attributes["UseTransparency"].InnerText == "")
                        {
                            userPointType.UseTransparencyColor = false;
                        }
                        else
                        {
                            userPointType.UseTransparencyColor = System.Convert.ToBoolean(layer["ShapeFileProperties"].Attributes["UseTransparency"].InnerText);
                        }

                        if (layer["ShapeFileProperties"].Attributes["TransparencyColor"].InnerText == "")
                        {
                            userPointType.TransparencyColor = 0;
                        }
                        else
                        {
                            userPointType.TransparencyColor = Convert.ToUInt32(layer["ShapeFileProperties"].Attributes["TransparencyColor"].InnerText);
                        }
                    }

                    //看是否需要移动图层
                    try
                    {
                        LoopPrevention = true;
                        if (existingLayerHandle == -1)
                        {
                            if (!File.Exists(filePath))
                            {
                                if (filePath.ToLower().Trim().StartsWith("ecwp://"))
                                {
                                    handle = Program.frmMain.m_Layers.AddLayer(objectOrFilename: filePath, layerName: name, layerVisible: layerVisible, color: color, outlineColor: outlineColor, drawFill: drawFill, lineOrPointSize: lineOrPointSize, pointType: pointType)[0].Handle;
                                }
                                else
                                {
                                    string cwd = Directory.GetCurrentDirectory();

                                    if (!PromptToBrowse(ref filePath, name))
                                    {
                                        this.m_ForcedModified = true;
                                        return;
                                    }

                                    handle = Program.frmMain.m_Layers.AddLayer(Path.GetFullPath(filePath), name, -1, layerVisible, color, outlineColor, drawFill, lineOrPointSize, pointType)[0].Handle;

                                    Directory.SetCurrentDirectory(cwd);
                                }
                            }
                            else //存在，直接添加
                            {
                                handle = System.Convert.ToInt32(Program.frmMain.m_Layers.AddLayer(System.IO.Path.GetFullPath(filePath), name, -1, layerVisible, color, outlineColor, drawFill, lineOrPointSize, pointType)[0].Handle);
                            }
                        }
                        else  //图层已经存在，不用添加
                        {
                            handle = existingLayerHandle;

                            //看是否需要移动该图层
                            int destGroup = -1;
                            for (int iz = 0; iz < Program.frmMain.m_Layers.Groups.Count; iz++)
                            {
                                if (Program.frmMain.m_Layers.Groups[iz].Text.ToLower().Trim() == groupname.ToLower().Trim() && groupname.Trim() != "")
                                {
                                    destGroup = Program.frmMain.m_Layers.Groups[iz].Handle;
                                    break;
                                }
                            }

                            //不是插件添加的图层，则将图层放置到指定地方，或新建一个组存放。
                            if (!pluginCall)
                            {
                                if (destGroup == -1)
                                {
                                    destGroup = Program.frmMain.m_Layers.Groups.Add(groupname.Trim(), 0);
                                }

                                Program.frmMain.m_Layers.MoveLayer(handle, 0, destGroup);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("LoadLayerProperties 错误: " + ex.ToString());
                    }
                    finally
                    {
                        LoopPrevention = false;
                    }

                    Program.frmMain.m_Layers[handle].Name = name;
                    Program.frmMain.m_Layers[handle].LineStipple = lineStipple;
                    Program.frmMain.m_Layers[handle].FillStipple = fillStipple;
                    Program.frmMain.m_Layers[handle].FillStippleLineColor = fillStippleLineColor;
                    Program.frmMain.m_Layers[handle].FillStippleTransparency = fillStippleTransparent;
                    Program.frmMain.m_Layers[handle].ShapeLayerFillTransparency = transPercent;
                    Program.frmMain.m_Layers[handle].LineOrPointSize = lineOrPointSize;
                    Program.frmMain.m_Layers[handle].DrawFill = drawFill;
                    if (type == (int)Interfaces.eLayerType.PointShapefile)
                    {
                        //顶点永远可见
                        Program.frmMain.m_Layers[handle].VerticesVisible = true;
                    }
                    else
                    {
                        Program.frmMain.m_Layers[handle].VerticesVisible = false;
                        try
                        {
                            Program.frmMain.m_Layers[handle].VerticesVisible = bool.Parse(layer["ShapeFileProperties"].Attributes["VerticesVisible"].InnerText);
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        Program.frmMain.m_Layers[handle].LabelsVisible = bool.Parse(layer["ShapeFileProperties"].Attributes["LabelsVisible"].InnerText);
                    }
                    catch
                    {
                        Program.frmMain.m_Layers[handle].LabelsVisible = true;
                    }

                    try
                    {
                        int.TryParse(layer["ShapeFileProperties"].Attributes["MapTooltipField"].InnerText, out Program.frmMain.Legend.Layers.ItemByHandle(handle).MapTooltipFieldIndex);
                        bool.TryParse(layer["ShapeFileProperties"].Attributes["MapTooltipsEnabled"].InnerText, out Program.frmMain.Legend.Layers.ItemByHandle(handle).MapTooltipsEnabled);
                        Program.frmMain.UpdateMapToolTipsAtLeastOneLayer();
                    }
                    catch
                    {
                    }

                    try
                    {
                        Program.frmMain.m_Layers[handle].OutlineColor = System.Drawing.ColorTranslator.FromOle(outlineColor);
                        Program.frmMain.m_Layers[handle].Color = System.Drawing.ColorTranslator.FromOle(color);
                    }
                    catch
                    {
                    }

                    //加载颜色设置
                    if (layer["ShapeFileProperties"]["Legend"] != null)
                    {
                        LoadShpFileColoringScheme(layer["ShapeFileProperties"]["Legend"], handle);
                    }

                    //设置用户自定义的点图像
                    if (userPointType != null)
                    {
                        Program.frmMain.m_Layers[handle].UserPointType = userPointType;
                    }

                    Program.frmMain.m_Layers[handle].PointType = pointType;

                    DeserializePointImageScheme(handle, layer["ShapeFileProperties"]);

                    DeserializeFillStippleScheme(handle, layer["ShapeFileProperties"]);

                    if (layer["ShapeFileProperties"]["ShapePropertiesList"] != null)
                    {
                        LoadShapePropertiesList(layer["ShapeFileProperties"]["ShapePropertiesList"], handle);
                    }

                    if (existingLayerHandle == -1)
                    {
                        Program.frmMain.SaveShapeLayerProps(handle);
                    }

                }
                else if (type == (int)Interfaces.eLayerType.Image || type == (int)Interfaces.eLayerType.Grid)
                {
                    //添加image或grid类型的图层。
                    //待续。。。
                }
                else //既不是shapefile类型也不是image、grid类型
                {
                    if (!System.IO.File.Exists(filePath))
                    {
                        string cwd = Directory.GetCurrentDirectory();

                        if (!PromptToBrowse(ref filePath, name))
                        {
                            return;
                        }

                        handle = System.Convert.ToInt32(Program.frmMain.m_Layers.AddLayer(objectOrFilename: filePath, layerName: name, layerVisible: layerVisible)[0].Handle);

                        Directory.SetCurrentDirectory(cwd);
                    }
                    else
                    {
                        handle = Program.frmMain.m_Layers.AddLayer(objectOrFilename: filePath, layerName: name, layerVisible: layerVisible)[0].Handle;
                    }
                }

                //应用图层的属性
                Program.frmMain.m_Layers[handle].Expanded = expanded;
                Program.frmMain.m_Layers[handle].Tag = tag;
                Program.frmMain.m_Layers[handle].LabelsVisible = labelsVisible;

                //图层图片
                XmlNode layerImage = layer["Image"];
                imageType = "";
                if (layerImage != null)
                {
                    try
                    {
                        imageType = layerImage.Attributes["Type"].InnerText;
                        if (imageType.Length > 0)
                        {
                            Program.frmMain.m_Layers[handle].Icon = ConvertStringToImage(layerImage.InnerText, imageType);
                        }
                    }
                    catch { }
                }

                MapWinGIS.Utility.Logger.Status("");
                Program.frmMain.m_Layers[handle].Expanded = expanded;

                LoadDynamicVisibility(Program.frmMain.m_Layers[handle], layer["DynamicVisibility"]);
            }
            catch(Exception ex)
            {
                m_ErrorMsg += "在LoadLayerProperties()方法中出现错误: " + ex.Message + "\r\n";
                m_ErrorOccured = true;
            }
        }

        private void LoadDynamicVisibility(Interfaces.Layer mapWinLayer, XmlNode node)
        {
            try
            {
                bool useDynamicVisibility = false;
                double xMin = 0;
                double yMin = 0;
                double xMax = 0;
                double yMax = 0;

                if (node != null)
                {
                    useDynamicVisibility = System.Convert.ToBoolean(node.Attributes["UseDynamicVisibility"].InnerText);

                    if (node.Attributes["Scale"] != null)
                    {
                        MapWinGIS.Extents exts = new MapWinGIS.Extents();
                        exts.SetBounds(0, 0, 0, 0, 0, 0);
                        try
                        {
                            exts = Program.frmMain.ScaleToExtents(double.Parse(node.Attributes["Scale"].InnerText), (MapWinGIS.Extents)Program.frmMain.MapMain.Extents);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.ToString());
                        }
                        if (exts == null)
                        {
                            exts = new MapWinGIS.Extents();
                            exts.SetBounds(0, 0, 0, 0, 0, 0);
                        }
                        xMin = exts.xMin;
                        xMax = exts.xMax;
                        yMin = exts.yMin;
                        yMax = exts.yMax;
                    }
                    else
                    {
                        //Extents
                        xMin = Utility.MiscUtils.ParseDouble(node.Attributes["xMin"].InnerText, 0.0); 
                        yMin = Utility.MiscUtils.ParseDouble(node.Attributes["yMin"].InnerText, 0.0); 
                        xMax = Utility.MiscUtils.ParseDouble(node.Attributes["xMax"].InnerText, 0.0); 
                        yMax = Utility.MiscUtils.ParseDouble(node.Attributes["yMax"].InnerText, 0.0); 
                    }
                }

                MapWinGIS.Extents ex = new MapWinGIS.Extents();
                ex.SetBounds(xMin, yMin, 0, xMax, yMax, 0);

                if (!(xMin == 0 && yMin == 0 && xMax == 0 && yMax == 0))
                {
                    if (Program.frmMain.m_AutoVis.Contains(mapWinLayer.Handle))
                    {
                        Program.frmMain.m_AutoVis.Remove(mapWinLayer.Handle);
                    }

                    Program.frmMain.m_AutoVis.Add(mapWinLayer.Handle, ex, useDynamicVisibility);
                }

            }
            catch (System.Exception e)
            {
                m_ErrorMsg += "在LoadDynamicVisibility()方法中出现错误：" + e.Message + "\r\n";
                m_ErrorOccured = true;
            }
        }

        /// <summary>
        /// 加载存储在Xml中的内部插件
        /// </summary>
        /// <param name="plugins"></param>
        /// <param name="loadingConfig"></param>
        private void LoadApplicationPlugins(XmlElement plugins, bool loadingConfig)
        {
            if (plugins == null) { return; }
            //加载尚未加载的内部插件
            if (loadingConfig)
            {
                if (plugins.Attributes["PluginDir"].InnerText != "")
                {
                    Program.appInfo.ApplicationPluginDir = Path.GetFullPath(plugins.Attributes["PluginDir"].InnerText);
                }
                Program.frmMain.m_PluginManager.LoadApplicationPlugins(Program.appInfo.ApplicationPluginDir);
            }
            //加载节点以下的插件，并调用插件的ProjectLoading方法
            foreach (XmlNode pluginNode in plugins.ChildNodes)
            {
                try
                {
                    string pluginKey = pluginNode.Attributes["Key"].InnerText;
                    LoadPlugin(pluginKey);
                    Program.frmMain.m_PluginManager.ProjectLoading(pluginKey, ProjectFileName, pluginNode.Attributes["SettingsString"].InnerText);
                }
                catch (Exception ex)
                {
                    m_ErrorMsg += "在LoadApplicationPlugins方法中出现错误:" + ex.ToString();
                    m_ErrorOccured = true;
                }
            }


        }

        /// <summary>
        /// 将存储在用户配置文件中的插件加载到程序中
        /// </summary>
        /// <param name="plugins">存储在用户配置文件中的插件节点</param>
        /// <param name="loadingConfig">无用的参数</param>
        private void LoadPlugins(XmlElement plugins, bool loadingConfig)
        {
            //记录Xml中存储的插件，key - 插件Key, Value - SettingsString
            Dictionary<string, string> lPluginSettings = new Dictionary<string, string>();
            foreach (XmlNode pluginNode in plugins.ChildNodes) //将没有加载的插件加载，并记录Xml中存储的插件
            {
                string pluginKey = pluginNode.Attributes["Key"].InnerText;
                try
                {
                    LoadPlugin(pluginKey);
                    lPluginSettings.Add(pluginKey, pluginNode.Attributes["SettingsString"].InnerText);
                }
                catch (Exception ex)
                {
                    MapWinGIS.Utility.Logger.Dbg("LoadPlugins方法中出现异常: " + pluginKey + ": " + ex.ToString());
                }
            }

            //调用所有已经加载的插件的ProjectLoading方法,让插件响应程序加载或项目加载时的事件
            Dictionary<string, Interfaces.IPlugin> loadedPlugins = Program.frmMain.m_PluginManager.m_LoadedPlugins;
            foreach (string loop_Key in loadedPlugins.Keys)
            {
                if (lPluginSettings.ContainsKey(loop_Key)) //将项目加载的信息发送给存储在项目中的指定插件
                {
                    Program.frmMain.m_PluginManager.ProjectLoading(loop_Key, ProjectFileName, lPluginSettings[loop_Key]);
                }
                else
                {
                    Program.frmMain.m_PluginManager.ProjectLoading(loop_Key, ProjectFileName, "");
                }
            }

        }

        /// <summary>
        /// 如果插件没有加载，加载该插件
        /// </summary>
        /// <param name="pluginName">要加载的插件的Key</param>
        private void LoadPlugin(string pluginName)
        {
            if (Program.frmMain.m_PluginManager.PluginIsLoaded(pluginName))
            {
                MapWinGIS.Utility.Logger.Dbg("插件已经加载: " + pluginName);
            }
            else
            {
                Program.frmMain.m_PluginManager.StartPlugin(pluginName);
                MapWinGIS.Utility.Logger.Dbg("加载插件: " + pluginName);
            }

        }

        #endregion  14

        #region 通用（静态）方法
        /// <summary>
        /// 保存toolbar上、Menus上的全部对象
        /// </summary>
        public static void SaveMainToolbarButtons()
        {
            //存储工具条中的全部按钮对象
            DictionaryEntry item;
            IEnumerator enumerator = Program.frmMain.m_Toolbar.m_Buttons.GetEnumerator();
            while (enumerator.MoveNext())
            {
                item = (DictionaryEntry)(enumerator.Current);
                if (!(m_MainToolbarButtons.ContainsKey(item.Key)))
                {
                    m_MainToolbarButtons.Add(item.Key, item.Value);
                }
            }

            //存储菜单栏中的全部对象
            enumerator = Program.frmMain.m_Menu.m_MenuTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                item = (DictionaryEntry)(enumerator.Current);
                if (!(m_MainToolbarButtons.ContainsKey(item.Key)))
                {
                    m_MainToolbarButtons.Add(item.Key, item.Value);
                }
            }

        }

        /// <summary>
        /// 通过最近一次文件的修改时间来比较两个文件 
        /// </summary>
        /// <rreturns>0- 有文件不存在，1- file1是最新的，-1- file2是最新的</rreturns>
        public int CompareFilesByTime(string file1, string file2)
        {
            if (File.Exists(file1) && File.Exists(file2))
            {
                System.IO.FileInfo fi1 = new System.IO.FileInfo(file1);
                System.IO.FileInfo fi2 = new System.IO.FileInfo(file2);

                DateTime fi1ChangedTime = fi1.LastWriteTime;
                if (fi1.CreationTime > fi1ChangedTime)
                {
                    fi1ChangedTime = fi1.CreationTime;
                }

                DateTime fi2ChangedTime = fi2.LastWriteTime;
                if (fi2.CreationTime > fi2ChangedTime)
                {
                    fi2ChangedTime = fi2.CreationTime;
                }

                if (fi1ChangedTime > fi2ChangedTime)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            //找不到两个文件之一
            return 0;
        }

        /// <summary>
        /// 获取相对路径
        /// eg;参1 D:\MapWinGIS\MapWinGIS\bin\Help\mapwingis.chm
        ///    参2 D:\MapWinGIS\MapWinGIS\bin\Config\user.cfg
        ///    结果：..\Help\mapwingis.chm
        /// </summary>
        public static string GetRelativePath(string filename, string projectFile)
        {
            MapWinGIS.Utility.Logger.Dbg("转换相对路径开始\r\n filename: " + filename + " 对应路径：" + projectFile);

            if (filename.Length == 0 || projectFile.Length == 0)
            {
                return "";
            }

            string relativePath = "";
            string[] a;
            string[] b;
            int i = 0, j = 0, k = 0, offset = 0;

            try
            {
                //如果两个路径不在同一个分区则使用绝对路径
                if (Path.GetPathRoot(filename).ToLower() != Path.GetPathRoot(projectFile).ToLower())
                {
                    MapWinGIS.Utility.Logger.Dbg("不在同一个分区，使用绝对路径");
                    return filename;
                }
                DirectoryInfo dirinfo;
                a = new string[1];
                a[0] = filename;
                i = 0;
                do
                {
                    i++;
                    Array.Resize(ref a, i + 1);
                    try
                    {
                        dirinfo = Directory.GetParent(a[i - 1]);
                        if (dirinfo == null)//没有父目录，则为空
                        {
                            a[i] = "";
                        }
                        else
                        {
                            a[i] = dirinfo.FullName;
                        }
                    }
                    catch (Exception)
                    {
                        a[i] = "";
                    }
                } while ((a[i] != ""));

                b = new string[1];
                b[0] = projectFile;
                i = 0;
                do
                {
                    i++;
                    Array.Resize(ref b, i + 1);
                    try
                    {
                        dirinfo = Directory.GetParent(b[i - 1]);
                        if (dirinfo == null)
                        {
                            b[i] = "";
                        }
                        else
                        {
                            b[i] = dirinfo.FullName;
                        }
                    }
                    catch (Exception)
                    {
                        b[i] = "";
                    }
                } while (!(b[i] == ""));

                //匹配两个路径
                bool flag = false;
                for (i = 0; i < a.Length; i++)
                {
                    for (j = 0; j < b.Length; j++)
                    {
                        if (a[i] == b[j])
                        {
                            //匹配成功，则跳出
                            flag = true;
                            break;
                        }
                    }
                    if (flag){ break; }
                }

                //j表示几级目录匹配
                for (k = 1; k < j; k++)
                {
                    relativePath = relativePath + "..\\";
                }

                //判断"fileName"是否以"\"结尾
                if (a[i].EndsWith("\\"))
                {
                    offset = 0;
                }
                else
                {
                    offset = 1;
                }
                if (filename.Length < a[i].Length + offset)
                {
                    MapWinGIS.Utility.Logger.Dbg("无法转换成相对路径，使用绝对路径: " + filename);
                    relativePath = filename;
                }
                else
                {
                    relativePath = relativePath + filename.Substring(a[i].Length + offset);
                    MapWinGIS.Utility.Logger.Dbg("转换相对路径完成: " + relativePath);
                }
                               
            }
            catch (Exception e)
            {
                //StackTrace st = new StackTrace(e, true);
                //StackFrame sf = st.GetFrame(2);
                //string mFileName = sf.GetMethod().ReflectedType.ToString();
                //string mthod = sf.GetMethod().Name;
                MapWinGIS.Utility.Logger.Dbg("转换相对路径错误: " + e.ToString());
                return filename;
            }
            return relativePath;
        }

        /// <summary>
        /// 为主窗体在屏幕上显示寻找合适的位置
        /// </summary>
        public void FindSafeWindowLocation(ref int w, ref int h, ref System.Drawing.Point location)
        {
            int index, upperBound;
            int maxw = 0, maxh = 0;

            //获取存储多个显示器的数组
            System.Windows.Forms.Screen[] Screens = System.Windows.Forms.Screen.AllScreens;
            upperBound = Screens.GetUpperBound(0);

            //获得屏幕最大的长和宽
            for (index = 0; index <= upperBound; index++)
            {
                maxw = Math.Max(maxw, Screens[index].WorkingArea.Right);
                maxh = Math.Max(maxh, Screens[index].WorkingArea.Bottom);
            }

            if (location.X + w > maxw)
            {
                location.X = maxw - w;
            }
            location.X = Math.Max(0, location.X);
            if (location.Y + h > maxh)
            {
                location.Y = maxh - h;
            }
            location.Y = Math.Max(0, location.Y);
        }

        private void SaveImage(XmlDocument m_Doc, object img, XmlElement parent, string elementName = "Image")
        {
            XmlElement image = m_Doc.CreateElement("Image");
            XmlAttribute type = m_Doc.CreateAttribute("type");
            string typ = "";

            //将图片转换成字符串存储
            image.InnerText = ConvertImageToString(img, ref typ);
            type.InnerText = typ;

            image.Attributes.Append(type);
            parent.AppendChild(image);
        }

        public string ConvertImageToString(object img, ref string type)
        {
            string s = "";
            string path = Program.GetMWGTempFile();

            if (img != null)
            {
                try
                {
                    //查询图片的类型
                    if (img is Icon)
                    {
                        type = "Icon";
                        Icon image = (Icon)img;

                        //将该图片以流的形式写入临时文件
                        Stream outStream = File.OpenWrite(path);
                        image.Save(outStream);
                        outStream.Close();
                    }
                    else if (img is stdole.IPictureDisp)
                    {
                        type = "IPictureDisp";
                        MapWinGIS.Utility.ImageUtils cvter = new MapWinGIS.Utility.ImageUtils();
                        System.Drawing.Image image = new Bitmap(cvter.IPictureDispToImage(img));

                        image.Save(path);
                    }
                    else if (img is Bitmap)
                    {
                        type = "Bitmap";
                        System.Drawing.Image image = (Bitmap)img;

                        image.Save(path);
                    }
                    else if (img is MapWinGIS.Image)
                    {

                        type = "MapWinGIS.Image";
                        MapWinGIS.Utility.ImageUtils utils = new MapWinGIS.Utility.ImageUtils();
                        MapWinGIS.Image mwimg = (MapWinGIS.Image)img;
                        System.Drawing.Image image = new Bitmap(utils.IPictureDispToImage(mwimg.Picture));

                        image.Save(path);
                    }
                    else
                    {
                        type = "Unknown";
                        return "";
                    }

                    Stream inStream = File.OpenRead(path);
                    BinaryReader reader = new BinaryReader(inStream);

                    //将读入的字节转换成字符形式
                    long numbytes = reader.BaseStream.Length;
                    s = System.Convert.ToBase64String(reader.ReadBytes((int)numbytes));

                    reader.Close();

                    //删除临时文件
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Program.frmMain.ShowErrorDialog(e);
                }
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return s;
        }

        public static object ConvertStringToImage(string image, string type)
        {
            Icon icon;
            Bitmap bmp;
            byte[] mybyte;
            string path;
            Stream outStream;

            if (image.Length > 0)
            {
                try
                {
                    path = Program.GetMWGTempFile();

                    outStream = File.OpenWrite(path);

                    mybyte = System.Convert.FromBase64String(image);

                    outStream.Write(mybyte, 0, mybyte.Length);

                    outStream.Close();

                    switch (type)
                    {
                        case "Icon":
                            icon = new Icon(path);
                            return icon;
                        case "Bitmap":
                            bmp = new Bitmap(path);
                            return bmp;
                        case "IPictureDisp":
                            bmp = new Bitmap(path);
                            MapWinGIS.Utility.ImageUtils cvter = new MapWinGIS.Utility.ImageUtils();
                            return cvter.ImageToIPictureDisp(bmp);
                        case "MapWinGIS.Image":
                            bmp = new Bitmap(path);
                            MapWinGIS.Image img = new MapWinGIS.Image();
                            MapWinGIS.Utility.ImageUtils utils = new MapWinGIS.Utility.ImageUtils();
                            img.Picture = (stdole.IPictureDisp)(utils.ImageToIPictureDisp(bmp));
                            return img;
                    }

                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                }
            }

            return null;
        }

        /// <summary>
        /// 提示用户选择文件
        /// </summary>
        internal bool PromptToBrowse(ref string filePath, string displayName)
        {
            if ((DateTime.Now.ToOADate() - Program.frmMain.m_CancelledPromptToBrowse) * 1440 < 1) //1440分钟是一天
            {
                return false;
            }
            string msg = string.Format(resources.GetString("msgDropMissingLayer_Text"), filePath + displayName == "" ? "" : " (" + displayName + ")");
            string msgTitle = string.Format(resources.GetString("msgDropMissingLayer_Title"), displayName == "" ? "" : " - " + displayName);

            DialogResult rslt = Program.messageBox.Display(msg, msgTitle, Utility.MessageButton.YesNoCancel);
	
            switch(rslt)
            {
                case DialogResult.No: return false;
                case DialogResult.Cancel:
                    Program.frmMain.m_CancelledPromptToBrowse = DateTime.Now.ToOADate();
                    return false;
                case DialogResult.Yes:
                    OpenFileDialog cdlOpen = new OpenFileDialog();
                    if (Directory.Exists(Program.appInfo.DefaultDir))
                    {
                        cdlOpen.InitialDirectory = Program.appInfo.DefaultDir;
                    }
                    cdlOpen.FileName = "";
                    cdlOpen.Title = "选择图层";
                    cdlOpen.Filter = Program.frmMain.m_Layers.GetSupportedFormats();
                    cdlOpen.CheckFileExists = true;
                    cdlOpen.CheckPathExists = true;
                    cdlOpen.Multiselect = false;
                    cdlOpen.ShowReadOnly = false;

                    cdlOpen.FileName = Path.GetFileName(filePath);
                    if (cdlOpen.ShowDialog() != DialogResult.Cancel)
                    {
                        filePath = cdlOpen.FileName;
                    }
                    else
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public MapWinGIS.tkFillStipple StringToStipple(string str)
        {
            switch (str)
            {
                case "Diagonal Down-Left":
                    return MapWinGIS.tkFillStipple.fsDiagonalDownLeft;
                case "Dialgonal Down-Right":
                    return MapWinGIS.tkFillStipple.fsDiagonalDownRight;
                case "Vertical":
                    return MapWinGIS.tkFillStipple.fsVerticalBars;
                case "Horizontal":
                    return MapWinGIS.tkFillStipple.fsHorizontalBars;
                case "Cross/Dot":
                    return MapWinGIS.tkFillStipple.fsPolkaDot;
                default:
                    return MapWinGIS.tkFillStipple.fsNone;
            }
        }

        public string StippleToString(MapWinGIS.tkFillStipple stipple)
        {
            switch (stipple)
            {
                case tkFillStipple.fsDiagonalDownLeft: return "Diagonal Down-Left";
                case MapWinGIS.tkFillStipple.fsDiagonalDownRight: return "Dialgonal Down-Right";
                case MapWinGIS.tkFillStipple.fsVerticalBars: return "Vertical";
                case MapWinGIS.tkFillStipple.fsHorizontalBars: return "Horizontal";
                case MapWinGIS.tkFillStipple.fsPolkaDot: return "Cross/Dot";
                default: return "None";
            }

        }



        #endregion


    }
}
