using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

using LiveSplit.Model;
using LiveSplit.Options;

namespace LiveSplit.ASL;

public class ASLScript
{
    public class Methods : IEnumerable<ASLMethod>
    {
        private static readonly ASLMethod no_op = new("");

        public ASLMethod startup = no_op;
        public ASLMethod shutdown = no_op;
        public ASLMethod init = no_op;
        public ASLMethod exit = no_op;
        public ASLMethod update = no_op;
        public ASLMethod start = no_op;
        public ASLMethod split = no_op;
        public ASLMethod reset = no_op;
        public ASLMethod isLoading = no_op;
        public ASLMethod gameTime = no_op;
        public ASLMethod onStart = no_op;
        public ASLMethod onSplit = no_op;
        public ASLMethod onReset = no_op;

        public ASLMethod[] GetMethods()
        {
            return
            [
                startup,
                shutdown,
                init,
                exit,
                update,
                start,
                split,
                reset,
                isLoading,
                gameTime,
                onStart,
                onSplit,
                onReset
            ];
        }

        public IEnumerator<ASLMethod> GetEnumerator()
        {
            return ((IEnumerable<ASLMethod>)GetMethods()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetMethods().GetEnumerator();
        }
    }

    public event EventHandler<double> RefreshRateChanged;
    public event EventHandler<string> GameVersionChanged;

    private string _game_version = string.Empty;
    public string GameVersion
    {
        get => _game_version;
        set
        {
            if (value != _game_version)
            {
                GameVersionChanged?.Invoke(this, value);
            }

            _game_version = value;
        }
    }

    private double _refresh_rate = 1000 / 15d;
    public double RefreshRate // per sec
    {
        get => _refresh_rate;
        set
        {
            if (Math.Abs(value - _refresh_rate) > 0.01)
            {
                RefreshRateChanged?.Invoke(this, value);
            }

            _refresh_rate = value;
        }
    }

    // public so other components (ASLVarViewer) can access
    public ASLState State { get; private set; }
    public ASLState OldState { get; private set; }
    public ExpandoObject Vars { get; }

    private readonly bool _uses_game_time;
    private bool _init_completed;

    private readonly ASLSettings _settings;

    private Process _game;
    private TimerModel _timer;

    private readonly Dictionary<string, List<ASLState>> _states;

    private readonly Methods _methods;

    public ASLScript(Methods methods, Dictionary<string, List<ASLState>> states)
    {
        _methods = methods;
        _states = states;

        _settings = new ASLSettings();
        Vars = new ExpandoObject();

        if (!_methods.start.IsEmpty)
        {
            _settings.AddBasicSetting("start");
        }

        if (!_methods.split.IsEmpty)
        {
            _settings.AddBasicSetting("split");
        }

        if (!_methods.reset.IsEmpty)
        {
            _settings.AddBasicSetting("reset");
        }

        _uses_game_time = !_methods.isLoading.IsEmpty || !_methods.gameTime.IsEmpty;
    }

    // Update the script
    public void Update(LiveSplitState state)
    {
        if (_game == null)
        {
            _timer ??= new TimerModel() { CurrentState = state };

            TryConnect(state);
        }
        else if (_game.HasExited)
        {
            DoExit(state);
        }
        else
        {
            if (!_init_completed)
            {
                DoInit(state);
            }
            else
            {
                DoUpdate(state);
            }
        }
    }

    // Run startup and return settings defined in ASL script.
    public ASLSettings RunStartup(LiveSplitState state)
    {
        Debug("Running startup");
        RunNoProcessMethod(_methods.startup, state, true);

        if (!_methods.onStart.IsEmpty)
        {
            state.OnStart += RunOnStart;
        }

        if (!_methods.onSplit.IsEmpty)
        {
            state.OnSplit += RunOnSplit;
        }

        if (!_methods.onReset.IsEmpty)
        {
            state.OnReset += RunOnReset;
        }

        return _settings;
    }

    private void RunOnStart(object sender, EventArgs e)
    {
        RunMethod(_methods.onStart, (LiveSplitState)sender);
    }

    private void RunOnSplit(object sender, EventArgs e)
    {
        RunMethod(_methods.onSplit, (LiveSplitState)sender);
    }

    private void RunOnReset(object sender, TimerPhase e)
    {
        RunMethod(_methods.onReset, (LiveSplitState)sender);
    }

    public void RunShutdown(LiveSplitState state)
    {
        Debug("Running shutdown");

        if (!_methods.onStart.IsEmpty)
        {
            state.OnStart -= RunOnStart;
        }

        if (!_methods.onSplit.IsEmpty)
        {
            state.OnSplit -= RunOnSplit;
        }

        if (!_methods.onReset.IsEmpty)
        {
            state.OnReset -= RunOnReset;
        }

        RunMethod(_methods.shutdown, state);
    }

