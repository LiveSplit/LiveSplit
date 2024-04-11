using System;
using System.Drawing;

namespace CustomFontDialog
{
    public class FontChangedEventArgs : EventArgs
    {
        public Font NewFont { get; set; }
    }
}
