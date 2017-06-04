/*********************************************************************************
 * 文件名:clsProject.cs （F）
 * 描  述:提供给插件操作当前项目的方法。投影操作、文件操作、项目配置操作、地图单位等
 * ********************************************************************************/

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using MapWinGIS.Interfaces;

namespace MapWinGIS.MainProgram
{
    public class Project : MapWinGIS.Interfaces.Project
    {
        #region Project 接口实现
        
        /// <summary>
        /// 触发项目投影改变时事件
        /// </summary>
        public void FireOnUpdateTableJoin(string filename, string fieldNames, string joinOptions, MapWinGIS.Table tableToFill)
        {
            if (OnUpdateTableJoin != null)
            {
                OnUpdateTableJoin(filename, fieldNames, joinOptions, tableToFill);
            }
        }

        /// <summary>
        /// 项目投影改变时发生的事件
        /// </summary>
        public event ProjectionChangedDelegate ProjectionChanged;

        /// <summary>
        /// 获取包含当前MainProgram项目的配置文件的文件名完整路径
        /// </summary>
        public string ConfigFileName
        {
            get { return Program.projInfo.ConfigFileName; }
        }

        /// <summary>
        /// 获取当前前项目的文件名的完整路径
        /// </summary>
        public string FileName
        {
            get { return Program.projInfo.ProjectFileName; }

        }

        /// <summary>
        /// 保存当前项目
        /// </summary>
        public bool Save(string filename)
        {
            Program.projInfo.ProjectFileName = filename;
            bool retval = Program.projInfo.SaveProject();
            Program.frmMain.SetModified(false);
            return retval;
        }

        /// <summary>
        /// 复制当前项目并保存
        /// </summary>
        public bool SaveCopy(string filename)
        {
            string backupFilename = Program.projInfo.ProjectFileName;
            Program.projInfo.ProjectFileName = filename;
            bool retval = Program.projInfo.SaveProject();
            Program.projInfo.ProjectFileName = backupFilename;
            Program.frmMain.SetModified(false);
            return retval;
        }