    private void TryConnect(LiveSplitState state)
    {
        _game = null;

        var state_process = _states.Keys.Select(proccessName => new
        {
            // default to the state with no version specified, if it exists
            State = _states[proccessName].FirstOrDefault(s => s.GameVersion == "") ?? _states[proccessName].First(),
            Process = Process.GetProcessesByName(proccessName).OrderByDescending(x => x.StartTime)
                .FirstOrDefault(x => !x.HasExited)
        }).FirstOrDefault(x => x.Process != null);

        if (state_process == null)
        {
            return;
        }

        _init_completed = false;
        _game = state_process.Process;
        State = state_process.State;

        if (State.GameVersion == "")
        {
            Debug("Connected to game: {0} (using default state descriptor)", _game.ProcessName);
        }
        else
        {
            Debug("Connected to game: {0} (state descriptor for version '{1}' chosen as default)",
                _game.ProcessName,
                State.GameVersion);
        }

        DoInit(state);
    }

    // This is executed each time after connecting to the game (usually just once,
    // unless an error occurs before the method finishes).
    private void DoInit(LiveSplitState state)
    {
        Debug("Initializing");

        State.RefreshValues(_game);
        OldState = State;
        GameVersion = string.Empty;

        // Fetch version from init-method
        string ver = string.Empty;
        RunMethod(_methods.init, state, ref ver);

        if (ver != GameVersion)
        {
            GameVersion = ver;

            ASLState version_state = _states.Where(kv => kv.Key.ToLower() == _game.ProcessName.ToLower())
                .Select(kv => kv.Value)
                .First() // states
                .FirstOrDefault(s => s.GameVersion == ver);

            if (version_state != null)
            {
                // This state descriptor may already be selected
                if (version_state != State)
                {
                    State = version_state;
                    State.RefreshValues(_game);
                    OldState = State;
                    Debug($"Switched to state descriptor for version '{GameVersion}'");
                }
            }
            else
            {
                Debug($"No state descriptor for version '{GameVersion}' (will keep using default one)");
            }
        }

        _init_completed = true;
        Debug("Init completed, running main methods");
    }

    private void DoExit(LiveSplitState state)
    {
        Debug("Running exit");
        _game = null;
        RunNoProcessMethod(_methods.exit, state);
    }

    // This is executed repeatedly as long as the game is connected and initialized.
    private void DoUpdate(LiveSplitState state)
    {
        OldState = State.RefreshValues(_game);

        if (!(RunMethod(_methods.update, state) ?? true))
        {
            // If Update explicitly returns false, don't run anything else
            return;
        }

        if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
        {
            if (_uses_game_time && !state.IsGameTimeInitialized)
            {
                _timer.InitializeGameTime();
            }

            dynamic is_paused = RunMethod(_methods.isLoading, state);
            if (is_paused != null)
            {
                state.IsGameTimePaused = is_paused;
            }

            dynamic game_time = RunMethod(_methods.gameTime, state);
            if (game_time != null)
            {
                state.SetGameTime(game_time);
            }

            if (RunMethod(_methods.reset, state) ?? false)
            {
                if (_settings.GetBasicSettingValue("reset"))
                {
                    _timer.Reset();
                }
            }
            else if (RunMethod(_methods.split, state) ?? false)
            {
                if (_settings.GetBasicSettingValue("split"))
                {
                    _timer.Split();
                }
            }
        }

        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            if (RunMethod(_methods.start, state) ?? false)
            {
                if (_settings.GetBasicSettingValue("start"))
                {
                    _timer.Start();
                }
            }
        }
    }

    private dynamic RunMethod(ASLMethod method, LiveSplitState state, ref string version)
    {
        double refresh_rate = RefreshRate;
        dynamic result = method.Call(state, Vars, ref version, ref refresh_rate, _settings.Reader,
            OldState?.Data, State?.Data, _game);
        RefreshRate = refresh_rate;
        return result;
    }

    private dynamic RunMethod(ASLMethod method, LiveSplitState state)
    {
        string version = GameVersion;
        return RunMethod(method, state, ref version);
    }

    // Run method without counting on being connected to the game (startup/shutdown).
    private void RunNoProcessMethod(ASLMethod method, LiveSplitState state, bool is_startup = false)
    {
        double refresh_rate = RefreshRate;
        string version = GameVersion;
        method.Call(state, Vars, ref version, ref refresh_rate,
            is_startup ? _settings.Builder : (object)_settings.Reader);
        RefreshRate = refresh_rate;
    }

    private void Debug(string output, params object[] args)
    {
        Log.Info(string.Format("[ASL/{1}] {0}",
            string.Format(output, args),
            GetHashCode()));
    }
}
