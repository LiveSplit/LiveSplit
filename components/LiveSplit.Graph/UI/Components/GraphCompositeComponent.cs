using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

namespace LiveSplit.UI.Components;

public class GraphCompositeComponent : IComponent
{
    protected GraphSettings Settings { get; set; }
    public ComponentRendererComponent InternalComponent { get; protected set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    public IDictionary<string, Action> ContextMenuControls => null;

    public GraphCompositeComponent(LiveSplitState state)
    {
        Settings = new GraphSettings()
        {
            CurrentState = state
        };
        InternalComponent = new ComponentRendererComponent();
        var components = new List<IComponent>
        {
            new GraphSeparatorComponent(Settings) { LockToBottom = true },
            new GraphComponent(Settings),
            new GraphSeparatorComponent(Settings) { LockToBottom = false }
        };
        InternalComponent.VisibleComponents = components;
        state.ComparisonRenamed += state_ComparisonRenamed;
    }

    private void state_ComparisonRenamed(object sender, EventArgs e)
    {
        var args = (RenameEventArgs)e;
        if (Settings.Comparison == args.OldName)
        {
            Settings.Comparison = args.NewName;
            ((LiveSplitState)sender).Layout.HasChanged = true;
        }
    }

    public Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public string ComponentName
        => "Graph" + (Settings.Comparison == "Current Comparison"
            ? ""
            : " (" + CompositeComparisons.GetShortComparisonName(Settings.Comparison) + ")");

    public float HorizontalWidth => InternalComponent.HorizontalWidth;

    public float MinimumHeight => InternalComponent.MinimumHeight;

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        InternalComponent.DrawHorizontal(g, state, height, clipRegion);
    }

    public float VerticalHeight => InternalComponent.VerticalHeight;

    public float MinimumWidth => InternalComponent.MinimumWidth;

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        InternalComponent.DrawVertical(g, state, width, clipRegion);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        if (invalidator != null)
        {
            InternalComponent.Update(invalidator, state, width, height, mode);
        }
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
