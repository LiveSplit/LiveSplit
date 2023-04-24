using System.Drawing;
using Xunit;
using LiveSplit.Options;

namespace LiveSplit.Tests.Options
{
    public class LayoutSettingsMust
    {
        [Fact]
        public void RememberValuesCorrectly()
        {
            var sut = CreateSubjectUnderTest();
            ValidateSubject(sut);
        }

        private static LayoutSettings CreateSubjectUnderTest() =>
            new LayoutSettings
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
            var original = CreateSubjectUnderTest();
            var sut = (LayoutSettings)original.Clone();
            ValidateSubject(sut);
        }

        [Fact]
        public void AssignLayoutSettingsCorrectly()
        {
            var original = CreateSubjectUnderTest();
            var sut = new LayoutSettings();
            sut.Assign(original);
            ValidateSubject(sut);
        }
    }
}
