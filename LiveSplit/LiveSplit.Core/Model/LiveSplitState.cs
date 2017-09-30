using LiveSplit.Model.Input;
using LiveSplit.Options;
using LiveSplit.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;

namespace LiveSplit.Model
{
    public class LiveSplitState : ICloneable
    {
        public IRun Run { get; set; }
        public ILayout Layout { get; set; }
        public LayoutSettings LayoutSettings { get; set; }
        public ISettings Settings { get; set; }
        public Forms.Form Form { get; set; }

        public AtomicDateTime AttemptStarted { get; set; }
        public AtomicDateTime AttemptEnded { get; set; }

        public TimeStamp AdjustedStartTime { get; set; }
        public TimeStamp StartTimeWithOffset { get; set; }
        public TimeStamp StartTime { get; set; }
        public TimeSpan TimePausedAt { get; set; }
        public TimeSpan? GameTimePauseTime { get; set; }
        public TimerPhase CurrentPhase { get; set; }
        public string CurrentComparison { get; set; }
        public TimingMethod CurrentTimingMethod { get; set; }
        public string CurrentHotkeyProfile { get; set; }

        internal TimeSpan? loadingTimes;
        public TimeSpan LoadingTimes { get { return loadingTimes ?? TimeSpan.Zero; } set { loadingTimes = value; } }
        public bool IsGameTimeInitialized
        {
            get
            {
                return loadingTimes.HasValue;
            }
            set
            {
                if (value)
                {
                    loadingTimes = LoadingTimes;
                }
                else
                    loadingTimes = null;
            }
        }
        private bool isGameTimePaused;
        public bool IsGameTimePaused
        {
            get { return isGameTimePaused; }
            set
            {
                if (!value && isGameTimePaused)
                    LoadingTimes = CurrentTime.RealTime.Value - (CurrentTime.GameTime ?? CurrentTime.RealTime.Value);
                else if (value && !isGameTimePaused)
                    GameTimePauseTime = (CurrentTime.GameTime ?? CurrentTime.RealTime);

                isGameTimePaused = value;
            }
        }

        public event EventHandler OnSplit;
        public event EventHandler OnUndoSplit;
        public event EventHandler OnSkipSplit;
        public event EventHandler OnStart;
        public event EventHandlerT<TimerPhase> OnReset;
        public event EventHandler OnPause;
        public event EventHandler OnUndoAllPauses;
        public event EventHandler OnResume;
        public event EventHandler OnScrollUp;
        public event EventHandler OnScrollDown;
        public event EventHandler OnSwitchComparisonPrevious;
        public event EventHandler OnSwitchComparisonNext;

        public event EventHandler RunManuallyModified;
        public event EventHandler ComparisonRenamed;

        public Time CurrentTime
        { 
            get 
            {
                var curTime = new Time();

                if (CurrentPhase == TimerPhase.NotRunning)
                    curTime.RealTime = TimeSpan.Zero;
                else if (CurrentPhase == TimerPhase.Running)
                    curTime.RealTime = TimeStamp.Now - AdjustedStartTime;
                else if (CurrentPhase == TimerPhase.Paused)
                    curTime.RealTime = TimePausedAt;
                else
                    curTime.RealTime = Run.Last().SplitTime.RealTime;

                if (CurrentPhase == TimerPhase.Ended)
                    curTime.GameTime = Run.Last().SplitTime.GameTime;
                else
                    curTime.GameTime = IsGameTimePaused 
                        ? GameTimePauseTime 
                        : curTime.RealTime - (IsGameTimeInitialized ? (TimeSpan?)LoadingTimes : null);

                return curTime;
            }
        }

        public TimeSpan? PauseTime
        {
            get
            {
                if (CurrentPhase == TimerPhase.Paused)
                    return TimeStamp.Now - StartTimeWithOffset - TimePausedAt;
                if (CurrentPhase != TimerPhase.NotRunning && StartTimeWithOffset != AdjustedStartTime)
                    return AdjustedStartTime - StartTimeWithOffset;
                return null;
            }
        }

