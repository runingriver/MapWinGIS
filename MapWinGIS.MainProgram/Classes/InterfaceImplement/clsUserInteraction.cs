using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.MainProgram
{
    public class UserInteraction : MapWinGIS.Interfaces.UserInteraction
    {
        /// <summary>
        /// 让用户选择一个项目，并且返回一个PROJ4类型文件替代这个加载项目
        /// 指定对话框的Caption和项目默认选项
        /// </summary>
        /// <param name="dialogCaption">选择投影对话框的标题</param>
        /// <param name="defaultProjection">默认投影，""表示无</param>
        /// <returns></returns>
        public string GetProjectionFromUser(string dialogCaption, string defaultProjection)
        {
            return Program.frmMain.GetProjectionFromUser(dialogCaption, defaultProjection);
        }

        /// <summary>
        /// 由用户定义的开始和结束色
        /// </summary>
        /// <param name="suggestedStart">初始化默认选择的开始颜色</param>
        /// <param name="suggestedEnd">初始化默认选择的结束颜色</param>
        /// <param name="selectedEnd">用户选择的结束色</param>
        /// <param name="selectedStart">用户选择的开始色</param>
        /// <returns></returns>
        public bool GetColorRamp(System.Drawing.Color suggestedStart, System.Drawing.Color suggestedEnd, out System.Drawing.Color selectedStart, out System.Drawing.Color selectedEnd)
        {
            ColorPicker dlg = new ColorPicker(suggestedStart, suggestedEnd);
            selectedStart = suggestedStart;
            selectedEnd = suggestedEnd;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedStart = dlg.btnStartColor.BackColor;
                selectedEnd = dlg.btnEndColor.BackColor;
                return true;
            }
            return false;
        }

    }
}
