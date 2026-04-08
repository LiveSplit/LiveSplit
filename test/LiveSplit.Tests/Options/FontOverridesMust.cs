using System;
using System.Drawing;

using LiveSplit.Options;

using Xunit;

namespace LiveSplit.Tests.Options;

public class FontOverridesMust
{
    [Fact]
    public void ApplyOverride_WhenTimerFontOverrideSet()
    {
        var sut = new FontOverrides
        {
            OverrideTimerFont = true,
            TimerFont = new Font("Arial", 12f)
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Arial", settings.TimerFont.Name);
        Assert.Equal(12f, settings.TimerFont.Size);
        Assert.Equal("Segoe UI", settings.TimesFont.Name);
        Assert.Equal("Segoe UI", settings.TextFont.Name);
        Assert.Equal("Segoe UI", origTimer.Name);
    }

    [Fact]
    public void ApplyOverride_WhenTimesFontOverrideSet()
    {
        var sut = new FontOverrides
        {
            OverrideTimesFont = true,
            TimesFont = new Font("Consolas", 10f)
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Segoe UI", settings.TimerFont.Name);
        Assert.Equal("Consolas", settings.TimesFont.Name);
        Assert.Equal(10f, settings.TimesFont.Size);
        Assert.Equal("Segoe UI", settings.TextFont.Name);
        Assert.Equal("Segoe UI", origTimes.Name);
    }

    [Fact]
    public void ApplyOverride_WhenTextFontOverrideSet()
    {
        var sut = new FontOverrides
        {
            OverrideTextFont = true,
            TextFont = new Font("Courier New", 14f)
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Segoe UI", settings.TimerFont.Name);
        Assert.Equal("Segoe UI", settings.TimesFont.Name);
        Assert.Equal("Courier New", settings.TextFont.Name);
        Assert.Equal(14f, settings.TextFont.Size);
        Assert.Equal("Segoe UI", origText.Name);
    }

    [Fact]
    public void ApplyOverride_WhenAllOverridesSet()
    {
        var sut = new FontOverrides
        {
            OverrideTimerFont = true,
            TimerFont = new Font("Arial", 12f),
            OverrideTimesFont = true,
            TimesFont = new Font("Consolas", 10f),
            OverrideTextFont = true,
            TextFont = new Font("Courier New", 14f)
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Arial", settings.TimerFont.Name);
        Assert.Equal("Consolas", settings.TimesFont.Name);
        Assert.Equal("Courier New", settings.TextFont.Name);
        Assert.Equal("Segoe UI", origTimer.Name);
        Assert.Equal("Segoe UI", origTimes.Name);
        Assert.Equal("Segoe UI", origText.Name);
    }

    [Fact]
    public void PreserveOriginalFonts_WhenNoOverridesSet()
    {
        var sut = new FontOverrides();
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Segoe UI", settings.TimerFont.Name);
        Assert.Equal(16f, settings.TimerFont.Size);
        Assert.Equal("Segoe UI", settings.TimesFont.Name);
        Assert.Equal("Segoe UI", settings.TextFont.Name);
    }

    [Fact]
    public void RestoreOriginalFonts_AfterApply()
    {
        var sut = new FontOverrides
        {
            OverrideTimerFont = true,
            TimerFont = new Font("Arial", 12f),
            OverrideTimesFont = true,
            TimesFont = new Font("Consolas", 10f),
            OverrideTextFont = true,
            TextFont = new Font("Courier New", 14f)
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Arial", settings.TimerFont.Name);

        FontOverrides.Restore(settings, origTimer, origTimes, origText);

        Assert.Equal("Segoe UI", settings.TimerFont.Name);
        Assert.Equal(16f, settings.TimerFont.Size);
        Assert.Equal("Segoe UI", settings.TimesFont.Name);
        Assert.Equal("Segoe UI", settings.TextFont.Name);
    }

    [Fact]
    public void HandleNullFont_WhenOverrideFlagTrueButFontNull()
    {
        var sut = new FontOverrides
        {
            OverrideTimerFont = true,
            TimerFont = null,
            OverrideTimesFont = true,
            TimesFont = null,
            OverrideTextFont = true,
            TextFont = null
        };
        var settings = new LayoutSettings
        {
            TimerFont = new Font("Segoe UI", 16f),
            TimesFont = new Font("Segoe UI", 16f),
            TextFont = new Font("Segoe UI", 16f)
        };

        sut.ApplyTo(settings, out Font origTimer, out Font origTimes, out Font origText);

        Assert.Equal("Segoe UI", settings.TimerFont.Name);
        Assert.Equal("Segoe UI", settings.TimesFont.Name);
        Assert.Equal("Segoe UI", settings.TextFont.Name);
    }

    [Fact]
    public void CloneProducesIndependentCopy()
    {
        var sut = new FontOverrides
        {
            OverrideTimerFont = true,
            TimerFont = new Font("Arial", 12f),
            OverrideTimesFont = false,
            OverrideTextFont = true,
            TextFont = new Font("Courier New", 14f)
        };

        var clone = (FontOverrides)sut.Clone();

        Assert.True(clone.OverrideTimerFont);
        Assert.Equal("Arial", clone.TimerFont.Name);
        Assert.Equal(12f, clone.TimerFont.Size);
        Assert.False(clone.OverrideTimesFont);
        Assert.Null(clone.TimesFont);
        Assert.True(clone.OverrideTextFont);
        Assert.Equal("Courier New", clone.TextFont.Name);

        clone.OverrideTimerFont = false;
        clone.TimerFont.Dispose();
        clone.TimerFont = new Font("Verdana", 20f);

        Assert.True(sut.OverrideTimerFont);
        Assert.Equal("Arial", sut.TimerFont.Name);
    }

    [Fact]
    public void HasOverridesReturnsFalse_WhenNoFlagsSet()
    {
        var sut = new FontOverrides();

        Assert.False(sut.HasOverrides);
    }

    [Fact]
    public void HasOverridesReturnsTrue_WhenAnyFlagSet()
    {
        var timerOnly = new FontOverrides { OverrideTimerFont = true };
        var timesOnly = new FontOverrides { OverrideTimesFont = true };
        var textOnly = new FontOverrides { OverrideTextFont = true };

        Assert.True(timerOnly.HasOverrides);
        Assert.True(timesOnly.HasOverrides);
        Assert.True(textOnly.HasOverrides);
    }
}
