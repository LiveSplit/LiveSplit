namespace LiveSplit.Model
{
    public struct IndexedTime : IIndexedTime
    {
        public Time Time { get; private set; }
        public int Index { get; private set; }

        public IndexedTime (Time time, int index) : this()
        {
            Time = time;
            Index = index;
        }
    }
}
