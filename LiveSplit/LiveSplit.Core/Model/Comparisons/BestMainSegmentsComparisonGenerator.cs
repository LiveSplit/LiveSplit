using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons
{
    public class BestMainSegmentsComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Best Main Segments";
        public const string ShortComparisonName = "Mains";
        public string Name => ComparisonName;

        public BestMainSegmentsComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(ISettings settings)
        {
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }

        public void Generate(TimingMethod method)
        {
            var mains = new List<ISegment>();
            var mainIndices = new Dictionary<ISegment, int>();
            var subsegments = new Dictionary<ISegment, IList<ISegment>>();
            var subsegmentTimes = new Dictionary<ISegment, TimeSpan?>();
            var bestMainTimes = new Dictionary<ISegment, TimeSpan?>();

            var subsegmentList = new List<ISegment>();
            for (int segmentIndex = 0; segmentIndex < Run.Count; ++segmentIndex)
            {
                var segment = Run[segmentIndex];
                subsegmentTimes.Add(segment, null);
                subsegmentList.Add(segment);

                if ((segment.Name.Length == 0) || (segment.Name[0] != '-'))
                {
                    mains.Add(segment);
                    mainIndices.Add(segment, segmentIndex);
                    subsegments.Add(segment, subsegmentList);
                    bestMainTimes.Add(segment, null);

                    subsegmentList = new List<ISegment>();
                }
            }

            var attemptIndices = from attempt in Run.AttemptHistory
                                 select attempt.Index;

            foreach (var attemptIndex in attemptIndices)
            {
                for (int ind = 0; (ind < mains.Count) && (mains[ind].SegmentHistory.ContainsKey(attemptIndex)); ++ind)
                {
                    var mainSegment = mains[ind];
                    if (mainSegment.SegmentHistory[attemptIndex][method] != null)
                    {
                        var currMainTime = TimeSpan.Zero;
                        var currSubsegments = new Dictionary<ISegment, TimeSpan?>();
                        foreach (var subsegment in subsegments[mainSegment])
                        {
                            var subsegmentTime = subsegment.SegmentHistory[attemptIndex][method];
                            currSubsegments.Add(subsegment, subsegmentTime);
                            currMainTime += subsegmentTime ?? TimeSpan.Zero;
                        }

                        if (currMainTime < (bestMainTimes[mainSegment] ?? TimeSpan.MaxValue))
                        {
                            bestMainTimes[mainSegment] = currMainTime;
                            foreach (var subsegment in subsegments[mainSegment])
                            {
                                subsegmentTimes[subsegment] = currSubsegments[subsegment];
                            }
                        }
                    }
                }
            }

            var totalTime = TimeSpan.Zero;
            foreach (var segment in Run)
            {
                if (subsegmentTimes[segment] != null)
                {
                    totalTime += (TimeSpan) subsegmentTimes[segment];
                    var time = new Time(segment.Comparisons[Name]);
                    time[method] = totalTime;
                    segment.Comparisons[Name] = time;
                }
            }
        }
    }
}
