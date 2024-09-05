using System;
using System.Xml;

using LiveSplit.Web;

namespace LiveSplit.Model;

public struct Time
{
    public static readonly Time Zero = new(TimeSpan.Zero, TimeSpan.Zero);

    public TimeSpan? RealTime { get; set; }
    public TimeSpan? GameTime { get; set; }

    public Time(TimeSpan? realTime = null, TimeSpan? gameTime = null)
        : this()
    {
        RealTime = realTime;
        GameTime = gameTime;
    }

    public Time(TimingMethod method, TimeSpan? time)
        : this()
    {
        this[method] = time;
    }

    public Time(Time time)
        : this()
    {
        RealTime = time.RealTime;
        GameTime = time.GameTime;
    }

    public TimeSpan? this[TimingMethod method]
    {
        readonly get => method == TimingMethod.RealTime ? RealTime : GameTime;
        set
        {
            if (method == TimingMethod.RealTime)
            {
                RealTime = value;
            }
            else
            {
                GameTime = value;
            }
        }
    }

    public static Time FromXml(XmlElement element)
    {
        var newTime = new Time();
        TimeSpan x;
        if (element.GetElementsByTagName("RealTime").Count > 0)
        {
            if (TimeSpan.TryParse(element["RealTime"].InnerText, out x))
            {
                newTime.RealTime = x;
            }
        }

        if (element.GetElementsByTagName("GameTime").Count > 0)
        {
            if (TimeSpan.TryParse(element["GameTime"].InnerText, out x))
            {
                newTime.GameTime = x;
            }
        }

        return newTime;
    }

    public readonly XmlElement ToXml(XmlDocument document, string name = "Time")
    {
        XmlElement parent = document.CreateElement(name);
        if (RealTime != null)
        {
            XmlElement realTime = document.CreateElement("RealTime");
            realTime.InnerText = RealTime.ToString();
            parent.AppendChild(realTime);
        }

        if (GameTime != null)
        {
            XmlElement gameTime = document.CreateElement("GameTime");
            gameTime.InnerText = GameTime.ToString();
            parent.AppendChild(gameTime);
        }

        return parent;
    }

    public readonly DynamicJsonObject ToJson()
    {
        dynamic json = new DynamicJsonObject();
        json.realTime = RealTime.ToString();
        json.gameTime = GameTime.ToString();
        return json;
    }

    public static Time ParseText(string text)
    {
        string[] splits = text.Split('|');
        var newTime = new Time();
        if (TimeSpan.TryParse(splits[0].TrimEnd(), out TimeSpan x))
        {
            newTime.RealTime = x;
        }
        else
        {
            newTime.RealTime = null;
        }

        if (splits.Length > 1)
        {
            if (TimeSpan.TryParse(splits[1].TrimStart(), out x))
            {
                newTime.GameTime = x;
            }
            else
            {
                newTime.GameTime = null;
            }
        }

        return newTime;
    }

    public override readonly string ToString()
    {
        return $"{RealTime} | {GameTime}";
    }

    public static Time operator +(Time a, Time b)
    {
        return new Time(a.RealTime + b.RealTime, a.GameTime + b.GameTime);
    }

    public static Time operator -(Time a, Time b)
    {
        return new Time(a.RealTime - b.RealTime, a.GameTime - b.GameTime);
    }
}
