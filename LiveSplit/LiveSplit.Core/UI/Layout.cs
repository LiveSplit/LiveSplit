using LiveSplit.Options;
using LiveSplit.UI.Components;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.UI
{
    public class Layout : ILayout
    {
        public const int InvalidSize = -1;

        public LayoutSettings Settings { get; set; }
        public IList<ILayoutComponent> LayoutComponents { get; set; }
        public IEnumerable<IComponent> Components
        {
            get { return LayoutComponents.Select(x => x.Component); }
        }

        public LayoutMode Mode { get; set; }

        public Layout()
        {
            LayoutComponents = new List<ILayoutComponent>();
        }

        public int VerticalWidth { get; set; }
        public int VerticalHeight { get; set; }
        public int HorizontalWidth { get; set; }
        public int HorizontalHeight { get; set; }

        public string FilePath { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public bool HasChanged { get; set; }

        public object Clone()
        {
            return new Layout()
            {
                LayoutComponents = new List<ILayoutComponent>(LayoutComponents),
                VerticalWidth = VerticalWidth,
                VerticalHeight = VerticalHeight,
                HorizontalWidth = HorizontalWidth,
                HorizontalHeight = HorizontalHeight,
                FilePath = FilePath,
                X = X,
                Y = Y,
                HasChanged = HasChanged,
                Settings = (LayoutSettings)Settings.Clone(),
                Mode = Mode
            };
        }
    }
}
