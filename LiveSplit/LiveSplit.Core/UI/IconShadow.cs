using System;
using System.Drawing;

namespace LiveSplit.UI
{
    public class IconShadow
    {
        public static readonly float[] Kernel = { 0.398942f, 0.241971f, 0.053991f, 0.00443185f };

        /*public static Image GenerateMultipleFrames(Image image, float shadowStrength = 0.8f)
        {
            var dimension = new FrameDimension(image.FrameDimensionsList[0]);
            var frameCount = image.GetFrameCount(dimension);

            var gEnc = new GifBitmapEncoder();

            for (var frame = 0; frame < frameCount; ++frame)
            {
                image.SelectActiveFrame(dimension, frame);
                var sourceFrame = new Bitmap(image);

                var result = GenerateFrame(sourceFrame, shadowStrength);

                var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    result.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                gEnc.Frames.Add(BitmapFrame.Create(src));
            }
            using (var stream = new MemoryStream())
            {
                gEnc.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Image.FromStream(stream);
                return result;
            }
        }*/

        public static Bitmap Generate(Image image/*, float shadowStrength = 0.8f*/, Color shadowColor)
        {
            var shadowStrength = 0.8f;
            var red = shadowColor.R;
            var green = shadowColor.G;
            var blue = shadowColor.B;
            var alpha = 255f * (Math.Pow((shadowColor.A / 255f - 1f), 3) + 1f);
            /*var scaleWidth = 24.0f;
            var scaleHeight = 24.0f;
            if (image.Width > image.Height)
                scaleWidth *= image.Height / (float)image.Width;
            else
                scaleHeight *= image.Width / (float)image.Height;*/

            var scaledCopy = new Bitmap(image, 24, 24);//(int)(scaleWidth + 0.5f), (int)(scaleHeight + 0.5f));
            var tempImage = new Bitmap(30, 30);
            var resultImage = new Bitmap(30, 30);

            Func<int, int, float> get1 = (x, y) =>
            {
                x -= 3;
                y -= 3;
                if (x < 0 || y < 0 || x >= scaledCopy.Width || y >= scaledCopy.Height)
                    return 0.0f;
                return scaledCopy.GetPixel(x, y).A / 255.0f;
            };

            Func<int, int, float> get2 = (x, y) =>
            {
                if (x < 0 || y < 0 || x >= tempImage.Width || y >= tempImage.Height)
                    return 0.0f;
                return tempImage.GetPixel(x, y).A / 255.0f;
            };

            //Horizontal Blur
            for (var x = 0; x < tempImage.Width; ++x)
            {
                for (var y = 0; y < tempImage.Height; ++y)
                {
                    var result = Kernel[0] * get1(x, y);

                    for (int i = 1; i < 4; ++i)
                    {
                        var weight = Kernel[i];
                        result += weight * (get1(x - i, y) + get1(x + i, y));
                    }

                    tempImage.SetPixel(x, y, Color.FromArgb((int)(alpha * result + 0.5f), red, green, blue));
                }
            }

            //Vertical Blur
            for (var x = 0; x < resultImage.Width; ++x)
            {
                for (var y = 0; y < resultImage.Height; ++y)
                {
                    var result = Kernel[0] * get2(x, y);

                    for (int i = 1; i < 4; ++i)
                    {
                        var weight = Kernel[i];
                        result += weight * (get2(x, y - i) + get2(x, y + i));
                    }

                    resultImage.SetPixel(x, y, Color.FromArgb((int)(shadowStrength * alpha * result + 0.5f), red, green, blue));
                }
            }

            return resultImage;
        }
    }
}
