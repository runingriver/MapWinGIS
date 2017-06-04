using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace MapWinGIS.Utility
{
    public partial class CustomMsgBox : Form
    {
        /// <summary>
        /// 默认按钮
        /// </summary>
        public string LabelAccept = "Ok";

        /// <summary>
        /// 由Esc激活的按钮
        /// </summary>
        public string LabelCancel = "Cancel";

        /// <summary>
        /// 超时后自动返回按钮
        /// </summary>
        public string LabelTimeout = "Cancel";

        /// <summary>
        /// 返回计时变量
        /// </summary>
        public int TimeoutSeconds = 0;

        /// <summary>
        /// 获取或设置程序名
        /// </summary>
        public string RegistryAppName = "";

        /// <summary>
        /// 获取或设置注册值(Section)
        /// </summary>
        public string RegistrySection = "";

        /// <summary>
        /// 获取或设置注册值(Key)
        /// </summary>
        public string RegistryKey = "";

        /// <summary>
        /// 显示在checkbox上，以指示是否显示该消息
        /// </summary>
        public string RegistryCheckboxText = "总是使用此窗口";

        /// <summary>
        /// 控件间的间隔(pix)
        /// </summary>
        public int LayoutMargin = 12;

        /// <summary>
        /// 被点击的标签按钮
        /// </summary>
        private string pLabelClicked;

        public CustomMsgBox()
        {
            InitializeComponent();
        }

        public string AskUser(string aMessage, string aTitle, params string[] aButtonLabels)
        {
            return AskUser(aMessage, aTitle, Array.AsReadOnly(aButtonLabels));
        }

        public string AskUser(string aMessage, string aTitle, IEnumerable aButtonLabels)
        {
            int lButtonLeft = LayoutMargin;
            CheckBox lChkAlways = null;

            if (RegistryAppName.Length > 0 && RegistrySection.Length > 0 && RegistryKey.Length > 0)
            {
                string lRegistryLabel = Interaction.GetSetting(RegistryAppName, RegistrySection, RegistryKey, "");
                if (lRegistryLabel.Length > 0)
                {
                    return lRegistryLabel;
                }
                else
                {
                    lChkAlways = new CheckBox();
                    lChkAlways.AutoSize = true;
                    lChkAlways.Text = RegistryCheckboxText;
                    this.Controls.Add(lChkAlways);
                    lChkAlways.Left = LayoutMargin;
                }
            }

            Text = aTitle;
            lblMessage.Text = aMessage;
            List<Button> lButtons = new List<Button>();
            bool lSetHeight = false;

            foreach (string curLabel in aButtonLabels)
            {
                Button btn = new Button();
                btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                btn.AutoSize = true;
                btn.Left = lButtonLeft;

                string lLabel = curLabel;
                btn.Tag = lLabel;
                while (lLabel.StartsWith("+") || lLabel.StartsWith("-"))
                {
                    if (lLabel.StartsWith("+"))
                    {
                        LabelAccept = Convert.ToString(btn.Tag);
                    }
                    else
                    {
                        LabelCancel = Convert.ToString(btn.Tag);
                    }
                    lLabel = lLabel.Substring(1);
                }

                if (Convert.ToString(btn.Tag).ToLower() == LabelAccept.ToLower())
                {
                    this.AcceptButton = btn;
                }
                if (Convert.ToString(btn.Tag).ToLower() == LabelCancel.ToLower())
                {
                    this.CancelButton = btn;
                }

                btn.Text = lLabel;

                if (!lSetHeight)
                {
                    this.Height = lblMessage.Top + lblMessage.Height + LayoutMargin + btn.Height + LayoutMargin + this.Height - this.ClientSize.Height;
                    if (lChkAlways != null)
                    {
                        this.Height += lChkAlways.Height + LayoutMargin;
                        lChkAlways.Top = this.ClientSize.Height - btn.Height - LayoutMargin - lChkAlways.Height - LayoutMargin;
                        lChkAlways.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Right);
                    }
                    lSetHeight = true;
                }

                btn.Top = this.ClientSize.Height - btn.Height - LayoutMargin;

                lButtons.Add(btn);
                this.Controls.Add(btn);
                lButtonLeft += btn.Width + LayoutMargin;

                btn.Click += new System.EventHandler(btnClick);

            }

            int lWidest = Math.Max(lButtonLeft, lblMessage.Left + lblMessage.Width + LayoutMargin);
            if (lChkAlways != null)
            {
                lWidest = Math.Max(lWidest, LayoutMargin + lChkAlways.Width + LayoutMargin);
            }
            this.Width = lWidest;

            lblMessage.Left = (this.Width - lblMessage.Width) / 2;
            if (lChkAlways != null)
            {
                lChkAlways.Left = (this.Width - lChkAlways.Width) / 2;
            }

            if (lWidest > lButtonLeft)
            {
                int lMoveButtons = (int)((lWidest - lButtonLeft) / 2);
                foreach (Button lButton in lButtons)
                {
                    lButton.Left += lMoveButtons;
                }
            }

            pLabelClicked = "";
            if (TimeoutSeconds == 0 && this.Owner != null)
            {
                this.ShowDialog(this.Owner);
            }
            else
            {
                this.Show();
                this.BringToFront();

                double lTimeLimit = DateTime.Now.AddSeconds(System.Convert.ToDouble(TimeoutSeconds)).ToOADate();
                while (pLabelClicked.Length == 0)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                    if (TimeoutSeconds > 0 && DateTime.Now.ToOADate() > lTimeLimit)
                    {
                        pLabelClicked = LabelCancel;
                    }
                }

                this.Visible = false;
            }

            foreach (Button lButton in lButtons)
            {
                this.Controls.Remove(lButton);
            }

            if (lChkAlways != null)
            {
                if (lChkAlways.Checked)
                {
                    Interaction.SaveSetting(RegistryAppName, RegistrySection, RegistryKey, pLabelClicked);
                }
                this.Controls.Remove(lChkAlways);
            }

            return pLabelClicked;
        }

        private void btnClick(object sender, System.EventArgs e)
        {
            pLabelClicked = Convert.ToString(((Button)sender).Tag);
            if (this.Modal)
            {
                this.Close();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //If the window is being closed before a buttons was clicked, pretend the Cancel button was clicked.
            if (pLabelClicked.Length == 0)
            {
                pLabelClicked = LabelCancel;
            }
        }


    }
}
