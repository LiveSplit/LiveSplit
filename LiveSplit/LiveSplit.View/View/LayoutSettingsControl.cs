using System;
using System.Drawing;
using System.Windows.Forms;
using LiveSplit.UI;
using LiveSplit.Options;

namespace LiveSplit.View
{
    public partial class LayoutSettingsControl : UserControl
    {
        public LayoutSettingsControl()
        {
            InitializeComponent();
        }

        public Options.LayoutSettings Settings { get; set; }
        public ILayout Layout { get; set; }

        private Image originalBackgroundImage { get; set; }

        public string TimerFont { get { return SettingsHelper.FormatFont(Settings.TimerFont); } }
        public string MainFont { get { return SettingsHelper.FormatFont(Settings.TimesFont); } }
        public string SplitNamesFont { get { return SettingsHelper.FormatFont(Settings.TextFont); } }

        public float Opacity { get { return Settings.Opacity * 100f; } set { Settings.Opacity = value / 100f; } }
        public float ImageOpacity { get { return Settings.ImageOpacity * 100f; } set { Settings.ImageOpacity = value / 100f; } }
        public float ImageBlur { get { return Settings.ImageBlur * 100f; } set { Settings.ImageBlur = value / 100f; } }

        public LayoutSettingsControl(Options.LayoutSettings settings, ILayout layout)
        {
            InitializeComponent();
            Settings = settings;
            Layout = layout;
            chkBestSegments.DataBindings.Add("Checked", Settings, "ShowBestSegments", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAlwaysOnTop.DataBindings.Add("Checked", Settings, "AlwaysOnTop", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAntiAliasing.DataBindings.Add("Checked", Settings, "AntiAliasing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropShadows.DataBindings.Add("Checked", Settings, "DropShadows", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRainbow.DataBindings.Add("Checked", Settings, "UseRainbowColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", Settings, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground.DataBindings.Add("BackColor", Settings, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground2.DataBindings.Add("BackColor", Settings, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            btnThinSep.DataBindings.Add("BackColor", Settings, "ThinSeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSeparators.DataBindings.Add("BackColor", Settings, "SeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPB.DataBindings.Add("BackColor", Settings, "PersonalBestColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnGlod.DataBindings.Add("BackColor", Settings, "BestSegmentColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadGaining.DataBindings.Add("BackColor", Settings, "AheadGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadLosing.DataBindings.Add("BackColor", Settings, "AheadLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindGaining.DataBindings.Add("BackColor", Settings, "BehindGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindLosing.DataBindings.Add("BackColor", Settings, "BehindLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnNotRunning.DataBindings.Add("BackColor", Settings, "NotRunningColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPausedColor.DataBindings.Add("BackColor", Settings, "PausedColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextOutlineColor.DataBindings.Add("BackColor", Settings, "TextOutlineColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnShadowsColor.DataBindings.Add("BackColor", Settings, "ShadowsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimer.DataBindings.Add("Text", this, "TimerFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblText.DataBindings.Add("Text", this, "SplitNamesFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimes.DataBindings.Add("Text", this, "MainFont", false, DataSourceUpdateMode.OnPropertyChanged);
            trkOpacity.DataBindings.Add("Value", this, "Opacity", false, DataSourceUpdateMode.OnPropertyChanged);
            chkMousePassThroughWhileRunning.DataBindings.Add("Checked", Settings, "MousePassThroughWhileRunning", false, DataSourceUpdateMode.OnPropertyChanged);
            trkImageOpacity.DataBindings.Add("Value", this, "ImageOpacity", false, DataSourceUpdateMode.OnPropertyChanged);
            trkBlur.DataBindings.Add("Value", this, "ImageBlur", false, DataSourceUpdateMode.OnPropertyChanged);

            cmbBackgroundType.SelectedItem = GetBackgroundTypeString(Settings.BackgroundType);
            originalBackgroundImage = Settings.BackgroundImage;
        }        

        private string GetBackgroundTypeString(BackgroundType type)
        {
            switch (type)
            {
                case BackgroundType.HorizontalGradient:
                    return "Horizontal Gradient";
                case BackgroundType.VerticalGradient:
                    return "Vertical Gradient";
                case BackgroundType.Image:
                    return "Image";
                default:
                    return "Solid Color";
            }
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = cmbBackgroundType.SelectedItem.ToString();
            btnBackground.Visible = selectedItem != "Solid Color" && selectedItem != "Image";
            btnBackground2.DataBindings.Clear();
            lblImageOpacity.Enabled = lblBlur.Enabled = trkImageOpacity.Enabled = trkBlur.Enabled = selectedItem == "Image";
            if (selectedItem == "Image")
            {
                btnBackground2.BackgroundImage = Settings.BackgroundImage;
                btnBackground2.BackColor = Color.Transparent;
                lblBackground.Text = "Image:";
            }
            else
            {
                btnBackground2.BackgroundImage = null;
                btnBackground2.DataBindings.Add("BackColor", Settings, btnBackground.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
                lblBackground.Text = "Color:";
            }
            Settings.BackgroundType = (BackgroundType)Enum.Parse(typeof(BackgroundType), selectedItem.Replace(" ", ""));
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void BackgroundColorButtonClick(object sender, EventArgs e)
        {
            if (cmbBackgroundType.SelectedItem.ToString() == "Image")
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*";
                dialog.Title = "Set Background Image...";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try
                    {
                        var image = Image.FromFile(dialog.FileName);
                        if (Settings.BackgroundImage != null && Settings.BackgroundImage != originalBackgroundImage)
                            Settings.BackgroundImage.Dispose();

                        Settings.BackgroundImage = ((Button)sender).BackgroundImage = image;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        MessageBox.Show("Could not load image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                SettingsHelper.ColorButtonClick((Button)sender, this);
            }
        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            var timerFont = new Font(Settings.TimerFont.FontFamily.Name, (Settings.TimerFont.Size / 50f) * 18f, Settings.TimerFont.Style, GraphicsUnit.Point);
            var dialog = SettingsHelper.GetFontDialog(timerFont, 7, 20);
            dialog.FontChanged += (s, ev) => updateTimerFont(((CustomFontDialog.FontChangedEventArgs)ev).NewFont);
            dialog.ShowDialog(this);
            lblTimer.Text = TimerFont;
        }

        private void updateTimerFont(Font timerFont)
        {
            Settings.TimerFont = new Font(timerFont.FontFamily.Name, (timerFont.Size / 18f) * 50f, timerFont.Style, GraphicsUnit.Pixel);
        }

        private void btnTimes_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(Settings.TimesFont, 7, 20);
            dialog.FontChanged += (s, ev) => Settings.TimesFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblTimes.Text = MainFont;
        }

        private void btnTextFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(Settings.TextFont, 7, 20);
            dialog.FontChanged += (s, ev) => Settings.TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblText.Text = SplitNamesFont;
        }

        private void chkRainbow_CheckedChanged(object sender, EventArgs e)
        {
            label9.Enabled = btnGlod.Enabled = !chkRainbow.Checked;
        }

        private void chkAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            lblOutlines.Enabled = btnTextOutlineColor.Enabled = chkAntiAliasing.Checked;
        }

        private void LayoutSettingsControl_Load(object sender, EventArgs e)
        {
            chkRainbow_CheckedChanged(this, null);
        }
    }
}
