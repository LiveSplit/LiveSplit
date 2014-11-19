using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public interface IComponent : IDisposable
    {
        /// <summary>
        /// Returns the name of the component.
        /// </summary>
        String ComponentName { get; }

        /// <summary>
        /// Returns the width of the component if it is rendered horizontally.
        /// </summary>
        float HorizontalWidth { get; }
        /// <summary>
        /// Returns the minimum height where the component
        /// still looks visually pleasing.
        /// </summary>
        float MinimumHeight { get; }

        /// <summary>
        /// Returns the height of the component if it is rendered vertically.
        /// </summary>
        float VerticalHeight { get; }
        /// <summary>
        /// Returns the minimum width where the component
        /// still looks visually pleasing.
        /// </summary>
        float MinimumWidth { get; }

        /// <summary>
        /// Returns the intrinsic padding of the component.
        /// <remarks>Padding is combined if two components with padding are next to each other.</remarks>
        /// </summary>
        float PaddingTop { get; }
        float PaddingBottom { get; }
        float PaddingLeft { get; }
        float PaddingRight { get; }

        /// <summary>
        /// Returns a Dictionary with all the controls available
        /// in the context menu for controlling the component.
        /// </summary>
        IDictionary<String, Action> ContextMenuControls { get; }

        /// <summary>
        /// Draws the contents of the component horizontally onto the window.
        /// </summary>
        /// <param name="g">The graphics object used for drawing</param>
        /// <param name="state">Represents the current state of LiveSplit</param>
        /// <param name="height">The height of the window and the component</param>
        /// <param name="clipRegion">The rectangle on the form that is being redrawn for this paint event.</param>
        void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion);

        /// <summary>
        /// Draws the contents of the component vertically onto the window.
        /// </summary>
        /// <param name="g">The graphics object used for drawing</param>
        /// <param name="state">Represents the current state of LiveSplit</param>
        /// <param name="width">The width of the window and the component</param>
        /// <param name="clipRegion">The rectangle on the form that is being redrawn for this paint event.</param>
        void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion);

        /// <summary>
        /// Shows a dialog where the user can configure the component.
        /// </summary>
        /// <returns>Returns in what way the dialog was closed.</returns>
        Control GetSettingsControl(LayoutMode mode);
        /// <summary>
        /// Returns the XML serialization of the component's settings.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <returns> Returns the XML serialization of the component's settings.</returns>
        XmlNode GetSettings(XmlDocument document);
        /// <summary>
        /// Sets the settings of the component based on the serialized version of the settings.
        /// </summary>
        /// <param name="settings">A serialized version of the settings that need to be set.</param>
        void SetSettings(XmlNode settings);

        /// <summary>
        /// Updates the component, checks if it has changed, and invalidates the necessary region if it needs to be redrawn.
        /// </summary>
        /// <param name="invalidator">An invalidator object. Used to invalidate a specific region on the form.</param>
        /// <param name="state">Represents the current state of LiveSplit</param>
        /// <param name="width">The width of the region that needs to be invalidated.</param>
        /// <param name="height">The height of the region that needs to be invalidated.</param>
        /// <param name="mode">The Layout Mode (Horizontal or Vertical)</param>
        void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode);
    }
}
