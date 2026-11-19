using LiveSplit.UI;
using System.Drawing;
using Xunit;

namespace LiveSplit.Tests.UI;

public class SimpleLabelDpiMust
{
    [Fact]
    public void MeasureMonospacedPixelTextConsistentlyAcrossDpi()
    {
        using var font = new Font("Arial", 24f, FontStyle.Regular, GraphicsUnit.Pixel);
        var label96 = CreateLabel(font);
        var label192 = CreateLabel(font);

        using var bitmap96 = new Bitmap(600, 200);
        bitmap96.SetResolution(96, 96);
        using var graphics96 = Graphics.FromImage(bitmap96);
        label96.SetActualWidth(graphics96);

        using var bitmap192 = new Bitmap(600, 200);
        bitmap192.SetResolution(192, 192);
        using var graphics192 = Graphics.FromImage(bitmap192);
        label192.SetActualWidth(graphics192);

        Assert.Equal(label96.ActualWidth, label192.ActualWidth);
    }

    private static SimpleLabel CreateLabel(Font font)
    {
        return new SimpleLabel("88:88:88", font: font, width: 493, height: 150)
        {
            IsMonospaced = true
        };
    }
}
