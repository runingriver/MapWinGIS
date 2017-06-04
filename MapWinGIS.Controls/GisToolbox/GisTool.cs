/*******************************************************************
 * 文件名：GisTool.cs
 * 描  述：提供Gis工具的相关信息
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Windows.Forms;
using MapWinGIS.Interfaces;

namespace MapWinGIS.Controls.GisToolbox
{
    /// <summary>
    /// 提供GIS工具的信息
    /// </summary>
    public class GisTool : IGisTool
    {
        /// <summary>
        /// 工具名
        /// </summary>
        string m_name;

        /// <summary>
        /// 工具描述
        /// </summary>
        string m_description;

        /// <summary>
        /// 工具关键字
        /// </summary>
        string m_key;

        /// <summary>
        /// 存储与工具有关的额外信息
        /// </summary>
        object m_tag;

        /// <summary>
        /// 树节点
        /// </summary>
        TreeNode m_node;

        /// <summary>
        /// 创建一个新的GIS工具类实例
        /// </summary>
        /// <param name="name">工具名</param>
        /// <param name="key">工具关键字</param>
        /// <param name="description">工具描述</param>
        internal GisTool(string name, string key, string description)
        {
            m_name = name;
            m_description = description;
            m_key = key;
            m_tag = null;
            m_node = new TreeNode();
            m_node.Text = m_name;
            m_node.ImageIndex = GisToolbox.ICON_TOOL;
            m_node.Tag = this;
        }

        /// <summary>
        /// 参照下面的节点
        /// </summary>
        internal TreeNode Node
        {
            get { return m_node; }
        }

        /// <summary>
        /// 获取或设置GIS工具的描述
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// 获取或设置GIS工具名
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                if (m_node != null)
                    m_node.Text = m_name;
            }
        }

        /// <summary>
        /// 获取或设置GIS工具的额外信息
        /// </summary>
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        /// <summary>
        /// 获取或设置GIS工具的关键字
        /// </summary>
        public string Key
        {
            get { return m_key; }
            set { m_key = value; }
        }
    }
}
