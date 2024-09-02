using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UpdateManager;

public class Update
{
    public Version Version { get; set; }
    public IList<string> ChangeLog { get; set; }
    public IList<FileChange> FileChanges { get; set; }

    public Update(Version version, IEnumerable<string> changeLog, IEnumerable<FileChange> fileChanges)
    {
        Version = version;
        ChangeLog = changeLog.ToList();
        FileChanges = fileChanges.ToList();
    }

    public static Update Parse(XmlNode node)
    {
        var version = Version.Parse(node.Attributes["version"].InnerText);

        List<string> changeLog = [];
        foreach (XmlNode changeNode in node["changelog"].ChildNodes)
        {
            changeLog.Add(changeNode.InnerText);
        }

        List<FileChange> fileChanges = [];
        foreach (XmlNode changeNode in node["files"].ChildNodes)
        {
            fileChanges.Add(FileChange.Parse(changeNode));
        }

        return new Update(version, changeLog, fileChanges);
    }
}
