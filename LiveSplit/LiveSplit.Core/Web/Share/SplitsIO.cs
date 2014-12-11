using LiveSplit.Model;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using LiveSplit.Updates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class SplitsIO : IRunUploadPlatform
    {
        protected static SplitsIO _Instance = new SplitsIO();

        public static SplitsIO Instance { get { return _Instance; } }

        protected SplitsIO() { }

        public string PlatformName
        {
            get { return "Splits.io"; }
        }

        public String Description
        {
            get 
            {
                return "Splits.io is the best platform for sharing individual "
                + "splits with the world and downloading them from there. "
                + "You can also directly import Splits.io links with \"Open Splits from URL...\" "
                + "or import them as a comparison with \"Import Comparison from URL...\". "
                + "Splits downloaded from Splits.io have not data loss, and the splits can be "
                + "downloaded in the format for any timer.";
            }
        }

        public ISettings Settings { get; set; }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            yield break;
        }

        public IEnumerable<string> GetGameNames()
        {
            yield break;
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

        public String Share(IRun run, Func<Image> screenShotFunction = null)
        {
            String image_url = null;

            if (screenShotFunction != null)
            {
                var image = screenShotFunction();
                if (image.Width < 280 || image.Height < 150)
                {
                    var factor1 = 280.0 / image.Width;
                    var factor2 = 150.0 / image.Height;
                    var factor = Math.Max(factor1, factor2);
                    image = new Bitmap(image, (int)(factor * image.Width + 0.5), (int)(factor * image.Height + 0.5));
                }
                var result = Imgur.Instance.UploadImage(image);
                image_url = (String)result.data.link;
            }

            var request = (HttpWebRequest)HttpWebRequest.Create("http://splits.io/api/v2/runs");
            request.Method = "POST";
            request.Host = "splits.io";
            request.UserAgent = "LiveSplit/" + UpdateHelper.Version.ToString();
            using (var stream = request.GetRequestStream())
            {
                request.ContentType = "multipart/form-data; boundary=AaB03x";
                request.Referer = "http://splits.io/upload/fallback";
                request.Headers.Add("Origin", "http://splits.io");

                var writer = new StreamWriter(stream);

                if (image_url != null)
                {
                    writer.WriteLine("--AaB03x");
                    writer.WriteLine("Content-Disposition: form-data; name=\"image_url\"");
                    writer.WriteLine();
                    writer.WriteLine(image_url);
                }

                writer.WriteLine("--AaB03x");
                writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"splits\"");
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();

                new XMLRunSaver().Save(run, stream);

                writer.WriteLine();
                writer.WriteLine("--AaB03x--");
                writer.Flush();
            }

            var response = request.GetResponse();
            var json = JSON.FromUri(new Uri(response.Headers["Location"]));

            var url = "http://splits.io" + json.run.path;
            return url;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var url = Share(run, screenShotFunction);
            Process.Start(url);
            Clipboard.SetText(url);

            return true;
        }
    }
}
