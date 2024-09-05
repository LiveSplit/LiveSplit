using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(HotkeyIndicatorFactory))]

namespace LiveSplit.UI.Components;

public class HotkeyIndicatorFactory : IComponentFactory
{
    public string ComponentName => "Hotkey Indicator";

    public string Description => "Shows whether global hotkeys are on or off. Green indicates that global hotkeys are on, while red indicates off.";

    public ComponentCategory Category => ComponentCategory.Other;

    public IComponent Create(LiveSplitState state)
    {
        return new HotkeyIndicator();
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.HotkeyIndicator.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
