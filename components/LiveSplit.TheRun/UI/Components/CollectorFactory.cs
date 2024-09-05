using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(CollectorFactory))]

namespace LiveSplit.UI.Components;

public class CollectorFactory : IComponentFactory
{
    public string ComponentName => "therun.gg";

    public string Description => "Uploads your runs to therun.gg";

    public ComponentCategory Category => ComponentCategory.Other;

    public IComponent Create(LiveSplitState state)
    {
        return new CollectorComponent(state);
    }

    public string UpdateName => ComponentName;

    public string UpdateURL => "https://raw.githubusercontent.com/therungg/LiveSplit.TheRun/main/";
    public string XMLURL => UpdateURL + "update.LiveSplit.TheRun.xml";

    public Version Version => Version.Parse("0.3.2");
}
