using System;
namespace LiveSplit.Model
{
    public class IndexedTime : IIndexedTime
    {
        public Time Time { get; set; }
        public int Index { get; set; }

        public IndexedTime (Time time, int index)
        {
            Time = time;
            Index = index;
        }
    }
}
