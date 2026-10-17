using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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

        var resultImage = new Bitmap(image, width, height);
        BitmapData data = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        try
        {
            int[] pixels = new int[width * height];
            for (int y = 0; y < height; y++)
            {
                Marshal.Copy(data.Scan0 + (y * data.Stride), pixels, y * width, width);
            }

            // Horizontal pass: pixels -> tempBuffer (4 channels per pixel)
            double[] tempBuffer = new double[width * height * 4];
            Parallel.For(0, height, y =>
            {
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        int shift = c * 8;
                        double result = kernel[0] * ((pixels[row + x] >> shift) & 0xFF);
                        for (int i = 1; i < kernelWidth; i++)
                        {
                            int left = pixels[row + Max(0, x - i)];
                            int right = pixels[row + Min(width - 1, x + i)];
                            result += kernel[i] * (((left >> shift) & 0xFF) + (double)((right >> shift) & 0xFF));
                        }

                        tempBuffer[((row + x) * 4) + c] = result / overallWeight;
                    }
                }
            });

            // Vertical pass: tempBuffer -> pixels
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int pixel = 0;
                    for (int c = 0; c < 4; c++)
                    {
                        double result = kernel[0] * tempBuffer[(((y * width) + x) * 4) + c];
                        for (int i = 1; i < kernelWidth; i++)
                        {
                            int up = Max(0, y - i);
                            int down = Min(height - 1, y + i);
                            result += kernel[i] * (tempBuffer[(((up * width) + x) * 4) + c] + tempBuffer[(((down * width) + x) * 4) + c]);
                        }

                        result /= overallWeight;
                        pixel |= (int)Round(Max(0, Min(255, result))) << (c * 8);
                    }

                    pixels[(y * width) + x] = pixel;
                }
            });

            for (int y = 0; y < height; y++)
            {
                Marshal.Copy(pixels, y * width, data.Scan0 + (y * data.Stride), width);
            }
        }
        finally
        {
            resultImage.UnlockBits(data);
        }

        return resultImage;
    }
}
