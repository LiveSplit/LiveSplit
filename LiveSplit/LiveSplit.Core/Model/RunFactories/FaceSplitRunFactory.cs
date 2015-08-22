using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model.RunFactories
{
    public class FaceSplitRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public FaceSplitRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        private static Time parseTime(string time)
        {
            var parsedTime = new Time();

            //Replace "," by "." as "," wouldn't parse in most cultures 
            parsedTime.RealTime = TimeSpanParser.ParseNullable(time.Replace(',', '.'));

            //Null is stored as a zero time in FaceSplit Splits
            if (parsedTime.RealTime == TimeSpan.Zero)
                parsedTime.RealTime = null;

            return parsedTime;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var reader = new StreamReader(Stream);

            var title = reader.ReadLine();
            run.CategoryName = title;

            var goal = reader.ReadLine();
            //TODO Store goal

            var attemptCount = reader.ReadLine();
            run.AttemptCount = Convert.ToInt32(attemptCount);

            var runsCompleted = reader.ReadLine();
            //TODO Store this somehow

            string segmentLine;
            while ((segmentLine = reader.ReadLine()) != null)
            {
                //Parse the Segment Line from right to left, as dashes in the 
                //title are not escaped. Therefore we can't just split it by
                //the dashes. FaceSplit itself does that, but LiveSplit fixes 
                //that bug.
                var index = segmentLine.LastIndexOf('-');
                var bestSegment = segmentLine.Substring(index + 1);
                var bestSegmentTime = parseTime(bestSegment);

                segmentLine = segmentLine.Substring(0, index);
                index = segmentLine.LastIndexOf('-');
                //Ignore Segment Time

                segmentLine = segmentLine.Substring(0, index);
                index = segmentLine.LastIndexOf('-');
                var splitTimeString = segmentLine.Substring(index + 1);
                var splitTime = parseTime(splitTimeString);

                var segmentName = segmentLine.Substring(0, index);

                run.AddSegment(segmentName, splitTime, bestSegmentTime);
            }

            return run;
        }
    }
}
