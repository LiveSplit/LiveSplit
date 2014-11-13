using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model.RunFactories
{
    public class WSplitRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public WSplitRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            var iconsList = new List<Image>();

            var reader = new StreamReader(Stream);

            var oldRunExists = false;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    if (line.StartsWith("Title="))
                    {
                        run.CategoryName = line.Substring("Title=".Length);
                    }
                    else if (line.StartsWith("Attempts="))
                    {
                        run.AttemptCount = Int32.Parse(line.Substring("Attempts=".Length));
                    }
                    else if (line.StartsWith("Offset="))
                    {
                        run.Offset = new TimeSpan(0, 0, 0, 0, -Int32.Parse(line.Substring("Offset=".Length)));
                    }
                    else if (line.StartsWith("Size="))
                    {
                        //Ignore
                    }
                    else if (line.StartsWith("Icons="))
                    {
                        var iconsString = line.Substring("Icons=".Length);
                        iconsList.Clear();
                        foreach (var iconPath in iconsString.Split(','))
                        {
                            var realIconPath = iconPath.Substring(1, iconPath.Length - 2);
                            Image icon = null;
                            if (realIconPath.Length > 0) 
                            {
                                try
                                {
                                    icon = Image.FromFile(realIconPath);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }
                            }
                            iconsList.Add(icon);
                        }
                    }
                    else //must be a split Kappa
                    {
                        var splitInfo = line.Split(',');
                        Time pbSplitTime = new Time();
                        Time goldTime = new Time();
                        Time oldRunTime = new Time();
                        pbSplitTime.RealTime = TimeSpan.FromSeconds(Convert.ToDouble(splitInfo[2], CultureInfo.InvariantCulture.NumberFormat));
                        goldTime.RealTime = TimeSpan.FromSeconds(Convert.ToDouble(splitInfo[3], CultureInfo.InvariantCulture.NumberFormat));
                        oldRunTime.RealTime = TimeSpan.FromSeconds(Convert.ToDouble(splitInfo[1], CultureInfo.InvariantCulture.NumberFormat));

                        if (pbSplitTime.RealTime == TimeSpan.Zero)
                            pbSplitTime.RealTime = null;

                        if (goldTime.RealTime == TimeSpan.Zero)
                            goldTime.RealTime = null;

                        if (oldRunTime.RealTime == TimeSpan.Zero)
                            oldRunTime.RealTime = null;
                        else
                            oldRunExists = true;
                            
                        run.AddSegment(splitInfo[0], pbSplitTime, goldTime);
                        run.Last().Comparisons["Old Run"] = oldRunTime;
                    }
                }
            }

            if (oldRunExists)
                run.CustomComparisons.Add("Old Run");


            for (var i = 0; i < iconsList.Count; ++i)
            {
                run[i].Icon = iconsList[i];
            }

            return run;
        }
    }
}
