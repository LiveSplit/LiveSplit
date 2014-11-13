using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.UI.Components.AutoSplit
{
    public abstract class AutoSplitComponent : LogicComponent
    {
        protected ITimerModel Model { get; set; }
        protected IAutoSplitter AutoSplitter { get; set; }

        protected AutoSplitComponent(IAutoSplitter autoSplitter, LiveSplitState state)
        {
            Model = new TimerModel() { CurrentState = state };
            AutoSplitter = autoSplitter;
        }

        public override void Update(IInvalidator invalidator, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AutoSplitter.ShouldStart(state))
                {
                    Model.Start();
                }
            }
            else if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
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

                var isLoading = AutoSplitter.IsGameTimePaused(state);
                if (isLoading != null)
                    state.IsGameTimePaused = isLoading;

                var gameTime = AutoSplitter.GetGameTime(state);
                if (gameTime != null)
                    state.SetGameTime(gameTime);
            }
        }
    }
}
