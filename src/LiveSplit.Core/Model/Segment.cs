using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model;

[Serializable]
public class Segment : ISegment
{
    public Image Icon { get; set; }
    public string Name { get; set; }
    public Time PersonalBestSplitTime
    {
        get => Comparisons[Run.PersonalBestComparisonName];
        set => Comparisons[Run.PersonalBestComparisonName] = value;
    }
    public IComparisons Comparisons { get; set; }
    public Time BestSegmentTime { get; set; }
    public Time SplitTime { get; set; }
    public SegmentHistory SegmentHistory { get; set; }
    public Dictionary<string, string> CustomVariableValues { get; private set; }

    public Segment(
        string name, Time pbSplitTime = default,
        Time bestSegmentTime = default, Image icon = null,
        Time splitTime = default)
    {
        Comparisons = new CompositeComparisons();
        Name = name;
        PersonalBestSplitTime = pbSplitTime;
        BestSegmentTime = bestSegmentTime;
        SplitTime = splitTime;
        Icon = icon;
        SegmentHistory = [];
        CustomVariableValues = [];
    }

    public Segment Clone()
    {
        SegmentHistory newSegmentHistory = SegmentHistory.Clone();

        return new Segment(Name)
        {
            BestSegmentTime = BestSegmentTime,
            SplitTime = SplitTime,
            Icon = Icon,
            SegmentHistory = newSegmentHistory,
            CustomVariableValues = CustomVariableValues.ToDictionary(x => x.Key, x => x.Value),
            Comparisons = (IComparisons)Comparisons.Clone()
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
