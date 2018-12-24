using LinqToTwitter;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace LiveSplit.Web.Share
{
    public class Twitter : IRunUploadPlatform
    {
        internal const string ConsumerKey = "9oXx7khrRLpQdBjaEUUFw";
        internal const string ConsumerSecret = "KIvv2ZT89ZN1x99f7aUfFwXiwEyU9Am9Z9DYlspX0nU";

        protected static readonly Twitter _Instance = new Twitter();

        public static Twitter Instance => _Instance;

        public TwitterContext Context { get; set; }
        public ISettings Settings { get; set; }

        private string screenName;

        protected Twitter() { }

        public string PlatformName => "Twitter";

        public string Description =>
@"Twitter allows you to share your run with the world. 
When sharing, a screenshot of your splits will automatically 
be included. When you click share, Twitter will ask to 
authenticate with LiveSplit. After the authentication, LiveSplit 
will automatically send the tweet."; 

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

        static ITwitterAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    string result = null;
                    InputBox.Show("Twitter Authentication", 
                        "Enter the PIN number Twitter will give you here: ", ref result);
                    return result;
                }
            };

            return auth;
        }

        static ITwitterAuthorizer DoSingleUserAuth(string accessToken, string oauthToken, out string screenName)
        {
            var auth = new SingleUserAuthorizer
            {
                Credentials = new SingleUserInMemoryCredentials
                {
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret,
                    AccessToken = accessToken,
                    OAuthToken = oauthToken
                }
            };

            auth.Authorize();

            screenName = auth.Credentials.ScreenName;

            return auth;
        }

        static ITwitterAuthorizer DoXAuth(string username, string password)
        {
            var auth = new XAuthAuthorizer
            {
                Credentials = new XAuthCredentials
                {
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret,
                    UserName = username,
                    Password = password
                }
            };

            return auth;
        }

        static ITwitterAuthorizer DoFormOAuth(out string screenName)
        {
            var form = new TwitterOAuthForm();
            form.ShowDialog();

            screenName = ((WebAuthorizer)form.Authorizer).Credentials.ScreenName;

            return form.Authorizer;
        }

        public bool VerifyLogin(string username, string password)
        {
            try
            {
                if (Context != null)
                    return true;

                ITwitterAuthorizer auth;

                ShareSettings.Default.Reload();
                string accessToken = WebCredentials.TwitterAccessToken;
                string oauthToken = WebCredentials.TwitterOAuthToken;

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(oauthToken))
                    auth = DoFormOAuth(out screenName);
                else
                {
                    try
                    {
                        auth = DoSingleUserAuth(accessToken, oauthToken, out screenName);
                        var context = new TwitterContext(auth);
                        context.Trends.FirstOrDefault(x => x.Type == TrendType.Place &&
                                                           x.WoeID == 2486982);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);

                        auth = DoFormOAuth(out screenName);
                    }
                }

                Context = new TwitterContext(auth);
                
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return false;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            if (attachSplits)
                comment += " " + SplitsIO.Instance.Share(run, screenShotFunction);

            if (!VerifyLogin(username, password))
                return false;

            if (screenShotFunction == null || attachSplits)
            {
                var status = Context.UpdateStatus(comment);
                var url = $"http://twitter.com/{status.User.Name}/status/{status.StatusID}";
                Process.Start(url);
            }
            else
            {
                var image = screenShotFunction();
                var media = new Media();

                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    media.ContentType = MediaContentType.Png;
                    media.FileName = "livesplit.png";
                    media.Data = stream.GetBuffer();
                }

                var tweet = Context.TweetWithMedia(comment, false, new List<Media>() { media });
                var url = tweet.Text.Substring(tweet.Text.LastIndexOf("https://"));
                Process.Start(url);
            }

            return true;
        }
    }
}
