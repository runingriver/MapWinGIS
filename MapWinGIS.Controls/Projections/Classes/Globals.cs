using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.Controls
{
    /// <summary>
    /// 根据标题和内容调整列宽
    /// 也许这是可以做到无需额外的编码，但我还没有找到如何
    /// </summary>
     public static class Globals
    {
        public static void AutoResizeColumns(ListView listView1)
        {
            foreach (ColumnHeader cmn in listView1.Columns)
            {
                int cmnIndex = cmn.Index;
                int maxLength = 0;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    int length = listView1.Items[i].SubItems[cmnIndex].Text.Length;
                    if (length > maxLength)
                    {
                        maxLength = length;
                    }
                }

                ColumnHeaderAutoResizeStyle style = maxLength > cmn.Text.Length ? ColumnHeaderAutoResizeStyle.ColumnContent : ColumnHeaderAutoResizeStyle.HeaderSize;
                cmn.AutoResize(style);
            }
        }

        /// <summary>
        /// 显示带感叹号图标的消息框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption">标题</param>
        public static void MessageBoxExlamation(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// 显示圆圈和小写字母i的消息框
        /// </summary>
        public static void MessageBoxInformation(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}
