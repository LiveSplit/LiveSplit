using LiveSplit.Model;
using LiveSplit.Options;
using System.Drawing;
using Xunit;

namespace LiveSplit.Tests.Options;

public class LayoutSettingsMust
{
    [Fact]
    public void RememberValuesCorrectly()
    {
        LayoutSettings sut = CreateSubjectUnderTest();
        ValidateSubject(sut);
    }

    private static LayoutSettings CreateSubjectUnderTest()
    {
        return new LayoutSettings
        {
            TextColor = Color.Yellow,
            AheadGainingTimeColor = Color.Red,
            AheadLosingTimeColor = Color.AliceBlue,
            AlwaysOnTop = true,
            AntiAliasing = true,
            BackgroundColor = Color.Red,
            BackgroundColor2 = Color.Transparent,
            BackgroundImage = new Bitmap(10, 10),
            BackgroundType = BackgroundType.HorizontalGradient,
            BehindGainingTimeColor = Color.Magenta,
            BehindLosingTimeColor = Color.IndianRed,
            BestSegmentColor = Color.AntiqueWhite,
            DropShadows = true,
            ImageBlur = 4.5F,
            ImageOpacity = 1,
            MousePassThroughWhileRunning = true,
            AllowResizing = true,
            AllowMoving = true,
            NotRunningColor = Color.Gray,
            Opacity = 9.1F,
            PausedColor = Color.HotPink,
            PersonalBestColor = Color.Aqua,
            SeparatorsColor = Color.White,
            ShadowsColor = Color.Brown,
            ShowBestSegments = true,
            TextFont = new Font("Arial", 8.0F),
            TextOutlineColor = Color.CadetBlue,
            ThinSeparatorsColor = Color.Chartreuse,
            TimerFont = new Font("Arial", 9.0F),
            TimesFont = new Font("Arial", 10.0F)
        };
    }

    private static void ValidateSubject(LayoutSettings sut)
    {
        Assert.Equal(Color.Yellow, sut.TextColor);
        Assert.Equal(Color.Red, sut.AheadGainingTimeColor);
        Assert.Equal(Color.AliceBlue, sut.AheadLosingTimeColor);
        Assert.True(sut.AlwaysOnTop);
        Assert.True(sut.AntiAliasing);
        Assert.Equal(Color.Red, sut.BackgroundColor);
        Assert.Equal(Color.Transparent, sut.BackgroundColor2);
        Assert.NotNull(sut.BackgroundImage);
        Assert.Equal(10, sut.BackgroundImage.Width);
        Assert.Equal(10, sut.BackgroundImage.Height);
        Assert.Equal(BackgroundType.HorizontalGradient, sut.BackgroundType);
        Assert.Equal(Color.Magenta, sut.BehindGainingTimeColor);
        Assert.Equal(Color.IndianRed, sut.BehindLosingTimeColor);
        Assert.Equal(Color.AntiqueWhite, sut.BestSegmentColor);
        Assert.True(sut.DropShadows);
        Assert.Equal(4.5F, sut.ImageBlur);
        Assert.Equal(1, sut.ImageOpacity);
        Assert.True(sut.MousePassThroughWhileRunning);
        Assert.True(sut.AllowResizing);
        Assert.True(sut.AllowMoving);
        Assert.Equal(Color.Gray, sut.NotRunningColor);
        Assert.Equal(9.1F, sut.Opacity);
        Assert.Equal(Color.HotPink, sut.PausedColor);
        Assert.Equal(Color.Aqua, sut.PersonalBestColor);
        Assert.Equal(Color.White, sut.SeparatorsColor);
        Assert.Equal(Color.Brown, sut.ShadowsColor);
        Assert.True(sut.ShowBestSegments);
        Assert.NotNull(sut.TextFont);
        Assert.Equal("Arial", sut.TextFont.Name);
        Assert.Equal(Color.CadetBlue, sut.TextOutlineColor);
        Assert.Equal(Color.Chartreuse, sut.ThinSeparatorsColor);
        Assert.Equal("Arial", sut.TimerFont.Name);
        Assert.Equal("Arial", sut.TimesFont.Name);
    }

    [Fact]
    public void CloneLayoutSettingsCorrectly()
    {
        LayoutSettings original = CreateSubjectUnderTest();
        var sut = (LayoutSettings)original.Clone();
        ValidateSubject(sut);
    }

    [Fact]
    public void AssignLayoutSettingsCorrectly()
    {
        LayoutSettings original = CreateSubjectUnderTest();
        var sut = new LayoutSettings();
        sut.Assign(original);
        ValidateSubject(sut);
    }

    [Theory]
    [InlineData(true, TimerPhase.Running, false, true)]
    [InlineData(true, TimerPhase.Running, true, false)]
    [InlineData(true, TimerPhase.NotRunning, false, false)]
    [InlineData(true, TimerPhase.Paused, false, false)]
    [InlineData(false, TimerPhase.Running, false, false)]
    public void IgnoreMouseOnlyWhenEnabledRunningAndUnfocused(
        bool enabled,
        TimerPhase phase,
        bool isForeground,
        bool expected)
    {
        var sut = new LayoutSettings
        {
            MousePassThroughWhileRunning = enabled
        };

        Assert.Equal(expected, sut.ShouldIgnoreMouse(phase, isForeground));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(128, true)]
    [InlineData(255, false)]
    public void RequireTransparentWindowForSolidColorAlpha(int alpha, bool expected)
    {
        var sut = new LayoutSettings
        {
            BackgroundType = BackgroundType.SolidColor,
            BackgroundColor = Color.FromArgb(alpha, 1, 2, 3)
        };

        Assert.Equal(expected, sut.RequiresTransparentWindow);
    }

    [Theory]
    [InlineData(0, 255, true)]
    [InlineData(255, 0, true)]
    [InlineData(255, 255, false)]
    public void RequireTransparentWindowForGradientAlpha(int alpha1, int alpha2, bool expected)
    {
        var sut = new LayoutSettings
        {
            BackgroundType = BackgroundType.VerticalGradient,
            BackgroundColor = Color.FromArgb(alpha1, 1, 2, 3),
            BackgroundColor2 = Color.FromArgb(alpha2, 4, 5, 6)
        };

        Assert.Equal(expected, sut.RequiresTransparentWindow);
    }

    [Fact]
    public void RequireTransparentWindowForTransparentImage()
    {
        var sut = new LayoutSettings
        {
            BackgroundType = BackgroundType.Image,
            BackgroundImage = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb),
            ImageOpacity = 1f
        };

        Assert.True(sut.RequiresTransparentWindow);
    }

    [Fact]
    public void RequireTransparentWindowForOpaqueImageWithReducedOpacity()
    {
        var sut = new LayoutSettings
        {
            BackgroundType = BackgroundType.Image,
            BackgroundImage = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format24bppRgb),
            ImageOpacity = 0.5f
        };

        Assert.True(sut.RequiresTransparentWindow);
    }

    [Fact]
    public void NotRequireTransparentWindowForOpaqueImage()
    {
        var sut = new LayoutSettings
        {
            BackgroundType = BackgroundType.Image,
            BackgroundImage = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format24bppRgb),
            ImageOpacity = 1f
        };

        Assert.False(sut.RequiresTransparentWindow);
    }
}
