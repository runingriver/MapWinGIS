using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapWinGIS.Interfaces;

namespace MapWinGIS.MainProgram
{
    public partial class StatusBar : Form, MapWinGIS.Interfaces.StatusBar
    {
        public StatusBar()
        {
            InitializeComponent();
            this.Controls.Remove(this.StatusBar1);
        }

        #region 同步显示地图事件
        /// <summary>
        /// 投影改变时，修改状态栏名字改
        /// </summary>
        internal void HandleProjectionChanged(MapWinGIS.GeoProjection oldProjection, MapWinGIS.GeoProjection newProjection)
        {
            this.StatusBarPanelProjection.Text = !newProjection.IsEmpty ? newProjection.Name : "Not defined";
        }

        /// <summary>
        /// 当地图Extent改变时，将当前比例添加到状态栏
        /// </summary>
        internal void HandleExtentsChanged(System.Object sender, System.EventArgs e)
        {
            ToolStripStatusLabel scalePanel = (ToolStripStatusLabel)StatusBar1.Items["StatusBarPanelScale"];
            string scale = Program.frmMain.View.Scale.ToString(System.Globalization.CultureInfo.InvariantCulture);
            scalePanel.Text = "1:" + Math.Round(double.Parse(scale, System.Globalization.CultureInfo.InvariantCulture)).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 未实现
        /// 处理地图的MouseMove事件
        /// </summary>
        internal void HandleMapMouseMove(System.Object sender, AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        {
        }

        /// <summary>
        /// 格式化在状态栏坐标系显示格式
        /// </summary>
        private string FormatCoords(double x, double y, int decimals, string useCommas, string units)
        {
            string nf ; //the number formatting string

            if (useCommas == "true")
            {
                nf = "N" + decimals.ToString();
            }
            else
            {
                nf = "F" + decimals.ToString();
            }
            if (units == "Lat/Long")
            {
                return string.Format("Lat: {0} Long: {1}", y.ToString(nf), x.ToString(nf));
            }
            else
            {
                return string.Format("X: {0} Y: {1} {2}", x.ToString(nf), y.ToString(nf), units);
            }

        }
        
        #endregion


        #region StatusStrip事件处理

        private void StatusBar1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == StatusBar1.Items["StatusBarPanelScale"])
            {
                // 弹出设置比例的对话框
                Program.frmMain.DoSetScale();
            }
        }

        #endregion

        #region StatusBar接口实现

        /// <summary>
        /// 状态栏是否可用
        /// </summary>
        public new bool Enabled
        {
            get { return StatusBar1.Enabled; }
            set { StatusBar1.Enabled = value; }           
        }

