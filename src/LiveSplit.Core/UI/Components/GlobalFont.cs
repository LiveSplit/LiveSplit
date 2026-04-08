using System;

namespace LiveSplit.UI.Components;

/// <summary>
/// Flags enum identifying which global layout fonts a component consumes.
/// </summary>
[Flags]
public enum GlobalFont
{
    None = 0,
    TimerFont = 1,
    TimesFont = 2,
    TextFont = 4,
    All = TimerFont | TimesFont | TextFont
}
