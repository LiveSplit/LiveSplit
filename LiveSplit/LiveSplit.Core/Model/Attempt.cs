using LiveSplit.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace LiveSplit.Model
{
    public struct Attempt
    {
        public int Index { get; set; }
        public Time Time { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }

        /// <summary>
        /// Returns the Real Time Duration of the attempt.
        /// This even returns times if the attempt was resetted.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (Time.RealTime.HasValue)
                    return Time.RealTime.Value;
                else
                    return Ended - Started;
            }
        }

        public Attempt(int index, Time time, DateTime? started, DateTime? ended)
            : this()
        {
            Index = index;
            Time = time;
            Started = started;
            Ended = ended;
        }

        public XmlNode ToXml(XmlDocument document)
        {
            var attempt = document.CreateElement("Attempt");

            var time = Time.ToXml(document);
            attempt.InnerXml = time.InnerXml;
            
            var id = document.CreateAttribute("id");
            id.InnerText = Index.ToString();
            attempt.Attributes.Append(id);
            
            if (Started.HasValue)
            {
                var started = document.CreateAttribute("started");
                started.InnerText = Started.Value.ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(started);
            }

            if (Ended.HasValue)
            {
                var ended = document.CreateAttribute("ended");
                ended.InnerText = Ended.Value.ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(ended);
            }

            return attempt;
        }

        public static Attempt ParseXml(XmlElement node)
        {
            var newTime = Time.FromXml(node);
            var index = int.Parse(node.Attributes["id"].InnerText, CultureInfo.InvariantCulture);
            DateTime? started = null, ended = null;

            if (node.HasAttribute("started"))
            {
                started = DateTime.Parse(node.Attributes["started"].InnerText, CultureInfo.InvariantCulture);
            }

            if (node.HasAttribute("ended"))
            {
                started = DateTime.Parse(node.Attributes["ended"].InnerText, CultureInfo.InvariantCulture);
            }

            return new Attempt(index, newTime, started, ended);
        }

        public DynamicJsonObject ToJson()
        {
            dynamic json = new DynamicJsonObject();
            json.id = Index;
            json.realTime = Time.RealTime;
            json.gameTime = Time.GameTime;
            json.started = Started;
            json.ended = Ended;
            return json;
        }
    }
}
