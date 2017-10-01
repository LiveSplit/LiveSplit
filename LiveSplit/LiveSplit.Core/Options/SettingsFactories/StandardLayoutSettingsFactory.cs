using System.Drawing;
using static System.Drawing.Color;

namespace LiveSplit.Options.SettingsFactories
{
    public class StandardLayoutSettingsFactory : ILayoutSettingsFactory
    {
        public LayoutSettings Create()
        {
            return new LayoutSettings()
            {
                TextColor = FromArgb(255, 255, 255),
                BackgroundColor = FromArgb(0, 0, 0, 0),
                BackgroundColor2 = FromArgb(0, 0, 0, 0),
                ThinSeparatorsColor = FromArgb(9, 255, 255, 255),
                SeparatorsColor = FromArgb(38, 255, 255, 255),
                PersonalBestColor = FromArgb(22, 166, 255),
                AheadGainingTimeColor = FromArgb(41, 204, 84),
                AheadLosingTimeColor = FromArgb(112, 204, 137),
                BehindGainingTimeColor = FromArgb(204, 120, 112),
                BehindLosingTimeColor = FromArgb(204, 55, 41),
                BestSegmentColor = FromArgb(216, 175, 31),
                NotRunningColor = FromArgb(122, 122, 122),
                PausedColor = FromArgb(122, 122, 122),
                TextOutlineColor = FromArgb(0, 0, 0, 0),
                ShadowsColor = FromArgb(128, 0, 0, 0),
                TimerFont = new Font("Century Gothic", 43.75f, FontStyle.Bold, GraphicsUnit.Pixel),
                TimesFont = new Font("Segoe UI", 13, FontStyle.Bold, GraphicsUnit.Pixel),
                TextFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel),
                ShowBestSegments = true,
                UseRainbowColor = false,
                AlwaysOnTop = true,
                AntiAliasing = true,
                DropShadows = true,
                BackgroundType = BackgroundType.SolidColor,
                BackgroundImage = null,
                ImageOpacity = 1f,
                ImageBlur = 0f,
                Opacity = 1,
                MousePassThroughWhileRunning = false
            };
        }
    }
}
