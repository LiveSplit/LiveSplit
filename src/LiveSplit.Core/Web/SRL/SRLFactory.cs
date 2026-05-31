using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.Web.SRL;

public class SRLFactory : IRaceProviderFactory
{
    public string UpdateName => "SRL";

    public string XMLURL => "";

    public string UpdateURL => "";

    public Version Version => new();

    public RaceProviderAPI Create(ITimerModel model, RaceProviderSettings settings)
    {
        return SpeedRunsLiveAPI.Instance;
    }

    public RaceProviderSettings CreateSettings()
    {
        return new SRLSettings();
    }
}
