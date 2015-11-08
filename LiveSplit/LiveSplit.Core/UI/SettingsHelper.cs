using Fetze.WinFormsColor;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI
{
    public class SettingsHelper
    {
        public static CustomFontDialog.FontDialog GetFontDialog(Font previousFont, int minSize, int maxSize)
        {
            var dialog = new CustomFontDialog.FontDialog();
            dialog.OriginalFont = previousFont;
            dialog.MinSize = minSize;
            dialog.MaxSize = maxSize;
            return dialog;
        }

        public static string FormatFont(Font font)
        {
            return $"{ font.FontFamily.Name } { font.Style }";
        }

        public static void ColorButtonClick(Button button, Control control)
        {
            var picker = new ColorPickerDialog();
            picker.SelectedColorChanged += (s, x) => button.BackColor = picker.SelectedColor;
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.ShowDialog(control);
            button.BackColor = picker.SelectedColor;
        }

        public static Color ParseColor(XmlElement colorElement, Color defaultColor = default(Color))
        {
            return colorElement != null 
                ? Color.FromArgb(int.Parse(colorElement.InnerText, NumberStyles.HexNumber)) 
                : defaultColor;
        }

        public static Font GetFontFromElement(XmlElement element)
        {
            if (element != null && !element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(data);
                return (Font)bf.Deserialize(ms);
            }
            return null;
        }

        public static int CreateSetting(XmlDocument document, XmlElement parent, string elementName, Font font)
        {
            if (document != null)
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
                parent.AppendChild(element);
            }
            return getFontHashCode(font);
        }

        private static int getFontHashCode(Font font)
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + font.Name.GetHashCode();
                hash = hash * 23 + font.FontFamily.GetHashCode();
                hash = hash * 23 + font.Size.GetHashCode();
                hash = hash * 23 + font.Style.GetHashCode();
            }
            return hash;
        }

        public static int CreateSetting(XmlDocument document, XmlElement parent, string elementName, Image image)
        {
            if (document != null)
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

                parent.AppendChild(element);
            }

            return image != null ? image.GetHashCode() : 0;
        }

        public static Image GetImageFromElement(XmlElement element)
        {
            if (element != null && !element.IsEmpty)
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

        public static bool ParseBool(XmlElement boolElement, bool defaultBool = false)
        {
            return boolElement != null
                ? bool.Parse(boolElement.InnerText)
                : defaultBool;
        }

        public static int ParseInt(XmlElement intElement, int defaultInt = 0)
        {
            return intElement != null
                ? int.Parse(intElement.InnerText)
                : defaultInt;
        }

        public static float ParseFloat(XmlElement floatElement, float defaultFloat = 0f)
        {
            return floatElement != null
                ? float.Parse(floatElement.InnerText.Replace(',', '.'), CultureInfo.InvariantCulture)
                : defaultFloat;
        }

        public static double ParseDouble(XmlElement doubleElement, double defaultDouble = 0.0)
        {
            return doubleElement != null
                ? double.Parse(doubleElement.InnerText, CultureInfo.InvariantCulture)
                : defaultDouble;
        }

        public static string ParseString(XmlElement stringElement, string defaultString = null)
        {
            if (defaultString == null)
                defaultString = string.Empty;

            return stringElement != null
                ? stringElement.InnerText
                : defaultString;
        }

        public static TimeSpan ParseTimeSpan(XmlElement timeSpanElement, TimeSpan defaultTimeSpan = default(TimeSpan))
        {
            return timeSpanElement != null
                ? TimeSpan.Parse(timeSpanElement.InnerText)
                : defaultTimeSpan;
        }

        public static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value?.ToString();
            return element;
        }

        public static int CreateSetting(XmlDocument document, XmlElement parent, string name, Color color)
        {
            if (document != null)
            {
                var element = document.CreateElement(name);
                element.InnerText = color.ToArgb().ToString("X8");
                parent.AppendChild(element);
            }
            return color.GetHashCode();
        }

        public static int CreateSetting<T>(XmlDocument document, XmlElement parent, string name, T value)
        {
            if (document != null)
            {
                var element = document.CreateElement(name);
                element.InnerText = value?.ToString();
                parent.AppendChild(element);
            }
            return value != null ? value.GetHashCode() : 0;
        }

        public static int CreateSetting(XmlDocument document, XmlElement parent, string name, float value)
        {
            if (document != null)
            {
                var element = document.CreateElement(name);
                element.InnerText = value.ToString(CultureInfo.InvariantCulture);
                parent.AppendChild(element);
            }
            return value.GetHashCode();
        }

        public static int CreateSetting(XmlDocument document, XmlElement parent, string name, double value)
        {
            if (document != null)
            {
                var element = document.CreateElement(name);
                element.InnerText = value.ToString(CultureInfo.InvariantCulture);
                parent.AppendChild(element);
            }
            return value.GetHashCode();
        }

        public static XmlAttribute ToAttribute<T>(XmlDocument document, string name, T value)
        {
            var element = document.CreateAttribute(name);
            element.Value = value?.ToString();
            return element;
        }

        public static T ParseEnum<T>(XmlElement element, T defaultEnum = default(T))
        {
            return element != null 
                ? (T)Enum.Parse(typeof(T), element.InnerText) 
                : defaultEnum;
        }

        public static Version ParseVersion(XmlElement element)
        {
            return element != null 
                ? Version.Parse(element.InnerText) 
                : new Version(1, 0, 0, 0);
        }

        public static Version ParseAttributeVersion(XmlElement element)
        {
            return element.HasAttribute("version")
                ? Version.Parse(element.GetAttribute("version"))
                : new Version(1, 0, 0, 0);
        }
    }
}
