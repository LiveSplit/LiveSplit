using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class GraphSeparatorComponent : IComponent
{
    protected LineComponent Line { get; set; }
    protected GraphSettings Settings { get; set; }

    public bool LockToBottom { get; set; }

    public float PaddingTop => 0f;
    public float PaddingBottom => 0f;
    public float PaddingLeft => 0f;
    public float PaddingRight => 0f;

    public GraphicsCache Cache { get; set; }

    public float VerticalHeight => 1f;

    public float MinimumWidth => 0f;

    public IDictionary<string, Action> ContextMenuControls => null;

    public GraphSeparatorComponent(GraphSettings settings)
    {
        Line = new LineComponent(1, Color.White);
        Settings = settings;
        Cache = new GraphicsCache();
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        Region oldClip = g.Clip;
        System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
        System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        g.Clip = new Region();
        Line.LineColor = Settings.GraphLinesColor;
        float scale = g.Transform.Elements.First();
        float newHeight = Math.Max((int)((1f * scale) + 0.5f), 1) / scale;
        Line.VerticalHeight = newHeight;
        if (LockToBottom)
        {
            g.TranslateTransform(0, 1f - newHeight);
        }

        Line.DrawVertical(g, state, width, clipRegion);
        g.Clip = oldClip;
        g.Transform = oldMatrix;
        g.SmoothingMode = oldMode;
    }

    public string ComponentName => "Graph Separator";

    public Control GetSettingsControl(LayoutMode mode)
    {
        throw new NotSupportedException();
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        throw new NotSupportedException();
    }

    public void SetSettings(XmlNode settings)
    {
        throw new NotSupportedException();
    }

    public float HorizontalWidth => 1f;

    public float MinimumHeight => 0f;

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        Region oldClip = g.Clip;
        System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
        System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        g.Clip = new Region();
        Line.LineColor = Settings.GraphLinesColor;
        float scale = g.Transform.Elements.First();
        float newWidth = Math.Max((int)((1f * scale) + 0.5f), 1) / scale;
        if (LockToBottom)
        {
            g.TranslateTransform(1f - newWidth, 0);
        }

        Line.HorizontalWidth = newWidth;
        Line.DrawHorizontal(g, state, height, clipRegion);
        g.Clip = oldClip;
        g.Transform = oldMatrix;
        g.SmoothingMode = oldMode;
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        Cache.Restart();
        Cache["LockToBottom"] = LockToBottom;

        if (invalidator != null && Cache.HasChanged)
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {
    }
}
