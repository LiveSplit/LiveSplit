using System;
using System.Drawing;

namespace LiveSplit.Options
{
    public class LayoutSettings : ICloneable
    {
        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public Color ThinSeparatorsColor { get; set; }
        public Color SeparatorsColor { get; set; }
        public Color PersonalBestColor { get; set; }
        public Color AheadGainingTimeColor { get; set; }
        public Color AheadLosingTimeColor { get; set; }
        public Color BehindGainingTimeColor { get; set; }
        public Color BehindLosingTimeColor { get; set; }
        public Color BestSegmentColor { get; set; }
        public Color NotRunningColor { get; set; }
        public Color PausedColor { get; set; }
        public Color TextOutlineColor { get; set; }
        public Color ShadowsColor { get; set; }

        public BackgroundType BackgroundType { get; set; }

        public Image BackgroundImage { get; set; }
        public float ImageOpacity { get; set; }
        public float ImageBlur { get; set; }

        public Font TimerFont { get; set; }
        public Font TimesFont { get; set; }
        public Font TextFont { get; set; }

        public bool ShowBestSegments { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool AntiAliasing { get; set; }
        public bool DropShadows { get; set; }
        public bool UseRainbowColor { get; set; }

        public float Opacity { get; set; }
        public bool MousePassThroughWhileRunning { get; set; }

        public object Clone()
        {
            var settings = new LayoutSettings();
            settings.Assign(this);
            return settings;
        }

        public void Assign(LayoutSettings settings)
        {
            TextColor = settings.TextColor;
            BackgroundColor = settings.BackgroundColor;
            BackgroundColor2 = settings.BackgroundColor2;
            ThinSeparatorsColor = settings.ThinSeparatorsColor;
            SeparatorsColor = settings.SeparatorsColor;
            PersonalBestColor = settings.PersonalBestColor;
            AheadGainingTimeColor = settings.AheadGainingTimeColor;
            AheadLosingTimeColor = settings.AheadLosingTimeColor;
            BehindGainingTimeColor = settings.BehindGainingTimeColor;
            BehindLosingTimeColor = settings.BehindLosingTimeColor;
            BestSegmentColor = settings.BestSegmentColor;
            UseRainbowColor = settings.UseRainbowColor;
            NotRunningColor = settings.NotRunningColor;
            PausedColor = settings.PausedColor;
            TextOutlineColor = settings.TextOutlineColor;
            ShadowsColor = settings.ShadowsColor;
            TimerFont = settings.TimerFont.Clone() as Font;
            TimesFont = settings.TimesFont.Clone() as Font;
            TextFont = settings.TextFont.Clone() as Font;
            ShowBestSegments = settings.ShowBestSegments;
            AlwaysOnTop = settings.AlwaysOnTop;
            AntiAliasing = settings.AntiAliasing;
            DropShadows = settings.DropShadows;
            Opacity = settings.Opacity;
            MousePassThroughWhileRunning = settings.MousePassThroughWhileRunning;
            BackgroundType = settings.BackgroundType;
            BackgroundImage = settings.BackgroundImage;
            ImageOpacity = settings.ImageOpacity;
            ImageBlur = settings.ImageBlur;
        }
    }
}
