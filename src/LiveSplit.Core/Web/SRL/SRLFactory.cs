using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.SRL
{
    public class SRLFactory : IRaceProviderFactory
    {
        public string UpdateName => "SRL";

        public string XMLURL => "";

        public string UpdateURL => "";

        public Version Version => new Version();

        public RaceProviderAPI Create(ITimerModel model, RaceProviderSettings settings)
        {
            return SpeedRunsLiveAPI.Instance;
        }

        public RaceProviderSettings CreateSettings()
        {
            return new SRLSettings();
        }
    }
}
