/****************************************************************************
 * 文件名:clsDraw.cs （F）
 * 描  述:该类是开放给插件的，在层上绘制各种图形以及操作绘制的图形的方法。
 * **************************************************************************/

namespace MapWinGIS.MainProgram
{
    public class Draw : MapWinGIS.Interfaces.Draw
    {
        /// <summary>
        /// 在指定的层上清空所有drawing
        /// 只清空单一的drawing，比清空所有的drawing要快
        /// </summary>
        /// <param name="DrawHandle">要清空的层的DrawHandle</param>
        public void ClearDrawing(int drawHandle)
        {
            Program.frmMain.MapMain.ClearDrawing(drawHandle);
        }

        /// <summary>
        /// 在所有正在绘制的层上清空所有自定义绘制的对象
        /// </summary>
        public void ClearDrawings()
        {
            Program.frmMain.MapMain.ClearDrawings();
        }

        /// <summary>
        /// 在当前绘制的层上绘制一个圆
        /// </summary>
        /// <param name="x">圆点x点坐标</param>
        /// <param name="y">圆点y坐标</param>
        /// <param name="PixelRadius">半径，单位：像素</param>
        /// <param name="Color">园的颜色</param>
        /// <param name="FillCircle">是否填充</param>
        public void DrawCircle(double x, double y, double pixelRadius, System.Drawing.Color color, bool fillCircle)
        {
            Program.frmMain.MapMain.DrawCircle(x, y, pixelRadius, ColorScheme.ColorToUInt(color), fillCircle);
        }
        
        /// <summary>
        ///在当前绘制的层上绘制一条线
        /// </summary>
        /// <param name="X1">起点x坐标</param>
        /// <param name="Y1">起点y坐标</param>
        /// <param name="X2">终点x坐标</param>
        /// <param name="Y2">终点y坐标.</param>
        /// <param name="PixelWidth">线宽，单位：像素</param>
        /// <param name="Color">线的颜色</param>
        public void DrawLine(double X1, double Y1, double X2, double Y2, int pixelWidth, System.Drawing.Color color)
        {
            Program.frmMain.MapMain.DrawLine(X1, Y1, X2, Y2, pixelWidth, ColorScheme.ColorToUInt(color));
        }

        /// <summary>
        /// 在当前绘制的层上，绘制一个点
        /// </summary>
        /// <param name="x">点的x坐标</param>
        /// <param name="y">点的y坐标</param>
        /// <param name="PixelSize">点的大小，单位：像素.</param>
        /// <param name="Color">点的颜色</param>
        public void DrawPoint(double x, double y, int pixelSize, System.Drawing.Color color)
        {
            Program.frmMain.MapMain.DrawPoint(x, y, pixelSize, ColorScheme.ColorToUInt(color));
        }

        /// <summary>
        /// 在当前绘制的层上，绘制一个多边形
        /// </summary>
        /// <param name="x">多边形x坐标数组</param>
        /// <param name="y">多边形y坐标数组</param>
        /// <param name="Color">绘制多边形的颜色</param>
        /// <param name="FillPolygon">是否填充多边形.</param>
        /// <remarks>点要以顺时针存放，并且如果要填充颜色的话要求没有交叉，起点要与终点相同</remarks>
        public void DrawPolygon(double[] x, double[] y, System.Drawing.Color color, bool fillPolygon)
        {
            object xPoints = x;
            object yPoints = y;
            Program.frmMain.MapMain.DrawPolygon(ref xPoints, ref yPoints, y.Length, ColorScheme.ColorToUInt(color), fillPolygon);
        }

        /// <summary>
        /// 创建一个新的绘制层
        /// Creates a new drawing layer.
        /// </summary>
        /// <param name="Projection">指定是用屏幕坐标还是使用地图坐标</param>
        /// <returns>返回绘制的句柄handle.  如果以后要清除这个绘制的层，应该保存这个handle</returns>
        /// <remarks>图层绘制，在这个版本只有部分功能实现了</remarks>
        public int NewDrawing(MapWinGIS.tkDrawReferenceList projection)
        {
            int returnValue = default(int);
            returnValue = System.Convert.ToInt32(Program.frmMain.MapMain.NewDrawing(projection));
            return returnValue;
        }
        
        /// <summary>
        /// 是否使用双缓冲，这样可以使得绘制的对象显得圆滑
        /// </summary>
        public bool DoubleBuffer
        {
            get
            {
                bool returnValue = default(bool);
                returnValue = System.Convert.ToBoolean(Program.frmMain.MapMain.DoubleBuffer);
                return returnValue;
            }
            set
            {
                Program.frmMain.MapMain.DoubleBuffer = value;
            }
        }

        /// <summary>
        /// 给当前绘制的层添加一个标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="Text">标签文本</param>
        /// <param name="Color">文本颜色</param>
        /// <param name="x">在地图上的x坐标</param>
        /// <param name="y">在地图上的y坐标</param>
        /// <param name="hJustification">文本对齐方式.</param>
        public void AddDrawingLabel(int drawHandle, string Text, System.Drawing.Color color, double x, double y, MapWinGIS.tkHJustification hJustification)
        {
            Program.frmMain.MapMain.AddDrawingLabel(drawHandle, Text, ColorScheme.ColorToUInt(color), x, y, hJustification);
        }

