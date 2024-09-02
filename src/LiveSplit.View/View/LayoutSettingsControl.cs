using System;
using System.Drawing;
using System.Windows.Forms;

using LiveSplit.Options;
using LiveSplit.UI;

namespace LiveSplit.View;

public partial class LayoutSettingsControl : UserControl
{
    public LayoutSettingsControl()
    {
        InitializeComponent();
    }

    public Options.LayoutSettings Settings { get; set; }
    public new ILayout Layout { get; set; }

    private Image originalBackgroundImage { get; set; }

    public string TimerFont => SettingsHelper.FormatFont(Settings.TimerFont);
    public string MainFont => SettingsHelper.FormatFont(Settings.TimesFont);
    public string SplitNamesFont => SettingsHelper.FormatFont(Settings.TextFont);

    public float Opacity { get => Settings.Opacity * 100f; set => Settings.Opacity = value / 100f; }
    public float ImageOpacity { get => Settings.ImageOpacity * 100f; set => Settings.ImageOpacity = value / 100f; }
    public float ImageBlur { get => Settings.ImageBlur * 100f; set => Settings.ImageBlur = value / 100f; }

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
        return type switch
        {
            BackgroundType.HorizontalGradient => "Horizontal Gradient",
            BackgroundType.VerticalGradient => "Vertical Gradient",
            BackgroundType.Image => "Image",
            _ => "Solid Color",
        };
    }

    private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedItem = cmbBackgroundType.SelectedItem.ToString();
        btnBackground.Visible = selectedItem is not "Solid Color" and not "Image";
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
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*",
                Title = "Set Background Image..."
            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    var image = Image.FromFile(dialog.FileName);
                    if (Settings.BackgroundImage != null && Settings.BackgroundImage != originalBackgroundImage)
                    {
                        Settings.BackgroundImage.Dispose();
                    }

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
        // Scale down font in dialog so that size is closer to other font settings, and to allow more granular control over size
        var timerFont = new Font(Settings.TimerFont.FontFamily.Name, Settings.TimerFont.Size / 50f * 18f, Settings.TimerFont.Style, GraphicsUnit.Pixel);
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(timerFont, 7, 20);
        dialog.FontChanged += (s, ev) => updateTimerFont(((CustomFontDialog.FontChangedEventArgs)ev).NewFont);
        dialog.ShowDialog(this);
        lblTimer.Text = TimerFont;
    }

    private void updateTimerFont(Font timerFont)
    {
        // Scale font back up to the size the Timer component expects
        Settings.TimerFont = new Font(timerFont.FontFamily.Name, timerFont.Size / 18f * 50f, timerFont.Style, GraphicsUnit.Pixel);
    }

    private void btnTimes_Click(object sender, EventArgs e)
    {
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(Settings.TimesFont, 11, 26);
        dialog.FontChanged += (s, ev) => Settings.TimesFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
        dialog.ShowDialog(this);
        lblTimes.Text = MainFont;
    }

    private void btnTextFont_Click(object sender, EventArgs e)
    {
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(Settings.TextFont, 11, 26);
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
        chkRainbow_CheckedChanged(null, null);
        chkAntiAliasing_CheckedChanged(null, null);
    }
}
