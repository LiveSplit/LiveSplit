using System;
using System.Globalization;
using System.IO;

using LiveSplit.Localization;

using Xunit;

namespace LiveSplit.Tests.Localization;

public class LanguageResolverMust
{
    [Fact]
    public void PreferConfiguredLanguageOverCurrentUiCulture()
    {
        try
        {
            UiTextCatalog.Initialize(GetBuiltApplicationDirectory());
            LanguageResolver.SetCurrentLanguageSetting("zh-CN");

            AppLanguage language = LanguageResolver.ResolveCurrentCultureLanguage(new CultureInfo("en-US"));

            Assert.Equal("zh-CN", language.Code);
        }
        finally
        {
            ResetLocalizationState();
        }
    }

    [Fact]
    public void FallBackToCurrentUiCultureWhenConfiguredLanguageIsAuto()
    {
        try
        {
            UiTextCatalog.Initialize(GetBuiltApplicationDirectory());
            LanguageResolver.SetCurrentLanguageSetting(string.Empty);

            AppLanguage language = LanguageResolver.ResolveCurrentCultureLanguage(new CultureInfo("en-US"));

            Assert.Equal("en-US", language.Code);
        }
        finally
        {
            ResetLocalizationState();
        }
    }

    private static string GetBuiltApplicationDirectory()
    {
        return Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "bin",
            "release"));
    }

    private static void ResetLocalizationState()
    {
        LanguageResolver.SetCurrentLanguageSetting(string.Empty);
        UiTextCatalog.Initialize(AppDomain.CurrentDomain.BaseDirectory);
    }
}
