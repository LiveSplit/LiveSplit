using System;

namespace LiveSplit.UI.Components;

/// <summary>
/// Declares which global layout fonts a component consumes.
/// This controls which font override rows appear in the Layout Editor.
/// Components without this attribute default to <see cref="GlobalFont.None"/>
/// (no font override panel shown).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class GlobalFontConsumerAttribute : Attribute
{
    public GlobalFont UsedGlobalFonts { get; }

    public GlobalFontConsumerAttribute(GlobalFont usedGlobalFonts)
    {
        UsedGlobalFonts = usedGlobalFonts;
    }
}
