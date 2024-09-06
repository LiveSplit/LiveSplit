using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public partial class SplitsSettings : UserControl
{
    private int _VisualSplitCount { get; set; }
    public int VisualSplitCount
    {
        get => _VisualSplitCount;
        set
        {
            _VisualSplitCount = value;
            int max = Math.Max(0, _VisualSplitCount - (AlwaysShowLastSplit ? 2 : 1));
            if (dmnUpcomingSegments.Value > max)
            {
                dmnUpcomingSegments.Value = max;
            }

            dmnUpcomingSegments.Maximum = max;
        }
    }
    static public ISegment HilightSplit { get; set; }
    static public ISegment SectionSplit { get; set; }

    public bool AutomaticAbbreviation { get; set; }
    public Color CurrentSplitTopColor { get; set; }
    public Color CurrentSplitBottomColor { get; set; }
    public int SplitPreviewCount { get; set; }
    public int MinimumMajorSplits { get; set; }
    public float SplitWidth { get; set; }
    public float SplitHeight { get; set; }
    public float ScaledSplitHeight { get => SplitHeight * 10f; set => SplitHeight = value / 10f; }
    public float IconSize { get; set; }

    public bool Display2Rows { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BackgroundColor2 { get; set; }

    public ExtendedGradientType BackgroundGradient { get; set; }
    public string GradientString
    {
        get => BackgroundGradient.ToString();
        set => BackgroundGradient = (ExtendedGradientType)Enum.Parse(typeof(ExtendedGradientType), value);
    }

    public string HeaderComparison { get; set; }
    public string HeaderTimingMethod { get; set; }
    public LiveSplitState CurrentState { get; set; }

    public bool DisplayIcons { get; set; }
    public bool IconShadows { get; set; }
    public bool IndentBlankIcons { get; set; }
    public bool ShowThinSeparators { get; set; }
    public bool AlwaysShowLastSplit { get; set; }
    public bool LockLastSplit { get; set; }
    public bool SeparatorLastSplit { get; set; }

    public bool IndentSubsplits { get; set; }
    public bool HideSubsplits { get; set; }
    public bool ShowSubsplits { get; set; }
    public bool CurrentSectionOnly { get; set; }
    public bool OverrideSubsplitColor { get; set; }
    public Color SubsplitTopColor { get; set; }
    public Color SubsplitBottomColor { get; set; }
    public GradientType SubsplitGradient { get; set; }
    public string SubsplitGradientString
    {
        get => SubsplitGradient.ToString();
        set => SubsplitGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public bool ShowHeader { get; set; }
    public bool IndentSectionSplit { get; set; }
    public bool ShowIconSectionSplit { get; set; }
    public bool ShowSectionIcon { get; set; }
    public Color HeaderTopColor { get; set; }
    public Color HeaderBottomColor { get; set; }
    public GradientType HeaderGradient { get; set; }
    public string HeaderGradientString
    {
        get => HeaderGradient.ToString();
        set => HeaderGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }
    public bool OverrideHeaderColor { get; set; }
    public Color HeaderTextColor { get; set; }
    public bool HeaderText { get; set; }
    public Color HeaderTimesColor { get; set; }
    public bool HeaderTimes { get; set; }
    public TimeAccuracy HeaderAccuracy { get; set; }
    public bool SectionTimer { get; set; }
    public Color SectionTimerColor { get; set; }
    public bool SectionTimerGradient { get; set; }
    public TimeAccuracy SectionTimerAccuracy { get; set; }

    public bool DropDecimals { get; set; }
    public TimeAccuracy DeltasAccuracy { get; set; }

    public bool OverrideDeltasColor { get; set; }
    public Color DeltasColor { get; set; }

    public bool ShowColumnLabels { get; set; }
    public Color LabelsColor { get; set; }

    public Color BeforeNamesColor { get; set; }
    public Color CurrentNamesColor { get; set; }
    public Color AfterNamesColor { get; set; }
    public bool OverrideTextColor { get; set; }
    public Color BeforeTimesColor { get; set; }
    public Color CurrentTimesColor { get; set; }
    public Color AfterTimesColor { get; set; }
    public bool OverrideTimesColor { get; set; }

    public TimeAccuracy SplitTimesAccuracy { get; set; }
    public GradientType CurrentSplitGradient { get; set; }
    public string SplitGradientString
    {
        get => CurrentSplitGradient.ToString();
        set => CurrentSplitGradient = (GradientType)Enum.Parse(typeof(GradientType), value);
    }

    public event EventHandler SplitLayoutChanged;

    public LayoutMode Mode { get; set; }

    public IList<ColumnSettings> ColumnsList { get; set; }
    public Size StartingSize { get; set; }
    public Size StartingTableLayoutSize { get; set; }
    private readonly int startingColumnSettingHeight;

    public SplitsSettings(LiveSplitState state)
    {
        InitializeComponent();

        CurrentState = state;

        StartingSize = Size;
        StartingTableLayoutSize = tableColumns.Size;

        AutomaticAbbreviation = false;
        VisualSplitCount = 8;
        SplitPreviewCount = 1;
        MinimumMajorSplits = 0;
        DisplayIcons = true;
        IconShadows = true;
        ShowThinSeparators = false;
        AlwaysShowLastSplit = true;
        LockLastSplit = true;
        SeparatorLastSplit = true;
        SplitTimesAccuracy = TimeAccuracy.Seconds;
        CurrentSplitTopColor = Color.FromArgb(51, 115, 244);
        CurrentSplitBottomColor = Color.FromArgb(21, 53, 116);
        SplitWidth = 20;
        SplitHeight = 3.6f;
        ScaledSplitHeight = 60;
        IconSize = 24f;
        BeforeNamesColor = Color.FromArgb(255, 255, 255);
        CurrentNamesColor = Color.FromArgb(255, 255, 255);
        AfterNamesColor = Color.FromArgb(255, 255, 255);
        OverrideTextColor = false;
        BeforeTimesColor = Color.FromArgb(255, 255, 255);
        CurrentTimesColor = Color.FromArgb(255, 255, 255);
        AfterTimesColor = Color.FromArgb(255, 255, 255);
        OverrideTimesColor = false;
        CurrentSplitGradient = GradientType.Vertical;
        cmbSplitGradient.SelectedIndexChanged += cmbSplitGradient_SelectedIndexChanged;
        BackgroundColor = Color.Transparent;
        BackgroundColor2 = Color.FromArgb(1, 255, 255, 255);
        BackgroundGradient = ExtendedGradientType.Alternating;
        DropDecimals = true;
        DeltasAccuracy = TimeAccuracy.Tenths;
        OverrideDeltasColor = false;
        DeltasColor = Color.FromArgb(255, 255, 255);
        HeaderComparison = "Current Comparison";
        HeaderTimingMethod = "Current Timing Method";
        Display2Rows = false;
        ShowColumnLabels = false;
        LabelsColor = Color.FromArgb(255, 255, 255);

        IndentBlankIcons = true;
        IndentSubsplits = true;
        HideSubsplits = false;
        ShowSubsplits = false;
        CurrentSectionOnly = false;
        OverrideSubsplitColor = false;
        SubsplitTopColor = Color.FromArgb(0x8D, 0x00, 0x00, 0x00);
        SubsplitBottomColor = Color.Transparent;
        SubsplitGradient = GradientType.Plain;
        ShowHeader = true;
        IndentSectionSplit = true;
        ShowIconSectionSplit = true;
        ShowSectionIcon = true;
        HeaderTopColor = Color.FromArgb(0x2B, 0xFF, 0xFF, 0xFF);
        HeaderBottomColor = Color.FromArgb(0xD8, 0x00, 0x00, 0x00);
        HeaderGradient = GradientType.Vertical;
        OverrideHeaderColor = false;
        HeaderTextColor = Color.FromArgb(255, 255, 255);
        HeaderText = true;
        HeaderTimesColor = Color.FromArgb(255, 255, 255);
        HeaderTimes = true;
        HeaderAccuracy = TimeAccuracy.Tenths;
        SectionTimer = true;
        SectionTimerColor = Color.FromArgb(0x77, 0x77, 0x77);
        SectionTimerGradient = true;
        SectionTimerAccuracy = TimeAccuracy.Tenths;

        chkAutomaticAbbreviation.DataBindings.Add("Checked", this, "AutomaticAbbreviation", false, DataSourceUpdateMode.OnPropertyChanged);
        dmnTotalSegments.DataBindings.Add("Value", this, "VisualSplitCount", false, DataSourceUpdateMode.OnPropertyChanged);
        dmnUpcomingSegments.DataBindings.Add("Value", this, "SplitPreviewCount", false, DataSourceUpdateMode.OnPropertyChanged);
        dmnMinimumMajorSplits.DataBindings.Add("Value", this, "MinimumMajorSplits", false, DataSourceUpdateMode.OnPropertyChanged);
        btnTopColor.DataBindings.Add("BackColor", this, "CurrentSplitTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnBottomColor.DataBindings.Add("BackColor", this, "CurrentSplitBottomColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnBeforeNamesColor.DataBindings.Add("BackColor", this, "BeforeNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnCurrentNamesColor.DataBindings.Add("BackColor", this, "CurrentNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnAfterNamesColor.DataBindings.Add("BackColor", this, "AfterNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnBeforeTimesColor.DataBindings.Add("BackColor", this, "BeforeTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnCurrentTimesColor.DataBindings.Add("BackColor", this, "CurrentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnAfterTimesColor.DataBindings.Add("BackColor", this, "AfterTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkDisplayIcons.DataBindings.Add("Checked", this, "DisplayIcons", false, DataSourceUpdateMode.OnPropertyChanged);
        chkIconShadows.DataBindings.Add("Checked", this, "IconShadows", false, DataSourceUpdateMode.OnPropertyChanged);
        chkIndentBlankIcons.DataBindings.Add("Checked", this, "IndentBlankIcons", false, DataSourceUpdateMode.OnPropertyChanged);
        chkThinSeparators.DataBindings.Add("Checked", this, "ShowThinSeparators", false, DataSourceUpdateMode.OnPropertyChanged);
        chkLastSplit.DataBindings.Add("Checked", this, "AlwaysShowLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideTimesColor.DataBindings.Add("Checked", this, "OverrideTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkLockLastSplit.DataBindings.Add("Checked", this, "LockLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSeparatorLastSplit.DataBindings.Add("Checked", this, "SeparatorLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideDeltaColor.DataBindings.Add("Checked", this, "OverrideDeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnDeltaColor.DataBindings.Add("BackColor", this, "DeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);

        chkIndentSubsplits.DataBindings.Add("Checked", this, "IndentSubsplits", false, DataSourceUpdateMode.OnPropertyChanged);
        chkCurrentSectionOnly.DataBindings.Add("Checked", this, "CurrentSectionOnly", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideSubsplitColor.DataBindings.Add("Checked", this, "OverrideSubsplitColor", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbSubsplitGradient.DataBindings.Add("SelectedItem", this, "SubsplitGradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        btnSubsplitTopColor.DataBindings.Add("BackColor", this, "SubsplitTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnSubsplitBottomColor.DataBindings.Add("BackColor", this, "SubsplitBottomColor", false, DataSourceUpdateMode.OnPropertyChanged);

        chkShowHeader.DataBindings.Add("Checked", this, "ShowHeader", false, DataSourceUpdateMode.OnPropertyChanged);
        chkIndentSectionSplit.DataBindings.Add("Checked", this, "IndentSectionSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShowIconSectionSplit.DataBindings.Add("Checked", this, "ShowIconSectionSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        chkShowSectionIcon.DataBindings.Add("Checked", this, "ShowSectionIcon", false, DataSourceUpdateMode.OnPropertyChanged);
        btnHeaderTopColor.DataBindings.Add("BackColor", this, "HeaderTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnHeaderBottomColor.DataBindings.Add("BackColor", this, "HeaderBottomColor", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbHeaderGradient.DataBindings.Add("SelectedItem", this, "HeaderGradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        chkOverrideHeaderColor.DataBindings.Add("Checked", this, "OverrideHeaderColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnHeaderTextColor.DataBindings.Add("BackColor", this, "HeaderTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkHeaderText.DataBindings.Add("Checked", this, "HeaderText", false, DataSourceUpdateMode.OnPropertyChanged);
        btnHeaderTimesColor.DataBindings.Add("BackColor", this, "HeaderTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkHeaderTimes.DataBindings.Add("Checked", this, "HeaderTimes", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSectionTimer.DataBindings.Add("Checked", this, "SectionTimer", false, DataSourceUpdateMode.OnPropertyChanged);
        btnSectionTimerColor.DataBindings.Add("BackColor", this, "SectionTimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
        chkSectionTimerGradient.DataBindings.Add("Checked", this, "SectionTimerGradient", false, DataSourceUpdateMode.OnPropertyChanged);

        trkIconSize.DataBindings.Add("Value", this, "IconSize", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbSplitGradient.DataBindings.Add("SelectedItem", this, "SplitGradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbHeaderComparison.DataBindings.Add("SelectedItem", this, "HeaderComparison", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbHeaderTimingMethod.DataBindings.Add("SelectedItem", this, "HeaderTimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);

        btnLabelColor.DataBindings.Add("BackColor", this, "LabelsColor", false, DataSourceUpdateMode.OnPropertyChanged);

        ColumnsList = [];
        ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.Delta, "Current Comparison", "Current Timing Method") });
        ColumnsList.Add(new ColumnSettings(CurrentState, "Time", ColumnsList) { Data = new ColumnData("Time", ColumnType.SplitTime, "Current Comparison", "Current Timing Method") });

        startingColumnSettingHeight = ColumnsList[0].Height;
    }

    private void chkColumnLabels_CheckedChanged(object sender, EventArgs e)
    {
        btnLabelColor.Enabled = chkColumnLabels.Checked;
    }

    private void chkDisplayIcons_CheckedChanged(object sender, EventArgs e)
    {
        trkIconSize.Enabled = label5.Enabled = chkIconShadows.Enabled = chkDisplayIcons.Checked;
    }

    private void chkIndentBlankIcons_CheckedChanged(object sender, EventArgs e)
    {
        SplitLayoutChanged(this, null);
    }

    private void chkOverrideTimesColor_CheckedChanged(object sender, EventArgs e)
    {
        label6.Enabled = label9.Enabled = label7.Enabled = btnBeforeTimesColor.Enabled
            = btnCurrentTimesColor.Enabled = btnAfterTimesColor.Enabled = chkOverrideTimesColor.Checked;
    }

    private void chkOverrideDeltaColor_CheckedChanged(object sender, EventArgs e)
    {
        label8.Enabled = btnDeltaColor.Enabled = chkOverrideDeltaColor.Checked;
    }

    private void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
    {
        label3.Enabled = label10.Enabled = label13.Enabled = btnBeforeNamesColor.Enabled
        = btnCurrentNamesColor.Enabled = btnAfterNamesColor.Enabled = chkOverrideTextColor.Checked;
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

    private void chkSeparatorLastSplit_CheckedChanged(object sender, EventArgs e)
    {
        SeparatorLastSplit = chkSeparatorLastSplit.Checked;
        SplitLayoutChanged(this, null);
    }

    private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
        btnColor2.DataBindings.Clear();
        btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
        GradientString = cmbGradientType.SelectedItem.ToString();
    }

    private void cmbSplitGradient_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnTopColor.Visible = cmbSplitGradient.SelectedItem.ToString() != "Plain";
        btnBottomColor.DataBindings.Clear();
        btnBottomColor.DataBindings.Add("BackColor", this, btnTopColor.Visible ? "CurrentSplitBottomColor" : "CurrentSplitTopCOlor", false, DataSourceUpdateMode.OnPropertyChanged);
        SplitGradientString = cmbSplitGradient.SelectedItem.ToString();
    }

    private void chkLockLastSplit_CheckedChanged(object sender, EventArgs e)
    {
        LockLastSplit = chkLockLastSplit.Checked;
        SplitLayoutChanged(this, null);
    }

    private void rdoHundredths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateAccuracy();
    }

    private void rdoTenths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateAccuracy();
    }

    private void rdoSeconds_CheckedChanged(object sender, EventArgs e)
    {
        UpdateAccuracy();
    }

    private void UpdateSubsplitVisibility()
    {
        if (rdoShowSubsplits.Checked)
        {
            ShowSubsplits = true;
            HideSubsplits = false;
            CurrentSectionOnly = false;
            chkCurrentSectionOnly.Enabled = false;
            chkIndentSubsplits.Enabled = true;
            chkIndentSectionSplit.Enabled = false;
            lblMinimumMajorSplits.Enabled = false;
            dmnMinimumMajorSplits.Enabled = false;
        }
        else if (rdoHideSubsplits.Checked)
        {
            ShowSubsplits = false;
            HideSubsplits = true;
            CurrentSectionOnly = chkCurrentSectionOnly.Checked;
            chkCurrentSectionOnly.Enabled = true;
            chkIndentSubsplits.Enabled = false;
            chkIndentSectionSplit.Enabled = false;
            lblMinimumMajorSplits.Enabled = false;
            dmnMinimumMajorSplits.Enabled = false;
        }
        else
        {
            ShowSubsplits = false;
            HideSubsplits = false;
            CurrentSectionOnly = chkCurrentSectionOnly.Checked;
            chkCurrentSectionOnly.Enabled = true;
            chkIndentSubsplits.Enabled = true;
            chkIndentSectionSplit.Enabled = chkIndentSubsplits.Checked;
            lblMinimumMajorSplits.Enabled = true;
            dmnMinimumMajorSplits.Enabled = true;
        }
    }

    private void UpdateAccuracy()
    {
        if (rdoSeconds.Checked)
        {
            SplitTimesAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoTenths.Checked)
        {
            SplitTimesAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoHundredths.Checked)
        {
            SplitTimesAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            SplitTimesAccuracy = TimeAccuracy.Milliseconds;
        }
    }

    private void UpdateDeltaAccuracy()
    {
        if (rdoDeltaSeconds.Checked)
        {
            DeltasAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoDeltaTenths.Checked)
        {
            DeltasAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoDeltaHundredths.Checked)
        {
            DeltasAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            DeltasAccuracy = TimeAccuracy.Milliseconds;
        }
    }

    private void chkLastSplit_CheckedChanged(object sender, EventArgs e)
    {
        AlwaysShowLastSplit = chkLastSplit.Checked;
        VisualSplitCount = VisualSplitCount;
        SplitLayoutChanged(this, null);
    }

    private void chkThinSeparators_CheckedChanged(object sender, EventArgs e)
    {
        ShowThinSeparators = chkThinSeparators.Checked;
        SplitLayoutChanged(this, null);
    }

    private void SplitsSettings_Load(object sender, EventArgs e)
    {
        ResetColumns();

        chkOverrideDeltaColor_CheckedChanged(null, null);
        chkOverrideTextColor_CheckedChanged(null, null);
        chkOverrideTimesColor_CheckedChanged(null, null);
        chkOverrideSubsplitColor_CheckedChanged(null, null);
        chkShowHeader_CheckedChanged(null, null);
        chkOverrideHeaderColor_CheckedChanged(null, null);
        chkSectionTimer_CheckedChanged(null, null);
        chkDisplayIcons_CheckedChanged(null, null);
        chkColumnLabels_CheckedChanged(null, null);

        cmbHeaderComparison.Items.Clear();
        cmbHeaderComparison.Items.Add("Current Comparison");
        cmbHeaderComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != NoneComparisonGenerator.ComparisonName).ToArray());
        if (!cmbHeaderComparison.Items.Contains(HeaderComparison))
        {
            cmbHeaderComparison.Items.Add(HeaderComparison);
        }

        rdoHideSubsplits.Checked = !ShowSubsplits && HideSubsplits;
        rdoShowSubsplits.Checked = ShowSubsplits && !HideSubsplits;
        rdoNormalSubsplits.Checked = !ShowSubsplits && !HideSubsplits;

        rdoSeconds.Checked = SplitTimesAccuracy == TimeAccuracy.Seconds;
        rdoTenths.Checked = SplitTimesAccuracy == TimeAccuracy.Tenths;
        rdoHundredths.Checked = SplitTimesAccuracy == TimeAccuracy.Hundredths;
        rdoMilliseconds.Checked = SplitTimesAccuracy == TimeAccuracy.Milliseconds;

        rdoDeltaSeconds.Checked = DeltasAccuracy == TimeAccuracy.Seconds;
        rdoDeltaTenths.Checked = DeltasAccuracy == TimeAccuracy.Tenths;
        rdoDeltaHundredths.Checked = DeltasAccuracy == TimeAccuracy.Hundredths;
        rdoDeltaMilliseconds.Checked = DeltasAccuracy == TimeAccuracy.Milliseconds;

        rdoHeaderAccuracySeconds.Checked = HeaderAccuracy == TimeAccuracy.Seconds;
        rdoHeaderAccuracyTenths.Checked = HeaderAccuracy == TimeAccuracy.Tenths;
        rdoHeaderAccuracyHundredths.Checked = HeaderAccuracy == TimeAccuracy.Hundredths;
        rdoHeaderAccuracyMilliseconds.Checked = HeaderAccuracy == TimeAccuracy.Milliseconds;

        rdoSectionTimerAccuracySeconds.Checked = SectionTimerAccuracy == TimeAccuracy.Seconds;
        rdoSectionTimerAccuracyTenths.Checked = SectionTimerAccuracy == TimeAccuracy.Tenths;
        rdoSectionTimerAccuracyHundredths.Checked = SectionTimerAccuracy == TimeAccuracy.Hundredths;
        rdoSectionTimerAccuracyMilliseconds.Checked = SectionTimerAccuracy == TimeAccuracy.Milliseconds;

        if (Mode == LayoutMode.Horizontal)
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 5;
            trkSize.Maximum = 120;
            SplitWidth = Math.Min(Math.Max(trkSize.Minimum, SplitWidth), trkSize.Maximum);
            trkSize.DataBindings.Add("Value", this, "SplitWidth", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSplitSize.Text = "Split Width:";
            chkDisplayRows.Enabled = false;
            chkDisplayRows.DataBindings.Clear();
            chkDisplayRows.Checked = true;
            chkColumnLabels.DataBindings.Clear();
            chkColumnLabels.Enabled = chkColumnLabels.Checked = false;
        }
        else
        {
            trkSize.DataBindings.Clear();
            trkSize.Minimum = 0;
            trkSize.Maximum = 250;
            ScaledSplitHeight = Math.Min(Math.Max(trkSize.Minimum, ScaledSplitHeight), trkSize.Maximum);
            trkSize.DataBindings.Add("Value", this, "ScaledSplitHeight", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSplitSize.Text = "Split Height:";
            chkDisplayRows.Enabled = true;
            chkDisplayRows.DataBindings.Clear();
            chkDisplayRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            chkColumnLabels.DataBindings.Clear();
            chkColumnLabels.Enabled = true;
            chkColumnLabels.DataBindings.Add("Checked", this, "ShowColumnLabels", false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        Version version = SettingsHelper.ParseVersion(element["Version"]);

        AutomaticAbbreviation = SettingsHelper.ParseBool(element["AutomaticAbbreviation"], false);
        CurrentSplitTopColor = SettingsHelper.ParseColor(element["CurrentSplitTopColor"], Color.FromArgb(51, 115, 244));
        CurrentSplitBottomColor = SettingsHelper.ParseColor(element["CurrentSplitBottomColor"], Color.FromArgb(21, 53, 116));
        VisualSplitCount = SettingsHelper.ParseInt(element["VisualSplitCount"], 8);
        SplitPreviewCount = SettingsHelper.ParseInt(element["SplitPreviewCount"], 1);
        MinimumMajorSplits = SettingsHelper.ParseInt(element["MinimumMajorSplits"], 0);
        DisplayIcons = SettingsHelper.ParseBool(element["DisplayIcons"], true);
        ShowThinSeparators = SettingsHelper.ParseBool(element["ShowThinSeparators"], false);
        AlwaysShowLastSplit = SettingsHelper.ParseBool(element["AlwaysShowLastSplit"], true);
        SplitWidth = SettingsHelper.ParseFloat(element["SplitWidth"], 20);
        IndentBlankIcons = SettingsHelper.ParseBool(element["IndentBlankIcons"], true);
        IndentSubsplits = SettingsHelper.ParseBool(element["IndentSubsplits"], true);
        HideSubsplits = SettingsHelper.ParseBool(element["HideSubsplits"], false);
        ShowSubsplits = SettingsHelper.ParseBool(element["ShowSubsplits"], false);
        CurrentSectionOnly = SettingsHelper.ParseBool(element["CurrentSectionOnly"], false);
        OverrideSubsplitColor = SettingsHelper.ParseBool(element["OverrideSubsplitColor"], false);
        SubsplitTopColor = SettingsHelper.ParseColor(element["SubsplitTopColor"], Color.FromArgb(0x8D, 0x00, 0x00, 0x00));
        SubsplitBottomColor = SettingsHelper.ParseColor(element["SubsplitBottomColor"], Color.Transparent);
        SubsplitGradientString = SettingsHelper.ParseString(element["SubsplitGradient"], GradientType.Plain.ToString());
        ShowHeader = SettingsHelper.ParseBool(element["ShowHeader"], true);
        IndentSectionSplit = SettingsHelper.ParseBool(element["IndentSectionSplit"], true);
        ShowIconSectionSplit = SettingsHelper.ParseBool(element["ShowIconSectionSplit"], true);
        ShowSectionIcon = SettingsHelper.ParseBool(element["ShowSectionIcon"], true);
        HeaderTopColor = SettingsHelper.ParseColor(element["HeaderTopColor"], Color.FromArgb(0x2B, 0xFF, 0xFF, 0xFF));
        HeaderBottomColor = SettingsHelper.ParseColor(element["HeaderBottomColor"], Color.FromArgb(0xD8, 0x00, 0x00, 0x00));
        HeaderGradientString = SettingsHelper.ParseString(element["HeaderGradient"], GradientType.Vertical.ToString());
        OverrideHeaderColor = SettingsHelper.ParseBool(element["OverrideHeaderColor"], false);
        HeaderTextColor = SettingsHelper.ParseColor(element["HeaderTextColor"], Color.FromArgb(255, 255, 255));
        HeaderText = SettingsHelper.ParseBool(element["HeaderText"], true);
        HeaderTimesColor = SettingsHelper.ParseColor(element["HeaderTimesColor"], Color.FromArgb(255, 255, 255));
        HeaderTimes = SettingsHelper.ParseBool(element["HeaderTimes"], true);
        HeaderAccuracy = SettingsHelper.ParseEnum(element["HeaderAccuracy"], TimeAccuracy.Tenths);
        SectionTimer = SettingsHelper.ParseBool(element["SectionTimer"], true);
        SectionTimerColor = SettingsHelper.ParseColor(element["SectionTimerColor"], Color.FromArgb(0x77, 0x77, 0x77));
        SectionTimerGradient = SettingsHelper.ParseBool(element["SectionTimerGradient"], true);
        SectionTimerAccuracy = SettingsHelper.ParseEnum(element["SectionTimerAccuracy"], TimeAccuracy.Tenths);
        OverrideTimesColor = SettingsHelper.ParseBool(element["OverrideTimesColor"], false);
        BeforeTimesColor = SettingsHelper.ParseColor(element["BeforeTimesColor"], Color.FromArgb(255, 255, 255));
        CurrentTimesColor = SettingsHelper.ParseColor(element["CurrentTimesColor"], Color.FromArgb(255, 255, 255));
        AfterTimesColor = SettingsHelper.ParseColor(element["AfterTimesColor"], Color.FromArgb(255, 255, 255));
        SplitHeight = SettingsHelper.ParseFloat(element["SplitHeight"], 3.6f);
        SplitGradientString = SettingsHelper.ParseString(element["CurrentSplitGradient"], GradientType.Vertical.ToString());
        BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Transparent);
        BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.FromArgb(1, 255, 255, 255));
        GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], ExtendedGradientType.Alternating.ToString());
        SeparatorLastSplit = SettingsHelper.ParseBool(element["SeparatorLastSplit"], true);
        DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"], true);
        DeltasAccuracy = SettingsHelper.ParseEnum(element["DeltasAccuracy"], TimeAccuracy.Tenths);
        OverrideDeltasColor = SettingsHelper.ParseBool(element["OverrideDeltasColor"], false);
        DeltasColor = SettingsHelper.ParseColor(element["DeltasColor"], Color.FromArgb(255, 255, 255));
        Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        SplitTimesAccuracy = SettingsHelper.ParseEnum(element["SplitTimesAccuracy"], TimeAccuracy.Seconds);
        LockLastSplit = SettingsHelper.ParseBool(element["LockLastSplit"], true);
        IconSize = SettingsHelper.ParseFloat(element["IconSize"], 24f);
        IconShadows = SettingsHelper.ParseBool(element["IconShadows"], true);
        ShowColumnLabels = SettingsHelper.ParseBool(element["ShowColumnLabels"], false);
        LabelsColor = SettingsHelper.ParseColor(element["LabelsColor"], Color.FromArgb(255, 255, 255));

        if (version >= new Version(1, 7))
        {
            HeaderComparison = SettingsHelper.ParseString(element["HeaderComparison"], "Current Comparison");
            HeaderTimingMethod = SettingsHelper.ParseString(element["HeaderTimingMethod"], "Current Timing Method");
            XmlElement columnsElement = element["Columns"];
            ColumnsList.Clear();
            foreach (object child in columnsElement.ChildNodes)
            {
                var columnData = ColumnData.FromXml((XmlNode)child);
                ColumnsList.Add(new ColumnSettings(CurrentState, columnData.Name, ColumnsList) { Data = columnData });
            }
        }
        else
        {
            HeaderComparison = SettingsHelper.ParseString(element["Comparison"], "Current Comparison");
            HeaderTimingMethod = "Current Timing Method";
            ColumnsList.Clear();
            if (SettingsHelper.ParseBool(element["ShowSplitTimes"]))
            {
                ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.Delta, HeaderComparison, "Current Timing Method") });
                ColumnsList.Add(new ColumnSettings(CurrentState, "Time", ColumnsList) { Data = new ColumnData("Time", ColumnType.SplitTime, HeaderComparison, "Current Timing Method") });
            }
            else
            {
                ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.DeltaorSplitTime, HeaderComparison, "Current Timing Method") });
            }
        }

        if (version >= new Version(1, 3))
        {
            BeforeNamesColor = SettingsHelper.ParseColor(element["BeforeNamesColor"]);
            CurrentNamesColor = SettingsHelper.ParseColor(element["CurrentNamesColor"]);
            AfterNamesColor = SettingsHelper.ParseColor(element["AfterNamesColor"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
        }
        else
        {
            if (version >= new Version(1, 2))
            {
                BeforeNamesColor = CurrentNamesColor = AfterNamesColor = SettingsHelper.ParseColor(element["SplitNamesColor"]);
            }
            else
            {
                BeforeNamesColor = Color.FromArgb(255, 255, 255);
                CurrentNamesColor = Color.FromArgb(255, 255, 255);
                AfterNamesColor = Color.FromArgb(255, 255, 255);
            }

            OverrideTextColor = !SettingsHelper.ParseBool(element["UseTextColor"], true);
        }
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
        int hashCode = SettingsHelper.CreateSetting(document, parent, "Version", "1.7") ^
        SettingsHelper.CreateSetting(document, parent, "AutomaticAbbreviation", AutomaticAbbreviation) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentSplitTopColor", CurrentSplitTopColor) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentSplitBottomColor", CurrentSplitBottomColor) ^
        SettingsHelper.CreateSetting(document, parent, "VisualSplitCount", VisualSplitCount) ^
        SettingsHelper.CreateSetting(document, parent, "SplitPreviewCount", SplitPreviewCount) ^
        SettingsHelper.CreateSetting(document, parent, "MinimumMajorSplits", MinimumMajorSplits) ^
        SettingsHelper.CreateSetting(document, parent, "DisplayIcons", DisplayIcons) ^
        SettingsHelper.CreateSetting(document, parent, "ShowThinSeparators", ShowThinSeparators) ^
        SettingsHelper.CreateSetting(document, parent, "AlwaysShowLastSplit", AlwaysShowLastSplit) ^
        SettingsHelper.CreateSetting(document, parent, "SplitWidth", SplitWidth) ^
        SettingsHelper.CreateSetting(document, parent, "SplitTimesAccuracy", SplitTimesAccuracy) ^
        SettingsHelper.CreateSetting(document, parent, "BeforeNamesColor", BeforeNamesColor) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentNamesColor", CurrentNamesColor) ^
        SettingsHelper.CreateSetting(document, parent, "AfterNamesColor", AfterNamesColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
        SettingsHelper.CreateSetting(document, parent, "BeforeTimesColor", BeforeTimesColor) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentTimesColor", CurrentTimesColor) ^
        SettingsHelper.CreateSetting(document, parent, "AfterTimesColor", AfterTimesColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTimesColor", OverrideTimesColor) ^
        SettingsHelper.CreateSetting(document, parent, "LockLastSplit", LockLastSplit) ^
        SettingsHelper.CreateSetting(document, parent, "IconSize", IconSize) ^
        SettingsHelper.CreateSetting(document, parent, "IconShadows", IconShadows) ^
        SettingsHelper.CreateSetting(document, parent, "SplitHeight", SplitHeight) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentSplitGradient", CurrentSplitGradient) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
        SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
        SettingsHelper.CreateSetting(document, parent, "SeparatorLastSplit", SeparatorLastSplit) ^
        SettingsHelper.CreateSetting(document, parent, "DeltasAccuracy", DeltasAccuracy) ^
        SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideDeltasColor", OverrideDeltasColor) ^
        SettingsHelper.CreateSetting(document, parent, "DeltasColor", DeltasColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderComparison", HeaderComparison) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderTimingMethod", HeaderTimingMethod) ^
        SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
        SettingsHelper.CreateSetting(document, parent, "IndentBlankIcons", IndentBlankIcons) ^
        SettingsHelper.CreateSetting(document, parent, "IndentSubsplits", IndentSubsplits) ^
        SettingsHelper.CreateSetting(document, parent, "HideSubsplits", HideSubsplits) ^
        SettingsHelper.CreateSetting(document, parent, "ShowSubsplits", ShowSubsplits) ^
        SettingsHelper.CreateSetting(document, parent, "CurrentSectionOnly", CurrentSectionOnly) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideSubsplitColor", OverrideSubsplitColor) ^
        SettingsHelper.CreateSetting(document, parent, "SubsplitGradient", SubsplitGradient) ^
        SettingsHelper.CreateSetting(document, parent, "ShowHeader", ShowHeader) ^
        SettingsHelper.CreateSetting(document, parent, "IndentSectionSplit", IndentSectionSplit) ^
        SettingsHelper.CreateSetting(document, parent, "ShowIconSectionSplit", ShowIconSectionSplit) ^
        SettingsHelper.CreateSetting(document, parent, "ShowSectionIcon", ShowSectionIcon) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderGradient", HeaderGradient) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideHeaderColor", OverrideHeaderColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderText", HeaderText) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderTimes", HeaderTimes) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderAccuracy", HeaderAccuracy) ^
        SettingsHelper.CreateSetting(document, parent, "SectionTimer", SectionTimer) ^
        SettingsHelper.CreateSetting(document, parent, "SectionTimerGradient", SectionTimerGradient) ^
        SettingsHelper.CreateSetting(document, parent, "SectionTimerAccuracy", SectionTimerAccuracy) ^
        SettingsHelper.CreateSetting(document, parent, "SubsplitTopColor", SubsplitTopColor) ^
        SettingsHelper.CreateSetting(document, parent, "SubsplitBottomColor", SubsplitBottomColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderTopColor", HeaderTopColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderBottomColor", HeaderBottomColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderTextColor", HeaderTextColor) ^
        SettingsHelper.CreateSetting(document, parent, "HeaderTimesColor", HeaderTimesColor) ^
        SettingsHelper.CreateSetting(document, parent, "SectionTimerColor", SectionTimerColor) ^
        SettingsHelper.CreateSetting(document, parent, "ShowColumnLabels", ShowColumnLabels) ^
        SettingsHelper.CreateSetting(document, parent, "LabelsColor", LabelsColor);

        XmlElement columnsElement = null;
        if (document != null)
        {
            columnsElement = document.CreateElement("Columns");
            parent.AppendChild(columnsElement);
        }

        int count = 1;
        foreach (ColumnData columnData in ColumnsList.Select(x => x.Data))
        {
            XmlElement settings = null;
            if (document != null)
            {
                settings = document.CreateElement("Settings");
                columnsElement.AppendChild(settings);
            }

            hashCode ^= columnData.CreateElement(document, settings) * count;
            count++;
        }

        return hashCode;
    }

    private void ColorButtonClick(object sender, EventArgs e)
    {
        SettingsHelper.ColorButtonClick((Button)sender, this);
    }

    private void rdoHideSubsplits_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSubsplitVisibility();
    }

    private void rdoNormalSubsplits_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSubsplitVisibility();
    }

    private void rdoShowSubsplits_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSubsplitVisibility();
    }

    private void chkOverrideSubsplitColor_CheckedChanged(object sender, EventArgs e)
    {
        label15.Enabled = btnSubsplitTopColor.Enabled = btnSubsplitBottomColor.Enabled = cmbSubsplitGradient.Enabled = chkOverrideSubsplitColor.Checked;
    }

    private void cmbSubsplitGradient_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnSubsplitTopColor.Visible = cmbSubsplitGradient.SelectedItem.ToString() != "Plain";
        btnSubsplitBottomColor.DataBindings.Clear();
        btnSubsplitBottomColor.DataBindings.Add("BackColor", this, btnSubsplitTopColor.Visible ? "SubsplitBottomColor" : "SubsplitTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
        SubsplitGradientString = cmbSubsplitGradient.SelectedItem.ToString();
    }

    private void chkShowHeader_CheckedChanged(object sender, EventArgs e)
    {
        chkShowSectionIcon.Enabled = groupBox12.Enabled = groupBox13.Enabled
            = lblComparison.Enabled = cmbHeaderComparison.Enabled = lblTimingMethod.Enabled = cmbHeaderTimingMethod.Enabled
            = chkShowHeader.Checked;
    }

    private void cmbHeaderGradient_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnHeaderTopColor.Visible = cmbHeaderGradient.SelectedItem.ToString() != "Plain";
        btnHeaderBottomColor.DataBindings.Clear();
        btnHeaderBottomColor.DataBindings.Add("BackColor", this, btnHeaderTopColor.Visible ? "HeaderBottomColor" : "HeaderTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
        HeaderGradientString = cmbHeaderGradient.SelectedItem.ToString();
    }

    private void chkOverrideHeaderColor_CheckedChanged(object sender, EventArgs e)
    {
        label17.Enabled = btnHeaderTextColor.Enabled = label18.Enabled = btnHeaderTimesColor.Enabled = chkOverrideHeaderColor.Checked;
    }

    private void UpdateHeaderAccuracy()
    {
        if (rdoHeaderAccuracySeconds.Checked)
        {
            HeaderAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoHeaderAccuracyTenths.Checked)
        {
            HeaderAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoHeaderAccuracyHundredths.Checked)
        {
            HeaderAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            HeaderAccuracy = TimeAccuracy.Milliseconds;
        }
    }

    private void rdoHeaderAccuracySeconds_CheckedChanged(object sender, EventArgs e)
    {
        UpdateHeaderAccuracy();
    }

    private void rdoHeaderAccuracyTenths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateHeaderAccuracy();
    }

    private void rdoHeaderAccuracyHundredths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateHeaderAccuracy();
    }

    private void chkSectionTimer_CheckedChanged(object sender, EventArgs e)
    {
        label19.Enabled = btnSectionTimerColor.Enabled = chkSectionTimerGradient.Enabled = groupBox14.Enabled = chkSectionTimer.Checked;
    }

    private void UpdateSectionTimerAccuracy()
    {
        if (rdoSectionTimerAccuracySeconds.Checked)
        {
            SectionTimerAccuracy = TimeAccuracy.Seconds;
        }
        else if (rdoSectionTimerAccuracyTenths.Checked)
        {
            SectionTimerAccuracy = TimeAccuracy.Tenths;
        }
        else if (rdoSectionTimerAccuracyHundredths.Checked)
        {
            SectionTimerAccuracy = TimeAccuracy.Hundredths;
        }
        else
        {
            SectionTimerAccuracy = TimeAccuracy.Milliseconds;
        }
    }

    private void rdoSectionTimerAccuracySeconds_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSectionTimerAccuracy();
    }

    private void rdoSectionTimerAccuracyTenths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSectionTimerAccuracy();
    }

    private void rdoSectionTimerAccuracyHundredths_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSectionTimerAccuracy();
    }

    private void cmbHeaderComparison_SelectedIndexChanged(object sender, EventArgs e)
    {
        HeaderComparison = cmbHeaderComparison.SelectedItem.ToString();
    }

    private void cmbHeaderTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        HeaderTimingMethod = cmbHeaderTimingMethod.SelectedItem.ToString();
    }

    private void chkIndentSubsplits_CheckedChanged(object sender, EventArgs e)
    {
        if (!ShowSubsplits && !HideSubsplits)
        {
            chkIndentSectionSplit.Enabled = chkIndentSubsplits.Checked;
        }
    }

    private void ResetColumns()
    {
        ClearLayout();
        int index = 1;
        foreach (ColumnSettings column in ColumnsList)
        {
            UpdateLayoutForColumn();
            AddColumnToLayout(column, index);
            column.UpdateEnabledButtons();
            index++;
        }
    }

    private void AddColumnToLayout(ColumnSettings column, int index)
    {
        tableColumns.Controls.Add(column, 0, index);
        tableColumns.SetColumnSpan(column, 4);
        column.ColumnRemoved -= column_ColumnRemoved;
        column.MovedUp -= column_MovedUp;
        column.MovedDown -= column_MovedDown;
        column.ColumnRemoved += column_ColumnRemoved;
        column.MovedUp += column_MovedUp;
        column.MovedDown += column_MovedDown;
    }

    private void column_MovedDown(object sender, EventArgs e)
    {
        var column = (ColumnSettings)sender;
        int index = ColumnsList.IndexOf(column);
        ColumnsList.Remove(column);
        ColumnsList.Insert(index + 1, column);
        ResetColumns();
        column.SelectControl();
    }

    private void column_MovedUp(object sender, EventArgs e)
    {
        var column = (ColumnSettings)sender;
        int index = ColumnsList.IndexOf(column);
        ColumnsList.Remove(column);
        ColumnsList.Insert(index - 1, column);
        ResetColumns();
        column.SelectControl();
    }

    private void column_ColumnRemoved(object sender, EventArgs e)
    {
        var column = (ColumnSettings)sender;
        int index = ColumnsList.IndexOf(column);
        ColumnsList.Remove(column);
        ResetColumns();
        if (ColumnsList.Count > 0)
        {
            ColumnsList.Last().SelectControl();
        }
        else
        {
            chkColumnLabels.Select();
        }
    }

    private void ClearLayout()
    {
        tableColumns.RowCount = 1;
        tableColumns.RowStyles.Clear();
        tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, StartingTableLayoutSize.Height));
        tableColumns.Size = StartingTableLayoutSize;
        foreach (ColumnSettings control in tableColumns.Controls.OfType<ColumnSettings>().ToList())
        {
            tableColumns.Controls.Remove(control);
        }

        Size = StartingSize;
    }

    private void UpdateLayoutForColumn()
    {
        tableColumns.RowCount++;
        tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, startingColumnSettingHeight));
        tableColumns.Size = new Size(tableColumns.Size.Width, tableColumns.Size.Height + startingColumnSettingHeight);
        Size = new Size(Size.Width, Size.Height + startingColumnSettingHeight);
        groupColumns.Size = new Size(groupColumns.Size.Width, groupColumns.Size.Height + startingColumnSettingHeight);
    }

    private void btnAddColumn_Click(object sender, EventArgs e)
    {
        UpdateLayoutForColumn();

        var columnControl = new ColumnSettings(CurrentState, "#" + (ColumnsList.Count + 1), ColumnsList);
        ColumnsList.Add(columnControl);
        AddColumnToLayout(columnControl, ColumnsList.Count);

        foreach (ColumnSettings column in ColumnsList)
        {
            column.UpdateEnabledButtons();
        }
    }

    private void chkAutomaticAbbreviation_CheckedChanged(object sender, EventArgs e)
    {
        AutomaticAbbreviation = chkAutomaticAbbreviation.Checked;
    }
}
