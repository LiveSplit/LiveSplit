using LiveSplit.Model;
using System.Drawing;
using System.IO;

namespace LiveSplit.UI.LayoutFactories
{
    public class StandardLayoutFactory : ILayoutFactory
    {
        public ILayout Create(LiveSplitState state)
        {
            using (var stream = new MemoryStream(Properties.Resources.DefaultLayout))
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
