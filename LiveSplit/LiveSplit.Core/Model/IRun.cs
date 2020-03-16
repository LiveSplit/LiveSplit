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
            ReattachUnattachedSegmentHistoryElements(run);
        }

        private static void FixWithMethod(IRun run, TimingMethod method)
        {
            FixComparisonTimesAndHistory(run, method);
            RemoveDuplicates(run, method);
        }

        private static void ReattachUnattachedSegmentHistoryElements(IRun run)
        {
            int max_id = run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty().Max();
            int min_id = run.GetMinSegmentHistoryIndex();

            int unattached_id;
            // can't use `default` keyword because repo isn't on C# 7.1 yet so we use 0 instead
            while ((unattached_id = run
                .Select(seg => seg.SegmentHistory.Select(x => x.Key).DefaultIfEmpty().Max())
                .Where(i => i > max_id)
                .DefaultIfEmpty()
                .Max()) > 0)
            {
                var reassign_id = min_id - 1;

                foreach (Segment segment in run)
                {
                    if (segment.SegmentHistory.TryGetValue(unattached_id, out Time time))
                    {
                        segment.SegmentHistory.Add(reassign_id, time);
                        segment.SegmentHistory.Remove(unattached_id);
                    }
                }

                min_id = reassign_id;
            }
        }

        private static void FixHistoryFromNullBestSegments(ISegment curSplit, TimingMethod method, int minIndex, int maxIndex)
        {
            if (curSplit.BestSegmentTime[method] == null)
            {
                for (var runIndex = minIndex; runIndex <= maxIndex; runIndex++)
                {
                    Time historyTime;
                    if (curSplit.SegmentHistory.TryGetValue(runIndex, out historyTime))
                    {
                        //If the Best Segment is gone, clear the history
                        if (historyTime[method] != null)
                            curSplit.SegmentHistory.Remove(runIndex);
                    }
                }
            }
        }

        private static void FixHistoryFromBestSegmentTimes(ISegment curSplit, TimingMethod method, int minIndex, int maxIndex)
        {
            if (curSplit.BestSegmentTime[method] != null)
            {
                for (var runIndex = minIndex; runIndex <= maxIndex; runIndex++)
                {
                    Time historyTime;
                    if (curSplit.SegmentHistory.TryGetValue(runIndex, out historyTime))
                    {
                        //Make sure no times in the history are lower than the Best Segment
                        if (historyTime[method] < curSplit.BestSegmentTime[method])
                        {
                            historyTime[method] = curSplit.BestSegmentTime[method];
                            curSplit.SegmentHistory[runIndex] = historyTime;
                        }
                    }
                }
            }
        }

        private static void FixComparisonTimesAndHistory(IRun run, TimingMethod method)
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

            var maxIndex = run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();

            foreach (var curSplit in run)
            {
                var minIndex = curSplit.SegmentHistory.GetMinIndex();
                FixHistoryFromNullBestSegments(curSplit, method, minIndex, maxIndex);
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
                        if (comparison == Run.PersonalBestComparisonName)
                        {
                            var currentSegment = curSplit.Comparisons[comparison][method] - previousTime;
                            if (curSplit.BestSegmentTime[method] == null || curSplit.BestSegmentTime[method] > currentSegment)
                            {
                                var newTime = curSplit.BestSegmentTime;
                                newTime[method] = currentSegment;
                                curSplit.BestSegmentTime = newTime;
                            }
                        }

                        previousTime = curSplit.Comparisons[comparison][method].Value;
                    }
                }
            }

            foreach (var curSplit in run)
            {
                var minIndex = curSplit.SegmentHistory.GetMinIndex();
                FixHistoryFromBestSegmentTimes(curSplit, method, minIndex, maxIndex);
            }
        }

        private static void RemoveNullValues(IRun run)
        {
            var cache = new List<int>();
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
                        cache.Add(runIndex);
                    else
                        cache.Clear();
                }
                RemoveItemsFromCache(run, run.Count, cache);
            }
        }

        private static void RemoveDuplicates(IRun run, TimingMethod method)
        {
            var rtaSet = new HashSet<TimeSpan>();
            var igtSet = new HashSet<TimeSpan>();
            foreach (var segment in run)
            {
                rtaSet.Clear();
                igtSet.Clear();

                foreach (var attempt in run.AttemptHistory)
                {
                    var ind = attempt.Index;
                    Time element;
                    if (segment.SegmentHistory.TryGetValue(ind, out element))
                    {
                        if (element.RealTime != null)
                            rtaSet.Add(element.RealTime.Value);
                        if (element.GameTime != null)
                            igtSet.Add(element.GameTime.Value);
                    }
                }

                for (var runIndex = segment.SegmentHistory.GetMinIndex(); runIndex <= 0; runIndex++)
                {
                    Time element;
                    if (segment.SegmentHistory.TryGetValue(runIndex, out element))
                    {
                        var isNull = true;
                        var isUnique = false;
                        if (element.RealTime != null)
                        {
                            isUnique |= rtaSet.Add(element.RealTime.Value);
                            isNull = false;
                        }
                        if (element.GameTime != null)
                        {
                            isUnique |= igtSet.Add(element.GameTime.Value);
                            isNull = false;
                        }

                        if (!isUnique && !isNull)
                            segment.SegmentHistory.Remove(runIndex);
                    }
                }
            }
        }

        private static void RemoveItemsFromCache(IRun run, int index, IList<int> cache)
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
                {
                    string categoryId = null;
                    if ((run.Metadata.CategoryAvailable || waitForOnlineData) && run.Metadata.Category != null)
                        categoryId = run.Metadata.Category.ID;
                    variables = run.Metadata.Game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId).Select(x => x.Name);
                }

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
                var doSimpleRegion = !run.Metadata.GameAvailable && !waitForOnlineData;
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
                var doSimplePlatform = !run.Metadata.GameAvailable && !waitForOnlineData;
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
