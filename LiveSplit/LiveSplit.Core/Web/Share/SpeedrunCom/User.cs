using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class User
    {
        public string ID { get; private set; }
        public Uri WebLink { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }
        public UserNameStyle NameStyle { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime? SignUpDate { get; private set; }
        public Location Location { get; private set; }

        #region Links

        private Lazy<ReadOnlyCollection<Run>> runs;
        private Lazy<ReadOnlyCollection<Game>> moderatedGames;

        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }
        public ReadOnlyCollection<Game> ModeratedGames { get { return moderatedGames.Value; } }

        public Uri TwitchProfile { get; private set; }
        public Uri HitboxProfile { get; private set; }
        public Uri YoutubeProfile { get; private set; }
        public Uri TwitterProfile { get; private set; }
        public Uri SpeedRunsLiveProfile { get; private set; }

        #endregion

        private User() { }

        private static UserRole parseUserRole(string role)
        {
            switch (role)
            {
                case "banned":
                    return UserRole.Banned;
                case "user":
                    return UserRole.User;
                case "trusted":
                    return UserRole.Trusted;
                case "moderator":
                    return UserRole.Moderator;
                case "admin":
                    return UserRole.Admin;
                case "programmer":
                    return UserRole.Programmer;
            }

            throw new ArgumentException("role");
        }

        public static User Parse(SpeedrunComClient client, dynamic userElement)
        {
            var user = new User();

            var properties = userElement.Properties as IDictionary<string, dynamic>;
            var links = properties["links"] as IEnumerable<dynamic>;

            //Parse Attributes

            user.ID = userElement.id as string;
            user.WebLink = new Uri(userElement.weblink as string);
            user.Name = userElement.names.international as string;
            user.JapaneseName = userElement.names.japanese as string;
            user.NameStyle = UserNameStyle.Parse(client, properties["name-style"]) as UserNameStyle;
            user.Role = parseUserRole(userElement.role as string);

            var signUpDate = userElement.signup as string;
            if (!string.IsNullOrEmpty(signUpDate))
                user.SignUpDate = DateTime.Parse(signUpDate, CultureInfo.InvariantCulture);

            user.Location = Location.Parse(client, userElement.location) as Location;

            //Parse Links

            user.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(userId: user.ID).ToList().AsReadOnly());
            user.moderatedGames = new Lazy<ReadOnlyCollection<Game>>(() => client.Games.GetGames(moderatorId: user.ID).ToList().AsReadOnly());

            var twitchLink = links.FirstOrDefault(x => x.rel == "twitch");
            if (twitchLink != null)
                user.TwitchProfile = new Uri(twitchLink.uri as string);

            var hitboxLink = links.FirstOrDefault(x => x.rel == "hitbox");
            if (hitboxLink != null)
                user.HitboxProfile = new Uri(hitboxLink.uri as string);

            var youtubeLink = links.FirstOrDefault(x => x.rel == "youtube");
            if (youtubeLink != null)
                user.YoutubeProfile = new Uri(youtubeLink.uri as string);

            var twitterLink = links.FirstOrDefault(x => x.rel == "twitter");
            if (twitterLink != null)
                user.TwitterProfile = new Uri(twitterLink.uri as string);

            var speedRunsLiveLink = links.FirstOrDefault(x => x.rel == "speedrunslive");
            if (speedRunsLiveLink != null)
                user.SpeedRunsLiveProfile = new Uri(speedRunsLiveLink.uri as string);

            return user;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
