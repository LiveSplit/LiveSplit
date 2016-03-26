﻿using LiveSplit.Model;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using LiveSplit.Updates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.Comparisons;
using System.Globalization;
using System.Linq;

namespace LiveSplit.Web.Share
{
    public class SplitsIO : IRunUploadPlatform
    {
        protected static readonly SplitsIO _Instance = new SplitsIO();

        public static SplitsIO Instance => _Instance;

        public static readonly Uri BaseUri = new Uri("http://splits.io/");
        public static readonly Uri APIUri = new Uri("https://splits.io/api/v3/");
        public static readonly Uri APIV4Uri = new Uri("https://splits.io/api/v4/");

        public const decimal NoTime = 0.0m;

        protected SplitsIO() { }

        public string PlatformName => "Splits.io";

<<<<<<< HEAD
        public string Description
        {
            get 
            {
                return "Splits.ioはスプリット共有サイトで、自分のスプリットをアップロードまたは"
                + "他人のものをダウンロードすることができます。"
                + "\"Splits.ioからスプリットを開く\"を使うことでSplits.ioの閲覧、 "
                + "または\"Splits.ioから比較をインポート\"で他人のスプリットを比較することができます。 "
                + "Splits.ioからダウンロードしたスプリットはデータ損失なし、そして他タイマー用に形式を選択して"
                + "ダウンロードすることができます。";
            }
        }
=======
        public string Description =>
            "Splits.io is the best platform for sharing individual "
            + "splits with the world and downloading them from there. "
            + "You can also browse Splits.io with \"Open Splits From Splits.io...\" "
            + "or import splits as a comparison with \"Import Comparison From Splits.io...\". "
            + "Splits downloaded from Splits.io have not data loss, and the splits can be "
            + "downloaded in the format for any timer.";
>>>>>>> refs/remotes/LiveSplit/master

        public ISettings Settings { get; set; }

        public Uri GetSiteUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public Uri GetAPIUri(string subUri)
        {
            return new Uri(APIUri, subUri);
        }

        public Uri GetAPIV4Uri(string subUri)
        {
            return new Uri(APIV4Uri, subUri);
        }

        #region Not supported

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameList()
        {
            yield break;
        }

        IEnumerable<string> IRunUploadPlatform.GetGameNames()
        {
            yield break;
        }

        string IRunUploadPlatform.GetGameIdByName(string gameName)
        {
            return string.Empty;
        }

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameCategories(string gameId)
        {
            yield break;
        }

        string IRunUploadPlatform.GetCategoryIdByName(string gameId, string categoryName)
        {
            return string.Empty;
        }

        bool IRunUploadPlatform.VerifyLogin(string username, string password)
        {
            return true;
        }

        #endregion

        private static IEnumerable<dynamic> DoPaginatedRequest(Uri uri)
        {
            var page = 1;
            var totalItems = 1;
            var perPage = 1;
            var lastPage = 1;

            do
            {
                var request = WebRequest.Create($"{uri.AbsoluteUri}?page={page}");

                using (var response = request.GetResponse())
                {
                    int.TryParse(response.Headers["Total"], NumberStyles.Integer, CultureInfo.InvariantCulture, out totalItems);
                    int.TryParse(response.Headers["Per-Page"], NumberStyles.Integer, CultureInfo.InvariantCulture, out perPage);
                    lastPage = (int) Math.Ceiling(totalItems/(double) perPage);

                    yield return JSON.FromResponse(response);
                }

            } while (page++ < lastPage);
        }

        public IEnumerable<dynamic> SearchGame(string name)
        {
            var escapedName = HttpUtility.UrlPathEncode(name);
            var uri = GetAPIUri($"games?search={escapedName}");
            var response = JSON.FromUri(uri);
            return (response.games as IEnumerable<dynamic>) ?? new dynamic[0];
        }

