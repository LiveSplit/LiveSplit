﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class SeparatorComponent : IComponent
{
    public float PaddingTop => 0;
    public float PaddingLeft => 0;
    public float PaddingBottom => 0;
    public float PaddingRight => 0;

    public float DisplayedSize { get; set; }
    public bool UseSeparatorColor { get; set; }
    public bool LockToBottom { get; set; }

    protected LineComponent Line { get; set; }

    public GraphicsCache Cache { get; set; }

    public float VerticalHeight => 2f;

    public float MinimumWidth => 0;
    public float HorizontalWidth => 2f;

    public float MinimumHeight => 0;

    public SeparatorComponent()
    {
        Line = new LineComponent(2, Color.White);
        DisplayedSize = 2f;
        UseSeparatorColor = true;
        LockToBottom = false;
        Cache = new GraphicsCache();
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        if (DisplayedSize > 0)
        {
            Region oldClip = g.Clip;
            System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
            System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = UseSeparatorColor ? state.LayoutSettings.SeparatorsColor : state.LayoutSettings.ThinSeparatorsColor;
            float scale = g.Transform.Elements.First();
            float newHeight = Math.Max((int)((DisplayedSize * scale) + 0.5f), 1) / scale;
            Line.VerticalHeight = newHeight;
            if (LockToBottom)
            {
                g.TranslateTransform(0, 2f - newHeight);
            }
            else if (DisplayedSize > 1)
            {
                g.TranslateTransform(0, (2f - newHeight) / 2f);
            }

            Line.DrawVertical(g, state, width, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        if (DisplayedSize > 0)
        {
            Region oldClip = g.Clip;
            System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
            System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = UseSeparatorColor ? state.LayoutSettings.SeparatorsColor : state.LayoutSettings.ThinSeparatorsColor;
            float scale = g.Transform.Elements.First();
            float newWidth = Math.Max((int)((DisplayedSize * scale) + 0.5f), 1) / scale;
            if (LockToBottom)
            {
                g.TranslateTransform(2f - newWidth, 0);
            }
            else if (DisplayedSize > 1)
            {
                g.TranslateTransform((2f - newWidth) / 2f, 0);
            }

            Line.HorizontalWidth = newWidth;
            Line.DrawHorizontal(g, state, height, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }
    }

    public string ComponentName
        => "----------------------------------------------------------------------------";

    public Control GetSettingsControl(LayoutMode mode)
    {
        return null;
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return document.CreateElement("SeparatorSettings");
    }

    public string UpdateName => throw new NotSupportedException();

    public string XMLURL => throw new NotSupportedException();

    public string UpdateURL => throw new NotSupportedException();

    public Version Version => throw new NotSupportedException();

    public IDictionary<string, Action> ContextMenuControls => null;

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        Cache.Restart();
        Cache["DisplayedSize"] = DisplayedSize;
        Cache["UseSeparatorColor"] = UseSeparatorColor;
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

    public int GetSettingsHashCode()
    {
        return 1;
    }
}
