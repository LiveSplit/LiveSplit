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

        public Color TextColor { get { return Settings.TextColor; } set { Settings.TextColor = value; } }
        public Color BackgroundColor { get { return Settings.BackgroundColor; } set { Settings.BackgroundColor = value; } }
        public Color BackgroundColor2 { get { return Settings.BackgroundColor2; } set { Settings.BackgroundColor2 = value; } }
        public Color ThinSeparatorsColor { get { return Settings.ThinSeparatorsColor; } set { Settings.ThinSeparatorsColor = value; } }
        public Color SeparatorsColor { get { return Settings.SeparatorsColor; } set { Settings.SeparatorsColor = value; } }
        public Color PersonalBestColor { get { return Settings.PersonalBestColor; } set { Settings.PersonalBestColor = value; } }
        public Color AheadGainingTimeColor { get { return Settings.AheadGainingTimeColor; } set { Settings.AheadGainingTimeColor = value; } }
        public Color AheadLosingTimeColor { get { return Settings.AheadLosingTimeColor; } set { Settings.AheadLosingTimeColor = value; } }
        public Color BehindGainingTimeColor { get { return Settings.BehindGainingTimeColor; } set { Settings.BehindGainingTimeColor = value; } }
        public Color BehindLosingTimeColor { get { return Settings.BehindLosingTimeColor; } set { Settings.BehindLosingTimeColor = value; } }
        public Color BestSegmentColor { get { return Settings.BestSegmentColor; } set { Settings.BestSegmentColor = value; } }
        public Color NotRunningColor { get { return Settings.NotRunningColor; } set { Settings.NotRunningColor = value; } }
        public Color PausedColor { get { return Settings.PausedColor; } set { Settings.PausedColor = value; } }
        public Color ShadowsColor { get { return Settings.ShadowsColor; } set { Settings.ShadowsColor = value; } }
        public string TimerFont { get { return string.Format("{0} {1}", Settings.TimerFont.FontFamily.Name, Settings.TimerFont.Style); }}
        public string MainFont { get { return string.Format("{0} {1}", Settings.TimesFont.FontFamily.Name, Settings.TimesFont.Style); ; } }
        public string SplitNamesFont { get { return string.Format("{0} {1}", Settings.TextFont.FontFamily.Name, Settings.TextFont.Style); ; } }

        public GradientType BackgroundGradient { get { return Settings.BackgroundGradient; } set { Settings.BackgroundGradient = value; } }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public bool ShowBestSegments { get { return Settings.ShowBestSegments; } set { Settings.ShowBestSegments = value; } }
        public bool AlwaysOnTop { get { return Settings.AlwaysOnTop; } set { Settings.AlwaysOnTop = value; } }
        public bool AntiAliasing { get { return Settings.AntiAliasing; } set { Settings.AntiAliasing = value; } }
        public bool DropShadows { get { return Settings.DropShadows; } set { Settings.DropShadows = value; } }

        public float Opacity { get { return Settings.Opacity*100f; } set { Settings.Opacity = value/100f; } }

        public LayoutSettingsControl(Options.LayoutSettings settings, ILayout layout)
        {
            InitializeComponent();
            Settings = settings;
            Layout = layout;
            chkBestSegments.DataBindings.Add("Checked", this, "ShowBestSegments", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAlwaysOnTop.DataBindings.Add("Checked", this, "AlwaysOnTop", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAntiAliasing.DataBindings.Add("Checked", this, "AntiAliasing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropShadows.DataBindings.Add("Checked", this, "DropShadows", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            btnThinSep.DataBindings.Add("BackColor", this, "ThinSeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSeparators.DataBindings.Add("BackColor", this, "SeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPB.DataBindings.Add("BackColor", this, "PersonalBestColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadGaining.DataBindings.Add("BackColor", this, "AheadGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadLosing.DataBindings.Add("BackColor", this, "AheadLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindGaining.DataBindings.Add("BackColor", this, "BehindGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindLosing.DataBindings.Add("BackColor", this, "BehindLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnGlod.DataBindings.Add("BackColor", this, "BestSegmentColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnNotRunning.DataBindings.Add("BackColor", this, "NotRunningColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPausedColor.DataBindings.Add("BackColor", this, "PausedColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnShadowsColor.DataBindings.Add("BackColor", this, "ShadowsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimer.DataBindings.Add("Text", this, "TimerFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblText.DataBindings.Add("Text", this, "SplitNamesFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimes.DataBindings.Add("Text", this, "MainFont", false, DataSourceUpdateMode.OnPropertyChanged);
            trkOpacity.DataBindings.Add("Value", this, "Opacity", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnBackground.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnBackground2.DataBindings.Clear();
            btnBackground2.DataBindings.Add("BackColor", this, btnBackground.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
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
    }
}
