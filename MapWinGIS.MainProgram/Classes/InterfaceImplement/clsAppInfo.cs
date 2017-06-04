/****************************************************************************
 * 文件名:clsAppInfo.cs
 * 描  述:存储与宿主程序相关配置,计算shape的距离、面积的方法
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MapWinGIS.Interfaces;
using System.Collections;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    internal class AppInfo : MapWinGIS.Interfaces.AppInfo
    {
        #region 变量声明
        public string Version;//版本
        public string BuildDate;//创建时间
        public string Developer;//开发者
        public string Comments;//注释

        private string _ApplicationName; //应用程序名
        private string _HelpFilePath; //帮助目录
        private string _WelcomePlugin; //欢迎界面名
        private System.Drawing.Image _SplashPicture; //闪屏图片，注意定义Image时要指明是MapWinGIS.ocx中的还是System中的
        private System.Drawing.Icon _FormIcon; //程序图标
        private double _SplashTime; //闪屏时间
        private string _DefaultDir;//文件对话框的默认目录
        private string _URL; // URL地址
        private bool _ShowWelcomeScreen = false; //是否显示欢迎界面
        private bool _UseSplashScreen;  //使用闪屏
        private bool _ShowMapWinGISVersion; //显示版本信息
        private bool _ShowFloatingScalebar; //显示浮动比例尺
        private bool _ShowRedrawSpeed; //显示重绘速度
        private bool _ShowAvoidCollision; 
        private bool _ShowAutoCreateSpatialindex;

        public string ApplicationPluginDir; //内部插件目录
        public bool m_neverShowProjectionDialog;
        public string ProjectionDialog_PreviousNoProjAnswer;
        public string ProjectionDialog_PreviousMismatchAnswer;
        public GeoTIFFAndImgBehavior LoadTIFFandIMGasgrid = GeoTIFFAndImgBehavior.Automatic; //MainWinForm中的枚举
        public ESRIBehavior LoadESRIAsGrid = ESRIBehavior.LoadAsGrid;
        public MouseWheelZoomDir MouseWheelZoom = MouseWheelZoomDir.NoAction;
        public string LogfilePath; //记录调试信息的文件路径
        public bool LabelsUseProjectLevel; //使用项目的标签还是shapefile的标签

        //默认地图背景色
        public Color DefaultBackColor = Color.FromArgb(0, 255, 255, 255);

        //用户定义语言设置
        /// <summary>
        /// 指示是否覆盖本地语言
        /// true-使用用户设置的语言，false-使用本地语言
        /// </summary>
        public bool OverrideSystemLocale = false;
        /// <summary>
        /// 存储当前语言标识符
        /// </summary>
        public string Locale;

        //距离测量 Distance Measuring Stuff:
        public bool MeasuringCurrently;
        public double MeasuringStartX;
        public double MeasuringStartY;
        public System.Drawing.Point MeasuringScreenPointStart;
        public System.Drawing.Point MeasuringScreenPointFinish;
        public double MeasuringTotalDistance;
        public int MeasuringDrawing;
        public ArrayList MeasuringPreviousSegments;

        //面积测量 Area Measuring Stuff:
        public bool AreaMeasuringCurrently;
        public ArrayList AreaMeasuringlstDrawPoints = new ArrayList();
        public ArrayList AreaMeasuringReversibleDrawn = new ArrayList();
        public double AreaMeasuringLastStartPtX = -1;
        public double AreaMeasuringLastStartPtY = -1;
        public double AreaMeasuringStartPtX = -1;
        public double AreaMeasuringStartPtY = -1;
        public double AreaMeasuringLastEndX = -1;
        public double AreaMeasuringLastEndY = -1;
        public bool AreaMeasuringEraseLast = false;
        public System.Drawing.Color AreaMeasuringmycolor = new System.Drawing.Color();

        private System.Windows.Forms.Cursor MeasureCursor = null;

        public bool ShowDynamicVisibilityWarnings = true;
        public bool ShowLayerAfterDynamicVisibility = true;
        public SymbologyBehavior m_symbologyLoadingBehavior;
        public ProjectionAbsenceBehavior m_projectionAbsenceBehavior;
        public ProjectionMismatchBehavior m_projectionMismatchBehavior;
        private bool m_showLoadingReport;
        public bool ProjectReloading;

        // favorite projections列表
        private List<int> m_favoriteProjections = null;

        #endregion

        public AppInfo()
        {   
            //D;\MapWinGIS\MapWinGIS\bin
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(this.GetType()).Location);
            
            ApplicationPluginDir = App.Path + "\\" + "ApplicationPlugins";

            Version = App.VersionString;            
            Developer = "MapWinGIS Team";
            BuildDate = "";
            Comments = "";

            _ApplicationName = "MapWinGIS";      
            _SplashTime = 2;
            _URL = "http://www.baidu.com";
            _ShowWelcomeScreen = true;
            _ShowMapWinGISVersion = true;
            _HelpFilePath = path + "\\help\\MapWinGIS.chm";
            _DefaultDir = path;
            _FormIcon = MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;
            _UseSplashScreen = true;

            m_neverShowProjectionDialog = false;
            ProjectionDialog_PreviousNoProjAnswer = "";
            ProjectionDialog_PreviousMismatchAnswer = "";
            m_showLoadingReport = true;

            OverrideSystemLocale = false;
            Locale = string.Empty;
            ProjectReloading = false;

            m_symbologyLoadingBehavior = SymbologyBehavior.DefaultOptions;
            m_projectionAbsenceBehavior = Interfaces.ProjectionAbsenceBehavior.AssignFromProject;
            m_projectionMismatchBehavior = Interfaces.ProjectionMismatchBehavior.Reproject;

            m_favoriteProjections = new List<int>();
            m_favoriteProjections.Add(4326);
            m_favoriteProjections.Add(3857);

            LogfilePath = App.Path + "\\" + "Log";
        }

        /// <summary>
        /// 显示在帮助菜单中的帮助路径
        /// </summary>
        public string HelpFilePath
        {
            get
            {
                return this._HelpFilePath;
            }
            set
            {
                this._HelpFilePath = value;
            }
        }

        /// <summary>
        /// 在应用开始时，是否显示一个splash screen（启动界面）
        /// </summary>
        public bool UseSplashScreen
        {
            get
            {
                return this._UseSplashScreen;
            }
            set
            {
                this._UseSplashScreen = value;
            }
 
        }

        /// <summary>
        /// 欢迎界面的名字
        /// </summary>
        public string WelcomePlugin
        {
            get
            {
                return this._WelcomePlugin;
            }
            set
            {
                this._WelcomePlugin = value;
            }
        }

        /// <summary>
        /// 显示在启动界面上的图像
        /// </summary>
        public System.Drawing.Image SplashPicture
        {
            get
            {
                if (_SplashPicture != null)
                {
                    return this._SplashPicture;
                }
                return global::MapWinGIS.MainProgram.GlobalResource.splashPic; 
            }
            set
            {
                this._SplashPicture = value;
            }
        }

        /// <summary>
        /// 默认窗体的图标
        /// </summary>
        public System.Drawing.Icon FormIcon
        {
            get
            {
                if (_FormIcon != null)
                {
                 return this._FormIcon;
                }
                return global::MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;
            }
            set
            {
                this._FormIcon = value;
            }
        }

        /// <summary>
        /// 启动界面的显示时间
        /// </summary>
        public double SplashTime
        {
            get
            {
                return this._SplashTime;
            }
            set
            {
               this._SplashTime = value;
            }
        }

        /// <summary>
        /// 文件对话框的默认目录
        /// </summary>
        public string DefaultDir
        {
            get
            {
                return this._DefaultDir;
            }
            set
            {
                this._DefaultDir = value;
            }
        }

        /// <summary>
        /// 在帮助->关于 对话框的URL
        /// </summary>
        public string URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }

        /// <summary>
        /// 主窗体的名字
        /// </summary>
        public string ApplicationName
        {
            get { return this._ApplicationName; }
            set { this._ApplicationName = value; }
        }

        /// <summary>
        /// 是否显示启动界面
        /// </summary>
        public bool ShowWelcomeScreen
        {
            get
            {
                return this._ShowWelcomeScreen;
            }
            set
            {
                this._ShowWelcomeScreen = value;
            }
        }

        /// <summary>
        /// 是否显示MainProgram的版本
        /// </summary>
        public bool ShowMapWinGISVersion
        {
            get
            {
                return this._ShowMapWinGISVersion;
            }
            set
            {
                this._ShowMapWinGISVersion = value;
            }
        }

        /// <summary>
        /// 是否显示FloatingScalebar（浮动比例尺）
        /// </summary>
        public bool ShowFloatingScalebar
        {
            get
            {
                return this._ShowFloatingScalebar;
            }
            set
            {
                this._ShowFloatingScalebar = value;
            }
        }

        /// <summary>
        /// 是否显示redraw speed标签
        /// </summary>
        public bool ShowRedrawSpeed
        {
            //ShowHideRedrawSpeed改为ShowRedrawSpeed
            get
            {
                return this._ShowRedrawSpeed;
            }
            set
            {
                this._ShowRedrawSpeed = value;
            }
        }

        /// <summary>
        /// 在加载层数据时，定义加载可视化选项的行为
        /// Defines behavior for loading visualazation options while loading data layer
        /// </summary>
        public SymbologyBehavior SymbologyLoadingBehavior
        {
            get
            {
                return this.m_symbologyLoadingBehavior;
            }
            set
            {
                this.m_symbologyLoadingBehavior = value;
            }
        }

        /// <summary>
        /// List of EPSG codes for favorite projections
        /// </summary>
        public List<int> FavoriteProjections
        {
            get
            {
                return m_favoriteProjections;
            }
        }

        /// <summary>
        /// 当添加的图层没有投影时，定义应用程序的行为
        /// </summary>
        public ProjectionAbsenceBehavior ProjectionAbsenceBehavior
        {
            get
            {
                return m_projectionAbsenceBehavior;
            }
            set
            {
                m_projectionAbsenceBehavior = value;
            }
        }

        /// <summary>
        /// 当添加的层的投影不匹配于已添加的层的投影时，定义应该程序行为
        /// Defines application behavior when projection of the layer being added is different from project one
        /// </summary>
        public ProjectionMismatchBehavior ProjectionMismatchBehavior
        {
            get
            {
                return m_projectionMismatchBehavior;
            }
            set
            {
                m_projectionMismatchBehavior = value;
            }
        }

        /// <summary>
        /// 获取设置一个值，指示在投影不匹配的情况下，是否显示投影不匹配对话框
        /// </summary>
        public bool NeverShowProjectionDialog
        {
            get
            {
                return m_neverShowProjectionDialog;
            }
            set
            {
                m_neverShowProjectionDialog = value;
            }
        }

        /// <summary>
        /// 获取设置一个值，指示在加载一个图层之后，投影不匹配的情况下是否显示加载报告
        /// </summary>
        public bool ShowLoadingReport
        {
            get
            {
                return m_showLoadingReport;
            }
            set
            {
                m_showLoadingReport = value;
            }
        }

        public bool ShowAvoidCollision
        {
            get { return this._ShowAvoidCollision; }
            set { this._ShowAvoidCollision = value; }
        }

        /// <summary>
        /// 是否显示自动创建空间索引
        /// </summary>
        public bool ShowAutoCreateSpatialindex
        {
            get { return this._ShowAutoCreateSpatialindex; }
            set { this._ShowAutoCreateSpatialindex = value; }
        }

        #region 测量方法

        internal void AreaMeasuringClearTempLines()
        {
            if (this.AreaMeasuringEraseLast)
            {
                System.Windows.Forms.ControlPaint.DrawReversibleLine(new System.Drawing.Point((int)this.AreaMeasuringLastStartPtX, (int)this.AreaMeasuringLastStartPtY), new System.Drawing.Point((int)this.AreaMeasuringLastEndX, (int)this.AreaMeasuringLastEndY), this.AreaMeasuringmycolor);
                System.Windows.Forms.ControlPaint.DrawReversibleLine(new System.Drawing.Point((int)this.AreaMeasuringStartPtX, (int)this.AreaMeasuringStartPtY), new System.Drawing.Point((int)this.AreaMeasuringLastEndX, (int)this.AreaMeasuringLastEndY), this.AreaMeasuringmycolor);
                this.AreaMeasuringLastStartPtX = -1;
                this.AreaMeasuringLastStartPtY = -1;
            }
            for (int i = 0; i < this.AreaMeasuringReversibleDrawn.Count; i += 4)
            {
                System.Windows.Forms.ControlPaint.DrawReversibleLine(new System.Drawing.Point((int)this.AreaMeasuringReversibleDrawn[i], (int)this.AreaMeasuringReversibleDrawn[i + 1]), new System.Drawing.Point((int)this.AreaMeasuringReversibleDrawn[i + 2], (int)this.AreaMeasuringReversibleDrawn[i + 3]), this.AreaMeasuringmycolor);
            }
            this.AreaMeasuringEraseLast = false;
            this.AreaMeasuringReversibleDrawn.Clear();
            this.AreaMeasuringlstDrawPoints.Clear();
            Program.frmMain.m_View.Draw.ClearDrawings();//取不到，显示转换
        }

        internal void AreaMeasuringStop()
        {
            Program.frmMain.MapMain.UDCursorHandle = -1;
            Program.frmMain.MapMain.MapCursor = MapWinGIS.tkCursor.crsrArrow;
            Program.frmMain.MapMain.CursorMode = MapWinGIS.tkCursorMode.cmNone;

            this.AreaMeasuringCurrently = false;
            this.AreaMeasuringStartPtX = -1;
            this.AreaMeasuringStartPtY = -1;
            this.AreaMeasuringLastEndX = -1;
            this.AreaMeasuringLastEndY = -1;
            this.AreaMeasuringLastStartPtX = -1;
            this.AreaMeasuringLastStartPtY = -1;
            AreaMeasuringClearTempLines();

            Program.frmMain.GetOrRemovePanel(Program.frmMain.resources.GetString("msgPanelArea_Text"), true);
        }

        internal void AreaMeasuringBegin()
        {
        //    if (MeasureCursor == null)
        //    {
        //        MeasureCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream("MapWinGIS.MainProgram.Resources.measuring.ico"));
        //    }

        //    Program.frmMain.MapMain.UDCursorHandle = (int)MeasureCursor.Handle;
        //    Program.frmMain.MapMain.MapCursor = MapWinGIS.tkCursor.crsrUserDefined;
        //    Program.frmMain.MapMain.CursorMode = MapWinGIS.tkCursorMode.cmNone;
        //    // frmMain.tbbMeasureArea.Checked = True
        //    this.AreaMeasuringCurrently = true;
        //    this.AreaMeasuringlstDrawPoints = new ArrayList();
        //    this.AreaMeasuringReversibleDrawn = new ArrayList();
        //    Program.frmMain.m_StatusBar.AddPanel(Program.frmMain.resources.GetString("msgPanelArea_Text"), 0, 100, System.Windows.Forms.StatusBarPanelAutoSize.Contents);
        }

        internal string AreaMeasuringCalculate()//计算绘制的多边形的面积，返回结果是包含单位名字
        {
            //MapWinGIS.Shape tempPoly = new MapWinGIS.Shape();
            //tempPoly.Create(MapWinGIS.ShpfileType.SHP_POLYGON);
            //// Loop the points, inserting them into new poly
            //int i;
            //for (i = 0; i < this.AreaMeasuringlstDrawPoints.Count; i++)
            //{
            //    tempPoly.InsertPoint(((MapWinGIS.Point)(this.AreaMeasuringlstDrawPoints[i])), tempPoly.numPoints);
            //}
            ////Add the first point again to complete the polygon
            //tempPoly.InsertPoint((MapWinGIS.Point)this.AreaMeasuringlstDrawPoints[0], tempPoly.numPoints);

            //UnitOfMeasure DataUnit; //the unit specified in Project Settings..Map Data Units
            //UnitOfMeasure MeasureUnit; //the unit specified in Project Settings..Show Additional Unit
            //DataUnit = MapWinGeoProc.UnitConverter.StringToUOM(Program.frmMain.m_Project.MapUnits);
            //MeasureUnit = DataUnit;
            //if (Program.projInfo.ShowStatusBarCoords_Alternate.ToLower() != "(none)")
            //{
            //    MeasureUnit = MapWinGeoProc.UnitConverter.StringToUOM(Program.projInfo.ShowStatusBarCoords_Alternate);
            //}

            ////Convert the total area from Map Data units to Alternate units
            //double newArea = MapWinGeoProc.Utils.Area(tempPoly, DataUnit);

            ////if Map Data Units are DecimalDegrees, the area() function returns the result in kilometers
            //if (DataUnit == UnitOfMeasure.DecimalDegrees)
            //{
            //    DataUnit = UnitOfMeasure.Kilometers;
            //}
            //if (MeasureUnit == UnitOfMeasure.DecimalDegrees)
            //{
            //    MeasureUnit = DataUnit;
            //}

            //newArea = MapWinGeoProc.UnitConverter.ConvertArea(DataUnit, MeasureUnit, newArea);

            //string squared = (Convert.ToChar(178)).ToString(); //the exponent sign

            //// internationalization - show area in the in status bar
            //string msgArea = string.Format("{0} {1}{2}", Program.frmMain.FormatDistance(newArea), MeasureUnit.ToString(), squared);

            //return msgArea;
            return null;
        }

        internal void MeasuringBegin()
        {
            //if (MeasureCursor == null)
            //{
            //    MeasureCursor = new Cursor(this.GetType().Assembly.GetManifestResourceStream("MapWinGIS.MainProgram.Resources.measuring.ico"));
            //}

            //Program.frmMain.MapMain.UDCursorHandle = (int)MeasureCursor.Handle;
            //Program.frmMain.MapMain.MapCursor = MapWinGIS.tkCursor.crsrUserDefined;
            //Program.frmMain.MapMain.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            ////  frmMain.tbbMeasure.Checked = True
            //this.MeasuringCurrently = true;
            //this.MeasuringDrawing = -1;
            //this.MeasuringPreviousSegments = new ArrayList();
            ////StatusBar.AddPanel("Distance: Click first point", 0, 100, Windows.Forms.StatusBarPanelAutoSize.Contents)
            //Program.frmMain.StatusBar.AddPanel(Program.frmMain.resources.GetString("msgPanelDistance_Text"), 0, 100, System.Windows.Forms.StatusBarPanelAutoSize.Contents);
        }

        internal void MeasuringStop()
        {
            Program.frmMain.MapMain.UDCursorHandle = -1;
            Program.frmMain.MapMain.MapCursor = MapWinGIS.tkCursor.crsrArrow;
            Program.frmMain.MapMain.CursorMode = MapWinGIS.tkCursorMode.cmNone;
            //  frmMain.tbbMeasure.Checked = False
            this.MeasuringCurrently = false;
            this.MeasuringTotalDistance = 0;
            this.MeasuringStartX = 0;
            this.MeasuringStartY = 0;
            this.MeasuringScreenPointStart = default(System.Drawing.Point);
            this.MeasuringScreenPointFinish = default(System.Drawing.Point);
            this.MeasuringPreviousSegments.Clear();
            this.MeasuringPreviousSegments = null;
            if (this.MeasuringDrawing != -1)
            {
                Program.frmMain.MapMain.ClearDrawing(this.MeasuringDrawing);
                this.MeasuringDrawing = -1;
            }

            Program.frmMain.GetOrRemovePanel(Program.frmMain.resources.GetString("msgPanelDistance_Text"), true);
        }

        internal void MeasuringDrawPreviousSegments()
        {
            for (int i = 0; i < this.MeasuringPreviousSegments.Count; i += 4)
            {
                Program.frmMain.MapMain.DrawLine((double)this.MeasuringPreviousSegments[i], (double)this.MeasuringPreviousSegments[i+1],(double)this.MeasuringPreviousSegments[i+2], (double)this.MeasuringPreviousSegments[i+3], 2, (uint)(System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)));
            }
        }

        #endregion


    }
}
