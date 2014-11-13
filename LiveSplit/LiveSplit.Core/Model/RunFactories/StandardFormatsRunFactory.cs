using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var timesplittracker = new TimeSplitTrackerRunFactory();

            Add(xml, (s, f) => { xml.Stream = s; xml.FilePath = f; });
            Add(wsplit, (s, f) => wsplit.Stream = s);
            Add(splitterz, (s, f) => splitterz.Stream = s);
            Add(llanfair, (s, f) => llanfair.Path = f);
            Add(shitsplit, (s, f) => shitsplit.Stream = s);
            Add(timesplittracker, (s, f) => { timesplittracker.Stream = s; timesplittracker.Path = f; });
        }
    }
}
