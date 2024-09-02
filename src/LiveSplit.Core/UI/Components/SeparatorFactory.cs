using System;

namespace LiveSplit.UI.Components;

public class SeparatorFactory : IComponentFactory
{
    public string ComponentName
        => "Separator";

    public string Description
        => "Shows a line to separate components.";

    public ComponentCategory Category
        => ComponentCategory.Other;

    public IComponent Create(Model.LiveSplitState state)
    {
        return new SeparatorComponent();
    }

    public string UpdateName => throw new NotSupportedException();

    public string XMLURL => throw new NotSupportedException();

    public string UpdateURL => throw new NotSupportedException();

    public Version Version => throw new NotSupportedException();
}
