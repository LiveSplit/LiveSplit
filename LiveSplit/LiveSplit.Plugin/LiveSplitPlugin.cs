using CLROBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
