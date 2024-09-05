using System;
using System.Drawing;

namespace LiveSplit.UI;

public class IconShadow
{
    public static readonly float[] Kernel = [0.398942f, 0.241971f, 0.053991f, 0.00443185f];

    public static Bitmap Generate(Image image, Color shadowColor)
    {
        float shadowStrength = 0.8f;
        byte red = shadowColor.R;
        byte green = shadowColor.G;
        byte blue = shadowColor.B;
        double alpha = 255f * (Math.Pow((shadowColor.A / 255f) - 1f, 3) + 1f);

        using var scaledCopy = new Bitmap(image, 24, 24);
        using var tempImage = new Bitmap(30, 30);
        var resultImage = new Bitmap(30, 30);

        float get1(int x, int y)
        {
            x -= 3;
            y -= 3;
            if (x < 0 || y < 0 || x >= scaledCopy.Width || y >= scaledCopy.Height)
            {
                return 0.0f;
            }

            return scaledCopy.GetPixel(x, y).A / 255.0f;
        }

        float get2(int x, int y)
        {
            if (x < 0 || y < 0 || x >= tempImage.Width || y >= tempImage.Height)
            {
                return 0.0f;
            }

            return tempImage.GetPixel(x, y).A / 255.0f;
        }

        //Horizontal Blur
        for (int x = 0; x < tempImage.Width; ++x)
        {
            for (int y = 0; y < tempImage.Height; ++y)
            {
                float result = Kernel[0] * get1(x, y);

                for (int i = 1; i < 4; ++i)
                {
                    float weight = Kernel[i];
                    result += weight * (get1(x - i, y) + get1(x + i, y));
                }

                tempImage.SetPixel(x, y, Color.FromArgb((int)((alpha * result) + 0.5f), red, green, blue));
            }
        }

        //Vertical Blur
        for (int x = 0; x < resultImage.Width; ++x)
        {
            for (int y = 0; y < resultImage.Height; ++y)
            {
                float result = Kernel[0] * get2(x, y);

                for (int i = 1; i < 4; ++i)
                {
                    float weight = Kernel[i];
                    result += weight * (get2(x, y - i) + get2(x, y + i));
                }

                resultImage.SetPixel(x, y, Color.FromArgb((int)((shadowStrength * alpha * result) + 0.5f), red, green, blue));
            }
        }

        return resultImage;
    }
}
