﻿using System.Xml;

using LiveSplit.Web;

namespace LiveSplit.Model;

public interface IIndexedTime
{
    Time Time { get; }
    int Index { get; }
}
public static class IndexedTimeHelper
{
    public static XmlNode ToXml(this IIndexedTime indexedTime, XmlDocument document)
    {
        XmlElement element = indexedTime.Time.ToXml(document);
        XmlAttribute attribute = document.CreateAttribute("id");
        attribute.InnerText = indexedTime.Index.ToString();
        element.Attributes.Append(attribute);
        return element;
    }

    public static IIndexedTime ParseXml(XmlElement node)
    {
        var newTime = Time.FromXml(node);
        int index = int.Parse(node.GetAttribute("id"));
        return new IndexedTime(newTime, index);
    }

    public static IIndexedTime ParseXmlOld(XmlElement node)
    {
        Time newTime = node == null ? default : Time.ParseText(node.InnerText);
        int index = int.Parse(node.GetAttribute("id"));
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
