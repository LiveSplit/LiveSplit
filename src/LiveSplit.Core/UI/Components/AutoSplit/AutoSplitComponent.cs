﻿using LiveSplit.Model;
namespace LiveSplit.UI.Components.AutoSplit;

public abstract class AutoSplitComponent : LogicComponent
{
    protected ITimerModel Model { get; set; }
    protected IAutoSplitter AutoSplitter { get; set; }

    protected AutoSplitComponent(IAutoSplitter autoSplitter, LiveSplitState state)
    {
        Model = new TimerModel() { CurrentState = state };
        AutoSplitter = autoSplitter;
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            if (AutoSplitter.ShouldStart(state))
            {
                Model.Start();
            }
        }
        else if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
        {
            if (AutoSplitter.ShouldReset(state))
            {
                Model.Reset();
                return;
            }
            else if (AutoSplitter.ShouldSplit(state))
            {
                Model.Split();
            }

            state.IsGameTimePaused = AutoSplitter.IsGameTimePaused(state);

            System.TimeSpan? gameTime = AutoSplitter.GetGameTime(state);
            if (gameTime != null)
            {
                state.SetGameTime(gameTime);
            }
        }
    }
}
