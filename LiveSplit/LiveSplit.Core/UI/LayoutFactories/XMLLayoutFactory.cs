using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        private Image GetImageFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);

                using (var ms = new MemoryStream(data))
                {
                    return (Image)bf.Deserialize(ms);
                }
            }
            return null;
        }

        private Font GetFontFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);

                using (var ms = new MemoryStream(data))
                {
                    return (Font)bf.Deserialize(ms);
                }
            }
            return null;
        }

        private LayoutSettings ParseSettings (XmlElement element, Version version)
        {
            var settings = new LayoutSettings();
            settings.TextColor = ParseColor(element["TextColor"]);
            settings.BackgroundColor = ParseColor(element["BackgroundColor"]);
            settings.ThinSeparatorsColor = ParseColor(element["ThinSeparatorsColor"]);
            settings.SeparatorsColor = ParseColor(element["SeparatorsColor"]);
            settings.PersonalBestColor = ParseColor(element["PersonalBestColor"]);
            settings.AheadGainingTimeColor = ParseColor(element["AheadGainingTimeColor"]);
            settings.AheadLosingTimeColor = ParseColor(element["AheadLosingTimeColor"]);
            settings.BehindGainingTimeColor = ParseColor(element["BehindGainingTimeColor"]);
            settings.BehindLosingTimeColor = ParseColor(element["BehindLosingTimeColor"]);
            settings.BestSegmentColor = ParseColor(element["BestSegmentColor"]);
            settings.NotRunningColor = ParseColor(element["NotRunningColor"]);
            if (version > new Version(1, 0, 0, 0))
            {
                settings.PausedColor = ParseColor(element["PausedColor"]);
                settings.AntiAliasing = Boolean.Parse(element["AntiAliasing"].InnerText);
                settings.DropShadows = Boolean.Parse(element["DropShadows"].InnerText);
            }
            else
            {
                settings.PausedColor = Color.FromArgb(122, 122, 122);
                settings.AntiAliasing = true;
                settings.DropShadows = true;
            }
            if (version >= new Version(1, 2))
            {
                settings.Opacity = Single.Parse(element["Opacity"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
            else
            {
                settings.Opacity = 1;
            }
            if (version >= new Version(1, 3))
            {
                settings.BackgroundColor2 = ParseColor(element["BackgroundColor2"]);
                settings.BackgroundGradient = ParseEnum<GradientType>(element["BackgroundGradient"]);
                settings.ShadowsColor = ParseColor(element["ShadowsColor"]);
                settings.TimesFont = GetFontFromElement(element["TimesFont"]);
                settings.TextFont = GetFontFromElement(element["TextFont"]);
            }
            else
            {
                if (settings.BackgroundColor == Color.Black)
                    settings.BackgroundColor = settings.BackgroundColor2 = Color.Transparent;
                else
                    settings.BackgroundColor2 = settings.BackgroundColor;
                settings.BackgroundGradient = GradientType.Plain;
                settings.ShadowsColor = Color.FromArgb(128, 0, 0, 0);
                settings.TimesFont = GetFontFromElement(element["MainFont"]);
                settings.TextFont = GetFontFromElement(element["SplitNamesFont"]);
            }

            settings.TimerFont = GetFontFromElement(element["TimerFont"]);
            using (var timerFont = new Font(settings.TimerFont.FontFamily.Name, (settings.TimerFont.Size / 50f) * 18f, settings.TimerFont.Style, GraphicsUnit.Point))
            {
                settings.TimerFont = new Font(timerFont.FontFamily.Name, (timerFont.Size / 18f) * 50f, timerFont.Style, GraphicsUnit.Pixel);
            }

            settings.ShowBestSegments = Boolean.Parse(element["ShowBestSegments"].InnerText);
            settings.AlwaysOnTop = Boolean.Parse(element["AlwaysOnTop"].InnerText);
            return settings;
        }

        private Color ParseColor(XmlElement colorElement)
        {
            return Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber));
        }

        private T ParseEnum<T>(XmlElement element)
        {
            return (T)Enum.Parse(typeof(T), element.InnerText);
        }

        public ILayout Create(LiveSplitState state)
        {
            var document = new XmlDocument();
            document.Load(Stream);
            var layout = new Layout();
            var parent = document["Layout"];
            var version = parent.HasAttribute("version")
                ? Version.Parse(parent.Attributes["version"].Value)
                : new Version(1, 0, 0, 0);
            var xCord = parent["X"];
            layout.X = Int32.Parse(xCord.InnerText);
            var yCord = parent["Y"];
            layout.Y = Int32.Parse(yCord.InnerText);
            var verticalWidth = parent["VerticalWidth"];
            layout.VerticalWidth = Int32.Parse(verticalWidth.InnerText);
            var verticalHeight = parent["VerticalHeight"];
            layout.VerticalHeight = Int32.Parse(verticalHeight.InnerText);
            var horizontalWidth = parent["HorizontalWidth"];
            layout.HorizontalWidth = Int32.Parse(horizontalWidth.InnerText);
            var horizontalHeight = parent["HorizontalHeight"];
            layout.HorizontalHeight = Int32.Parse(horizontalHeight.InnerText);
            var mode = parent["Mode"];
            layout.Mode = (mode.InnerText == "Horizontal") ? LayoutMode.Horizontal : LayoutMode.Vertical;
            var layoutSettings = parent["Settings"];
            layout.Settings = ParseSettings(layoutSettings, version);
            var components = parent["Components"];
            foreach (var componentNode in components.GetElementsByTagName("Component"))
            {
                var componentElement = componentNode as XmlElement;
                var path = componentElement["Path"];
                var settings = componentElement["Settings"];
                var layoutComponent = ComponentManager.LoadLayoutComponent(path.InnerText, state);
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
            return layout;
        }
    }
}
