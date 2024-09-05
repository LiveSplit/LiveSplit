using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(RunPredictionFactory))]

namespace LiveSplit.UI.Components;

public class RunPredictionFactory : IComponentFactory
{
    public string ComponentName => "Run Prediction";

    public string Description => "Displays what the final run time would be if the run continues at the same pace as a set comparison.";

    public ComponentCategory Category => ComponentCategory.Information;

    public IComponent Create(LiveSplitState state)
    {
        return new RunPrediction(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.RunPrediction.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
