using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Location
    {
        public Country Country { get; private set; }
        public CountryRegion Region { get; private set; }

        private Location() { }

        public static Location Parse(SpeedrunComClient client, dynamic locationElement)
        {
            var location = new Location();

            location.Country = Country.Parse(client, locationElement.country) as Country;

            if (locationElement.region != null)
                location.Region = CountryRegion.Parse(client, locationElement.region) as CountryRegion;

            return location;
        }

        public override string ToString()
        {
            if (Region == null)
                return Country.Name;
            else
                return Country.Name + " " + Region.Name;
        }
    }
}
