using System;
using UpdateManager;

namespace LiveSplit.Updates
{
    public class LiveSplitUpdateable : IUpdateable
    {
        public string UpdateName => "LiveSplit";

        public string XMLURL => "http://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/release-1.8.29/update/update.xml";

        public string UpdateURL => "http://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/release-1.8.29/update/";

        public Version Version => UpdateHelper.Version;
    }
}
