using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Game
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }
        public string ShortName { get; private set; }
        public int YearOfRelease { get; private set; }
        public ReadOnlyCollection<int> PlatformIDs { get; private set; }
        public ReadOnlyCollection<int> RegionIDs { get; private set; }
        public DateTime Created { get; private set; }

        #region Links

        public ReadOnlyCollection<int> RunIDs { get; private set; }
        public ReadOnlyCollection<int> LevelIDs { get; private set; }
        public ReadOnlyCollection<int> CategoryIDs { get; private set; }
        public int ParentGameID { get; private set; }

        #endregion
    }
}
