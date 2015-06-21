using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Players
    {
        public PlayersType Type { get; private set; }
        public int Value { get; private set; }

        private Players() { }

        public static Players Parse(SpeedrunComClient client, dynamic playersElement)
        {
            var players = new Players();

            players.Value = (int)playersElement.value;
            players.Type = playersElement.type == "exactly" ? PlayersType.Exactly : PlayersType.UpTo;

            return players;
        }

        public override string ToString()
        {
            return Type.ToString() + " " + Value;
        }
    }
}
