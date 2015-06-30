using System;
using System.Drawing;
using System.Windows.Forms;
using Fetze.WinFormsColor;
using LiveSplit.Options;
using LiveSplit.UI;

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

        public string TimerFont { get { return string.Format("{0} {1}", Settings.TimerFont.FontFamily.Name, Settings.TimerFont.Style); }}
        public string MainFont { get { return string.Format("{0} {1}", Settings.TimesFont.FontFamily.Name, Settings.TimesFont.Style); ; } }
        public string SplitNamesFont { get { return string.Format("{0} {1}", Settings.TextFont.FontFamily.Name, Settings.TextFont.Style); ; } }

        public GradientType BackgroundGradient { get { return Settings.BackgroundGradient; } set { Settings.BackgroundGradient = value; } }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public float Opacity { get { return Settings.Opacity*100f; } set { Settings.Opacity = value/100f; } }

        public LayoutSettingsControl(LiveSplit.UI.LayoutSettings settings, ILayout layout)
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
            btnShadowsColor.DataBindings.Add("BackColor", Settings, "ShadowsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimer.DataBindings.Add("Text", this, "TimerFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblText.DataBindings.Add("Text", this, "SplitNamesFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimes.DataBindings.Add("Text", this, "MainFont", false, DataSourceUpdateMode.OnPropertyChanged);
            trkOpacity.DataBindings.Add("Value", this, "Opacity", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnBackground.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnBackground2.DataBindings.Clear();
            btnBackground2.DataBindings.Add("BackColor", Settings, btnBackground.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            var timerFont = new Font(Settings.TimerFont.FontFamily.Name, (Settings.TimerFont.Size / 50f) * 18f, Settings.TimerFont.Style, GraphicsUnit.Point);
            timerFont = SettingsHelper.ChooseFont(this, timerFont, 7, 20);
            Settings.TimerFont = new Font(timerFont.FontFamily.Name, (timerFont.Size / 18f) * 50f, timerFont.Style, GraphicsUnit.Pixel);
            lblTimer.Text = TimerFont;
        }

        private void btnTimes_Click(object sender, EventArgs e)
        {
            Settings.TimesFont = SettingsHelper.ChooseFont(this, Settings.TimesFont, 7, 20);
            lblTimes.Text = MainFont;
        }

        private void btnTextFont_Click(object sender, EventArgs e)
        {
            Settings.TextFont = SettingsHelper.ChooseFont(this, Settings.TextFont, 7, 20);
            lblText.Text = SplitNamesFont;
        }

        private void chkRainbow_CheckedChanged(object sender, EventArgs e)
        {
            label9.Enabled = btnGlod.Enabled = !chkRainbow.Checked;
        }

        private void LayoutSettingsControl_Load(object sender, EventArgs e)
        {
            chkRainbow_CheckedChanged(this, null);
        }
    }
}
