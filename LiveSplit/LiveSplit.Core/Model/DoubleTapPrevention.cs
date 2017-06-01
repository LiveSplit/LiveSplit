using LiveSplit.Model.Input;
using System;

namespace LiveSplit.Model
{
    public class DoubleTapPrevention : ITimerModel
    {
        public LiveSplitState CurrentState
        {
            get
            {
                return InternalModel.CurrentState;
            }
            set
            {
                InternalModel.CurrentState = value;
            }
        }

        private TimeSpan Delay = new TimeSpan(0, 0, 0, 0, 300);
        private TimeSpan LongDelay = new TimeSpan(0, 0, 0, 0, 600);

        public ITimerModel InternalModel { get; set; }

        public event EventHandler OnSplit { add { InternalModel.OnSplit += value; } remove { InternalModel.OnSplit -= value; } }

        public event EventHandler OnUndoSplit { add { InternalModel.OnUndoSplit += value; } remove { InternalModel.OnUndoSplit -= value; } }

        public event EventHandler OnSkipSplit { add { InternalModel.OnSkipSplit += value; } remove { InternalModel.OnSkipSplit -= value; } }

        public event EventHandler OnStart { add { InternalModel.OnStart += value; } remove { InternalModel.OnStart -= value; } }

        public event EventHandlerT<TimerPhase> OnReset { add { InternalModel.OnReset += value; } remove { InternalModel.OnReset -= value; } }

        public event EventHandler OnPause { add { InternalModel.OnPause += value; } remove { InternalModel.OnPause -= value; } }

        public event EventHandler OnUndoAllPauses { add { InternalModel.OnUndoAllPauses += value; } remove { InternalModel.OnUndoAllPauses -= value; } }

        public event EventHandler OnResume { add { InternalModel.OnResume += value; } remove { InternalModel.OnResume -= value; } }

        public event EventHandler OnScrollUp { add { InternalModel.OnScrollUp += value; } remove { InternalModel.OnScrollUp -= value; } }

        public event EventHandler OnScrollDown { add { InternalModel.OnScrollDown += value; } remove { InternalModel.OnScrollDown -= value; } }

        public event EventHandler OnSwitchComparisonPrevious { add { InternalModel.OnSwitchComparisonPrevious += value; } remove { InternalModel.OnSwitchComparisonPrevious -= value; } }

        public event EventHandler OnSwitchComparisonNext { add { InternalModel.OnSwitchComparisonNext += value; } remove { InternalModel.OnSwitchComparisonNext -= value; } }

        public DoubleTapPrevention(ITimerModel model)
        {
            InternalModel = model;
        }

        protected bool CheckDoubleTap()
        {
            TimeSpan? lastSplit = null;
            if (CurrentState.Settings.DoubleTapPrevention)
            {
                var index = CurrentState.CurrentSplitIndex - 1;
                while (index >= 0)
                {
                    if (CurrentState.Run[index].SplitTime.RealTime != null)
                    {
                        lastSplit = CurrentState.Run[index].SplitTime.RealTime.Value;
                        break;
                    }
                    index--;
                }
            }
            if (!CurrentState.Settings.DoubleTapPrevention
                || (CurrentState.CurrentPhase == TimerPhase.Running
                && (lastSplit == null || TimeStamp.Now - CurrentState.StartTime > lastSplit + Delay)
                && CurrentState.CurrentTime.RealTime > CurrentState.TimePausedAt + Delay)
                || (CurrentState.CurrentPhase == TimerPhase.Paused
                && TimeStamp.Now - CurrentState.StartTime > CurrentState.TimePausedAt + Delay)
                || (CurrentState.CurrentPhase == TimerPhase.Ended
                && TimeStamp.Now - CurrentState.StartTime > CurrentState.CurrentTime.RealTime + LongDelay)
                || (CurrentState.CurrentPhase == TimerPhase.NotRunning
                && TimeStamp.Now - CurrentState.StartTime > Delay))
                return true;
            return false;
        }

        public void Start()
        {
            if (CheckDoubleTap())
                InternalModel.Start();
        }

        public void Split()
        {
            if (CheckDoubleTap())
                InternalModel.Split();
        }

        public void SkipSplit()
        {
            if (CheckDoubleTap())
                InternalModel.SkipSplit();
        }

        public void UndoSplit()
        {
            if (CheckDoubleTap())
                InternalModel.UndoSplit();
        }

        public void Reset() => Reset(true);

        public void Reset(bool updateSplits = true)
        {
            if (CheckDoubleTap())
                InternalModel.Reset(updateSplits);
        }

        public void Pause()
        {
            if (CheckDoubleTap())
                InternalModel.Pause();
        }

        public void UndoAllPauses()
        {
            InternalModel.UndoAllPauses();
        }

        public void ScrollUp() => InternalModel.ScrollUp();

        public void ScrollDown() => InternalModel.ScrollDown();

        public void SwitchComparisonPrevious() => InternalModel.SwitchComparisonPrevious();

        public void SwitchComparisonNext() => InternalModel.SwitchComparisonNext();

        public void InitializeGameTime() => InternalModel.InitializeGameTime();
    }
}