        public TimeSpan CurrentAttemptDuration
        {
            get
            {
                if (CurrentPhase == TimerPhase.Paused || CurrentPhase == TimerPhase.Running)
                    return TimeStamp.Now - StartTime;
                if (CurrentPhase == TimerPhase.Ended)
                    return AttemptEnded - AttemptStarted;
                return TimeSpan.Zero;
            }
        }

        public int CurrentSplitIndex { get; set; }
        public ISegment CurrentSplit => (CurrentSplitIndex >= 0 && CurrentSplitIndex < Run.Count) ? Run[CurrentSplitIndex] : null;

        private LiveSplitState() { }

        public LiveSplitState(IRun run, Forms.Form form, ILayout layout, LayoutSettings layoutSettings, ISettings settings)
        {
            Run = run;
            Form = form;
            Layout = layout;
            Settings = settings;
            LayoutSettings = layoutSettings;
            AdjustedStartTime = StartTimeWithOffset = StartTime = TimeStamp.Now;
            CurrentPhase = TimerPhase.NotRunning;
            CurrentSplitIndex = -1;
        }

        public object Clone()
        {
            return new LiveSplitState()
            {
                Run = Run.Clone() as IRun,
                Form = Form,
                Layout = Layout.Clone() as ILayout,
                Settings = Settings.Clone() as ISettings,
                LayoutSettings = LayoutSettings.Clone() as LayoutSettings,
                AdjustedStartTime = AdjustedStartTime,
                StartTimeWithOffset = StartTimeWithOffset,
                StartTime = StartTime,
                TimePausedAt = TimePausedAt,
                GameTimePauseTime = GameTimePauseTime,
                isGameTimePaused = isGameTimePaused,
                LoadingTimes = LoadingTimes,
                CurrentPhase = CurrentPhase,
                CurrentSplitIndex = CurrentSplitIndex,
                CurrentComparison = CurrentComparison,
                CurrentHotkeyProfile = CurrentHotkeyProfile,
                CurrentTimingMethod = CurrentTimingMethod,
                AttemptStarted = AttemptStarted,
                AttemptEnded = AttemptEnded
            };
        }

        public void RegisterTimerModel(ITimerModel model)
        {
            model.OnSplit                    += (s, e) => OnSplit?.Invoke(this, e);
            model.OnSkipSplit                += (s, e) => OnSkipSplit?.Invoke(this, e);
            model.OnUndoSplit                += (s, e) => OnUndoSplit?.Invoke(this, e);
            model.OnStart                    += (s, e) => OnStart?.Invoke(this, e);
            model.OnReset                    += (s, e) => OnReset?.Invoke(this, e);
            model.OnPause                    += (s, e) => OnPause?.Invoke(this, e);
            model.OnUndoAllPauses            += (s, e) => OnUndoAllPauses?.Invoke(this, e);
            model.OnResume                   += (s, e) => OnResume?.Invoke(this, e);
            model.OnScrollUp                 += (s, e) => OnScrollUp?.Invoke(this, e);
            model.OnScrollDown               += (s, e) => OnScrollDown?.Invoke(this, e);
            model.OnSwitchComparisonPrevious += (s, e) => OnSwitchComparisonPrevious?.Invoke(this, e);
            model.OnSwitchComparisonNext     += (s, e) => OnSwitchComparisonNext?.Invoke(this, e);
        }

        public void SetGameTime(TimeSpan? gameTime)
        {
            if (CurrentTime.RealTime.HasValue && gameTime.HasValue)
            {
                LoadingTimes = CurrentTime.RealTime.Value - gameTime.Value;
                if (IsGameTimePaused)
                    GameTimePauseTime = gameTime.Value;
            }
        }

        public void CallRunManuallyModified() => RunManuallyModified?.Invoke(this, null);

        public void CallComparisonRenamed(EventArgs e) => ComparisonRenamed?.Invoke(this, e);
    }
}
