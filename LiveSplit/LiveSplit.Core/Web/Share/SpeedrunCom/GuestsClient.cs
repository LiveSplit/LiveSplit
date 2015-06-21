using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class GuestsClient
    {
        private SpeedrunComClient baseClient;

        public GuestsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetGuestsUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("guests{0}", subUri));
        }

        public Guest GetGuest(string guestName)
        {
            var uri = GetGuestsUri(string.Format("/{0}", HttpUtility.UrlPathEncode(guestName)));
            var result = JSON.FromUri(uri);

            return Guest.Parse(baseClient, result.data);
        }
    }
}
