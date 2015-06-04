using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Level
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Uri Uri { get; private set; }
        public string Rules { get; private set; }
        public int GameID { get; private set; }
        public ReadOnlyCollection<int> RunIDs { get; private set; }
    }
}
