using System.IO;

namespace LiveSplit.UI.LayoutSavers
{
    public interface ILayoutSaver
    {
        void Save(ILayout layout, Stream stream);
    }
}
