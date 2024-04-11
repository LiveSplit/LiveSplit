using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI
{
    public static class ImageExtensions
    {
        public static Image ScaleIcon(this Image image)
        {
            return image.Scale(96);
        }

        public static Image Scale(this Image image, int maxDim)
        {
            if (image == null)
                return null;

            var width = image.Width;
            var height = image.Height;
            if (width <= maxDim && height <= maxDim)
                return image;

            using (image)
            {
                if (width > height)
                {
                    height = maxDim * height / width;
                    width = maxDim;
                }
                else
                {
                    width = maxDim * width / height;
                    height = maxDim;
                }

                var bitmap = new Bitmap(width, height, image.PixelFormat);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var attributes = new ImageAttributes();

                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(
                        image,
                        new Rectangle(0, 0, width, height),
                        0, 0, image.Width, image.Height,
                        GraphicsUnit.Pixel,
                        attributes
                    );
                }

                return bitmap;
            }
        }
    }
}
