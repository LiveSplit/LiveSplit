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
        private static int ToElement(XmlDocument document, XmlElement element, LayoutSettings settings)
        {
            return SettingsHelper.CreateSetting(document, element, "TextColor", settings.TextColor) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundColor", settings.BackgroundColor) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundColor2", settings.BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, element, "ThinSeparatorsColor", settings.ThinSeparatorsColor) ^
            SettingsHelper.CreateSetting(document, element, "SeparatorsColor", settings.SeparatorsColor) ^
            SettingsHelper.CreateSetting(document, element, "PersonalBestColor", settings.PersonalBestColor) ^
            SettingsHelper.CreateSetting(document, element, "AheadGainingTimeColor", settings.AheadGainingTimeColor) ^
            SettingsHelper.CreateSetting(document, element, "AheadLosingTimeColor", settings.AheadLosingTimeColor) ^
            SettingsHelper.CreateSetting(document, element, "BehindGainingTimeColor", settings.BehindGainingTimeColor) ^
            SettingsHelper.CreateSetting(document, element, "BehindLosingTimeColor", settings.BehindLosingTimeColor) ^
            SettingsHelper.CreateSetting(document, element, "BestSegmentColor", settings.BestSegmentColor) ^
            SettingsHelper.CreateSetting(document, element, "UseRainbowColor", settings.UseRainbowColor) ^
            SettingsHelper.CreateSetting(document, element, "NotRunningColor", settings.NotRunningColor) ^
            SettingsHelper.CreateSetting(document, element, "PausedColor", settings.PausedColor) ^
            SettingsHelper.CreateSetting(document, element, "TextOutlineColor", settings.TextOutlineColor) ^
            SettingsHelper.CreateSetting(document, element, "ShadowsColor", settings.ShadowsColor) ^
            SettingsHelper.CreateSetting(document, element, "TimesFont", settings.TimesFont) ^
            SettingsHelper.CreateSetting(document, element, "TimerFont", settings.TimerFont) ^
            SettingsHelper.CreateSetting(document, element, "TextFont", settings.TextFont) ^
            SettingsHelper.CreateSetting(document, element, "AlwaysOnTop", settings.AlwaysOnTop) ^
            SettingsHelper.CreateSetting(document, element, "ShowBestSegments", settings.ShowBestSegments) ^
            SettingsHelper.CreateSetting(document, element, "AntiAliasing", settings.AntiAliasing) ^
            SettingsHelper.CreateSetting(document, element, "DropShadows", settings.DropShadows) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundType", settings.BackgroundType) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundImage", settings.BackgroundImage) ^
            SettingsHelper.CreateSetting(document, element, "ImageOpacity", settings.ImageOpacity) ^
            SettingsHelper.CreateSetting(document, element, "ImageBlur", settings.ImageBlur) ^
            SettingsHelper.CreateSetting(document, element, "Opacity", settings.Opacity) ^
            SettingsHelper.CreateSetting(document, element, "MousePassThroughWhileRunning", settings.MousePassThroughWhileRunning);
        }

        public int CreateLayoutNode(XmlDocument document, XmlElement parent, ILayout layout)
        {
            XmlElement element = null, components = null;
            if (document != null)
            {
                element = document.CreateElement("Settings");
                components = document.CreateElement("Components");
            }

            var hashCode = SettingsHelper.CreateSetting(document, parent, "Mode", layout.Mode)
                ^ SettingsHelper.CreateSetting(document, parent, "X", layout.X)
                ^ SettingsHelper.CreateSetting(document, parent, "Y", layout.Y)
                ^ SettingsHelper.CreateSetting(document, parent, "VerticalWidth", layout.VerticalWidth)
                ^ SettingsHelper.CreateSetting(document, parent, "VerticalHeight", layout.VerticalHeight) * 1000
                ^ SettingsHelper.CreateSetting(document, parent, "HorizontalWidth", layout.HorizontalWidth)
                ^ SettingsHelper.CreateSetting(document, parent, "HorizontalHeight", layout.HorizontalHeight) * 1000
                ^ ToElement(document, element, layout.Settings);

            if (document != null)
            {
                parent.AppendChild(element);
                parent.AppendChild(components);
            }

            var layoutComponents = new List<ILayoutComponent>(layout.LayoutComponents);
            var count = 1;

            foreach (var component in layoutComponents)
            {
                try
                {
                    if (document != null)
                    {
                        var componentElement = document.CreateElement("Component");
                        components.AppendChild(componentElement);
                        SettingsHelper.CreateSetting(document, componentElement, "Path", component.Path);
                        var settings = document.CreateElement("Settings");

                        settings.InnerXml = component.Component.GetSettings(document).InnerXml;

                        componentElement.AppendChild(settings);
                    }
                    else
                    {
                        var type = component.Component.GetType();
                        if (type.GetMethod("GetSettingsHashCode") != null)
                            hashCode ^= ((dynamic)component.Component).GetSettingsHashCode() ^ component.GetHashCode() * count;
                        else
                            hashCode ^= component.Component.GetSettings(new XmlDocument()).InnerXml.GetHashCode() ^ component.GetHashCode() * count;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                count++;
            }

            return hashCode;
        }

        public void Save(ILayout layout, Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);
            var parent = document.CreateElement("Layout");
            parent.Attributes.Append(SettingsHelper.ToAttribute(document, "version", "1.6.1"));
            CreateLayoutNode(document, parent, layout);
            document.AppendChild(parent);

            document.Save(stream);
        }
    }
}

