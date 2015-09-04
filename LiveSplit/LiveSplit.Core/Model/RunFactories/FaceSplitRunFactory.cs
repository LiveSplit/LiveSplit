using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;

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
                var splitted = segmentLine.Split(new[] { '-' }, 5);

                var segmentName = (splitted.Length >= 1) ? splitted[0].Replace("\"?\"", "-") : string.Empty;

                var splitTimeString = (splitted.Length >= 2) ? splitted[1] : string.Empty;
                var splitTime = parseTime(splitTimeString);

                var bestSegment = (splitted.Length >= 4) ? splitted[3] : string.Empty;
                var bestSegmentTime = parseTime(bestSegment);

                var iconPath = (splitted.Length >= 5) ? splitted[4] : string.Empty;
                Image icon = null;
                if (!string.IsNullOrWhiteSpace(iconPath))
                {
                    try
                    {
                        icon = Image.FromFile(iconPath);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                run.AddSegment(segmentName, splitTime, bestSegmentTime, icon);
            }

            return run;
        }
    }
}
