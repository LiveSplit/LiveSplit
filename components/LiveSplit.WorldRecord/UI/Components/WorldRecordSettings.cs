using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public partial class WorldRecordSettings : UserControl
{
    public Color TextColor { get; set; }
    public bool OverrideTextColor { get; set; }
    public Color TimeColor { get; set; }
    public bool OverrideTimeColor { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BackgroundColor2 { get; set; }
    public GradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public LiveSplitState CurrentState { get; set; }
    public bool Display2Rows { get; set; }
    public bool CenteredText { get; set; }

    public bool FilterVariables { get; set; }
    public bool FilterPlatform { get; set; }
    public bool FilterRegion { get; set; }
    public bool FilterSubcategories { get; set; }

    public string TimingMethod { get; set; }

    public LayoutMode Mode { get; set; }

    public WorldRecordSettings()
    {
        InitializeComponent();

        TextColor = Color.FromArgb(255, 255, 255);
        OverrideTextColor = false;
        TimeColor = Color.FromArgb(255, 255, 255);
        OverrideTimeColor = false;
        BackgroundColor = Color.Transparent;
        BackgroundColor2 = Color.Transparent;
        BackgroundGradient = GradientType.Plain;
        Display2Rows = false;
        CenteredText = true;
        FilterVariables = false;
        FilterPlatform = false;
        FilterRegion = false;
        FilterSubcategories = true;
        TimingMethod = "Default for Leaderboard";

        chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideTimeColor.DataBindings.Add("Checked", this, "OverrideTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnTimeColor.DataBindings.Add("BackColor", this, "TimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkRegion.DataBindings.Add("Checked", this, "FilterRegion", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPlatform.DataBindings.Add("Checked", this, "FilterPlatform", false, DataSourceUpdateMode.OnPropertyChanged);
        chkVariables.DataBindings.Add("Checked", this, "FilterVariables", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSubcategories.DataBindings.Add("Checked", this, "FilterSubcategories", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void chkOverrideTimeColor_CheckedChanged(object sender, EventArgs e)
    {
        label2.Enabled = btnTimeColor.Enabled = chkOverrideTimeColor.Checked;
    }

    private void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
    {
        label1.Enabled = btnTextColor.Enabled = chkOverrideTextColor.Checked;
    }

    private void WorldRecordSettings_Load(object sender, EventArgs e)
    {
        chkOverrideTextColor_CheckedChanged(null, null);
        chkOverrideTimeColor_CheckedChanged(null, null);
        if (Mode == LayoutMode.Horizontal)
        {
            chkTwoRows.Enabled = false;
            chkTwoRows.DataBindings.Clear();
            chkTwoRows.Checked = true;
        }
        else
        {
            chkTwoRows.Enabled = true;
            chkTwoRows.DataBindings.Clear();
            chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        chkTwoRows_CheckedChanged(null, null);
    }

    private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
        btnColor2.DataBindings.Clear();
        btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        GradientString = cmbGradientType.SelectedItem.ToString();
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        TextColor = SettingsHelper.ParseColor(element["TextColor"]);
        OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
        TimeColor = SettingsHelper.ParseColor(element["TimeColor"]);
        OverrideTimeColor = SettingsHelper.ParseBool(element["OverrideTimeColor"]);
        BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
        BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
        GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
        Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"]);
        CenteredText = SettingsHelper.ParseBool(element["CenteredText"]);
        FilterRegion = SettingsHelper.ParseBool(element["FilterRegion"]);
        FilterPlatform = SettingsHelper.ParseBool(element["FilterPlatform"]);
        FilterVariables = SettingsHelper.ParseBool(element["FilterVariables"]);
        FilterSubcategories = SettingsHelper.ParseBool(element["FilterSubcategories"], true);
        TimingMethod = SettingsHelper.ParseString(element["TimingMethod"], "Default for Leaderboard");
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
        return SettingsHelper.CreateSetting(document, parent, "Version", "1.6") ^
        SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
        SettingsHelper.CreateSetting(document, parent, "TimeColor", TimeColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTimeColor", OverrideTimeColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
        SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
        SettingsHelper.CreateSetting(document, parent, "CenteredText", CenteredText) ^
        SettingsHelper.CreateSetting(document, parent, "FilterRegion", FilterRegion) ^
        SettingsHelper.CreateSetting(document, parent, "FilterPlatform", FilterPlatform) ^
        SettingsHelper.CreateSetting(document, parent, "FilterVariables", FilterVariables) ^
        SettingsHelper.CreateSetting(document, parent, "FilterSubcategories", FilterSubcategories) ^
        SettingsHelper.CreateSetting(document, parent, "TimingMethod", TimingMethod);
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }

    private void chkTwoRows_CheckedChanged(object sender, EventArgs e)
    {
        if (chkTwoRows.Checked)
        {
            chkCenteredText.Enabled = false;
            chkCenteredText.DataBindings.Clear();
            chkCenteredText.Checked = false;
        }
        else
        {
            chkCenteredText.Enabled = true;
            chkCenteredText.DataBindings.Clear();
            chkCenteredText.DataBindings.Add("Checked", this, "CenteredText", false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }

    private void cmbTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        TimingMethod = cmbTimingMethod.SelectedItem.ToString();
    }
}
