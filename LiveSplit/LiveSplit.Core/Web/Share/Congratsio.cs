using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiveSplit.Web.Share
{
    public class Congratsio : IRunUploadPlatform
    {
        protected static Congratsio _Instance = new Congratsio();
        public static Congratsio Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("http://www.congratsio.com");

        protected Congratsio() { }

        protected Uri GetUri(String subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public string PlatformName
        {
            get { return "Congratsio"; }
        }

        public String Description
        {
            get 
            {
                return "Congratsio is a platform that allows you to track "
                     + "your Personal Bests with the bonus of a congratulatory "
                     + "tweet whenever you get a new Personal Best.";
            }
        }

        public ISettings Settings { get; set; }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            yield break;
        }

        public IEnumerable<string> GetGameNames()
        {
            var json = (IEnumerable<dynamic>)JSON.FromUri(GetUri("ajax/gameajax.php"));

            Func<dynamic, String> selector = x => x.value;

            return json.Select(selector);
        }

        public string GetGameIdByName(string gameName)
        {
            return String.Empty;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return String.Empty;
        }

        public bool VerifyLogin(string username, string password)
        {
            return true;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var timeFormatter = new RegularTimeFormatter(TimeAccuracy.Seconds);

            if (attachSplits)
                comment += " " + SplitsIO.Instance.Share(run, screenShotFunction);

            var postRequest = (HttpWebRequest)HttpWebRequest.Create(GetUri("submit"));
            postRequest.Method = "POST";
            postRequest.ContentType = "application/x-www-form-urlencoded";

            using (var postStream = postRequest.GetRequestStream())
            {
                var writer = new StreamWriter(postStream);

                writer.Write("nickname=");
                writer.Write(HttpUtility.UrlEncode(username));

                //nickname=CryZeTesting&country=Germany&twitter=CryZe107&youtube=CryZe92&twitch=CryZe92&game=The+Legend+of+Zelda%3A+The+Wind+Waker&category=Test%25&console=&played_on=Emulator&emulator=Dolphin+3.5&region=NTSC-J&time=5%3A07%3A25&date=12%2F18%2F2013&video=http%3A%2F%2Fyoutube.com%2FTesting&notes=Still+in+Browser.+And+I+just+fucked+up+xD

                //Country

                //Twitter
                /*if (Twitter.Instance.)
                writer.Write("nickname=");
                writer.Write(HttpUtility.UrlEncode(username));*/

                //Youtube
                /*writer.Write("nickname=");
                writer.Write(HttpUtility.UrlEncode(username));*/

                if (Twitch.Instance.IsLoggedIn)
                {
                    writer.Write("&twitch=");
                    writer.Write(HttpUtility.UrlEncode(Twitch.Instance.ChannelName));
                }

                writer.Write("&game=");
                writer.Write(HttpUtility.UrlEncode(run.GameName));

                writer.Write("&category=");
                writer.Write(HttpUtility.UrlEncode(run.CategoryName));

                writer.Write("&console="); //TODO We need console
                //writer.Write(HttpUtility.UrlEncode(run.CategoryName));

                //Played on

                //region

                //Time
                writer.Write("&time=");
                writer.Write(HttpUtility.UrlEncode(timeFormatter.Format(run.Last().PersonalBestSplitTime.RealTime)));

                writer.Write("&date=");
                var dateTime = TripleDateTime.Now;
                writer.Write(HttpUtility.UrlEncode(String.Format("{0:00}/{1:00}/{2}", dateTime.UtcNow.Month, dateTime.UtcNow.Day, dateTime.UtcNow.Year)));

                writer.Write("&video=");
                writer.Write(HttpUtility.UrlEncode(video));

                writer.Write("&notes=");
                writer.Write(HttpUtility.UrlEncode(comment));

                writer.Flush();
            }

            using (var resultStream = postRequest.GetResponse().GetResponseStream())
            {
                var reader = new StreamReader(resultStream);

                return reader.ReadToEnd().Contains("<strong>Submitted!</strong>");
            }
        }
    }
}
