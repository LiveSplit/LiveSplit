using LiveSplit.UI;
using System.Drawing;
using System.Windows.Forms;
using Xunit;

namespace LiveSplit.Tests.UI;

public class InvalidatorMust
{
    [Fact]
    public void TrackAnInvalidationWhenNormalPaintIsSuppressed()
    {
        using var form = new Form();
        var invalidator = new Invalidator(form)
        {
            SuppressInvalidation = true
        };

        invalidator.Invalidate(0, 0, 100, 100);
        invalidator.Invalidate(150, 25, 50, 75);

        Assert.True(invalidator.HasInvalidated);
        Assert.Equal(new Rectangle(0, 0, 200, 100), invalidator.InvalidatedBounds);
    }

    [Fact]
    public void ResetInvalidationTrackingWhenRestarted()
    {
        using var form = new Form();
        var invalidator = new Invalidator(form)
        {
            SuppressInvalidation = true
        };
        invalidator.Invalidate(0, 0, 100, 100);

        invalidator.Restart();

        Assert.False(invalidator.HasInvalidated);
        Assert.Null(invalidator.InvalidatedBounds);
        Assert.False(invalidator.SuppressInvalidation);
    }
}
