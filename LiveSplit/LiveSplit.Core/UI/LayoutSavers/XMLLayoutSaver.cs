using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LiveSplit.UI.LayoutSavers
{
    public class XMLLayoutSaver : ILayoutSaver
    {
        private static XmlElement ToElement(XmlDocument document, LayoutSettings settings)
        {
            var element = document.CreateElement("Settings");
            element.AppendChild(SettingsHelper.ToElement(document, settings.TextColor, "TextColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.BackgroundColor, "BackgroundColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.BackgroundColor2, "BackgroundColor2"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.ThinSeparatorsColor, "ThinSeparatorsColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.SeparatorsColor, "SeparatorsColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.PersonalBestColor, "PersonalBestColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.AheadGainingTimeColor, "AheadGainingTimeColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.AheadLosingTimeColor, "AheadLosingTimeColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.BehindGainingTimeColor, "BehindGainingTimeColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.BehindLosingTimeColor, "BehindLosingTimeColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.BestSegmentColor, "BestSegmentColor"));
            element.AppendChild(SettingsHelper.ToElement(document, "UseRainbowColor", settings.UseRainbowColor));
            element.AppendChild(SettingsHelper.ToElement(document, settings.NotRunningColor, "NotRunningColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.PausedColor, "PausedColor"));
            element.AppendChild(SettingsHelper.ToElement(document, settings.ShadowsColor, "ShadowsColor"));
            element.AppendChild(SettingsHelper.CreateFontElement(document, "TimesFont", settings.TimesFont));
            element.AppendChild(SettingsHelper.CreateFontElement(document, "TimerFont", settings.TimerFont));
            element.AppendChild(SettingsHelper.CreateFontElement(document, "TextFont", settings.TextFont));
            element.AppendChild(SettingsHelper.ToElement(document, "AlwaysOnTop", settings.AlwaysOnTop));
            element.AppendChild(SettingsHelper.ToElement(document, "ShowBestSegments", settings.ShowBestSegments));
            element.AppendChild(SettingsHelper.ToElement(document, "AntiAliasing", settings.AntiAliasing));
            element.AppendChild(SettingsHelper.ToElement(document, "DropShadows", settings.DropShadows));
            element.AppendChild(SettingsHelper.ToElement(document, "BackgroundType", settings.BackgroundType));
            element.AppendChild(SettingsHelper.ToElement(document, "BackgroundImagePath", settings.BackgroundImagePath));
            element.AppendChild(SettingsHelper.ToElement(document, "Opacity", settings.Opacity));
            return element;
        }

        public XmlNode GetLayoutNode(XmlDocument document, ILayout layout)
        {
            var parent = document.CreateElement("Layout");
            parent.Attributes.Append(SettingsHelper.ToAttribute(document, "version", "1.6.1"));

            parent.AppendChild(SettingsHelper.ToElement(document, "Mode", layout.Mode));
            parent.AppendChild(SettingsHelper.ToElement(document, "X", layout.X));
            parent.AppendChild(SettingsHelper.ToElement(document, "Y", layout.Y));
            parent.AppendChild(SettingsHelper.ToElement(document, "VerticalWidth", layout.VerticalWidth));
            parent.AppendChild(SettingsHelper.ToElement(document, "VerticalHeight", layout.VerticalHeight));
            parent.AppendChild(SettingsHelper.ToElement(document, "HorizontalWidth", layout.HorizontalWidth));
            parent.AppendChild(SettingsHelper.ToElement(document, "HorizontalHeight", layout.HorizontalHeight));

            parent.AppendChild(ToElement(document, layout.Settings));

            var components = document.CreateElement("Components");
            parent.AppendChild(components);

            var layoutComponents = new List<ILayoutComponent>(layout.LayoutComponents);

            foreach (var component in layoutComponents)
            {
                var componentElement = document.CreateElement("Component");
                components.AppendChild(componentElement);
                componentElement.AppendChild(SettingsHelper.ToElement(document, "Path", component.Path));
                var settings = document.CreateElement("Settings");
                try
                {
                    settings.InnerXml = component.Component.GetSettings(document).InnerXml;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                componentElement.AppendChild(settings);
            }

            return parent;
        }

        public void Save(ILayout layout, Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);

            document.AppendChild(GetLayoutNode(document, layout));

            document.Save(stream);
        }
    }
}

