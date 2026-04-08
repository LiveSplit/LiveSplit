using System;

namespace LiveSplit.Localization;

public sealed class AppLanguage : IEquatable<AppLanguage>
{
    public static readonly AppLanguage Auto = new(string.Empty, "Follow System", string.Empty);
    public static readonly AppLanguage English = new(LanguageResolver.EnglishCultureName, "English", LanguageResolver.EnglishCultureName);

    public string Code { get; }
    public string DisplayName { get; }
    public string CultureName { get; }

    public bool IsAuto => string.IsNullOrWhiteSpace(Code);
    public bool IsDefault => string.Equals(Code, LanguageResolver.EnglishCultureName, StringComparison.OrdinalIgnoreCase);
    public bool RequiresLocalization => !IsAuto && !IsDefault;

    public AppLanguage(string code, string displayName, string cultureName = null)
    {
        Code = code?.Trim() ?? string.Empty;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? Code : displayName.Trim();
        CultureName = string.IsNullOrWhiteSpace(cultureName) ? Code : cultureName.Trim();
    }

    public bool Equals(AppLanguage other)
    {
        return other != null && string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as AppLanguage);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Code);
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
