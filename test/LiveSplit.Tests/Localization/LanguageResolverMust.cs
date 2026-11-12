using LiveSplit.Localization;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.UI.Components;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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

    [Fact]
    public void LocalizeDataBoundListDisplayMember()
    {
        string localizationDirectory = CreateTemporaryLocalizationDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(localizationDirectory, "Localization", "zh-CN.json"),
                """
                {
                  "code": "zh-CN",
                  "displayName": "简体中文",
                  "cultureName": "zh-CN",
                  "keys": {},
                  "sources": {
                    "Timer": "计时器"
                  }
                }
                """,
                new UTF8Encoding(false));

            UiTextCatalog.Initialize(localizationDirectory);
            LanguageResolver.SetCurrentLanguageSetting("zh-CN");
            AppLanguage language = LanguageResolver.ResolveCurrentCultureLanguage(new CultureInfo("en-US"));

            RunInSta(() =>
            {
                using var form = new Form();
                using var listBox = new ListBox
                {
                    DisplayMember = nameof(LayoutComponent.DisplayName)
                };
                form.Controls.Add(listBox);
                listBox.DataSource = new[]
                {
                    new LayoutComponent("LiveSplit.Timer.dll", new LiveSplit.UI.Components.Timer())
                };

                UiLocalizer.Apply(form, language);

                Assert.Single(listBox.Items);
                Assert.Equal("计时器", listBox.GetItemText(listBox.Items[0]));
            });
        }
        finally
        {
            DeleteDirectory(localizationDirectory);
            ResetLocalizationState();
        }
    }

    [Fact]
    public void LocalizePreviousSegmentComparisonSuffix()
    {
        string localizationDirectory = CreateTemporaryLocalizationDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(localizationDirectory, "Localization", "zh-CN.json"),
                """
                {
                  "code": "zh-CN",
                  "displayName": "简体中文",
                  "cultureName": "zh-CN",
                  "keys": {},
                  "sources": {
                    "Previous Segment": "上一分段",
                    "Best": "最佳"
                  }
                }
                """,
                new UTF8Encoding(false));

            UiTextCatalog.Initialize(localizationDirectory);
            LanguageResolver.SetCurrentLanguageSetting("zh-CN");
            _ = new StandardComparisonGeneratorsFactory();

            var state = new LiveSplitState(null, null, null, null, null);
            var component = new PreviousSegment(state)
            {
                Settings = { Comparison = "Best Segments" }
            };

            Assert.Equal("上一分段 (最佳)", component.ComponentName);
        }
        finally
        {
            DeleteDirectory(localizationDirectory);
            ResetLocalizationState();
        }
    }

    private static void RunInSta(Action action)
    {
        Exception exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
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
