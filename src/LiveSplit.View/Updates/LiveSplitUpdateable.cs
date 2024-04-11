using System;
using UpdateManager;

namespace LiveSplit.Updates
{
    public class LiveSplitUpdateable : IUpdateable
    {
        public string UpdateName => "LiveSplit";

        public string XMLURL => "http://livesplit.org/update/update.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => UpdateHelper.Version;
    }
}
