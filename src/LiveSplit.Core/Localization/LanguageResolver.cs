using System;
using System.Globalization;

namespace LiveSplit.Localization;

public static class LanguageResolver
{
    public const string EnglishCultureName = "en-US";
    public const string AutoLanguageValue = "auto";
    private static string currentLanguageSetting = string.Empty;

    public static bool IsAuto(string settingValue)
    {
        return string.IsNullOrWhiteSpace(settingValue)
            || string.Equals(settingValue, AutoLanguageValue, StringComparison.OrdinalIgnoreCase);
    }

    public static AppLanguage Resolve(string settingValue, CultureInfo systemCulture = null)
    {
        UiTextCatalog.Initialize();

        if (!IsAuto(settingValue))
        {
            return UiTextCatalog.FindLanguage(settingValue) ?? UiTextCatalog.DefaultLanguage;
        }

        return UiTextCatalog.MatchLanguage(systemCulture ?? CultureInfo.InstalledUICulture);
    }

    public static string NormalizeSettingValue(string settingValue)
    {
        if (IsAuto(settingValue))
        {
            return string.Empty;
        }

        return UiTextCatalog.FindLanguage(settingValue)?.Code ?? settingValue?.Trim() ?? string.Empty;
    }

    public static string ToSettingValue(AppLanguage language)
    {
        return language == null || language.IsAuto
            ? string.Empty
            : language.Code;
    }

    public static void SetCurrentLanguageSetting(string settingValue)
    {
        currentLanguageSetting = NormalizeSettingValue(settingValue);
    }

    public static CultureInfo ResolveCulture(string settingValue, CultureInfo systemCulture = null)
    {
        AppLanguage language = Resolve(settingValue, systemCulture);

        try
        {
            return CultureInfo.GetCultureInfo(language.CultureName);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.GetCultureInfo(EnglishCultureName);
        }
    }

    public static AppLanguage ResolveCurrentCultureLanguage(CultureInfo currentCulture = null)
    {
        if (!IsAuto(currentLanguageSetting))
        {
            return Resolve(currentLanguageSetting, currentCulture);
        }

        return UiTextCatalog.MatchLanguage(currentCulture ?? CultureInfo.CurrentUICulture);
    }
}
