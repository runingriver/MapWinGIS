/****************************************************************************
 * 文件名:clsComboBoxItem.cs (F)
 * 描  述:提供一个下拉列表框对象及其相关的信息.
 * **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public class ComboBoxItem : MapWinGIS.Interfaces.ComboBoxItem
    {
        private ToolStripComboBox m_Box;
        private string m_Description;
        private string m_Tooltip;

        public ComboBoxItem(ToolStripComboBox comboBox)
        {
            m_Box = comboBox;
        }

        #region 接口实现

        /// <summary>
        /// 鼠标
        /// </summary>
        public System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return System.Windows.Forms.Cursors.Default;
            }
            set 
            {
                return;
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
        /// 下拉列表框的下拉的样式
        /// </summary>
        public System.Windows.Forms.ComboBoxStyle DropDownStyle
        {
            get
            {
                return this.m_Box.DropDownStyle;
            }
            set 
            {
                this.m_Box.DropDownStyle = value;
            }
        }

        /// <summary>
        /// 获取或设置下拉列表框是否启用
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_Box.Enabled;
            }
            set 
            {
                m_Box.Enabled = value;
            }
        }

        /// <summary>
        /// 下拉列表框的名字
        /// </summary>
        public string Name
        {
            get
            {
                return m_Box.Name;
            }
        }

        /// <summary>
        /// 选择的以零开始的索引的条目
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return m_Box.SelectedIndex;
            }
            set 
            {
                this.m_Box.SelectedIndex = value;
            }
        }

        /// <summary>
        /// 选择的对象
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return m_Box.SelectedItem;
            }
            set 
            {
                m_Box.SelectedItem = value;
            }
        }

        /// <summary>
        /// 选择的文本
        /// </summary>
        public string SelectedText
        {
            get
            {
                return m_Box.SelectedText;
            }
            set 
            {
                m_Box.SelectedText = value;
            }
        }

        /// <summary>
        /// 选择的高亮文本的长度
        /// </summary>
        public int SelectionLength
        {
            get
            {
                return m_Box.SelectionLength;
            }
            set 
            {
                m_Box.SelectionLength = value;
            }
        }
        /// <summary>
        /// 高亮文本的开始索引
        /// </summary>
        public int SelectionStart
        {
            get
            {
                return m_Box.SelectionStart;
            }
            set 
            {
                m_Box.SelectionStart = value;
            }
        }

        /// <summary>
        /// 这个对象的文本
        /// </summary>
        public string Text
        {
            get
            {
                return m_Box.Text;
            }
            set 
            {
                m_Box.Text = value;
            }
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        public string Tooltip
        {
            get
            {
                return this.m_Tooltip;
            }
            set 
            {
                this.m_Tooltip = value;
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width
        {
            get
            {
                return m_Box.Width;
            }
            set
            {
                m_Box.Width = value;
            }
        }

        /// <summary>
        /// 返回下拉列表框的子项的集合
        /// </summary>
        public System.Windows.Forms.ComboBox.ObjectCollection Items()
        {
            return m_Box.Items;
        }

        #endregion

    }
}
