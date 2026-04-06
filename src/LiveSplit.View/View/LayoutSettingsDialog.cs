using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Localization;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.View;

public partial class LayoutSettingsDialog : Form
{
    public Options.LayoutSettings Settings { get; set; }
    public new UI.ILayout Layout { get; set; }
    public List<XmlNode> ComponentSettings { get; set; }
    public List<IComponent> Components { get; set; }

    private List<FontOverrides> _fontOverrideSnapshots;
    private List<LayoutComponent> _layoutComponents;

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

        // Restore font overrides from snapshots, disposing fonts set during the dialog session
        for (int i = 0; i < _layoutComponents.Count; i++)
        {
            FontOverrides snapshot = _fontOverrideSnapshots[i];
            FontOverrides current = _layoutComponents[i].FontOverrides;

            if (current.TimerFont != snapshot.TimerFont)
            {
                current.TimerFont?.Dispose();
            }

            if (current.TimesFont != snapshot.TimesFont)
            {
                current.TimesFont?.Dispose();
            }

            if (current.TextFont != snapshot.TextFont)
            {
                current.TextFont?.Dispose();
            }

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

            var fontAttr = component.GetType().GetCustomAttributes(typeof(GlobalFontConsumerAttribute), true);
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
                    // Both: stack font panel above settings
                    var container = new Panel { Dock = DockStyle.Fill };
                    var fontPanel = new FontOverridePanel();
                    fontPanel.Bind(lc.FontOverrides, Layout.Settings, usedFonts);
                    fontPanel.Dock = DockStyle.Top;
                    container.Controls.Add(settingsControl);
                    settingsControl.Dock = DockStyle.Fill;
                    container.Controls.Add(fontPanel);
                    tabContent = container;
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
}
