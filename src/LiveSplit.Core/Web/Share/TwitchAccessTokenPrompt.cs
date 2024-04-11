using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public class TwitchAccessTokenPrompt
    {
        internal const string RedirectUrl = "http://livesplit.org/twitch/";

        public static string GetAccessToken()
        {
            var oauthUrl = string.Format(
                "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}&scope={2}",
                Twitch.ClientId,
                RedirectUrl,
                "channel:manage:broadcast");

            Process.Start(oauthUrl);

            string urlWithToken = null;
            InputBox.Show("Twitch Authentication", "After completing the OAuth flow and being redirected to a GitHub Pages 404 error page, copy the full URL from the address bar of your browser:", ref urlWithToken);

            try
            {
                var match = Regex.Match(urlWithToken, ".*access_token=(\\w+).+");

                return match.Groups[1].Value;
            }
            catch
            {
                throw new Exception("Could not retrieve access token from URL!");
            }
        }
    }
}
