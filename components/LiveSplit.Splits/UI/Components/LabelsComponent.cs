using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;

namespace LiveSplit.UI.Components;

public class LabelsComponent : IComponent
{
    public SplitsSettings Settings { get; set; }

    protected SimpleLabel MeasureTimeLabel { get; set; }
    protected SimpleLabel MeasureDeltaLabel { get; set; }

    protected ITimeFormatter TimeFormatter { get; set; }
    protected ITimeFormatter DeltaTimeFormatter { get; set; }

    protected TimeAccuracy CurrentAccuracy { get; set; }
    protected TimeAccuracy CurrentDeltaAccuracy { get; set; }
    protected bool CurrentDropDecimals { get; set; }

    protected int FrameCount { get; set; }

    public GraphicsCache Cache { get; set; }

    public IEnumerable<ColumnData> ColumnsList { get; set; }
    public IList<SimpleLabel> LabelsList { get; set; }

    public float PaddingTop => 0f;
    public float PaddingLeft => 0f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 0f;

    public float VerticalHeight => 25 + Settings.SplitHeight;

    public float MinimumWidth { get; set; }

    public float HorizontalWidth => 10f; /* not available in horizontal mode */

    public float MinimumHeight { get; set; }

    public IDictionary<string, Action> ContextMenuControls => null;
    public LabelsComponent(SplitsSettings settings, IEnumerable<ColumnData> columns)
    {
        Settings = settings;
        MinimumHeight = 31;

        MeasureTimeLabel = new SimpleLabel();
        MeasureDeltaLabel = new SimpleLabel();
        TimeFormatter = new SplitTimeFormatter(Settings.SplitTimesAccuracy);
        DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);

        Cache = new GraphicsCache();
        LabelsList = [];
        ColumnsList = columns;
    }

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        if (Settings.BackgroundGradient == ExtendedGradientType.Alternating)
        {
            g.FillRectangle(new SolidBrush(
                Settings.BackgroundColor
                ), 0, 0, width, height);
        }

        MeasureTimeLabel.Text = TimeFormatter.Format(new TimeSpan(24, 0, 0));
        MeasureDeltaLabel.Text = DeltaTimeFormatter.Format(new TimeSpan(0, 9, 0, 0));

        MeasureTimeLabel.Font = state.LayoutSettings.TimesFont;
        MeasureTimeLabel.IsMonospaced = true;
        MeasureDeltaLabel.Font = state.LayoutSettings.TimesFont;
        MeasureDeltaLabel.IsMonospaced = true;

        MeasureTimeLabel.SetActualWidth(g);
        MeasureDeltaLabel.SetActualWidth(g);

        if (Settings.SplitTimesAccuracy != CurrentAccuracy)
        {
            TimeFormatter = new SplitTimeFormatter(Settings.SplitTimesAccuracy);
            CurrentAccuracy = Settings.SplitTimesAccuracy;
        }

        if (Settings.DeltasAccuracy != CurrentDeltaAccuracy || Settings.DropDecimals != CurrentDropDecimals)
        {
            DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);
            CurrentDeltaAccuracy = Settings.DeltasAccuracy;
            CurrentDropDecimals = Settings.DropDecimals;
        }

        foreach (SimpleLabel label in LabelsList)
        {
            label.ShadowColor = state.LayoutSettings.ShadowsColor;
            label.OutlineColor = state.LayoutSettings.TextOutlineColor;
            label.Y = 0;
            label.Height = height;
        }

        MinimumWidth = 10f;

        if (ColumnsList.Count() == LabelsList.Count)
        {
            float curX = width - 7;
            foreach (SimpleLabel label in LabelsList.Reverse())
            {
                ColumnData column = ColumnsList.ElementAt(LabelsList.IndexOf(label));

                float labelWidth = 0f;
                if (column.Type is ColumnType.DeltaorSplitTime or ColumnType.SegmentDeltaorSegmentTime)
                {
                    labelWidth = Math.Max(MeasureDeltaLabel.ActualWidth, MeasureTimeLabel.ActualWidth);
                }
                else if (column.Type is ColumnType.Delta or ColumnType.SegmentDelta)
                {
                    labelWidth = MeasureDeltaLabel.ActualWidth;
                }
                else
                {
                    labelWidth = MeasureTimeLabel.ActualWidth;
                }

                curX -= labelWidth + 5;
                label.Width = labelWidth;
                label.X = curX + 5;

                label.Font = state.LayoutSettings.TextFont;
                label.HasShadow = state.LayoutSettings.DropShadows;
                label.Draw(g);
            }
        }
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
    }

    public string ComponentName => "Labels";

    public Control GetSettingsControl(LayoutMode mode)
    {
        throw new NotSupportedException();
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        throw new NotSupportedException();
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        throw new NotSupportedException();
    }

    public string UpdateName => throw new NotSupportedException();

    public string XMLURL => throw new NotSupportedException();

    public string UpdateURL => throw new NotSupportedException();

    public Version Version => throw new NotSupportedException();

    protected void UpdateAll(LiveSplitState state)
    {
        RecreateLabels();

        foreach (SimpleLabel label in LabelsList)
        {
            ColumnData column = ColumnsList.ElementAt(LabelsList.IndexOf(label));
            if (string.IsNullOrEmpty(column.Name))
            {
                label.Text = CompositeComparisons.GetShortComparisonName(column.Comparison == "Current Comparison" ? state.CurrentComparison : column.Comparison);
            }
            else
            {
                label.Text = column.Name;
            }

            label.ForeColor = Settings.LabelsColor;
        }
    }

    protected void RecreateLabels()
    {
        if (ColumnsList != null && LabelsList.Count != ColumnsList.Count())
        {
            LabelsList.Clear();
            foreach (ColumnData column in ColumnsList)
            {
                LabelsList.Add(new SimpleLabel()
                {
                    HorizontalAlignment = StringAlignment.Far,
                    VerticalAlignment = StringAlignment.Center
                });
            }
        }
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        UpdateAll(state);

        Cache.Restart();
        Cache["ColumnsCount"] = ColumnsList.Count();
        foreach (SimpleLabel label in LabelsList)
        {
            Cache["Columns" + LabelsList.IndexOf(label) + "Text"] = label.Text;
        }

        if (invalidator != null && (Cache.HasChanged || FrameCount > 1))
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {
    }
}
