using System;
using UpdateManager;

namespace LiveSplit.Updates
{
    public class LiveSplitUpdateable : IUpdateable
    {
        public string UpdateName
        {
            get { return "LiveSplit"; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/update.xml"; }
#else
            get { return "http://livesplit.org/update/update.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/"; }
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return UpdateHelper.Version; }
        }
    }
}
