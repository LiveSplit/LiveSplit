using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CustomFontDialog
{
    public partial class FontList : UserControl
    {

        public event EventHandler SelectedFontFamilyChanged;
        private int lastSelectedIndex = -1;

        public FontList()
        {
            InitializeComponent();

            foreach (FontFamily f in FontFamily.Families)
            {
                try
                {
                    if (!string.IsNullOrEmpty(f.Name))
                    {
                        FontStyle? firstAvailableStyle = null;
                        foreach (FontStyle style in Enum.GetValues(typeof(FontStyle)))
                        {
                            if (f.IsStyleAvailable(style))
                            {
                                firstAvailableStyle = style;
                                break;
                            }
                        }
                        if (firstAvailableStyle.HasValue)
                        {
                            lstFont.Items.Add(new Font(f, 12, firstAvailableStyle.Value));
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }
        }

        public FontFamily SelectedFontFamily
        {
            get
            {
                if (lstFont.SelectedItem != null)
                    return ((Font)lstFont.SelectedItem).FontFamily;
                else
                    return null;
            }
            set
            {
                if (value == null) lstFont.ClearSelected();
                else
                {
                    lstFont.SelectedIndex = IndexOf(value);
                }

            }
        }

        public int IndexOf(FontFamily ff)
        {
            for (int i = 1; i < lstFont.Items.Count; i++)
            {
                if (lstFont.Items[i] == "Section") continue;
                Font f = (Font)lstFont.Items[i];
                if (f.FontFamily.Name == ff.Name)
                {
                    return i;
                }
            }

            return -1;
        }

        private void lstFont_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();

            Font font = (Font)lstFont.Items[e.Index];
            e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void lstFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFont.SelectedItem != null)
            {
                if (!txtFont.Focused)
                {
                    Font f = (Font)lstFont.SelectedItem;
                    txtFont.Text = f.Name;
                }

                SelectedFontFamilyChanged(lstFont, new EventArgs());
                lastSelectedIndex = lstFont.SelectedIndex;
            }
        }

        private void txtFont_TextChanged(object sender, EventArgs e)
        {
            if (!txtFont.Focused) return;

            for (int i = 0; i < lstFont.Items.Count; i++)
            {
                string str = ((Font)lstFont.Items[i]).Name;
                if (str.StartsWith(txtFont.Text, true, null))
                {
                    lstFont.SelectedIndex = i;

                    const uint WM_VSCROLL = 0x0115;
                    const uint SB_THUMBPOSITION = 4;

                    uint b = ((uint)(lstFont.SelectedIndex) << 16) | (SB_THUMBPOSITION & 0xffff);
                    SendMessage(lstFont.Handle, WM_VSCROLL, b, 0);

                    return;
                }
            }
        }

        private void txtFont_MouseClick(object sender, MouseEventArgs e)
        {
            txtFont.SelectAll();
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstFont_KeyDown(object sender, KeyEventArgs e)
        {
            // if you type alphanumeric characters while focus is on ListBox, it shifts the focus to TextBox.
            if (Char.IsLetterOrDigit((char)e.KeyValue))
            {
                txtFont.Focus();
                txtFont.Text = ((char)e.KeyValue).ToString();
                txtFont.SelectionStart = 1;
            }
        }

        // ensures that focus is lstFont control whenever the form is loaded
        private void FontList_Load(object sender, EventArgs e)
        {
            this.ActiveControl = lstFont;
        }
    }
}
