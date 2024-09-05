using System;

using LiveSplit.AutoSplittingRuntime;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(ASRFactory))]

namespace LiveSplit.AutoSplittingRuntime;

public class ASRFactory : IComponentFactory
{
    public string ComponentName => "Auto Splitting Runtime";
    public string Description => "Allows auto splitters provided as WebAssembly modules to define the splitting behaviour.";
    public ComponentCategory Category => ComponentCategory.Control;
    public Version Version => Version.Parse("0.0.9");

    public string UpdateName => ComponentName;
    public string UpdateURL => "http://livesplit.org/update/";
    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.AutoSplittingRuntime.xml";

    public IComponent Create(LiveSplitState state)
    {
        return new ASRComponent(state);
    }

    public IComponent Create(LiveSplitState state, string script)
    {
        return new ASRComponent(state, script);
    }
}
