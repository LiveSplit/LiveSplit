﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web
{
    public static class WebCredentials
    {
        private const string Twitch = "LiveSplit_TwitchAccessToken";
        private const string SpeedrunCom = "LiveSplit_SpeedrunComAccessToken";
        private const string SpeedRunsLive = "LiveSplit_SpeedRunsLiveIRC";

        public static string TwitchAccessToken
        {
            get { return CredentialManager.ReadCredential(Twitch)?.Password; }
            set { CredentialManager.WriteCredential(Twitch, "", value); }
        }
        public static string SpeedrunComAccessToken
        {
            get { return CredentialManager.ReadCredential(SpeedrunCom)?.Password; }
            set { CredentialManager.WriteCredential(SpeedrunCom, "", value); }
        }
        public static SRLCredentials SpeedRunsLiveIRCCredentials
        {
            get
            {
                var credentials = CredentialManager.ReadCredential(SpeedRunsLive);
                return new SRLCredentials(credentials?.UserName, credentials?.Password);
            }
            set { CredentialManager.WriteCredential(SpeedRunsLive, value.Username, value.Password); }
        }

        public static void DeleteAllCredentials()
        {
            CredentialManager.DeleteCredential(Twitch);
            CredentialManager.DeleteCredential(SpeedrunCom);
            CredentialManager.DeleteCredential(SpeedRunsLive);
        }

        public static bool AnyCredentialsExist()
        {
            return CredentialManager.CredentialExists(Twitch)
                || CredentialManager.CredentialExists(SpeedrunCom)
                || CredentialManager.CredentialExists(SpeedRunsLive);
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
