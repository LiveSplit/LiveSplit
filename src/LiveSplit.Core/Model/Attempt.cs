using LiveSplit.UI;
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
        public AtomicDateTime? Started { get; set; }
        public AtomicDateTime? Ended { get; set; }
        public TimeSpan? PauseTime { get; set; }

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

        public Attempt(int index, Time time, AtomicDateTime? started, AtomicDateTime? ended, TimeSpan? pauseTime)
            : this()
        {
            Index = index;
            Time = time;
            Started = started;
            Ended = ended;
            PauseTime = pauseTime;
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
                started.InnerText = Started.Value.Time.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(started);
                attempt.Attributes.Append(SettingsHelper.ToAttribute(document, "isStartedSynced", Started.Value.SyncedWithAtomicClock));
            }

            if (Ended.HasValue)
            {
                var ended = document.CreateAttribute("ended");
                ended.InnerText = Ended.Value.Time.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                attempt.Attributes.Append(ended);
                attempt.Attributes.Append(SettingsHelper.ToAttribute(document, "isEndedSynced", Ended.Value.SyncedWithAtomicClock));
            }

            if (PauseTime.HasValue)
            {
                var pauseTime = document.CreateElement("PauseTime");
                pauseTime.InnerText = PauseTime.ToString();
                attempt.AppendChild(pauseTime);
            }

            return attempt;
        }

        public static Attempt ParseXml(XmlElement node)
        {
            var newTime = Time.FromXml(node);
            var index = int.Parse(node.Attributes["id"].InnerText, CultureInfo.InvariantCulture);
            AtomicDateTime? started = null;
            var startedSynced = false;
            AtomicDateTime? ended = null;
            var endedSynced = false;

            if (node.HasAttribute("started"))
            {
                var startedTime = DateTime.Parse(node.Attributes["started"].InnerText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                if (node.HasAttribute("isStartedSynced"))
                    startedSynced = bool.Parse(node.Attributes["isStartedSynced"].InnerText);
                started = new AtomicDateTime(startedTime, startedSynced);
            }

            if (node.HasAttribute("ended"))
            {
                var endedTime = DateTime.Parse(node.Attributes["ended"].InnerText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                if (node.HasAttribute("isEndedSynced"))
                    endedSynced = bool.Parse(node.Attributes["isEndedSynced"].InnerText);
                ended = new AtomicDateTime(endedTime, endedSynced);
            }

            TimeSpan? pauseTime = null;
            if (node.GetElementsByTagName("PauseTime").Count > 0)
            {
                TimeSpan x;
                if (TimeSpan.TryParse(node["PauseTime"].InnerText, out x))
                    pauseTime = x;
            }

            return new Attempt(index, newTime, started, ended, pauseTime);
        }

        public DynamicJsonObject ToJson()
        {
            dynamic json = new DynamicJsonObject();
            json.id = Index;
            json.realTime = Time.RealTime;
            json.gameTime = Time.GameTime;
            json.started = Started;
            json.ended = Ended;
            json.pauseTime = PauseTime;
            return json;
        }
    }
}
