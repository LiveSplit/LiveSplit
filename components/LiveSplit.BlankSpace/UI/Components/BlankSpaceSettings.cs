using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

public partial class BlankSpaceSettings : UserControl
{
    public float SpaceHeight { get; set; }
    public float SpaceWidth { get; set; }

    public LayoutMode Mode { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BackgroundColor2 { get; set; }
    public GradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public BlankSpaceSettings()
    {
        InitializeComponent();

        SpaceHeight = 100;
        SpaceWidth = 100;
        BackgroundColor = Color.Transparent;
        BackgroundColor2 = Color.Transparent;
        BackgroundGradient = GradientType.Plain;
        cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedText = cmbGradientType.SelectedItem.ToString();
        btnColor1.Visible = selectedText != "Plain";
        btnColor2.DataBindings.Clear();
        btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        GradientString = cmbGradientType.SelectedItem.ToString();
    }

    private void BlankSpaceSettings_Load(object sender, EventArgs e)
    {
        if (Mode == LayoutMode.Horizontal)
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 1;
            trkSize.Maximum = 1000;
            trkSize.DataBindings.Add("Value", this, "SpaceWidth", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSize.Text = "Width:";
        }
        else
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 1;
            trkSize.Maximum = 1000;
            trkSize.DataBindings.Add("Value", this, "SpaceHeight", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSize.Text = "Height:";
        }
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        Version version = SettingsHelper.ParseVersion(element["Version"]);

        SpaceHeight = SettingsHelper.ParseFloat(element["SpaceHeight"]);
        SpaceWidth = SettingsHelper.ParseFloat(element["SpaceWidth"]);
        BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Transparent);
        BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.Transparent);
        GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], GradientType.Plain.ToString());
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
        return SettingsHelper.CreateSetting(document, parent, "Version", "1.7") ^
        SettingsHelper.CreateSetting(document, parent, "SpaceHeight", SpaceHeight) ^
        SettingsHelper.CreateSetting(document, parent, "SpaceWidth", SpaceWidth) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient);
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }
}
