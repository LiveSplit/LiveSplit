using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Updateable(String xmlURL, String updateURL, Version version)
        {
            XMLURL = xmlURL;
            UpdateURL = updateURL;
            Version = version;
        }
    }
}
