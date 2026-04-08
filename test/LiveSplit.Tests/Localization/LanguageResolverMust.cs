using System;
using System.Globalization;
using System.IO;
using System.Text;

using LiveSplit.Localization;

using Xunit;

namespace LiveSplit.Tests.Localization;

public class LanguageResolverMust
{
    [Fact]
    public void PreferConfiguredLanguageOverCurrentUiCulture()
    {
        string localizationDirectory = CreateTemporaryLocalizationDirectory();
        try
        {
            UiTextCatalog.Initialize(localizationDirectory);
            LanguageResolver.SetCurrentLanguageSetting("zh-CN");

            AppLanguage language = LanguageResolver.ResolveCurrentCultureLanguage(new CultureInfo("en-US"));

            Assert.Equal("zh-CN", language.Code);
        }
        finally
        {
            DeleteDirectory(localizationDirectory);
            ResetLocalizationState();
        }
    }

    [Fact]
    public void FallBackToCurrentUiCultureWhenConfiguredLanguageIsAuto()
    {
        string localizationDirectory = CreateTemporaryLocalizationDirectory();
        try
        {
            UiTextCatalog.Initialize(localizationDirectory);
            LanguageResolver.SetCurrentLanguageSetting(string.Empty);

            AppLanguage language = LanguageResolver.ResolveCurrentCultureLanguage(new CultureInfo("en-US"));

            Assert.Equal("en-US", language.Code);
        }
        finally
        {
            DeleteDirectory(localizationDirectory);
            ResetLocalizationState();
        }
    }

    private static string CreateTemporaryLocalizationDirectory()
    {
        string rootDirectory = Path.Combine(Path.GetTempPath(), "LiveSplit.LocalizationTests", Guid.NewGuid().ToString("N"));
        string localizationDirectory = Path.Combine(rootDirectory, "Localization");
        Directory.CreateDirectory(localizationDirectory);

        File.WriteAllText(
            Path.Combine(localizationDirectory, "zh-CN.json"),
            """
            {
              "code": "zh-CN",
              "displayName": "\u7b80\u4f53\u4e2d\u6587",
              "cultureName": "zh-CN",
              "keys": {},
              "sources": {}
            }
            """,
            new UTF8Encoding(false));

        return rootDirectory;
    }

    private static void DeleteDirectory(string path)
    {
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    private static void ResetLocalizationState()
    {
        LanguageResolver.SetCurrentLanguageSetting(string.Empty);
        UiTextCatalog.Initialize(AppDomain.CurrentDomain.BaseDirectory);
    }
}
