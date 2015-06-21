using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Level
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public Uri WebLink { get; private set; }
        public string Rules { get; private set; }

        #region Links

        private Lazy<Game> game;
        private Lazy<ReadOnlyCollection<Category>> categories;
        private Lazy<ReadOnlyCollection<Variable>> variables;
        private Lazy<ReadOnlyCollection<Run>> runs;

        public string GameID { get; private set; }
        public Game Game { get { return game.Value; } }
        public ReadOnlyCollection<Category> Categories { get { return categories.Value; } }
        public ReadOnlyCollection<Variable> Variables { get { return variables.Value; } }
        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }

        #endregion

        private Level() { }

        public static Level Parse(SpeedrunComClient client, dynamic levelElement)
        {
            var level = new Level();

            //Parse Attributes

            level.ID = levelElement.id as string;
            level.Name = levelElement.name as string;
            level.WebLink = new Uri(levelElement.weblink as string);
            level.Rules = levelElement.rules;

            //Parse Links

            var properties = levelElement.Properties as IDictionary<string, dynamic>;
            var links = properties["links"] as IEnumerable<dynamic>;

            var gameUri = links.First(x => x.rel == "game").uri as string;
            level.GameID = gameUri.Substring(gameUri.LastIndexOf('/') + 1);
            level.game = new Lazy<Game>(() => client.Games.GetGame(level.GameID));

            if (properties.ContainsKey("categories"))
            {
                var categoryElements = properties["categories"].data as IEnumerable<dynamic>;
                var categories = categoryElements.Select(x => Category.Parse(client, x) as Category).ToList().AsReadOnly();
                level.categories = new Lazy<ReadOnlyCollection<Category>>(() => categories);
            }
            else
            {
                level.categories = new Lazy<ReadOnlyCollection<Category>>(() => client.Levels.GetCategories(level.ID));
            }

            if (properties.ContainsKey("variables"))
            {
                var variableElements = properties["variables"].data as IEnumerable<dynamic>;
                var variables = variableElements.Select(x => Variable.Parse(client, x) as Variable).ToList().AsReadOnly();
                level.variables = new Lazy<ReadOnlyCollection<Variable>>(() => variables);
            }
            else
            {
                level.variables = new Lazy<ReadOnlyCollection<Variable>>(() => client.Levels.GetVariables(level.ID));
            }

            level.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(levelId: level.ID).ToList().AsReadOnly());

            return level;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
