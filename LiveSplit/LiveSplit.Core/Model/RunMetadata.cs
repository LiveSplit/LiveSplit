using LiveSplit.Model.Comparisons;
using LiveSplit.Web.Share;
using SpeedrunComSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace LiveSplit.Model
{
    public class RunMetadata
    {
        private IRun run;
        private string oldGameName;
        private string oldCategoryName;
        private Game game;
        private Category category;
        private bool usesEmulator;

        public string PlatformID { get; set; }
        public string PlatformName
        {
            get
            {
                if (Platform != null)
                    return Platform.Name;
                return string.Empty;
            }
            set
            {
                var platform = Game.Platforms.FirstOrDefault(x => x.Name == value);
                if (platform == null)
                    PlatformID = string.Empty;
                else
                    PlatformID = platform.ID;
            }
        }
        public Platform Platform { get { return Game.Platforms.FirstOrDefault(x => x.ID == PlatformID); } }

        public string RegionID { get; set; }
        public string RegionName
        {
            get
            {
                if (Region != null)
                    return Region.Name;
                return string.Empty;
            }
            set
            {
                var region = Game.Regions.FirstOrDefault(x => x.Name == value);
                if (region == null)
                    RegionID = string.Empty;
                else
                    RegionID = region.ID;
            }
        }
        public Region Region { get { return Game.Regions.FirstOrDefault(x => x.ID == RegionID); } }

        public IDictionary<string, string> VariableValueIDs { get; set; }
        public IDictionary<string, string> VariableValueNames { get { return VariableValues.ToDictionary(x => x.Key.Name, x => (x.Value != null) ? x.Value.Value : string.Empty); } }
        public IDictionary<Variable, VariableChoice> VariableValues
        {
            get
            {
                var categoryId = Category != null ? Category.ID : null;
                var variables = game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId);
                return variables.ToDictionary(x => x, x => 
                {
                    if (!VariableValueIDs.ContainsKey(x.ID))
                        return null;

                    var variableChoiceID = VariableValueIDs[x.ID];
                    return x.Choices.FirstOrDefault(y => y.ID == variableChoiceID); 
                });
            }
        }

        public bool UsesEmulator
        {
            get
            {
                if (Game == null || !Game.Ruleset.EmulatorsAllowed)
                    return false;
                return usesEmulator;
            }
            set
            {
                usesEmulator = Game != null && Game.Ruleset.EmulatorsAllowed && value;
            }
        }

        public Game Game
        {
            get
            {
                if (run.GameName != oldGameName)
                {
                    game = SpeedrunCom.Client.Games.SearchGameExact(run.GameName, new GameEmbeds(embedRegions: true, embedPlatforms: true));
                    oldGameName = run.GameName;
                    oldCategoryName = null;
                }
                return game;
            }
        }

        public Category Category
        {
            get
            {
                if (run.CategoryName != oldCategoryName)
                {
                    category = SpeedrunCom.Client.Games.GetCategories(Game.ID, embeds: new CategoryEmbeds(embedVariables: true))
                        .FirstOrDefault(x => x.Type == CategoryType.PerGame && x.Name == run.CategoryName);
                    oldCategoryName = run.CategoryName;
                }
                return category;
            }
        }

        public RunMetadata(IRun run)
        {
            this.run = run;
            VariableValueIDs = new Dictionary<string, string>();
        }

        public RunMetadata Clone(IRun run)
        {
            return new RunMetadata(run)
            {
                oldGameName = oldGameName,
                oldCategoryName = oldCategoryName,
                game = game,
                category = category,
                PlatformID = PlatformID,
                RegionID = RegionID,
                VariableValueIDs = VariableValueIDs.ToDictionary(x => x.Key, x => x.Value),
                usesEmulator = usesEmulator
            };
        }
    }
}
