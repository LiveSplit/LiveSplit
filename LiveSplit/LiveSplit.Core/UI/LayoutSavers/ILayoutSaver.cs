using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.LayoutSavers
{
    public interface ILayoutSaver
    {
        void Save(ILayout layout, Stream stream);
    }
}