        /// <summary>
        /// 从项目文件中加载一个符号（symbology），到指定的层
        /// </summary>
        /// <param name="filename">包含项目文件名的完整路径</param>
        /// <param name="handle">要加载symbology的图层的handle</param>
        public bool LoadLayerSymbologyFromProjectFile(string filename, int handle)
        {
            XmlDocument projectFile = new XmlDocument();
            projectFile.Load(filename);

            //确保该层的句柄存在
            if (Program.frmMain.m_Layers[handle] == null)
            {
                return false;
            }

            XmlProjectFile projectXml = new XmlProjectFile();
            foreach (XmlNode layerNode in projectFile.GetElementsByTagName("Layer"))
            {
                //图层名和存储的图层名一致，文件名也要一致
                if (layerNode.Attributes["Name"].InnerText == Program.frmMain.m_Layers[handle].Name && Path.GetFileName(layerNode.Attributes["Path"].InnerText).ToLower() == Path.GetFileName(Program.frmMain.m_Layers[handle].FileName).ToLower())
                {
                    projectXml.LoadLayerProperties(layerNode, handle, true);
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 加载一个项目文件
        /// </summary>
        /// <param name="filename">要加载的项目文件路径名</param>
        public bool Load(string filename)
        {
            if (File.Exists(filename))
            {
                Program.projInfo.ProjectFileName = filename;
                return Program.projInfo.LoadProject(FileName);
            }
            return false;
        }

        /// <summary>
        /// 加载一个项目文件到当前项目中，作为一个子group
        /// </summary>
        /// <param name="filename">要加载的完整项目路径</param>
        public void LoadIntoCurrentProject(string filename)
        {
            Program.frmMain.DoOpenIntoCurrent(filename);
        }

        /// <summary>
        /// 保存当前配置到配置文件中
        /// </summary>
        /// <param name="Filename">保存配置文件的文件路径</param>
        public bool SaveConfig(string filename)
        {
            if (Path.GetExtension(filename) != ".mwgcfg")
            {
                filename += ".mwgcfg";
            }
            Program.projInfo.ConfigFileName = filename;
            return Program.projInfo.SaveConfig();

        }

        /// <summary>
        /// 获取或设置项目文件是否修改
        /// </summary>
        public bool Modified
        {
            get { return Program.projInfo.Modified; }
            set { Program.frmMain.SetModified(value); }
        }

        /// <summary>
        /// 返回或者设置当前项目投影
        /// 格式：+proj=tmerc +ellps=WGS84  +datum=WGS84
        /// </summary>
        public string ProjectProjection
        {
            get 
            {
                MapWinGIS.GeoProjection prj = Program.projInfo.GeoProjection;
                if (prj.IsEmpty)
                {
                    return "";
                }
                return prj.ExportToProj4();
            }
            set 
            {
                if (value == null) { value = ""; }
                MapWinGIS.GeoProjection prj = new MapWinGIS.GeoProjection();
                prj.ImportFromProj4(value);
                this.GeoProjection = prj;
            }
        }

        /// <summary>
        /// 项目投影改变时发生的事件
        /// </summary>
        public event OnUpdateTableJoinDelegate OnUpdateTableJoin;

        /// <summary>
        /// 此处触发ProjectionChanged事件
        /// 获取或设置项目投影
        /// </summary>
        public MapWinGIS.GeoProjection GeoProjection
        {
            get { return Program.projInfo.GeoProjection; }
            set
            {
                if (value != null)
                {
                    MapWinGIS.GeoProjection projOld = Program.projInfo.GeoProjection;
                    if (value.IsEmpty)
                    {
                        Program.projInfo.GeoProjection = new MapWinGIS.GeoProjection();
                    }
                    else
                    {
                        //确保投影在数据库中
                        if (Program.ProjectionDB != null)
                        {
                            //MapWinGIS.Controls.Projections.CoordinateSystem cs = Program.ProjectionDB.GetCoordinateSystem(value, ProjectionSearchType.Standard);
                            //if (cs == null)
                            //{
                            //    cs = Program.ProjectionDB.GetCoordinateSystem(value, ProjectionSearchType.UseDialects);
                            //    if (cs != null)
                            //    {
                            //        MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
                            //        if (proj.ImportFromEPSG(cs.Code))
                            //        {
                            //            value = proj;
                            //        }
                            //    }
                            //}
                        }
                        Program.projInfo.GeoProjection.CopyFrom(value);
                    }
                    if (ProjectionChanged != null)
                    { 
                        ProjectionChanged(projOld, Program.projInfo.GeoProjection); 
                    }
                }

            }
        }

        /// <summary>
        /// 该项目的指定的配置文件是否已经加载了
        /// </summary>
        public bool ConfigLoaded
        {
            get { return Program.projInfo.ConfigLoaded; }

        }

        /// <summary>
        /// 返回ArrayList数组，存储最近的项目(完整路径)
        /// </summary>
        public System.Collections.ArrayList RecentProjects
        {
            get { return Program.projInfo.RecentProjects; }
        }

        /// <summary>
        /// 当前项目的单位，格式："Meters", "Feet"
        /// </summary>
        public string MapUnits
        {
            get
            {
                if (Program.projInfo.m_MapUnits == null || Program.projInfo.m_MapUnits == "")
                {
                    Program.projInfo.m_MapUnits = ""; //防止没有引用

                    //从proj4字符串中检索地图的单位，但是这只支持meters
                    if (ProjectProjection != "")
                    {
                        if ((ProjectProjection.ToLower()).IndexOf("+proj=longlat") + 1 > 0 || (ProjectProjection.ToLower()).IndexOf("+proj=latlong") + 1 > 0)
                        {
                            Program.projInfo.m_MapUnits = "Lat/Long";
                        }
                        else if ((ProjectProjection.ToLower()).IndexOf("+units=m") + 1 > 0)
                        {
                            Program.projInfo.m_MapUnits = "Meters";
                        }
                        else if ((ProjectProjection.ToLower()).IndexOf("+units=ft") + 1 > 0)
                        {
                            Program.projInfo.m_MapUnits = "Feet";
                        }
                        else if ((ProjectProjection.ToLower()).IndexOf("+to_meter=") + 1 > 0) //添加支持feet
                        {
                            double toMeter;
                            toMeter = Convert.ToDouble(System.Text.RegularExpressions.Regex.Replace(ProjectProjection.ToLower(), "^.*to_meter=([.0-9]+).*$", "$1"));
                            if (toMeter > 0.3047 && toMeter < 0.3049)
                            {
                                Program.projInfo.m_MapUnits = "Feet";
                            }
                        }
                    }

                }

                return Program.projInfo.m_MapUnits.Trim();
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }

                Program.projInfo.m_MapUnits = value.Trim();
            }
        }

        /// <summary>
        /// 获取或设置当前更替的显示单位，格式："Meters", "Feet"
        /// </summary>
        public string MapUnitsAlternate
        {
            get
            {
                if (Program.projInfo.ShowStatusBarCoords_Alternate == null || Program.projInfo.ShowStatusBarCoords_Alternate == "")
                {
                    return "";
                }
                else
                {
                    return Program.projInfo.ShowStatusBarCoords_Alternate.Trim();
                }
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }

                Program.projInfo.ShowStatusBarCoords_Alternate = value.Trim();
            }
        }

        /// <summary>
        /// 保存shape的设置
        /// </summary>
        public bool SaveShapeSettings
        {
            get { return Program.projInfo.SaveShapeSettings; }
            set { Program.projInfo.SaveShapeSettings = value; }
        }

        #endregion

        /// <summary>
        /// 显示对话框，选择投影，提示是否将投影应用于项目，并返回用户选择的投影
        ///  在状态栏和项目属性中可以调用
        /// </summary>
        public MapWinGIS.GeoProjection SetProjectProjectionByDialog()
        {
            MapWinGIS.GeoProjection proj = this.GetProjectionFromUser(); //从对话框中获取到选择的投影

            bool needsReloading = false;

            if (proj != null)
            {
                //检测是否需要重新加载项目，来重新设置投影
                if (Program.frmMain.Layers.NumLayers > 0 && !Program.frmMain.Project.GeoProjection.IsEmpty)
                {

                    MapWinGIS.Extents ext = Program.frmMain.MapMain.MaxExtents;
                    if (!proj.IsSameExt[Program.frmMain.Project.GeoProjection, ext]) //检测当前投影和项目投影是否一致
                    {
                        //不一致，则重投
                        if (MessageBox.Show("此操作需要重新加载项目，来在图层上重新投影。继续？", Program.frmMain.ApplicationInfo.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            needsReloading = true;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                //设置新的投影
                MapWinGIS.GeoProjection projOld = Program.frmMain.Project.GeoProjection;
                Program.frmMain.Project.GeoProjection = proj;

                if (needsReloading) //需要重新加载，则保存加载
                {
                    // 保存原来的
                    if (this.Modified)
                    {
                        Program.frmMain.DoSave();
                    }

                    // 取消保存
                    if (this.Modified)
                    {
                        Program.frmMain.Project.GeoProjection = projOld;
                        return null;
                    }
                    else
                    {
                        // 确保用户在重新加载时看见必要的对话框
                        Program.frmMain.ApplicationInfo.ShowLoadingReport = true;
                        Program.frmMain.ApplicationInfo.NeverShowProjectionDialog = false;
                        Program.appInfo.ProjectReloading = true;

                        return proj;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 显示“选择投影”对话框，并且返回用户选择的投影
        /// </summary>
        public MapWinGIS.GeoProjection GetProjectionFromUser()
        {
            //frmChooseProjection form = new frmChooseProjection(Program.ProjectionDB, Program.frmMain);
            MapWinGIS.GeoProjection proj = null;

            //if (form.ShowDialog(Program.frmMain) == DialogResult.OK)
            //{
            //    if (form.projectionTreeView1.SelectedProjection == null && form.projectionTreeView1.SelectedCoordinateSystem != null)
            //    {
            //        MessageBox.Show("初始化选择的投影出错！", Program.appInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        proj = form.projectionTreeView1.SelectedProjection;
            //    }
            //}

            //form.Dispose();
            return proj;
        }

    }
}
