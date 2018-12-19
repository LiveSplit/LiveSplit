using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomFontDialog
{
    public partial class FontDialog : Form
    {
        public FontDialog()
        {
            InitializeComponent();

            lstFont.SelectedFontFamilyChanged += lstFont_SelectedFontFamilyChanged;
            lstFont.SelectedFontFamily = FontFamily.GenericSansSerif;
            txtSize.Text = Convert.ToString(10);
        }

        private int minSize { get; set; }
        private int maxSize { get; set; }

        public int MinSize { get { return minSize; } set { minSize = value; UpdateSizeOptions(); } }
        public int MaxSize { get { return maxSize; } set { maxSize = value; UpdateSizeOptions(); } }

        private Font originalFont { get; set; }
        public Font OriginalFont { get { return originalFont; } set { originalFont = Font = value; } }

        public Font Font
        {
            get
            {
                return lblSampleText.Font;
            }
            set
            {
                lstFont.SelectedFontFamily = value.FontFamily;
                txtSize.Text = value.Size.ToString();
                chbBold.Checked = value.Bold;
                chbItalic.Checked = value.Italic;
            }
        }

        public event EventHandler FontChanged;

        private void lstFont_SelectedFontFamilyChanged(object sender, EventArgs e)
        {
            var family = lstFont.SelectedFontFamily;
            var bold = family.IsStyleAvailable(FontStyle.Bold);
            var regular = family.IsStyleAvailable(FontStyle.Regular);
            var italics = family.IsStyleAvailable(FontStyle.Italic);

            chbBold.Enabled = true;
            chbItalic.Enabled = true;

            if (!bold && !regular)
            {
                chbBold.Enabled = false;
                chbBold.Checked = false;
                if (italics)
                {
                    chbItalic.Enabled = false;
                    chbItalic.Checked = true;
                }
            }
            else if (!bold)
            {
                chbBold.Enabled = false;
                chbBold.Checked = false;
            }
            else if (!regular && !italics)
            {
                chbBold.Enabled = false;
                chbBold.Checked = true;
            }
            if (!italics)
            {
                chbItalic.Enabled = false;
                chbItalic.Checked = false;
            }

            UpdateSampleText();
        }

        private void lstSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstSize.SelectedItem != null)
                txtSize.Text = lstSize.SelectedItem.ToString();
        }

        private void txtSize_TextChanged(object sender, EventArgs e)
        {
            if (lstSize.Items.Contains(txtSize.Text))
                lstSize.SelectedItem = txtSize.Text;
            else
                lstSize.ClearSelected();
            
            UpdateSampleText();
        }

        
        private void txtSize_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {                   
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.End:
                case Keys.Enter:
                case Keys.Home:                
                case Keys.Back:
                case Keys.Delete:
                case Keys.Escape:
                case Keys.Left:
                case Keys.Right:
                    break;
                case Keys.Decimal:
                case (Keys)190: //decimal point
                    if (txtSize.Text.Contains("."))
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                    }
                    break;
                default:
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;
            }
            
        }

        private void UpdateSampleText()
        {
            float size = txtSize.Text != "" ? float.Parse(txtSize.Text) : 1;
            var family = lstFont.SelectedFontFamily;

            if (family != null)
            {
                FontStyle? style = null;
                if (chbBold.Checked && family.IsStyleAvailable(FontStyle.Bold))
                {
                    style = FontStyle.Bold;
                }
                if (chbItalic.Checked && family.IsStyleAvailable(FontStyle.Italic))
                {
                    if (style == null)
                        style = FontStyle.Italic;
                    else
                        style |= FontStyle.Italic;
                }
                if (style == null && family.IsStyleAvailable(FontStyle.Regular))
                {
                    style = FontStyle.Regular;
                }

                if (style.HasValue)
                {
                    if (size < 1)
                        size = 1;
                    else if (size > float.MaxValue)
                        size = float.MaxValue;
                    lblSampleText.Font = new Font(family, size, style.Value);

                    TriggerFontChanged();
                }
            }
        }

        /// <summary>
        /// Handles CheckedChanged event for Bold, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chb_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSampleText();
        }

        private void UpdateSizeOptions()
        {
            var sizes = new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"};

            this.lstSize.Items.Clear();
            foreach (var size in sizes)
            {
                int sizeNum = int.Parse((string)size);
                if (sizeNum >= MinSize && sizeNum <= MaxSize)
                    this.lstSize.Items.Add(size);
            }
        }

        private void FontDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                Font = OriginalFont;
            }
        }

        private void TriggerFontChanged()
        {
            if (FontChanged != null)
                FontChanged(this, new FontChangedEventArgs() { NewFont = Font });
        }
    }
}
