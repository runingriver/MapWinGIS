/***************************************************************
 * 文件名：ProjectionClasses.cs
 * 描  述：
 * *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MapWinGIS.Controls.Projections
{
    /// <summary>
    /// 坐标系统的类型
    /// </summary>
    public enum GeographicalCSType
    {
        /// <summary>
        /// 用于单一国家的坐标系统
        /// </summary>
        Local = 0,

        /// <summary>
        /// 用于某些区域内部的坐标系统
        /// </summary>
        Regional = 1,

        /// <summary>
        /// 用于全世界的坐标系统
        /// </summary>
        Global = 2,
    }

    /// <summary>
    /// 坐标系统和国家的基类
    /// </summary>
    public class Territory
    {
        /// <summary>
        /// 领土（坐标系统，国家或区域）的代码
        /// </summary>
        public int Code;

        /// <summary>
        /// 领土的名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 左边界
        /// </summary>
        public double Left;

        /// <summary>
        /// 右边界
        /// </summary>
        public double Right;

        /// <summary>
        /// 上边界
        /// </summary>
        public double Top;

        /// <summary>
        /// 下边界
        /// </summary>
        public double Bottom;

        /// <summary>
        /// 属于边界
        /// </summary>
        internal bool IsActive;

        /// <summary>
        /// 设置字符串形式的名字
        /// </summary>
        public override string ToString()
        {
            return this.Name == "" ? "not defined" : this.Name;
        }
    }

    /// <summary>
    /// 保存有关区域信息的类
    /// </summary>
    public class Region
    {
        /// <summary>
        /// 区域的名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 区域的代码
        /// </summary>
        public int Code;

        /// <summary>
        /// 父区域的代码
        /// </summary>
        public int ParentCode;

        /// <summary>
        /// 属于区域的国家列表
        /// </summary>
        public List<Country> Countries = new List<Country>();
    }

    /// <summary>
    /// 保存有关国家信息的类
    /// </summary>
    public class Country : Territory
    {
        /// <summary>
        /// 区域的代码，属于国家的
        /// </summary>
        public int RegionCode;

        /// <summary>
        /// 国家的地理坐标系统列表
        /// </summary>
        public List<GeographicCS> GeographicCS = new List<GeographicCS>();

        /// <summary>
        /// 区域投影坐标系统的EPSG代码（引用能通过GCS列表获得）
        /// </summary>
        public List<int> ProjectedCS = new List<int>();
    }

    /// <summary>
    /// 坐标系统
    /// </summary>
    public class CoordinateSystem : Territory
    {
        /// <summary>
        /// 一个EPSG数据库范围的字符串描述，国家的通常名字
        /// </summary>
        public string Scope;

        /// <summary>
        /// 适用于区域的坐标系统的文字描述
        /// </summary>
        public string AreaName;

        /// <summary>
        /// 坐标系统的备注
        /// </summary>
        public string Remarks;

        /// <summary>
        /// 设置字符串形式的名字
        /// </summary>
        public override string ToString()
        {
            return base.Name == "" ? "not defined" : base.Name;
        }

        /// <summary>
        /// 坐标系统的Proj4字符串
        /// </summary>
        public string proj4;

        /// <summary>
        /// 指定的投影替代Proj4陈数的列表
        /// </summary>
        public List<string> Dialects;

        /// <summary>
        /// 创建一个新坐标系统类的实例
        /// </summary>
        public CoordinateSystem()
        {
            Dialects = new List<string>();
        }

        /// <summary>
        /// 获取适用的坐标系统的扩充（十进制度数）
        /// </summary>
        public MapWinGIS.Extents Extents
        {
            get
            {
                MapWinGIS.Extents extents = new MapWinGIS.Extents();
                extents.SetBounds(this.Left, this.Bottom, 0.0, this.Right, this.Top, 0.0);
                return extents;
            }
        }
    }

    /// <summary>
    /// 保存有关GCS信息的类
    /// </summary>
    public class GeographicCS : CoordinateSystem
    {
        /// <summary>
        /// 地理坐标系统的投影列表
        /// </summary>
        public List<ProjectedCS> Projections = new List<ProjectedCS>();

        /// <summary>
        /// 地理坐标系统的类型
        /// </summary>
        public GeographicalCSType Type;

        /// <summary>
        /// EPSG数据库的代码区域
        /// </summary>
        public int AreaCode;

        /// <summary>
        /// 属于（仅区域系统）区域坐标系统的代码
        /// </summary>
        public int RegionCode;

        /// <summary>
        /// 投影代码的哈希表
        /// </summary>
        private Hashtable m_dctProjections = null;

        /// <summary>
        /// 通过代码（哈希表）快速查找投影
        /// </summary>
        /// <param name="pcsCode">投影的代码</param>
        public ProjectedCS ProjectionByCode(int pcsCode)
        {
            if (m_dctProjections == null)//哈希表为空
            {
                m_dctProjections = new Hashtable();//新建哈希表
                foreach (ProjectedCS pcs in Projections)
                {
                    m_dctProjections.Add(pcs.Code, pcs);//将投影列表中的投影坐标系统逐一存入哈希表中
                }
            }

            if (m_dctProjections.ContainsKey(pcsCode))//哈希表中有对应的投影代码
                return (ProjectedCS)m_dctProjections[pcsCode];//返回对应的投影坐标系
            else
                return null;
        }

        /// <summary>
        /// Settings name as string representation
        /// </summary>
        //public override string ToString()
        //{
        //    return base.Name == "" ? "not defined" : base.Name;
        //}
    }

    /// <summary>
    /// 保存有关投影坐标系统的类
    /// </summary>
    public class ProjectedCS : CoordinateSystem
    {
        /// <summary>
        /// 源地理坐标系统的EPSG代码
        /// </summary>
        public int SourceCode;

        /// <summary>
        /// 投影的类型（自定义分类为特定系统）
        /// </summary>
        public string ProjectionType;

        /// <summary>
        /// 计量单位
        /// </summary>
        public int Units;

        /// <summary>
        /// 标志着局部投影，应在国家唯一的节点显示
        /// </summary>
        public bool Local;

        /// <summary>
        /// Settings name as string representation
        /// </summary>
        //public override string ToString()
        //{
        //    return base.Name == "" ? "not defined" : base.Name;
        //}
    }
}
