using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Category
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public CategoryType Type { get; private set; }
        public string Rules { get; private set; }
        public Players Players { get; private set; }
        public bool IsMiscellaneous { get; private set; }

        #region Links

        private Lazy<Game> game;
        private Lazy<ReadOnlyCollection<Variable>> variables;
        private Lazy<ReadOnlyCollection<Run>> runs;

        public string GameID { get; private set; }
        public Game Game { get { return game.Value; } }
        public ReadOnlyCollection<Variable> Variables { get { return variables.Value; } }
        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }

        #endregion

        private Category() { }

        public static Category Parse(SpeedrunComClient client, dynamic categoryElement)
        {
            var category = new Category();

            //Parse Attributes

            category.ID = categoryElement.id as string;
            category.Name = categoryElement.name as string;
            category.Type = categoryElement.type == "per-game" ? CategoryType.PerGame : CategoryType.PerLevel;
            category.Rules = categoryElement.rules as string;
            category.Players = Players.Parse(client, categoryElement.players);
            category.IsMiscellaneous = categoryElement.miscellaneous;

            //Parse Links

            var properties = categoryElement.Properties as IDictionary<string, dynamic>;
            var links = properties["links"] as IEnumerable<dynamic>;

            var gameUri = links.First(x => x.rel == "game").uri as string;
            category.GameID = gameUri.Substring(gameUri.LastIndexOf('/') + 1);

            if (properties.ContainsKey("game"))
            {
                var gameElement = properties["game"].data;
                var game = Game.Parse(client, gameElement) as Game;
                category.game = new Lazy<Game>(() => game);
            }
            else
            {
                category.game = new Lazy<Game>(() => client.Games.GetGame(category.GameID));
            }

            if (properties.ContainsKey("variables"))
            {
                var variableElements = properties["variables"].data as IEnumerable<dynamic>;
                var variables = variableElements.Select(x => Variable.Parse(client, x) as Variable).ToList().AsReadOnly();
                category.variables = new Lazy<ReadOnlyCollection<Variable>>(() => variables);
            }
            else
            {
                category.variables = new Lazy<ReadOnlyCollection<Variable>>(() => client.Categories.GetVariables(category.ID));
            }

            category.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(categoryId: category.ID).ToList().AsReadOnly());

            return category;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
