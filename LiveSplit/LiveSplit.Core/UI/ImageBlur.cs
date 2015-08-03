using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace LiveSplit.UI
{
    public class ImageBlur
    {
        public static Bitmap Generate(Image image, double sigma)
        {
            var scaleFactor = 1.5 * sigma + 1;
            var kernelWidth = (int)Math.Round((3 * sigma) / scaleFactor);
            if (kernelWidth == 0)
                return new Bitmap(image);
            var kernel = new double[kernelWidth];
            for (var i = 0; i < kernelWidth; i++)
            {
                var x = i * scaleFactor;
                kernel[i] = Math.Exp(-x * x / (2 * sigma * sigma)) / (sigma * Math.Sqrt(2 * Math.PI));
            }
            var overallWeight = kernel[0];
            for (var i = 1; i < kernelWidth; i++)
            {
                overallWeight += 2 * kernel[i];
            }

            var width = (int)Math.Round(image.Width / scaleFactor);
            var height = (int)Math.Round(image.Height / scaleFactor);

            var sourceImage = new Bitmap(image, width, height);
            var resultImage = sourceImage;
            var sourceBuffer = new BlurColor[width, height];
            var tempBuffer = new BlurColor[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    sourceBuffer[x, y] = new BlurColor(sourceImage.GetPixel(x, y));
                }
            }

                for (var y = 0; y < height; y++)
                {
                    Parallel.For(0, width, x =>
                    {
                        var result = kernel[0] * GetColor(sourceBuffer, x, y);
                        for (var i = 1; i < kernelWidth; i++)
                        {
                            var weight = kernel[i];
                            result += weight * (GetColor(sourceBuffer, x - i, y) + GetColor(sourceBuffer, x + i, y));
                        }
                        result /= overallWeight;
                        tempBuffer[x, y] = result;
                    });
                }

                for (var y = 0; y < height; y++)
                {
                    Parallel.For(0, width, x =>
                    {
                        var result = kernel[0] * GetColor(tempBuffer, x, y);
                        for (var i = 1; i < kernelWidth; i++)
                        {
                            var weight = kernel[i];
                            result += weight * (GetColor(tempBuffer, x, y - i) + GetColor(tempBuffer, x, y + i));
                        }
                        result /= overallWeight;
                        lock (resultImage)
                        {
                            resultImage.SetPixel(x, y, result.ActualColor);
                        }
                    });
                }

                return resultImage;
        }

        private static BlurColor GetColor(BlurColor[,] buffer, int x, int y)
        {
            x = Math.Max(0, Math.Min(buffer.GetLength(0) - 1, x));
            y = Math.Max(0, Math.Min(buffer.GetLength(1) - 1, y));
            return buffer[x, y];
        }

        private class BlurColor
        {
            public double R { get; private set; }
            public double G { get; private set; }
            public double B { get; private set; }
            public double A { get; private set; }

            public Color ActualColor
            {
                get
                {
                    var saturated = Saturate();
                    return Color.FromArgb((int)Math.Round(saturated.A), (int)Math.Round(saturated.R), (int)Math.Round(saturated.G), (int)Math.Round(saturated.B));
                }
            }

        public BlurColor(double r, double g, double b, double a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public BlurColor(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        private static double clamp(double value)
        {
            return Math.Max(0, Math.Min(255, value));
        }

        public BlurColor Saturate()
        {
            return new BlurColor(clamp(R), clamp(G), clamp(B), clamp(A));
        }

        public static BlurColor operator +(BlurColor a, BlurColor b)
        {
            return new BlurColor(
                a.R + b.R,
                a.G + b.G,
                a.B + b.B,
                a.A + b.A);
        }

        public static BlurColor operator +(BlurColor a, double b)
        {
            return new BlurColor(
                a.R + b,
                a.G + b,
                a.B + b,
                a.A + b);
        }

        public static BlurColor operator -(BlurColor a, BlurColor b)
        {
            return new BlurColor(
                a.R - b.R,
                a.G - b.G,
                a.B - b.B,
                a.A - b.A);
        }

        public static BlurColor operator -(BlurColor a, double b)
        {
            return new BlurColor(
                a.R - b,
                a.G - b,
                a.B - b,
                a.A - b);
        }

        public static BlurColor operator *(BlurColor a, double b)
        {
            return b * a;
        }

        public static BlurColor operator *(double c, BlurColor a)
        {
            return new BlurColor(a.R * c, a.G * c, a.B * c, a.A * c);
        }

        public static BlurColor operator /(BlurColor a, double c)
        {
            return new BlurColor(a.R / c, a.G / c, a.B / c, a.A / c);
        }
        }
    }
}
