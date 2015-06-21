using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Run
    {
        public string ID { get; private set; }
        public Uri WebLink { get; private set; }
        public string GameID { get; private set; }
        public string LevelID { get; private set; }
        public string CategoryID { get; private set; }
        public Uri Video { get; private set; }
        public string Comment { get; private set; }
        public RunStatus Status { get; private set; }
        public ReadOnlyCollection<Player> Players { get; private set; }
        public DateTime? Date { get; private set; }
        public DateTime? DateSubmitted { get; private set; }
        public RunTimes Times { get; private set; }
        public RunSystem System { get; private set; }
        public ReadOnlyCollection<VariableValue> VariableValues { get; private set; }

        #region Links

        private Lazy<Game> game;
        private Lazy<Category> category;
        private Lazy<Level> level;
        private Lazy<Platform> platform;
        private Lazy<Region> region;
        private Lazy<User> examiner;

        public Game Game { get { return game.Value; } }
        public Category Category { get { return category.Value; } }
        public Level Level { get { return level.Value; } }
        public Platform Platform { get { return platform.Value; } }
        public Region Region { get { return region.Value; } }
        public User Examiner { get { return examiner.Value; } }
        public Uri SplitsIOUri { get; private set; }

        #endregion

        private Run() { }

        internal static Run Parse(SpeedrunComClient client, dynamic runElement)
        {
            var run = new Run();

            //Parse Attributes

            run.ID = runElement.id as string;
            run.WebLink = new Uri(runElement.weblink as string);
            run.LevelID = runElement.level as string;
            run.CategoryID = runElement.category as string;

            var videoUri = runElement.video as string;
            if (!string.IsNullOrEmpty(videoUri))
            {
                if (!videoUri.StartsWith("http"))
                    videoUri = "http://" + videoUri;

                if (Uri.IsWellFormedUriString(videoUri, UriKind.Absolute))
                    run.Video = new Uri(videoUri);
            }

            run.Comment = runElement.comment as string;
            run.Status = RunStatus.Parse(client, runElement.status) as RunStatus;

            var playerElements = runElement.players as IEnumerable<dynamic>;
            run.Players = playerElements.Select(x => Player.Parse(client, x) as Player).ToList().AsReadOnly();

            var runDate = runElement.date;
            if (!string.IsNullOrEmpty(runDate))
                run.Date = DateTime.Parse(runDate, CultureInfo.InvariantCulture);

            var dateSubmitted = runElement.submitted;
            if (!string.IsNullOrEmpty(dateSubmitted))
                run.DateSubmitted = DateTime.Parse(dateSubmitted, CultureInfo.InvariantCulture);

            run.Times = RunTimes.Parse(client, runElement.times) as RunTimes;
            run.System = RunSystem.Parse(client, runElement.system) as RunSystem;

            if (runElement.values is DynamicJsonObject)
            {
                var valueProperties = runElement.values.Properties as IDictionary<string, dynamic>;
                run.VariableValues = valueProperties.Select(x => VariableValue.Parse(client, x) as VariableValue).ToList().AsReadOnly();
            }
            else
            {
                run.VariableValues = new List<VariableValue>().AsReadOnly();
            }

            //Parse Links

            var properties = runElement.Properties as IDictionary<string, dynamic>;
            var links = properties["links"] as IEnumerable<dynamic>;

            if (properties["game"] is string)
            {
                run.GameID = runElement.game as string;
                run.game = new Lazy<Game>(() => client.Games.GetGame(run.GameID));
            }
            else
            {
                var game = Game.Parse(client, properties["game"].data) as Game;
                run.game = new Lazy<Game>(() => game);
                run.GameID = game.ID;
            }
            

            if (!string.IsNullOrEmpty(run.CategoryID))
            {
                run.category = new Lazy<Category>(() => client.Categories.GetCategory(run.CategoryID));
            }
            else
            {
                run.category = new Lazy<Category>(() => null);
            }

            if (!string.IsNullOrEmpty(run.LevelID))
            {
                run.level = new Lazy<Level>(() => client.Levels.GetLevel(run.LevelID));
            }
            else
            {
                run.level = new Lazy<Level>(() => null);
            }

            if (!string.IsNullOrEmpty(run.System.PlatformID))
            {
                run.platform = new Lazy<Platform>(() => client.Platforms.GetPlatform(run.System.PlatformID));
            }
            else
            {
                run.platform = new Lazy<Platform>(() => null);
            }

            if (!string.IsNullOrEmpty(run.System.RegionID))
            {
                run.region = new Lazy<Region>(() => client.Regions.GetRegion(run.System.RegionID));
            }
            else
            {
                run.region = new Lazy<Region>(() => null);
            }

            if (!string.IsNullOrEmpty(run.Status.ExaminerUserID))
            {
                run.examiner = new Lazy<User>(() => client.Users.GetUser(run.Status.ExaminerUserID));
            }
            else
            {
                run.examiner = new Lazy<User>(() => null);
            }

            var splitsIOLink = links.FirstOrDefault(x => x.rel == "splits");
            if (splitsIOLink != null)
            {
                run.SplitsIOUri = new Uri(splitsIOLink.uri as string);
            }

            return run;
        }

        public override string ToString()
        {
            return new[] { GameID, Times.ToString() }.Aggregate((a, b) => a + " " + b);
        }
    }
}
