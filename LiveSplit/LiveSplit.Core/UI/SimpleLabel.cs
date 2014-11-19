using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI
{
    public class SimpleLabel
    {
        public String Text { get; set; }
        public ICollection<String> AlternateText { get; set; }
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
        //public bool HasGlassEffects { get; set; }

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
                    if (Brush != null && Brush is SolidBrush)
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
            String text = "",
            float x = 0.0f, float y = 0.0f,
            Font font = null, Brush brush = null,
            float width = float.MaxValue, float height = float.MaxValue,
            StringAlignment horizontalAlignment = StringAlignment.Near,
            StringAlignment verticalAlignment = StringAlignment.Near,
            IEnumerable<String> alternateText = null
            )
        {
            Text = text;
            X = x;
            Font = font ?? new Font("Arial", 1.0f);
            Brush = brush ?? new SolidBrush(Color.Black);
            Width = width;
            Height = height;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            IsMonospaced = false;
            HasShadow = true;
            ShadowColor = Color.FromArgb(128, 0, 0, 0);
            ((List<String>)(AlternateText = new List<String>())).AddRange(alternateText ?? new String[0]);
            //HasGlassEffects = false;
        }

        public void Draw(Graphics g)
        {
            var format = new StringFormat() { Alignment = HorizontalAlignment, LineAlignment = VerticalAlignment, FormatFlags = StringFormatFlags.NoWrap, Trimming = StringTrimming.EllipsisCharacter };

            if (!IsMonospaced)
            {
                var actualText = Text;

                ActualWidth = g.MeasureString(Text, Font, 9999, format).Width;
                foreach (var curText in AlternateText.OrderByDescending(x => x.Length))
                {
                    if (Width < ActualWidth)
                    {
                        actualText = curText;
                        ActualWidth = g.MeasureString(actualText, Font, 9999, format).Width;
                    }
                    else break;
                }
                if (HasShadow)
                {
                    var shadowBrush = new SolidBrush(ShadowColor);
                    g.DrawString(actualText, Font, shadowBrush, new RectangleF(X + 1, Y + 1, Width, Height), format);
                    g.DrawString(actualText, Font, shadowBrush, new RectangleF(X + 2, Y + 2, Width, Height), format);
                }
                g.DrawString(actualText, Font, Brush, new RectangleF(X, Y, Width, Height), format);
            }
            else
            {
                var charIndex = 0;
                var monoFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = VerticalAlignment };
                var measurement = TextRenderer.MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
                var offset = Width;
                charIndex = 0;
                SetActualWidth(g);
                offset = Width - ActualWidth;
                if (HorizontalAlignment != StringAlignment.Far)
                    offset = 0f;

                var cutOffText = CutOff(g);

                while (charIndex < cutOffText.Length)
                {
                    var curOffset = 0f;
                    var curChar = cutOffText[charIndex];
                    if (curChar.Equals('0') || curChar.Equals('1') || curChar.Equals('2')
                       || curChar.Equals('3') || curChar.Equals('4') || curChar.Equals('5')
                        || curChar.Equals('6') || curChar.Equals('7') || curChar.Equals('8') || curChar.Equals('9'))
                        curOffset = measurement;
                    else
                        curOffset = TextRenderer.MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
                    if (HasShadow)
                    {
                        var shadowBrush = new SolidBrush(ShadowColor);
                        g.DrawString(curChar.ToString(), Font, shadowBrush, new RectangleF(X + 1 + offset - curOffset * 2f, Y + 1, curOffset * 5f, Height), monoFormat);
                        g.DrawString(curChar.ToString(), Font, shadowBrush, new RectangleF(X + 2 + offset - curOffset * 2f, Y + 2, curOffset * 5f, Height), monoFormat);
                    }
                    g.DrawString(cutOffText[charIndex].ToString(), Font, Brush, new RectangleF(X + offset - curOffset / 2f, Y, curOffset * 2f, Height), monoFormat);
                    charIndex++;
                    offset += curOffset;
                }
            }
            
            /*if (HasGlassEffects)
            {
                var glassBrush = new LinearGradientBrush(
                    new PointF(0.0f, 0.0f),
                    new PointF(0.0f, Height),
                    Color.Transparent,
                    Color.Transparent);

                var cb = new ColorBlend();
                cb.Positions = new float[] { 0f, 0.5f, 0.55f, 1f };
                var c1 = Color.FromArgb(150, Color.White);
                var c2 = Color.FromArgb(100, Color.White);
                cb.Colors = new Color[] { c1, c2, Color.FromArgb(255, Color.Black), Color.Transparent };

                glassBrush.InterpolationColors = cb;

                g.DrawString(cutOffText, Font, glassBrush, new RectangleF(X, Y, Width, Height), format);
            }*/
        }
        public void SetActualWidth(Graphics g)
        {
            var format = new StringFormat() { Alignment = HorizontalAlignment, LineAlignment = VerticalAlignment, FormatFlags = StringFormatFlags.NoWrap, Trimming = StringTrimming.EllipsisCharacter };

            if (!IsMonospaced)
                ActualWidth = g.MeasureString(Text, Font, 9999, format).Width;
            else
                ActualWidth = MeasureActualWidth(Text, g);
        }

        private float MeasureActualWidth(String text, Graphics g)
        {
            var charIndex = 0;
            var monoFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = VerticalAlignment };
            var measurement = TextRenderer.MeasureText(g, "0", Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
            var offset = 0;
            while (charIndex < text.Length)
            {
                var curChar = text[charIndex];
                if (curChar.Equals('0') || curChar.Equals('1') || curChar.Equals('2')
                    || curChar.Equals('3') || curChar.Equals('4') || curChar.Equals('5')
                    || curChar.Equals('6') || curChar.Equals('7') || curChar.Equals('8') || curChar.Equals('9'))
                    offset += measurement;
                else
                    offset += TextRenderer.MeasureText(g, curChar.ToString(), Font, new Size((int)(Width + 0.5f), (int)(Height + 0.5f)), TextFormatFlags.NoPadding).Width;
                charIndex++;
            }
            return offset;
        }

        private String CutOff(Graphics g)
        {
            if (ActualWidth < Width)
                return Text;
            var cutOffText = Text;
            while (ActualWidth >= Width && !String.IsNullOrEmpty(cutOffText))
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
