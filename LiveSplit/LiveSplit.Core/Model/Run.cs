using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace LiveSplit.Model
{
    /// <summary>
    /// Describes a run for a game with all the splits and times.
    /// </summary>
    [Serializable]
    public class Run : IRun
    {
        /// <summary>
        /// The name of the comparison used to save your Personal Best splits.
        /// </summary>
        public const string PersonalBestComparisonName = "Personal Best";

        /// <summary>
        /// This is the internal list being used to save the segments, which the run is a facade to.
        /// </summary>
        protected IList<ISegment> InternalList { get; set; }

        /// <summary>
        /// Gets or sets the icon of the game the run is for.
        /// </summary>
        public Image GameIcon { get; set; }
        /// <summary>
        /// Gets or sets the name of the game the run is for.
        /// </summary>
        public string GameName { get; set; }
        /// <summary>
        /// Gets or sets the category of the run.
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Gets or sets the time where the timer starts at.
        /// <remarks>This can be both a negative time as well to simulate a countdown.</remarks>
        /// </summary>
        public TimeSpan Offset { get; set; }

        /// <summary>
        /// Gets or sets the amount of times the run has been started.
        /// </summary>
        public int AttemptCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<IIndexedTime> RunHistory { get; set; }

        public AutoSplitter AutoSplitter { get; set; }
        public XmlElement AutoSplitterSettings { get; set; }

        public IList<IComparisonGenerator> ComparisonGenerators { get; set; }
        public IList<string> CustomComparisons { get; set; }
        public IEnumerable<string> Comparisons { get { return CustomComparisons.Concat(ComparisonGenerators.Select(x => x.Name)); } }

        protected IComparisonGeneratorsFactory Factory { get; set; }

        public bool HasChanged { get; set; }
        public string FilePath { get; set; }

        public Run(IComparisonGeneratorsFactory factory)
        {
            InternalList = new List<ISegment>();
            RunHistory = new List<IIndexedTime>();
            Factory = factory;
            ComparisonGenerators = Factory.Create(this).ToList();
            CustomComparisons = new List<string>() { PersonalBestComparisonName };
        }

        public Run(IEnumerable<ISegment> collection, IComparisonGeneratorsFactory factory)
        {
            InternalList = new List<ISegment>();
            foreach (var x in collection)
            {
                InternalList.Add(x.Clone() as ISegment);
            }
            RunHistory = new List<IIndexedTime>();
            Factory = factory;
            ComparisonGenerators = Factory.Create(this).ToList();
            CustomComparisons = new List<string>() { PersonalBestComparisonName };
        }

        public int IndexOf(ISegment item)
        {
            return InternalList.IndexOf(item);
        }

        public void Insert(int index, ISegment item)
        {
            InternalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalList.RemoveAt(index);
        }

        public ISegment this[int index]
        {
            get
            {
                return InternalList[index];
            }
            set
            {
                InternalList[index] = value;
            }
        }

        public void Add(ISegment item)
        {
            InternalList.Add(item);
        }

        public void AddSegment(string name, Time pbSplitTime = default(Time), Time bestSegmentTime = default(Time), Image icon = null, Time splitTime = default(Time), IList<Time> segmentHistory = null)
        {
            Add(new Segment(name, pbSplitTime, bestSegmentTime, icon, splitTime));
        }

        public void Clear()
        {
            InternalList.Clear();
        }

        public bool Contains(ISegment item)
        {
            return InternalList.Contains(item);
        }

        public void CopyTo(ISegment[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return InternalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return InternalList.IsReadOnly; }
        }

        public bool Remove(ISegment item)
        {
            return InternalList.Remove(item);
        }

        public IEnumerator<ISegment> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        public object Clone()
        {
            return new Run(this, Factory)
            {
                GameIcon = GameIcon,
                GameName = GameName,
                CategoryName = CategoryName,
                Offset = Offset,
                AttemptCount = AttemptCount,
                RunHistory = new List<IIndexedTime>(RunHistory),
                HasChanged = HasChanged,
                FilePath = FilePath,
                CustomComparisons = new List<string>(CustomComparisons),
                ComparisonGenerators = new List<IComparisonGenerator>(ComparisonGenerators),
                AutoSplitter = AutoSplitter
            };
        }

        public void FixSplits()
        {
            FixWithMethod(TimingMethod.RealTime);
            FixWithMethod(TimingMethod.GameTime);
        }

        public void FixWithMethod(TimingMethod method)
        {
            FixSegmentHistory(method);
            FixComparisonTimes(method);
            RemoveNullValues(method);
        }

        protected void FixSegmentHistory(TimingMethod method)
        {
            foreach (var curSplit in InternalList)
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

        protected void FixComparisonTimes(TimingMethod method)
        {
            foreach (var comparison in CustomComparisons)
            {
                var previousTime = TimeSpan.Zero;
                foreach (var curSplit in InternalList)
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
                        if (comparison == PersonalBestComparisonName)
                        {
                            var newTime = new Time(curSplit.BestSegmentTime);
                            //Fix best segments
                            if (curSplit.BestSegmentTime[method] == null)
                                newTime[method] = currentSegment;
                            if (curSplit.BestSegmentTime[method] > currentSegment)
                                newTime[method] = currentSegment;
                            curSplit.BestSegmentTime = newTime;
                        }
                        previousTime = curSplit.Comparisons[comparison][method].Value;
                    }
                }
            }
        }

        protected void RemoveNullValues(TimingMethod method)
        {
            var cache = new List<IIndexedTime>();
            for (var runIndex = GetMinSegmentHistoryIndex(); runIndex <= RunHistory.Count; runIndex++)
            {
                for (var index = 0; index < Count; index++)
                {
                    var segmentHistoryElement = this[index].SegmentHistory.FirstOrDefault(x => x.Index == runIndex);
                    if (segmentHistoryElement == null)
                    {
                        RemoveItemsFromCache(index, cache);
                    }
                    else if (segmentHistoryElement.Time.RealTime == null && segmentHistoryElement.Time.GameTime == null)
                        cache.Add(segmentHistoryElement);
                    else
                        cache.Clear();
                }
                RemoveItemsFromCache(Count, cache);
                cache.Clear();
            }
        }

        protected void RemoveItemsFromCache(int index, IList<IIndexedTime> cache)
        {
            var ind = index - cache.Count;
            foreach (var item in cache)
            {
                this[ind].SegmentHistory.Remove(item);
                ind++;
            }
            cache.Clear();
        }

        public int GetMinSegmentHistoryIndex()
        {
            var minIndex = 1;
            foreach (var segment in InternalList)
            {
                foreach (var history in segment.SegmentHistory)
                {
                    if (history.Index < minIndex)
                        minIndex = history.Index;
                }
            }
            return minIndex-1;
        }

        public void ImportSegmentHistory()
        {
            var prevTimeRTA = TimeSpan.Zero;
            var prevTimeGameTime = TimeSpan.Zero;
            var index = GetMinSegmentHistoryIndex();
            var nullValue = false;

            foreach (var segment in InternalList)
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


        public void ImportBestSegment(int segmentIndex)
        {
            var segment = InternalList[segmentIndex];
            var newTime = new Time();
            if (segment.BestSegmentTime[TimingMethod.RealTime] != null || segment.BestSegmentTime[TimingMethod.GameTime] != null)
            {
                newTime[TimingMethod.RealTime] = segment.BestSegmentTime[TimingMethod.RealTime];
                newTime[TimingMethod.GameTime] = segment.BestSegmentTime[TimingMethod.GameTime];
                segment.SegmentHistory.Add(new IndexedTime(newTime, GetMinSegmentHistoryIndex()));
            }
        }
    }
}
