using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Country
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }

        private Country() { }

        public static Country Parse(SpeedrunComClient client, dynamic countryElement)
        {
            var country = new Country();

            country.Code = countryElement.code as string;
            country.Name = countryElement.names.international as string;
            country.JapaneseName = countryElement.names.japanese as string;

            return country;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
