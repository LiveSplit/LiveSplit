using System;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.Racetime;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(RacetimeFactory))]

namespace LiveSplit.Racetime;

public class RacetimeFactory : IRaceProviderFactory
{
    public RaceProviderAPI Create(ITimerModel model, RaceProviderSettings settings)
    {
        RacetimeAPI.Instance.Settings = settings;
        return RacetimeAPI.Instance;
    }

    public RaceProviderSettings CreateSettings()
    {
        return new RacetimeSettings();
    }

    public string UpdateName => "Racetime Integration";

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Racetime.v2.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
