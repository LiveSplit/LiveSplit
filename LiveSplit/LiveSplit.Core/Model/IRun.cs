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

        public static void AddSegment(this IRun run, string name, Time pbSplitTime = default(Time), Time bestSegmentTime = default(Time), Image icon = null, Time splitTime = default(Time), SegmentHistory segmentHistory = null)
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
            RemoveNullValues(run);
        }

        private static void FixWithMethod(IRun run, TimingMethod method)
        {
            FixComparisonTimes(run, method);
            FixSegmentHistory(run, method);
            RemoveDuplicates(run, method);
        }

        private static void FixSegmentHistory(IRun run, TimingMethod method)
        {
            var maxIndex = run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            foreach (var curSplit in run)
            {
                for (var runIndex = curSplit.SegmentHistory.GetMinIndex(); runIndex <= maxIndex; runIndex++)
                {
                    Time historyTime;
                    if (curSplit.SegmentHistory.TryGetValue(runIndex, out historyTime))
                    {
                        //Make sure no times in the history are lower than the Best Segment
                        if (curSplit.BestSegmentTime[method] != null && historyTime[method] < curSplit.BestSegmentTime[method])
                            historyTime[method] = curSplit.BestSegmentTime[method];

                        //If the Best Segment is gone, clear the history
                        if (curSplit.BestSegmentTime[method] == null && historyTime[method] != null)
                            curSplit.SegmentHistory.Remove(runIndex);
                        else
                            curSplit.SegmentHistory[runIndex] = historyTime;
                    }
                }
            }
        }

        private static void FixComparisonTimes(IRun run, TimingMethod method)
        {
            //Remove negative Best Segment times
            foreach (var curSplit in run)
            {
                if (curSplit.BestSegmentTime[method] < TimeSpan.Zero)
                {
                    var newTime = curSplit.BestSegmentTime;
                    newTime[method] = null;
                    curSplit.BestSegmentTime = newTime;
                }
            }

            foreach (var comparison in run.CustomComparisons)
            {
                var previousTime = TimeSpan.Zero;
                foreach (var curSplit in run)
                {
                    if (curSplit.Comparisons[comparison][method] != null)
                    {
                        //Prevent comparison times from decreasing from one split to the next
                        if (curSplit.Comparisons[comparison][method] < previousTime)
                        {
                            var newComparison = curSplit.Comparisons[comparison];
                            newComparison[method] = previousTime;
                            curSplit.Comparisons[comparison] = newComparison;
                        }

                        //Fix Best Segment time if the PB segment is faster
                        var currentSegment = curSplit.Comparisons[comparison][method] - previousTime;
                        if (comparison == Run.PersonalBestComparisonName && (curSplit.BestSegmentTime[method] == null || curSplit.BestSegmentTime[method] > currentSegment))
                        {
                            var newTime = curSplit.BestSegmentTime;
                            newTime[method] = currentSegment;
                            curSplit.BestSegmentTime = newTime;
                        }
                        previousTime = curSplit.Comparisons[comparison][method].Value;
                    }
                }
            }
        }

        private static void RemoveNullValues(IRun run)
        {
            var cache = new List<IIndexedTime>();
            var maxIndex = run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var runIndex = run.GetMinSegmentHistoryIndex(); runIndex <= maxIndex; runIndex++)
            {
                for (var index = 0; index < run.Count; index++)
                {
                    Time segmentHistoryElement;
                    if (!run[index].SegmentHistory.TryGetValue(runIndex, out segmentHistoryElement))
                    {
                        //Remove null times in history that aren't followed by a non-null time
                        RemoveItemsFromCache(run, index, cache);
                    }
                    else if (segmentHistoryElement.RealTime == null && segmentHistoryElement.GameTime == null)
                        cache.Add(new IndexedTime(segmentHistoryElement, runIndex));
                    else
                        cache.Clear();
                }
                RemoveItemsFromCache(run, run.Count, cache);
                cache.Clear();
            }
        }

        private static void RemoveDuplicates(IRun run, TimingMethod method)
        {
            foreach (var segment in run)
            {
                var history = segment.SegmentHistory.Select(x => x.Value[method]).Where(x => x != null).ToList();
                for (var runIndex = segment.SegmentHistory.GetMinIndex(); runIndex <= 0; runIndex++)
                {
                    //Remove elements in the imported Segment History if they're duplicates of the real Segment History
                    Time element;
                    if (segment.SegmentHistory.TryGetValue(runIndex, out element) && history.Count(x => x.Equals(element[method])) > 1)
                    {
                        segment.SegmentHistory.Remove(runIndex);
                        history.Remove(element[method]);
                    }
                }
            }
        }

        private static void RemoveItemsFromCache(IRun run, int index, IList<IIndexedTime> cache)
        {
            var ind = index - cache.Count;
            foreach (var item in cache)
            {
                run[ind].SegmentHistory.Remove(item.Index);
                ind++;
            }
            cache.Clear();
        }

        public static int GetMinSegmentHistoryIndex(this IRun run)
        {
            return run.Min(segment => segment.SegmentHistory.GetMinIndex());
        }

        public static void ImportSegmentHistory(this IRun run)
        {
            var minIndex = GetMinSegmentHistoryIndex(run);
            run.ImportSegmentHistory(TimingMethod.RealTime, minIndex - 1);
            run.ImportSegmentHistory(TimingMethod.GameTime, minIndex - 2);
        }

        private static void ImportSegmentHistory(this IRun run, TimingMethod method, int index)
        {
            var prevTime = TimeSpan.Zero;

            foreach (var segment in run)
            {
                //Import the PB splits into the history
                segment.SegmentHistory.Add(index, new Time(method, segment.PersonalBestSplitTime[method] - prevTime));
                if (segment.PersonalBestSplitTime[method].HasValue)
                    prevTime = segment.PersonalBestSplitTime[method].Value;
            }
        }


        public static void ImportBestSegment(this IRun run, int segmentIndex)
        {
            var segment = run[segmentIndex];
            if (segment.BestSegmentTime.RealTime.HasValue || segment.BestSegmentTime.GameTime.HasValue)
            {
                segment.SegmentHistory.Add(GetMinSegmentHistoryIndex(run) - 1, segment.BestSegmentTime);
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

        public static string GetExtendedCategoryName(this IRun run, bool showRegion = false, bool showPlatform = false, bool showVariables = true, bool waitForOnlineData = false)
        {
            var list = new List<string>();

            var categoryName = run.CategoryName;
            var afterParentheses = "";

            if (string.IsNullOrEmpty(categoryName))
                return string.Empty;

            var indexStart = categoryName.IndexOf('(');
            var indexEnd = categoryName.IndexOf(')', indexStart + 1);
            if (indexStart >= 0 && indexEnd >= 0)
            {
                var inside = categoryName.Substring(indexStart + 1, indexEnd - indexStart - 1);
                list.Add(inside);
                afterParentheses = categoryName.Substring(indexEnd + 1).Trim();
                categoryName = categoryName.Substring(0, indexStart).Trim();
            }

            if (showVariables)
            {
                IEnumerable<string> variables = run.Metadata.VariableValueNames.Keys;
                if ((run.Metadata.GameAvailable || waitForOnlineData) && run.Metadata.Game != null)
                    variables = run.Metadata.Game.FullGameVariables.Select(x => x.Name);

                foreach (var variable in variables)
                {
                    if (run.Metadata.VariableValueNames.ContainsKey(variable))
                    {
                        var name = variable.TrimEnd('?');
                        var variableValue = run.Metadata.VariableValueNames[variable];
                        var valueLower = variableValue.ToLowerInvariant();

                        if (valueLower == "yes")
                        {
                            list.Add(name);
                        }
                        else if (valueLower == "no")
                        {
                            list.Add($"No { name }");
                        }
                        else
                        {
                            list.Add(variableValue);
                        }
                    }
                }
            }

            if (showRegion)
            {
                var doSimpleRegion = !run.Metadata.RegionAvailable && !waitForOnlineData;
                if (doSimpleRegion)
                {
                    if (!string.IsNullOrEmpty(run.Metadata.RegionName))
                        list.Add(run.Metadata.RegionName);
                }
                else if (run.Metadata.Region != null && !string.IsNullOrEmpty(run.Metadata.Region.Abbreviation) && run.Metadata.Game != null && run.Metadata.Game.Regions.Count > 1)
                {
                    list.Add(run.Metadata.Region.Abbreviation);
                }
            }

            if (showPlatform)
            {
                var doSimplePlatform = !run.Metadata.PlatformAvailable && !waitForOnlineData;
                if (!string.IsNullOrEmpty(run.Metadata.PlatformName) && (doSimplePlatform || (run.Metadata.Game != null && run.Metadata.Game.Platforms.Count > 1)))
                {
                    if (run.Metadata.UsesEmulator)
                        list.Add($"{ run.Metadata.PlatformName } Emulator");
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
                categoryName = $"{ categoryName } ({ string.Join(", ", list) }) { afterParentheses }";
            }

            return categoryName.Trim();
        }
    }
}
