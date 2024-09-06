using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(DetailedTimerFactory))]

namespace LiveSplit.UI.Components;

public class DetailedTimerFactory : IComponentFactory
{
    public string ComponentName => "Detailed Timer";

    public string Description => "Displays the run timer, segment timer, and segment times for up to two comparisons.";

    public ComponentCategory Category => ComponentCategory.Timer;

    public IComponent Create(LiveSplitState state)
    {
        return new DetailedTimer(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.DetailedTimer.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
