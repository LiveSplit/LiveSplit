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
            SettingsHelper.CreateSetting(document, element, "ShadowsColor", settings.ShadowsColor) ^
            SettingsHelper.CreateSetting(document, element, "TimesFont", settings.TimesFont) ^
            SettingsHelper.CreateSetting(document, element, "TimerFont", settings.TimerFont) ^
            SettingsHelper.CreateSetting(document, element, "TextFont", settings.TextFont) ^
            SettingsHelper.CreateSetting(document, element, "AlwaysOnTop", settings.AlwaysOnTop) ^
            SettingsHelper.CreateSetting(document, element, "ShowBestSegments", settings.ShowBestSegments) ^
            SettingsHelper.CreateSetting(document, element, "AntiAliasing", settings.AntiAliasing) ^
            SettingsHelper.CreateSetting(document, element, "DropShadows", settings.DropShadows) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundType", settings.BackgroundType) ^
            SettingsHelper.CreateSetting(document, element, "BackgroundImagePath", settings.BackgroundImagePath) ^
            SettingsHelper.CreateSetting(document, element, "ImageOpacity", settings.ImageOpacity) ^
            SettingsHelper.CreateSetting(document, element, "ImageBlur", settings.ImageBlur) ^
            SettingsHelper.CreateSetting(document, element, "Opacity", settings.Opacity);
        }

        public int CreateLayoutNode(XmlDocument document, XmlElement parent, ILayout layout)
        {
            XmlElement element = null, components = null;
            if (document != null)
            {
                element = document.CreateElement("Settings");

                components = document.CreateElement("Components");
                parent.AppendChild(components);
            }

            var hashCode = SettingsHelper.CreateSetting(document, parent, "Mode", layout.Mode)
                ^ SettingsHelper.CreateSetting(document, parent, "X", layout.X)
                ^ SettingsHelper.CreateSetting(document, parent, "Y", layout.Y)
                ^ SettingsHelper.CreateSetting(document, parent, "VerticalWidth", layout.VerticalWidth)
                ^ SettingsHelper.CreateSetting(document, parent, "VerticalHeight", layout.VerticalHeight)
                ^ SettingsHelper.CreateSetting(document, parent, "HorizontalWidth", layout.HorizontalWidth)
                ^ SettingsHelper.CreateSetting(document, parent, "HorizontalHeight", layout.HorizontalHeight)
                ^ ToElement(document, element, layout.Settings);

            var layoutComponents = new List<ILayoutComponent>(layout.LayoutComponents);

            foreach (var component in layoutComponents)
            {
                try
                {
                    if (document != null)
                    {
                        var componentElement = document.CreateElement("Component");
                        components.AppendChild(componentElement);
                        componentElement.AppendChild(SettingsHelper.ToElement(document, "Path", component.Path));
                        var settings = document.CreateElement("Settings");

                        settings.InnerXml = component.Component.GetSettings(document).InnerXml;

                        componentElement.AppendChild(settings);
                    }
                    else
                    {
                        //This is temporary. GetSettingsHashCode() will become a part of IComponent at some point
                        var type = component.GetType();
                        if (type.GetMethod("GetSettingsHashCode") != null)
                            hashCode ^= ((dynamic)component.Component).GetSettingsHashCode();
                        else
                            hashCode ^= component.Component.GetSettings(new XmlDocument()).InnerXml.GetHashCode();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
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

