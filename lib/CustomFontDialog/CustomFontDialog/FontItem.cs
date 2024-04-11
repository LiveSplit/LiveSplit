using System.Drawing;

namespace CustomFontDialog
{
    struct FontItem
    {
        public Font Font { get; set; }

        public FontItem(Font font)
        {
            Font = font;
        }

        public override string ToString()
        {
            return Font.FontFamily.Name;
        }
    }
}
