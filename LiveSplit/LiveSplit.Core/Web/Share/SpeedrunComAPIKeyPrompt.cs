using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public class SpeedrunComApiKeyPrompt : ISpeedrunComAuthenticator
    {
        public string GetAccessToken()
        {
            Process.Start("https://www.speedrun.com/settings/api");

            string accessToken = null;
            InputBox.Show("Speedrun.com Authentication", "Enter your Speedrun.com API Key:", ref accessToken);
            return accessToken;
        }
    }
}
