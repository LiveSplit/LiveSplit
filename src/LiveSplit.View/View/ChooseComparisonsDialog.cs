using System;
using System.Collections.Generic;
using System.Windows.Forms;

using LiveSplit.Model.Comparisons;

namespace LiveSplit.View;

public partial class ChooseComparisonsDialog : Form
{
    public IDictionary<string, bool> ComparisonGeneratorStates { get; set; }
    public int HcpHistorySize { get; set; }
    public int HcpNBestRuns { get; set; }

    protected bool DialogInitialized;

    public ChooseComparisonsDialog()
    {
        InitializeComponent();
        DialogInitialized = false;
        comparisonsListBox.Items.AddRange(new[]
        {
            BestSegmentsComparisonGenerator.ComparisonName,
            BestSplitTimesComparisonGenerator.ComparisonName,
            AverageSegmentsComparisonGenerator.ComparisonName,
            MedianSegmentsComparisonGenerator.ComparisonName,
            WorstSegmentsComparisonGenerator.ComparisonName,
            PercentileComparisonGenerator.ComparisonName,
            LatestRunComparisonGenerator.ComparisonName,
            HCPComparisonGenerator.ComparisonName,
            NoneComparisonGenerator.ComparisonName
        });

    }

    private void UpdateHcpSettingsVisibility()
    {
        // Find the index of the HCP comparison in the list box
        int hcpIndex = comparisonsListBox.Items.IndexOf(HCPComparisonGenerator.ComparisonName);

        // Enable/disable the group box based on whether HCP is checked
        bool isHcpChecked = comparisonsListBox.GetItemChecked(hcpIndex);
        groupBoxHcpSettings.Visible = isHcpChecked;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void comparisonsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (DialogInitialized)
        {
            string generatorName = (string)comparisonsListBox.Items[e.Index];
            ComparisonGeneratorStates[generatorName] = e.NewValue == CheckState.Checked;

            if (generatorName == HCPComparisonGenerator.ComparisonName)
            {
                BeginInvoke(new Action(UpdateHcpSettingsVisibility));
            }
        }
    }

    private void ChooseComparisonsDialog_Load(object sender, EventArgs e)
    {
        foreach (KeyValuePair<string, bool> generator in ComparisonGeneratorStates)
        {
            comparisonsListBox.SetItemChecked(comparisonsListBox.Items.IndexOf(generator.Key), generator.Value);
        }

        numericUpDownHcpHistorySize.ValueChanged -= numericUpDownHcpHistorySize_ValueChanged;
        numericUpDownHcpNBestRuns.ValueChanged -= numericUpDownHcpNBestRuns_ValueChanged;

        numericUpDownHcpHistorySize.Value = HcpHistorySize;
        numericUpDownHcpNBestRuns.Value = HcpNBestRuns;
        
        numericUpDownHcpHistorySize.ValueChanged += numericUpDownHcpHistorySize_ValueChanged;
        numericUpDownHcpNBestRuns.ValueChanged += numericUpDownHcpNBestRuns_ValueChanged;

        DialogInitialized = true;

        UpdateHcpSettingsVisibility();
    }

    private void numericUpDownHcpHistorySize_ValueChanged(object sender, EventArgs e)
    {
        if (numericUpDownHcpHistorySize.Value < numericUpDownHcpNBestRuns.Value)
        {
            numericUpDownHcpNBestRuns.Value = numericUpDownHcpHistorySize.Value;
        }

        HcpHistorySize = (int)numericUpDownHcpHistorySize.Value;
    }

    private void numericUpDownHcpNBestRuns_ValueChanged(object sender, EventArgs e)
    {
        if (numericUpDownHcpNBestRuns.Value > numericUpDownHcpHistorySize.Value)
        {
            numericUpDownHcpHistorySize.Value = numericUpDownHcpNBestRuns.Value;
        }

        HcpNBestRuns = (int)numericUpDownHcpNBestRuns.Value;
    }
}
