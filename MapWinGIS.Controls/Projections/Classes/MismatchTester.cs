/*********************************************************************
 * 文件名：MismatchTester.cs
 * 描  述：
 * *******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using MapWinGIS.Interfaces;
using System.Windows.Forms;

namespace MapWinGIS.Controls.Projections
{
    /// <summary>
    /// 提供测试结果
    /// </summary>
    public enum TestingResult
    {
        /// <summary>
        /// 对象是正确的或者用户忽略了Mismatch
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 文件应该被跳过
        /// </summary>
        SkipFile = 1,
        /// <summary>
        /// 行动应该被取消
        /// </summary>
        CancelOperation = 2,
        /// <summary>
        /// 在处理时发生错误
        /// </summary>
        Error = 3,
        /// <summary>
        /// 该层对象被置换成另一种文件
        /// </summary>
        Substituted = 4
    }

    /// <summary>
    /// 一个处理当在地图上添加另一个图层不匹配情况的类
    /// </summary>
    public class MismatchTester
    {
        #region 声明

        /// <summary>
        /// MapWinGIS的引用
        /// </summary>
        private MapWinGIS.Interfaces.IMapWin m_mapWin = null;

        /// <summary>
        /// 用于显示报告的窗口
        /// </summary>
        private frmTesterReport m_report = new frmTesterReport();

        /// <summary>
        /// 当用户不希望看到时不显示文件组对话框
        /// </summary>
        private bool m_usePreviousAnswerMismatch = false;

        /// <summary>
        /// 当用户不希望看到时不显示文件组对话框
        /// </summary>   
        private bool m_usePreviousAnswerAbsence = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建一个新的MismatchTester类实例
        /// </summary>
        /// <param name="mapWin"></param>
        public MismatchTester(MapWinGIS.Interfaces.IMapWin mapWin)
        {
            if (mapWin == null)
                throw new NullReferenceException("没有声明的MapWinGIS");

            m_mapWin = mapWin;
        }
        #endregion

        #region 属性

        /// <summary>
        /// 获取不匹配投影（或不存在投影）的文件列表
        /// </summary>
        public int FileCount
        {
            get
            {
                if (m_report == null)
                {
                    return 0;
                }
                else
                {
                    return m_report.MismatchedCount;
                }
            }
        }
        #endregion

        #region Report方法

        /// <summary>
        /// 显示投影进度
        /// </summary>
        /// <param name="proj">投影的目标工程</param>
        public void ShowReprojectionProgress(MapWinGIS.GeoProjection proj)
        {
            m_report.InitProgress(proj);
        }

        /// <summary>
        /// 显示报告
        /// </summary>
        /// <param name="proj">投影的目标工程</param>
        public void ShowReport(MapWinGIS.GeoProjection proj)
        {
            m_report.ShowReport(proj, "", ReportType.Loading);
        }

        /// <summary>
        /// 隐藏进度
        /// </summary>
        public void HideProgress()
        {
            m_report.Visible = false;
        }
        #endregion

        #region 测试...

        /// <summary>
        /// 测试一个类型是由拓展形成的图层的单层投影
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="newName">输出新的文件名</param>
        /// <returns>返回测试结果</returns>
        public TestingResult TestLayer(string filename, out string newName)
        {
            newName = filename;
            LayerSource layer = new LayerSource(filename, this as MapWinGIS.ICallback);//打开指定文件的图层
            if (layer.Type == LayerSourceType.Undefined)//图层类型是没有定义的
            {
                string message = layer.GetErrorMessage();
                if (message == "")
                    message = "Unspecified error";

                MessageBox.Show("Invalid datasource: " + message.ToLower() + Environment.NewLine + filename, m_mapWin.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return TestingResult.Error;
            }
            else
            {
                LayerSource newLayer = null;
                TestingResult result = this.TestLayer(layer, out newLayer);
                if (result == TestingResult.Substituted)
                {
                    newName = newLayer.Filename;
                    newLayer.Close();
                }

                layer.Close();
                return result;
            }
        }

        /// <summary>
        /// 测试单层投影
        /// </summary>
        /// <param name="layer">图层源（Shapefile或grid对象）</param>
        /// <param name="newLayer">输出新图层</param>
        /// <returns>返回测试结果</returns>
        public TestingResult TestLayer(LayerSource layer, out LayerSource newLayer)
        {
            if (layer == null)
                throw new ArgumentException("空图层引用被通过");

            newLayer = null;

            MapWinGIS.GeoProjection projectProj = m_mapWin.Project.GeoProjection;
            MapWinGIS.GeoProjection layerProj = layer.Projection;
            bool isSame = projectProj.get_IsSameExt(layerProj, layer.Extents, 10);

            // 让我们看看我们是否有项目的投影的一种方言
            if (!isSame && !projectProj.IsEmpty)
            {
                ProjectionDatabase db = (ProjectionDatabase)m_mapWin.ProjectionDatabase;//投影数据源
                if (db != null)
                {
                    CoordinateSystem cs = db.GetCoordinateSystem(projectProj, ProjectionSearchType.Enhanced);//坐标系统
                    if (cs != null)
                    {
                        db.ReadDialects(cs);
                        foreach (string dialect in cs.Dialects)
                        {
                            MapWinGIS.GeoProjection projTemp = new MapWinGIS.GeoProjection();
                            if (!projTemp.ImportFromAutoDetect(dialect))
                                continue;

                            if (layerProj.get_IsSame(projTemp))
                            {
                                isSame = true;
                                break;
                            }
                        }
                    }
                }
            }

            // 投影中可以包含的文件的后缀名，让我们试着用正确的后缀搜索文件
            if (!isSame)
            {
                if (CoordinateTransformation.SeekSubstituteFile(layer, projectProj, out newLayer))
                {
                    m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.Substituted, newLayer.Filename);
                    return TestingResult.Substituted;
                }
            }

            if (!layer.Projection.IsEmpty)
            {
                if (projectProj.IsEmpty)
                {
                    // 层具有投影，项目没有；分配到投影，不提示用户

                    // 让我们找个众所周知的投影与EPSG编码
                    ProjectionDatabase db = m_mapWin.ProjectionDatabase as ProjectionDatabase;
                    if (db != null)
                    {
                        CoordinateSystem cs = db.GetCoordinateSystem(layerProj, ProjectionSearchType.UseDialects);
                        if (cs != null)
                        {
                            MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
                            if (proj.ImportFromEPSG(cs.Code))
                                layerProj = proj;
                        }
                    }

                    m_mapWin.Project.GeoProjection = layerProj;
                    return TestingResult.Ok;
                }
                else if (isSame)
                {
                    // 相同投影
                    return TestingResult.Ok;
                }
                else
                {
                    // 用户必须被提示
                    if (!m_usePreviousAnswerMismatch && !m_mapWin.ApplicationInfo.NeverShowProjectionDialog)
                    {
                        bool dontShow = false;
                        bool useForOthers = false;

                        ArrayList list = new ArrayList();
                        list.Add("Ignore mismatch");
                        list.Add("Reproject file");
                        //list.Add("Skip file");
                        // PM 2013-05-03:
                        list.Add("Don't load the layer");

                        frmProjectionMismatch form = new frmProjectionMismatch((ProjectionDatabase)m_mapWin.ProjectionDatabase);

                        int choice = form.ShowProjectionMismatch(list, (int)m_mapWin.ApplicationInfo.ProjectionMismatchBehavior,
                                                                 projectProj, layer.Projection, out useForOthers, out dontShow);

                        form.Dispose();
                        if (choice == -1)
                            return TestingResult.CancelOperation;

                        m_usePreviousAnswerMismatch = useForOthers;
                        m_mapWin.ApplicationInfo.ProjectionMismatchBehavior = (ProjectionMismatchBehavior)choice;
                        m_mapWin.ApplicationInfo.NeverShowProjectionDialog = dontShow;
                    }

                    MapWinGIS.Interfaces.ProjectionMismatchBehavior behavior = m_mapWin.ApplicationInfo.ProjectionMismatchBehavior;

                    switch (behavior)
                    {
                        case ProjectionMismatchBehavior.Reproject:
                            TestingResult result = CoordinateTransformation.ReprojectLayer(layer, out newLayer, projectProj, m_report);
                            if (result == TestingResult.Ok || result == TestingResult.Substituted)
                            {
                                ProjectionOperaion oper = result == TestingResult.Ok ? ProjectionOperaion.Reprojected : ProjectionOperaion.Substituted;
                                string newName = newLayer == null ? "" : newLayer.Filename;
                                m_report.AddFile(layer.Filename, layer.Projection.Name, oper, newName);
                                return newName == layer.Filename ? TestingResult.Ok : TestingResult.Substituted;
                            }
                            else
                            {
                                m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.FailedToReproject, "");
                                return TestingResult.Error;
                            }

                        case ProjectionMismatchBehavior.IgnoreMismatch:
                            m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.MismatchIgnored, "");
                            return TestingResult.Ok;

                        case ProjectionMismatchBehavior.SkipFile:
                            m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.Skipped, "");
                            return TestingResult.SkipFile;
                    }
                }
            }
            else if (!projectProj.IsEmpty)          // 图层投影是空的
            {
                bool projectProjectionExists = !projectProj.IsEmpty;

                // 用户必须被提示
                if (!m_usePreviousAnswerAbsence && !m_mapWin.ApplicationInfo.NeverShowProjectionDialog)
                {
                    bool dontShow = false;
                    bool useForOthers = false;

                    ArrayList list = new ArrayList();

                    // 当在投影第一变体应排除
                    int val = projectProjectionExists ? 0 : 1;

                    if (projectProjectionExists)
                    {
                        // PM 2013-05-03:
                        //list.Add("Assign projection from project");
                        list.Add("Use the project's projection");
                    }
                    // list.Add("Ignore the absence");
                    // list.Add("Skip the file");
                    list.Add("Ignore the missing of projection file");
                    list.Add("Don't load the layer");

                    frmProjectionMismatch form = new frmProjectionMismatch((ProjectionDatabase)m_mapWin.ProjectionDatabase);
                    int choice = form.ShowProjectionAbsence(list, (int)m_mapWin.ApplicationInfo.ProjectionAbsenceBehavior - val, projectProj, out useForOthers, out dontShow);
                    form.Dispose();

                    if (choice == -1)
                        return TestingResult.CancelOperation;

                    choice += val;

                    m_usePreviousAnswerAbsence = useForOthers;
                    m_mapWin.ApplicationInfo.ProjectionAbsenceBehavior = (ProjectionAbsenceBehavior)choice;
                    m_mapWin.ApplicationInfo.NeverShowProjectionDialog = dontShow;
                }

                // 当在项目没有投影，它不能分配层
                ProjectionAbsenceBehavior behavior = m_mapWin.ApplicationInfo.ProjectionAbsenceBehavior;
                if (!projectProjectionExists && m_mapWin.ApplicationInfo.ProjectionAbsenceBehavior == ProjectionAbsenceBehavior.AssignFromProject)
                {
                    behavior = ProjectionAbsenceBehavior.IgnoreAbsence;
                }

                switch (behavior)
                {
                    case ProjectionAbsenceBehavior.AssignFromProject:
                        m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.Assigned, "");
                        layer.Projection = projectProj;
                        return TestingResult.Ok;

                    case ProjectionAbsenceBehavior.IgnoreAbsence:
                        m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.AbsenceIgnored, "");
                        return TestingResult.Ok;

                    case ProjectionAbsenceBehavior.SkipFile:
                        m_report.AddFile(layer.Filename, layer.Projection.Name, ProjectionOperaion.Skipped, "");
                        return TestingResult.SkipFile;
                }
            }
            else
            {
                // 层没有投影，项目也没有，不在这里
            }

            System.Diagnostics.Debug.Print("Invalid result in projection tester");
            return TestingResult.Ok;
        }
        #endregion
    }
}
