using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Run
    {
        public int ID { get; private set; }
        public Uri RunUri { get; private set; }
        public int GameID { get; private set; }
        public int LevelID { get; private set; }
        public int CategoryID { get; private set; }
        public Uri Video { get; private set; }
        public string Comment { get; private set; }
        public RunStatus Status { get; private set; }
        public ReadOnlyCollection<PossiblyAnonymousUser> Runners { get; private set; }
        public DateTime? Date { get; private set; }
        public DateTime? DateSubmitted { get; private set; }
        public RunTimes Times { get; private set; }
        public System System { get; private set; }
        public ReadOnlyCollection<string> Values { get; private set; } //TODO: What's a "map"?

        #region Links

        public Uri GameUri { get; private set; }
        public Uri CategoryUri { get; private set; }
        public Uri PlatformUri { get; private set; }
        public ReadOnlyCollection<string> PlayerUris { get; private set; }
        public Uri ExaminerUri { get; private set; }

        #endregion
    }
}
