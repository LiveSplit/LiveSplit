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
        String GameName { get; set; }
        String CategoryName { get; set; }
        TimeSpan Offset { get; set; }
        int AttemptCount { get; set; }
        IList<IIndexedTime> RunHistory { get; set; }

        AutoSplitter AutoSplitter { get; set; }
        XmlElement AutoSplitterSettings { get; set; }

        IList<IComparisonGenerator> ComparisonGenerators { get; set; }
        IList<String> CustomComparisons { get; set; }
        IEnumerable<String> Comparisons { get; }

        bool HasChanged { get; set; }
        String FilePath { get; set; }

        void AddSegment(String name, Time pbSplitTime = default(Time), Time bestSegmentTime = default(Time), Image icon = null, Time splitTime = default(Time), IList<Time> segmentHistory = null);
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
