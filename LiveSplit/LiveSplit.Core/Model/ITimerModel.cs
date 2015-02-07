using LiveSplit.Model.Input;
using System;

namespace LiveSplit.Model
{
    public interface ITimerModel
    {
        LiveSplitState CurrentState { get; set; }

        event EventHandler OnSplit;
        event EventHandler OnUndoSplit;
        event EventHandler OnSkipSplit;
        event EventHandler OnStart;
        event EventHandlerT<TimerPhase> OnReset;
        event EventHandler OnPause;
        event EventHandler OnResume;
        event EventHandler OnScrollUp;
        event EventHandler OnScrollDown;
        event EventHandler OnSwitchComparisonPrevious;
        event EventHandler OnSwitchComparisonNext;
        
        void Start();
        void Split();
        void SkipSplit();
        void UndoSplit();
        void Reset(bool updateSplits = true);
        void Pause();
        void ScrollUp();
        void ScrollDown();
        void SwitchComparisonPrevious();
        void SwitchComparisonNext();
    }
}
