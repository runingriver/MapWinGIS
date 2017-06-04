/****************************************************************************
 * 文件名:clsStatusBarItem.cs (F)
 * 描  述: 该类提供一个状态栏的子对象，并提供该对象的基本信息。
 * **************************************************************************/

using System;

namespace MapWinGIS.MainProgram
{
    public class StatusBarItem : MapWinGIS.Interfaces.StatusBarItem
    {
        internal System.Windows.Forms.ToolStripItem m_Item = null;

        public StatusBarItem(System.Windows.Forms.ToolStripItem item)
        {
            if (item == null)
            {
                throw new ArgumentException("没有指向ToolStripItem的引用。");
            }
            this.m_Item = item;
        }


        #region StatusBarItem接口实现

        /// <summary>
        /// 文本对齐
        /// </summary>
        public MapWinGIS.Interfaces.eAlignment Alignment
        {
            get
            {
                    // 此处将HorizontalAlignment改为ToolStripItemAlignment
                    if (this.m_Item.Alignment == System.Windows.Forms.ToolStripItemAlignment.Left)
                    {
                        return MapWinGIS.Interfaces.eAlignment.Left;
                    }
                    else
                    {
                        return MapWinGIS.Interfaces.eAlignment.Right;
                    }
            }
            set
            {
                try
                {
                    this.m_Item.Alignment = (System.Windows.Forms.ToolStripItemAlignment)value;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 自动调整大小
        /// </summary>
        public bool AutoSize
        {
            get
            {
                try
                {
                    return this.m_Item.AutoSize;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return false;
                }
            }
            set
            {
                try
                {
                    this.m_Item.AutoSize = value;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 设置最小宽度
        /// </summary>
        public int MinWidth
        {
            get
            {
                try
                {
                    return this.m_Item.Width;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return 0;
                }
            }
            set
            {
                try
                {
                    this.m_Item.Width = value;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 子条目的文本
        /// </summary>
        public string Text
        {
            get
            {
                try
                {
                    return this.m_Item.Text;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return "";
                }
            }
            set
            {
                try
                {
                    this.m_Item.Text = value;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// 设置宽度
        /// </summary>
        public int Width
        {
            get
            {
                try
                {
                    return this.m_Item.Width;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                    return 0;
                }
            }
            set
            {
                try
                {
                    this.m_Item.Width = value;
                }
                catch (Exception ex)
                {
                    Program.g_error = ex.ToString();
                    Program.ShowError(ex);
                }
            }
        }

        #endregion

    }
}
