using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(PossibleTimeSaveFactory))]

namespace LiveSplit.UI.Components;

public class PossibleTimeSaveFactory : IComponentFactory
{
    public string ComponentName => "Possible Time Save";

    public string Description => "Displays the difference between a comparison segment and the best segment, effectively showing how much time can be saved.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new PossibleTimeSave(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.PossibleTimeSave.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
