using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

using LiveSplit.UI;

namespace LiveSplit.Web.Share;

public class TwitchAccessTokenPrompt
{
    internal const string RedirectUrl = "http://livesplit.org/twitch/";

    public static string GetAccessToken()
    {
        string oauthUrl = string.Format(
            "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={0}&redirect_uri={1}&scope={2}",
            Twitch.ClientId,
            RedirectUrl,
            "channel:manage:broadcast");

        Process.Start(oauthUrl);

        string urlWithToken = null;
        System.Windows.Forms.DialogResult result = InputBox.Show("Twitch Authentication", "After completing the OAuth flow and being redirected to a GitHub Pages 404 error page, copy the full URL from the address bar of your browser:", ref urlWithToken);

        if (result == System.Windows.Forms.DialogResult.Cancel)
        {
            return null;
        }

        try
        {
            Match match = Regex.Match(urlWithToken, ".*access_token=(\\w+).+");

            return match.Groups[1].Value;
        }
        catch
        {
            throw new Exception("Could not retrieve access token from URL!");
        }
    }
}
