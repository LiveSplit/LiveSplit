using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Ruleset
    {
        public bool ShowMilliseconds { get; private set; }
        public bool RequiresVerification { get; private set; }
        public bool RequiresVideo { get; private set; }

        private Ruleset() { }

        public static Ruleset Parse(SpeedrunComClient client, dynamic rulesetElement)
        {
            var ruleset = new Ruleset();

            var properties = rulesetElement.Properties as IDictionary<string, dynamic>;

            ruleset.ShowMilliseconds = properties["show-milliseconds"];
            ruleset.RequiresVerification = properties["require-verification"];
            ruleset.RequiresVideo = properties["require-video"];

            return ruleset;
        }
    }
}
