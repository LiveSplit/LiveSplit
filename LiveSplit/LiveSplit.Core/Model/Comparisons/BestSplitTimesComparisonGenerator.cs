using LiveSplit.Options;
using System;
using System.Linq;

namespace LiveSplit.Model.Comparisons
{
    public class BestSplitTimesComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Best Split Times";
        public const string ShortComparisonName = "Best Pace";
        public string Name => ComparisonName;

        public BestSplitTimesComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(TimingMethod method)
        {
            var maxIndex = Run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var y = Run.GetMinSegmentHistoryIndex(); y <= maxIndex; y++)
            {
                var time = TimeSpan.Zero;

                foreach (var segment in Run)
                {
                    Time segmentHistoryElement;
                    if (segment.SegmentHistory.TryGetValue(y, out segmentHistoryElement))
                    {
                        var segmentTime = segmentHistoryElement[method];
                        if (segmentTime != null)
                        {
                            time += segmentTime.Value;

                            if (segment.Comparisons[Name][method] == null || time < segment.Comparisons[Name][method])
                            {
                                var newTime = new Time(segment.Comparisons[Name]);
                                newTime[method] = time;
                                segment.Comparisons[Name] = newTime;
                            }
                        }
                    }
                    else break;
                }
            }
        }

        public void Generate(ISettings settings)
        {
            foreach (var segment in Run)
            {
                if (Run.IndexOf(segment) > 0)
                    segment.Comparisons[Name] = segment.Comparisons[Model.Run.PersonalBestComparisonName];
                else
                    segment.Comparisons[Name] = segment.BestSegmentTime;
            }
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }
    }
}
