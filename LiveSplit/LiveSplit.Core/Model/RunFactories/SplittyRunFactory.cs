using LiveSplit.Model.Comparisons;
using LiveSplit.Web;
using System;
using System.Collections.Generic;
using System.IO;

namespace LiveSplit.Model.RunFactories
{
    public class SplittyRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public SplittyRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        private static Time parseTime(int? time, TimingMethod timingMethod)
        {
            var parsedTime = new Time();

            if (time.HasValue)
                parsedTime[timingMethod] = TimeSpan.FromMilliseconds(time.Value);

            return parsedTime;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var json = JSON.FromStream(Stream);

            run.GameName = json.run_name as string;
            run.AttemptCount = json.run_count;

            var timingMethod = (int)(json.timer_type) == 0 
                ? TimingMethod.RealTime 
                : TimingMethod.GameTime;

            var segments = json.splits as IEnumerable<dynamic>;

            foreach (var segment in segments)
            {
                var segmentName = segment.name as string;
                var pbSplitTime = parseTime((int?)segment.pb_split, timingMethod);
                var bestSegment = parseTime((int?)segment.split_best, timingMethod);

                var parsedSegment = new Segment(segmentName, pbSplitTime, bestSegment);
                run.Add(parsedSegment);
            }

            return run;
        }
    }
}
