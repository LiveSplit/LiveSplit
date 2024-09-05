using System;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.ManualGameTime.UI.Components;

public partial class ShitSplitter : Form
{
    protected ITimerModel Model { get; set; }
    protected ManualGameTimeSettings Settings { get; set; }

    public ShitSplitter(LiveSplitState state, ManualGameTimeSettings settings)
    {
        InitializeComponent();
        Model = new TimerModel()
        {
            CurrentState = state
        };
        Settings = settings;
    }

    private void txtGameTime_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == '\r')
        {
            try
            {
                string[] timeSpans = txtGameTime.Text.Replace(" ", "").Split('+');
                TimeSpan totalTime = TimeSpan.Zero;
                foreach (string time in timeSpans)
                {
                    totalTime += TimeSpanParser.Parse(time);
                }

                TimeSpan? newGameTime = totalTime + (Settings.UseSegmentTimes ? Model.CurrentState.CurrentTime.GameTime : TimeSpan.Zero);
                Model.CurrentState.SetGameTime(newGameTime);
                Model.Split();
                txtGameTime.Text = "";
            }
            catch { }

            e.Handled = true;
        }
    }
}
