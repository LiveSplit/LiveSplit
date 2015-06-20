using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Game
    {
        public GameHeader Header { get; private set; }
        public int ID { get { return Header.ID; } }
        public string Name { get { return Header.Name; } }
        public string JapaneseName { get { return Header.JapaneseName; } }
        public string Abbreviation { get { return Header.Abbreviation; } }
        public Uri WebLink { get { return Header.WebLink; } }
        public int? YearOfRelease { get; private set; }
        public Ruleset Ruleset { get; private set; }
        public DateTime? CreationDate { get; private set; }

        #region Embeds

        private Lazy<ReadOnlyCollection<User>> moderatorUsers;
        private Lazy<ReadOnlyCollection<Platform>> platforms;
        private Lazy<ReadOnlyCollection<Region>> regions;

        /// <summary>
        /// null when embedded
        /// </summary>
        public ReadOnlyCollection<int> PlatformIDs { get; private set; }
        /// <summary>
        /// null when embedded
        /// </summary>
        public ReadOnlyCollection<int> RegionIDs { get; private set; }
        /// <summary>
        /// null when embedded
        /// </summary>
        public ReadOnlyCollection<Moderator> Moderators { get; private set; }

        public ReadOnlyCollection<User> ModeratorUsers { get { return moderatorUsers.Value; } }
        public ReadOnlyCollection<Platform> Platforms { get { return platforms.Value; } }
        public ReadOnlyCollection<Region> Regions { get { return regions.Value; } }

        #endregion

        #region Links

        private Lazy<ReadOnlyCollection<Run>> runs;
        private Lazy<ReadOnlyCollection<Level>> levels;
        private Lazy<ReadOnlyCollection<Category>> categories;
        private Lazy<ReadOnlyCollection<Variable>> variables;
        private Lazy<Game> parent;
        private Lazy<ReadOnlyCollection<Game>> children;

        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }
        public ReadOnlyCollection<Level> Levels { get { return levels.Value; } }
        public ReadOnlyCollection<Category> Categories { get { return categories.Value; } }
        public ReadOnlyCollection<Variable> Variables { get { return variables.Value; } }
        public int? ParentGameID { get; private set; }
        public Game Parent { get { return parent.Value; } }
        public ReadOnlyCollection<Game> Children { get { return children.Value; } }

        #endregion

        private Game() { }

        public static Game Parse(SpeedrunComClient client, dynamic gameElement)
        {
            var game = new Game();

            //Parse Attributes

            game.Header = GameHeader.Parse(client, gameElement);
            game.YearOfRelease = gameElement.released;
            game.Ruleset = Ruleset.Parse(client, gameElement.ruleset);

            var created = gameElement.created as string;
            if (!string.IsNullOrEmpty(created))
                game.CreationDate = DateTime.Parse(created);

            //Parse Embeds

            var properties = gameElement.Properties as IDictionary<string, dynamic>;

            if (properties["moderators"] is IDictionary<string, dynamic> && properties["moderators"].data is IEnumerable<dynamic>)
            {
                var userElements = properties["moderators"].data as IEnumerable<dynamic>;
                var users = userElements.Select(x => User.Parse(client, x) as User).ToList().AsReadOnly();
                game.moderatorUsers = new Lazy<ReadOnlyCollection<User>>(() => users);
            }
            else if (gameElement.moderators is DynamicJsonObject)
            {
                var moderatorsProperties = gameElement.moderators.Properties as IDictionary<string, dynamic>;
                game.Moderators = moderatorsProperties.Select(x => Moderator.Parse(client, x)).ToList().AsReadOnly();
                game.moderatorUsers = new Lazy<ReadOnlyCollection<User>>(
                    () => game.Moderators.Select(x => client.Users.GetUser(x.UserID)).ToList().AsReadOnly());
            }
            else
            {
                game.Moderators = new ReadOnlyCollection<Moderator>(new Moderator[0]);
                game.moderatorUsers = new Lazy<ReadOnlyCollection<User>>(() => new List<User>().AsReadOnly());
            }

            if (properties["platforms"] is IDictionary<string, dynamic> && properties["platforms"].data is IEnumerable<dynamic>)
            {
                var platformElements = properties["platforms"].data as IEnumerable<dynamic>;
                var platforms = platformElements.Select(x => Platform.Parse(client, x) as Platform).ToList().AsReadOnly();
                game.platforms = new Lazy<ReadOnlyCollection<Platform>>(() => platforms);
            }
            else
            {
                game.PlatformIDs = SpeedrunComClient.ParseCollection<int>(gameElement.platforms);
                game.platforms = new Lazy<ReadOnlyCollection<Platform>>(
                    () => game.PlatformIDs.Select(x => client.Platforms.GetPlatform(x)).ToList().AsReadOnly());
            }

            if (properties["regions"] is IDictionary<string, dynamic> && properties["regions"].data is IEnumerable<dynamic>)
            {
                var regionElements = properties["regions"].data as IEnumerable<dynamic>;
                var regions = regionElements.Select(x => Region.Parse(client, x) as Region).ToList().AsReadOnly();
                game.regions = new Lazy<ReadOnlyCollection<Region>>(() => regions);
            }
            else
            {
                game.RegionIDs = SpeedrunComClient.ParseCollection<int>(gameElement.regions);
                game.regions = new Lazy<ReadOnlyCollection<Region>>(
                    () => game.RegionIDs.Select(x => client.Regions.GetRegion(x)).ToList().AsReadOnly());
            }

            //Parse Links

            game.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(gameId: game.ID));

            if (properties.ContainsKey("levels"))
            {
                var levelElements = properties["levels"].data as IEnumerable<dynamic>;
                var levels = levelElements.Select(x => Level.Parse(client, x) as Level).ToList().AsReadOnly();
                game.levels = new Lazy<ReadOnlyCollection<Level>>(() => levels);
            }
            else
            {
                game.levels = new Lazy<ReadOnlyCollection<Level>>(() => client.Games.GetLevels(game.ID));
            }

            if (properties.ContainsKey("categories"))
            {
                var categoryElements = properties["categories"].data as IEnumerable<dynamic>;
                var categories = categoryElements.Select(x => Category.Parse(client, x) as Category).ToList().AsReadOnly();
                game.categories = new Lazy<ReadOnlyCollection<Category>>(() => categories);
            }
            else
            {
                game.categories = new Lazy<ReadOnlyCollection<Category>>(() => client.Games.GetCategories(game.ID));
            }

            if (properties.ContainsKey("variables"))
            {
                var variableElements = properties["variables"].data as IEnumerable<dynamic>;
                var variables = variableElements.Select(x => Variable.Parse(client, x) as Variable).ToList().AsReadOnly();
                game.variables = new Lazy<ReadOnlyCollection<Variable>>(() => variables);
            }
            else
            {
                game.variables = new Lazy<ReadOnlyCollection<Variable>>(() => client.Games.GetVariables(game.ID));
            }

            var links = properties["links"] as IEnumerable<dynamic>;
            var parentLink = links.FirstOrDefault(x => x.rel == "parent");
            if (parentLink != null)
            {
                var parentUri = parentLink.uri as string;
                game.ParentGameID = Convert.ToInt32(parentUri.Substring(parentUri.LastIndexOf('/') + 1), CultureInfo.InvariantCulture);
                game.parent = new Lazy<Game>(() => client.Games.GetGame(game.ParentGameID.Value));
            }
            else
            {
                game.parent = new Lazy<Game>(() => null);
            }

            game.children = new Lazy<ReadOnlyCollection<Game>>(() => client.Games.GetChildren(game.ID));
                 
            return game;
        }
    }
}
