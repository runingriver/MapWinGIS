using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MapWinGIS;

namespace MapWinGIS.LegendControl
{
    /// <summary>
    /// 封装label属性和drawing的类
    /// </summary>
    public class LabelStyle : MapWinGIS.LabelCategoryClass
    {
        //LabelCategoryClass继承自LabelCategory、ILabelCategory
        public LabelStyle(LabelCategory cat)
        {
            PropertyInfo[] props = cat.GetType().GetProperties(); //类型+ 名称
            foreach (PropertyInfo prop in props)
            {
                PropertyInfo propDest = this.GetType().GetProperty(prop.Name);
                propDest.SetValue(this, prop.GetValue(cat, null), null);
            }
        }

        /// <summary>
        /// 在当前选项中绘制的字符串的大小
        /// </summary>
        public Size MeasureString(Graphics g, string s, int maxFontSize)
        {
            int fontSize = this.FontSize;
            if (maxFontSize > 0 && maxFontSize < fontSize)
            {
                fontSize = maxFontSize;
            }

            // font options
            FontStyle style = FontStyle.Regular;
            if (this.FontUnderline)
            {
                style |= FontStyle.Underline;
            }

            if (this.FontBold)
            {
                style |= FontStyle.Bold;
            }

            if (this.FontItalic)
            {
                style |= FontStyle.Italic;
            }

            if (this.FontStrikeOut)
            {
                style |= FontStyle.Strikeout;
            }

            Font font = new Font(this.FontName, fontSize, style);
            StringFormat format = this.StringFormatByAlignment(this.InboxAlignment);

            SizeF sizef = g.MeasureString(s, font);
            Size size = new Size((int)sizef.Width, (int)sizef.Height);
            size.Width += 1;
            size.Height += 1;

            if (this.FrameVisible)
            {
                size.Width += this.FramePaddingX;
                size.Height += this.FramePaddingY;
            }

            return size;
        }

        /// <summary>
        /// 通过给定的OLE_COLOR和alpha的值，返回一个Color对象
        /// </summary>
        public Color GetColor(uint color, int alpha)
        {
            if (alpha != 255)
            {
                return Color.FromArgb(alpha, ColorTranslator.FromOle(Convert.ToInt32(color)));
            }
            else
            {
                return ColorTranslator.FromOle(Convert.ToInt32(color));
            }
        }

        //绘制lable标签的样式
        #region lable标签区域绘制
        /// <summary>
        /// 用label目录中的选项绘制label（矩形）
        /// </summary>
        /// <param name="g">制图对象</param>
        /// <param name="pntOrigin">开始绘制的位置</param>
        /// <param name="s">绘制的字符串</param>
        /// <param name="useAlignment">是否使用对齐方式</param>
        /// <param name="maxFontSize">可以使用的最大字体</param>
        public Rectangle Draw(Graphics g, System.Drawing.Point pntOrigin, string s, bool useAlignment, int maxFontSize)
        {
            int fontSize = this.FontSize;
            if (maxFontSize > 0 && maxFontSize < fontSize)
            {
                fontSize = maxFontSize;
            }

            // 字体选项
            FontStyle style = FontStyle.Regular;
            if (this.FontUnderline)
            {
                style |= FontStyle.Underline;
            }

            if (this.FontBold)
            {
                style |= FontStyle.Bold;
            }

            if (this.FontItalic)
            {
                style |= FontStyle.Italic;
            }

            if (this.FontStrikeOut)
            {
                style |= FontStyle.Strikeout;
            }

            Font font = new Font(this.FontName, fontSize, style);
            StringFormat format = this.StringFormatByAlignment(this.InboxAlignment);

            SizeF sizef = g.MeasureString(s, font);
            Size size = new Size((int)sizef.Width, (int)sizef.Height);
            Rectangle rect = new Rectangle(pntOrigin, size);
            rect.Height += 1;   // to avoid clipping he letters in some cases
            rect.Width += 1;

            if (useAlignment)
            {
                this.AlignRectangle(ref rect, this.Alignment);

                // offset
                rect.X += (int)this.OffsetX;
                rect.Y += (int)this.OffsetY;
            }

            if (this.FrameVisible)
            {
                rect.Width += this.FramePaddingX;
                rect.Height += this.FramePaddingY;
            }

            // drawing a frame
            if ((this.FrameTransparency != 0) && this.FrameVisible)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                Pen penFrame = new Pen(this.GetColor(this.FrameOutlineColor, this.FrameTransparency), this.FrameOutlineWidth);  // Colors.IntegerToColor(base.FrameOutlineColor));
                penFrame.DashStyle = (DashStyle)this.FrameOutlineStyle;
                if (this.FrameGradientMode != MapWinGIS.tkLinearGradientMode.gmNone)
                {
                    LinearGradientBrush lgb = new LinearGradientBrush(
                                                                      rect,
                                                                      this.GetColor(this.FrameBackColor, this.FrameTransparency),
                                                                      this.GetColor(this.FrameBackColor2, this.FrameTransparency),
                                                                      (LinearGradientMode)this.FrameGradientMode);
                    this.DrawLabelFrame(g, lgb, penFrame, rect);
                    lgb.Dispose();
                }
                else
                {
                    SolidBrush brush = new SolidBrush(this.GetColor(this.FrameBackColor, this.FrameTransparency));
                    this.DrawLabelFrame(g, brush, penFrame, rect);
                    brush.Dispose();
                }

                penFrame.Dispose();
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
            }

