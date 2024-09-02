﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class ComponentRendererComponent : ComponentRenderer, IComponent
{
    public float VerticalHeight
    {
        get
        {
            CalculateOverallSize(LayoutMode.Vertical);
            return OverallSize;
        }
    }
    public float HorizontalWidth
    {
        get
        {
            CalculateOverallSize(LayoutMode.Horizontal);
            return OverallSize;
        }
    }

    public float PaddingTop => VisibleComponents.Any() ? VisibleComponents.First().PaddingTop : 0;
    public float PaddingLeft => VisibleComponents.Any() ? VisibleComponents.First().PaddingLeft : 0;
    public float PaddingBottom => VisibleComponents.Any() ? VisibleComponents.Last().PaddingBottom : 0;
    public float PaddingRight => VisibleComponents.Any() ? VisibleComponents.Last().PaddingRight : 0;

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        Render(g, state, width, 0, LayoutMode.Vertical, clipRegion);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        Render(g, state, 0, height, LayoutMode.Horizontal, clipRegion);
    }

    public string ComponentName => throw new NotSupportedException();

    public Control GetSettingsControl(LayoutMode mode)
    {
        throw new NotSupportedException();
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        throw new NotSupportedException();
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        throw new NotSupportedException();
    }

    public IDictionary<string, Action> ContextMenuControls
        => VisibleComponents
            .Select(x => x.ContextMenuControls)
            .Where(x => x != null).SelectMany(x => x)
            .ToDictionary(x => x.Key, x => x.Value);

    public void Dispose()
    {
        foreach (IComponent component in VisibleComponents)
        {
            component.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
