using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using System;
using System.Drawing;
using System.IO;
using static System.TimeSpan;

namespace LiveSplit.Model.RunFactories
{
    public class SplitterZRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public SplitterZRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        protected string Unescape(string text) => text.Replace(@"‡", @",");

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var reader = new StreamReader(Stream);
            
            var line = reader.ReadLine();
            var titleInfo = line.Split(',');
            //Title Stuff here, do later
            run.CategoryName = Unescape(titleInfo[0]);
            run.AttemptCount = int.Parse(titleInfo[1]);
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    var splitInfo = line.Split(',');
                    Time pbSplitTime = new Time();
                    Time goldTime = new Time();

                    try
                    {
                        pbSplitTime.RealTime = Parse(splitInfo[1]);
                    }
                    catch
                    {
                        try
                        {
                            pbSplitTime.RealTime = Parse("0:" + splitInfo[1]);
                        }
                        catch
                        {
                            pbSplitTime.RealTime = Parse("0:0:" + splitInfo[1]);
                        }
                    }

                    try
                    {
                        goldTime.RealTime = Parse(splitInfo[2]);
                    }
                    catch 
                    {
                        try
                        {
                            goldTime.RealTime = Parse("0:" + splitInfo[2]);
                        }
                        catch
                        {
                            goldTime.RealTime = Parse("0:0:" + splitInfo[2]);
                        }
                    }

                    if (pbSplitTime.RealTime == Zero)
                        pbSplitTime.RealTime = null;

                    if (goldTime.RealTime == Zero)
                        goldTime.RealTime = null;

                    var realIconPath = "";
                    if (splitInfo.Length > 3)
                        realIconPath = Unescape(splitInfo[3]);
                    Image icon = null;
                    if (realIconPath.Length > 0)
                    {
                        try
                        {
                            using (var stream = File.OpenRead(realIconPath))
                            {
                                icon = Image.FromStream(stream);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                    run.AddSegment(Unescape(splitInfo[0]), pbSplitTime, goldTime, icon);
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
