using System;

namespace UpdateManager
{
    public class UpdateManagerUpdateable : IUpdateable
    {
        private UpdateManagerUpdateable() { }

        private static UpdateManagerUpdateable _Instance { get; set; }

        public static UpdateManagerUpdateable Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UpdateManagerUpdateable();
                return _Instance;
            }
        }

        public string UpdateName
        {
            get { return "Update Manager"; }
        }

        public string XMLURL
        {
            get { return "http://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/release-1.8.29/update/update.updater.xml"; }
        }

        public string UpdateURL
        {
            get { return "http://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/release-1.8.29/update/"; }
        }

        public Version Version
        {
            get { return Version.Parse("2.0.3"); }
        }
    }
}
