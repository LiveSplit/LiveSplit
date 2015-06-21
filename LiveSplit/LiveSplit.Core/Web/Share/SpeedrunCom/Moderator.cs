using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Moderator
    {
        public string UserID { get; private set; }
        public ModeratorType Type { get; private set; }

        private Moderator() { }

        public static Moderator Parse(SpeedrunComClient client, KeyValuePair<string, dynamic> moderatorElement)
        {
            var moderator = new Moderator();

            moderator.UserID = moderatorElement.Key;
            moderator.Type = moderatorElement.Value as string == "moderator" 
                ? ModeratorType.Moderator 
                : ModeratorType.SuperModerator;

            return moderator;
        }

        public override string ToString()
        {
            return UserID;
        }
    }
}
