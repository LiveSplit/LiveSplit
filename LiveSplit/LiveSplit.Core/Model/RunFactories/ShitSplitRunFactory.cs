using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.RunFactories
{
    public class ShitSplitRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public ShitSplitRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var reader = new StreamReader(Stream);
            
            var line = reader.ReadLine();
            var titleInfo = line.Split('|');
            run.CategoryName = titleInfo[0].Substring(1);
            run.AttemptCount = Int32.Parse(titleInfo[1]);
            TimeSpan totalTime = TimeSpan.Zero;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    var majorSplitInfo = line.Split('|');
                    totalTime += TimeSpanParser.Parse(majorSplitInfo[1]);
                    while (!reader.EndOfStream && reader.Read() == (int)('*'))
                    {
                        line = reader.ReadLine();
                        run.AddSegment(line);
                    }
                    var newTime = new Time(run.Last().PersonalBestSplitTime);
                    newTime.GameTime = totalTime;
                    run.Last().PersonalBestSplitTime = newTime;
                }
                else
                {
                    break;
                }
            }

            return run;
        }
    }
}
