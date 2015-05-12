using LiveSplit.Options;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public class WeightedComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const string ComparisonName = "Balanced PB";
        public const string ShortComparisonName = "Balanced";
        public string Name { get { return ComparisonName; } }

        public WeightedComparisonGenerator(IRun run)
        {
            Run = run;
        }


        public void Generate(TimingMethod method)
        {
            int i, n;
            int RunCount = Run.Count - 1;
            List<List<int>> SplitArrayList = new List<List<int>>();

            int [,] TempArray1 = new int[RunCount + 1, Run.RunHistory.Count];
            for (i = 0; i <= RunCount; i++)
            {
                for (n = 0; n <= Run[i].SegmentHistory.Count - 1; n++)
                {
                    var Segment = Run[i].SegmentHistory[n];
                    if (Segment.Time[method].HasValue && Segment.Index > 0)
                    {
                        TempArray1[i, Segment.Index - 1] = Convert.ToInt32(Segment.Time[method].Value.TotalMilliseconds);
                    }

                }
                List<int> TempArray2 = new List<int>();
                for (n = 0; n <= Run.RunHistory.Count - 1; n++)
                {
                    if (i > 0)
                    {
                        if (TempArray1[i, n] > 0 && TempArray1[i - 1, n] > 0)
                        {
                            TempArray2.Add(TempArray1[i, n]);
                        }
                    }
                    else if (TempArray1[i, n] > 0)
                    {
                        TempArray2.Add(TempArray1[i, n]);
                    }

                }
                TempArray2.Sort();
                SplitArrayList.Add(TempArray2);
            }

            int Goaltime = 0;
            if (Run[RunCount].PersonalBestSplitTime[method] != null)
            {
                Goaltime = Convert.ToInt32(Run[RunCount].PersonalBestSplitTime[method].Value.TotalMilliseconds);
                int RunSum, PercPosiUp, PercPosiDn;
                double Percentile, PercPosi, PercMin, PercMax;
                List<int> OutputSplits = new List<int>();
                PercMin = 0.0;
                PercMax = 1.0;

                do
                {
                    RunSum = 0;
                    Percentile = 0.5 * (PercMax - PercMin) + PercMin;
                    for (i = 0; i <= RunCount; i++)
                    {
                        List<int> Splitception = SplitArrayList[i];
                        PercPosi = Percentile * (Splitception.Count - 1);
                        PercPosiUp = Convert.ToInt32(Math.Ceiling(PercPosi));
                        PercPosiDn = Convert.ToInt32(Math.Floor(PercPosi));
                        int SegTime;
                        if (PercPosiUp > PercPosiDn)
                        {
                            SegTime = Convert.ToInt32(Splitception[PercPosiUp] * (PercPosi - PercPosiDn) + Splitception[PercPosiDn] * (PercPosiUp - PercPosi));
                        }
                        else SegTime = Splitception[PercPosiUp];
                        OutputSplits.Add(SegTime);
                        RunSum += SegTime;
                    }

                    if (RunSum == Goaltime || Percentile < 0.0000000001 || PercMax == PercMin)
                    {
                        break;
                    }
                    else if (RunSum > Goaltime)
                    {
                        PercMax = Percentile;
                    }
                    else PercMin = Percentile;
                    OutputSplits.Clear();
                } while (true);

                TimeSpan? totalTime = TimeSpan.Zero;
                for (var ind = 0; ind < OutputSplits.Count; ind++)
                {
                    if (OutputSplits.Count == 0)
                        totalTime = null;
                    if (totalTime != null)
                        totalTime += TimeSpan.FromMilliseconds(OutputSplits[ind]);
                    var time = new Time(Run[ind].Comparisons[Name]);
                    time[method] = totalTime;
                    Run[ind].Comparisons[Name] = time;
                }
            }
            else
            {
                TimeSpan? totalTime = null;
            }
        }

        public void Generate(ISettings settings)
        {
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }

    }
}
