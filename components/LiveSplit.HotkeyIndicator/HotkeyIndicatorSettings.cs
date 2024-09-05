using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

public partial class HotkeyIndicatorSettings : UserControl
{
    public float IndicatorHeight { get; set; }
    public float IndicatorWidth { get; set; }
    public LayoutMode Mode { get; set; }
    public Color HotkeysOnColor { get; set; }
    public Color HotkeysOffColor { get; set; }
    public GradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public HotkeyIndicatorSettings()
    {
        InitializeComponent();

        IndicatorHeight = 3f;
        IndicatorWidth = 3f;

        HotkeysOnColor = Color.FromArgb(41, 204, 84);
        HotkeysOffColor = Color.FromArgb(204, 55, 41);

        btnColor1.DataBindings.Add("BackColor", this, "HotkeysOnColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "HotkeysOffColor", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        IndicatorHeight = SettingsHelper.ParseFloat(element["IndicatorHeight"], 3f);
        IndicatorWidth = SettingsHelper.ParseFloat(element["IndicatorWidth"], 3f);
        HotkeysOnColor = SettingsHelper.ParseColor(element["HotkeysOnColor"]);
        HotkeysOffColor = SettingsHelper.ParseColor(element["HotkeysOffColor"]);
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        XmlElement parent = document.CreateElement("Settings");
        CreateSettingsNode(document, parent);
        return parent;
    }

    public int GetSettingsHashCode()
    {
        return CreateSettingsNode(null, null);
    }

    private int CreateSettingsNode(XmlDocument document, XmlElement parent)
    {
        return SettingsHelper.CreateSetting(document, parent, "HotkeysOnColor", HotkeysOnColor) ^
        SettingsHelper.CreateSetting(document, parent, "HotkeysOffColor", HotkeysOffColor) ^
        SettingsHelper.CreateSetting(document, parent, "IndicatorHeight", IndicatorHeight) ^
        SettingsHelper.CreateSetting(document, parent, "IndicatorWidth", IndicatorWidth);
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }

    private void HotkeyIndicatorSettings_Load(object sender, EventArgs e)
    {
        if (Mode == LayoutMode.Horizontal)
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 1;
            trkSize.Maximum = 50;
            trkSize.DataBindings.Add("Value", this, "IndicatorWidth", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSize.Text = "Width:";
        }
        else
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 1;
            trkSize.Maximum = 50;
            trkSize.DataBindings.Add("Value", this, "IndicatorHeight", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSize.Text = "Height:";
        }
    }
}
