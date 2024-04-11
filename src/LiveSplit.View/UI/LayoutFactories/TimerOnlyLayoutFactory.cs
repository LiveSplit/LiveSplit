using LiveSplit.Model;
using LiveSplit.Options.SettingsFactories;
using LiveSplit.UI.Components;

namespace LiveSplit.UI.LayoutFactories
{
    public class TimerOnlyLayoutFactory : ILayoutFactory
    {
        public ILayout Create(LiveSplitState state)
        {
            var layout = new Layout();
            var component = ComponentManager.LoadLayoutComponent("LiveSplit.Timer.dll", state);
            layout.LayoutComponents.Add(component);
            layout.VerticalWidth = 252;
            layout.VerticalHeight = 50;
            layout.HorizontalWidth = 252;
            layout.HorizontalHeight = 50;
            layout.X = 100;
            layout.Y = 100;
            layout.Mode = LayoutMode.Vertical;
            layout.Settings = new StandardLayoutSettingsFactory().Create();
            StandardLayoutFactory.CenturyGothicFix(layout);
            return layout;
        }
    }
}
