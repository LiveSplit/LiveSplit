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

        protected Twitter() { }

        public string PlatformName
        {
            get { return "Twitter"; }
        }

        public string Description
        {
            get 
            { 
                return "Twitter allows you to share your run with the world. "
                + "When sharing, a screenshot of your splits will automatically "
                + "be included. When you click share, Twitter will ask to "
                + "authenticate with LiveSplit. After the authentication, LiveSplit "
                + "will automatically send the tweet."; 
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

        public bool VerifyLogin(string username, string password)
        {
            try
            {
                ShareSettings.Default.Reload();

                string oauthToken = ShareSettings.Default.TwitterOAuthToken;
                string accessToken = ShareSettings.Default.TwitterAccessToken;

                PinAuthorizer authorizer = new PinAuthorizer
                {
                    Credentials = new InMemoryCredentials
                    {
                        ConsumerKey = Twitter.ConsumerKey,
                        ConsumerSecret = Twitter.ConsumerSecret,
                        OAuthToken = oauthToken,
                        AccessToken = accessToken
                    },
                    GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                    GetPin = () =>
                    {
                        string result = null;

                        InputBox.Show("Twitter Authentication", "Enter the PIN number Twitter will give you", ref result);

                        return result;
                    }
                };

                Context = new TwitterContext(authorizer);

                try
                {
                    // Verify current credentials
                    // If an error occurs during the credential verification (credentials expired), an exception will be triggered
                    Account account = (from acct in Context.Account where acct.Type == AccountType.VerifyCredentials select acct).SingleOrDefault();

                    return true;
                }
                catch
                {
                    // Reset expired credentials
                    authorizer.Credentials.OAuthToken = string.Empty;
                    authorizer.Credentials.AccessToken = string.Empty;

                    // Begin authorization
                    authorizer.Authorize();

                    // Save new credentials
                    ShareSettings.Default.TwitterOAuthToken = authorizer.Credentials.OAuthToken;
                    ShareSettings.Default.TwitterAccessToken = authorizer.Credentials.AccessToken;

                    ShareSettings.Default.Save();

                    return true;
                }
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

            Status status;

            if (screenShotFunction == null || attachSplits)
            {
                status = Context.UpdateStatus(comment);
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

                status = Context.TweetWithMedia(comment, false, new List<Media>() { media });
            }

            var url = String.Format("http://twitter.com/{0}/status/{1}", status.User.Identifier.ScreenName, status.StatusID);

            Process.Start(url);

            return true;
        }
    }
}
