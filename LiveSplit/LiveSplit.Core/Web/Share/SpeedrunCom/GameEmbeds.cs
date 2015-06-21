using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public struct GameEmbeds
    {
        private Embeds embeds;

        public bool EmbedLevels
        {
            get { return embeds["levels"]; }
            set { embeds["levels"] = value; }
        }
        public bool EmbedCategories
        {
            get { return embeds["categories"]; }
            set { embeds["categories"] = value; }
        }
        public bool EmbedModerators
        {
            get { return embeds["moderators"]; }
            set { embeds["moderators"] = value; }
        }
        public bool EmbedPlatforms
        {
            get { return embeds["platforms"]; }
            set { embeds["platforms"] = value; }
        }
        public bool EmbedRegions
        {
            get { return embeds["regions"]; }
            set { embeds["regions"] = value; }
        }
        public bool EmbedVariables
        {
            get { return embeds["variables"]; }
            set { embeds["variables"] = value; }
        }

        public GameEmbeds(
            bool embedLevels = false,
            bool embedCategories = false,
            bool embedModerators = false,
            bool embedPlatforms = false,
            bool embedRegions = false,
            bool embedVariables = false)
        {
            embeds = new Embeds();
            EmbedLevels = embedLevels;
            EmbedCategories = embedCategories;
            EmbedModerators = embedModerators;
            EmbedPlatforms = embedPlatforms;
            EmbedRegions = embedRegions;
            EmbedVariables = embedVariables;
        }

        public override string ToString()
        {
            return embeds.ToString();
        }
    }
}
