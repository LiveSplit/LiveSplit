using System;
using System.Drawing;
using System.Windows.Forms;

using LiveSplit.Options;

namespace LiveSplit.Utils;

public static class RichTextBoxExtensions
{
    public static void AppendBBCode(this RichTextBox textBox, string text)
    {
        try
        {
            string[] tokens = text.Split('[');

            FontStyle style = FontStyle.Regular;
            float size = 8.25f;
            Color backColor = Color.Transparent;
            Color textColor = Color.Black;
            HorizontalAlignment alignment = HorizontalAlignment.Left;
            string url = string.Empty;

            foreach (string token in tokens)
            {
                string actualText = token;
                int closingIndex = token.IndexOf("]");
                bool isEnclosed = closingIndex >= 0;
                if (isEnclosed)
                {
                    string code = token[..closingIndex];
                    actualText = token[(closingIndex + 1)..];

                    if (code == "b")
                    {
                        style |= FontStyle.Bold;
                    }
                    else if (code == "/b")
                    {
                        style &= ~FontStyle.Bold;
                    }
                    else if (code == "i")
                    {
                        style |= FontStyle.Italic;
                    }
                    else if (code == "/i")
                    {
                        style &= ~FontStyle.Italic;
                    }
                    else if (code == "u")
                    {
                        style |= FontStyle.Underline;
                    }
                    else if (code == "/u")
                    {
                        style &= ~FontStyle.Underline;
                    }
                    else if (code == "s")
                    {
                        style |= FontStyle.Strikeout;
                    }
                    else if (code == "/s")
                    {
                        style &= ~FontStyle.Strikeout;
                    }
                    else if (code == "small")
                    {
                        size = 7;
                    }
                    else if (code == "big")
                    {
                        size = 12;
                    }
                    else if (code is "/small" or "/big")
                    {
                        size = 8.25f;
                    }
                    else if (code is "center" or "centre")
                    {
                        alignment = HorizontalAlignment.Center;
                    }
                    else if (code is "/center" or "/centre")
                    {
                        textBox.AppendText(Environment.NewLine);
                        alignment = HorizontalAlignment.Left;
                    }
                    else if (code == "spoiler")
                    {
                        textBox.AppendText(Environment.NewLine);
                        textBox.SelectionFont = new Font(textBox.Font.FontFamily, 8.25f, FontStyle.Bold);
                        textBox.SelectionColor = textColor = Color.White;
                        textBox.SelectionBackColor = backColor = Color.Black;
                        textBox.AppendText("Spoiler: ");
                        textColor = Color.Black;
                    }
                    else if (code == "/spoiler")
                    {
                        backColor = Color.Transparent;
                        actualText = Environment.NewLine + actualText;
                    }
                    else if (code == "quote" || code.StartsWith("quote="))
                    {
                        string quoteText = "Quote";
                        if (code.StartsWith("quote="))
                        {
                            quoteText = string.Format("Originally posted by {0}", code[6..]);
                        }

                        textBox.AppendText(Environment.NewLine);
                        textBox.SelectionFont = new Font(textBox.Font.FontFamily, 8.25f, FontStyle.Underline | FontStyle.Italic);
                        textBox.SelectionBackColor = backColor = Color.LightGray;
                        textBox.AppendText(quoteText);
                        actualText = ": " + actualText;
                    }
                    else if (code == "/quote")
                    {
                        backColor = Color.Transparent;
                        actualText = Environment.NewLine + actualText;
                    }
                    else if (code is "list" or "/list")
                    {
                    }
                    else if (code == "*")
                    {
                        actualText = "    ●  " + actualText;
                    }
                    else if (code == "/section")
                    {
                    }
                    else if (code.StartsWith("section="))
                    {
                        string sectionText = code[8..];
                        textBox.AppendText(Environment.NewLine);
                        textBox.SelectionFont = new Font(textBox.Font.FontFamily, 10f, FontStyle.Bold);
                        textBox.SelectionAlignment = HorizontalAlignment.Center;
                        textBox.AppendText(sectionText + Environment.NewLine);
                    }
                    else if (code == "url")
                    {
                    }
                    else if (code == "/url")
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            actualText = ": " + url + actualText;
                            url = string.Empty;
                        }
                    }
                    else if (code.StartsWith("url="))
                    {
                        url = code[4..];
                    }
                    else
                    {
                        actualText = "[" + token;
                    }
                }

                textBox.SelectionFont = new Font(textBox.Font.FontFamily, size, style);
                textBox.SelectionBackColor = backColor;
                textBox.SelectionColor = textColor;
                textBox.SelectionAlignment = alignment;
                textBox.AppendText(actualText);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
