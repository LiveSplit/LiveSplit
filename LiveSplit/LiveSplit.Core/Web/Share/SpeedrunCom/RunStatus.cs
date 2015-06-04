using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunStatus
    {
        public RunStatusType Type { get; private set; }
        public string Examiner { get; private set; }
        public string Reason { get; private set; }
    }
}
