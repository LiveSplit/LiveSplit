using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.UI.Components
{
    public interface IDeactivatableComponent : IComponent
    {
        bool Activated { get; set; }
    }
}
