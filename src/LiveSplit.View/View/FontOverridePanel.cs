using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.View;

public partial class FontOverridePanel : UserControl
{
    private FontOverrides _fontOverrides;
    private Options.LayoutSettings _globalSettings;

    // Snapshot of the Font references at Bind time. The btn*Font_Click handlers
    // use these to tell apart "the Font that already existed when the dialog
    // opened" (owned by LayoutSettingsDialog's snapshot — must NOT be disposed
    // here) from "a Font we created in this dialog session via FontChanged"
    // (orphaned the moment the user picks again — safe to dispose).
    private Font _origTimerFont;
    private Font _origTimesFont;
    private Font _origTextFont;

    public FontOverridePanel()
    {
        InitializeComponent();
    }

    public void Bind(FontOverrides fontOverrides, Options.LayoutSettings globalSettings, GlobalFont usedFonts = GlobalFont.All)
    {
        _fontOverrides = fontOverrides;
        _globalSettings = globalSettings;
        _origTimerFont = fontOverrides.TimerFont;
        _origTimesFont = fontOverrides.TimesFont;
        _origTextFont = fontOverrides.TextFont;
        UpdateUI();
        ApplyFontFilter(usedFonts);
    }

    private void ApplyFontFilter(GlobalFont usedFonts)
    {
        bool showTimer = usedFonts.HasFlag(GlobalFont.TimerFont);
        bool showTimes = usedFonts.HasFlag(GlobalFont.TimesFont);
        bool showText = usedFonts.HasFlag(GlobalFont.TextFont);

        SetRowVisible(0, showTimer);
        SetRowVisible(1, showTimes);
        SetRowVisible(2, showText);

        int visibleRows = (showTimer ? 1 : 0) + (showTimes ? 1 : 0) + (showText ? 1 : 0);
        AdjustHeight(visibleRows);
    }

    private void AdjustHeight(int visibleRows)
    {
        if (visibleRows >= _tableLayout.RowCount)
        {
            return;
        }

        // Force a layout pass so _tableLayout.Height reflects the Dock=Fill
        // size inside the GroupBox client area (~86 px) rather than the
        // unparented designer Size (101 px). Using the designer value
        // over-shrinks the panel by about 9 px per hidden row, which clips
        // the bottom of the "Choose..." button (23 px) in the 1-visible-row
        // case (e.g. Timer / Title / Text / Counter / Splits.SplitsComponent).
        PerformLayout();

        int hiddenRows = _tableLayout.RowCount - visibleRows;
        Height -= _tableLayout.Height * hiddenRows / _tableLayout.RowCount;
    }

    private void SetRowVisible(int row, bool visible)
    {
        foreach (Control control in _tableLayout.Controls)
        {
            if (_tableLayout.GetRow(control) == row)
            {
                control.Visible = visible;
            }
        }

        _tableLayout.RowStyles[row] = visible
            ? new RowStyle(SizeType.Percent, 33.33f)
            : new RowStyle(SizeType.Absolute, 0f);
    }

    private void UpdateUI()
    {
        // Timer
        _chkOverrideTimerFont.Checked = _fontOverrides.OverrideTimerFont;
        _btnTimerFont.Enabled = _fontOverrides.OverrideTimerFont;
        UpdateFontLabel(_lblTimerFont, _fontOverrides.OverrideTimerFont ? _fontOverrides.TimerFont : null, _globalSettings.TimerFont);

        // Times
        _chkOverrideTimesFont.Checked = _fontOverrides.OverrideTimesFont;
        _btnTimesFont.Enabled = _fontOverrides.OverrideTimesFont;
        UpdateFontLabel(_lblTimesFont, _fontOverrides.OverrideTimesFont ? _fontOverrides.TimesFont : null, _globalSettings.TimesFont);

        // Text
        _chkOverrideTextFont.Checked = _fontOverrides.OverrideTextFont;
        _btnTextFont.Enabled = _fontOverrides.OverrideTextFont;
        UpdateFontLabel(_lblTextFont, _fontOverrides.OverrideTextFont ? _fontOverrides.TextFont : null, _globalSettings.TextFont);
    }

