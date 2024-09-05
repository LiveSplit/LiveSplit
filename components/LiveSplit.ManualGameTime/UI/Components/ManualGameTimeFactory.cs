using System;

using LiveSplit.Delta;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(ManualGameTimeFactory))]

namespace LiveSplit.Delta;

public class ManualGameTimeFactory : IComponentFactory
{
    public string ComponentName => "Manual Game Time";

    public string Description => "Allows manually entering segment times as game time.";

    public ComponentCategory Category => ComponentCategory.Control;

    public IComponent Create(LiveSplitState state)
    {
        return new ManualGameTimeComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.ManualGameTime.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
