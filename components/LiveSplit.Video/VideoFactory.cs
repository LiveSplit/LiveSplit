using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.Video;

[assembly: ComponentFactory(typeof(VideoFactory))]

namespace LiveSplit.UI.Components;

public class VideoFactory : IComponentFactory
{
    public string ComponentName => "Video";

    public string Description => "Shows a PB or WR video that is synced up to the current run time.";

    public ComponentCategory Category => ComponentCategory.Media;

    public IComponent Create(LiveSplitState state)
    {
        return new VideoComponent(state);
    }

    public string UpdateName => ComponentName;

    public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Video.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("1.8.29");
}
