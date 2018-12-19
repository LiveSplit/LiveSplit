using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class Twitch : IRunUploadPlatform
    {
        internal const string ClientId = "lkz3x9qaxaeujde1tvq21r8d7cdr40x";

        public static readonly Uri BaseUri = new Uri("https://api.twitch.tv/kraken/");

        protected static readonly Twitch _Instance = new Twitch();
        public static Twitch Instance => _Instance;

        protected string AccessToken { get; set; }
        public string ChannelName { get; protected set; }

        internal List<string> _Subscribers;
        public IEnumerable<string> Subscribers
        {
            get
            {
                if (_Subscribers == null)
                {
                    try
                    {
                        _Subscribers = new List<string>();
                        int offset = 0;
                        dynamic result = null;
                        do
                        {
                            result = curl(string.Format("channels/{0}/subscriptions?limit=100&offset={1}", HttpUtility.UrlEncode(ChannelName), offset));
                            var subscribers = (IEnumerable<dynamic>)result.subscriptions;
                            var subscriberNames = subscribers.Select(new Func<dynamic, string>(x => x.user.display_name));
                            _Subscribers.AddRange(subscriberNames);
                        } while ((offset += 100) < result._total);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                return _Subscribers;
            }
        }

        public TwitchChat Chat => ConnectedChats.Values.First();
        public IDictionary<string, TwitchChat> ConnectedChats { get; protected set; }

        public ITimerModel _AutoUpdateModel;
        public ITimerModel AutoUpdateModel
        {
            get
            {
                return _AutoUpdateModel;
            }
            set
            {
                _AutoUpdateModel = value;
                value.OnSplit += UpdateTwitch;
                value.OnReset += (s, e) => UpdateTwitch(s, null);
                value.OnStart += UpdateTwitch;
                value.OnSkipSplit += UpdateTwitch;
                value.OnUndoSplit += UpdateTwitch;
            }
        }

        void UpdateTwitch(object sender, EventArgs e)
        {
            new Thread(() =>
                {
                    try
                    {
                        if (IsLoggedIn)
                        {
                            var state = AutoUpdateModel.CurrentState;
                            var phase = state.CurrentPhase;
                            var run = state.Run;

                            var deltaFormatter = new DeltaTimeFormatter();
                            var title = $"{run.GameName} - {run.CategoryName} Speedrun";

                            if (phase == TimerPhase.Running)
                            {
                                if (state.CurrentSplitIndex > 0)
                                {
                                    var lastSplit = run[state.CurrentSplitIndex - 1];
                                    var delta = deltaFormatter.Format(lastSplit.SplitTime[state.CurrentTimingMethod] - lastSplit.PersonalBestSplitTime[state.CurrentTimingMethod]);
                                    var splitname = lastSplit.Name;
                                    title = $"{title} ({delta} on {splitname})";
                                }
                            }

                            SetStreamTitleAndGame(title);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }).Start();
        }

        public bool IsAutoUpdating { get; set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(ChannelName);

        protected Twitch()
        {
            ConnectedChats = new Dictionary<string, TwitchChat>();
        }

        protected Uri GetUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }
    
        public string PlatformName => "Twitch";

        public string Description =>
@"Sharing to Twitch will automatically update your 
stream title and game playing based on the information 
in your splits. Twitch must authenticate with LiveSplit 
the first time that sharing to Twitch is used.";

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
            return string.Empty;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return string.Empty;
        }

        public TwitchChat ConnectToChat(string channel = null)
        {
            channel = (channel ?? ChannelName).ToLower();
            if (ConnectedChats.ContainsKey(channel))
                throw new ArgumentException("Already connected to channel");
            var chat = new TwitchChat(AccessToken, channel);
            ConnectedChats.Add(channel, chat);
            return chat;
        }

        public void CloseAllChatConnections()
        {
            foreach (var chat in ConnectedChats.Values)
                chat.Dispose();

            ConnectedChats.Clear();
        }

        bool IRunUploadPlatform.VerifyLogin(string username, string password)
        {
            return VerifyLogin();
        }

        public bool VerifyLogin()
        {
            AccessToken = WebCredentials.TwitchAccessToken;

            if (VerifyAccessToken())
            {
                return true;
            }
            else
            {
                var form = new TwitchOAuthForm();
                form.ShowDialog();

                AccessToken = form.AccessToken;

                var verified = VerifyAccessToken();

                return verified;
            }
        }

        protected dynamic curl(string subUri, string method = "GET", string data = "")
        {
            var uri = GetUri(subUri);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Client-ID", ClientId);
            request.Method = method;
            request.Accept = "application/vnd.twitchtv.v3+json";
            if (!string.IsNullOrEmpty(AccessToken))
                request.Headers.Add($"Authorization: OAuth {AccessToken}");
            if (!string.IsNullOrEmpty(data))
            {
                request.ContentType = "application/json; charset=utf-8";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                return JSON.FromString(json);
            }
        }

        public bool SetStreamTitleAndGame(string title, string game = null)
        {
            dynamic result = curl(
                $"channels/{ChannelName}",
                "PUT",
                string.Format("{{" +
                    "\"channel\":{{" +
                    "\"status\":\"{0}\"" +
                    (game == null ? "" : ",\"game\":\"{1}\"") +
                    "}}" +
                "}}", title, game));

            return false;
        }

        public bool VerifyAccessToken()
        {
            dynamic verificationInfo = GetVerificationInfo();
            try
            {
                ChannelName = verificationInfo.token.user_name;
                return verificationInfo.token.valid;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }

        public dynamic SearchGame(string name)
        {
            return curl($"search/games?q={HttpUtility.UrlEncode(name)}&type=suggest");
        }

        public IEnumerable<string> FindGame(string name)
        {
            var result = SearchGame(name);
            var games = (IEnumerable<dynamic>)result.games;

            Func<dynamic, string> func = x => x.name;
            return games.Select(func);
        }

        public dynamic GetVerificationInfo()
        {
            return curl("");
        }

        public Image GetGameBoxArt(string gameName)
        {
            var url = ((IEnumerable<dynamic>)(SearchGame(gameName).games)).First().box.large;
            var request = WebRequest.Create(url);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public bool SubmitRun(
            IRun run,
            string username, string password,
            Func<Image> screenShotFunction = null,
            bool attachSplits = false,
            TimingMethod method = TimingMethod.RealTime,
            string gameId = "", string categoryId = "",
            string version = "", string comment = "",
            string video = "", params string[] additionalParams)
        {
            if (!IsLoggedIn)
            {
                if (!VerifyLogin())
                    return false;
            }

            string game = "";

            try
            {
                var gameList = FindGame(run.GameName);
                if (gameList.Contains(run.GameName))
                    game = run.GameName;
                else
                    game = gameList.First();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                bool resolved = false;

                do
                {
                    try
                    {
                        var dialog = new TwitchGameResolveDialog(run.GameName);
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK)
                        {
                            return false;
                        }
                        else if (dialog.GameName != null)
                        {
                            var gameList = FindGame(dialog.GameName);
                            if (gameList.Contains(dialog.GameName))
                                game = dialog.GameName;
                            else
                                game = gameList.First();
                        }
                        resolved = true;
                    }
                    catch (Exception exc)
                    {
                        Log.Error(exc);
                    }
                } while (!resolved);
            }

            SetStreamTitleAndGame(comment, game);

            return true;
        }
    }
}