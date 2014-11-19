using System;

namespace LiveSplit.UI.Components
{
    public interface ILayoutComponent
    {
        String Path { get; set; }
        IComponent Component { get; set; }
    }
}