        /// <summary>
        /// 给当前新绘制的层添加一个标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="Text">标签文本</param>
        /// <param name="Color">文本颜色t</param>
        /// <param name="x">在地图上的x坐标</param>
        /// <param name="y">在地图上的y坐标</param>
        /// <param name="hJustification">文本对齐方式</param>
        /// <param name="Rotation">标签的旋转角度</param>
        public void AddDrawingLabelEx(int drawHandle, string text, System.Drawing.Color color, double x, double y, MapWinGIS.tkHJustification hJustification, double rotation)
        {
            Program.frmMain.MapMain.AddDrawingLabelEx(drawHandle, text, ColorScheme.ColorToUInt(color), x, y, hJustification, rotation);
        }

        /// <summary>
        /// 在当前新绘制的层上清除所有的标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        public void ClearDrawingLabels(int drawHandle)
        {
            Program.frmMain.MapMain.ClearDrawingLabels(drawHandle);
        }

        /// <summary>
        /// 在当前新绘制的层上绘制一个圆
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="x">圆点x坐标</param>
        /// <param name="y">圆点y坐标</param>
        /// <param name="pixelRadius">半径，单位：像素</param>
        /// <param name="Color">颜色</param>
        /// <param name="FillCircle">是否填充/param>
        public void DrawCircleEx(int drawHandle, double x, double y, double pixelRadius, System.Drawing.Color color, bool fill)
        {
            Program.frmMain.MapMain.DrawCircleEx(drawHandle, x, y, pixelRadius, ColorScheme.ColorToUInt(color), fill);
        }

        /// <summary>
        /// 在新绘制的层上，绘制条线
        /// </summary>
        public void DrawLineEx(int drawHandle, double x1, double y1, double x2, double y2, int pixelWidth, System.Drawing.Color color)
        {
            Program.frmMain.MapMain.DrawLineEx(drawHandle, x1, y1, x2, y2, pixelWidth, ColorScheme.ColorToUInt(color));
        }

        /// <summary>
        /// 在新绘制的层上，绘制一个点
        /// </summary>
        public void DrawPointEx(int drawHandle, double x, double y, int pixelSize, System.Drawing.Color color)
        {
            Program.frmMain.MapMain.DrawPointEx(drawHandle, x, y, pixelSize, ColorScheme.ColorToUInt(color));
        }

        /// <summary>
        /// 在新绘制的层上，绘制一个多边形
        /// </summary>
        public void DrawPolygonEx(int drawHandle, double[] x, double[] y, System.Drawing.Color Color, bool fillPolygon)
        {
            object xPoints = x;
            object yPoints = y;
            Program.frmMain.MapMain.DrawPolygonEx(drawHandle, ref xPoints, ref yPoints, y.Length, ColorScheme.ColorToUInt(Color), fillPolygon);
        }

        /// <summary>
        /// 在当前绘制的层上，绘制一个指定线宽的圆
        /// </summary>
        public void DrawWideCircle(double x, double y, double pixelRadius, System.Drawing.Color color, bool fill, short width)
        {
            Program.frmMain.MapMain.DrawWideCircle(x, y, pixelRadius, ColorScheme.ColorToUInt(color), fill, width);
        }
       
        /// <summary>
        /// 在当前绘制的层上，绘制一个指定线宽的多边形
        /// </summary>
        public void DrawWidePolygon(double[] x, double[] y, System.Drawing.Color color, bool fillPolygon, short width)
        {
            object xPoints = x;
            object yPoints = y;
            Program.frmMain.MapMain.DrawWidePolygon(ref xPoints, ref yPoints, y.Length, ColorScheme.ColorToUInt(color), fillPolygon, width);
        }
      
        /// <summary>
        /// 在当前绘制的层上设置字体
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="fontName">字体的名字</param>
        /// <param name="fontSize">字体的大小</param>
        public void DrawingFont(int drawHandle, string fontName, int fontSize)
        {
            Program.frmMain.MapMain.DrawingFont(drawHandle, fontName, fontSize);
        }
      
        /// <summary>
        /// 设置是否显示这个图层
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="visible">Visible or not</param>
        public void SetDrawingLayerVisible(int drawHandle, bool visible)
        {
            Program.frmMain.MapMain.SetDrawingLayerVisible(drawHandle, visible);
        }
     
        /// <summary>
        /// 设置在当前图层上，标签是否显示
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="visible">Visible or not</param>
        public void SetDrawingLabelsVisible(int drawHandle, bool visible)
        {
            Program.frmMain.MapMain.set_DrawingLabelsVisible(drawHandle, visible);
        }
    
        /// <summary>
        /// 在新绘制的层上，绘制一个指定线宽的多边形
        /// </summary>
        public void DrawWidePolygonEx(int drawHandle, double[] x, double[] y, System.Drawing.Color color, bool fillPolygon, short pixelWidth)
        {
            object xPoints = x;
            object yPoints = y;
            Program.frmMain.MapMain.DrawWidePolygonEx(drawHandle, ref xPoints, ref yPoints, y.Length, ColorScheme.ColorToUInt(color), fillPolygon, pixelWidth);
        }
    
        /// <summary>
        /// 在新绘制的层上，绘制一个指定线宽的原
        /// Draws a circle on the current drawing layer and specify the width of the line
        /// </summary>
        public void DrawWideCircleEx(int drawHandle, double x, double y, double radius, System.Drawing.Color color, bool FillPolygon, short pixelWidth)
        {
            Program.frmMain.MapMain.DrawWideCircleEx(drawHandle, x, y, radius, ColorScheme.ColorToUInt(color), FillPolygon, pixelWidth);
        }
    }
}
