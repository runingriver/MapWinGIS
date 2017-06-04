/****************************************************************************
 * 文件名:clsPreViewMap.cs (F)
 * 描  述: 提供插件操作PreviewMap的方法。
 * **************************************************************************/

using System;
using System.IO;

namespace MapWinGIS.MainProgram
{
    public class PreviewMap : MapWinGIS.Interfaces.PreviewMap
    {
        internal bool m_ShowLocatorBox;
        internal System.Drawing.Rectangle g_ExtentsRect;
        internal bool g_Dragging;
        private int m_DrawHandle = -1;

        private int m_Color;

        public PreviewMap()
        {
            m_Color = Microsoft.VisualBasic.Information.RGB(255, 0, 0);
        }


        #region PreviewMap 接口实现
        /// <summary>
        /// 背景颜色
        /// </summary>
        public System.Drawing.Color BackColor
        {
            get { return Program.frmMain.MapPreview.CtlBackColor; }
            set { Program.frmMain.MapPreview.CtlBackColor = value; }
        }

        /// <summary>
        /// 显示的图片
        /// </summary>
        public System.Drawing.Image Picture
        {
            get
            {
                try
                {
                    if (Program.frmMain.MapPreview.NumLayers > 0)
                    {
                        MapWinGIS.Utility.ImageUtils cvter = new MapWinGIS.Utility.ImageUtils();
                        return cvter.IPictureDispToImage(((MapWinGIS.Image)(Program.frmMain.MapPreview.get_GetObject(Program.frmMain.MapPreview.get_LayerHandle(0)))).Picture);
                    }
                }
                catch (System.Exception ex)
                {
                    Program.g_error = ex.Message;
                    Program.ShowError(ex);
                }
                return null;
            }
            set
            {
                try
                {
                    if (value != null)
                    {
                        m_ShowLocatorBox = false;
                        MapWinGIS.Image img = new MapWinGIS.Image();
                        MapWinGIS.Utility.ImageUtils cvter = new MapWinGIS.Utility.ImageUtils();
                        img.Picture = (stdole.IPictureDisp)(cvter.ImageToIPictureDisp(value));
                        Program.frmMain.MapPreview.RemoveAllLayers();
                        Program.frmMain.MapPreview.AddLayer(img, true);
                        Program.frmMain.MapPreview.ZoomToMaxExtents();
                    }
                }
                catch (System.Exception ex)
                {
                    Program.ShowError(ex);
                }
            }
        }

        /// <summary>
        /// LocatorBox的颜色
        /// </summary>
        public System.Drawing.Color LocatorBoxColor
        {
            get { return ColorScheme.IntToColor(m_Color); }
            set { m_Color = ColorScheme.ColorToInt(value); }
        }

        /// <summary>
        /// 按照传过来的地图重新显示地图
        /// </summary>
        public void GetPictureFromMap()
        {
            GetPictureFromMap(false);
        }

        /// <summary>
        /// 让PreviewMap重绘地图，根据主地图中当前范围传过来的数据 (current extents).
        /// </summary>
        public void Update()
        {
            GetPictureFromMap(false);
        }

        /// <summary>
        /// 更新自己的显示的地图.
        /// <param name="updateExtents">从当前范围还是从全图范围更新Preview的视图</param>
        /// </summary>
        public void Update(MapWinGIS.Interfaces.ePreviewUpdateExtents updateExtents)
        {
            GetPictureFromMap(updateExtents == Interfaces.ePreviewUpdateExtents.FullExtents ? true : false);
        }

