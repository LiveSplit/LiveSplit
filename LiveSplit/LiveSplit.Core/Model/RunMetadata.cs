using LiveSplit.Model.Comparisons;
using LiveSplit.Web.Share;
using SpeedrunComSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace LiveSplit.Model
{
    public class RunMetadata
    {
        public IRun LiveSplitRun { get; private set; }
        private string oldGameName;
        private string oldCategoryName;
        private Lazy<Game> game;
        private Lazy<Category> category;
        private Lazy<SpeedrunComSharp.Run> run;
        private bool usesEmulator;
        private string runId;

        public event EventHandler PropertyChanged;

        public string RunID
        {
            get
            {
                return runId;
            }
            set
            {
                runId = value;
                run = new Lazy<SpeedrunComSharp.Run>(() => SpeedrunCom.Client.Runs.GetRun(runId));

                if (value != null)
                    TriggerPropertyChanged(false);
            }
        }
        public SpeedrunComSharp.Run Run
        {
            get
            {
                return run.Value;
            }
            set
            {
                RunID = value.ID;
            }
        }

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
                if (Game == null)
                    return;

                var platform = Game.Platforms.FirstOrDefault(x => x.Name == value);
                if (platform == null)
                    PlatformID = string.Empty;
                else
                    PlatformID = platform.ID;

                TriggerPropertyChanged(true);
            }
        }
        public Platform Platform
        {
            get
            {
                if (Game == null)
                    return null;

                return Game.Platforms.FirstOrDefault(x => x.ID == PlatformID);
            }
        }

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
                if (Game == null)
                    return;

                var region = Game.Regions.FirstOrDefault(x => x.Name == value);
                if (region == null)
                    RegionID = string.Empty;
                else
                    RegionID = region.ID;

                TriggerPropertyChanged(true);
            }
        }
        public Region Region
        {
            get
            {
                if (Game == null)
                    return null;

                return Game.Regions.FirstOrDefault(x => x.ID == RegionID);
            }
        }

        public IDictionary<string, string> VariableValueIDs { get; set; }
        public IDictionary<string, string> VariableValueNames { get { return VariableValues.ToDictionary(x => x.Key.Name, x => (x.Value != null) ? x.Value.Value : string.Empty); } }
        public IDictionary<Variable, VariableChoice> VariableValues
        {
            get
            {
                if (Game == null)
                    return new Dictionary<Variable, VariableChoice>();

                var categoryId = Category != null ? Category.ID : null;
                var variables = Game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId);
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

                TriggerPropertyChanged(true);
            }
        }

        public Game Game
        {
            get
            {
                Refresh();
                return game.Value;
            }
        }

        public Category Category
        {
            get
            {
                Refresh();
                return category.Value;
            }
        }

        public RunMetadata(IRun run)
        {
            this.LiveSplitRun = run;
            VariableValueIDs = new Dictionary<string, string>();
            game = new Lazy<Game>(() => null);
            category = new Lazy<Category>(() => null);
        }

        public void Refresh()
        {
            lock (this)
            {
                if (LiveSplitRun.GameName != oldGameName)
                {
                    oldGameName = LiveSplitRun.GameName;
                    if (!string.IsNullOrEmpty(LiveSplitRun.GameName))
                    {
                        var gameTask = Task.Factory.StartNew(() => SpeedrunCom.Client.Games.SearchGameExact(LiveSplitRun.GameName, new GameEmbeds(embedRegions: true, embedPlatforms: true)));
                        game = new Lazy<Game>(() => gameTask.Result);
                    }
                    else
                        game = new Lazy<Game>(() => null);
                    oldCategoryName = null;
                }

                if (LiveSplitRun.CategoryName != oldCategoryName)
                {
                    oldCategoryName = LiveSplitRun.CategoryName;
                    if (!string.IsNullOrEmpty(LiveSplitRun.CategoryName))
                    {
                        var categoryTask = Task.Factory.StartNew(() => 
                        {
                            if (Game == null)
                                return null;

                            return SpeedrunCom.Client.Games.GetCategories(Game.ID, embeds: new CategoryEmbeds(embedVariables: true))
                                .FirstOrDefault(x => x.Type == CategoryType.PerGame && x.Name == LiveSplitRun.CategoryName);
                        });
                        category = new Lazy<Category>(() => categoryTask.Result);
                    }
                    else
                        category = new Lazy<Category>(() => null);
                }
            }
        }

        public RunMetadata Clone(IRun run)
        {
            return new RunMetadata(run)
            {
                oldGameName = oldGameName,
                oldCategoryName = oldCategoryName,
                game = game,
                category = category,
                run = this.run,
                runId = runId,
                PlatformID = PlatformID,
                RegionID = RegionID,
                VariableValueIDs = VariableValueIDs.ToDictionary(x => x.Key, x => x.Value),
                usesEmulator = usesEmulator
            };
        }

        private void TriggerPropertyChanged(bool clearRunID)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new MetadataChangedEventArgs(clearRunID));
        }
    }
}
