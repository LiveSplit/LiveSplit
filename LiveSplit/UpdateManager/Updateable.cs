using System;

namespace UpdateManager
{
    internal class Updateable : IUpdateable
    {
        public string UpdateName
        {
            get { return ""; }
        }

        public string XMLURL { get; set; }

        public string UpdateURL { get; set; }

        public Version Version { get; set; }

        public Updateable(string xmlURL, string updateURL, Version version)
        {
            XMLURL = xmlURL;
            UpdateURL = updateURL;
            Version = version;
        }
    }
}
