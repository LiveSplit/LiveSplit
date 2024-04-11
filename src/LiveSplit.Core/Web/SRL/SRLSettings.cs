using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.SRL
{
    public class SRLSettings : RaceProviderSettings
    {
        public override string Name { get => "SRL"; set { } }

        public override string DisplayName => "SRL";

        public override string WebsiteLink => "https://www.speedrunslive.com";
        public override string RulesLink => SRLRulesLink;

        public static string SRLRulesLink => "https://www.speedrunslive.com/rules-faq/rules";

        public override object Clone()
        {
            return new SRLSettings()
            {
                Enabled = Enabled,
                Name = Name
            };
        }
    }
}
