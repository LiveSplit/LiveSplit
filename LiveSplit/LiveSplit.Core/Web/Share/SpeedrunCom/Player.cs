using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Player
    {
        public bool IsUser { get { return string.IsNullOrEmpty(GuestName); } }
        public string UserID { get; private set; }
        public string GuestName { get; private set; }

        private Player() { }

        public static Player Parse(SpeedrunComClient client, dynamic playerElement)
        {
            var player = new Player();

            var id = playerElement.id as string;

            if (playerElement.rel as string == "user")
                player.UserID = id;
            else
                player.GuestName = id;

            return player;
        }

        public override string ToString()
        {
            if (IsUser)
                return UserID;
            else
                return GuestName;
        }
    }
}
