using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.LayoutFactories
{
    public class StandardLayoutFactory : ILayoutFactory
    {
        public ILayout Create(LiveSplitState state)
        {
            using (var stream = new MemoryStream(LiveSplit.Properties.Resources.DefaultLayout))
            {
                var layout = new XMLLayoutFactory(stream).Create(state);

                layout.X = layout.Y = 100;
                CenturyGothicFix(layout);

                return layout;
            }
        }

        public static void CenturyGothicFix(ILayout layout)
        {
            if (layout.Settings.TimerFont.Name != "Century Gothic")
                layout.Settings.TimerFont = new Font("Calibri", layout.Settings.TimerFont.Size, FontStyle.Bold, GraphicsUnit.Pixel);
        }
    }
}
