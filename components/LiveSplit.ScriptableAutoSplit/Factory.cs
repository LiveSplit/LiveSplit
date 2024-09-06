using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(Factory))]

namespace LiveSplit.UI.Components;

public class Factory : IComponentFactory
{
    public string ComponentName => "Scriptable Auto Splitter";
    public string Description => "Allows scripts written in the ASL language to define the splitting behaviour.";
    public ComponentCategory Category => ComponentCategory.Control;
    public Version Version => Version.Parse("1.8.29");

    public string UpdateName => ComponentName;
    public string UpdateURL => "http://livesplit.org/update/";
    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.ScriptableAutoSplit.xml";

    public IComponent Create(LiveSplitState state)
    {
        return new ASLComponent(state);
    }

    public IComponent Create(LiveSplitState state, string script)
    {
        return new ASLComponent(state, script);
    }
}
