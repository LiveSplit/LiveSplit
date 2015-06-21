using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public struct RunEmbeds
    {
        private Embeds embeds;

        public bool EmbedGame 
        { 
            get { return embeds["game"]; } 
            set { embeds["game"] = value; } 
        }

        public RunEmbeds(
            bool embedGame = false)
        {
            embeds = new Embeds();
            EmbedGame = embedGame;
        }

        public override string ToString()
        {
            return embeds.ToString();
        }
    }
}
