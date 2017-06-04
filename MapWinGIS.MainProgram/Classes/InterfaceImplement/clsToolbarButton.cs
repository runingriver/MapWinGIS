/****************************************************************************
 * 文件名:clsToolbarButton.cs (F)
 * 描  述: 该类提供一个工具条按钮对象及其相关的信息
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public class ToolbarButton : MapWinGIS.Interfaces.ToolbarButton
    {
        private System.Windows.Forms.ToolStripItem m_Button;
        private object m_Picture;

        private bool m_BeginsGroup;
        private System.Windows.Forms.Cursor m_Cursor;
        private string m_Description;
        private string m_Category;
        private bool m_Displayed;
        private string m_Name;

        public ToolbarButton(System.Windows.Forms.ToolStripButton button)
        {
            m_Button = button;
        }

        public ToolbarButton(System.Windows.Forms.ToolStripDropDownButton button)
        {
            m_Button = button; 
        }

        public ToolbarButton(System.Windows.Forms.ToolStripItem button)
        {
            m_Button = button; 
        }

        #region 接口实现

        /// <summary>
        /// 是否按下
        /// </summary>
        public bool Pressed
        {
            get
            {
                if (m_Button is ToolStripButton)
                {
                    return ((ToolStripButton)m_Button).Checked;
                }
                return false;
            }
            set
            {
                if (m_Button is ToolStripButton)
                {
                    ((ToolStripButton)m_Button).Checked = value;
                }
            }
        }

        /// <summary>
        /// 按钮的文本
        /// </summary>
        public string Text
        {
            get
            {
                return m_Button.Text;
            }
            set
            {
                m_Button.Text = value;
            }
        }

        /// <summary>
        /// 按钮图片
        /// </summary>
        public object Picture
        {
            get
            {
                return m_Picture;
            }
            set
            {
                m_Picture = value;

                try
                {
                    if (value is System.Drawing.Icon)
                    {
                        System.Drawing.Image img = ((System.Drawing.Icon)value).ToBitmap();
                        m_Button.Image = img;
                    }
                    else
                    {
                        m_Button.Image = (System.Drawing.Image)value;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置图片是出错，Picture" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 按钮类别
        /// </summary>
        public string Category
        {
            get
            {
                return this.m_Category;
            }
            set
            {
                this.m_Category = value;
            }
        }

        /// <summary>
        /// 按钮提示文本
        /// </summary>
        public string Tooltip
        {
            get
            {
                return m_Button.ToolTipText;
            }
            set
            {
                m_Button.ToolTipText = value;
            }
        }

        /// <summary>
        /// 是否在前面加一竖线，分组
        /// </summary>
        public bool BeginsGroup
        {
            get 
            {
                return this.m_BeginsGroup;
            }
            set
            {
                this.m_BeginsGroup = value;
            }
        }

        /// <summary>
        /// 鼠标
        /// </summary>
        public System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return this.m_Cursor;
            }
            set
            {
                this.m_Cursor = value;
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Displayed
        {
            get
            {
                return this.m_Displayed;
            }
            set
            {
                this.m_Displayed = value;
            }
        }

        /// <summary>
        /// Gets/Sets the enabled state
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_Button.Enabled;
            }
            set
            {
                this.m_Button.Enabled = value;
            }
        }

        /// <summary>
        /// 按钮名字
        /// </summary>
        public string Name
        {
            get
            {
                return m_Button.Tag.ToString();
            }
        }

        /// <summary>
        /// 按钮的可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.m_Button.Visible;
            }
            set
            {
                this.m_Button.Visible = value;
            }
        }

        /// <summary>
        /// 子条目的编号
        /// </summary>
        public int NumSubItems
        {
            get
            {
                if (m_Button is ToolStripDropDownButton)
                {
                    return ((ToolStripDropDownButton)m_Button).DropDownItems.Count;
                }
                return 0;
            }
        }

        public Interfaces.ToolbarButton SubItem(int index)
        {
            if (m_Button is System.Windows.Forms.ToolStripDropDownButton)
            {
                ToolStripDropDownButton ddbutton = (ToolStripDropDownButton)m_Button;
                if (ddbutton.DropDownItems.Count < index && index > 0)
                {
                    return new ToolbarButton((ToolStripButton)ddbutton.DropDownItems[index]);
                }
                else
                {
                    return null;
                }
            }
            return null;

        }

        public Interfaces.ToolbarButton SubItem(string name)
        {
            if (m_Button is ToolStripDropDownButton)
            {
                ToolStripDropDownButton ddbutton = (ToolStripDropDownButton)m_Button;
                for (int i = 0; i < ddbutton.DropDownItems.Count; i++)
                {
                    if (ddbutton.DropDownItems[i].Name == name)
                    {
                        return new ToolbarButton((ToolStripButton)ddbutton.DropDownItems[i]);
                    }
                }
            }
            return null;

        }


        #endregion
    }
}
