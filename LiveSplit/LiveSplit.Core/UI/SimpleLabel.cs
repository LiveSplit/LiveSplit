using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.TextRenderer;

namespace LiveSplit.UI
{
    public class SimpleLabel
    {
        public string Text { get; set; }
        public ICollection<string> AlternateText { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public StringAlignment HorizontalAlignment { get; set; }
        public StringAlignment VerticalAlignment { get; set; }
        public Color ShadowColor { get; set; }

        public bool HasShadow { get; set; }
        public bool IsMonospaced { get; set; }

        private StringFormat Format { get; set; }

        public float ActualWidth { get; set; }

        public Color ForeColor
        {
            get
            {
                return ((SolidBrush)Brush).Color;
            }
            set
            {
                try
                {
                    if (Brush is SolidBrush)
                    {
                        ((SolidBrush)Brush).Color = value;
                    }
                    else
                    {
                        Brush = new SolidBrush(value);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public SimpleLabel(
            string text = "",
            float x = 0.0f, float y = 0.0f,
            Font font = null, Brush brush = null,
            float width = float.MaxValue, float height = float.MaxValue,
            StringAlignment horizontalAlignment = StringAlignment.Near,
            StringAlignment verticalAlignment = StringAlignment.Near,
            IEnumerable<string> alternateText = null)
        {
            Text = text;
            X = x;
            Y = y;
            Font = font ?? new Font("Arial", 1.0f);
            Brush = brush ?? new SolidBrush(Color.Black);
            Width = width;
            Height = height;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            IsMonospaced = false;
            HasShadow = true;
            ShadowColor = Color.FromArgb(128, 0, 0, 0);
            ((List<string>)(AlternateText = new List<string>())).AddRange(alternateText ?? new string[0]);
            Format = new StringFormat
            {
                Alignment = HorizontalAlignment,
                LineAlignment = VerticalAlignment,
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisCharacter
            };
        }

        public void Draw(Graphics g)
        {
            Format.Alignment = HorizontalAlignment;
            Format.LineAlignment = VerticalAlignment;

            if (!IsMonospaced)
            {
                var actualText = CalculateAlternateText(g, Width);

                if (HasShadow)
                {
                    using (var shadowBrush = new SolidBrush(ShadowColor))
                    {
                        g.DrawString(actualText, Font, shadowBrush, new RectangleF(X + 1, Y + 1, Width, Height), Format);
                        g.DrawString(actualText, Font, shadowBrush, new RectangleF(X + 2, Y + 2, Width, Height), Format);
                    }
                }
                g.DrawString(actualText, Font, Brush, new RectangleF(X, Y, Width, Height), Format);
            }
            else
            {
                var monoFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = VerticalAlignment
                };

                var measurement = MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
                var offset = Width;
                var charIndex = 0;
                SetActualWidth(g);
                var cutOffText = CutOff(g);

                offset = Width - MeasureActualWidth(cutOffText, g);
                if (HorizontalAlignment != StringAlignment.Far)
                    offset = 0f;


                while (charIndex < cutOffText.Length)
                {
                    var curOffset = 0f;
                    var curChar = cutOffText[charIndex];

                    if (char.IsDigit(curChar))
                        curOffset = measurement;
                    else
                        curOffset = MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;

                    if (HasShadow)
                    {
                        using (var shadowBrush = new SolidBrush(ShadowColor))
                        {
                            g.DrawString(curChar.ToString(), Font, shadowBrush, new RectangleF(X + 1 + offset - curOffset * 2f, Y + 1, curOffset * 5f, Height), monoFormat);
                            g.DrawString(curChar.ToString(), Font, shadowBrush, new RectangleF(X + 2 + offset - curOffset * 2f, Y + 2, curOffset * 5f, Height), monoFormat);
                        }
                    }

                    g.DrawString(cutOffText[charIndex].ToString(), Font, Brush, new RectangleF(X + offset - curOffset / 2f, Y, curOffset * 2f, Height), monoFormat);
                    charIndex++;
                    offset += curOffset;
                }
            }
        }

        public void SetActualWidth(Graphics g)
        {
            Format.Alignment = HorizontalAlignment;
            Format.LineAlignment = VerticalAlignment;

            if (!IsMonospaced)
                ActualWidth = g.MeasureString(Text, Font, 9999, Format).Width;
            else
                ActualWidth = MeasureActualWidth(Text, g);
        }

        public string CalculateAlternateText(Graphics g, float width)
        {
            var actualText = Text;
            ActualWidth = g.MeasureString(Text, Font, 9999, Format).Width;
            foreach (var curText in AlternateText.OrderByDescending(x => x.Length))
            {
                if (width < ActualWidth)
                {
                    actualText = curText;
                    ActualWidth = g.MeasureString(actualText, Font, 9999, Format).Width;
                }
                else
                {
                    break;
                }
            }
            return actualText;
        }

        private float MeasureActualWidth(string text, Graphics g)
        {
            var charIndex = 0;
            var measurement = MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
            var offset = 0;

            while (charIndex < text.Length)
            {
                var curChar = text[charIndex];

                if (char.IsDigit(curChar))
                    offset += measurement;
                else
                    offset += MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;

                charIndex++;
            }
            return offset;
        }

        private string CutOff(Graphics g)
        {
            if (ActualWidth < Width)
                return Text;
            var cutOffText = Text;
            while (ActualWidth >= Width && !string.IsNullOrEmpty(cutOffText))
            {
                cutOffText = cutOffText.Remove(cutOffText.Length - 1, 1);
                ActualWidth = MeasureActualWidth(cutOffText + "...", g);
            }
            if (ActualWidth >= Width)
                return "";
            return cutOffText + "...";
        }
    }
}
