using System.Drawing;
using System.Windows.Forms;

namespace MapWinGIS.Utility
{
    public partial class ImageUtils : System.Windows.Forms.AxHost
    {
        public ImageUtils()
            : base("75A9135C-8D64-4CE0-AD6B-EA48F7BF07D8")
        { }

        /// <summary>
        /// 将运行时的Icon和Image的object类型对象转换成Image类型对象
        /// </summary>
        /// <param name="Picture">object类型的图片对戏</param>
        /// <param name="newWidth">图片宽</param>
        /// <param name="newHeight">图片高</param>
        public static System.Drawing.Image ObjectToImage(object Picture, int newWidth = -1, int newHeight = -1)
        {
            System.Drawing.Image img = null;
            if (Picture is System.Drawing.Icon)
            {
                img = (System.Drawing.Image)(((System.Drawing.Icon)Picture).ToBitmap());
            }
            else if (Picture is System.Drawing.Image)
            {
                img = (System.Drawing.Image)((System.Drawing.Image)Picture);
            }
            else if (Picture is stdole.IPictureDisp)
            {
                stdole.IPictureDisp ipdisp = (stdole.IPictureDisp)Picture;

                const int PIC_BITMAP = 1;
                const int PIC_ICON = 3;

                if (ipdisp.Type == PIC_BITMAP)
                {
                    ImageUtils cvter = new ImageUtils();
                    img = cvter.IPictureDispToImage(Picture);
                }
                else if (ipdisp.Type == PIC_ICON)
                {
                    throw new System.Exception("暂不支持Icons类型的图像");
                }
                else
                {
                    throw new System.Exception("不支持的类型图像"); 
                }
            }

            System.Drawing.Image retval;

            if (newHeight > 0 && newWidth > 0)
            {
                retval = new System.Drawing.Bitmap(newWidth, newHeight);
                System.Drawing.Graphics drawtool = System.Drawing.Graphics.FromImage(retval);
                if (img != null)
                {
                    drawtool.DrawImage(img, new Rectangle(0, 0, newWidth, newHeight));
                }
            }
            else
            {
                retval = img;
            }

            return retval;
        }

        public object ImageToIPictureDisp(System.Drawing.Image image)
        {
            return AxHost.GetIPictureDispFromPicture(image);
        }

        public System.Drawing.Image IPictureDispToImage(object image)
        {
            return AxHost.GetPictureFromIPicture(image);
        }


    }
}
