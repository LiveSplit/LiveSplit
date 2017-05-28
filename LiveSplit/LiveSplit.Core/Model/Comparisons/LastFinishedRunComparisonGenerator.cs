using LiveSplit.Options;
using System;
using System.Linq;

namespace LiveSplit.Model.Comparisons
{
    public class LastFinishedRunComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Last Finished Run";
        public const string ShortComparisonName = "Last Run";
        public string Name => ComparisonName;

        public LastFinishedRunComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(TimingMethod method)
        {
            Attempt? mostRecentFinished = null;
            foreach (var attempt in Run.AttemptHistory.Reverse())
            {
                if (attempt.Time[method] != null)
                {
                    mostRecentFinished = attempt;
                    break;
                }
            }

            TimeSpan? totalTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                TimeSpan? segmentTime;
                if (mostRecentFinished != null)
                    segmentTime = Run[ind].SegmentHistory[mostRecentFinished.Value.Index][method];
                else
                    segmentTime = null;
                var time = new Time(Run[ind].Comparisons[Name]);
                if (totalTime != null && segmentTime != null)
                {
                    totalTime += segmentTime;
                    time[method] = totalTime;
                }
                else
                    time[method] = null;
                Run[ind].Comparisons[Name] = time;
            }
        }

        public void Generate(ISettings settings)
        {
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }
    }
}
