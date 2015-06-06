using LiveSplit.Web;
using System;
using System.Globalization;
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
        /// This either returns a 1.6+ Time Stamp based duration
        /// or the duration of the run (assuming it's not resetted)
        /// if it's from before LiveSplit 1.6. If it is from before
        /// 1.6 and resetted then it will return null.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (Ended.HasValue && Started.HasValue)
                    return Ended - Started;
                else
                    return Time.RealTime;   
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
                started.InnerText = Started.Value.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(started);
            }

            if (Ended.HasValue)
            {
                var ended = document.CreateAttribute("ended");
                ended.InnerText = Ended.Value.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(ended);
            }

            return attempt;
        }

        public static Attempt ParseXml(XmlElement node)
        {
            var newTime = Time.FromXml(node);
            var index = int.Parse(node.Attributes["id"].InnerText, CultureInfo.InvariantCulture);
            DateTime? started = null;
            DateTime? ended = null;

            if (node.HasAttribute("started"))
            {
                started = DateTime.Parse(node.Attributes["started"].InnerText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }

            if (node.HasAttribute("ended"))
            {
                ended = DateTime.Parse(node.Attributes["ended"].InnerText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
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