        public dynamic SearchUser(string name)
        {
            try
            {
                var escapedName = HttpUtility.UrlPathEncode(name.ToLowerInvariant());
                var uri = GetAPIUri($"users/{escapedName}");
                var response = JSON.FromUri(uri);
                return response.user;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public dynamic GetGameById(int gameId)
        {
            var uri = GetAPIUri($"games/{gameId}");
            var response = JSON.FromUri(uri);
            return response.game;
        }

        public IEnumerable<dynamic> GetRunsForCategory(int gameId, int categoryId)
        {
            var uri = GetAPIUri($"games/{gameId}/categories/{categoryId}/runs");
            var pages = DoPaginatedRequest(uri);
            return pages.SelectMany(page => (page.runs as IEnumerable<dynamic>) ?? new dynamic[0]);
        }

        public IEnumerable<dynamic> GetRunsForUser(string userId)
        {
            var uri = GetAPIUri($"users/{userId}/runs");
            var pages = DoPaginatedRequest(uri);
            return pages.SelectMany(page => (page.runs as IEnumerable<dynamic>) ?? new dynamic[0]);
        }

        public dynamic GetRunById(int runId)
        {
            var uri = GetAPIUri($"runs/{runId}");
            var response = JSON.FromUri(uri);
            return response.run;
        }

        public dynamic GetV4RunById(string runId)
        {
            var uri = GetAPIV4Uri($"runs/{runId}");
            return JSON.FromUri(uri);
        }

        private void PatchRun(IRun run, string splitsIORunId)
        {
            try
            {
                var splitsIORun = GetV4RunById(splitsIORunId);
                var speedrunComId = splitsIORun.srdc_id as string;
                if (string.IsNullOrEmpty(speedrunComId))
                    return;

                var speedrunComRun = SpeedrunCom.Client.Runs.GetRun(speedrunComId);
                if (speedrunComRun == null)
                    return;

                run.PatchRun(speedrunComRun);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public IRun DownloadRunByPath(string path, bool patchRun)
        {
            var uri = GetSiteUri(path);
            return DownloadRunByUri(uri, patchRun);
        }

        public IRun DownloadRunByUri(Uri uri, bool patchRun)
        {
            var id = uri.LocalPath;
            var downloadUri = GetSiteUri($"{id}/download/livesplit");

            var request = WebRequest.Create(downloadUri);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var runFactory = new XMLRunFactory();

                    runFactory.Stream = memoryStream;
                    runFactory.FilePath = null;

                    var run = runFactory.Create(new StandardComparisonGeneratorsFactory());
                    if (patchRun)
                        PatchRun(run, id);
                    return run;
                }
            }
        }

        public IRun DownloadRunById(int runId, bool patchRun)
        {
            var run = GetRunById(runId);
            var uri = GetSiteUri(run.path);
            return DownloadRunByUri(uri, patchRun);
        }

        public void ClaimWithSpeedrunComRun(string splitsIORunId, string claimToken, string srdcRunId)
        {
            var uri = GetAPIV4Uri(
                $"runs/{Uri.EscapeDataString(splitsIORunId)}?claim_token={Uri.EscapeDataString(claimToken)}&srdc_id={Uri.EscapeDataString(srdcRunId)}");

            var request = WebRequest.Create(uri);
            request.Method = "PUT";
            request.GetResponse();
        }

        public string Share(IRun run, Func<Image> screenShotFunction = null, bool claimTokenUri = false)
        {
            string image_url = null;

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
                image_url = (string)result.data.link;
            }

            var request = (HttpWebRequest)WebRequest.Create(GetAPIUri("runs").AbsoluteUri);
            request.Method = "POST";
            request.Host = "splits.io";
            request.UserAgent = UpdateHelper.UserAgent;
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

            dynamic json;
            using (var response = request.GetResponse())
            {
                json = JSON.FromResponse(response);
            }

            return claimTokenUri ? json.uris.claim_uri : json.uris.public_uri;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            var url = Share(run, screenShotFunction, claimTokenUri: true);
            Process.Start(url);
            Clipboard.SetText(url);

            return true;
        }
    }
}
