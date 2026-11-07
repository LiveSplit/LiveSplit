using System;
using System.Drawing;

namespace LiveSplit.UI;

public static class DpiScale
{
    public const int DefaultDpi = 96;

    public static int NormalizeDpi(int dpi)
        => dpi > 0 ? dpi : DefaultDpi;

    public static float GetFactor(int dpi)
        => NormalizeDpi(dpi) / (float)DefaultDpi;

    public static int Scale(int value, int dpi)
    {
        if (value < 0)
        {
            return value;
        }

        return (int)Math.Round(value * GetFactor(dpi), MidpointRounding.AwayFromZero);
    }

    public static int Unscale(int value, int dpi)
    {
        if (value < 0)
        {
            return value;
        }

        return (int)Math.Round(value / GetFactor(dpi), MidpointRounding.AwayFromZero);
    }

    public static Size Scale(Size value, int dpi)
        => new(Scale(value.Width, dpi), Scale(value.Height, dpi));

    public static Size Unscale(Size value, int dpi)
        => new(Unscale(value.Width, dpi), Unscale(value.Height, dpi));
}