        /// <summary>
        /// 从指定的目录加载一张图片到PreviewMap中
        /// </summary>
        /// <param name="filename">图片的路径</param>
        /// <returns>true 加载成功，false，失败</returns>
        public bool GetPictureFromFile(string filename)
        {
            MapWinGIS.Image img = new MapWinGIS.Image();
            string extentName = MapWinGIS.Utility.MiscUtils.GetExtensionName(filename);
            if ((img.CdlgFilter.ToLower()).IndexOf(extentName.ToLower()) > 0)
            {
                if (img.Open(filename, MapWinGIS.ImageType.USE_FILE_EXTENSION, false) == false)
                {
                    Program.g_error = "打开文件并加载到Preview Map上失败";
                    return false;
                }
                else //文件打开
                {
                    string cutExtentName = filename.Substring(0, filename.Length - extentName.Length - 1);
                    string tStr = Path.GetDirectoryName(cutExtentName + ".*");
                    if (tStr != "")
                    {
                        switch (MapWinGIS.Utility.MiscUtils.GetExtensionName(tStr).ToLower())
                        {
                            case "bpw": //world类型的图片文件
                            case "gfw":
                                m_ShowLocatorBox = true;
                                break;
                            default: // 不是一个world类型的文件
                                m_ShowLocatorBox = false;
                                break;
                        }
                    }
                    Program.frmMain.MapPreview.AddLayer(img, true);
                }
            }
            else
            {
                Program.g_error = "不支持的图片格式";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭预览地图控件
        /// </summary>
        public void Close()
        {
            if (Program.frmMain.previewPanel != null)
            {
                Program.frmMain.previewPanel.Hide();
            }
        }

        /// <summary>
        /// 浮动停靠预览地图控件
        /// </summary>
        public void DockTo(MapWinGIS.Interfaces.MapWinGISDockStyle dockStyle)
        { 
            if (Program.frmMain.previewPanel != null)
            {
                Program.frmMain.previewPanel.Show();
                Program.frmMain.previewPanel.DockState = UIPanel.SimplifyDockstate(dockStyle);
            }
        }

        #endregion

        /// <summary>
        /// 更新PreviewMap的地图显示
        /// </summary>
        /// <param name="fullExtents">指示是否使用全图显示地图</param>
        public void GetPictureFromMap(bool fullExtents)
        {
            try
            {
                MapWinGIS.Extents exts;
                MapWinGIS.Image img = new MapWinGIS.Image();
                double ratio = 0;
                MapWinGIS.Extents oldExts = (MapWinGIS.Extents)Program.frmMain.MapMain.Extents;
                
                //锁定地图
                Program.frmMain.MapPreview.LockWindow(MapWinGIS.tkLockMode.lmLock);
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmLock);

                if (fullExtents) //更新为全图显示，主地图也更新为全图显示
                {
                    exts = Program.frmMain.m_View.MaxVisibleExtents;
                    if (Program.frmMain.MapPreview.Width < Program.frmMain.MapPreview.Height) //宽小于高
                    {
                        ratio = System.Convert.ToDouble(Program.frmMain.MapPreview.Width / Program.frmMain.MapMain.Width);
                    }
                    else //宽大于高
                    {
                        ratio = System.Convert.ToDouble(Program.frmMain.MapPreview.Height / Program.frmMain.MapMain.Height);
                    }
                    ratio *= 1.5; //缩放比例

                    Program.frmMain.MapMain.Extents = exts;
                    exts = (MapWinGIS.Extents)Program.frmMain.MapMain.Extents;
                }
                else
                {
                    exts = (MapWinGIS.Extents)Program.frmMain.MapMain.Extents;
                }

                img = (MapWinGIS.Image)(Program.frmMain.MapMain.SnapShot(exts));

                MapWinGIS.Utility.ImageUtils cvter = new MapWinGIS.Utility.ImageUtils();
                System.Drawing.Image tmpImg = MapWinGIS.Utility.ImageUtils.ObjectToImage(img.Picture, System.Convert.ToInt32(img.Width * ratio), System.Convert.ToInt32(img.Height * ratio));
                
                img.Picture = (stdole.IPictureDisp)(cvter.ImageToIPictureDisp(tmpImg));
                img.dX = (exts.xMax - exts.xMin) / img.Width;
                img.dY = (exts.yMax - exts.yMin) / img.Height;
                img.XllCenter = exts.xMin + 0.5 * img.dX;
                img.YllCenter = exts.yMin + 0.5 * img.dX;
                img.DownsamplingMode = MapWinGIS.tkInterpolationMode.imHighQualityBicubic;
                img.UpsamplingMode = MapWinGIS.tkInterpolationMode.imHighQualityBicubic;

                Program.frmMain.MapPreview.RemoveAllLayers();
                Program.frmMain.MapPreview.AddLayer(img, true);
                Program.frmMain.MapPreview.ExtentPad = 0;
                Program.frmMain.MapPreview.ZoomToMaxExtents();
                Program.frmMain.m_PreviewMap.m_ShowLocatorBox = true;
                Program.frmMain.m_PreviewMap.UpdateLocatorBox();

                Program.frmMain.mnuZoomPreviewMap.Enabled = Program.frmMain.MapMain.NumLayers > 0 && Program.frmMain.PreviewMapExtentsValid();
                if (Program.frmMain.m_Menu["mnuZoomToPreviewExtents"] != null)
                {
                    Program.frmMain.m_Menu["mnuZoomToPreviewExtents"].Enabled = Program.frmMain.MapMain.NumLayers > 0 && Program.frmMain.PreviewMapExtentsValid();
                }

            }
            catch (Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
            finally
            {
                //解锁地图
                Program.frmMain.MapPreview.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                Program.frmMain.MapMain.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
            }
        }

        /// <summary>
        /// 更新当前的红色矩形盒
        /// </summary>
        internal void UpdateLocatorBox()
        {
            Program.frmMain.MapPreview.ZoomToMaxExtents();

            MapWinGIS.Extents exts = (MapWinGIS.Extents)Program.frmMain.MapMain.Extents;
            double newLeft = 0; ;
            double newRight = 0;
            double newTop = 0;
            double newBottom = 0;

            if (m_ShowLocatorBox == false) //不显示红色方框盒子，则清空返回
            {
                Program.frmMain.MapPreview.ClearDrawings();
                return;
            }

            //获取盒子线条宽度
            Program.frmMain.MapPreview.ProjToPixel(exts.xMin, exts.yMax, ref newLeft, ref newTop);
            Program.frmMain.MapPreview.ProjToPixel(exts.xMax, exts.yMin, ref newRight, ref newBottom);

            try
            {
                g_ExtentsRect = new System.Drawing.Rectangle((int)newLeft, (int)newTop, System.Convert.ToInt32(newRight - newLeft), System.Convert.ToInt32(newBottom - newTop));

                DrawBox(g_ExtentsRect);//绘制
            }
            catch
            {
                //忽略这个异常，因为可能范围溢出
            }
        }

        /// <summary>
        /// 绘制一个(红色)方框盒子
        /// </summary>
        internal void DrawBox(System.Drawing.Rectangle rect)
        {
            uint color = Convert.ToUInt32(m_Color);
            if (m_DrawHandle >= 0)
            {
                Program.frmMain.MapPreview.ClearDrawing(m_DrawHandle);
            }
            m_DrawHandle = Program.frmMain.MapPreview.NewDrawing(MapWinGIS.tkDrawReferenceList.dlScreenReferencedList);

            Program.frmMain.MapPreview.DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, 2, color);
            Program.frmMain.MapPreview.DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, 2, color);
            Program.frmMain.MapPreview.DrawLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom, 2, color);
            Program.frmMain.MapPreview.DrawLine(rect.Left, rect.Bottom, rect.Left, rect.Top, 2, color);

        }

    }
}
