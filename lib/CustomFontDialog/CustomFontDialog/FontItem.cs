using System.Drawing;

namespace CustomFontDialog;

internal struct FontItem
{
    public Font Font { get; set; }

    public FontItem(Font font)
    {
        Font = font;
    }

    public override readonly string ToString()
    {
        return Font.FontFamily.Name;
    }
}
