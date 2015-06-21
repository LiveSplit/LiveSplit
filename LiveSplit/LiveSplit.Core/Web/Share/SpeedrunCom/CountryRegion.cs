using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class CountryRegion
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }

        private CountryRegion() { }

        public static CountryRegion Parse(SpeedrunComClient client, dynamic regionElement)
        {
            var region = new CountryRegion();

            region.Code = regionElement.code as string;
            region.Name = regionElement.names.international as string;
            region.JapaneseName = regionElement.names.japanese as string;

            return region;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
