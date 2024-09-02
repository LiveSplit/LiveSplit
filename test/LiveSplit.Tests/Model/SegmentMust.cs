using System;
using System.Drawing;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

using Xunit;

using static LiveSplit.Tests.Model.Constants;

namespace LiveSplit.Tests.Model;

public class SegmentMust
{
    private static readonly Bitmap AnyBitmap = new(10, 10);
    private static readonly Time AnyPersonalBestSplitTime = Time.Zero;
    private static readonly Time AnyBestSegmentTime = new(AnyTimeSpan, AnotherTimeSpan);
    private static readonly Time AnySplitTime = new(YetAnotherTimeSpan, AnyTimeSpan);

    [Fact]
    public void BeInitializedCorrectly_WhenUsingDefaultValues()
    {
        var sut = new Segment("any name");
        Assert.Equal("any name", sut.Name);
        Assert.Equal(default, sut.PersonalBestSplitTime);
        Assert.Equal(default, sut.BestSegmentTime);
        Assert.Equal(default, sut.SplitTime);
        Assert.Null(sut.Icon);
        Assert.IsType<CompositeComparisons>(sut.Comparisons);
        Assert.NotNull(sut.SegmentHistory);
    }

    [Fact]
    public void BeInitializedCorrectly()
    {
        Segment sut = CreateSubjectUnderTest();
        VerifySegment(sut);
    }

    private static Segment CreateSubjectUnderTest()
    {
        return new Segment("any name", AnyPersonalBestSplitTime, AnyBestSegmentTime, AnyBitmap, AnySplitTime);
    }

    private static void VerifySegment(Segment sut)
    {
        Assert.Equal("any name", sut.Name);
        Assert.Equal(AnyPersonalBestSplitTime, sut.PersonalBestSplitTime);
        Assert.Equal(AnyBestSegmentTime, sut.BestSegmentTime);
        Assert.Equal(AnySplitTime, sut.SplitTime);
        Assert.Same(AnyBitmap, sut.Icon);
        Assert.IsType<CompositeComparisons>(sut.Comparisons);
        Assert.NotNull(sut.SegmentHistory);
    }

    [Fact]
    public void BeClonedCorrectly()
    {
        Segment original = CreateSubjectUnderTest();
        Segment sut = original.Clone();
        VerifySegment(sut);
    }

    [Fact]
    public void BeClonedCorrectly_WhenUsingCloneableInterface()
    {
        ICloneable original = CreateSubjectUnderTest();
        var sut = (Segment)original.Clone();
        VerifySegment(sut);
    }
}