        /// <summary>
        /// 状态栏的进展条是否显示
        /// </summary>
        public bool ShowProgressBar
        {
            get { return ProgressBar1.Visible; }
            set 
            {
                ProgressBar1.Visible = value;
                if (!value)
                {
                    this.StatusBarPanelStatus.Text = "";
                }
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 进度条的值
        /// </summary>
        public int ProgressBarValue
        {
            get { return this.ProgressBar1.Value; }
            set
            {
                if (value > 100)
                {
                    value = 100;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                if (value > 0)
                {
                    if (!this.ProgressBar1.Visible)
                    {
                        this.ProgressBar1.Visible = true;
                    }
                }
                else
                {
                    this.ProgressBar1.Visible = false;
                }

                ProgressBar1.Value = value;
                try
                {
                    Application.DoEvents();
                }
                catch
                { }

            }
        }

        /// <summary>
        /// 添加一个panel到状态栏，但是这个功能被去掉了。
        /// 可以用AddPanel（）覆盖
        /// </summary>
        /// <returns>返回添加的panel条目</returns>
        public MapWinGIS.Interfaces.StatusBarItem AddPanel()
        {
            return this.AddPanel(this.StatusBar1.Items.Count);
        }

        /// <summary>
        /// 添加一个panel到状态栏，但是这个功能被去掉了。
        /// 可以用AddPanel（）覆盖
        /// </summary>
        /// <param name="insertAt">插入位置</param>
        /// <returns> 被添加的panel</returns>
        public MapWinGIS.Interfaces.StatusBarItem AddPanel(int insertAt)
        {
            try
            {
                if (insertAt <= 0)
                {
                    insertAt = 0;
                }
                if (insertAt > this.StatusBar1.Items.Count)
                {
                    insertAt = this.StatusBar1.Items.Count;
                }

                System.Windows.Forms.ToolStripStatusLabel newPanel = new System.Windows.Forms.ToolStripStatusLabel();
 
                StatusBar1.Items.Insert(insertAt, newPanel);

                MapWinGIS.MainProgram.StatusBarItem newItem = new MapWinGIS.MainProgram.StatusBarItem(newPanel);
                return newItem;
            }
            catch (Exception ex)
            {
                throw (new Exception("添加StatusBar Panel失败." + "\r\n" + ex.ToString()));
            }
        }

        /// <summary>
        /// 添加一个panel的首选方法
        /// </summary>
        /// <param name="text">显示在panel上的文字</param>
        /// <param name="position">插入位置</param>
        /// <param name="width">宽度</param>
        /// <param name="autoSize">是否AutoSize</param>
        /// <returns>StatusBarPanel对象.</returns>
        [System.Obsolete("过时的方法！", true)]
        public System.Windows.Forms.StatusBarPanel AddPanel(string text, int position, int width, System.Windows.Forms.StatusBarPanelAutoSize autoSize)
        {
            return null;
        }

        /// <summary>
        /// 按照索引的方式移除panel，但是必须存在一个panel
        /// </summary>
        /// <param name="index">以零为索引</param>
        public void RemovePanel(int index)
        {
            try
            {
                if (StatusBar1.Items.Count > index)
                {
                    StatusBar1.Items.RemoveAt(index);
                }
                if (this.NumPanels == 0)
                {
                    MapWinGIS.Interfaces.StatusBarItem item = this.AddPanel();
                }
            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }

        /// <summary>
        /// 以指定panel对象的方式移除.  但是必须存在一个panel
        /// </summary>
        /// <param name="panel"><c>StatusBarPanel</c> to remove.</param>
        [System.Obsolete("过时的方法！", true)]
        public void RemovePanel(ref System.Windows.Forms.StatusBarPanel panel)
        { }

        /// <summary>
        /// 移除指定的状态栏中的条目
        /// </summary>
        public void RemovePanel(ref MapWinGIS.Interfaces.StatusBarItem panel)
        {
            if (panel == null)
            {
                return;
            }
            MapWinGIS.MainProgram.StatusBarItem item = panel as MapWinGIS.MainProgram.StatusBarItem;
            if (item != null)
            {
                for (int i = 0; i < this.StatusBar1.Items.Count ; i++)
                {
                    if (item.m_Item == this.StatusBar1.Items[i])
                    {
                        this.RemovePanel(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">Index of the StatusBarItem to retrieve</param>
        public MapWinGIS.Interfaces.StatusBarItem this[int index]
        {
            get
            {
                if (index < 0 || index >= StatusBar1.Items.Count)
                {
                    return null;
                }
                return new MapWinGIS.MainProgram.StatusBarItem(this.StatusBar1.Items[index]);
            }
        }

        /// <summary>
        /// 状态栏上panel的数量
        /// </summary>
        /// <returns>Number of panels in the <c>StatusBar</c>.</returns>
        public int NumPanels
        {
            get { return StatusBar1.Items.Count; }
        }

        /// <summary>
        /// 使得进度条合适的显示在状态栏中.
        /// 在任何你要改变状态栏傻瓜任何panel尺寸的地方调用.
        /// 但是不需要再AddPanel和RemovePanel的地方调用.
        /// </summary>
        [System.Obsolete("不在需要调整进度条的大小！")]
        public void ResizeProgressBar()
        { }

        /// <summary>
        /// 刷新状态栏
        /// </summary>
        public new void Refresh()
        {
            this.StatusBar1.Refresh();
        }

        #endregion

        #region 投影
        //投影的按钮（未定义）按下
        private void StatusBarPanelProjection_ButtonClick(object sender, EventArgs e)
        {
            if (Program.frmMain.Project.GeoProjection == null || Program.frmMain.Project.GeoProjection.IsEmpty)
            {
                this.ChooseProjection();
            }
            else
            {
                this.ShowProperties();
            }
        }

        //显示投影选择对话框
        private void ChooseProjection()
        {
            Program.frmMain.m_Project.SetProjectProjectionByDialog();
        }

        /// <summary>
        /// 显示选择的投影的属性 未实现
        /// </summary>
        private void ShowProperties()
        {
            // 如果有投影，则查看其属性 
            /****注释，需要 MapWindow.Controls.Projections****/
            //CoordinateSystem cs = Program.ProjectionDB.GetCoordinateSystem(Program.frmMain.Project.GeoProjection, ProjectionSearchType.UseDialects);
            //if (cs != null)
            //{
            //    frmProjectionProperties form = new frmProjectionProperties(cs, (MapWindow.Controls.Projections.ProjectionDatabase)(Program.frmMain.ProjectionDatabase));
            //    form.ShowDialog();
            //    form.Dispose();
            //}
            //else
            //{
            //    MapWinGIS.GeoProjection proj = Program.frmMain.Project.GeoProjection;
            //    if (!proj.IsEmpty)
            //    {
            //        frmProjectionProperties form = new frmProjectionProperties(proj);
            //        form.ShowDialog();
            //        form.Dispose();
            //    }
            //    else
            //    {
            //        MessageBox.Show("没有投影的相关信息", Program.frmMain.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
        }


        //投影按钮下拉按钮按下
        private void StatusBarPanelProjection_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == btnChoose.Name) //选择投影
            {
                this.ChooseProjection();
            }
            else if (e.ClickedItem.Name == btnProperties.Name) //属性
            {
                this.ShowProperties();
            }
            else if (e.ClickedItem.Name == btnShowWarnings.Name) //显示警告
            {
                btnShowWarnings.Checked = !btnShowWarnings.Checked;
                Program.appInfo.NeverShowProjectionDialog = !btnShowWarnings.Checked;
            }
            else if (e.ClickedItem.Name == btnShowReport.Name) //显示加载报告
            {
                btnShowReport.Checked = !btnShowReport.Checked;
                Program.appInfo.ShowLoadingReport = btnShowReport.Checked;
            }
        }

        //附在投影下拉按钮的右键菜单
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            btnShowWarnings.Checked = !Program.appInfo.NeverShowProjectionDialog;
            btnShowReport.Checked = Program.appInfo.ShowLoadingReport;
            this.SetAbsenceBehavior(Program.appInfo.ProjectionAbsenceBehavior);
            this.SetMismatchBehavior(Program.appInfo.ProjectionMismatchBehavior);
        }

        //设置缺省行为
        private void SetAbsenceBehavior(ProjectionAbsenceBehavior behavior)
        {
            btnAbsenceAssign.Checked = (behavior == ProjectionAbsenceBehavior.AssignFromProject);
            btnAbsenceIgnore.Checked = (behavior == ProjectionAbsenceBehavior.IgnoreAbsence);
            btnAbsenceSkip.Checked = (behavior == ProjectionAbsenceBehavior.SkipFile);
        }

        //设置不匹配行为
        private void SetMismatchBehavior(ProjectionMismatchBehavior behavior)
        {
            btnMismatchIgnore.Checked = (behavior == ProjectionMismatchBehavior.IgnoreMismatch);
            btnMismatchProjectOld.Checked = (behavior == ProjectionMismatchBehavior.Reproject);
            btnMismatchSkip.Checked = (behavior == ProjectionMismatchBehavior.SkipFile);
        }


        //缺省行为的子项被点击
        private void btnAbsenceBehavior_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == btnAbsenceAssign.Name)
            {
                Program.appInfo.ProjectionAbsenceBehavior = ProjectionAbsenceBehavior.AssignFromProject;
                this.SetAbsenceBehavior(Program.appInfo.ProjectionAbsenceBehavior);//标识选择状态
            }
            else if (e.ClickedItem.Name == btnAbsenceIgnore.Name)
            {
                Program.appInfo.ProjectionAbsenceBehavior = ProjectionAbsenceBehavior.IgnoreAbsence;
                this.SetAbsenceBehavior(Program.appInfo.ProjectionAbsenceBehavior);
            }
            else if (e.ClickedItem.Name == btnAbsenceSkip.Name)
            {
                Program.appInfo.ProjectionAbsenceBehavior = ProjectionAbsenceBehavior.SkipFile;
                this.SetAbsenceBehavior(Program.appInfo.ProjectionAbsenceBehavior);
            }
        }

        //不匹配行为的子项被点击
        private void btnMismatchBehavior_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == btnMismatchBehavior.Name)
            {
                Program.appInfo.ProjectionMismatchBehavior = ProjectionMismatchBehavior.IgnoreMismatch;
                this.SetMismatchBehavior(Program.appInfo.ProjectionMismatchBehavior);
            }
            else if (e.ClickedItem.Name == btnMismatchProjectOld.Name)
            {
                Program.appInfo.ProjectionMismatchBehavior = ProjectionMismatchBehavior.Reproject;
                this.SetMismatchBehavior(Program.appInfo.ProjectionMismatchBehavior);
            }
            else if (e.ClickedItem.Name == btnMismatchSkip.Name)
            {
                Program.appInfo.ProjectionMismatchBehavior = ProjectionMismatchBehavior.SkipFile;
                this.SetMismatchBehavior(Program.appInfo.ProjectionMismatchBehavior);
            }
        }

        #endregion

        /// <summary>
        /// 显示进度条任务信息
        /// </summary>
        internal void ShowMessage(string message)
        {
            this.StatusBarPanelStatus.Text = message;
        }
  

    }
}
