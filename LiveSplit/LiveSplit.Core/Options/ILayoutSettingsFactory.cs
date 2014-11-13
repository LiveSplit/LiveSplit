using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Options
{
    public interface ILayoutSettingsFactory
    {
        LayoutSettings Create();
    }
}
