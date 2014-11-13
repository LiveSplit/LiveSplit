using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.Components
{
    public interface ILayoutComponent
    {
        String Path { get; set; }
        IComponent Component { get; set; }
    }
}
