using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;

namespace CustomFontDialog
{
    public partial class FontList : UserControl
    {
		private static readonly Array AllFontStyles = Enum.GetValues(typeof(FontStyle));

		public event EventHandler SelectedFontFamilyChanged;
        private int lastSelectedIndex = -1;
		

        public FontList()
        {
            InitializeComponent();

            foreach (FontFamily f in new InstalledFontCollection().Families)
            {
                try
                {
                    if (!string.IsNullOrEmpty(f.Name))
                    {
                        FontStyle? firstAvailableStyle = null;
                        foreach (FontStyle style in AllFontStyles)
                        {							
                            if (f.IsStyleAvailable(style))
                            {
                                firstAvailableStyle = style;
                                break;
                            }
                        }
                        if (firstAvailableStyle.HasValue)
                        {
                            lstFont.Items.Add(new FontItem(new Font(f, 12, firstAvailableStyle.Value)));
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
                    return ((FontItem)lstFont.SelectedItem).Font.FontFamily;
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
                var f = (FontItem)lstFont.Items[i];
                if (f.ToString() == ff.Name)
                {
                    return i;
                }
            }

            return -1;
        }

        private void lstFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFont.SelectedItem != null)
            {
                if (!txtFont.Focused)
                {
                    var f = ((FontItem)lstFont.SelectedItem).Font;
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
                string str = lstFont.Items[i].ToString();
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
