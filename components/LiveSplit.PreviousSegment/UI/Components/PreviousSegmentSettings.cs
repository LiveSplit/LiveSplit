using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public partial class PreviousSegmentSettings : UserControl
{
    public Color TextColor { get; set; }
    public bool OverrideTextColor { get; set; }
    public Color BackgroundColor { get; set; }
    public Color BackgroundColor2 { get; set; }
    public GradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public TimeAccuracy DeltaAccuracy { get; set; }
    public bool DropDecimals { get; set; }
    public bool Display2Rows { get; set; }
    public bool ShowPossibleTimeSave { get; set; }
    public TimeAccuracy TimeSaveAccuracy { get; set; }

    public string Comparison { get; set; }
    public LiveSplitState CurrentState { get; set; }

    public LayoutMode Mode { get; set; }

    public PreviousSegmentSettings()
    {
        InitializeComponent();

        TextColor = Color.FromArgb(255, 255, 255);
        OverrideTextColor = false;
        BackgroundColor = Color.Transparent;
        BackgroundColor2 = Color.Transparent;
        BackgroundGradient = GradientType.Plain;
        DeltaAccuracy = TimeAccuracy.Tenths;
        TimeSaveAccuracy = TimeAccuracy.Tenths;
        DropDecimals = true;
        Comparison = "Current Comparison";
        Display2Rows = false;
        ShowPossibleTimeSave = false;

        btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverride.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
        chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
        chkPossibleTimeSave.DataBindings.Add("Checked", this, "ShowPossibleTimeSave", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void chkOverride_CheckedChanged(object sender, EventArgs e)
    {
        label1.Enabled = btnTextColor.Enabled = chkOverride.Checked;
    }

    private void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
    {
        Comparison = cmbComparison.SelectedItem.ToString();
    }

    private void rdoDeltaHundredths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateDeltaAccuracy();
    }

    private void rdoDeltaTenths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateDeltaAccuracy();
    }

    private void rdoDeltaSeconds_CheckedChanged(object sender, EventArgs e)
    {
        UpdateDeltaAccuracy();
    }

    private void PreviousSegmentSettings_Load(object sender, EventArgs e)
    {
        chkOverride_CheckedChanged(null, null);
        chkPossibleTimeSave_CheckedChanged(null, null);
        cmbComparison.Items.Clear();
        cmbComparison.Items.Add("Current Comparison");
        cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x is not BestSplitTimesComparisonGenerator.ComparisonName and not NoneComparisonGenerator.ComparisonName).ToArray());
        if (!cmbComparison.Items.Contains(Comparison))
        {
            cmbComparison.Items.Add(Comparison);
        }

        rdoDeltaHundredths.Checked = DeltaAccuracy == TimeAccuracy.Hundredths;
        rdoDeltaTenths.Checked = DeltaAccuracy == TimeAccuracy.Tenths;
        rdoDeltaSeconds.Checked = DeltaAccuracy == TimeAccuracy.Seconds;
        rdoTimeSaveHundredths.Checked = TimeSaveAccuracy == TimeAccuracy.Hundredths;
        rdoTimeSaveTenths.Checked = TimeSaveAccuracy == TimeAccuracy.Tenths;
        rdoTimeSaveSeconds.Checked = TimeSaveAccuracy == TimeAccuracy.Seconds;
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
    }

    private void UpdateDeltaAccuracy()
    {
        if (rdoDeltaSeconds.Checked)
        {
            DeltaAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoDeltaTenths.Checked)
        {
            DeltaAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoDeltaHundredths.Checked)
        {
            DeltaAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            DeltaAccuracy = TimeAccuracy.Milliseconds;
        }
    }

    private void UpdateTimeSaveAccuracy()
    {
        if (rdoTimeSaveSeconds.Checked)
        {
            TimeSaveAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoTimeSaveTenths.Checked)
        {
            TimeSaveAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoTimeSaveHundredths.Checked)
        {
            TimeSaveAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            TimeSaveAccuracy = TimeAccuracy.Milliseconds;
        }
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
        BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
        BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
        GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
        DeltaAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["DeltaAccuracy"]);
        DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"]);
        Comparison = SettingsHelper.ParseString(element["Comparison"]);
        Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        ShowPossibleTimeSave = SettingsHelper.ParseBool(element["ShowPossibleTimeSave"], false);
        TimeSaveAccuracy = SettingsHelper.ParseEnum(element["TimeSaveAccuracy"], TimeAccuracy.Tenths);
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
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
        SettingsHelper.CreateSetting(document, parent, "DeltaAccuracy", DeltaAccuracy) ^
        SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
        SettingsHelper.CreateSetting(document, parent, "Comparison", Comparison) ^
        SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
        SettingsHelper.CreateSetting(document, parent, "ShowPossibleTimeSave", ShowPossibleTimeSave) ^
        SettingsHelper.CreateSetting(document, parent, "TimeSaveAccuracy", TimeSaveAccuracy);
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }

    private void rdoTimeSaveSeconds_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTimeSaveAccuracy();
    }

    private void rdoTimeSaveTenths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTimeSaveAccuracy();
    }

    private void rdoTimeSaveHundredths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTimeSaveAccuracy();
    }

    private void chkPossibleTimeSave_CheckedChanged(object sender, EventArgs e)
    {
        rdoTimeSaveSeconds.Enabled = rdoTimeSaveTenths.Enabled = rdoTimeSaveHundredths.Enabled = boxTimeSaveAccuracy.Enabled = chkPossibleTimeSave.Checked;
    }
}
