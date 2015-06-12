using Fetze.WinFormsColor;
using LiveSplit.Options;
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
        public static Font ChooseFont(Control control, Font previousFont, int minSize, int maxSize)
        {
            var dialog = new FontDialog();
            dialog.Font = previousFont;
            dialog.MinSize = minSize;
            dialog.MaxSize = maxSize;
            try
            {
                var result = dialog.ShowDialog(control);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.Font;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("This font is not supported. If this font is freshly installed, a restart of LiveSplit is required.", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return previousFont;
        }

        public static Color ParseColor(XmlElement colorElement, Color defaultColor = default(Color))
        {
            return colorElement != null ? Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber)) : defaultColor;
        }

        public static Font GetFontFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(data);
                return (Font)bf.Deserialize(ms);
            }
            return null;
        }

        public static XmlElement CreateFontElement(XmlDocument document, string elementName, Font font)
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

        public static bool ParseBool(XmlElement boolElement, bool defaultBool = false)
        {
            return boolElement != null ? Boolean.Parse(boolElement.InnerText) : defaultBool;
        }

        public static int ParseInt(XmlElement intElement, int defaultInt = 0)
        {
            return intElement != null ? Int32.Parse(intElement.InnerText) : defaultInt;
        }

        public static float ParseFloat(XmlElement floatElement, float defaultFloat = 0f)
        {
            return floatElement != null ? Single.Parse(floatElement.InnerText.Replace(',', '.'), CultureInfo.InvariantCulture) : defaultFloat;
        }

        public static string ParseString(XmlElement stringElement, string defaultString = default(string))
        {
            return stringElement != null ? stringElement.InnerText : defaultString;
        }

        public static XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        public static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value is float ? Convert.ToSingle(value).ToString(CultureInfo.InvariantCulture) : value.ToString();
            return element;
        }

        public static T ParseEnum<T>(XmlElement element, T defaultEnum = default(T))
        {
            return element != null ? (T)Enum.Parse(typeof(T), element.InnerText) : defaultEnum;
        }

        public static Version ParseVersion(XmlElement element)
        {
            return element != null ? Version.Parse(element.InnerText) : new Version(1, 0, 0, 0);
        }
    }
}
