using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MapWinGIS.Controls
{
    /// <summary>
    /// 图层源类型
    /// </summary>
    public enum LayerSourceType
    {
        Shapefile = 0,
        Image = 1,
        Grid = 2,
        Undefined = 3,
    }

    /// <summary>
    /// 图层源错误信息
    /// </summary>
    public enum LayerSourceError
    {
        None = 0,
        DbfIsMissing = 1,
        DbfRecordCountMismatch = 2,
        OcxBased = 3,
    }

    /// <summary>
    /// 每种MapWinGIS图层类型的层常用功能的简单包装：
    /// shapefile, image, grid
    /// </summary>
    public class LayerSource
    {
        #region 声明

        private MapWinGIS.Shapefile m_shapefile = null;
        private MapWinGIS.Image m_image = null;
        private MapWinGIS.Grid m_grid = null;
        private LayerSourceError m_error = LayerSourceError.None;
        private string m_ErrorString = "";
        #endregion

        #region 初始化

        /// <summary>
        /// 创建一个新的LayerSource类实例
        /// </summary>
        public LayerSource(object obj)
        {
            if (obj is MapWinGIS.Shapefile)
            {
                m_shapefile = obj as MapWinGIS.Shapefile;
            }
            else if (obj is MapWinGIS.Image)
            {
                m_image = obj as MapWinGIS.Image;
            }
            else if (obj is MapWinGIS.Grid)
            {
                m_grid = obj as MapWinGIS.Grid;
            }
        }

        /// <summary>
        /// 创建一个新的LayerSource类实例（Shapefile）
        /// </summary>
        public LayerSource(MapWinGIS.Shapefile sf)
        {
            m_shapefile = sf;
        }

        /// <summary>
        /// 创建一个新的LayerSource类实例（Grid）
        /// </summary>
        public LayerSource(MapWinGIS.Grid grid)
        {
            m_grid = grid;
        }

        /// <summary>
        /// 创建一个新的LayerSource类实例（Image）
        /// </summary>
        public LayerSource(MapWinGIS.Image image)
        {
            m_image = image;
        }

        /// <summary>
        /// 根据文件名初始化对象
        /// </summary>
        public LayerSource(string filename) : this(filename, null) { }

        /// <summary>
        /// 根据文件名初始化对象
        /// </summary>
        public LayerSource(string filename, MapWinGIS.ICallback callback)
        {
            this.Open(filename, callback);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 返回最后错误代码的描述
        /// </summary>
        public string GetErrorMessage()
        {
            string s = "";
            switch (m_error)
            {
                case LayerSourceError.DbfIsMissing:
                    s = "Dbf table丢失";
                    break;
                case LayerSourceError.DbfRecordCountMismatch:
                    s = "DBF表的行数是不同的形状";
                    break;
                case LayerSourceError.OcxBased:
                    s = m_ErrorString;
                    break;
            }
            m_ErrorString = "";
            m_error = LayerSourceError.None;
            return s;
        }


        /// <summary>
        /// 获取或设置相关的ICallback对象
        /// </summary>
        public MapWinGIS.ICallback Callback
        {
            get
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        return m_shapefile.GlobalCallback;
                    case LayerSourceType.Image:
                        return m_image.GlobalCallback;
                    case LayerSourceType.Grid:
                        return m_grid.GlobalCallback;
                    default:
                        return null;
                }
            }
            set
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        m_shapefile.GlobalCallback = value;
                        break;
                    case LayerSourceType.Image:
                        m_image.GlobalCallback = value;
                        break;
                    case LayerSourceType.Grid:
                        m_grid.GlobalCallback = value;
                        break;
                    default:
                        // 不做任何事
                        break;
                }
            }
        }

        /// <summary>
        /// 返回数据源下的文件名
        /// </summary>
        public string Filename
        {
            get
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        return m_shapefile.Filename;
                    case LayerSourceType.Image:
                        return m_image.Filename;
                    case LayerSourceType.Grid:
                        return m_grid.Filename;
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 返回未定义类型的MapWinGIS接口
        /// </summary>
        public object GetObject
        {
            get
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        return (object)m_shapefile;
                    case LayerSourceType.Image:
                        return (object)m_image;
                    case LayerSourceType.Grid:
                        return (object)m_grid;
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 返回图层类型
        /// </summary>
        public LayerSourceType Type
        {
            get
            {
                if (m_shapefile != null)
                {
                    return LayerSourceType.Shapefile;
                }
                else if (m_image != null)
                {
                    return LayerSourceType.Image;
                }
                else if (m_grid != null)
                {
                    return LayerSourceType.Grid;
                }
                else
                {
                    return LayerSourceType.Undefined;
                }
            }
        }

        /// <summary>
        /// 返回图层投影
        /// </summary>
        public MapWinGIS.GeoProjection Projection
        {
            get
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        return m_shapefile.GeoProjection;
                    case LayerSourceType.Grid:
                        return m_grid.Header.GeoProjection;
                    case LayerSourceType.Image:
                        MapWinGIS.GeoProjection proj = new MapWinGIS.GeoProjection();
                        proj.ImportFromAutoDetect(m_image.GetProjection());
                        return proj;
                    default:
                        return null;
                }
            }

            set
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        m_shapefile.GeoProjection = value;
                        break;
                    case LayerSourceType.Grid:
                        string s = value != null && !value.IsEmpty ? value.ExportToProj4() : "";
                        m_grid.AssignNewProjection(s);
                        break;
                    case LayerSourceType.Image:
                    // TODO: implement
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 返回数据源下的文件名
        /// </summary>
        public MapWinGIS.Extents Extents
        {
            get
            {
                switch (this.Type)
                {
                    case LayerSourceType.Shapefile:
                        return m_shapefile.Extents;
                    case LayerSourceType.Image:
                        return m_image.Extents;
                    case LayerSourceType.Grid:
                        // 临时, 没有返回空对象
                        MapWinGIS.Extents ext = new MapWinGIS.Extents();
                        ext.SetBounds(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
                        return ext;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// 获得shapefile
        /// </summary>
        public MapWinGIS.Shapefile Shapefile
        {
            get { return m_shapefile; }
        }

        /// <summary>
        /// 获得image
        /// </summary>
        public MapWinGIS.Image Image
        {
            get { return m_image; }
        }

        /// <summary>
        /// 获得image
        /// </summary>
        public MapWinGIS.Grid Grid
        {
            get { return m_grid; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 关闭图层. 设置未定义的图层类型
        /// </summary>
        public void Close()
        {
            m_error = LayerSourceError.None;
            switch (this.Type)
            {
                case LayerSourceType.Shapefile:
                    m_shapefile.Close();
                    m_shapefile = null;
                    break;
                case LayerSourceType.Image:
                    m_image.Close();
                    m_image = null;
                    break;
                case LayerSourceType.Grid:
                    m_grid.Close();
                    m_grid = null;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 打开指定数据源的图层
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Open(string filename, MapWinGIS.ICallback callback)
        {
            this.Close();

            if (filename.ToLower().EndsWith(".shp"))
            {
                MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
                if (sf.Open(filename, callback))
                {
                    // 检查dbf是否存在
                    bool error = false;
                    if (!File.Exists(Path.ChangeExtension(sf.Filename, ".dbf")))
                    {
                        m_error = LayerSourceError.DbfIsMissing;
                        error = true;
                    }

                    // 检查DBF记录数相匹配的形状的数量。
                    MapWinGIS.Table table = new MapWinGIS.Table();
                    table.Open(Path.ChangeExtension(sf.Filename, ".dbf"), null);
                    if (sf.NumShapes != table.NumRows)
                    {
                        m_error = LayerSourceError.DbfRecordCountMismatch;
                        error = true;
                    }

                    table.Close();

                    if (error)
                    {
                        sf.Close();
                    }
                    else
                    {
                        m_shapefile = sf;
                    }
                    return !error;
                }
                else
                {
                    m_error = LayerSourceError.OcxBased;
                    m_ErrorString = sf.get_ErrorMsg(sf.LastErrorCode);
                }
            }
            else
            {
                bool asGrid = true;
                if (filename.ToLower().EndsWith(".tif"))
                {
                    asGrid = false;
                }

                // TODO: 可能更聪明的选择是在grid/image中使用应用程序设置
                if (asGrid)
                {
                    MapWinGIS.Grid grid = new MapWinGIS.Grid();
                    if (grid.Open(filename, MapWinGIS.GridDataType.UnknownDataType, false, MapWinGIS.GridFileType.UseExtension, callback))
                    {
                        m_grid = grid;
                        return true;
                    }
                }

                // 尝试image
                MapWinGIS.Image image = new MapWinGIS.Image();
                if (image.Open(filename, MapWinGIS.ImageType.USE_FILE_EXTENSION, false, callback))
                {
                    m_image = image;
                    return true;
                }
                else
                {
                    m_error = LayerSourceError.OcxBased;
                    m_ErrorString = image.get_ErrorMsg(image.LastErrorCode);
                }
            }
            return false;
        }
        #endregion
    }
}