            // drawing the label itself
            if (this.FontTransparency != 0)
            {
                GraphicsPath path = new GraphicsPath();
                path.StartFigure();
                path.AddString(s, font.FontFamily, (int)font.Style, (float)fontSize * 96f / 72f, rect, format);
                path.CloseFigure();

                // shadow
                if (this.ShadowVisible)
                {
                    SolidBrush brushShadow = new SolidBrush(this.GetColor(this.ShadowColor, this.FontTransparency));
                    Matrix mtx = new Matrix();
                    mtx.Translate(this.ShadowOffsetX, this.ShadowOffsetY);
                    path.Transform(mtx);
                    g.FillPath(brushShadow, path);
                    mtx.Translate(-2 * this.ShadowOffsetX, -2 * this.ShadowOffsetY);
                    path.Transform(mtx);
                    mtx.Dispose();
                }

                // halo
                if (this.HaloVisible)
                {
                    float width = (float)font.Size / 16.0f * (float)this.HaloSize;
                    Pen penHalo = new Pen(this.GetColor(this.HaloColor, this.FontTransparency), width);
                    penHalo.LineJoin = LineJoin.Round;
                    g.DrawPath(penHalo, path);
                    penHalo.Dispose();
                }

                // font outline
                if (this.FontOutlineVisible)
                {
                    Pen penOutline = new Pen(this.GetColor(this.FontOutlineColor, this.FontTransparency), this.FontOutlineWidth);
                    penOutline.LineJoin = LineJoin.Round;
                    g.DrawPath(penOutline, path);
                    penOutline.Dispose();
                }

                // the font itself
                if (this.FontGradientMode != MapWinGIS.tkLinearGradientMode.gmNone)
                {
                    LinearGradientBrush lgb = new LinearGradientBrush(
                                                                        rect,
                                                                        this.GetColor(this.FontColor, this.FontTransparency),
                                                                        this.GetColor(this.FontColor2, this.FontTransparency),
                                                                        (LinearGradientMode)this.FontGradientMode);
                    g.FillPath(lgb, path);
                    lgb.Dispose();
                }
                else
                {
                    SolidBrush brush = new SolidBrush(this.GetColor(this.FontColor, this.FontTransparency));
                    g.FillPath(brush, path);
                    brush.Dispose();
                }

                path.Dispose();
            }   // (fontTransparency != 0)

            return rect;
        }

        /// <summary>
        /// 根据对齐方式选项绘制字符串文本的布局
        /// </summary>
        private StringFormat StringFormatByAlignment(MapWinGIS.tkLabelAlignment alignment)
        {
            StringFormat fmt = new StringFormat();
            switch (alignment)
            {
                case MapWinGIS.tkLabelAlignment.laCenter:
                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Center;
                    break;
                case MapWinGIS.tkLabelAlignment.laCenterLeft:
                    fmt.Alignment = StringAlignment.Near;
                    fmt.LineAlignment = StringAlignment.Center;
                    break;
                case MapWinGIS.tkLabelAlignment.laCenterRight:
                    fmt.Alignment = StringAlignment.Far;
                    fmt.LineAlignment = StringAlignment.Center;
                    break;
                case MapWinGIS.tkLabelAlignment.laBottomCenter:
                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Far;
                    break;
                case MapWinGIS.tkLabelAlignment.laBottomLeft:
                    fmt.Alignment = StringAlignment.Near;
                    fmt.LineAlignment = StringAlignment.Far;
                    break;
                case MapWinGIS.tkLabelAlignment.laBottomRight:
                    fmt.Alignment = StringAlignment.Far;
                    fmt.LineAlignment = StringAlignment.Far;
                    break;
                case MapWinGIS.tkLabelAlignment.laTopCenter:
                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Near;
                    break;
                case MapWinGIS.tkLabelAlignment.laTopLeft:
                    fmt.Alignment = StringAlignment.Near;
                    fmt.LineAlignment = StringAlignment.Near;
                    break;
                case MapWinGIS.tkLabelAlignment.laTopRight:
                    fmt.Alignment = StringAlignment.Far;
                    fmt.LineAlignment = StringAlignment.Near;
                    break;
            }

            return fmt;
        }

