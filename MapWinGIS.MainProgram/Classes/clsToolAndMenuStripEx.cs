/****************************************************************************
 * 文件名:clsToolAndMenuStrip.cs
 * 描  述:基础ToolStrip和MenuStrip控件类，实现自定义功能
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace MapWinGIS.ToolStripExtensions
{
    /*******************************************************************************************************************
     * 扩展TooStrip和MenuStrip类是为了能够实现允许用户去自定义接口，实现某些特定功能
     * 以下几个bool型属性将会显示在设计器中
     * ClickThrough - 允许第一次点击激活控件，即使Form是没有激活。
     * SupressHighlighting - ，当包含在form中的控件没有激活时，防止当鼠标over过控件时显示高亮
     * ****************************************************************************************************************/


    /// <summary>
    /// 修改菜单栏(MenuStrip)的两个功能： ClickThrough、SuppressHighlighting
    /// MenuStrip是继承自ToolStrip
    /// </summary>
    [DesignerCategory("")]
    public class MenuStripEx : MenuStrip
    {
        private bool m_clickThrough = false;
        private bool m_suppressHighlighting = true;

        /// <summary>
        /// 获取设置，当窗体没有给定焦点时，菜单项是否被点击
        /// 默认false，即保持和原ToolStrip类一致
        /// </summary>
        [Description("当窗体没有给定焦点时，菜单项是否被点击")]
        public bool ClickThrough
        {
            get
            {
                return this.m_clickThrough;
            }
            set
            {
                this.m_clickThrough = value;
            }
        }

        /// <summary>
        /// 获取设置，当MouseOver时，控件是否显示高亮
        /// 默认true，即保持和原Menustrip类一致
        /// </summary>
        [Description("当MouseOver时，控件是否显示高亮")]
        public bool SuppressHighlighting
        {
            get
            {
                return this.m_suppressHighlighting;
            }
            set
            {
                this.m_suppressHighlighting = value;
            }
        }

        /// <summary>
        /// 此方法重写基类中的WndProc方法处理windows消息 
        /// 拦截WM_MOUSEMOVE消息，并且当SuppressHighlighting为true和TopLevelControl没有获得焦点时忽略事件处理。
        /// 否则，调用基类的方法来处理这个消息
        /// 拦截WM_MOUSEACTIVATE消息，并且当ClickThrough为true时，将"Activate and Eat"替换为 "Activate"
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            // 如果我们不想高亮，移除mouseover命令即可
            // 父窗体或者它的一个子对象没有获得焦点时，不高亮
            if (m.Msg == WinConst.WM_MOUSEMOVE && this.m_suppressHighlighting && !this.TopLevelControl.ContainsFocus)
            {
                return;
            }
            else
            {
                base.WndProc(ref m);
            }

            //如果想ClickThrough，在WM_MOUSEACTIVATE消息上用Activate替换Activate and Eat
            if (m.Msg == WinConst.WM_MOUSEACTIVATE && this.m_clickThrough && m.Result == (IntPtr)WinConst.MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)(WinConst.MA_ACTIVATE);
            }
        }
    }

    /// <summary>
    /// 此类在ToolStrip基础上添加一些新功能
    /// 通过ClickThrough可以让你不需给form一个焦点，并且实现想要的行为
    /// 实现了SuppressHighlighting属性，当窗体没激活时，关闭高亮
    /// </summary>
    [DesignerCategory("")]
    public class ToolStripEx : ToolStrip
    {
        private bool m_clickThrough = false;
        private bool m_suppressHighlighting = true;

        /// <summary>
        /// 获取或设置，当窗体没有给定焦点时，菜单项是否被点击
        /// 默认false，即保持和原ToolStrip类一致
        /// </summary>
        [Description("当窗体没有给定焦点时，菜单项是否被点击")]
        public bool ClickThrough
        {
            get
            {
                return this.m_clickThrough;
            }
            set
            {
                this.m_clickThrough = value;
            }
        }

        /// <summary>
        /// 获取或设置，当MouseOver时，控件是否显示高亮
        /// 默认true，即保持和原Menustrip类一致
        /// </summary>
        [Description("当MouseOver时，控件是否显示高亮")]
        public bool SuppressHighlighting
        {
            get
            {
                return this.m_suppressHighlighting;
            }
            set
            {
                this.m_suppressHighlighting = value;
            }
        }

        /// <summary>
        /// 此方法重写基类中的WndProc方法处理windows消息 
        /// 拦截WM_MOUSEMOVE消息，并且当SuppressHighlighting为true和TopLevelControl没有获得焦点时忽略事件处理。
        /// 否则，调用基类的方法来处理这个消息
        /// 拦截WM_MOUSEACTIVATE消息，并且当ClickThrough为true时，将"Activate and Eat"替换为 "Activate"
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinConst.WM_MOUSEMOVE && this.m_suppressHighlighting && this.TopLevelControl != null && !this.TopLevelControl.ContainsFocus)
            {
                return;
            }
            else
            {
                base.WndProc(ref m);
            }

            if (m.Msg == WinConst.WM_MOUSEACTIVATE && this.m_clickThrough && m.Result == (IntPtr)WinConst.MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)WinConst.MA_ACTIVATE;
            }
        }
    }

    /// <summary>
    /// 定义窗口信息常量
    /// </summary>
    public class WinConst
    {
        public const uint WM_MOUSEMOVE = 0x200;
        public const uint WM_MOUSEACTIVATE = 0x21;
        public const uint MA_ACTIVATE = 1;
        public const uint MA_ACTIVATEANDEAT = 2;
        public const uint MA_NOACTIVATE = 3;
        public const uint MA_NOACTIVATEANDEAT = 4;
    }
}
