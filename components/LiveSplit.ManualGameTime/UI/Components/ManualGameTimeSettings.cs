using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

public partial class ManualGameTimeSettings : UserControl
{
    public bool UseSegmentTimes { get; set; }

    public ManualGameTimeSettings()
    {
        InitializeComponent();

        UseSegmentTimes = true;

        rdoSegmentTimes.DataBindings.Add("Checked", this, "UseSegmentTimes", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        UseSegmentTimes = SettingsHelper.ParseBool(element["UseSegmentTimes"]);
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        XmlElement parent = document.CreateElement("Settings");
        CreateSettingsNode(document, parent);
        return parent;
    }

    public int GetSettingsHashCode()
    {
        return CreateSettingsNode(null, null);
    }

    private int CreateSettingsNode(XmlDocument document, XmlElement parent)
    {
        return SettingsHelper.CreateSetting(document, parent, "Version", "1.4") ^
        SettingsHelper.CreateSetting(document, parent, "UseSegmentTimes", UseSegmentTimes);
    }
}
