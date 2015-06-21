using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class UsersClient
    {
        private SpeedrunComClient baseClient;

        public UsersClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetUsersUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("users{0}", subUri));
        }

        public IEnumerable<User> GetUsers(
            string name = null, string twitch = null,
            string hitbox = null, string speedrunslive = null)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrEmpty(name))
                parameters.Add(string.Format("name={0}", 
                    HttpUtility.UrlPathEncode(name)));

            if (!string.IsNullOrEmpty(twitch))
                parameters.Add(string.Format("twitch={0}",
                    HttpUtility.UrlPathEncode(twitch)));

            if (!string.IsNullOrEmpty(hitbox))
                parameters.Add(string.Format("hitbox={0}",
                    HttpUtility.UrlPathEncode(hitbox)));

            if (!string.IsNullOrEmpty(speedrunslive))
                parameters.Add(string.Format("speedrunslive={0}",
                    HttpUtility.UrlPathEncode(speedrunslive)));

            var uri = GetUsersUri(parameters.ToParameters());
            return SpeedrunComClient.DoPaginatedRequest(uri,
                x => User.Parse(baseClient, x) as User);
        }

        public User GetUser(string userId)
        {
            var uri = GetUsersUri(string.Format("/{0}",
                HttpUtility.UrlPathEncode(userId)));

            var result = JSON.FromUri(uri);

            return User.Parse(baseClient, result.data);
        }
    }
}
