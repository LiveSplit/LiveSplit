using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                LayoutComponents = new List<ILayoutComponent>(this.LayoutComponents),
                VerticalWidth = this.VerticalWidth,
                VerticalHeight = this.VerticalHeight,
                HorizontalWidth = this.HorizontalWidth,
                HorizontalHeight = this.HorizontalHeight,
                FilePath = this.FilePath,
                X = this.X,
                Y = this.Y,
                HasChanged = this.HasChanged,
                Settings = (LayoutSettings)this.Settings.Clone(),
                Mode = this.Mode
            };
        }
    }
}
