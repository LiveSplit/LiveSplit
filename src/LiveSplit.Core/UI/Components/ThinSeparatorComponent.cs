﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class ThinSeparatorComponent : IComponent
{
    public float PaddingTop => 0f;
    public float PaddingLeft => 0f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 0f;

    public bool LockToBottom { get; set; }

    public GraphicsCache Cache { get; set; }

    protected LineComponent Line { get; set; }

    public float VerticalHeight => 1f;

    public float MinimumWidth => 0f;

    public float HorizontalWidth => 1f;

    public float MinimumHeight => 0f;

    public ThinSeparatorComponent()
    {
        Line = new LineComponent(1, Color.White);
        Cache = new GraphicsCache();
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        Region oldClip = g.Clip;
        System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
        System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        g.Clip = new Region();
        Line.LineColor = state.LayoutSettings.ThinSeparatorsColor;
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

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        Region oldClip = g.Clip;
        System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
        System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        g.Clip = new Region();
        Line.LineColor = state.LayoutSettings.ThinSeparatorsColor;
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

    public string ComponentName
        => "Thin Separator";

    public Control GetSettingsControl(LayoutMode mode)
    {
        throw new NotImplementedException();
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        throw new NotImplementedException();
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        throw new NotImplementedException();
    }

    public string UpdateName => throw new NotSupportedException();

    public string XMLURL => throw new NotSupportedException();

    public string UpdateURL => throw new NotSupportedException();

    public Version Version => throw new NotSupportedException();

    public IDictionary<string, Action> ContextMenuControls
        => null;

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
        GC.SuppressFinalize(this);
    }
}
