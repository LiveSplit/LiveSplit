using LiveSplit.Model.Comparisons;
using LiveSplit.Web;
using System;
using System.Collections.Generic;
using System.IO;

namespace LiveSplit.Model.RunFactories
{
    public class UrnRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public UrnRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        private static Time parseTime(string time)
        {
            var parsedTime = new Time();

            parsedTime.RealTime = TimeSpanParser.ParseNullable(time);

            //Null is stored as a zero time in Urn Splits
            if (parsedTime.RealTime == TimeSpan.Zero)
                parsedTime.RealTime = null;

            return parsedTime;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var json = JSON.FromStream(Stream);

            run.CategoryName = json.title as string;
            run.AttemptCount = json.attempt_count;
            run.Offset = TimeSpanParser.Parse(json.start_delay as string);

            //Best Split Times can be used for the Segment History
            //Every single best split time should be included as its own run, 
            //since the best split times could be apart from each other less 
            //than the best segments, so we have to assume they are from different runs.
            var attemptHistoryIndex = 1;

            var segments = json.splits as IEnumerable<dynamic>;

            foreach (var segment in segments)
            {
                var segmentName = segment.title as string;
                var pbSplitTime = parseTime(segment.time as string);
                var bestSegment = parseTime(segment.best_segment as string);

                var parsedSegment = new Segment(segmentName, pbSplitTime, bestSegment);

                var bestSplitTime = parseTime(segment.best_time as string);
                if (bestSplitTime.RealTime != null)
                {
                    run.AttemptHistory.Add(new Attempt(attemptHistoryIndex, default(Time), null, null, null));

                    //Insert a new run that skips to the current split
                    foreach (var alreadyInsertedSegment in run)
                    {
                        alreadyInsertedSegment.SegmentHistory.Add(attemptHistoryIndex, default(Time));
                    }

                    parsedSegment.SegmentHistory.Add(attemptHistoryIndex, bestSplitTime);

                    attemptHistoryIndex++;
                }

                run.Add(parsedSegment);
            }

            return run;
        }
    }
}
