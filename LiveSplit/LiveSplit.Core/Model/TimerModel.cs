using LiveSplit.Model.Input;
using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

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
                value?.RegisterTimerModel(this);
            }
        }
        public Boolean firstSplit = true;
        public void WriteCurrentSplitInfo()
        {

            String currentsplitdirectory = Application.StartupPath + @"\CurrentSplit";
            if (!Directory.Exists(currentsplitdirectory))
            {
                Directory.CreateDirectory(currentsplitdirectory);
            }
            if (CurrentState.CurrentPhase == TimerPhase.Running)
            {
                String bestSegmentTime = CurrentState.CurrentSplit.BestSegmentTime.RealTime.ToString();
                String pbSplitTime = CurrentState.CurrentSplit.PersonalBestSplitTime.ToString();
                String pbSegmentTime = CurrentState.CurrentSplit.SegmentHistory.Last().Value.ToString();
                if (bestSegmentTime.Substring(0, 3).CompareTo("00:") == 0)
                {

                    bestSegmentTime = bestSegmentTime.Remove(11);
                    bestSegmentTime = bestSegmentTime.Remove(0, 3);

                }
                else
                {
                    bestSegmentTime = bestSegmentTime.Remove(11);
                }
                if (pbSplitTime.Substring(0, 3).CompareTo("00:") == 0)
                {
                    pbSplitTime = pbSplitTime.Remove(11);
                    pbSplitTime = pbSplitTime.Remove(0, 3);

                }
                else
                {
                    pbSplitTime = pbSplitTime.Remove(11);
                }
                if (pbSegmentTime.Substring(0, 3).CompareTo("00:") == 0)
                {
                    pbSegmentTime = pbSegmentTime.Remove(11);
                    pbSegmentTime = pbSegmentTime.Remove(0, 3);
                }
                else
                {
                    pbSegmentTime = pbSegmentTime.Remove(11);
                }
                if (firstSplit)
                {
                    DateTime startedAT = TimeZoneInfo.ConvertTime(CurrentState.AttemptStarted.Time, TimeZoneInfo.Utc, TimeZoneInfo.Local);
                    File.WriteAllText(currentsplitdirectory + @"\AttemptStartedAt.txt", startedAT.TimeOfDay.ToString().Remove(8));
                    firstSplit = false;

                }
                File.WriteAllText(currentsplitdirectory + @"\TimingMethod.txt", CurrentState.CurrentTimingMethod.ToString());
                File.WriteAllText(currentsplitdirectory + @"\Name.txt", CurrentState.CurrentSplit.Name.ToString());
                File.WriteAllText(currentsplitdirectory + @"\BestSegmentTime.txt", bestSegmentTime);
                File.WriteAllText(currentsplitdirectory + @"\SplitTime.txt", pbSplitTime);
                File.WriteAllText(currentsplitdirectory + @"\CurrentComparison.txt", CurrentState.CurrentComparison.ToString());
                File.WriteAllText(currentsplitdirectory + @"\PbSegmentTime.txt", pbSegmentTime.ToString());
            }
            else
            {

                DateTime endedAT = TimeZoneInfo.ConvertTime(CurrentState.AttemptEnded.Time, TimeZoneInfo.Utc, TimeZoneInfo.Local);
                File.WriteAllText(currentsplitdirectory + @"\AttemptEndedAt.txt", endedAT.TimeOfDay.ToString().Remove(8));
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
        public BackgroundWorker worker = new BackgroundWorker();
        public void StartBackgroundWorker()
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
            //in case people spam too fast undo and the program can't handle it
            if (!worker.IsBusy)
            {


                worker.DoWork += backgroundWorker1_DoWork;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            WriteCurrentSplitInfo();
        }
        public void Start()
        {
            if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
            {
                CurrentState.CurrentPhase = TimerPhase.Running;
                CurrentState.CurrentSplitIndex = 0;
                CurrentState.AttemptStarted = TimeStamp.CurrentDateTime;
                CurrentState.StartTime = TimeStamp.Now - CurrentState.Run.Offset;
                CurrentState.PauseTime = CurrentState.Run.Offset;
                CurrentState.IsGameTimeInitialized = false;
                CurrentState.Run.AttemptCount++;
                CurrentState.Run.HasChanged = true;
                OnStart?.Invoke(this, null);
                StartBackgroundWorker();

            }
        }

        public void InitializeGameTime() => CurrentState.IsGameTimeInitialized = true;

        public void Split()
        {
            if (CurrentState.CurrentPhase == TimerPhase.Running && CurrentState.CurrentTime.RealTime > TimeSpan.Zero)
            {
                CurrentState.CurrentSplit.SplitTime = CurrentState.CurrentTime;
                CurrentState.CurrentSplitIndex++;
                if (CurrentState.Run.Count == CurrentState.CurrentSplitIndex)
                {
                    CurrentState.CurrentPhase = TimerPhase.Ended;
                    CurrentState.AttemptEnded = TimeStamp.CurrentDateTime;

                }
                CurrentState.Run.HasChanged = true;

                OnSplit?.Invoke(this, null);
                StartBackgroundWorker();
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

                OnSkipSplit?.Invoke(this, null);
                StartBackgroundWorker();
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

                OnUndoSplit?.Invoke(this, null);
                StartBackgroundWorker();
            }
        }

        public void Reset()
        {
            Reset(true);
        }

        public void Reset(bool updateSplits)
        {
            if (CurrentState.CurrentPhase != TimerPhase.NotRunning)
            {
                if (CurrentState.CurrentPhase != TimerPhase.Ended)
                    CurrentState.AttemptEnded = TimeStamp.CurrentDateTime;
                CurrentState.IsGameTimePaused = false;
                CurrentState.StartTime = TimeStamp.Now;
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

            firstSplit = true;

            OnReset?.Invoke(this, oldPhase);
        }

        public void Pause()
        {
            if (CurrentState.CurrentPhase == TimerPhase.Running)
            {
                CurrentState.PauseTime = CurrentState.CurrentTime.RealTime.Value;
                CurrentState.CurrentPhase = TimerPhase.Paused;
                OnPause?.Invoke(this, null);
            }
            else if (CurrentState.CurrentPhase == TimerPhase.Paused)
            {
                CurrentState.StartTime = TimeStamp.Now - CurrentState.PauseTime;
                CurrentState.CurrentPhase = TimerPhase.Running;
                OnResume?.Invoke(this, null);
            }
            else if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
                Start(); //fuck abahbob                
        }

        public void SwitchComparisonNext()
        {
            var comparisons = CurrentState.Run.Comparisons.ToList();
            CurrentState.CurrentComparison =
                comparisons.ElementAt((comparisons.IndexOf(CurrentState.CurrentComparison) + 1)
                % (comparisons.Count));
            OnSwitchComparisonNext?.Invoke(null, null);
        }

        public void SwitchComparisonPrevious()
        {
            var comparisons = CurrentState.Run.Comparisons.ToList();
            CurrentState.CurrentComparison =
                comparisons.ElementAt((comparisons.IndexOf(CurrentState.CurrentComparison) - 1 + comparisons.Count())
                % (comparisons.Count));
            OnSwitchComparisonPrevious?.Invoke(null, null);
        }

        public void ScrollUp()
        {
            OnScrollUp?.Invoke(this, null);
        }

        public void ScrollDown()
        {
            OnScrollDown?.Invoke(this, null);
        }

        public void UpdateAttemptHistory()
        {
            Time time = new Time();
            time = (CurrentState.CurrentPhase == TimerPhase.Ended) ? CurrentState.CurrentTime : default(Time);
            var maxIndex = CurrentState.Run.AttemptHistory.DefaultIfEmpty().Max(x => x.Index);
            var newIndex = Math.Max(0, maxIndex + 1);
            var newAttempt = new Attempt(newIndex, time, CurrentState.AttemptStarted, CurrentState.AttemptEnded);
            CurrentState.Run.AttemptHistory.Add(newAttempt);
            firstSplit = true;
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
                if (split.SplitTime.RealTime != null)
                {
                    currentSegmentRTA = split.SplitTime.RealTime - previousSplitTimeRTA;
                    previousSplitTimeRTA = split.SplitTime.RealTime;
                    if (split.BestSegmentTime.RealTime == null || currentSegmentRTA < split.BestSegmentTime.RealTime)
                        newBestSegment.RealTime = currentSegmentRTA;
                }
                if (split.SplitTime.GameTime != null)
                {
                    currentSegmentGameTime = split.SplitTime.GameTime - previousSplitTimeGameTime;
                    previousSplitTimeGameTime = split.SplitTime.GameTime;
                    if (split.BestSegmentTime.GameTime == null || currentSegmentGameTime < split.BestSegmentTime.GameTime)
                        newBestSegment.GameTime = currentSegmentGameTime;
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
                split.SegmentHistory.Add(CurrentState.Run.AttemptHistory.Last().Index, newTime);
                if (split.SplitTime.RealTime.HasValue)
                    splitTimeRTA = split.SplitTime.RealTime;
                if (split.SplitTime.GameTime.HasValue)
                    splitTimeGameTime = split.SplitTime.GameTime;
            }
        }

        public void SetRunAsPB()
        {
            CurrentState.Run.ImportSegmentHistory();
            CurrentState.Run.FixSplits();
            foreach (var current in CurrentState.Run)
                current.PersonalBestSplitTime = current.SplitTime;
            CurrentState.Run.Metadata.RunID = null;
        }
    }
}
