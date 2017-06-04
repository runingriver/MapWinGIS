using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.Utility
{
    /// <summary>
    /// 处理进度条的状态、进度更新
    /// </summary>
    public interface IProgressStatus
    {
        /// <summary>
        /// 记录运行时间很长的任务的进度
        /// </summary>
        /// <param name="currentPosition">任务执行的当前位置</param>
        /// <param name="lastPosition">任务完成时的位置</param>
        void Progress(int currentPosition, int lastPosition);

        /// <summary>
        /// 更新当前的状态信息
        /// </summary>
        /// <param name="statusMessage">显示当前进度条的状态</param>
        void Status(string statusMessage);
    }

    /// <summary>
    /// 实现IProgressStatus接口的时候也可以根据需要实现该接口。
    /// 当用户需要提供一个选项去取消一个长时间运行的活动时，可以实现该接口
    /// </summary>
    public interface IProgressStatusCancel
    {
        /// <summary>
        /// true - 用户请求终止由进度条监测的活动
        /// 返回true后自动重置为false
        /// </summary>
        bool Canceled { get; set; }
    }

    /// <summary>
    /// 实现IProgressStatus，但不做任何事情
    /// </summary>
    public class NullProgressStatus : IProgressStatus
    {
        public void Progress(int aCurrentPosition, int aLastPosition)
        { }

        public void Status(string statusMessage)
        { }
    }

    public class ProgressCancelException : ApplicationException
    {
        public override string Message
        {
            get
            {
                return "用户已经取消任务";
            }
        }
    }

}
