using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateManager;

namespace LiveSplit.UI.Components
{
    public interface IComponentFactory : IUpdateable
    {
        /// <summary>
        /// Returns the name of the component.
        /// </summary>
        String ComponentName { get; }
        /// <summary>
        /// Returns a description of the component.
        /// </summary>
        String Description { get; }
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
