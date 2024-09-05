using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(LiveSplit.WorldRecord.UI.Components.WorldRecordFactory))]

namespace LiveSplit.WorldRecord.UI.Components;

public class WorldRecordFactory : IComponentFactory
{
    public string ComponentName => "World Record";

    public string Description => "Shows the World Record for the run";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new WorldRecordComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.WorldRecord.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
