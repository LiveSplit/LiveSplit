using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Text;

namespace LiveSplit.Model
{
    public interface IRun : IList<ISegment>, ICloneable, INotifyPropertyChanged
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

        public static void ClearHistory(this IRun run)
        {
            run.AttemptHistory.Clear();
            foreach (var segment in run)
            {
                segment.SegmentHistory.Clear();
            }
        }

        public static void ClearTimes(this IRun run)
        {
            run.ClearHistory();
            run.CustomComparisons.Clear();
            run.CustomComparisons.Add(Model.Run.PersonalBestComparisonName);
            foreach (var segment in run)
            {
                segment.Comparisons.Clear();
                segment.BestSegmentTime = default(Time);
            }
            run.AttemptCount = 0;
            run.Metadata.RunID = null;
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
            var maxIndex = run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var runIndex = run.GetMinSegmentHistoryIndex(); runIndex <= maxIndex; runIndex++)
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
            return minIndex;
        }

        public static void ImportSegmentHistory(this IRun run)
        {
            var prevTimeRTA = TimeSpan.Zero;
            var prevTimeGameTime = TimeSpan.Zero;
            var index = GetMinSegmentHistoryIndex(run) - 1;
            var nullValue = false;

            foreach (var segment in run)
            {
                if (segment.PersonalBestSplitTime.RealTime == null || segment.PersonalBestSplitTime.GameTime == null || nullValue)
                {
                    segment.SegmentHistory.Add(new IndexedTime(
                        new Time(segment.PersonalBestSplitTime.RealTime - prevTimeRTA,
                        segment.PersonalBestSplitTime.GameTime - prevTimeGameTime)
                        , index));
                    nullValue = false;
                }

                if (segment.PersonalBestSplitTime.RealTime != null)
                    prevTimeRTA = segment.PersonalBestSplitTime.RealTime.Value;
                else
                    nullValue = true;

                if (segment.PersonalBestSplitTime.GameTime!= null)
                    prevTimeGameTime = segment.PersonalBestSplitTime.GameTime.Value;
                else
                    nullValue = true;
            }
        }


        public static void ImportBestSegment(this IRun run, int segmentIndex)
        {
            var segment = run[segmentIndex];
            if (segment.BestSegmentTime.RealTime != null || segment.BestSegmentTime.GameTime != null)
            {
                segment.SegmentHistory.Add(new IndexedTime(segment.BestSegmentTime, GetMinSegmentHistoryIndex(run) - 1));
            }
        }

        public static string GetExtendedFileName(this IRun run, bool useExtendedCategoryName = true)
        {
            var extendedName = run.GetExtendedName(useExtendedCategoryName);

            var stringBuilder = new StringBuilder();

            foreach (var c in extendedName)
            {
                if (c != '\\' && c != '/' && c != ':' && c != '*' && c != '?' && c != '"' && c != '<' && c != '>' && c != '|')
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string GetExtendedName(this IRun run, bool useExtendedCategoryName = true)
        {
            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(run.GameName))
            {
                stringBuilder.Append(run.GameName);
            }

            var categoryName = run.CategoryName;

            if (useExtendedCategoryName)
            {
                categoryName = run.GetExtendedCategoryName();
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" - ");
                }

                stringBuilder.Append(categoryName);
            }

            return stringBuilder.ToString();
        }

        public static string GetExtendedCategoryName(this IRun run, bool showRegion = false, bool showPlatform = false, bool showVariables = true)
        {
            var list = new List<string>();

            var categoryName = run.CategoryName;

            if (string.IsNullOrEmpty(categoryName))
                return string.Empty;

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
                var variables = run.Metadata.VariableValueNames.Where(x => !string.IsNullOrEmpty(x.Value));

                foreach (var variable in variables)
                {
                    var name = variable.Key.TrimEnd('?');
                    var value = variable.Value;
                    var valueLower = value.ToLowerInvariant();

                    if (valueLower == "yes")
                    {
                        list.Add(name);
                    }
                    else if (valueLower == "no")
                    {
                        list.Add("No " + name);
                    }
                    else
                    {
                        list.Add(value);
                    }
                }
            }

            if (run.Metadata.Region != null && !string.IsNullOrEmpty(run.Metadata.Region.Abbreviation) && run.Metadata.Game.Regions.Count > 1 && showRegion)
            {
                list.Add(run.Metadata.Region.Abbreviation);
            }

            if (showPlatform)
            {
                if (run.Metadata.Platform != null && !string.IsNullOrEmpty(run.Metadata.PlatformName) && run.Metadata.Game.Platforms.Count > 1)
                {
                    if (run.Metadata.UsesEmulator)
                        list.Add(run.Metadata.PlatformName + " Emulator");
                    else
                        list.Add(run.Metadata.PlatformName);
                }
                else if (run.Metadata.UsesEmulator)
                {
                    list.Add("Emulator");
                }
            }

            if (list.Any())
            {
                categoryName += " (" + list.Aggregate((a, b) => a + ", " + b) + ")";
            }

            return categoryName;
        }
    }
}
