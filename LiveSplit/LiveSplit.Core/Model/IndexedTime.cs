namespace LiveSplit.Model
{
    public struct IndexedTime : IIndexedTime
    {
        public Time Time { get; }
        public int Index { get; }

        public IndexedTime (Time time, int index)
        {
            Time = time;
            Index = index;
        }
    }
}
