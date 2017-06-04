/*******************************************************************
 * 文件名：GisToolboxGroup.cs
 * 描  述：提供Gis工具集合的相关信息
 *         属性Tools可以获取工具集合中的工具清单
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using System.Windows.Forms;

namespace MapWinGIS.Controls.GisToolbox
{
    /// <summary>
    /// 提供GIS工具集合的信息
    /// </summary>
    public class GisToolboxGroup : IGisToolboxGroup
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
        /// 存储与工具有关的额外信息
        /// </summary>
        object m_tag;

        /// <summary>
        /// 树节点
        /// </summary>
        TreeNode m_node;
        
        /// <summary>
        /// 创建一个新的GIS工具组类实例
        /// </summary>
        /// <param name="name">工具名</param>
        /// <param name="description">工具描述</param>
        internal GisToolboxGroup(string name, string description)
        {
            m_name = name;
            m_description = description;
            m_tag = null;

            m_node = new TreeNode();
            m_node.Text = name;
            m_node.ImageIndex = GisToolbox.ICON_FOLDER;
            m_node.Expand();//展开树节点

            m_node.Tag = this;
        }

        /// <summary>
        /// 该组树的根节点
        /// </summary>
        internal TreeNode Node
        {
            get
            {
                return m_node;
            }
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
            set
            {
                m_tag = value;
            }
        }

        /// <summary>
        /// 获取当前组的子组的清单
        /// </summary>
        public IGisToolboxGroups SubGroups
        {
            get
            {
                if (m_node == null)
                    throw new NullReferenceException();
                else
                    return new GisToolboxGroups(m_node.Nodes);
            }
        }

        /// <summary>
        /// 获取当前组工具清单
        /// </summary>
        public IGisTools Tools
        {
            get
            {
                return new GisTools(m_node.Nodes);
            }
        }

        /// <summary>
        /// 获取或设置组在树形视图中的展开状态
        /// </summary>
        public bool Expanded
        {
            get
            {
                return m_node.IsExpanded;//获取树节点是否处于可展开状态
            }
            set
            {
                if (value)
                    m_node.Expand();//展开树节点
                else
                    m_node.Collapse();//折叠树节点
            }
        }
    }
}
