using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(SoundFactory))]

namespace LiveSplit.UI.Components;

public class SoundFactory : IComponentFactory
{
    public string ComponentName => "Sound Effects";

    public string Description => "Plays sound effects for different situations.";

    public ComponentCategory Category => ComponentCategory.Media;

    public IComponent Create(LiveSplitState state)
    {
        return new SoundComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Sound.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
