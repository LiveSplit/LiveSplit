using System;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

using Timer = System.Timers.Timer;

namespace LiveSplit.AutoSplittingRuntime;

public class ASRComponent : LogicComponent
{
    private readonly TimerModel model;
    private readonly ComponentSettings settings;
    private readonly Form parentForm;
    private Timer updateTimer;

    static ASRComponent()
    {
        try
        {
            ASRLoader.LoadASR();
        }
        catch { }
    }

    public ASRComponent(LiveSplitState state)
    {
        parentForm = state.Form;

        model = new TimerModel() { CurrentState = state };

        settings = new ComponentSettings(model);

        InitializeUpdateTimer();
    }

    public ASRComponent(LiveSplitState state, string scriptPath)
    {
        model = new TimerModel() { CurrentState = state };

        settings = new ComponentSettings(model, scriptPath);

        InitializeUpdateTimer();
    }

    private void InitializeUpdateTimer()
    {
        updateTimer = new Timer() { Interval = 15 };
        updateTimer.Elapsed += UpdateTimerElapsed;
        updateTimer.Start();
    }

    public override string ComponentName => "Auto Splitting Runtime";

    public override void Dispose()
    {
        updateTimer.Elapsed -= UpdateTimerElapsed;
        updateTimer.Dispose();
        updateTimer = null;
        settings.runtime?.Dispose();
    }

    public override XmlNode GetSettings(XmlDocument document)
    {
        return settings.GetSettings(document);
    }

    public override Control GetSettingsControl(LayoutMode mode)
    {
        return settings;
    }

    public override void SetSettings(XmlNode settings)
    {
        this.settings.SetSettings(settings);
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

    public void UpdateTimerElapsed(object sender, EventArgs e)
    {
        // This refresh timer behavior is similar to the ASL refresh timer

        // Disable timer, to wait for execution of this iteration to
        // finish. This can be useful if blocking operations like
        // showing a message window are used.
        updateTimer?.Stop();

        try
        {
            InvokeIfNeeded(() =>
            {
                if (settings.runtime != null)
                {
                    settings.runtime.Step();

                    try
                    {
                        if (
                            settings.previousMap == null
                            || settings.previousWidgets == null
                            || settings.runtime.AreSettingsChanged(settings.previousMap, settings.previousWidgets)
                        )
                        {
                            settings.BuildTree();
                        }
                    }
                    catch { }

                    // Poll the tick rate and modify the update interval if it has been changed
                    double tickRate = settings.runtime.TickRate().TotalMilliseconds;

                    if (updateTimer != null && tickRate != updateTimer.Interval)
                    {
                        updateTimer.Interval = tickRate;
                    }
                }
            });
        }
        finally
        {
            updateTimer?.Start();
        }
    }

    private void InvokeIfNeeded(Action x)
    {
        if (parentForm != null && parentForm.InvokeRequired)
        {
            parentForm.Invoke(x);
        }
        else
        {
            x();
        }
    }
}