        /// <summary>
        /// 根据原始点调整矩形label，使其成一行
        /// </summary>
        private void AlignRectangle(ref Rectangle r, tkLabelAlignment alignment)
        {
            switch (alignment)
            {
                case tkLabelAlignment.laTopLeft:
                    r.X -= r.Width;
                    r.Y -= r.Height;
                    break;
                case tkLabelAlignment.laTopCenter:
                    r.X -= r.Width / 2;
                    r.Y -= r.Height;
                    break;
                case tkLabelAlignment.laTopRight:
                    r.X += 0;
                    r.Y -= r.Height;
                    break;
                case tkLabelAlignment.laCenterLeft:
                    r.X -= r.Width;
                    r.Y -= r.Height / 2;
                    break;
                case tkLabelAlignment.laCenter:
                    r.X -= r.Width / 2;
                    r.Y -= r.Height / 2;
                    break;
                case tkLabelAlignment.laCenterRight:
                    r.X += 0;
                    r.Y -= r.Height / 2;
                    break;
                case tkLabelAlignment.laBottomLeft:
                    r.X -= r.Width;
                    r.Y += 0;
                    break;
                case tkLabelAlignment.laBottomCenter:
                    r.X -= r.Width / 2;
                    r.Y += 0;
                    break;
                case tkLabelAlignment.laBottomRight:
                    // rect.MoveToXY(0, 0);
                    break;
            }
            return;
        }

        /// <summary>
        /// 为label绘制一个框架
        /// </summary>
        /// <param name="g">制图对象</param>
        /// <param name="brush">绘制框架背景刷对象</param>
        /// <param name="pen">绘制框架轮廓的画笔对象</param>
        /// <param name="rect">要绘制的矩形</param>
        private void DrawLabelFrame(Graphics g, Brush brush, Pen pen, Rectangle rect)
        {
            switch (this.FrameType)
            {
                case tkLabelFrameType.lfRectangle:
                    {
                        g.FillRectangle(brush, rect);
                        g.DrawRectangle(pen, rect);
                        break;
                    }

                case tkLabelFrameType.lfRoundedRectangle:
                    {
                        int left = rect.X;
                        int right = rect.X + rect.Width;
                        int top = rect.Y;
                        int bottom = rect.Y + rect.Height;

                        GraphicsPath path = new GraphicsPath();
                        path.StartFigure();

                        path.AddLine(left + rect.Height, top, right - rect.Height, top);
                        path.AddArc(right - rect.Height, top, rect.Height, rect.Height, -90.0f, 180.0f);
                        path.AddLine(right - rect.Height, bottom, left + rect.Height, bottom);
                        path.AddArc(left, top, rect.Height, rect.Height, 90.0f, 180.0f);
                        path.CloseFigure();
                        g.FillPath(brush, path);
                        g.DrawPath(pen, path);
                        path.Dispose();
                        break;
                    }

                case tkLabelFrameType.lfPointedRectangle:
                    {
                        float left = rect.X;
                        float right = rect.X + rect.Width;
                        float top = rect.Y;
                        float bottom = rect.Y + rect.Height;

                        GraphicsPath path = new GraphicsPath();
                        path.StartFigure();
                        path.AddLine(left + (rect.Height / 4), top, right - (rect.Height / 4), top);

                        path.AddLine(right - (rect.Height / 4), top, right, (top + bottom) / 2);
                        path.AddLine(right, (top + bottom) / 2, right - (rect.Height / 4), bottom);

                        path.AddLine(right - (rect.Height / 4), bottom, left + (rect.Height / 4), bottom);

                        path.AddLine(left + (rect.Height / 4), bottom, left, (top + bottom) / 2);
                        path.AddLine(left, (top + bottom) / 2, left + (rect.Height / 4), top);

                        path.CloseFigure();
                        g.FillPath(brush, path);
                        g.DrawPath(pen, path);
                        path.Dispose();
                        break;
                    }
            }
        }
        #endregion
    }
}
