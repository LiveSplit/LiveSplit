using LiveSplit.Options;
using System;
using System.Linq;

namespace LiveSplit.Model.Comparisons
{
    public class LastCompletedRunComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Last Completed Run";
        public const string ShortComparisonName = "Last Completed Run";
        public string Name => ComparisonName;

        public LastCompletedRunComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(TimingMethod method)
        {
            Attempt? mostRecentCompleted = null;
            foreach (var attempt in Run.AttemptHistory)
            {
                if ( 
                    attempt.Time[method] != null &&
                    ( mostRecentCompleted == null || ( mostRecentCompleted.Value.Ended - attempt.Ended ).Value.Seconds < 0 )
                    )
                {
                    mostRecentCompleted = attempt;
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
