using LiveSplit.UI;
using System.Drawing;
using Xunit;

namespace LiveSplit.Tests.UI;

public class DpiScaleMust
{
    [Theory]
    [InlineData(96, 286, 286, 519, 519)]
    [InlineData(120, 286, 358, 519, 649)]
    [InlineData(144, 286, 429, 519, 779)]
    [InlineData(192, 286, 572, 519, 1038)]
    public void ScaleLayoutSizeToDevicePixels(
        int dpi,
        int logicalWidth,
        int expectedWidth,
        int logicalHeight,
        int expectedHeight)
    {
        Size scaled = DpiScale.Scale(new Size(logicalWidth, logicalHeight), dpi);

        Assert.Equal(new Size(expectedWidth, expectedHeight), scaled);
    }

    [Theory]
    [InlineData(96, 286, 286)]
    [InlineData(120, 358, 286)]
    [InlineData(144, 429, 286)]
    [InlineData(192, 572, 286)]
    public void PersistLayoutSizeIn96DpiUnits(int dpi, int deviceWidth, int expectedWidth)
    {
        Size logical = DpiScale.Unscale(new Size(deviceWidth, deviceWidth), dpi);

        Assert.Equal(expectedWidth, logical.Width);
        Assert.Equal(expectedWidth, logical.Height);
    }

    [Theory]
    [InlineData(120)]
    [InlineData(144)]
    [InlineData(192)]
    public void RoundtripDeviceIndependentLayoutSize(int dpi)
    {
        var logical = new Size(286, 519);

        Size roundtripped = DpiScale.Unscale(DpiScale.Scale(logical, dpi), dpi);

        Assert.Equal(logical, roundtripped);
    }

    [Fact]
    public void PreserveInvalidLayoutSizes()
    {
        var invalid = new Size(LiveSplit.UI.Layout.InvalidSize, LiveSplit.UI.Layout.InvalidSize);

        Assert.Equal(invalid, DpiScale.Scale(invalid, 144));
        Assert.Equal(invalid, DpiScale.Unscale(invalid, 144));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FallBackTo96DpiForInvalidDpiValues(int dpi)
    {
        Assert.Equal(286, DpiScale.Scale(286, dpi));
    }
}
