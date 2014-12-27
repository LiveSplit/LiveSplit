using LiveSplit.Model;
using System;
using UpdateManager;

namespace LiveSplit.UI.Components
{
    public interface IComponentFactory : IUpdateable
    {
        /// <summary>
        /// Returns the name of the component.
        /// </summary>
        string ComponentName { get; }
        /// <summary>
        /// Returns a description of the component.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Returns the category of the component.
        /// </summary>
        ComponentCategory Category { get; }
        /// <summary>
        /// Constructs the component.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The component</returns>
        IComponent Create(LiveSplitState state);
    }
}
