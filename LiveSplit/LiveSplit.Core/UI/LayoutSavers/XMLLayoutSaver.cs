using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace LiveSplit.UI.LayoutSavers
{
    public class XMLLayoutSaver : ILayoutSaver
    {
        private XmlElement CreateImageElement(XmlDocument document, String elementName, Image image)
        {
            var element = document.CreateElement(elementName);

            if (image != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();

                    bf.Serialize(ms, image);
                    var data = ms.ToArray();
                    var cdata = document.CreateCDataSection(Convert.ToBase64String(data));
                    element.InnerXml = cdata.OuterXml;
                }
            }

            return element;
        }

        private XmlElement CreateFontElement(XmlDocument document, String elementName, Font font)
        {
            var element = document.CreateElement(elementName);

            if (font != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();

                    bf.Serialize(ms, font);
                    var data = ms.ToArray();
                    var cdata = document.CreateCDataSection(Convert.ToBase64String(data));
                    element.InnerXml = cdata.OuterXml;
                }
            }

            return element;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        private XmlElement ToElement(XmlDocument document, String name, float value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString(CultureInfo.InvariantCulture);
            return element;
        }

        private XmlElement ToElement(XmlDocument document, LayoutSettings settings)
        {
            var element = document.CreateElement("Settings");

            element.AppendChild(ToElement(document, settings.TextColor, "TextColor"));
            element.AppendChild(ToElement(document, settings.BackgroundColor, "BackgroundColor"));
            element.AppendChild(ToElement(document, settings.BackgroundColor2, "BackgroundColor2"));
            element.AppendChild(ToElement(document, settings.ThinSeparatorsColor, "ThinSeparatorsColor"));
            element.AppendChild(ToElement(document, settings.SeparatorsColor, "SeparatorsColor"));
            element.AppendChild(ToElement(document, settings.PersonalBestColor, "PersonalBestColor"));
            element.AppendChild(ToElement(document, settings.AheadGainingTimeColor, "AheadGainingTimeColor"));
            element.AppendChild(ToElement(document, settings.AheadLosingTimeColor, "AheadLosingTimeColor"));
            element.AppendChild(ToElement(document, settings.BehindGainingTimeColor, "BehindGainingTimeColor"));
            element.AppendChild(ToElement(document, settings.BehindLosingTimeColor, "BehindLosingTimeColor"));
            element.AppendChild(ToElement(document, settings.BestSegmentColor, "BestSegmentColor"));
            element.AppendChild(ToElement(document, settings.NotRunningColor, "NotRunningColor"));
            element.AppendChild(ToElement(document, settings.PausedColor, "PausedColor"));
            element.AppendChild(ToElement(document, settings.ShadowsColor, "ShadowsColor"));

            element.AppendChild(CreateFontElement(document, "TimesFont", settings.TimesFont));
            element.AppendChild(CreateFontElement(document, "TimerFont", settings.TimerFont));
            element.AppendChild(CreateFontElement(document, "TextFont", settings.TextFont));

            element.AppendChild(ToElement(document, "AlwaysOnTop", settings.AlwaysOnTop));
            element.AppendChild(ToElement(document, "ShowBestSegments", settings.ShowBestSegments));
            element.AppendChild(ToElement(document, "AntiAliasing", settings.AntiAliasing));
            element.AppendChild(ToElement(document, "DropShadows", settings.DropShadows));
            element.AppendChild(ToElement(document, "BackgroundGradient", settings.BackgroundGradient));

            element.AppendChild(ToElement(document, "Opacity", settings.Opacity));

            return element;
        }

        private XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        public XmlNode GetLayoutNode(XmlDocument document, ILayout layout)
        {
            var parent = document.CreateElement("Layout");
            var version = document.CreateAttribute("version");
            version.Value = "1.3";
            parent.Attributes.Append(version);

            var mode = document.CreateElement("Mode");
            mode.InnerText = layout.Mode == LayoutMode.Horizontal ? "Horizontal" : "Vertical";
            parent.AppendChild(mode);

            parent.AppendChild(ToElement(document, "X", layout.X));
            parent.AppendChild(ToElement(document, "Y", layout.Y));
            parent.AppendChild(ToElement(document, "VerticalWidth", layout.VerticalWidth));
            parent.AppendChild(ToElement(document, "VerticalHeight", layout.VerticalHeight));
            parent.AppendChild(ToElement(document, "HorizontalWidth", layout.HorizontalWidth));
            parent.AppendChild(ToElement(document, "HorizontalHeight", layout.HorizontalHeight));

            parent.AppendChild(ToElement(document, layout.Settings));

            var components = document.CreateElement("Components");
            parent.AppendChild(components);

            var layoutComponents = new List<ILayoutComponent>(layout.LayoutComponents);

            foreach (var component in layoutComponents)
            {
                var componentElement = document.CreateElement("Component");
                components.AppendChild(componentElement);
                var path = document.CreateElement("Path");
                path.InnerText = component.Path;
                componentElement.AppendChild(path);
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

        public void Save(ILayout layout, System.IO.Stream stream)
        {
            var document = new XmlDocument();

            XmlNode docNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(docNode);

            document.AppendChild(GetLayoutNode(document, layout));

            document.Save(stream);
        }
    }
}

