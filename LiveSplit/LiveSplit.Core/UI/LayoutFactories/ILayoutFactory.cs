using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.LayoutFactories
{
    public interface ILayoutFactory
    {
        ILayout Create(LiveSplitState state);
    }
}
