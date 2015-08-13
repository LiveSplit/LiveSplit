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
        private string runId;
        private string platformName;
        private string regionName;
        private bool usesEmulator;

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
                runId = value.ID;
                run = new Lazy<SpeedrunComSharp.Run>(() => value);

                if (value != null)
                    TriggerPropertyChanged(false);
            }
        }

        public string PlatformName
        {
            get
            {
                return platformName;
            }
            set
            {
                platformName = value; 
                TriggerPropertyChanged(true);
            }
        }
        public Platform Platform
        {
            get
            {
                if (Game == null)
                    return null;

                return Game.Platforms.FirstOrDefault(x => x.Name == PlatformName);
            }
        }

        public string RegionName
        {
            get
            {
                return regionName;
            }
            set
            {
                regionName = value;
                TriggerPropertyChanged(true);
            }
        }
        public Region Region
        {
            get
            {
                if (Game == null)
                    return null;

                return Game.Regions.FirstOrDefault(x => x.Name == RegionName);
            }
        }

        public IDictionary<string, string> VariableValueNames { get; set; }
        public IDictionary<Variable, VariableValue> VariableValues
        {
            get
            {
                if (Game == null)
                    return new Dictionary<Variable, VariableValue>();

                var categoryId = Category != null ? Category.ID : null;
                var variables = Game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId);
                return variables.ToDictionary(x => x, x =>
                {
                    if (!VariableValueNames.ContainsKey(x.Name))
                        return null;

                    var variableValue = VariableValueNames[x.Name];
                    var foundValue = x.Values.FirstOrDefault(y => y.Value == variableValue);

                    if (foundValue == null && x.IsUserDefined)
                    {
                        foundValue = x.CreateCustomValue(variableValue);
                    }

                    return foundValue;
                });
            }
        }

        public bool UsesEmulator
        {
            get
            {
                return usesEmulator;
            }
            set
            {
                usesEmulator = value;
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
            VariableValueNames = new Dictionary<string, string>();
            game = new Lazy<Game>(() => null);
            category = new Lazy<Category>(() => null);
            this.run = new Lazy<SpeedrunComSharp.Run>(() => null);
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
                        var gameTask = Task.Factory.StartNew(() => SpeedrunCom.Client.Games.SearchGameExact(oldGameName, new GameEmbeds(embedRegions: true, embedPlatforms: true)));
                        this.game = new Lazy<Game>(() => gameTask.Result);

                        var platformTask = Task.Factory.StartNew(() =>
                            {
                                var game = this.game.Value;
                                if (game != null && !game.Platforms.Any(x => x.Name == PlatformName))
                                    PlatformName = string.Empty;
                            });
                        var regionTask = Task.Factory.StartNew(() =>
                            {
                                var game = this.game.Value;
                                if (game != null && !game.Regions.Any(x => x.Name == RegionName))
                                    RegionName = string.Empty;
                            });
                    }
                    else
                        this.game = new Lazy<Game>(() => null);

                    oldCategoryName = null;
                    //TODO: kill off bad platforms and regions
                }


                if (LiveSplitRun.CategoryName != oldCategoryName)
                {
                    oldCategoryName = LiveSplitRun.CategoryName;
                    if (!string.IsNullOrEmpty(oldCategoryName))
                    {
                        var categoryTask = Task.Factory.StartNew(() =>
                        {
                            var game = this.game.Value;
                            if (game == null)
                                return null;

                            var category = SpeedrunCom.Client.Games.GetCategories(game.ID, embeds: new CategoryEmbeds(embedVariables: true))
                                .FirstOrDefault(x => x.Type == CategoryType.PerGame && x.Name == oldCategoryName);
                            return category;
                        });
                        this.category = new Lazy<Category>(() => categoryTask.Result);

                        var variableTask = Task.Factory.StartNew(() =>
                            {
                                var category = this.category.Value;
                                var categoryId = category != null ? category.ID : null;
                                var game = this.game.Value;
                                if (game == null)
                                    return;

                                var variables = game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId).ToList();
                                var deletions = new List<string>();
                                var variableValueNames = VariableValueNames.ToDictionary(x => x.Key, x => x.Value);

                                foreach (var variableNamePair in variableValueNames)
                                {
                                    var variable = variables.FirstOrDefault(x => x.Name == variableNamePair.Key);
                                    if (variable == null 
                                    || (!variable.Values.Any(x => x.Value == variableNamePair.Value) && !variable.IsUserDefined))
                                        deletions.Add(variableNamePair.Key);
                                }
                                foreach (var variable in deletions)
                                {
                                    variableValueNames.Remove(variable);
                                }
                                VariableValueNames = variableValueNames;
                            });
                    }
                    else
                        this.category = new Lazy<Category>(() => null);
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
                platformName = platformName,
                regionName = regionName,
                usesEmulator = usesEmulator,
                VariableValueNames = VariableValueNames.ToDictionary(x => x.Key, x => x.Value),
                //TODO: set members instead later
                //TODO: clone PropertyChanged
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
