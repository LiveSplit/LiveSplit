using LiveSplit.Options;
using LiveSplit.Web.Share;
using SpeedrunComSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public class RunMetadata
    {
        public IRun LiveSplitRun { get; private set; }

        private string oldGameName;
        private string oldCategoryName;
        private Lazy<Game> game;
        private bool gameLoaded;
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
                if (platformName != value)
                    TriggerPropertyChanged(true);

                platformName = value; 
            }
        }
        public Platform Platform => Game?.Platforms.FirstOrDefault(x => x.Name == PlatformName);

        public string RegionName
        {
            get
            {
                return regionName;
            }
            set
            {
                if (regionName != value)
                    TriggerPropertyChanged(true);

                regionName = value;
            }
        }
        public Region Region => Game?.Regions.FirstOrDefault(x => x.Name == RegionName);

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
                if (usesEmulator != value)
                    TriggerPropertyChanged(true);

                usesEmulator = value;
            }
        }


        public bool GameAvailable { get; private set; }
        public Game Game
        {
            get
            {
                Refresh();
                return game.Value;
            }
        }

        public bool CategoryAvailable { get; private set; }
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
            LiveSplitRun = run;
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
                    GameAvailable = false;
                    CategoryAvailable = false;

                    oldGameName = LiveSplitRun.GameName;
                    if (!string.IsNullOrEmpty(LiveSplitRun.GameName))
                    {
                        var gameTask = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                var gameId = Web.CompositeGameList.Instance.GetGameID(oldGameName);
                                var game = SpeedrunCom.Client.Games.GetGame(gameId, new GameEmbeds(embedRegions: true, embedPlatforms: true));
                                gameLoaded = true;
                                if (game != null)
                                    GameAvailable = true;
                                return game;
                            }
                            catch
                            {
                                gameLoaded = false;
                                return null;
                            }
                        });
                        this.game = new Lazy<Game>(() => gameTask.Result);

                        var platformTask = Task.Factory.StartNew(() =>
                            {
                                var game = this.game.Value;
                                if (game != null && !game.Platforms.Any(x => x.Name == PlatformName) || gameLoaded && game == null)
                                    PlatformName = string.Empty;
                            });
                        var regionTask = Task.Factory.StartNew(() =>
                            {
                                var game = this.game.Value;
                                if (game != null && !game.Regions.Any(x => x.Name == RegionName) || gameLoaded && game == null)
                                    RegionName = string.Empty;
                            });
                    }
                    else
                        this.game = new Lazy<Game>(() => null);

                    oldCategoryName = null;
                }


                if (LiveSplitRun.CategoryName != oldCategoryName)
                {
                    CategoryAvailable = false;

                    oldCategoryName = LiveSplitRun.CategoryName;
                    if (!string.IsNullOrEmpty(oldCategoryName))
                    {
                        var categoryTask = Task.Factory.StartNew(() =>
                        {
                            var game = this.game.Value;
                            if (game == null)
                                return null;

                            try
                            {
                                var category = SpeedrunCom.Client.Games.GetCategories(game.ID, embeds: new CategoryEmbeds(embedVariables: true))
                                    .FirstOrDefault(x => x.Type == CategoryType.PerGame && x.Name == oldCategoryName);
                                if (category != null)
                                    CategoryAvailable = true;
                                return category;
                            }
                            catch
                            {
                                return null;
                            }
                        });
                        this.category = new Lazy<Category>(() => categoryTask.Result);

                        var variableTask = Task.Factory.StartNew(() =>
                            {
                                var category = this.category.Value;
                                var categoryId = category != null ? category.ID : null;
                                var game = this.game.Value;
                                if (game == null && !gameLoaded)
                                    return;

                                try
                                {
                                    var deletions = new List<string>();
                                    var variableValueNames = VariableValueNames.ToDictionary(x => x.Key, x => x.Value);

                                    if (game != null)
                                    {
                                        var variables = game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId).ToList();

                                        foreach (var variableNamePair in variableValueNames)
                                        {
                                            var variable = variables.FirstOrDefault(x => x.Name == variableNamePair.Key);
                                            if (variable == null
                                            || (!variable.Values.Any(x => x.Value == variableNamePair.Value) && !variable.IsUserDefined))
                                                deletions.Add(variableNamePair.Key);
                                        }
                                    }
                                    else
                                    {
                                        deletions.AddRange(variableValueNames.Keys);
                                    }

                                    foreach (var variable in deletions)
                                    {
                                        variableValueNames.Remove(variable);
                                    }
                                    VariableValueNames = variableValueNames;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
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
                gameLoaded = gameLoaded,
                category = category,
                run = this.run,
                runId = runId,
                platformName = platformName,
                regionName = regionName,
                usesEmulator = usesEmulator,
                VariableValueNames = VariableValueNames.ToDictionary(x => x.Key, x => x.Value),
                CategoryAvailable = CategoryAvailable,
                GameAvailable = GameAvailable
                //TODO: set members instead later
                //TODO: clone PropertyChanged
            };
        }

        private void TriggerPropertyChanged(bool clearRunID)
        {
            PropertyChanged?.Invoke(this, new MetadataChangedEventArgs(clearRunID));
        }
    }
}
