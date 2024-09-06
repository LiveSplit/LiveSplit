using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(CurrentComparisonFactory))]

namespace LiveSplit.UI.Components;

public class CurrentComparisonFactory : IComponentFactory
{
    public string ComponentName => "Current Comparison";

    public string Description => "Shows which comparison you are currently comparing to.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new CurrentComparison(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.CurrentComparison.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
