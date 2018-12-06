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

            var subsegmentList = new List<ISegment>();
            for (int segmentIndex = 0; segmentIndex < Run.Count; ++segmentIndex)
            {
                var segment = Run[segmentIndex];
                subsegmentList.Add(segment);

                if ((segment.Name.Length == 0) || (segment.Name[0] != '-'))
                {
                    mains.Add(segment);
                    mainIndices.Add(segment, segmentIndex);
                    subsegments.Add(segment, subsegmentList);
                    subsegmentList = new List<ISegment>();
                }
            }

            var attemptIndices = from attempt in Run.AttemptHistory
                                 select attempt.Index;

            // TODO: record all subsegments of the best main for the comparison
            foreach (var attemptIndex in attemptIndices)
            {
                //foreach (var mainSegment in mains)
                for (int ind = 0; ind < mains.Count; ++ind)
                {
                    var mainSegment = mains[ind];
                    if (mainSegment.SegmentHistory.ContainsKey(attemptIndex))
                    {
                        var mainTime = TimeSpan.Zero;
                        foreach (var subsegment in subsegments[mainSegment])
                        {
                            mainTime += subsegment.SegmentHistory[attemptIndex][method] ?? TimeSpan.Zero;
                        }

                        TimeSpan? comparisonTime = Run[mainIndices[mainSegment]].Comparisons[Name][method];
                        if (ind > 0)
                            comparisonTime -= Run[mainIndices[mains[ind-1]]].Comparisons[Name][method];

                        if ((comparisonTime == null) || (mainTime < comparisonTime))
                        {

                        }
                    }
                }
            }
        }
    }
}
