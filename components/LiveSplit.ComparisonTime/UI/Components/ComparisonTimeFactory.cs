using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(ComparisonTimeFactory))]

namespace LiveSplit.UI.Components;

public class ComparisonTimeFactory : IComponentFactory
{
    public string ComponentName => "Comparison Time";

    public string Description => "Displays the final time (or a split time/segment time) from a particular comparison.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new ComparisonTime(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.ComparisonTime.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
