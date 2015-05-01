using LiveSplit.Model.Input;
using System;
using System.Linq;

namespace LiveSplit.Model
{
    public class TimerModel : ITimerModel
    {
        public LiveSplitState CurrentState
        {
            get
            {
                return _CurrentState;
            }
            set
            {
                _CurrentState = value;
                if (value != null)
                    value.RegisterTimerModel(this);
            }
        }

        private LiveSplitState _CurrentState;

        public event EventHandler OnSplit;
        public event EventHandler OnUndoSplit;
        public event EventHandler OnSkipSplit;
        public event EventHandler OnStart;
        public event EventHandlerT<TimerPhase> OnReset;
        public event EventHandler OnPause;
        public event EventHandler OnResume;
        public event EventHandler OnScrollUp;
        public event EventHandler OnScrollDown;
        public event EventHandler OnSwitchComparisonPrevious;
        public event EventHandler OnSwitchComparisonNext;

        public void Start()
        {
            if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
            {
                CurrentState.CurrentPhase = TimerPhase.Running;
                CurrentState.CurrentSplitIndex = 0;
                CurrentState.AttemptStarted = TripleDateTime.Now;
                CurrentState.StartTime = TripleDateTime.Now - CurrentState.Run.Offset;
                CurrentState.PauseTime = CurrentState.Run.Offset;
                CurrentState.LoadingTimes = TimeSpan.Zero;
                CurrentState.Run.AttemptCount++;
                CurrentState.Run.HasChanged = true;

                if (OnStart != null)
                    OnStart(this,null);
            }
        }

        public void Split()
        {
            if (CurrentState.CurrentPhase == TimerPhase.Running && CurrentState.CurrentTime[TimingMethod.RealTime] > TimeSpan.Zero)
            {
                CurrentState.CurrentSplit.SplitTime = CurrentState.CurrentTime;
                CurrentState.CurrentSplitIndex++;
                if (CurrentState.Run.Count == CurrentState.CurrentSplitIndex)
                {
                    CurrentState.CurrentPhase = TimerPhase.Ended;
                    CurrentState.AttemptEnded = TripleDateTime.Now;
                }
                CurrentState.Run.HasChanged = true;

                if (OnSplit != null)
                    OnSplit(this, null);
            }
        }

        public void SkipSplit()
        {
            if ((CurrentState.CurrentPhase == TimerPhase.Running
                || CurrentState.CurrentPhase == TimerPhase.Paused)
                && CurrentState.CurrentSplitIndex < CurrentState.Run.Count - 1)
            {
                CurrentState.CurrentSplit.SplitTime = default(Time);
                CurrentState.CurrentSplitIndex++;
                CurrentState.Run.HasChanged = true;

                if (OnSkipSplit != null)
                    OnSkipSplit(this, null);
            }
        }

        public void UndoSplit()
        {
            if (CurrentState.CurrentPhase != TimerPhase.NotRunning
                && CurrentState.CurrentSplitIndex > 0)
            {
                if (CurrentState.CurrentPhase == TimerPhase.Ended)
                    CurrentState.CurrentPhase = TimerPhase.Running;
                CurrentState.CurrentSplitIndex--;
                CurrentState.CurrentSplit.SplitTime = default(Time);
                CurrentState.Run.HasChanged = true;

                if (OnUndoSplit != null)
                    OnUndoSplit(this, null);
            }
        }

        public void Reset()
        {
            Reset(true);
        }

        public void Reset(bool updateSplits = true)
        {
            if (CurrentState.CurrentPhase != TimerPhase.NotRunning)
            {
                if (CurrentState.CurrentPhase != TimerPhase.Ended)
                    CurrentState.AttemptEnded = TripleDateTime.Now;
                CurrentState.IsGameTimePaused = false;
                CurrentState.StartTime = TripleDateTime.Now;
                CurrentState.LoadingTimes = TimeSpan.Zero;

                if (updateSplits)
                {
                    UpdateAttemptHistory();
                    UpdateBestSegments();
                    UpdatePBSplits();
                    UpdateSegmentHistory();
                }

                ResetSplits();

                CurrentState.Run.FixSplits();
            }
        }

        private void ResetSplits()
        {
            var oldPhase = CurrentState.CurrentPhase;
            CurrentState.CurrentPhase = TimerPhase.NotRunning;
            CurrentState.CurrentSplitIndex = -1;

            //Reset Splits
            foreach (var split in CurrentState.Run)
            {
                split.SplitTime = default(Time);
            }

            if (OnReset != null)
                OnReset(this, oldPhase);
        }

        public void Pause()
        {
            if (CurrentState.CurrentPhase == TimerPhase.Running)
            {
                CurrentState.PauseTime = CurrentState.CurrentTime[TimingMethod.RealTime].Value;
                CurrentState.CurrentPhase = TimerPhase.Paused;
                if (OnPause != null)
                    OnPause(this, null);
            }
            else if (CurrentState.CurrentPhase == TimerPhase.Paused)
            {
                CurrentState.StartTime = TripleDateTime.Now - CurrentState.PauseTime;
                CurrentState.CurrentPhase = TimerPhase.Running;
                if (OnResume != null)
                    OnResume(this, null);
            }
            else if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
                 Start(); //fuck abahbob                
        }

