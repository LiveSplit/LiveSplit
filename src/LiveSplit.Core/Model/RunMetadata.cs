using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LiveSplit.Options;
using LiveSplit.Web.Share;

using SpeedrunComSharp;

namespace LiveSplit.Model;

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
        get => runId;
        set
        {
            runId = value;
            run = new Lazy<SpeedrunComSharp.Run>(() => SpeedrunCom.Client.Runs.GetRun(runId));

            if (value != null)
            {
                TriggerPropertyChanged(false);
            }
        }
    }

    public SpeedrunComSharp.Run Run
    {
        get => run.Value;
        set
        {
            runId = value.ID;
            run = new Lazy<SpeedrunComSharp.Run>(() => value);

            if (value != null)
            {
                TriggerPropertyChanged(false);
            }
        }
    }

    public string PlatformName
    {
        get => platformName;
        set
        {
            if (platformName != value)
            {
                TriggerPropertyChanged(true);
            }

            platformName = value;
        }
    }
    public Platform Platform => Game?.Platforms.FirstOrDefault(x => x.Name == PlatformName);

    public string RegionName
    {
        get => regionName;
        set
        {
            if (regionName != value)
            {
                TriggerPropertyChanged(true);
            }

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
            {
                return new Dictionary<Variable, VariableValue>();
            }

            string categoryId = Category?.ID;
            IEnumerable<Variable> variables = Game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId);
            return variables.ToDictionary(x => x, x =>
            {
                if (!VariableValueNames.ContainsKey(x.Name))
                {
                    return null;
                }

                string variableValue = VariableValueNames[x.Name];
                VariableValue foundValue = x.Values.FirstOrDefault(y => y.Value == variableValue);

                if (foundValue == null && x.IsUserDefined)
                {
                    foundValue = x.CreateCustomValue(variableValue);
                }

                return foundValue;
            });
        }
    }

    /// <summary>
    ///     A dictionary mapping custom variable names to <see cref="CustomVariable"/> objects.
    /// </summary>
    public Dictionary<string, CustomVariable> CustomVariables { get; private set; } = new Dictionary<string, CustomVariable>();

    /// <summary>
    ///     Gets the custom variable with the specified <paramref name="name"/> if it exists;
    ///     otherwise, adds a new one.
    /// </summary>
    /// <param name="name">
    ///     The key of the custom variable to get or add.
    /// </param>
    /// <returns>
    ///     The custom variable corresponding to the specified <paramref name="name"/>.
    /// </returns>
    public CustomVariable GetOrAddCustomVariable(string name)
    {
        if (!CustomVariables.TryGetValue(name, out CustomVariable variable))
        {
            CustomVariables.Add(name, variable = new());
        }

        return variable;
    }

    /// <summary>
    ///     Gets the value of the custom variable corresponding to <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The key of the custom variable whose value to get.
    /// </param>
    /// <returns>
    ///     The value of the custom variable corresponding to <paramref name="name"/>.
    /// </returns>
    public string CustomVariableValue(string name)
    {
        return GetOrAddCustomVariable(name).Value;
    }

    /// <summary>
    ///     Sets a custom variable, mapping <paramref name="name"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="name">
    ///     The key of the custom variable.
    /// </param>
    /// <param name="value">
    ///     The value which the custom variable should have.
    /// </param>
    public void SetCustomVariable(string name, string value)
    {
        CustomVariable v = GetOrAddCustomVariable(name);
        v.Value = value;
        if (v.IsPermanent)
        {
            LiveSplitRun.HasChanged = true;
        }
    }

    public bool UsesEmulator
    {
        get => usesEmulator;
        set
        {
            if (usesEmulator != value)
            {
                TriggerPropertyChanged(true);
            }

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
        CustomVariables = new Dictionary<string, CustomVariable>();
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
                    Task<Game> gameTask = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string gameId = Web.CompositeGameList.Instance.GetGameID(oldGameName);
                            Game game = SpeedrunCom.Client.Games.GetGame(gameId, new GameEmbeds(embedRegions: true, embedPlatforms: true));
                            gameLoaded = true;
                            if (game != null)
                            {
                                GameAvailable = true;
                            }

                            return game;
                        }
                        catch
                        {
                            gameLoaded = false;
                            return null;
                        }
                    });
                    game = new Lazy<Game>(() => gameTask.Result);

                    Task platformTask = Task.Factory.StartNew(() =>
                        {
                            Game game = this.game.Value;
                            if ((game != null && !game.Platforms.Any(x => x.Name == PlatformName)) || (gameLoaded && game == null))
                            {
                                PlatformName = string.Empty;
                            }
                        });
                    Task regionTask = Task.Factory.StartNew(() =>
                        {
                            Game game = this.game.Value;
                            if ((game != null && !game.Regions.Any(x => x.Name == RegionName)) || (gameLoaded && game == null))
                            {
                                RegionName = string.Empty;
                            }
                        });
                }
                else
                {
                    game = new Lazy<Game>(() => null);
                }

                oldCategoryName = null;
            }

            if (LiveSplitRun.CategoryName != oldCategoryName)
            {
                CategoryAvailable = false;

                oldCategoryName = LiveSplitRun.CategoryName;
                if (!string.IsNullOrEmpty(oldCategoryName))
                {
                    Task<Category> categoryTask = Task.Factory.StartNew(() =>
                    {
                        Game game = this.game.Value;
                        if (game == null)
                        {
                            return null;
                        }

                        try
                        {
                            Category category = SpeedrunCom.Client.Games.GetCategories(game.ID, embeds: new CategoryEmbeds(embedVariables: true))
                                .FirstOrDefault(x => x.Type == CategoryType.PerGame && x.Name == oldCategoryName);
                            if (category != null)
                            {
                                CategoryAvailable = true;
                            }

                            return category;
                        }
                        catch
                        {
                            return null;
                        }
                    });
                    category = new Lazy<Category>(() => categoryTask.Result);

                    Task variableTask = Task.Factory.StartNew(() =>
                        {
                            Category category = this.category.Value;
                            string categoryId = category?.ID;
                            Game game = this.game.Value;
                            if (game == null && !gameLoaded)
                            {
                                return;
                            }

                            try
                            {
                                var deletions = new List<string>();
                                var variableValueNames = VariableValueNames.ToDictionary(x => x.Key, x => x.Value);

                                if (game != null)
                                {
                                    var variables = game.FullGameVariables.Where(x => x.CategoryID == null || x.CategoryID == categoryId).ToList();

                                    foreach (KeyValuePair<string, string> variableNamePair in variableValueNames)
                                    {
                                        Variable variable = variables.FirstOrDefault(x => x.Name == variableNamePair.Key);
                                        if (variable == null
                                        || (!variable.Values.Any(x => x.Value == variableNamePair.Value) && !variable.IsUserDefined))
                                        {
                                            deletions.Add(variableNamePair.Key);
                                        }
                                    }
                                }
                                else
                                {
                                    deletions.AddRange(variableValueNames.Keys);
                                }

                                foreach (string variable in deletions)
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
                {
                    category = new Lazy<Category>(() => null);
                }
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
            CustomVariables = CustomVariables.ToDictionary(x => x.Key, x => x.Value.Clone()),
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

/// <summary>
///     A custom variable that has a value and can be marked permanent.
/// </summary>
public sealed class CustomVariable
{
    /// <summary>
    ///     The current value of the custom variable.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the custom variable should be saved in the run permanently.
    /// </summary>
    /// <value>
    ///     <see langword="false"/> initially. Once set to <see langword="true"/>, will remain as such.
    /// </value>
    public bool IsPermanent { get; private set; }

    /// <summary>
    ///     Creates a new instance of the <see cref="CustomVariable"/> class
    ///     with a default value, not permanent.
    /// </summary>
    public CustomVariable()
        : this(null, false) { }

    /// <summary>
    ///     Creates a new instance of the <see cref="CustomVariable"/> class
    ///     with the specified <paramref name="value"/> and permanence.
    /// </summary>
    /// <param name="value">
    ///     The value of the custom variable.
    /// </param>
    /// <param name="isPermanent">
    ///     Indicates whether it should be saved in the run permanently.
    /// </param>
    public CustomVariable(string value, bool isPermanent)
    {
        Value = value;
        IsPermanent = isPermanent;
    }

    /// <summary>
    ///     Makes the custom variable permanent.
    /// </summary>
    /// <returns>
    ///     The same custom variable object marked permanent.
    /// </returns>
    public CustomVariable AsPermanent()
    {
        IsPermanent = true;
        return this;
    }

    /// <summary>
    ///     Constructs a new custom variable with the same value and permanence.
    /// </summary>
    /// <returns>
    ///     A copy of the custom variable.
    /// </returns>
    public CustomVariable Clone()
    {
        return new CustomVariable(Value, IsPermanent);
    }
}
