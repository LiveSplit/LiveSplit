using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ComponentFactory(typeof(EmbeddedLayoutFactory))]

namespace LiveSplit.UI.Components
{
    public class EmbeddedLayoutFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Embedded Layout"; }
        }

        public string Description
        {
            get { return "Embeds a layout with an opposite orientation, which allows for much greater customization."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Other; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new EmbeddedLayoutComponent(state);
        }

        public string UpdateName
        {
            get { return ComponentName; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/Components/update.LiveSplit.EmbeddedLayout.xml"; }
#else
            get { return "http://livesplit.org/update/Components/update.LiveSplit.EmbeddedLayout.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/"; }
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return Version.Parse("1.0.0"); }
        }
    }
}
