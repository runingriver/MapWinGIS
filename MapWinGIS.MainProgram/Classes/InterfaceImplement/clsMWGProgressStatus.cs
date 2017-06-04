/****************************************************************************
 * 文件名:clsMWGProgressStatus.cs (F)
 * 描  述: 提供处理进度条的状态、进度更新的类
 * **************************************************************************/

using System;

namespace MapWinGIS.MainProgram
{
    public class MWGProgressStatus : MapWinGIS.Utility.IProgressStatus
    {
        private System.Windows.Forms.Cursor m_OrigCursor;

        /// <summary>
        /// 记录运行时间很长的任务的进度
        /// </summary>
        /// <param name="aCurrentPosition">任务执行的当前位置</param>
        /// <param name="aLastPosition">任务完成时的位置</param>
        public void Progress(int aCurrentPosition, int aLastPosition)
        {
            if (aCurrentPosition >= aLastPosition) //到达了进度条的最后，停止显示进度条
            {
                Program.frmMain.m_StatusBar.ProgressBarValue = aLastPosition;
                Program.frmMain.m_StatusBar.ShowProgressBar = false;
                if (m_OrigCursor != null)
                {
                    Program.frmMain.Cursor = m_OrigCursor;
                }
                else
                {
                    Program.frmMain.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
            else // 长时间任务仍在执行，设置进度条值
            {
                try
                {
                    if (!Program.frmMain.m_StatusBar.ShowProgressBar || m_OrigCursor == null)
                    {
                        m_OrigCursor = Program.frmMain.Cursor; //保存鼠标样式，当进度条开始时使用
                        Program.frmMain.m_StatusBar.ShowProgressBar = true;
                    }
                    Program.frmMain.m_StatusBar.ProgressBarValue = (100 * aCurrentPosition) / aLastPosition;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                }
                catch
                {
                    throw new Exception("在更新进度条时出现异常！");
                }
 
            }
        }

        /// <summary>
        /// 更新当前的状态信息
        /// </summary>
        /// <param name="statusMessage">显示当前进度条的状态</param>
        public void Status(string statusMessage)
        {
            Program.frmMain.m_StatusBar.ShowMessage(statusMessage);
        }
    }
}
