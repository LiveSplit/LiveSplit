using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Guest
    {
        public string Name { get; private set; }

        #region Links

        private Lazy<ReadOnlyCollection<Run>> runs;

        public ReadOnlyCollection<Run> Runs { get { return runs.Value; } }

        #endregion

        private Guest() { }

        public static Guest Parse(SpeedrunComClient client, dynamic guestElement)
        {
            var guest = new Guest();

            guest.Name = guestElement.name;
            guest.runs = new Lazy<ReadOnlyCollection<Run>>(() => client.Runs.GetRuns(guestName: guest.Name).ToList().AsReadOnly());

            return guest;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
