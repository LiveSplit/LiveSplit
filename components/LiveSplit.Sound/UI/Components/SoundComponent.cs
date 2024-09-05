using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Options;

using NAudio.Wave;

namespace LiveSplit.UI.Components;

public class SoundComponent : LogicComponent, IDeactivatableComponent
{
    public override string ComponentName => "Sound Effects";

    public bool Activated { get; set; }

    private LiveSplitState State { get; set; }
    private SoundSettings Settings { get; set; }
    private WaveOut Player { get; set; }

    public SoundComponent(LiveSplitState state)
    {
        Activated = true;

        State = state;
        Settings = new SoundSettings();
        Player = new WaveOut();

        State.OnStart += State_OnStart;
        State.OnSplit += State_OnSplit;
        State.OnSkipSplit += State_OnSkipSplit;
        State.OnUndoSplit += State_OnUndoSplit;
        State.OnPause += State_OnPause;
        State.OnResume += State_OnResume;
        State.OnReset += State_OnReset;
    }

    public override void Dispose()
    {
        State.OnStart -= State_OnStart;
        State.OnSplit -= State_OnSplit;
        State.OnSkipSplit -= State_OnSkipSplit;
        State.OnUndoSplit -= State_OnUndoSplit;
        State.OnPause -= State_OnPause;
        State.OnResume -= State_OnResume;
        State.OnReset -= State_OnReset;

        Player.Stop();
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

    public override Control GetSettingsControl(LayoutMode mode)
    {
        return Settings;
    }

    public override XmlNode GetSettings(XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public override void SetSettings(XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    private void State_OnStart(object sender, EventArgs e)
    {
        PlaySound(Settings.StartTimer, Settings.StartTimerVolume);
    }

    private void State_OnSplit(object sender, EventArgs e)
    {
        if (State.CurrentPhase == TimerPhase.Ended)
        {
            if (State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod] == null || State.Run.Last().SplitTime[State.CurrentTimingMethod] < State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod])
            {
                PlaySound(Settings.PersonalBest, Settings.PersonalBestVolume);
            }
            else
            {
                PlaySound(Settings.NotAPersonalBest, Settings.NotAPersonalBestVolume);
            }
        }
        else
        {
            string path = string.Empty;
            int volume = Settings.SplitVolume;

            int splitIndex = State.CurrentSplitIndex - 1;
            TimeSpan? timeDifference = State.Run[splitIndex].SplitTime[State.CurrentTimingMethod] - State.Run[splitIndex].Comparisons[State.CurrentComparison][State.CurrentTimingMethod];

            if (timeDifference != null)
            {
                if (timeDifference < TimeSpan.Zero)
                {
                    path = Settings.SplitAheadGaining;
                    volume = Settings.SplitAheadGainingVolume;

                    if (LiveSplitStateHelper.GetPreviousSegmentDelta(State, splitIndex, State.CurrentComparison, State.CurrentTimingMethod) > TimeSpan.Zero)
                    {
                        path = Settings.SplitAheadLosing;
                        volume = Settings.SplitAheadLosingVolume;
                    }
                }
                else
                {
                    path = Settings.SplitBehindLosing;
                    volume = Settings.SplitBehindLosingVolume;

                    if (LiveSplitStateHelper.GetPreviousSegmentDelta(State, splitIndex, State.CurrentComparison, State.CurrentTimingMethod) < TimeSpan.Zero)
                    {
                        path = Settings.SplitBehindGaining;
                        volume = Settings.SplitBehindGainingVolume;
                    }
                }
            }

            //Check for best segment
            TimeSpan? curSegment = LiveSplitStateHelper.GetPreviousSegmentTime(State, splitIndex, State.CurrentTimingMethod);

            if (curSegment != null)
            {
                if (State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod] == null || curSegment < State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod])
                {
                    path = Settings.BestSegment;
                    volume = Settings.BestSegmentVolume;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Settings.Split;
            }

            PlaySound(path, volume);
        }
    }

    private void State_OnSkipSplit(object sender, EventArgs e)
    {
        PlaySound(Settings.SkipSplit, Settings.SkipSplitVolume);
    }

    private void State_OnUndoSplit(object sender, EventArgs e)
    {
        PlaySound(Settings.UndoSplit, Settings.UndoSplitVolume);
    }

    private void State_OnPause(object sender, EventArgs e)
    {
        PlaySound(Settings.Pause, Settings.PauseVolume);
    }

    private void State_OnResume(object sender, EventArgs e)
    {
        PlaySound(Settings.Resume, Settings.ResumeVolume);
    }

    private void State_OnReset(object sender, TimerPhase e)
    {
        if (e != TimerPhase.Ended)
        {
            PlaySound(Settings.Reset, Settings.ResetVolume);
        }
    }

    private void PlaySound(string location, int volume)
    {
        Player.Stop();

        if (Activated && File.Exists(location))
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var audioFileReader = new AudioFileReader(location)
                    {
                        Volume = volume / 100f * (Settings.GeneralVolume / 100f)
                    };

                    Player.DeviceNumber = Settings.OutputDevice;
                    Player.Init(audioFileReader);
                    Player.Play();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
        }
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
