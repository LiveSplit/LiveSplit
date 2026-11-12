using LiveSplit.Model.Input;
using LiveSplit.Options;
using System;
using System.Windows.Forms;
using System.Xml;
using Xunit;

namespace LiveSplit.Tests.Options;

public class HotkeyProfileMust
{
    [Fact]
    public void DefaultToNoTransparencyToggleHotkey()
    {
        var profile = new HotkeyProfile();

        Assert.Null(profile.ToggleTransparency);
    }

    [Fact]
    public void PreserveTransparencyToggleHotkeyWhenCloned()
    {
        var original = new HotkeyProfile
        {
            ToggleTransparency = new KeyOrButton(Keys.F1)
        };

        var clone = (HotkeyProfile)original.Clone();

        Assert.Equal(original.ToggleTransparency, clone.ToggleTransparency);
    }

    [Fact]
    public void PreserveTransparencyToggleHotkeyInXml()
    {
        var document = new XmlDocument();
        var original = new HotkeyProfile
        {
            ToggleTransparency = new KeyOrButton(Keys.F1)
        };
        XmlElement element = original.ToXml(document);

        HotkeyProfile parsed = HotkeyProfile.FromXml(element, new Version(1, 8, 18));

        Assert.Equal(original.ToggleTransparency, parsed.ToggleTransparency);
    }

    [Fact]
    public void ReadMissingTransparencyToggleHotkeyAsNone()
    {
        var document = new XmlDocument();
        var original = new HotkeyProfile
        {
            ToggleTransparency = new KeyOrButton(Keys.F1)
        };
        XmlElement element = original.ToXml(document);
        XmlElement toggleTransparency = element["ToggleTransparency"];
        element.RemoveChild(toggleTransparency);

        HotkeyProfile parsed = HotkeyProfile.FromXml(element, new Version(1, 8, 18));

        Assert.Null(parsed.ToggleTransparency);
    }
}
