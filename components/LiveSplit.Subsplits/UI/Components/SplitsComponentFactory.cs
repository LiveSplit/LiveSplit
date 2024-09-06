using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(SplitsComponentFactory))]

namespace LiveSplit.UI.Components;

public class SplitsComponentFactory : IComponentFactory
{
    public string ComponentName => "Subsplits";

    public string Description => "Displays a list of split times and deltas in relation to a comparison. Only shows subsplits when relevant";

    public ComponentCategory Category => ComponentCategory.List;

    public IComponent Create(LiveSplitState state)
    {
        return new SplitsComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Subsplits.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
