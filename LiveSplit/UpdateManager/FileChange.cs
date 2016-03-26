using System;
using System.Xml;

namespace UpdateManager
{
    public class FileChange
    {
        public string Path { get; set; }
        public ChangeStatus Status { get; set; }

        public FileChange(string path, ChangeStatus status)
        {
            Path = path;
            Status = status;
        }

        public static FileChange Parse(XmlNode node)
        {
            return new FileChange
                (
                node.Attributes["path"].InnerText, 
                (ChangeStatus)Enum.Parse(typeof(ChangeStatus), node.Attributes["status"].InnerText, true)
                );
        }
    }
}
