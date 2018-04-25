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

        private TimeStamp LastEvent;

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
            LastEvent = TimeStamp.Now - LongDelay;
        }

        protected bool CheckDoubleTap()
        {
            if (!CurrentState.Settings.HotkeyProfiles[CurrentState.CurrentHotkeyProfile].DoubleTapPrevention)
                return true;

            if (CurrentState.CurrentPhase == TimerPhase.Ended)
                return TimeStamp.Now - LastEvent > LongDelay;

            return TimeStamp.Now - LastEvent > Delay;
        }

        public void Start()
        {
            if (CheckDoubleTap())
            {
                InternalModel.Start();
                LastEvent = TimeStamp.Now;
            }
        }

        public void Split()
        {
            if (CheckDoubleTap())
            {
                InternalModel.Split();
                LastEvent = TimeStamp.Now;
            }
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
            {
                InternalModel.Reset(updateSplits);
                LastEvent = TimeStamp.Now;
            }
        }

        public void Pause()
        {
            if (CheckDoubleTap())
            {
                InternalModel.Pause();
                LastEvent = TimeStamp.Now;
            }
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