    private static void UpdateFontLabel(Label label, Font overrideFont, Font globalFont)
    {
        if (overrideFont != null)
        {
            label.Text = SettingsHelper.FormatFont(overrideFont);
            label.ForeColor = SystemColors.ControlText;
        }
        else
        {
            label.Text = globalFont != null
                ? $"Using global: {SettingsHelper.FormatFont(globalFont)}"
                : "Using global";
            label.ForeColor = SystemColors.GrayText;
        }
    }

    private void chkOverrideTimerFont_CheckedChanged(object sender, EventArgs e)
    {
        _fontOverrides.OverrideTimerFont = _chkOverrideTimerFont.Checked;
        _btnTimerFont.Enabled = _chkOverrideTimerFont.Checked;
        UpdateFontLabel(_lblTimerFont, _chkOverrideTimerFont.Checked ? _fontOverrides.TimerFont : null, _globalSettings.TimerFont);
    }

    private void chkOverrideTimesFont_CheckedChanged(object sender, EventArgs e)
    {
        _fontOverrides.OverrideTimesFont = _chkOverrideTimesFont.Checked;
        _btnTimesFont.Enabled = _chkOverrideTimesFont.Checked;
        UpdateFontLabel(_lblTimesFont, _chkOverrideTimesFont.Checked ? _fontOverrides.TimesFont : null, _globalSettings.TimesFont);
    }

    private void chkOverrideTextFont_CheckedChanged(object sender, EventArgs e)
    {
        _fontOverrides.OverrideTextFont = _chkOverrideTextFont.Checked;
        _btnTextFont.Enabled = _chkOverrideTextFont.Checked;
        UpdateFontLabel(_lblTextFont, _chkOverrideTextFont.Checked ? _fontOverrides.TextFont : null, _globalSettings.TextFont);
    }

    private void btnTimerFont_Click(object sender, EventArgs e)
    {
        Font current = _fontOverrides.TimerFont ?? _globalSettings.TimerFont;
        // Scale down for dialog display, matching LayoutSettingsControl pattern
        var dialogFont = new Font(current.FontFamily.Name, current.Size / 50f * 18f, current.Style, GraphicsUnit.Pixel);
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(dialogFont, 7, 20);
        dialog.FontChanged += (s, ev) =>
        {
            Font newFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            Font previous = _fontOverrides.TimerFont;
            // Scale back up to the size the Timer component expects
            _fontOverrides.TimerFont = new Font(newFont.FontFamily.Name, newFont.Size / 18f * 50f, newFont.Style, GraphicsUnit.Pixel);
            DisposePreviousDialogFont(previous, _origTimerFont);
            UpdateFontLabel(_lblTimerFont, _fontOverrides.TimerFont, _globalSettings.TimerFont);
        };
        dialog.ShowDialog(FindForm());
    }

    private void btnTimesFont_Click(object sender, EventArgs e)
    {
        Font current = _fontOverrides.TimesFont ?? _globalSettings.TimesFont;
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(current, 11, 26);
        dialog.FontChanged += (s, ev) =>
        {
            Font previous = _fontOverrides.TimesFont;
            _fontOverrides.TimesFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            DisposePreviousDialogFont(previous, _origTimesFont);
            UpdateFontLabel(_lblTimesFont, _fontOverrides.TimesFont, _globalSettings.TimesFont);
        };
        dialog.ShowDialog(FindForm());
    }

    private void btnTextFont_Click(object sender, EventArgs e)
    {
        Font current = _fontOverrides.TextFont ?? _globalSettings.TextFont;
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(current, 11, 26);
        dialog.FontChanged += (s, ev) =>
        {
            Font previous = _fontOverrides.TextFont;
            _fontOverrides.TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            DisposePreviousDialogFont(previous, _origTextFont);
            UpdateFontLabel(_lblTextFont, _fontOverrides.TextFont, _globalSettings.TextFont);
        };
        dialog.ShowDialog(FindForm());
    }

    /// <summary>
    /// Dispose <paramref name="previous"/> only when it was allocated during
    /// this dialog session — i.e. it's not the pre-dialog original captured at
    /// Bind. The original is owned by LayoutSettingsDialog's snapshot list and
    /// is needed there to roll back on Cancel, so we must not touch it here.
    /// </summary>
    private static void DisposePreviousDialogFont(Font previous, Font origAtBind)
    {
        if (previous != null && !ReferenceEquals(previous, origAtBind))
        {
            previous.Dispose();
        }
    }
}
