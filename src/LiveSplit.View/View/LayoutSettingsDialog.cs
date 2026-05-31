using LiveSplit.Localization;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.View;

public partial class LayoutSettingsDialog : Form
{
    public Options.LayoutSettings Settings { get; set; }
    public new UI.ILayout Layout { get; set; }
    public List<XmlNode> ComponentSettings { get; set; }
    public List<IComponent> Components { get; set; }

    private readonly List<FontOverrides> _fontOverrideSnapshots;
    private readonly List<LayoutComponent> _layoutComponents;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (FontOverrides snapshot in _fontOverrideSnapshots)
            {
                snapshot?.Dispose();
            }

            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    public LayoutSettingsDialog(Options.LayoutSettings settings, UI.ILayout layout, IComponent tabComponent = null)
    {
        InitializeComponent();
        Settings = settings;
        Layout = layout;
        ComponentSettings = [];
        Components = [];
        _fontOverrideSnapshots = [];
        _layoutComponents = [];
        AddNewTab("Layout", new LayoutSettingsControl(settings, layout));
        AddComponents(tabComponent);
        UiLocalizer.Apply(this, LanguageResolver.ResolveCurrentCultureLanguage());
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].SetSettings(ComponentSettings[i]);
        }

        // Restore font overrides from snapshots.
        //
        // Do not dispose the live current fonts here. The snapshot was produced
        // by FontOverrides.Clone(), so its font objects are different references
        // from the current ones even when the user changed nothing. Disposing on
        // reference inequality would therefore dispose the live overrides
        // unconditionally. Those Font objects can still be aliased into
        // LayoutSettings.*Font (via FontOverrides.ApplyTo) and captured by
        // component fields or label caches during render/update.
        for (int i = 0; i < _layoutComponents.Count; i++)
        {
            FontOverrides snapshot = _fontOverrideSnapshots[i];
            FontOverrides current = _layoutComponents[i].FontOverrides;

            current.OverrideTimerFont = snapshot.OverrideTimerFont;
            current.TimerFont = snapshot.TimerFont?.Clone() as Font;
            current.OverrideTimesFont = snapshot.OverrideTimesFont;
            current.TimesFont = snapshot.TimesFont?.Clone() as Font;
            current.OverrideTextFont = snapshot.OverrideTextFont;
            current.TextFont = snapshot.TextFont?.Clone() as Font;
        }

        DialogResult = DialogResult.Cancel;
        Close();
    }

    protected void AddComponents(IComponent tabComponent = null)
    {
        foreach (ILayoutComponent layoutComponent in Layout.LayoutComponents)
        {
            IComponent component = layoutComponent.Component;
            Control settingsControl = component.GetSettingsControl(Layout.Mode);
            var lc = layoutComponent as LayoutComponent;

            object[] fontAttr = component.GetType().GetCustomAttributes(typeof(GlobalFontConsumerAttribute), true);
            GlobalFont usedFonts = fontAttr.Length > 0
                ? ((GlobalFontConsumerAttribute)fontAttr[0]).UsedGlobalFonts
                : GlobalFont.None;

            // Snapshot font overrides for cancel
            if (lc != null)
            {
                _fontOverrideSnapshots.Add((FontOverrides)lc.FontOverrides.Clone());
                _layoutComponents.Add(lc);
            }

            bool showFontPanel = lc != null && usedFonts != GlobalFont.None;

            // Create a tab if component has settings OR if it should show font overrides
            if (settingsControl != null || showFontPanel)
            {
                Control tabContent;
                if (settingsControl != null && showFontPanel)
                {
                    AddSettingsWithFontOverrideTab(
                        component.ComponentName,
                        settingsControl,
                        lc.FontOverrides,
                        usedFonts);
                    ComponentSettings.Add(component.GetSettings(new XmlDocument()));
                    Components.Add(component);

                    if (component == tabComponent)
                    {
                        tabControl.SelectTab(tabControl.TabPages.Count - 1);
                    }

                    continue;
                }
                else if (showFontPanel)
                {
                    // No settings control: show only font panel
                    var fontPanel = new FontOverridePanel();
                    fontPanel.Bind(lc.FontOverrides, Layout.Settings, usedFonts);
                    tabContent = fontPanel;
                }
                else
                {
                    tabContent = settingsControl;
                }

                AddNewTab(component.ComponentName, tabContent);
                ComponentSettings.Add(component.GetSettings(new XmlDocument()));
                Components.Add(component);

                if (component == tabComponent)
                {
                    tabControl.SelectTab(tabControl.TabPages.Count - 1);
                }
            }
        }
    }

    protected void AddNewTab(string name, Control control)
    {
        var page = new TabPage(name);
        control.Location = new Point(0, 0);
        page.Controls.Add(control);
        page.AutoScroll = true;
        page.Name = name;
        tabControl.TabPages.Add(page);
    }

    protected void AddSettingsWithFontOverrideTab(
        string name,
        Control settingsControl,
        FontOverrides fontOverrides,
        GlobalFont usedFonts)
    {
        var page = new TabPage(name)
        {
            AutoScroll = true,
            Name = name,
        };

        var fontPanel = new FontOverridePanel();
        fontPanel.Bind(fontOverrides, Layout.Settings, usedFonts);
        fontPanel.Location = new Point(0, 0);
        fontPanel.Width = settingsControl.Width;

        settingsControl.Dock = DockStyle.None;
        settingsControl.Location = new Point(0, fontPanel.Height);

        page.Controls.Add(fontPanel);
        page.Controls.Add(settingsControl);
        page.AutoScrollMinSize = new Size(
            settingsControl.Width,
            fontPanel.Height + settingsControl.Height);

        tabControl.TabPages.Add(page);
    }
}
