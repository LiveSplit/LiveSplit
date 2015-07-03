using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        RunMetadata Metadata { get; }

        bool HasChanged { get; set; }
        string FilePath { get; set; }
    }

    public static class RunExtensions
    {
        public static bool IsAutoSplitterActive(this IRun run)
        {
            return run.AutoSplitter != null && run.AutoSplitter.IsActivated;
        }

        public static void AddSegment(this IRun run, string name, Time pbSplitTime = default(Time), Time bestSegmentTime = default(Time), Image icon = null, Time splitTime = default(Time), IList<IIndexedTime> segmentHistory = null)
        {
            var segment = new Segment(name, pbSplitTime, bestSegmentTime, icon, splitTime);
            if (segmentHistory != null)
                segment.SegmentHistory = segmentHistory;
            run.Add(segment);
        }

        public static void FixSplits(this IRun run)
        {
            FixWithMethod(run, TimingMethod.RealTime);
            FixWithMethod(run, TimingMethod.GameTime);
        }

        private static void FixWithMethod(IRun run, TimingMethod method)
        {
            FixSegmentHistory(run, method);
            FixComparisonTimes(run, method);
            RemoveDuplicates(run, method);
            RemoveNullValues(run, method);
        }

        private static void FixSegmentHistory(IRun run, TimingMethod method)
        {
            foreach (var curSplit in run)
            {
                var x = 0;
                while (x < curSplit.SegmentHistory.Count)
                {
                    var history = new Time(curSplit.SegmentHistory[x].Time);
                    if (curSplit.BestSegmentTime[method] != null && history[method] < curSplit.BestSegmentTime[method])
                        history[method] = curSplit.BestSegmentTime[method];
                    if (curSplit.BestSegmentTime[method] == null && history[method] != null)
                        curSplit.SegmentHistory.RemoveAt(x);
                    else
                    {
                        curSplit.SegmentHistory[x].Time = history;
                        x++;
                    }
                }

            }
        }

        private static void FixComparisonTimes(IRun run, TimingMethod method)
        {
            foreach (var comparison in run.CustomComparisons)
            {
                var previousTime = TimeSpan.Zero;
                foreach (var curSplit in run)
                {
                    if (curSplit.Comparisons[comparison][method] != null)
                    {
                        if (curSplit.Comparisons[comparison][method] < previousTime)
                        {
                            var newComparison = new Time(curSplit.Comparisons[comparison]);
                            newComparison[method] = previousTime;
                            curSplit.Comparisons[comparison] = newComparison;
                        }
                        var currentSegment = curSplit.Comparisons[comparison][method] - previousTime;
                        if (comparison == Run.PersonalBestComparisonName && (curSplit.BestSegmentTime[method] == null || curSplit.BestSegmentTime[method] > currentSegment))
                        {
                            var newTime = new Time(curSplit.BestSegmentTime);
                            newTime[method] = currentSegment;
                            curSplit.BestSegmentTime = newTime;
                        }
                        previousTime = curSplit.Comparisons[comparison][method].Value;
                    }
                }
            }
        }

        private static void RemoveNullValues(IRun run, TimingMethod method)
        {
            var cache = new List<IIndexedTime>();
            for (var runIndex = run.GetMinSegmentHistoryIndex(); runIndex <= run.AttemptHistory.Count; runIndex++)
            {
                for (var index = 0; index < run.Count; index++)
                {
                    var segmentHistoryElement = run[index].SegmentHistory.FirstOrDefault(x => x.Index == runIndex);
                    if (segmentHistoryElement == null)
                    {
                        RemoveItemsFromCache(run, index, cache);
                    }
                    else if (segmentHistoryElement.Time.RealTime == null && segmentHistoryElement.Time.GameTime == null)
                        cache.Add(segmentHistoryElement);
                    else
                        cache.Clear();
                }
                RemoveItemsFromCache(run, run.Count, cache);
                cache.Clear();
            }
        }

        private static void RemoveDuplicates(IRun run, TimingMethod method)
        {
            for (var index = 0; index < run.Count; index++)
            {
                var history = run[index].SegmentHistory.Select(x => x.Time[method]).Where(x => x != null);
                for (var runIndex = GetMinSegmentHistoryIndex(run); runIndex <= 0; runIndex++)
                {
                    var element = run[index].SegmentHistory.FirstOrDefault(x => x.Index == runIndex);
                    if (element != null && history.Where(x => x.Equals(element.Time[method])).Count() > 1)
                    {
                        var newTime = new Time(element.Time);
                        newTime[method] = null;
                        element.Time = newTime;
                    }
                }
            }

        }

        private static void RemoveItemsFromCache(IRun run, int index, IList<IIndexedTime> cache)
        {
            var ind = index - cache.Count;
            foreach (var item in cache)
            {
                run[ind].SegmentHistory.Remove(item);
                ind++;
            }
            cache.Clear();
        }

        public static int GetMinSegmentHistoryIndex(this IRun run)
        {
            var minIndex = 1;
            foreach (var segment in run)
            {
                foreach (var history in segment.SegmentHistory)
                {
                    if (history.Index < minIndex)
                        minIndex = history.Index;
                }
            }
            return minIndex - 1;
        }

        public static void ImportSegmentHistory(this IRun run)
        {
            var prevTimeRTA = TimeSpan.Zero;
            var prevTimeGameTime = TimeSpan.Zero;
            var index = GetMinSegmentHistoryIndex(run);
            var nullValue = false;

            foreach (var segment in run)
            {
                var newTime = new Time();
                if (segment.PersonalBestSplitTime[TimingMethod.RealTime] == null || segment.PersonalBestSplitTime[TimingMethod.GameTime] == null || nullValue)
                {
                    newTime[TimingMethod.RealTime] = segment.PersonalBestSplitTime[TimingMethod.RealTime] - prevTimeRTA;
                    newTime[TimingMethod.GameTime] = segment.PersonalBestSplitTime[TimingMethod.GameTime] - prevTimeGameTime;
                    segment.SegmentHistory.Add(new IndexedTime(newTime, index));
                    nullValue = false;
                }

                if (segment.PersonalBestSplitTime[TimingMethod.RealTime] != null)
                    prevTimeRTA = segment.PersonalBestSplitTime[TimingMethod.RealTime].Value;
                else
                    nullValue = true;

                if (segment.PersonalBestSplitTime[TimingMethod.GameTime] != null)
                    prevTimeGameTime = segment.PersonalBestSplitTime[TimingMethod.GameTime].Value;
                else
                    nullValue = true;
            }
        }


        public static void ImportBestSegment(this IRun run, int segmentIndex)
        {
            var segment = run[segmentIndex];
            var newTime = new Time();
            if (segment.BestSegmentTime[TimingMethod.RealTime] != null || segment.BestSegmentTime[TimingMethod.GameTime] != null)
            {
                newTime[TimingMethod.RealTime] = segment.BestSegmentTime[TimingMethod.RealTime];
                newTime[TimingMethod.GameTime] = segment.BestSegmentTime[TimingMethod.GameTime];
                segment.SegmentHistory.Add(new IndexedTime(newTime, GetMinSegmentHistoryIndex(run)));
            }
        }

        public static string GetExtendedCategoryName(this IRun run, bool showRegion = false, bool showPlatform = false, bool showVariables = true)
        {
            var list = new List<string>();

            var categoryName = run.CategoryName;

            while (categoryName.Contains('(') && categoryName.Contains(')'))
            {
                var indexStart = categoryName.IndexOf('(');
                var indexEnd = categoryName.IndexOf(')');
                var inside = categoryName.Substring(indexStart + 1, indexEnd - indexStart - 1);
                list.Add(inside);
                categoryName = categoryName.Substring(0, indexStart).Trim();
            }

            if (showVariables)
            {
                var variableValues = run.Metadata.VariableValueNames.Values.Where(x => !string.IsNullOrEmpty(x));
                list.AddRange(variableValues);
            }

            if (run.Metadata.Region != null && !string.IsNullOrEmpty(run.Metadata.Region.Abbreviation) && run.Metadata.Game.Regions.Count > 1 && showRegion)
            {
                list.Add(run.Metadata.Region.Abbreviation);
            }

            if (run.Metadata.Platform != null && !string.IsNullOrEmpty(run.Metadata.PlatformName) && run.Metadata.Game.Platforms.Count > 1 && showPlatform)
            {
                if (run.Metadata.UsesEmulator)
                    list.Add(run.Metadata.PlatformName + " Emulator");
                else
                    list.Add(run.Metadata.PlatformName);
            }

            if (list.Any())
            {
                categoryName += " (" + list.Aggregate((a, b) => a + ", " + b) + ")";
            }

            return categoryName;
        }
    }
}
