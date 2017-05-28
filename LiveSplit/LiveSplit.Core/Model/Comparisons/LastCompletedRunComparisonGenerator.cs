using LiveSplit.Options;
using System;
using System.Linq;

namespace LiveSplit.Model.Comparisons
{
    public class LastCompletedRunComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Last Completed Run";
        public const string ShortComparisonName = "Last Run";
        public string Name => ComparisonName;

        public LastCompletedRunComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(TimingMethod method)
        {
            Attempt? mostRecentCompleted = null;
            foreach (var attempt in Run.AttemptHistory.Reverse())
            {
                if (attempt.Time[method] != null)
                {
                    mostRecentCompleted = attempt;
                    break;
                }
            }

            TimeSpan? totalTime = TimeSpan.Zero;
            for (var ind = 0; ind < Run.Count; ind++)
            {
                TimeSpan? segmentTime;
                if (mostRecentCompleted != null)
                    segmentTime = Run[ind].SegmentHistory[mostRecentCompleted.Value.Index][method];
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
