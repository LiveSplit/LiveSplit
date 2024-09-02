using System;
using System.Drawing;

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
            Comparisons = (IComparisons)Comparisons.Clone()
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
