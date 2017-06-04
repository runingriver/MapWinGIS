using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public partial class ErrorDialog : Form
    {
        private static System.Resources.ResourceManager resources 
            = new System.Resources.ResourceManager("MapWinGIS.MainProgram.GlobalResource", System.Reflection.Assembly.GetExecutingAssembly());
        
        private Exception m_exception;
        private string m_Email = "";  

        public ErrorDialog(System.Exception ex, string SendNextToEmail) : base() //可以不写，默认调用它
        {
            InitializeComponent();
            this.Icon = MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;

            m_exception = ex;
            m_Email = SendNextToEmail;

            MapWinGIS.Utility.Logger.Dbg("没有处理的异常 " + ex.ToString());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmErrorDialogMoreInfo moreInfo = new frmErrorDialogMoreInfo();
            moreInfo.txtFullText.Text = Program.appInfo.ApplicationName + " " + App.VersionString + " (" + System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToShortDateString() + ")" + "\r\n" + "\r\n" + m_exception.ToString() + "\r\n" + "\r\n" + MapWinGIS.Utility.MiscUtils.GetDebugInfo();
            moreInfo.Owner = this;
            moreInfo.Show();
            moreInfo.BringToFront();
        }

        private void chkNoReport_CheckedChanged(object sender, EventArgs e)
        {
            Program.projInfo.NoPromptToSendErrors = chkNoReport.Checked;
            Program.projInfo.SaveConfig(true, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (m_exception != null)
                {
                    if (m_Email == null)
                    {
                        m_Email = "";
                    }
                    //post包括版本，异常信息，debug信息
                    string post = "prog=" + System.Web.HttpUtility.UrlEncode("MapWinGIS " + App.VersionString + " (" 
                        + System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToShortDateString() + ")") 
                        + "&ex=" + System.Web.HttpUtility.UrlEncode(m_exception.ToString()) 
                        + "&fromemail=" + System.Web.HttpUtility.UrlEncode(txtEMail.Text) 
                        + "&adtnl=" + System.Web.HttpUtility.UrlEncode(MapWinGIS.Utility.MiscUtils.GetDebugInfo()) 
                        + "&comments=" + System.Web.HttpUtility.UrlEncode(txtComments.Text) 
                        + "&copy=" + System.Web.HttpUtility.UrlEncode(m_Email);
                    try
                    {
                        MapWinGIS.Utility.NetOperator.ExecuteUrl("http://www.baidu.com/", post, true);
                    }
                    catch (Exception ex)
                    {
                        MapWinGIS.Utility.Logger.Dbg("DEBUG: " + ex.ToString());
                    }
                }
            }
            catch
            {
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            try
            {
                string msg = resources.GetString("msgDataSubmitted_Text");
                if (msg.Trim() == "")
                {
                    msg = "谢谢，您的数据提交成功！";
                }
                MapWinGIS.Utility.Logger.Msg(msg,Program.appInfo.ApplicationName, MessageBoxIcon.Information);
            }
            catch
            {
                MapWinGIS.Utility.Logger.Msg("谢谢，您的数据提交成功！", "完成", MessageBoxIcon.Information);
            }
            this.Close();
        }

        //您的问题提交到这里...
        private void lblAltLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.baidu.com");
            }
            catch
            { }
        }
    }
}
