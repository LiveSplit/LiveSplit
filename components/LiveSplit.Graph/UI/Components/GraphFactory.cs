using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(GraphFactory))]

namespace LiveSplit.UI.Components;

public class GraphFactory : IComponentFactory
{
    public string ComponentName => "Graph";

    public string Description => "Shows a graph of the current run in relation to a comparison.";

    public ComponentCategory Category => ComponentCategory.Media;

    public IComponent Create(LiveSplitState state)
    {
        return new GraphCompositeComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Graph.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
