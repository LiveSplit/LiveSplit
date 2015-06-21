using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public struct CategoryEmbeds
    {
        private Embeds embeds;

        public bool EmbedGame { get { return embeds["game"]; } set { embeds["game"] = value; } }
        public bool EmbedVariables { get { return embeds["variables"]; } set { embeds["variables"] = value; } }

        public CategoryEmbeds(
            bool embedGame = false, 
            bool embedVariables = false)
        {
            embeds = new Embeds();
            EmbedGame = embedGame;
            EmbedVariables = embedVariables;
        }

        public override string ToString()
        {
            return embeds.ToString();
        }
    }
}
