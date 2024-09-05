using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(BlankSpaceFactory))]

namespace LiveSplit.UI.Components;

public class BlankSpaceFactory : IComponentFactory
{
    public string ComponentName => "Blank Space";

    public string Description => "Shows empty space with a configurable size.";

    public ComponentCategory Category => ComponentCategory.Other;

    public IComponent Create(LiveSplitState state)
    {
        return new BlankSpace();
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.BlankSpace.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
