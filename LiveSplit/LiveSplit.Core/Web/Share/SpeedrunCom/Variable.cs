using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Variable
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public VariableScope Scope { get; private set; }
        public bool IsMandatory { get; private set; }
        public bool IsUserDefined { get; private set; }
        public bool IsUsedForObsoletingRuns { get; private set; }
        public ReadOnlyCollection<VariableChoice> Choices { get; private set; }
        public VariableChoice DefaultChoice { get; private set; }

        #region Links

        private Lazy<Game> game;
        private Lazy<Category> category;
        private Lazy<Level> level;

        public string GameID { get; private set; }
        public Game Game { get { return game.Value; } }
        public string CategoryID { get; private set; }
        public Category Category { get { return category.Value; } }
        public Level Level { get { return level.Value; } }

        #endregion

        private Variable() { }

        public static Variable Parse(SpeedrunComClient client, dynamic variableElement)
        {
            var variable = new Variable();

            var properties = variableElement.Properties as IDictionary<string, dynamic>;
            var links = properties["links"] as IEnumerable<dynamic>;

            //Parse Attributes

            variable.ID = variableElement.id as string;
            variable.Name = variableElement.name as string;
            variable.Scope = VariableScope.Parse(client, variableElement.scope) as VariableScope;
            variable.IsMandatory = (bool)variableElement.mandatory;
            variable.IsUserDefined = (bool)properties["user-defined"];
            variable.IsUsedForObsoletingRuns = (bool)variableElement.obsoletes;

            var choiceElements = variableElement.values.choices.Properties as IDictionary<string, dynamic>;
            variable.Choices = choiceElements.Select(x => VariableChoice.Parse(client, x) as VariableChoice).ToList().AsReadOnly();

            var valuesProperties = variableElement.values.Properties as IDictionary<string, dynamic>;
            var defaultChoice = valuesProperties["default"] as string;
            if (!string.IsNullOrEmpty(defaultChoice))
                variable.DefaultChoice = variable.Choices.First(x => x.ID == defaultChoice);

            //Parse Links

            var gameLink = links.FirstOrDefault(x => x.rel == "game");
            if (gameLink != null)
            {
                var gameUri = gameLink.uri as string;
                variable.GameID = gameUri.Substring(gameUri.LastIndexOf("/") + 1);
                variable.game = new Lazy<Game>(() => client.Games.GetGame(variable.GameID));
            }
            else
            {
                variable.game = new Lazy<Game>(() => null);
            }

            variable.CategoryID = variableElement.category as string;
            if (!string.IsNullOrEmpty(variable.CategoryID))
            {
                variable.category = new Lazy<Category>(() => client.Categories.GetCategory(variable.CategoryID));
            }
            else
            {
                variable.category = new Lazy<Category>(() => null);
            }

            if (!string.IsNullOrEmpty(variable.Scope.LevelID))
            {
                variable.level = new Lazy<Level>(() => client.Levels.GetLevel(variable.Scope.LevelID));
            }
            else
            {
                variable.level = new Lazy<Level>(() => null);
            }

            return variable;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