        public void SwitchComparisonNext()
        {
            var comparisons = CurrentState.Run.Comparisons.ToList();
            CurrentState.CurrentComparison = 
                comparisons.ElementAt((comparisons.IndexOf(CurrentState.CurrentComparison) + 1) 
                % (comparisons.Count()));
            OnSwitchComparisonNext(null, null);
        }

        public void SwitchComparisonPrevious()
        {
            var comparisons = CurrentState.Run.Comparisons.ToList();
            CurrentState.CurrentComparison = 
                comparisons.ElementAt((comparisons.IndexOf(CurrentState.CurrentComparison) - 1 + comparisons.Count())
                % (comparisons.Count()));
            OnSwitchComparisonPrevious(null, null);
        }

        public void ScrollUp()
        {
            if (OnScrollUp != null)
                OnScrollUp(this, null);
        }

        public void ScrollDown()
        {
            if (OnScrollDown != null)
                OnScrollDown(this, null);
        }

        public void UpdateAttemptHistory()
        {
            Time time = new Time();
            time = (CurrentState.CurrentPhase == TimerPhase.Ended) ? CurrentState.CurrentTime : default(Time);
            var maxIndex = CurrentState.Run.AttemptHistory.DefaultIfEmpty().Max(x => x.Index);
            var newIndex = Math.Max(0, maxIndex + 1);
            var newAttempt = new Attempt(newIndex, time, CurrentState.AttemptStarted.UtcNow, CurrentState.AttemptEnded.UtcNow);
            CurrentState.Run.AttemptHistory.Add(newAttempt);
        }

        public void UpdateBestSegments()
        {
            TimeSpan? currentSegmentRTA = TimeSpan.Zero;
            TimeSpan? previousSplitTimeRTA = TimeSpan.Zero;
            TimeSpan? currentSegmentGameTime = TimeSpan.Zero;
            TimeSpan? previousSplitTimeGameTime = TimeSpan.Zero;
            foreach (var split in CurrentState.Run)
            {
                var newBestSegment = new Time(split.BestSegmentTime);
                if (split.SplitTime[TimingMethod.RealTime] != null)
                {
                    currentSegmentRTA = split.SplitTime[TimingMethod.RealTime] - previousSplitTimeRTA;
                    previousSplitTimeRTA = split.SplitTime[TimingMethod.RealTime];
                    if (split.BestSegmentTime[TimingMethod.RealTime] == null || currentSegmentRTA < split.BestSegmentTime[TimingMethod.RealTime])
                        newBestSegment[TimingMethod.RealTime] = currentSegmentRTA;
                }
                if (split.SplitTime[TimingMethod.GameTime] != null)
                {
                    currentSegmentGameTime = split.SplitTime[TimingMethod.GameTime] - previousSplitTimeGameTime;
                    previousSplitTimeGameTime = split.SplitTime[TimingMethod.GameTime];
                    if (split.BestSegmentTime[TimingMethod.GameTime] == null || currentSegmentGameTime < split.BestSegmentTime[TimingMethod.GameTime])
                        newBestSegment[TimingMethod.GameTime] = currentSegmentGameTime;
                }
                split.BestSegmentTime = newBestSegment;
            }
        }

        public void UpdatePBSplits()
        {
            var curMethod = CurrentState.CurrentTimingMethod;
            if ((CurrentState.Run.Last().SplitTime[curMethod] != null && CurrentState.Run.Last().PersonalBestSplitTime[curMethod] == null) || CurrentState.Run.Last().SplitTime[curMethod] < CurrentState.Run.Last().PersonalBestSplitTime[curMethod])
                SetRunAsPB();
        }

        public void UpdateSegmentHistory()
        {
            TimeSpan? splitTimeRTA = TimeSpan.Zero;
            TimeSpan? splitTimeGameTime = TimeSpan.Zero;
            foreach (var split in CurrentState.Run.Take(CurrentState.CurrentSplitIndex))
            {
                var newTime = new Time();
                newTime.RealTime = split.SplitTime.RealTime - splitTimeRTA;
                newTime.GameTime = split.SplitTime.GameTime - splitTimeGameTime;
                split.SegmentHistory.Add(new IndexedTime(newTime, CurrentState.Run.AttemptHistory.Last().Index));
                if (split.SplitTime.RealTime.HasValue)
                    splitTimeRTA = split.SplitTime.RealTime;
                if (split.SplitTime.GameTime.HasValue)
                    splitTimeGameTime = split.SplitTime.GameTime;
            }
        }

        public void SetRunAsPB()
        {
            CurrentState.Run.ImportSegmentHistory();
            foreach (var current in CurrentState.Run)
                current.PersonalBestSplitTime = current.SplitTime;
        }
    }
}
