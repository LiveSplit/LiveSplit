using LiveSplit.Model.Input;
using LiveSplit.Options;
using LiveSplit.Options.SettingsFactories;
using LiveSplit.Options.SettingsSavers;
using LiveSplit.UI.Components;
using LiveSplit.Web.SRL;
using LiveSplit.Web.SRL.RaceViewers;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace LiveSplit.Tests.Options;

public class SettingsSerializationMust
{
    [Fact]
    public void PreserveWindowPosition()
    {
        Settings settings = CreateSettings();
        var originalRaceProviderFactories = ComponentManager.RaceProviderFactories;
        try
        {
            ComponentManager.RaceProviderFactories = new Dictionary<string, IRaceProviderFactory>();

            using var stream = new MemoryStream();
            new XMLSettingsSaver().Save(settings, stream);
            stream.Position = 0;

            ISettings loaded = new XMLSettingsFactory(stream).Create();

            Assert.Equal(2031, loaded.WindowX);
            Assert.Equal(-1211, loaded.WindowY);
            Assert.False(loaded.TransparencySupportEnabled);
        }
        finally
        {
            ComponentManager.RaceProviderFactories = originalRaceProviderFactories;
        }
    }

    [Fact]
    public void CloneWindowPosition()
    {
        Settings settings = CreateSettings();
        settings.WindowX = 10;
        settings.WindowY = 20;

        var clone = (Settings)settings.Clone();

        Assert.Equal(10, clone.WindowX);
        Assert.Equal(20, clone.WindowY);
        Assert.False(clone.TransparencySupportEnabled);
    }

    private static Settings CreateSettings()
    {
        return new Settings
        {
            HotkeyProfiles = new Dictionary<string, HotkeyProfile>
            {
                [HotkeyProfile.DefaultHotkeyProfileName] = new HotkeyProfile()
            },
            RecentSplits = [],
            RecentLayouts = [],
            RaceViewer = new SRLRaceViewer(),
            RaceProvider = [],
            ActiveAutoSplitters = [],
            ComparisonGeneratorStates = new Dictionary<string, bool>(),
            UILanguage = string.Empty,
            WindowX = 2031,
            WindowY = -1211,
            TransparencySupportEnabled = false
        };
    }
}
