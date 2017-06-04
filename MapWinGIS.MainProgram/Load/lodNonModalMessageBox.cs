/****************************************************************************
 * 文件名:lodNonModalMessageBox.cs （F）
 * 描  述:显示一个消息框
 * **************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    /// <summary>
    /// 显示一个指定标题、样式的消息框
    /// 采用多线程，可以使得该窗体和宿主分离
    /// </summary>
    public class NonModalMessageBox
    {
        private string m_Message;
        private MessageBoxButtons m_MsgBoxBtn;
        private string m_Caption;
        private MessageBoxIcon m_MsgBoxIcon;
        public NonModalMessageBox(string message,  string caption, MessageBoxButtons msgBoxBtn, MessageBoxIcon msgBoxIcon = MessageBoxIcon.None)
        {
            m_Message = message;
            m_MsgBoxBtn = msgBoxBtn;
            m_Caption = caption;
            m_MsgBoxIcon = msgBoxIcon;
            Thread thrd = new Thread(new ThreadStart(ShowNonModalMessageBox));
            thrd.Start();
            thrd = null;
        }

        private void ShowNonModalMessageBox()
        {
            MessageBox.Show(m_Message, m_Caption, m_MsgBoxBtn,m_MsgBoxIcon);
        }
    }
    
    public class NonModalMessageBoxVB
    {
        private static string m_Message;
        private static Microsoft.VisualBasic.MsgBoxStyle m_Style;
        private static string m_Caption;

        public NonModalMessageBoxVB(string Message, Microsoft.VisualBasic.MsgBoxStyle msgboxStyle, string Caption)
        {
            m_Message = Message;
            m_Caption = Caption;
            m_Style = msgboxStyle;
            Thread thrd = new Thread(new System.Threading.ThreadStart(doNonModalMessageBox));
            thrd.Start();
            thrd = null;
        }

        private void doNonModalMessageBox()
        {
            Microsoft.VisualBasic.Interaction.MsgBox(m_Message, m_Style, m_Caption);
        }
    }



}
