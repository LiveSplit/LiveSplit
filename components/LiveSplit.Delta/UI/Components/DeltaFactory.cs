using System;

using LiveSplit.Delta;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(DeltaFactory))]

namespace LiveSplit.Delta;

public class DeltaFactory : IComponentFactory
{
    public string ComponentName => "Delta";

    public string Description => "Displays the current delta to a comparison.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new DeltaComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Delta.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
