using LiveSplit.Web;
using System.Xml;

namespace LiveSplit.Model
{
    public interface IIndexedTime
    {
        Time Time { get; set; }
        int Index { get; set; }
    }
    public static class IndexedTimeHelper
    {
        public static XmlNode ToXml(this IIndexedTime indexedTime, XmlDocument document)
        {
            var element = indexedTime.Time.ToXml(document);
            var attribute = document.CreateAttribute("id");
            attribute.InnerText = indexedTime.Index.ToString();
            element.Attributes.Append(attribute);
            return element;
        }
        public static IIndexedTime ParseXml(XmlElement node)
        {
            var newTime = Time.FromXml(node);           
            var index = int.Parse(node.GetAttribute("id"));
            return new IndexedTime(newTime, index);
        }

        public static IIndexedTime ParseXmlOld(XmlElement node)
        {
            var newTime = node == null ? default(Time) : Time.ParseText(node.InnerText);
            var index = int.Parse(node.GetAttribute("id"));
            return new IndexedTime(newTime, index);
        }

        public static DynamicJsonObject ToJson(this IIndexedTime indexedTime)
        {
            dynamic coolObject = new DynamicJsonObject();
            coolObject.id = indexedTime.Index;
            coolObject.realTime = indexedTime.Time.RealTime;
            coolObject.gameTime = indexedTime.Time.GameTime;
            return coolObject;
        }
    }
}
