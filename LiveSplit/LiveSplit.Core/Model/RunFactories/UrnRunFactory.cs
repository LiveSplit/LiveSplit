using LiveSplit.Model.Comparisons;
using LiveSplit.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.RunFactories
{
    public class UrnRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public UrnRunFactory(Stream stream = null)
        {
            Stream = stream;
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
            var runHistoryIndex = 0;

            var segments = json.splits as IEnumerable<dynamic>;

            foreach (var segment in segments)
            {
                var segmentName = segment.title as string;
                
                var pbSplitTime = new Time();
                pbSplitTime.RealTime = TimeSpanParser.ParseNullable(segment.time as string);

                var bestSegment = new Time();
                bestSegment.RealTime = TimeSpanParser.ParseNullable(segment.best_segment as string);

                var parsedSegment = new Segment(segmentName, pbSplitTime, bestSegment);

                var bestSplitTime = TimeSpanParser.ParseNullable(segment.best_time as string);
                if (bestSplitTime != null)
                {
                    --runHistoryIndex;
                    run.RunHistory.Add(new IndexedTime(default(Time), runHistoryIndex));

                    //Insert a new run that skips to the current split
                    foreach (var alreadyInsertedSegment in run)
                    {
                        alreadyInsertedSegment.SegmentHistory.Add(new IndexedTime(default(Time), runHistoryIndex));
                    }

                    parsedSegment.SegmentHistory.Add(new IndexedTime(new Time(bestSplitTime), runHistoryIndex));
                }

                run.Add(parsedSegment);
            }

            return run;
        }
    }
}
