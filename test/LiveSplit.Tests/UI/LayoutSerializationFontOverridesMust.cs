using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.Options.SettingsFactories;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.UI.LayoutSavers;

using Xunit;

namespace LiveSplit.Tests.UI;

public class LayoutSerializationFontOverridesMust
{
    [Fact]
    public void RoundtripFontOverrides_WhenSavedAndLoaded()
    {
        var layout = new Layout { Settings = new StandardLayoutSettingsFactory().Create() };
        var layoutComponent = new LayoutComponent("test.dll", new StubComponent())
        {
            FontOverrides = new FontOverrides
            {
                OverrideTextFont = true,
                TextFont = new Font("Arial", 14f)
            }
        };
        layout.LayoutComponents.Add(layoutComponent);

        var saver = new XMLLayoutSaver();
        using var stream = new MemoryStream();
        saver.Save(layout, stream);
        stream.Position = 0;

        var doc = new XmlDocument();
        doc.Load(stream);

        XmlNode fontOverridesNode = doc.SelectSingleNode("//FontOverrides");
        Assert.NotNull(fontOverridesNode);

        XmlNode overrideTextFontNode = doc.SelectSingleNode("//FontOverrides/OverrideTextFont");
        Assert.NotNull(overrideTextFontNode);
        Assert.Equal("True", overrideTextFontNode.InnerText);

        XmlNode textFontNode = doc.SelectSingleNode("//FontOverrides/TextFont");
        Assert.NotNull(textFontNode);
        Assert.False(string.IsNullOrEmpty(textFontNode.InnerText));
    }

    [Fact]
    public void LoadOldLayoutWithoutFontOverrides_DefaultsToNoOverride()
    {
        var layoutComponent = new LayoutComponent("test.dll", new StubComponent());

        Assert.NotNull(layoutComponent.FontOverrides);
        Assert.False(layoutComponent.FontOverrides.HasOverrides);
        Assert.False(layoutComponent.FontOverrides.OverrideTimerFont);
        Assert.False(layoutComponent.FontOverrides.OverrideTimesFont);
        Assert.False(layoutComponent.FontOverrides.OverrideTextFont);
        Assert.Null(layoutComponent.FontOverrides.TimerFont);
        Assert.Null(layoutComponent.FontOverrides.TimesFont);
        Assert.Null(layoutComponent.FontOverrides.TextFont);
    }

    [Fact]
    public void IncludeFontOverridesInHash_WhenOverridePresent()
    {
        var layout1 = new Layout { Settings = new StandardLayoutSettingsFactory().Create() };
        layout1.LayoutComponents.Add(new LayoutComponent("test.dll", new StubComponent()));

        var layout2 = new Layout { Settings = new StandardLayoutSettingsFactory().Create() };
        var lcWithOverride = new LayoutComponent("test.dll", new StubComponent())
        {
            FontOverrides = new FontOverrides
            {
                OverrideTextFont = true,
                TextFont = new Font("Arial", 14f)
            }
        };
        layout2.LayoutComponents.Add(lcWithOverride);

        var saver = new XMLLayoutSaver();
        int hash1 = saver.CreateLayoutNode(null, null, layout1);
        int hash2 = saver.CreateLayoutNode(null, null, layout2);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void OmitFontOverridesFromXml_WhenNoOverridesSet()
    {
        var layout = new Layout { Settings = new StandardLayoutSettingsFactory().Create() };
        layout.LayoutComponents.Add(new LayoutComponent("test.dll", new StubComponent()));

        var saver = new XMLLayoutSaver();
        using var stream = new MemoryStream();
        saver.Save(layout, stream);
        stream.Position = 0;

        var doc = new XmlDocument();
        doc.Load(stream);

        XmlNode fontOverridesNode = doc.SelectSingleNode("//FontOverrides");
        Assert.Null(fontOverridesNode);
    }

    /// <summary>
    /// Minimal IComponent stub for serialization tests.
    /// Only GetSettings is needed by XMLLayoutSaver.
    /// </summary>
    private sealed class StubComponent : IComponent
    {
        public string ComponentName => "Stub";

        public float HorizontalWidth => 0;
        public float MinimumHeight => 0;
        public float VerticalHeight => 0;
        public float MinimumWidth => 0;
        public float PaddingTop => 0;
        public float PaddingBottom => 0;
        public float PaddingLeft => 0;
        public float PaddingRight => 0;

        public IDictionary<string, Action> ContextMenuControls => null;

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
        public Control GetSettingsControl(LayoutMode mode) { return null; }

        public XmlNode GetSettings(XmlDocument document)
        {
            return document.CreateElement("Settings");
        }

        public void SetSettings(XmlNode settings) { }
        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
        public void Dispose() { }
    }
}
