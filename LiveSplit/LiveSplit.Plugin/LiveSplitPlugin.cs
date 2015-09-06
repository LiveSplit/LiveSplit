using CLROBS;

namespace LiveSplit.Plugin
{
    public class LiveSplitPlugin : AbstractPlugin
    {
        public LiveSplitPlugin()
        {
            Name = "LiveSplit Plugin";
            Description = "420";
        }

        public override bool LoadPlugin()
        {
            API.Instance.AddImageSourceFactory(new LiveSplitImageSourceFactory());
            return true;
        }
    }
}
