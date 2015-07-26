using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public static class TwitchEmoteResolver
    {
        private static IDictionary<Regex, Lazy<Image>> _FrankerFaceZGlobalEmotes;
        private static IDictionary<Regex, Lazy<Image>> _FrankerFaceZChannelEmotes;
        private static IDictionary<Regex, Lazy<Image>> _TwitchEmotes;

        private static Task<Dictionary<Regex, Lazy<Image>>> TwitchEmoteDownload { get; set; }

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

        public static void DownloadTwitchEmotesList()
        {
            if (TwitchEmoteDownload == null)
            {
                TwitchEmoteDownload = Task.Factory.StartNew(() =>
                {
                    dynamic emotes = null;
                    try
                    {
                        emotes = JSON.FromUri(new Uri("https://api.twitch.tv/kraken/chat/emoticons"));
                    }
                    catch { }

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

        private static Image DownloadImage(string url)
        {
            var request = WebRequest.Create(url);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static bool IsEmote(string text)
        {
            try
            {
                var foundEmote = TwitchEmotes.Where(x => x.Key.IsMatch(text) && x.Key.ToString().Length == text.Length).Select(x => x.Value).FirstOrDefault();
                if (foundEmote != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        public static Image Resolve(string emote)
        {
            var foundEmote = TwitchEmotes.Where(x => x.Key.IsMatch(emote)).Select(x => x.Value).FirstOrDefault();
            if (foundEmote != null)
            {
                return foundEmote.Value;
            }
       
            throw new ArgumentException();
        }
    }
}
