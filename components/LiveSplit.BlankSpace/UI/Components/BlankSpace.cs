using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class BlankSpace : IComponent
{
    public BlankSpaceSettings Settings { get; set; }

    public string ComponentName => "Blank Space";

    public float VerticalHeight => Settings.SpaceHeight;

    public float MinimumWidth => 20;

    public float HorizontalWidth => Settings.SpaceWidth;

    public float MinimumHeight => 20;

    public float PaddingTop => 0f;
    public float PaddingLeft => 0f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 0f;

    public IDictionary<string, Action> ContextMenuControls => null;

    public BlankSpace()
    {
        Settings = new BlankSpaceSettings();
    }

    public static void DrawBackground(Graphics g, Color settingsColor1, Color settingsColor2,
        float width, float height, GradientType gradientType)
    {
        if (settingsColor1.A > 0
        || (gradientType != GradientType.Plain
        && settingsColor2.A > 0))
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        gradientType == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        settingsColor1,
                        gradientType == GradientType.Plain
                        ? settingsColor1
                        : settingsColor2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height)
    {
        DrawBackground(g, Settings.BackgroundColor, Settings.BackgroundColor2, width, height, Settings.BackgroundGradient);
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height);
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

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
