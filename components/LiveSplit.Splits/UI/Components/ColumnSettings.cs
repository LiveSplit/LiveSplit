using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

namespace LiveSplit.UI.Components;

public partial class ColumnSettings : UserControl
{
    public string ColumnName { get => Data.Name; set => Data.Name = value; }
    public string Type
    {
        get => GetColumnType(Data.Type);
        set
        {
            Data.Type = ParseColumnType(value);
            UpdateComparisonItems();
        }
    }
    public string Comparison { get => Data.Comparison; set => Data.Comparison = value; }
    public string TimingMethod { get => Data.TimingMethod; set => Data.TimingMethod = value; }

    public ColumnData Data { get; set; }
    protected LiveSplitState CurrentState { get; set; }
    protected IList<ColumnSettings> ColumnsList { get; set; }

    protected int ColumnIndex => ColumnsList.IndexOf(this);
    protected int TotalColumns => ColumnsList.Count;

    public event EventHandler ColumnRemoved;
    public event EventHandler MovedUp;
    public event EventHandler MovedDown;

    public ColumnSettings(LiveSplitState state, string columnName, IList<ColumnSettings> columnsList)
    {
        InitializeComponent();

        Data = new ColumnData(columnName, ColumnType.Delta, "Current Comparison", "Current Timing Method");

        CurrentState = state;
        ColumnsList = columnsList;
    }

    private void cmbTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        TimingMethod = cmbTimingMethod.SelectedItem.ToString();
    }

    private void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
    {
        Comparison = cmbComparison.SelectedItem.ToString();
    }

    private void cmbColumnType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Type = cmbColumnType.SelectedItem.ToString();
    }

    private void ColumnSettings_Load(object sender, EventArgs e)
    {
        UpdateComparisonItems();

        txtName.DataBindings.Clear();
        cmbColumnType.DataBindings.Clear();
        cmbTimingMethod.DataBindings.Clear();
        txtName.DataBindings.Add("Text", this, "ColumnName", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbColumnType.DataBindings.Add("SelectedItem", this, "Type", false, DataSourceUpdateMode.OnPropertyChanged);
        cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    public void UpdateEnabledButtons()
    {
        btnMoveDown.Enabled = ColumnIndex < TotalColumns - 1;
        btnMoveUp.Enabled = ColumnIndex > 0;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        groupColumn.Text = $"Column: {txtName.Text}";
    }

    private void UpdateComparisonItems()
    {
        cmbComparison.Items.Clear();
        cmbComparison.Items.Add("Current Comparison");

        if (Data.Type is ColumnType.Delta or ColumnType.DeltaorSplitTime or ColumnType.SplitTime)
        {
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != NoneComparisonGenerator.ComparisonName).ToArray());
        }
        else
        {
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x is not BestSplitTimesComparisonGenerator.ComparisonName and not NoneComparisonGenerator.ComparisonName).ToArray());
            if (Comparison == BestSplitTimesComparisonGenerator.ComparisonName)
            {
                Comparison = "Current Comparison";
            }
        }

        if (!cmbComparison.Items.Contains(Comparison))
        {
            cmbComparison.Items.Add(Comparison);
        }

        cmbComparison.DataBindings.Clear();
        cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private static string GetColumnType(ColumnType type)
    {
        if (type == ColumnType.SplitTime)
        {
            return "Split Time";
        }
        else if (type == ColumnType.Delta)
        {
            return "Delta";
        }
        else if (type == ColumnType.DeltaorSplitTime)
        {
            return "Delta or Split Time";
        }
        else if (type == ColumnType.SegmentTime)
        {
            return "Segment Time";
        }
        else if (type == ColumnType.SegmentDelta)
        {
            return "Segment Delta";
        }
        else
        {
            return "Segment Delta or Segment Time";
        }
    }

    private static ColumnType ParseColumnType(string columnType)
    {
        return (ColumnType)Enum.Parse(typeof(ColumnType), columnType.Replace(" ", string.Empty));
    }

    private void btnRemoveColumn_Click(object sender, EventArgs e)
    {
        ColumnRemoved?.Invoke(this, null);
    }

    private void btnMoveUp_Click(object sender, EventArgs e)
    {
        MovedUp?.Invoke(this, null);
    }

    private void btnMoveDown_Click(object sender, EventArgs e)
    {
        MovedDown?.Invoke(this, null);
    }

    public void SelectControl()
    {
        btnRemoveColumn.Select();
    }
}
