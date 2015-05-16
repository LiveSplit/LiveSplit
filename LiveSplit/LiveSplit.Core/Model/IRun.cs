using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace LiveSplit.Model
{
    public interface IRun : IList<ISegment>, ICloneable
    {
        Image GameIcon { get; set; }
        string GameName { get; set; }
        string CategoryName { get; set; }
        TimeSpan Offset { get; set; }
        int AttemptCount { get; set; }
        IList<Attempt> AttemptHistory { get; set; }

        AutoSplitter AutoSplitter { get; set; }
        XmlElement AutoSplitterSettings { get; set; }

        IList<IComparisonGenerator> ComparisonGenerators { get; set; }
        IList<string> CustomComparisons { get; set; }
        IEnumerable<string> Comparisons { get; }

        bool HasChanged { get; set; }
        string FilePath { get; set; }

        void AddSegment(string name, Time pbSplitTime = default(Time), Time bestSegmentTime = default(Time), Image icon = null, Time splitTime = default(Time), IList<IIndexedTime> segmentHistory = null);
        void FixSplits();
        void ImportSegmentHistory();
        void ImportBestSegment(int segmentIndex);
        int GetMinSegmentHistoryIndex();
    }

    public static class RunExtensions
    {
        public static bool IsAutoSplitterActive(this IRun run)
        {
            return run.AutoSplitter != null && run.AutoSplitter.IsActivated;
        }
    }
}
