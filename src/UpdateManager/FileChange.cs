using System;
using System.Xml;

namespace UpdateManager
{
    public class FileChange
    {
        public string Path { get; set; }
        public string LocalPath { get; set; } // Optional
        public ChangeStatus Status { get; set; }

        public FileChange(string path, string localPath, ChangeStatus status)
        {
            Path = path;
            LocalPath = localPath;
            Status = status;
        }

        public static FileChange Parse(XmlNode node) =>
            new FileChange
            (
                node.Attributes["path"].InnerText,
                node.Attributes["localPath"]?.InnerText,
                (ChangeStatus) Enum.Parse(typeof(ChangeStatus), node.Attributes["status"].InnerText, true)
            );
    }
}
