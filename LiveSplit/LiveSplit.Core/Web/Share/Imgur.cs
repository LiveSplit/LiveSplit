using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace LiveSplit.Web.Share
{
    public class Imgur : IRunUploadPlatform
    {
        private const string YOUR_CLIENT_ID = "63e6ae2de8601ef";

        protected static Imgur _Instance = new Imgur();

        public static Imgur Instance { get { return _Instance; } }

        public static readonly Uri BaseUri = new Uri("https://api.imgur.com/");

        protected Imgur() { }

        protected Uri GetUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public string PlatformName
        {
            get { return "Imgur"; }
        }

        public string Description
        {
            get
            {
                return "ImgurにLiveSplitのスクリーンショットを自動的にアップロードできます。";
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
            return null;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return null;
        }

        public bool VerifyLogin(string username, string password)
        {
            return true;
        }

        public dynamic UploadImage(Image image, string title = "", string description = "")
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var request = (HttpWebRequest)WebRequest.Create(GetUri("3/image"));
                request.Method = "POST";
                request.Headers.Add("Authorization", "Client-ID " + YOUR_CLIENT_ID);

                using (var stream = request.GetRequestStream())
                {
                    request.ContentType = "multipart/form-data; boundary=AaB03x";
                    var writer = new StreamWriter(stream);
                    writer.WriteLine("--AaB03x");
                    writer.WriteLine("Content-Disposition: form-data; name=\"title\"");
                    writer.WriteLine();
                    writer.WriteLine(title);
                    writer.WriteLine("--AaB03x");
                    writer.WriteLine("Content-Disposition: form-data; name=\"description\"");
                    writer.WriteLine();
                    writer.WriteLine(description);
                    writer.WriteLine("--AaB03x");
                    writer.WriteLine("Content-Disposition: form-data; name=\"image\"; filename=\"splits.png\"");
                    writer.WriteLine("Content-Type: image/png");
                    writer.WriteLine();
                    writer.Flush();

                    memoryStream.CopyTo(stream);

                    writer.WriteLine();
                    writer.WriteLine("--AaB03x--");
                    writer.Flush();
                }

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream);
                    var resultString = reader.ReadToEnd();
                    var result = JSON.FromString(resultString);

                    return result;
                }
            }
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var titleBuilder = new StringBuilder();

            var gameNameEmpty = string.IsNullOrEmpty(run.GameName);
            var categoryEmpty = string.IsNullOrEmpty(run.CategoryName);

            titleBuilder.Append(new RegularTimeFormatter(TimeAccuracy.Seconds).Format(run.Last().PersonalBestSplitTime[method]));
            if (titleBuilder.Length > 0 && (!gameNameEmpty || !categoryEmpty))
                titleBuilder.Append(" in ");
            titleBuilder.Append(run.GameName);
            if (!gameNameEmpty && !categoryEmpty)
                titleBuilder.Append(" - ");
            titleBuilder.Append(run.CategoryName);

            if (attachSplits)
                comment += " " + SplitsIO.Instance.Share(run, screenShotFunction);

            if (screenShotFunction != null)
            {
                var image = screenShotFunction();
                var result = UploadImage(image, titleBuilder.ToString(), comment);

                var url = "http://imgur.com/" + (string)result.data.id;
                Process.Start(url);
                Clipboard.SetText(url);
            }

            return true;
        }
    }
}
