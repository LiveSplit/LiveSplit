namespace LiveSplit.Model.RunFactories
{
    public class StandardFormatsRunFactory : CompositeRunFactory
    {
        public StandardFormatsRunFactory()
        {
            var xml = new XMLRunFactory();
            var wsplit = new WSplitRunFactory();
            var splitterz = new SplitterZRunFactory();
            var llanfair = new LlanfairRunFactory();
            var shitsplit = new ShitSplitRunFactory();
            var urn = new UrnRunFactory();
            var splitty = new SplittyRunFactory();
            var timesplittracker = new TimeSplitTrackerRunFactory();
            var portal2LiveTimer = new Portal2LiveTimerRunFactory();

            Add(xml, (s, f) => { xml.Stream = s; xml.FilePath = f; });
            Add(wsplit, (s, f) => wsplit.Stream = s);
            Add(splitterz, (s, f) => splitterz.Stream = s);
            Add(shitsplit, (s, f) => shitsplit.Stream = s);
            Add(urn, (s, f) => urn.Stream = s);
            Add(splitty, (s, f) => splitty.Stream = s);
            Add(timesplittracker, (s, f) => { timesplittracker.Stream = s; timesplittracker.Path = f; });
            Add(portal2LiveTimer, (s, f) => portal2LiveTimer.Stream = s);
            Add(llanfair, (s, f) => llanfair.Path = f);
        }
    }
}
