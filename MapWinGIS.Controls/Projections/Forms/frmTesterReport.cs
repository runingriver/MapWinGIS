/******************************************************************
 * 文件名：frmTesterReport.cs
 * 描  述：投影报告窗口
 * **************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.Controls.Projections
{
    /// <summary>
    /// 投影操作集合
    /// </summary>
    public enum ProjectionOperaion
    {
        /// <summary>
        /// 重新投影
        /// </summary>
        Reprojected = 0,
        /// <summary>
        /// 分配
        /// </summary>
        Assigned = 1,
        /// <summary>
        /// 跳过
        /// </summary>
        Skipped = 2,
        /// <summary>
        /// 不存在
        /// </summary>
        AbsenceIgnored = 3,
        /// <summary>
        /// 不匹配
        /// </summary>
        MismatchIgnored = 4,
        /// <summary>
        /// 替换
        /// </summary>
        Substituted = 5,
        /// <summary>
        /// 投影失败
        /// </summary>
        FailedToReproject = 6,
        /// <summary>
        /// 相同投影
        /// </summary>
        SameProjection = 7,
    }

    /// <summary>
    /// 类型预测报告
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// 加载
        /// </summary>
        Loading = 0,
        /// <summary>
        /// 分配
        /// </summary>
        Assignment = 1,
    }

    public partial class frmTesterReport : Form, MapWinGIS.ICallback
    {
        #region 声明
        //数据网格视图列
        private const int CMN_FILENAME = 0;//文件名
        private const int CMN_PROJECTION = 1;//投影
        private const int CMN_OPERATION = 2;//操作
        private const int CMN_NEW_NAME = 3;//新文件名
        private const int CMN_ERROR = 4;//错误
        #endregion

        /// <summary>
        /// 创建一个新的frmTesterReport类实例
        /// </summary>
        public frmTesterReport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获得不匹配的文件数
        /// </summary>
        public int MismatchedCount
        {
            get
            {
                return listView1.Items.Count;
            }
        }

        /// <summary>
        /// 显示报告窗口为非模态窗口
        /// 初始化进度
        /// </summary>
        /// <param name="proj">投影的目标工程</param>
        public void InitProgress(MapWinGIS.GeoProjection proj)
        {
            this.Text = "重新投影";
            this.label1.Text = "进行文件重新投影";
            this.SetProjectProjection(proj, ReportType.Loading);
            this.lblProjection.Visible = false;
            this.Show();
        }

        /// <summary>
        /// 显示报告窗口为模态窗口
        /// </summary>
        /// <param name="proj">投影的目标工程</param>
        /// <param name="message"></param>
        /// <param name="type">报告类型</param>
        public void ShowReport(MapWinGIS.GeoProjection proj, string message, ReportType type)
        {
            this.Text = type == ReportType.Loading ? "投影检查结果" : "投影分配结果";
            this.SetProjectProjection(proj, type);
            this.ShowReportCore(proj, message);
            this.ShowDialog();
        }

        /// <summary>
        /// 执行相同行动的报告显示，它不依赖于报告的类型（层加载或分配投影）
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="message"></param>
        private void ShowReportCore(MapWinGIS.GeoProjection proj, string message)
        {
            if (message == "")
            {
                message = "下列文件是由于投影的不匹配或不存在的影响:";
            }

            this.label1.Text = message;
            this.lblProjection.Visible = true;
            this.lblFile.Visible = false;
            this.progressBar1.Visible = false;
            this.button1.Visible = true;
            if (this.Visible)
            {
                this.Visible = false;
            }
        }

        /// <summary>
        /// 投影显示的项目
        /// </summary>
        /// <param name="proj">投影的目标工程</param>
        /// <param name="reportType"></param>
        private void SetProjectProjection(MapWinGIS.GeoProjection proj, ReportType reportType)
        {
            string suffix = "目标投影: ";

            if (proj == null || proj.IsEmpty)//投影目标为空
            {
                lblProjection.Text = suffix + "没有定义";
            }
            else
            {
                lblProjection.Text = suffix + proj.Name;
            }
        }

        /// <summary>
        /// 添加新的文件到报告中
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="projection">投影</param>
        /// <param name="operation">投影操作</param>
        /// <param name="newName">新文件名</param>
        public void AddFile(string filename, string projection, ProjectionOperaion operation, string newName)
        {
            string s = operation.ToString();
            switch (operation)
            {
                case ProjectionOperaion.AbsenceIgnored:
                    s = "不存在";
                    break;
                case ProjectionOperaion.MismatchIgnored:
                    s = "不匹配";
                    break;
                case ProjectionOperaion.FailedToReproject:
                    s = "投影失败";
                    break;
            }

            //将文件添加到ListView中
            ListViewItem item = listView1.Items.Add(System.IO.Path.GetFileName(filename));//添加文件名及路径
            item.SubItems.Add(projection == "" ? "none" : projection);//添加投影
            item.SubItems.Add(s);//添加操作方式
            item.SubItems.Add(System.IO.Path.GetFileName(newName));//添加新文件名

            if (operation == ProjectionOperaion.Skipped || operation == ProjectionOperaion.FailedToReproject)//跳过投影或投影失败
            {
                MapWinGIS.GlobalSettings settings = new MapWinGIS.GlobalSettings();
                item.SubItems.Add(settings.GdalReprojectionErrorMsg);
            }
            else
            {
                item.SubItems.Add("");
            }
            listView1.Refresh();
            Globals.AutoResizeColumns(this.listView1);//自动调整列宽
            Application.DoEvents();
        }

        #region ICallback 成员
        /// <summary>
        /// 显示文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        public void ShowFilename(string filename)
        {
            lblFile.Text = "文件: " + filename;
            lblFile.Visible = true;
            this.progressBar1.Visible = true;
            this.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// 清除文件名
        /// </summary>
        public void ClearFilename()
        {
            lblFile.Visible = false;
            this.progressBar1.Visible = false;
        }

        /// <summary>
        /// 进度
        /// </summary>
        /// <param name="KeyOfSender"></param>
        /// <param name="Percent">进度百分比</param>
        /// <param name="Message"></param>
        public void Progress(string KeyOfSender, int Percent, string Message)
        {
            this.progressBar1.Value = Percent;
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="KeyOfSender"></param>
        /// <param name="ErrorMsg"></param>
        public void Error(string KeyOfSender, string ErrorMsg)
        {
            //不做任何事
        }
        #endregion
    }
}
