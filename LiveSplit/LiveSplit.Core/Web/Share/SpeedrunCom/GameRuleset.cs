using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class GameRuleset
    {
        public bool ShowMilliseconds { get; private set; }
        public bool RequiresVerification { get; private set; }
        public bool RequiresVideo { get; private set; }
    }
}
