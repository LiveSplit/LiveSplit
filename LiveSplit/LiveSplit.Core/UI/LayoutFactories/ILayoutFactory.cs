using LiveSplit.Model;

namespace LiveSplit.UI.LayoutFactories
{
    public interface ILayoutFactory
    {
        ILayout Create(LiveSplitState state);
    }
}
