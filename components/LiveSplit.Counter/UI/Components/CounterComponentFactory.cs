using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(CounterComponentFactory))]

namespace LiveSplit.UI.Components;

public class CounterComponentFactory : IComponentFactory
{
    public string ComponentName => "Counter";

    public string Description => "A labelled counter, allowing an initial value to be incremented/decremented by a specified amount.";

    public ComponentCategory Category => ComponentCategory.Other;

    public IComponent Create(LiveSplitState state)
    {
        return new CounterComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Counter.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
