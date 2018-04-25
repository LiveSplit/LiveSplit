using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace LiveSplit.UI.LayoutFactories
{
    public class XMLLayoutFactory : ILayoutFactory
    {
        public Stream Stream { get; set; }
 
        public XMLLayoutFactory (Stream stream)
        {
            Stream = stream;
        }

        private static LayoutSettings ParseSettings(XmlElement element, Version version)
        {
            var settings = new LayoutSettings();
            settings.TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            settings.BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            settings.ThinSeparatorsColor = SettingsHelper.ParseColor(element["ThinSeparatorsColor"]);
            settings.SeparatorsColor = SettingsHelper.ParseColor(element["SeparatorsColor"]);
            settings.PersonalBestColor = SettingsHelper.ParseColor(element["PersonalBestColor"]);
            settings.AheadGainingTimeColor = SettingsHelper.ParseColor(element["AheadGainingTimeColor"]);
            settings.AheadLosingTimeColor = SettingsHelper.ParseColor(element["AheadLosingTimeColor"]);
            settings.BehindGainingTimeColor = SettingsHelper.ParseColor(element["BehindGainingTimeColor"]);
            settings.BehindLosingTimeColor = SettingsHelper.ParseColor(element["BehindLosingTimeColor"]);
            settings.BestSegmentColor = SettingsHelper.ParseColor(element["BestSegmentColor"]);
            settings.UseRainbowColor = SettingsHelper.ParseBool(element["UseRainbowColor"], false);
            settings.NotRunningColor = SettingsHelper.ParseColor(element["NotRunningColor"]);
            settings.PausedColor = SettingsHelper.ParseColor(element["PausedColor"], Color.FromArgb(122, 122, 122));
            settings.AntiAliasing = SettingsHelper.ParseBool(element["AntiAliasing"], true);
            settings.DropShadows = SettingsHelper.ParseBool(element["DropShadows"], true);
            settings.Opacity = SettingsHelper.ParseFloat(element["Opacity"], 1);
            settings.MousePassThroughWhileRunning = SettingsHelper.ParseBool(element["MousePassThroughWhileRunning"]);
            settings.TextOutlineColor = SettingsHelper.ParseColor(element["TextOutlineColor"], Color.FromArgb(0, 0, 0, 0));
            settings.ShadowsColor = SettingsHelper.ParseColor(element["ShadowsColor"], Color.FromArgb(128, 0, 0, 0));
            settings.ShowBestSegments = SettingsHelper.ParseBool(element["ShowBestSegments"]);
            settings.AlwaysOnTop = SettingsHelper.ParseBool(element["AlwaysOnTop"]);
            settings.TimerFont = SettingsHelper.GetFontFromElement(element["TimerFont"]);
            using (var timerFont = new Font(settings.TimerFont.FontFamily.Name, (settings.TimerFont.Size / 50f) * 18f, settings.TimerFont.Style, GraphicsUnit.Point))
            {
                settings.TimerFont = new Font(timerFont.FontFamily.Name, (timerFont.Size / 18f) * 50f, timerFont.Style, GraphicsUnit.Pixel);
            }
            settings.ImageOpacity = SettingsHelper.ParseFloat(element["ImageOpacity"], 1f);
            settings.ImageBlur = SettingsHelper.ParseFloat(element["ImageBlur"], 0f);

            if (version >= new Version(1, 3))
            {
                settings.BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
                settings.TimesFont = SettingsHelper.GetFontFromElement(element["TimesFont"]);
                settings.TextFont = SettingsHelper.GetFontFromElement(element["TextFont"]);
            }
            else
            {
                if (settings.BackgroundColor == Color.Black)
                    settings.BackgroundColor = settings.BackgroundColor2 = Color.Transparent;
                else
                    settings.BackgroundColor2 = settings.BackgroundColor;
                settings.TimesFont = SettingsHelper.GetFontFromElement(element["MainFont"]);
                settings.TextFont = SettingsHelper.GetFontFromElement(element["SplitNamesFont"]);
            }

            if (version >= new Version(1, 6, 1))
            {
                settings.BackgroundType = SettingsHelper.ParseEnum<BackgroundType>(element["BackgroundType"], BackgroundType.SolidColor);
            }
            else
            {
                var gradientType = element["BackgroundGradient"];
                if (gradientType == null || gradientType.InnerText == "Plain")
                    settings.BackgroundType = BackgroundType.SolidColor;
                else if (gradientType.InnerText == "Vertical")
                    settings.BackgroundType = BackgroundType.VerticalGradient;
                else
                    settings.BackgroundType = BackgroundType.HorizontalGradient;
            }

            settings.BackgroundImage = SettingsHelper.GetImageFromElement(element["BackgroundImage"]);

            return settings;
        }

        public ILayout Create(LiveSplitState state)
        {
            var document = new XmlDocument();
            document.Load(Stream);
            var layout = new Layout();
            var parent = document["Layout"];
            var version = SettingsHelper.ParseAttributeVersion(parent);

            layout.X = SettingsHelper.ParseInt(parent["X"]);
            layout.Y = SettingsHelper.ParseInt(parent["Y"]);
            layout.VerticalWidth = SettingsHelper.ParseInt(parent["VerticalWidth"]);
            layout.VerticalHeight = SettingsHelper.ParseInt(parent["VerticalHeight"]);
            layout.HorizontalWidth = SettingsHelper.ParseInt(parent["HorizontalWidth"]);
            layout.HorizontalHeight = SettingsHelper.ParseInt(parent["HorizontalHeight"]);
            layout.Mode = SettingsHelper.ParseEnum<LayoutMode>(parent["Mode"]);
            layout.Settings = ParseSettings(parent["Settings"], version);

            var components = parent["Components"];
            foreach (var componentNode in components.GetElementsByTagName("Component"))
            {
                var componentElement = componentNode as XmlElement;
                var path = componentElement["Path"];
                var settings = componentElement["Settings"];
                var layoutComponent = ComponentManager.LoadLayoutComponent(path.InnerText, state);
                if (layoutComponent != null)
                {
                    try
                    {
                        layoutComponent.Component.SetSettings(settings);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    layout.LayoutComponents.Add(layoutComponent);
                }
                else
                {
                    throw new Exception(path.InnerText + " could not be found");
                }
            }
            return layout;
        }
    }
}
