using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UpdateManager
{
    public class Update
    {
        public Version Version { get; set; }
        public IList<String> ChangeLog { get; set; }
        public IList<FileChange> FileChanges { get; set; }

        public Update(Version version, IEnumerable<String> changeLog, IEnumerable<FileChange> fileChanges)
        {
            Version = version;
            ChangeLog = changeLog.ToList();
            FileChanges = fileChanges.ToList();
        }

        public static Update Parse(XmlNode node)
        {
            Version version = Version.Parse(node.Attributes["version"].InnerText);

            List<String> changeLog = new List<string>();
            foreach (XmlNode changeNode in node["changelog"].ChildNodes)
            {
                changeLog.Add(changeNode.InnerText);
            }

            List<FileChange> fileChanges = new List<FileChange>();
            foreach (XmlNode changeNode in node["files"].ChildNodes)
            {
                fileChanges.Add(FileChange.Parse(changeNode));
            }

            return new Update(version, changeLog, fileChanges);
        }
    }
}
