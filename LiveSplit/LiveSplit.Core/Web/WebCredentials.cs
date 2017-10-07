using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web
{
    public static class WebCredentials
    {
        public static string TwitterOAuthToken
        {
            get { return CredentialManager.ReadCredential("TwitterOAuthToken")?.Password; }
            set { CredentialManager.WriteCredential("TwitterOAuthToken", "", value); }
        }
        public static string TwitterAccessToken
        {
            get { return CredentialManager.ReadCredential("TwitterAccessToken")?.Password; }
            set { CredentialManager.WriteCredential("TwitterAccessToken", "", value); }
        }
        public static string TwitchAccessToken
        {
            get { return CredentialManager.ReadCredential("TwitchAccessToken")?.Password; }
            set { CredentialManager.WriteCredential("TwitchAccessToken", "", value); }
        }
        public static string SpeedrunComAccessToken
        {
            get { return CredentialManager.ReadCredential("SpeedrunComAccessToken")?.Password; }
            set { CredentialManager.WriteCredential("SpeedrunComAccessToken", "", value); }
        }
        public static SRLCredentials SpeedRunsLiveIRCCredentials
        {
            get
            {
                var credentials = CredentialManager.ReadCredential("SpeedrunComAccessToken");
                return new SRLCredentials(credentials?.UserName, credentials?.Password);
            }
            set { CredentialManager.WriteCredential("SpeedrunComAccessToken", value.Username, value.Password); }
        }
    }

    public struct SRLCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public SRLCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
