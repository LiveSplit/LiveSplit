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

        public static Twitter Instance { get { return _Instance; } }

        public TwitterContext Context { get; set; }
        public ISettings Settings { get; set; }

        private string screenName;

        protected Twitter() { }

        public string PlatformName
        {
            get { return "Twitter"; }
        }

        public string Description
        {
            get 
            {
                return "Twitterを使ってあなたの記録を共有することができます。"
                + "共有時は、スプリットのスクリーンショットもツイート内に含まれます。"
                + "共有ボタンを押した時、TwitterはLiveSplitを認証するかどうか聞いてきます。"
                + "共有後は、Livesplitが自動的にツイートします。"; 
            }
        }

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

        /*static ITwitterAuthorizer DoPinFormOAuth(out String screenName)
        {
            var form = new OAuthForm();
            form.ShowDialog();

            screenName = form.Authorizer.Credentials.ScreenName;

            return form.Authorizer;
        }*/

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
                string accessToken = ShareSettings.Default.TwitterAccessToken;
                string oauthToken = ShareSettings.Default.TwitterOAuthToken;

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
                var url = string.Format("http://twitter.com/{0}/status/{1}", status.User.Name, status.StatusID);
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
                var url = tweet.Text.Substring(tweet.Text.LastIndexOf("http://"));
                //var url = String.Format("http://twitter.com/{0}/status/{1}", url, tweet.StatusID);
                Process.Start(url);
            }

            return true;
        }
    }
}
