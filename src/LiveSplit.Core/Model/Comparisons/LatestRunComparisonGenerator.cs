using System;
using System.Linq;

using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons;

public class LatestRunComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Latest Run";
    public const string ShortComparisonName = "Latest";
    public string Name => ComparisonName;

    public LatestRunComparisonGenerator(IRun run)
    {
        Run = run;
    }

    private int GetAttemptIndex()
    {
        int maxIndex = Run.AttemptHistory.LastOrDefault().Index;
        for (int segmentIndex = Run.Count - 1; segmentIndex >= 0; segmentIndex--)
        {
            ISegment segment = Run[segmentIndex];
            for (int attemptIndex = maxIndex; attemptIndex >= 1; attemptIndex--)
            {
                if (segment.SegmentHistory.ContainsKey(attemptIndex))
                {
                    return attemptIndex;
                }
            }
        }

        return 0;
    }

    public void Generate(TimingMethod method)
    {
        int attemptIndex = GetAttemptIndex();

        if (attemptIndex > 0)
        {
            TimeSpan? totalTime = TimeSpan.Zero;
            for (int ind = 0; ind < Run.Count; ind++)
            {
                TimeSpan? segmentTime = null;
                if (Run[ind].SegmentHistory.ContainsKey(attemptIndex))
                {
                    segmentTime = Run[ind].SegmentHistory[attemptIndex][method];
                }
                else
                {
                    totalTime = null;
                }

                var time = new Time(Run[ind].Comparisons[Name]);
                if (totalTime != null && segmentTime != null)
                {
                    totalTime += segmentTime;
                    time[method] = totalTime;
                }
                else
                {
                    time[method] = null;
                }

                Run[ind].Comparisons[Name] = time;
            }
        }
    }

    public void Generate(ISettings settings)
    {
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
