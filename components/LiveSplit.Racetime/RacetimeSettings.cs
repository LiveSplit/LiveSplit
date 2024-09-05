using System;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Options;
using LiveSplit.UI;

namespace LiveSplit.Racetime;

public class RacetimeSettings : RaceProviderSettings
{
    private readonly RacetimeSettingsControl control;

    public override string Name { get => "LiveSplit.Racetime.dll"; set { } }

    public override string DisplayName => "racetime.gg";

    public bool LoadChatHistory { get; set; } = true;
    public bool HideResults { get; set; } = false;

    public override string WebsiteLink => "https://racetime.gg";

    public override string RulesLink => "https://racetime.gg/about/rules";

    public RacetimeSettings()
    {
        control = new RacetimeSettingsControl();
    }

    public override Control GetSettingsControl()
    {
        control.Settings = this;
        return control;
    }

    public override void FromXml(XmlElement element, Version version)
    {
        base.FromXml(element, version);
        LoadChatHistory = SettingsHelper.ParseBool(element["LoadChatHistory"], true);
        HideResults = SettingsHelper.ParseBool(element["HideResults"], true);
    }

    public override XmlElement ToXml(XmlDocument document)
    {
        XmlElement e = base.ToXml(document);

        SettingsHelper.CreateSetting(document, e, "LoadChatHistory", LoadChatHistory);
        SettingsHelper.CreateSetting(document, e, "HideResults", HideResults);

        return e;
    }

    public override object Clone()
    {
        return new RacetimeSettings()
        {
            Enabled = Enabled,
            LoadChatHistory = LoadChatHistory,
            HideResults = HideResults
        };
    }
}
