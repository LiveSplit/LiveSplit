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
    private bool _fontSnapshotCleanupDone;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Belt-and-braces: if Dispose runs before OnFormClosing did
            // (rare — e.g. some test paths), still run the snapshot cleanup
            // so we don't leak the user's picks or dispose Fonts that are
            // still aliased into LayoutSettings/components.
            EnsureFontSnapshotCleanupDone();

            if (_fontOverrideSnapshots != null)
            {
                foreach (FontOverrides snapshot in _fontOverrideSnapshots)
                {
                    // Snapshot Fonts were null'd out by the cleanup above so
                    // this just drops the FontOverrides shells.
                    snapshot?.Dispose();
                }
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
        // The actual restoration (component XML settings + font overrides) is
        // done centrally in OnFormClosing → EnsureFontSnapshotCleanupDone so
        // the same cleanup also runs when the user closes the form via the
        // X button (which sets DialogResult to Cancel but does NOT fire
        // this click handler).
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

            // Snapshot font overrides for cancel. We deliberately do NOT
            // Clone() the Fonts: keeping the original references lets the
            // cleanup logic in EnsureFontSnapshotCleanupDone() tell apart
            // "user picked something new" (current.Font != snapshot.Font)
            // from "untouched" (==), which is how it decides what to dispose.
            if (lc != null)
            {
                _fontOverrideSnapshots.Add(SnapshotFontOverrides(lc.FontOverrides));
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
        EnableMouseWheelForwarding(page);
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

        // Add the page to the tab control first so it has the real ClientSize
        // before children Anchor against it. Without this, Anchor=Right is
        // computed against the unparented TabPage default (~200 px) and the
        // child stays narrow until something else forces re-layout.
        tabControl.TabPages.Add(page);

        var fontPanel = new FontOverridePanel();
        fontPanel.Bind(fontOverrides, Layout.Settings, usedFonts);
        fontPanel.Location = new Point(0, 0);

        // Stretch fontPanel across the TabPage width instead of pinning it
        // to settingsControl.Width. Some Settings designers are 459 px wide
        // (Splits / DetailedTimer / Subsplits) while the TabPage interior is
        // ~478 px — pinning to 459 leaves a visible ~19 px gap on the right
        // and makes those tabs look "weirdly spaced" relative to other tabs.
        fontPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        // Clear the default 3-px Margin so the panel doesn't contribute extra
        // horizontal extent and spuriously trigger the horizontal scrollbar
        // (Anchor=Right already stretches it exactly to the visible width).
        fontPanel.Margin = Padding.Empty;

        settingsControl.Dock = DockStyle.None;
        settingsControl.Location = new Point(0, fontPanel.Height);
        settingsControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        settingsControl.Margin = Padding.Empty;

        page.Controls.Add(fontPanel);
        page.Controls.Add(settingsControl);

        // Set AutoScrollMinSize.Height to the exact bottom of the settings
        // control. Width=0 lets WinForms auto-detect horizontal extent from
        // the (now Margin-less) children, which equals the page client width
        // and therefore never triggers the horizontal scrollbar.
        page.AutoScrollMinSize = new Size(
            0,
            fontPanel.Height + settingsControl.Height);

        EnableMouseWheelForwarding(page);
    }

    /// <summary>
    /// Subscribe to MouseWheel on focusable child controls inside <paramref name="scrollTarget"/>
    /// that natively consume the wheel for value-change behavior (NumericUpDown / ComboBox /
    /// TrackBar), and forward those events to <paramref name="scrollTarget"/> so the
    /// surrounding TabPage can still be scrolled when one of these is focused. Controls that
    /// have meaningful inner scrolling of their own (multi-line TextBox, ListBox, ...) are
    /// left untouched so their built-in wheel handling continues to work.
    /// </summary>
    private static void EnableMouseWheelForwarding(ScrollableControl scrollTarget)
    {
        HookMouseWheel(scrollTarget, scrollTarget);
    }

    private static void HookMouseWheel(Control parent, ScrollableControl scrollTarget)
    {
        foreach (Control child in parent.Controls)
        {
            if (child != scrollTarget && ShouldForwardWheelFrom(child))
            {
                child.MouseWheel += (s, e) => ForwardWheelToScrollTarget(e, scrollTarget);
            }

            if (child.HasChildren)
            {
                HookMouseWheel(child, scrollTarget);
            }
        }
    }

    private static bool ShouldForwardWheelFrom(Control control)
    {
        return control is NumericUpDown
            || control is TrackBar
            || control is ComboBox;
    }

    private static void ForwardWheelToScrollTarget(MouseEventArgs e, ScrollableControl scrollTarget)
    {
        // Mark the event as handled so NumericUpDown / ComboBox / TrackBar skip
        // their spin/select-next behavior (they check HandledMouseEventArgs.Handled
        // before applying the wheel).
        if (e is HandledMouseEventArgs hme)
        {
            hme.Handled = true;
        }

        if (!scrollTarget.VerticalScroll.Visible)
        {
            return;
        }

        int linesPerNotch = SystemInformation.MouseWheelScrollLines;
        if (linesPerNotch <= 0 || linesPerNotch > 100)
        {
            // SPI_GETWHEELSCROLLLINES returns -1 for "page" or 0 for "no scroll";
            // both should fall back to a sane default for our purposes.
            linesPerNotch = 3;
        }

        int notches = e.Delta / SystemInformation.MouseWheelScrollDelta;
        const int PixelsPerLine = 20;
        int scrollDelta = -notches * linesPerNotch * PixelsPerLine;

        // AutoScrollPosition has a quirky API: the getter returns the negated
        // scroll offset and the setter takes the positive position. Translate
        // both ways to preserve the current X while updating Y.
        int currentY = -scrollTarget.AutoScrollPosition.Y;
        int newY = Math.Max(0, currentY + scrollDelta);

        scrollTarget.AutoScrollPosition = new Point(
            -scrollTarget.AutoScrollPosition.X,
            newY);
    }

    /// <summary>
    /// Shallow-copy of <paramref name="source"/> that intentionally shares the
    /// Font references with it (no <c>Font.Clone()</c>). The snapshot is used
    /// purely as a "what did the world look like when the dialog opened?"
    /// record so that <see cref="EnsureFontSnapshotCleanupDone"/> can compare
    /// references (==) to decide whether a slot was changed by the user.
    /// </summary>
    private static FontOverrides SnapshotFontOverrides(FontOverrides source)
    {
        return new FontOverrides
        {
            OverrideTimerFont = source.OverrideTimerFont,
            TimerFont = source.TimerFont,
            OverrideTimesFont = source.OverrideTimesFont,
            TimesFont = source.TimesFont,
            OverrideTextFont = source.OverrideTextFont,
            TextFont = source.TextFont,
        };
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Run before base so the layout sees the restored state before any
        // other FormClosing subscriber reacts.
        EnsureFontSnapshotCleanupDone();
        base.OnFormClosing(e);
    }

    /// <summary>
    /// Idempotent finalization step for the per-component snapshots created
    /// in <see cref="AddComponents"/>. Disposes only the Fonts that are no
    /// longer needed:
    /// <list type="bullet">
    /// <item><description>On Cancel/X-button, disposes the Fonts the user
    /// picked during this session and reinstalls the original references on
    /// <c>current</c>.</description></item>
    /// <item><description>On OK, disposes only those original Fonts that
    /// the user actually replaced; untouched slots (where <c>current</c>
    /// and <c>snapshot</c> share the same reference) are left alive because
    /// they are still in use by the live layout.</description></item>
    /// </list>
    /// After running, <c>snapshot.*Font</c> are set to <c>null</c> so the
    /// subsequent <c>snapshot.Dispose()</c> call in <see cref="Dispose"/> is
    /// a no-op for Fonts.
    /// </summary>
    private void EnsureFontSnapshotCleanupDone()
    {
        if (_fontSnapshotCleanupDone)
        {
            return;
        }

        if (_fontOverrideSnapshots == null || _layoutComponents == null
            || Components == null || ComponentSettings == null)
        {
            return;
        }

        bool cancelPath = DialogResult != DialogResult.OK;

        if (cancelPath)
        {
            // Restore component XML settings so the X-button path behaves
            // like the Cancel button (it didn't before this change — that
            // was a latent bug).
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].SetSettings(ComponentSettings[i]);
            }
        }

        for (int i = 0; i < _layoutComponents.Count; i++)
        {
            FontOverrides snapshot = _fontOverrideSnapshots[i];
            FontOverrides current = _layoutComponents[i].FontOverrides;

            if (cancelPath)
            {
                // Dispose Fonts the user picked in this session (current
                // differs from snapshot) and reinstall the original
                // references onto current. After this, current owns the
                // originals again.
                if (!ReferenceEquals(current.TimerFont, snapshot.TimerFont))
                {
                    current.TimerFont?.Dispose();
                }
                if (!ReferenceEquals(current.TimesFont, snapshot.TimesFont))
                {
                    current.TimesFont?.Dispose();
                }
                if (!ReferenceEquals(current.TextFont, snapshot.TextFont))
                {
                    current.TextFont?.Dispose();
                }

                current.OverrideTimerFont = snapshot.OverrideTimerFont;
                current.TimerFont = snapshot.TimerFont;
                current.OverrideTimesFont = snapshot.OverrideTimesFont;
                current.TimesFont = snapshot.TimesFont;
                current.OverrideTextFont = snapshot.OverrideTextFont;
                current.TextFont = snapshot.TextFont;
            }
            else
            {
                // OK path: current holds the user's final picks. Dispose the
                // originals that were actually replaced; leave untouched
                // slots alone (current still references them, disposing
                // would be use-after-free in the next render).
                if (!ReferenceEquals(snapshot.TimerFont, current.TimerFont))
                {
                    snapshot.TimerFont?.Dispose();
                }
                if (!ReferenceEquals(snapshot.TimesFont, current.TimesFont))
                {
                    snapshot.TimesFont?.Dispose();
                }
                if (!ReferenceEquals(snapshot.TextFont, current.TextFont))
                {
                    snapshot.TextFont?.Dispose();
                }
            }

            // Detach Fonts from snapshot so the upcoming snapshot.Dispose()
            // in our Dispose() is a no-op for Fonts (we either disposed
            // them above or transferred ownership back to current).
            snapshot.TimerFont = null;
            snapshot.TimesFont = null;
            snapshot.TextFont = null;
        }

        _fontSnapshotCleanupDone = true;
    }
}
