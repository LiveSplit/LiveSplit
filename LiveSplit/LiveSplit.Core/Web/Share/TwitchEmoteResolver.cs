using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace LiveSplit.Web.Share
{
    public static class TwitchEmoteResolver
    {
        private static IDictionary<Regex, Lazy<Image>> _FrankerFaceZGlobalEmotes;
        private static IDictionary<Regex, Lazy<Image>> _FrankerFaceZChannelEmotes;
        private static IDictionary<Regex, Lazy<Image>> _TwitchEmotes;

        private static Task<Dictionary<Regex, Lazy<Image>>> TwitchEmoteDownload { get; set; }

        public static IDictionary<Regex, Lazy<Image>> FrankerFaceZGlobalEmotes
        {
            get
            {
                try
                {
                    if (_FrankerFaceZGlobalEmotes == null)
                        DownloadFrankerFaceZGlobalEmotesList();
                }
                catch
                {
                    return new Dictionary<Regex, Lazy<Image>>();
                }

                return _FrankerFaceZGlobalEmotes;
            }
        }
        public static IDictionary<Regex, Lazy<Image>> FrankerFaceZChannelEmotes
        {
            get
            {
                try
                {
                    if (_FrankerFaceZChannelEmotes == null)
                        DownloadFrankerFaceZChannelEmotesList();
                }
                catch
                {
                    return new Dictionary<Regex, Lazy<Image>>();
                }

                return _FrankerFaceZChannelEmotes;
            }
        }
        public static IDictionary<Regex, Lazy<Image>> TwitchEmotes
        {
            get
            {
                try
                {
                    if (_TwitchEmotes == null)
                        DownloadTwitchEmotesList();
                }
                catch
                {
                    return new Dictionary<Regex, Lazy<Image>>();
                }

                return _TwitchEmotes ?? new Dictionary<Regex, Lazy<Image>>();
            }
        }

        public static void DownloadFrankerFaceZGlobalEmotesList()
        {
            String cssStr;
            using (var wc = new WebClient())
            {
                cssStr = wc.DownloadString("http://frankerfacez.storage.googleapis.com/global.css");
            }
            _FrankerFaceZGlobalEmotes = new Dictionary<Regex, Lazy<Image>>();
            foreach (var line in cssStr.Split('\n'))
            {
                if (line.Contains("content: \""))
                {
                    var cutOff = line.Substring(line.IndexOf("content: \"") + "content: \"".Length);
                    var name = cutOff.Substring(0, cutOff.IndexOf("\""));

                    cutOff = line.Substring(line.IndexOf("background-image: url(\"") + "background-image: url(\"".Length);
                    var url = cutOff.Substring(0, cutOff.IndexOf("\""));

                    _FrankerFaceZGlobalEmotes.Add(new Regex(name), new Lazy<Image>(() => DownloadImage(url)));
                }
            }
        }

        public static void DownloadFrankerFaceZChannelEmotesList()
        {
            var xmlDoc = new XmlDocument();
            string xmlStr;
            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString("http://frankerfacez.storage.googleapis.com/");
            }

            xmlDoc.LoadXml(xmlStr);
            XmlNodeList emotes = xmlDoc.GetElementsByTagName("Contents");
            _FrankerFaceZChannelEmotes = new Dictionary<Regex, Lazy<Image>>();
            foreach (XmlNode node in emotes)
            {
                if (node.FirstChild.FirstChild.InnerText.Contains(".png"))
                {
                    var text = node.FirstChild.FirstChild.InnerText;
                    var url = "http://frankerfacez.storage.googleapis.com/" + text;
                    var cutOff = text.Substring(text.LastIndexOf("/") + 1);
                    var name = cutOff.Substring(0, cutOff.Length - 4);

                    _FrankerFaceZChannelEmotes.Add(new Regex(name), new Lazy<Image>(() => DownloadImage(url)));
                }
            }
        }

        public static void DownloadTwitchEmotesList()
        {
            if (TwitchEmoteDownload == null)
            {
                TwitchEmoteDownload = Task.Factory.StartNew(() =>
                {
                    var emotes = JSON.FromUri(new Uri("https://api.twitch.tv/kraken/chat/emoticons"));

                    if (emotes == null)
                        return null;

                    var twitchEmotes = new Dictionary<Regex, Lazy<Image>>();
                    foreach (var emoteSearch in emotes.emoticons)
                    {
                        var image = emoteSearch.images[0];
                        var url = image.url;
                        var regex = emoteSearch.regex;

                        twitchEmotes.Add(new Regex(regex), new Lazy<Image>(() => DownloadImage(url)));
                    }
                    return twitchEmotes;
                }
                );
            }
            if (TwitchEmoteDownload.IsCompleted)
                _TwitchEmotes = TwitchEmoteDownload.Result;
        }

        private static Image DownloadImage(String url)
        {
            using (var stream = WebRequest.Create(url).GetResponse().GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static bool IsEmote(
            String text,
            bool useTwitchEmotes = true,
            bool useFrankerFaceZGlobalEmotes = true,
            bool useFrankerFaceZChannelEmotes = true)
        {
            try
            {
                if (useTwitchEmotes)
                {
                    var foundEmote = TwitchEmotes.Where(x => x.Key.IsMatch(text) && x.Key.ToString().Length == text.Length).Select(x => x.Value).FirstOrDefault();
                    if (foundEmote != null)
                    {
                        return true;
                    }
                }
                if (useFrankerFaceZGlobalEmotes)
                {
                    var foundEmote = FrankerFaceZGlobalEmotes.Where(x => x.Key.IsMatch(text) && x.Key.ToString().Length == text.Length).Select(x => x.Value).FirstOrDefault();
                    if (foundEmote != null)
                    {
                        return true;
                    }
                }
                if (useFrankerFaceZChannelEmotes)
                {
                    var foundEmote = FrankerFaceZChannelEmotes.Where(x => x.Key.IsMatch(text) && x.Key.ToString().Length == text.Length).Select(x => x.Value).FirstOrDefault();
                    if (foundEmote != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        public static Image Resolve(
            String emote, 
            bool useTwitchEmotes = true, 
            bool useFrankerFaceZGlobalEmotes = true, 
            bool useFrankerFaceZChannelEmotes = false)
        {
            if (useTwitchEmotes)
            {
                var foundEmote = TwitchEmotes.Where(x => x.Key.IsMatch(emote)).Select(x => x.Value).FirstOrDefault();
                if (foundEmote != null)
                {
                    return foundEmote.Value;
                }
            }
            if (useFrankerFaceZGlobalEmotes)
            {
                var foundEmote = FrankerFaceZGlobalEmotes.Where(x => x.Key.IsMatch(emote)).Select(x => x.Value).FirstOrDefault();
                if (foundEmote != null)
                {
                    return foundEmote.Value;
                }
            }
            if (useFrankerFaceZChannelEmotes)
            {
                var foundEmote = FrankerFaceZChannelEmotes.Where(x => x.Key.IsMatch(emote)).Select(x => x.Value).FirstOrDefault();
                if (foundEmote != null)
                {
                    return foundEmote.Value;
                }
            }

            throw new ArgumentException();
        }
    }
}
