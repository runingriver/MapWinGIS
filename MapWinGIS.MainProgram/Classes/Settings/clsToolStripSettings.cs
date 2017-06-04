using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MapWinGIS.MainProgram
{
    [SettingsProvider("System.Configuration.LocalFileSettingsProvider")]
    [SettingsGroupName("System.Windows.Forms.ToolStripSettings.MapWinGIS.MainProgram.MapWinForm")]
    sealed class ToolStripSettings : ApplicationSettingsBase
    {
        public ToolStripSettings(string settingsKey) : base(settingsKey)//传过来的是toolstrip的Name属性
        {
        }
        [UserScopedSetting()]
        public System.Drawing.Point Location
        {
            get
            {
                if (this["Location"] == null)
                {
                    if (this.GetPreviousVersion("Location") == null)
                    {
                        return new System.Drawing.Point(-1, -1);
                    }

                    return ((System.Drawing.Point)(this.GetPreviousVersion("Location")));
                }

                return ((System.Drawing.Point)(this["Location"]));
            }
            set
            {
                this["Location"] = value;
            }
        }

        [UserScopedSetting(), DefaultSettingValue("StripDocker.Top")]
        public string ToolStripPanelName
        {
            get
            {
                if (string.IsNullOrEmpty((string)(this["ToolStripPanelName"])))
                {
                    // 设置早期设置的值
                    if (string.IsNullOrEmpty((string)(this.GetPreviousVersion("ToolStripPanelName"))))
                    {
                        // 默认值
                        return string.Empty;
                    }

                    return ((string)(this.GetPreviousVersion("ToolStripPanelName")));
                }

                return ((string)(this["ToolStripPanelName"]));
            }
            set
            {
                this["ToolStripPanelName"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("ImageAndText")]
        public string DisplayStyle
        {
            get
            {
                const string defaultValue = "ImageAndText";
                if (this["DisplayStyle"] == null || ((string)(this["DisplayStyle"])) == string.Empty)
                {
                    // 设置早期值
                    if (this.GetPreviousVersion("DisplayStyle") == null || ((string)(this.GetPreviousVersion("DisplayStyle"))) == string.Empty)
                    {
                        // 默认值
                        return defaultValue;
                    }

                    return ((string)(this.GetPreviousVersion("DisplayStyle")));
                }

                return ((string)(this["DisplayStyle"]));
            }
            set
            {
                this["DisplayStyle"] = value;
            }
        }

    }
}
