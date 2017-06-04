using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using MapWinGIS;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// Legend中的一个层对象
    /// </summary>
    public class Layer
    {
        #region 变量和区域设置

        /// <summary>
        /// 采用“stipple scheme”方式在Legend条目上显示标题，设置""表示无效
        /// </summary>
        public string ColorSchemeFieldCaption = string.Empty;

        /// <summary>
        /// 告诉legend你的自定义描绘Legend的高度，以便设置子条目的的尺寸
        /// </summary>
        public ExpansionBoxCustomHeight ExpansionBoxCustomHeightFunction;

        /// <summary>
        /// 描绘一个展开得到层的区域，并且必须设置ExpansionBoxCustomHeightFunction
        /// </summary>
        public ExpansionBoxCustomRenderer ExpansionBoxCustomRenderFunction;

        /// <summary>
        /// 可以用来控制expansion box的选项设置是否显示
        /// </summary>
        public bool ExpansionBoxForceAllowed;

        /// <summary>
        /// 如果你希望层去显示hatching和color scheme 信息
        /// 那么这将提供legend 的关于hatching scheme的信息
        /// </summary>
        public ShapefileFillStippleScheme HatchingScheme;

        /// <summary>
        /// 当要显示地图的提示文本时，指示哪个区域的索引应该被使用
        /// </summary>
        public int MapTooltipFieldIndex = -1;

        /// <summary>
        /// 指示该层对象的tooltip是否显示
        /// </summary>
        public bool MapTooltipsEnabled;

        /// <summary>
        /// PointImageFieldCaption
        /// </summary>
        public string PointImageFieldCaption = string.Empty;

        /// <summary>
        /// 如果你希望层去显示point image和color scheme 信息
        /// 那么这将提供legend 的关于point image schem的信息
        /// </summary>
        public ShapefilePointImageScheme PointImageScheme;

        /// <summary>
        /// 采用“coloring scheme”方式在Legend条目上显示标题，设置""表示无效
        /// </summary>
        public string StippleSchemeFieldCaption = string.Empty;

        /// <summary>
        /// 指示line（线）或者polygon（多边形）的顶点是否可见（不适用与shapefiles 的line）
        /// </summary>
        public bool VerticesVisible;

        /// <summary>
        /// 存储与层相关的自定义对象
        /// </summary>
        public Hashtable m_CustomObjects;

        /// <summary>
        /// 层的元素
        /// </summary>
        internal List<LayerElement> Elements; // 包含位置、大小和类型

        /// <summary>
        /// m_small图标是否被绘制
        /// </summary>
        internal bool m_smallIconWasDrawn;

        /// <summary>
        /// 这个层对象的 Color Scheme信息
        /// </summary>
        protected internal ArrayList ColorLegend;

        /// <summary>
        /// 如果是image类型的层，这指示层是否包含透明度设置
        /// </summary>
        protected internal bool HasTransparency;

        /// <summary>
        /// 这个层的顶部
        /// </summary>
        protected internal int Top;

        /// <summary>
        /// 这个层的句柄（handle），在MapWinGIS.Map中获取的
        /// </summary>
        protected internal int m_Handle; 

        /// <summary>
        /// Legend对象
        /// </summary>
        private readonly Legend m_Legend;

        /// <summary>
        /// 是否可展开变量
        /// </summary>
        private bool m_Expanded;

        /// <summary>
        /// 图标变量
        /// </summary>
        private object m_Icon;

        /// <summary>
        /// 动态可见性变量
        /// </summary>
        private bool m_UseDynamicVisibility;

        /// <summary>
        /// 高度变量
        /// </summary>
        private int m_height;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="leg">legend对象</param>
        public Layer(Legend leg)
        {
            this.m_Legend = leg;
            this.Expanded = this.m_Legend.m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology;

            this.ColorLegend = new ArrayList();
            this.m_Handle = -1;
            this.m_Icon = null;
            this.Type = eLayerType.Invalid;
            this.m_UseDynamicVisibility = false;
            this.HasTransparency = false;

            this.Elements = new List<LayerElement>();

            this.m_CustomObjects = new Hashtable();
            this.m_smallIconWasDrawn = false;

        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取和设置这个层是否可展开，这将显示或隐藏层的Color Scheme
        /// </summary>
        public bool Expanded
        {
            get
            {
                return this.m_Expanded;
            }

            set
            {
                this.m_Expanded = value;
                this.m_Legend.Redraw();
            }
        }

        /// <summary>
        ///   获取这个层的句柄
        /// </summary>
        public int Handle
        {
            get
            {
                return this.m_Handle;
            }
        }

        /// <summary>
        /// 获取层的高度
        /// </summary>
        public int Height
        {
            get
            {
                return this.CalcHeight();
            }
        }

        /// <summary>
        /// 当绘制legend的时候，指示是否跳过此layer
        /// </summary>
        public bool HideFromLegend { get; set; }

        /// <summary>
        /// 获取和设置显示在层下面的图标
        /// 设置值为null，表示移除该图标，并设为默认图标
        /// </summary>
        public object Icon
        {
            get
            {
                return this.m_Icon;
            }

            set
            {
                if (Globals.IsSupportedPicture(value))
                {
                    this.m_Icon = value;
                }
                else
                {
                    throw new Exception("Legend 错误: 无效的图标类型");
                }
            }
        }

        /// <summary>
        /// 当层是可见并且动态可见性被使用
        /// 获取和设置最大的范围
        /// </summary>
        public double MaxVisibleScale
        {
            get
            {
                return this.m_Legend.m_Map.get_LayerMaxVisibleScale(this.m_Handle);
            }

            set
            {
                this.m_Legend.m_Map.set_LayerMaxVisibleScale(this.m_Handle, value);
            }
        }

        /// <summary>
        ///  当层是可见并且动态可见性被使用
        /// 获取和设置最小范围
        /// </summary>
        public double MinVisibleScale
        {
            get
            {
                return this.m_Legend.m_Map.get_LayerMinVisibleScale(this.m_Handle);
            }

            set
            {
                this.m_Legend.m_Map.set_LayerMinVisibleScale(this.m_Handle, value);
            }
        }

        /// <summary>
        /// 当保存项目时，指示是否跳过该层
        /// </summary>
        public bool SkipOverDuringSave
        {
            get
            {
                return this.m_Legend.m_Map.get_LayerSkipOnSaving(this.Handle);
            }

            set
            {
                this.m_Legend.m_Map.set_LayerSkipOnSaving(this.Handle, value);
            }
        }

        /// <summary>
        /// 获取和设置层的数据类型
        /// 当指定的是grid类型层时需要进行设置，Shapefile和image类型自动设置为正确的值
        /// </summary>
        public eLayerType Type { get; set; }

        /// <summary>
        /// 指示该层是否设置为动态可见性（dynamic visibility）
        /// 如果层使用了（dynamic visibility）legend将设置checkbox为灰色
        /// </summary>
        public bool UseDynamicVisibility
        {
            get
            {
                if (this.m_Legend.m_Map.ShapeDrawingMethod != tkShapeDrawingMethod.dmNewSymbology)
                {
                    return this.m_UseDynamicVisibility;
                }

                return this.m_Legend.m_Map.get_LayerDynamicVisibility(this.Handle);
            }

            set
            {
                if (this.m_Legend.m_Map.ShapeDrawingMethod != tkShapeDrawingMethod.dmNewSymbology)
                {
                    this.m_UseDynamicVisibility = value;
                }
                else
                {
                    this.m_Legend.m_Map.set_LayerDynamicVisibility(this.Handle, value);
                }
            }
        }

        /// <summary>
        /// 获取和设置该层的可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.m_Legend.m_Map.get_LayerVisible(this.m_Handle);
            }

            set
            {
                var oldVal = this.m_Legend.m_Map.get_LayerVisible(this.m_Handle);
                if (oldVal != value)
                {
                    this.m_Legend.m_Map.set_LayerVisible(this.m_Handle, value);
                    this.m_Legend.Redraw();
                }
            }
        }

        #endregion

        #region 私有属性

        /// <summary>
        /// 设置NewColorLegend
        /// </summary>
        private object NewColorLegend
        {
            set
            {
                var ColorScheme = value;

                this.ColorLegend.Clear();

                System.Drawing.Color startColor, endColor;
                string startVal, endVal;
                string caption;
                int numBreaks;
                ColorInfo ci;

                this.HasTransparency = this.m_Legend.HasTransparency(this.m_Legend.Map.get_GetObject(this.Handle));

                if (ColorScheme != null)
                {
                    var sfcs = ColorScheme as MapWinGIS.ShapefileColorScheme;
                    if (sfcs != null)
                    {
                        ShapefileColorBreak s_Break = null;
                        numBreaks = sfcs.NumBreaks();
                        for (var i = 0; i < numBreaks; i++)
                        {
                            //获取break
                            s_Break = sfcs.get_ColorBreak(i);

                            // 获取start和end颜色
                            startColor = Globals.UintToColor(s_Break.StartColor);
                            endColor = Globals.UintToColor(s_Break.EndColor);

                            caption = s_Break.Caption;
                            if (caption.Length < 1)
                            {
                                // 获取标题值
                                startVal = s_Break.StartValue.ToString();

                                endVal = s_Break.EndValue.ToString();

                                if (startVal.CompareTo(endVal) == 0)
                                {
                                    caption = startVal;
                                }
                                else
                                {
                                    caption = startVal + " - " + endVal;
                                }
                            }


                            ci = new ColorInfo(startColor, endColor, caption);
                            // 添加到列表
                            this.ColorLegend.Add(ci);
                        }
                    }
                    else
                    {
                        var gcs = ColorScheme as MapWinGIS.GridColorScheme;
                        if (gcs != null)
                        {
                            GridColorBreak gcBreak;
                            numBreaks = gcs.NumBreaks;
                            for (var i = 0; i < numBreaks; i++)
                            {
                                // 获取break
                                gcBreak = gcs.get_Break(i);

                                // 获取start and end colors
                                startColor = Globals.UintToColor(gcBreak.LowColor);
                                endColor = Globals.UintToColor(gcBreak.HighColor);

                                caption = gcBreak.Caption;
                                if (caption.Length < 1)
                                {
                                    // 获取标题值
                                    startVal = Math.Round(gcBreak.LowValue, 3).ToString();
                                    endVal = Math.Round(gcBreak.HighValue, 3).ToString();
                                    caption = startVal + " - " + endVal; // 生成标题
                                }

                                // 添加到颜色列表
                                ci = new ColorInfo(startColor, endColor, caption);
                                this.ColorLegend.Add(ci);
                            }

                            // 添加NoDataColor（当绘制没有数据的值时使用的颜色）
                            ci = new ColorInfo(
                                Globals.UintToColor(gcs.NoDataColor),
                                Globals.UintToColor(gcs.NoDataColor),
                                "没数据",
                                this.HasTransparency);
                            this.ColorLegend.Add(ci);
                        }
                    }
                }
            }
        }

        #endregion

        #region 公共方法和操作

        /// <summary>
        /// 根据指定的key获取自定义对象
        /// </summary>
        public object GetCustomObject(string key)
        {
            return this.m_CustomObjects[key];
        }

        /// <summary>
        /// 重新产生与该层相关的Color Scheme并且让这个控件进行自身重绘
        /// </summary>
        public void Refresh()
        {
            this.NewColorLegend = this.m_Legend.m_Map.GetColorScheme(this.Handle);
            this.m_Legend.Redraw();
        }

        /// <summary>
        /// 设置于该层相关的自定义对象
        /// </summary>
        public void SetCustomObject(object obj, string key)
        {
            this.m_CustomObjects[key] = obj;
        }

        /// <summary>
        /// 获取该层的快照（snapshot）
        /// </summary>
        public Bitmap Snapshot()
        {
            return this.m_Legend.LayerSnapshot(this.Handle);
        }

        /// <summary>
        /// 获取该层的快照（snapshot）
        /// </summary>
        /// <param name="imgWidth">快照的宽度（pix）</param>
        public Bitmap Snapshot(int imgWidth)
        {
            return this.m_Legend.LayerSnapshot(this.Handle, imgWidth);
        }

        /// <summary>
        /// 计算给定范围的高度
        /// </summary>
        public int Get_CategoryHeight(ShapeDrawingOptions options)
        {
            if (this.Type == eLayerType.PolygonShapefile || this.Type == eLayerType.LineShapefile)
            {
                return Constants.CS_ITEM_HEIGHT + 2;
            }
            else if (this.Type == eLayerType.PointShapefile && options.Picture != null
                     && options.PointType == MapWinGIS.tkPointSymbolType.ptSymbolPicture)
            {
                if (options.Picture.Height * options.PictureScaleY + 2 <= Constants.CS_ITEM_HEIGHT)
                {
                    return Constants.CS_ITEM_HEIGHT + 2;
                }
                else
                {
                    return (int)(options.Picture.Height * options.PictureScaleY + 2);
                }
            }
            else if (this.Type == eLayerType.PointShapefile)
            {
                if (options.PointSize + 2 <= Constants.CS_ITEM_HEIGHT)
                {
                    return Constants.CS_ITEM_HEIGHT + 2;
                }
                else
                {
                    return (int)options.PointSize + 2;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 返回图标（icon）的宽度
        /// </summary>
        public int Get_CategoryWidth(ShapeDrawingOptions options)
        {
            var maxWidth = 100;
            if (this.Type == eLayerType.PolygonShapefile || this.Type == eLayerType.LineShapefile)
            {
                return Constants.ICON_WIDTH;
            }
            else if (this.Type == eLayerType.PointShapefile && options.Picture != null
                     && options.PointType == MapWinGIS.tkPointSymbolType.ptSymbolPicture)
            {
                if (options.Picture.Width * options.PictureScaleX <= Constants.ICON_WIDTH)
                {
                    return Constants.ICON_WIDTH;
                }
                else
                {
                    var width = (int)(options.Picture.Width * options.PictureScaleX);
                    if (width <= maxWidth)
                    {
                        return width;
                    }
                    else
                    {
                        return maxWidth;
                    }
                }
            }
            else if (this.Type == eLayerType.PointShapefile)
            {
                if (options.PointSize <= Constants.ICON_WIDTH)
                {
                    return Constants.ICON_WIDTH;
                }
                else
                {
                    var width = (int)options.PointSize;
                    if (width <= maxWidth)
                    {
                        return width;
                    }
                    else
                    {
                        return maxWidth;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 计算图标（icon）的最大宽度
        /// </summary>
        public int Get_MaxIconWidth(Shapefile sf)
        {
            if (sf == null)
            {
                return 0;
            }

            var maxWidth = this.Get_CategoryWidth(sf.DefaultDrawingOptions);
            for (var i = 0; i < sf.Categories.Count; i++)
            {
                var width = this.Get_CategoryWidth(sf.Categories.get_Item(i).DrawingOptions);
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }

            return maxWidth;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 计算层的高度
        /// </summary>
        /// <param name="UseExpandedHeight">true，展开的高度。false, 层显示的高度</param>
        protected internal int CalcHeight(bool UseExpandedHeight)
        {
            // to affect drawing of the expansion box externally
            if (this.m_Expanded && this.ExpansionBoxCustomHeightFunction != null)
            {
                var ht = Constants.ITEM_HEIGHT;
                var Handled = false;
                this.ExpansionBoxCustomHeightFunction(this.m_Handle, this.m_Legend.Width, ref ht, ref Handled);
                if (Handled)
                {
                    return ht + Constants.ITEM_HEIGHT + Constants.EXPAND_BOX_TOP_PAD * 2;
                }
                else
                {
                    return Constants.ITEM_HEIGHT;
                }
            }

            var ret = 0;

            if (this.m_Legend.m_Map.ShapeDrawingMethod != MapWinGIS.tkShapeDrawingMethod.dmNewSymbology
                || (this.Type == eLayerType.Grid || this.Type == eLayerType.Image || this.Type == eLayerType.Tiles))
            {
                // Our own calculation
                if (UseExpandedHeight == false && (this.m_Expanded == false || this.ColorLegend.Count == 0))
                {
                    // || (this.Type == eLayerType.Image))
                    ret = Constants.ITEM_HEIGHT;
                }
                else
                {
                    ret = Constants.ITEM_HEIGHT + (this.ColorLegend.Count * Constants.CS_ITEM_HEIGHT) + 2;
                }

                // Add in caption space
                if (UseExpandedHeight || this.m_Expanded)
                {
                    ret += (this.ColorSchemeFieldCaption.Trim() != string.Empty ? Constants.CS_ITEM_HEIGHT : 0)
                           + (this.StippleSchemeFieldCaption.Trim() != string.Empty ? Constants.CS_ITEM_HEIGHT : 0);
                }

                // Add in extra for hatching
                if ((UseExpandedHeight || this.m_Expanded)
                    && (this.HatchingScheme != null && this.HatchingScheme.NumHatches() > 0))
                {
                    ret += this.HatchingScheme.NumHatches() * Constants.CS_ITEM_HEIGHT;
                }

                // Add in extra for point image scheme (5/2/2010 DK)
                if ((UseExpandedHeight || this.m_Expanded)
                    && (this.PointImageScheme != null && this.PointImageScheme.NumberItems > 0))
                {
                    if (this.PointImageFieldCaption == string.Empty)
                    {
                        ret += (this.PointImageScheme.Items.Count * Constants.CS_ITEM_HEIGHT) + 2;
                    }
                    else
                    {
                        ret += ((this.PointImageScheme.Items.Count + 1) * Constants.CS_ITEM_HEIGHT) + 2;
                    }
                }
            }

            // --------------------------------------------------------
            // new drawing procedure
            // --------------------------------------------------------
            else if (this.m_Legend.m_Map.ShapeDrawingMethod == MapWinGIS.tkShapeDrawingMethod.dmNewSymbology)
            {
                var sf = this.m_Legend.m_Map.get_GetObject(this.Handle) as MapWinGIS.Shapefile;

                if (sf != null && (UseExpandedHeight || this.m_Expanded))
                {
                    ret = Constants.ITEM_HEIGHT + 2; // layer name

                    // height of symbology or label
                    int val1, val2;
                    val1 = (this.Get_CategoryHeight(sf.DefaultDrawingOptions) + 2); // default symbology

                    if (sf.Labels.Count == 0 || sf.Labels.Visible == false || true)
                    {
                        // labels aren't drawn currently
                        ret += val1;
                    }
                    else
                    {
                        // label preview is present
                        var style = new LabelStyle(sf.Labels.Options);
                        var img = new Bitmap(500, 200);
                        var g = Graphics.FromImage(img);
                        var size = style.MeasureString(g, "String", 30);
                        val2 = size.Height + 2;
                        ret += val1 > val2 ? val1 : val2;
                    }

                    if (sf.Categories.Count > 0)
                    {
                        ret += Constants.CS_ITEM_HEIGHT + 2; // caption

                        var categories = sf.Categories;
                        if (this.Type == eLayerType.LineShapefile || this.Type == eLayerType.PolygonShapefile)
                        {
                            ret += sf.Categories.Count * (Constants.CS_ITEM_HEIGHT + 2);
                        }
                        else
                        {
                            for (var i = 0; i < sf.Categories.Count; i++)
                            {
                                ret += this.Get_CategoryHeight(categories.get_Item(i).DrawingOptions);
                            }
                        }

                        ret += 2;
                    }

                    if (sf.Charts.Count > 0 && sf.Charts.NumFields > 0 && sf.Charts.Visible)
                    {
                        ret += (Constants.CS_ITEM_HEIGHT + 2); // caption
                        ret += sf.Charts.IconHeight;
                        ret += 2;

                        ret += (sf.Charts.NumFields * (Constants.CS_ITEM_HEIGHT + 2));
                    }
                }
                else
                {
                    ret = Constants.ITEM_HEIGHT;
                }
            }

            this.m_height = ret; // cahching height here to get ri of recalculation when there are lots of categories

            return ret;
        }

        /// <summary>
        /// 计算层的高度
        /// </summary>
        protected internal int CalcHeight()
        {
            return this.CalcHeight(this.Expanded);
        }

        #endregion

    }
}
