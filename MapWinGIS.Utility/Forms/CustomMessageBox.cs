using System;
using System.Windows.Forms;

namespace MapWinGIS.Utility
{
    public enum MessageButton
    {
        OK = 0,
        OKCancle = 1,
        YesNoCancel = 2,
        YseNo = 3,
        RetryCancel = 4
    }

    public enum MessageDefaultButton //设置TabIndex顺序即可
    {
        Button1, Button2, Button3
    }

    /// <summary>
    /// 若不用后，记得释放资源"实例.Dispose()"。
    /// </summary>
    public class CustomMessageBox : Form
    {

        // 控件间的间隔(pix)
        public int leftRightMargin = 15;

        //控件与窗体底部间距
        public int bottomTopMargin = 15;

        //根据触发的事件和按钮类型，存储返回结果
        private DialogResult dialogResult;

        //用于记录使用的按钮类型
        private MessageButton msgButtonType;

        //用于记录是按钮还是ControlBox触发的关闭事件
        private bool isButton;

        internal System.Windows.Forms.Button btnOk;
        internal System.Windows.Forms.Button btnNo;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Label messageLabel;

        public CustomMessageBox()
        {
            InitializeComponent();
            dialogResult = System.Windows.Forms.DialogResult.OK;
            msgButtonType = MessageButton.OK;
            isButton = false;
        }

        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.messageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(89, 59);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确 定";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(103, 65);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "否(&N)";
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(194, 65);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取 消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)), true);
            this.messageLabel.Location = new System.Drawing.Point(15, 20);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(77, 12);
            this.messageLabel.TabIndex = 3;
            this.messageLabel.Text = "messageLabel";
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 92);
            this.Controls.Add(this.messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(201, 116);
            this.Name = "CustomMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "消息";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomMessageBox_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public DialogResult Display(string message)
        {
            if (message == null || message == "") return System.Windows.Forms.DialogResult.OK;
            if (this.Controls.Count > 1)
            {
                this.Controls.Clear();
                this.Controls.Add(this.messageLabel);
            }
            isButton = false;
            msgButtonType = MessageButton.OK;
            this.messageLabel.Text = message;
            int msgheiht = this.messageLabel.Size.Height;
            int msgwidth = this.messageLabel.Size.Width;
            int btnwidth = this.btnOk.Size.Width;
            int btnhight = this.btnOk.Size.Height;
            int width, hight;
            //根据消息框的大小设置工作区大小
            if (msgwidth > this.ClientSize.Width - leftRightMargin * 2)
            {
                this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(192, 90);
            }
            width = this.ClientSize.Width;
            hight = this.ClientSize.Height;

            this.btnOk.Location = new System.Drawing.Point((width - btnwidth) / 2, hight - btnhight - bottomTopMargin);
            this.Controls.Add(this.btnOk);
            if (this.Owner == null)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ShowDialog();
            }
            else
            {
                this.ShowDialog(this.Owner);
            }

            return dialogResult;
        }

        public DialogResult Display(string message, string caption)
        {
            if (message == null || message == "") return System.Windows.Forms.DialogResult.OK;
            if (this.Controls.Count > 1)
            {
                this.Controls.Clear();
                this.Controls.Add(this.messageLabel);
            }
            isButton = false;
            msgButtonType = MessageButton.OK;
            this.Text = caption;
            this.messageLabel.Text = message;
            int msgheiht = this.messageLabel.Size.Height;
            int msgwidth = this.messageLabel.Size.Width;
            int btnwidth = this.btnOk.Size.Width;
            int btnhight = this.btnOk.Size.Height;
            int width, hight;
            //根据消息框的大小设置工作区大小
            if (msgwidth > this.ClientSize.Width - leftRightMargin * 2)
            {
                this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(192, 90);
            }
            width = this.ClientSize.Width;
            hight = this.ClientSize.Height;

            this.btnOk.Location = new System.Drawing.Point((width - btnwidth) / 2, hight - btnhight - bottomTopMargin);
            this.Controls.Add(this.btnOk);
            if (this.Owner == null)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ShowDialog();
            }
            else
            {
                this.ShowDialog(this.Owner);
            }

            return dialogResult;
        }

        public DialogResult Display(string message, string caption, MessageButton msgButton)
        {
            if (message == null || message == "") return System.Windows.Forms.DialogResult.OK;
            if (this.Controls.Count > 1)
            {
                this.Controls.Clear();
                this.Controls.Add(this.messageLabel);
            }

            this.Text = caption;
            this.messageLabel.Text = message;
            int msgheiht = this.messageLabel.Size.Height;
            int msgwidth = this.messageLabel.Size.Width;
            int btnwidth = this.btnOk.Size.Width;
            int btnhight = this.btnOk.Size.Height;
            int width, hight;

            if (msgButton == MessageButton.OK)
            {
                msgButtonType = MessageButton.OK;
                this.btnOk.Text = "确 定";
                if (msgwidth > this.ClientSize.Width - leftRightMargin * 2)
                {
                    this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
                }
                else
                {
                    this.ClientSize = new System.Drawing.Size(195, 90);
                }
                width = this.ClientSize.Width;
                hight = this.ClientSize.Height;

                this.btnOk.Location = new System.Drawing.Point((width - btnwidth) / 2, hight - btnhight - bottomTopMargin);
                this.Controls.Add(this.btnOk);
            }
            else if (msgButton == MessageButton.OKCancle)
            {
                msgButtonType = MessageButton.OKCancle;
                this.btnOk.Text = "确 定";
                this.btnCancel.Text = "取 消";
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                if (msgwidth + leftRightMargin * 2 > this.ClientSize.Width || btnwidth * 2 + leftRightMargin * 3 > this.ClientSize.Width)
                {
                    if (msgwidth + leftRightMargin * 2 > btnwidth * 2 + leftRightMargin * 3)
                    {
                        this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                    else
                    {
                        this.ClientSize = new System.Drawing.Size(btnwidth * 2 + leftRightMargin * 3, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                }
                else
                {
                    this.ClientSize = new System.Drawing.Size(195, 90);
                }
                width = this.ClientSize.Width;
                hight = this.ClientSize.Height;

                this.btnOk.Location = new System.Drawing.Point((width - btnwidth * 2 - leftRightMargin) / 2, hight - btnhight - bottomTopMargin);
                this.btnCancel.Location = new System.Drawing.Point(((width - btnwidth * 2 - leftRightMargin) / 2) + btnwidth + leftRightMargin, hight - btnhight - bottomTopMargin);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.btnCancel);
            }
            else if (msgButton == MessageButton.RetryCancel)
            {
                msgButtonType = MessageButton.RetryCancel;
                this.btnCancel.Text = "取 消";
                this.btnOk.Text = "重 试";
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                if (msgwidth + leftRightMargin * 2 > this.ClientSize.Width || btnwidth * 2 + leftRightMargin * 3 > this.ClientSize.Width)
                {
                    if (msgwidth + leftRightMargin * 2 > btnwidth * 2 + leftRightMargin * 3)
                    {
                        this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                    else
                    {
                        this.ClientSize = new System.Drawing.Size(btnwidth * 2 + leftRightMargin * 3, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                }
                else
                {
                    this.ClientSize = new System.Drawing.Size(195, 90);
                }
                width = this.ClientSize.Width;
                hight = this.ClientSize.Height;

                this.btnOk.Location = new System.Drawing.Point((width - btnwidth * 2 - leftRightMargin) / 2, hight - btnhight - bottomTopMargin);
                this.btnCancel.Location = new System.Drawing.Point(((width - btnwidth * 2 - leftRightMargin) / 2) + btnwidth + leftRightMargin, hight - btnhight - bottomTopMargin);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.btnCancel);
            }
            else if (msgButton == MessageButton.YesNoCancel)
            {
                msgButtonType = MessageButton.YesNoCancel;
                this.btnOk.Text = "是(&Y)";
                this.btnNo.Text = "否(&N)";
                this.btnCancel.Text = "取 消";
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                if (msgwidth + leftRightMargin * 2 > btnwidth * 3 + leftRightMargin * 4)
                {
                    this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
                }
                else
                {
                    this.ClientSize = new System.Drawing.Size(btnwidth * 3 + leftRightMargin * 4, msgheiht + btnhight + bottomTopMargin + 40);
                }
                width = this.ClientSize.Width;
                hight = this.ClientSize.Height;

                this.btnOk.Location = new System.Drawing.Point((width - btnwidth * 3 - leftRightMargin * 2) / 2, hight - btnhight - bottomTopMargin);
                this.btnNo.Location = new System.Drawing.Point(((width - btnwidth * 3 - leftRightMargin * 2) / 2) + btnwidth + leftRightMargin, hight - btnhight - bottomTopMargin);
                this.btnCancel.Location = new System.Drawing.Point(((width - btnwidth * 3 - leftRightMargin * 2) / 2) + btnwidth * 2 + leftRightMargin * 2, hight - btnhight - bottomTopMargin);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.btnNo);
                this.Controls.Add(this.btnCancel);
            }
            else // YesNo
            {
                msgButtonType = MessageButton.YseNo;
                this.btnOk.Text = "是(&Y)";
                this.btnNo.Text = "否(&N)";
                this.AcceptButton = btnOk;
                this.CancelButton = btnNo;
                this.ControlBox = false;
                if (msgwidth + leftRightMargin * 2 > this.ClientSize.Width || btnwidth * 2 + leftRightMargin * 3 > this.ClientSize.Width)
                {
                    if (msgwidth + leftRightMargin * 2 > btnwidth * 2 + leftRightMargin * 3)
                    {
                        this.ClientSize = new System.Drawing.Size(msgwidth + leftRightMargin * 2, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                    else
                    {
                        this.ClientSize = new System.Drawing.Size(btnwidth * 2 + leftRightMargin * 3, msgheiht + btnhight + bottomTopMargin + 40);
                    }
                }
                else
                {
                    this.ClientSize = new System.Drawing.Size(195, 90);
                }
                width = this.ClientSize.Width;
                hight = this.ClientSize.Height;

                this.btnOk.Location = new System.Drawing.Point((width - btnwidth * 2 - leftRightMargin) / 2, hight - btnhight - bottomTopMargin);
                this.btnNo.Location = new System.Drawing.Point(((width - btnwidth * 2 - leftRightMargin) / 2) + btnwidth + leftRightMargin, hight - btnhight - bottomTopMargin);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.btnNo);
            }

            if (this.Owner == null)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ShowDialog();
            }
            else
            {
                this.ShowDialog(this.Owner);
            }
            return dialogResult;
        }

        public DialogResult Display(string message, string caption, MessageButton msgButton, MessageDefaultButton msgDefaultButton)
        {
            if (msgButton == MessageButton.OK)
            {
                return Display(message, caption, msgButton);
            }
            else if (msgButton == MessageButton.OKCancle) //OKCancle
            {
                switch (msgDefaultButton)
                {
                    case MessageDefaultButton.Button1: break;
                    case MessageDefaultButton.Button2:
                        this.btnCancel.TabIndex = 0;
                        this.btnOk.TabIndex = 1;
                        this.btnNo.TabIndex = 2;
                        break;
                    default:
                        throw new Exception("不存在第三个按钮.");
                }
                return Display(message, caption, msgButton);
            }
            else if (msgButton == MessageButton.RetryCancel) //RetryCancel
            {
                switch (msgDefaultButton)
                {
                    case MessageDefaultButton.Button1: break;
                    case MessageDefaultButton.Button2:
                        this.btnCancel.TabIndex = 0;
                        this.btnOk.TabIndex = 1;
                        this.btnNo.TabIndex = 2;
                        break;
                    default:
                        throw new Exception("不存在第三个按钮.");
                }
                return Display(message, caption, msgButton);
            }
            else if (msgButton == MessageButton.YesNoCancel) //YesNoCance
            {
                switch (msgDefaultButton)
                {
                    case MessageDefaultButton.Button1: break;
                    case MessageDefaultButton.Button2:
                        this.btnCancel.TabIndex = 2;
                        this.btnOk.TabIndex = 1;
                        this.btnNo.TabIndex = 0;
                        break;
                    case MessageDefaultButton.Button3:
                        this.btnCancel.TabIndex = 0;
                        this.btnOk.TabIndex = 1;
                        this.btnNo.TabIndex = 2;
                        break;
                    default: break;
                }
                return Display(message, caption, msgButton);
            }
            else //YesNo
            {
                switch (msgDefaultButton)
                {
                    case MessageDefaultButton.Button1: break;
                    case MessageDefaultButton.Button2:
                        this.btnNo.TabIndex = 0;
                        this.btnOk.TabIndex = 1;
                        this.btnCancel.TabIndex = 2;
                        break;
                    default:
                        throw new Exception("不存在第三个按钮.");
                }
                return Display(message, caption, msgButton);
            }
        }

        //btnOk 确定OK、是Yes、重试Retry。btnNo-否。btnCancel-取消
        private void btnOk_Click(object sender, EventArgs e)
        {
            switch (msgButtonType)
            {
                case MessageButton.OK: dialogResult = System.Windows.Forms.DialogResult.OK; break;
                case MessageButton.OKCancle: dialogResult = System.Windows.Forms.DialogResult.OK; break;
                case MessageButton.RetryCancel: dialogResult = System.Windows.Forms.DialogResult.Retry; break;
                case MessageButton.YesNoCancel: dialogResult = System.Windows.Forms.DialogResult.Yes; break;
                default: dialogResult = System.Windows.Forms.DialogResult.Yes; break;
            }
            isButton = true;
            this.Close();
            this.Dispose(false);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            dialogResult = System.Windows.Forms.DialogResult.No;
            isButton = true;
            this.Close();
            this.Dispose(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dialogResult = System.Windows.Forms.DialogResult.Cancel;
            isButton = true;
            this.Close();
            this.Dispose(false);
        }

        private void CustomMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isButton)
            {
                return;
            }
            else
            {
                switch (msgButtonType)
                {
                    case MessageButton.OKCancle: dialogResult = System.Windows.Forms.DialogResult.Cancel; break;
                    case MessageButton.RetryCancel: dialogResult = System.Windows.Forms.DialogResult.Cancel; break;
                    case MessageButton.YesNoCancel: dialogResult = System.Windows.Forms.DialogResult.Cancel; break;
                    case MessageButton.OK: dialogResult = System.Windows.Forms.DialogResult.OK; break;
                    default: break;
                }
                isButton = false;
            }
            this.Dispose(false);
        }
    }
}
