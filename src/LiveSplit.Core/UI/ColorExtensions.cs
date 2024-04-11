using System.Drawing;
using static System.Convert;
using static System.Drawing.Color;
using static System.Math;

namespace LiveSplit.UI
{
    public static class ColorExtensions
    {
        public static void ToHSV(this Color color, out double hue, out double saturation, out double value)
        {
            int max = Max(color.R, Max(color.G, color.B));
            int min = Min(color.R, Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color FromHSV(double hue, double saturation, double value)
        {
            int hi = ToInt32(Floor(hue / 60)) % 6;
            double f = hue / 60 - Floor(hue / 60);

            value = value * 255;
            int v = ToInt32(value);
            int p = ToInt32(value * (1 - saturation));
            int q = ToInt32(value * (1 - f * saturation));
            int t = ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return FromArgb(255, v, t, p);
            else if (hi == 1)
                return FromArgb(255, q, v, p);
            else if (hi == 2)
                return FromArgb(255, p, v, t);
            else if (hi == 3)
                return FromArgb(255, p, q, v);
            else if (hi == 4)
                return FromArgb(255, t, p, v);
            else
                return FromArgb(255, v, p, q);
        }
    }
}
