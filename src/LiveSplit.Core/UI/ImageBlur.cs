using System.Drawing;
using System.Threading.Tasks;

using static System.Math;

namespace LiveSplit.UI;

public class ImageBlur
{
    public static Bitmap Generate(Image image, double sigma)
    {
        double scaleFactor = (1.5 * sigma) + 1;
        int kernelWidth = (int)Round(3 * sigma / scaleFactor);
        if (kernelWidth == 0)
        {
            return new Bitmap(image);
        }

        double[] kernel = new double[kernelWidth];
        for (int i = 0; i < kernelWidth; i++)
        {
            double x = i * scaleFactor;
            kernel[i] = Exp(-x * x / (2 * sigma * sigma)) / (sigma * Sqrt(2 * PI));
        }

        double overallWeight = kernel[0];
        for (int i = 1; i < kernelWidth; i++)
        {
            overallWeight += 2 * kernel[i];
        }

        int width = (int)Round(image.Width / scaleFactor);
        int height = (int)Round(image.Height / scaleFactor);

        var sourceImage = new Bitmap(image, width, height);
        Bitmap resultImage = sourceImage;
        var sourceBuffer = new BlurColor[width, height];
        var tempBuffer = new BlurColor[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                sourceBuffer[x, y] = new BlurColor(sourceImage.GetPixel(x, y));
            }
        }

        for (int y = 0; y < height; y++)
        {
            Parallel.For(0, width, x =>
            {
                BlurColor result = kernel[0] * GetColor(sourceBuffer, x, y);
                for (int i = 1; i < kernelWidth; i++)
                {
                    double weight = kernel[i];
                    result += weight * (GetColor(sourceBuffer, x - i, y) + GetColor(sourceBuffer, x + i, y));
                }

                result /= overallWeight;
                tempBuffer[x, y] = result;
            });
        }

        for (int y = 0; y < height; y++)
        {
            Parallel.For(0, width, x =>
            {
                BlurColor result = kernel[0] * GetColor(tempBuffer, x, y);
                for (int i = 1; i < kernelWidth; i++)
                {
                    double weight = kernel[i];
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
        x = Max(0, Min(buffer.GetLength(0) - 1, x));
        y = Max(0, Min(buffer.GetLength(1) - 1, y));
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
                BlurColor saturated = Saturate();
                return Color.FromArgb((int)Round(saturated.A), (int)Round(saturated.R), (int)Round(saturated.G), (int)Round(saturated.B));
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
            return Max(0, Min(255, value));
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
