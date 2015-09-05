using System.Diagnostics;
using System.Drawing;
using System;

namespace LiveSplit.UI
{
    static class RainbowHelper
    {
        private static readonly double numColors = 50;
        private static readonly int updateInterval = 10; // in milliseconds

        private static Color[] rainbowTable;
        private static Stopwatch timer;

        static RainbowHelper()
        {
            GenerateRainbowTable();
            timer = new Stopwatch();
        }

        private static void GenerateRainbowTable()
        {
            rainbowTable = new Color[(int)numColors];

            double r, g, b;

            // color generator
            for (int i = 0; i < numColors; i++)
            { 
                var frequency = 5/numColors;
                r = Math.Sin(frequency * i + 0) * (127) + 128;
                g = Math.Sin(frequency * i + 1) * (127) + 128;
                b = Math.Sin(frequency * i + 3) * (127) + 128;
                rainbowTable[i] = Color.FromArgb((int)Math.Floor(r), (int)Math.Floor(g), (int)Math.Floor(b));
            }
        }

        public static Tuple<Color, Color> GetRainbowColors()
        {
            // start stopwatch if it hasn't been started
            if (!timer.IsRunning)
                timer.Start();

            Tuple<Color, Color> t = Tuple.Create(rainbowTable[i1], rainbowTable[i2]);
            
            // update on interval
            if (timer.ElapsedMilliseconds >= updateInterval) { 
                i1 += di1;
                i2 += di2;
                timer.Restart();
            }

            // interate backwards through rainbow table
            if (i1 == numColors - 1) di1 = -1;
            if (i2 == numColors - 1) di2 = -1;

            // iterate forwards through rainbow table
            if (i1 == 0) di1 = 1;
            if (i2 == 0) di2 = 1;

            return t;
        }

        private static int i1 = 0;
        private static int di1 = 1;
        private static int i2 = (int)numColors/3; // start second gradient color a third of the way through table
        private static int di2 = 1;
    }
}
