using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// 与组相关的操作，目的是将legend中的层分组
    /// </summary>
   public class Groups : MapWinGIS.Interfaces.Groups
    {
        private Legend m_Legend;

       /// <summary>
       /// 构造函数
       /// </summary>
        /// <param name="leg">Legend对象</param>
        public Groups(Legend leg)
        {
            m_Legend = leg;
        }

        /// <summary>
        /// 添加一个新的组到legend中的最上面
        /// </summary>
        public int Add()
        {
            return m_Legend.AddGroup("新建组");
        }

        /// <summary>
        /// 用指定名称（Caption）在legend的最上面添加一个新的组
        /// </summary>
        public int Add(string Name)
        {
            return m_Legend.AddGroup(Name);
        }

        /// <summary>
        /// 指定名称、位置在legend上添加一个新的组
        /// </summary>
        public int Add(string Name, int Position)
        {
            return m_Legend.AddGroup(Name, Position);
        }

        /// <summary>
        /// 移除一个组及其所有的层
        /// </summary>
        public bool Remove(int Handle)
        {
            return m_Legend.RemoveGroup(Handle);
        }

        /// <summary>
        /// 获取在当前legend中的组的数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_Legend.NumGroups;
            }
        }

        /// <summary>
        /// 索引，可以通过组的位置获取一个组对象
        /// </summary>
        public Interfaces.Group this[int Position]
        {
            get
            {
                if (Position >= 0 && Position < this.Count)
                    return (Group)m_Legend.m_AllGroups[Position];
                else
                {
                    Globals.LastError = "Invalid Group Position ( Must be >= 0 and < Count )";
                    return null;
                }
            }
        }
        /// <summary>
        /// 清空所有的组和层
        /// </summary>
        public void Clear()
        {
            m_Legend.ClearGroups();
        }

        /// <summary>
        /// 可以通过组的位置获取一个组对象
        /// </summary>
        public Interfaces.Group ItemByPosition(int Position)
        {
            return this[Position];
        }

        /// <summary>
        /// 通过组的句柄查找一个组
        /// </summary>
        /// <param name="Handle">代表该组的唯一的号码Handle</param>
        /// <returns>返回一个Group对象，以便读取或者改变该Group的属性,null表示获取失败</returns>
        public Interfaces.Group ItemByHandle(int Handle)
        {
            if (m_Legend.IsValidGroup(Handle))
                return this[(int)m_Legend.m_GroupPositions[Handle]];
            else
            {
                Globals.LastError = "Invalid Group Handle";
                return null;
            }
        }

        /// <summary>
        /// 在组列表中查找照组的位置（index）
        /// </summary>
        /// <param name="GroupHandle">代表该组的唯一的号码Handle</param>
        public int PositionOf(int GroupHandle)
        {
            if (m_Legend.IsValidGroup(GroupHandle))
            {
                return (int)m_Legend.m_GroupPositions[GroupHandle];
            }
            else
            {
                Globals.LastError = "Invalid Group Handle";
                return -1;
            }
        }

        /// <summary>
        /// 检查指定handle的组是否仍存在于组列表中
        /// </summary>
        /// <param name="Handle">Group handle</param>
        /// <returns>True存在, False 其他</returns>
        public bool IsValidHandle(int Handle)
        {
            return m_Legend.IsValidGroup(Handle);
        }

        /// <summary>
        /// 将指定的组移动到新的位置
        /// </summary>
        /// <param name="GroupHandle">要移动组的handle</param>
        /// <param name="NewPos">组要放置的位置（从0开始）</param>
        /// <returns>True 移动成功, False 其他</returns>
        public bool MoveGroup(int GroupHandle, int NewPos)
        {
            return m_Legend.MoveGroup(GroupHandle, NewPos);
        }

        /// <summary>
        /// 折叠所有的组
        /// </summary>
        public void CollapseAll()
        {
            m_Legend.Lock();
            int i, count;

            count = Count;
            for (i = 0; i < count; i++)
                this[i].Expanded = false;
            m_Legend.Unlock();
        }

        /// <summary>
        /// 展开所有的组
        /// </summary>
        public void ExpandAll()
        {
            m_Legend.Lock();
            int i, count;

            count = Count;
            for (i = 0; i < count; i++)
                this[i].Expanded = true;
            m_Legend.Unlock();
        }
    }
}
