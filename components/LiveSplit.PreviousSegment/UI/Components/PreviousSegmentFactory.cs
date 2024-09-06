using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(PreviousSegmentFactory))]

namespace LiveSplit.UI.Components;

public class PreviousSegmentFactory : IComponentFactory
{
    public string ComponentName => "Previous Segment";

    public string Description => "Displays how much time was saved or lost on the previous segment in relation to a comparison.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new PreviousSegment(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.PreviousSegment.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
