using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

public partial class CollectorSettings : UserControl
{

    public LayoutMode Mode { get; set; }

    private readonly string UploadKeyFile = "Livesplit.TheRun/uploadkey.txt";
    private readonly string UploadKeyFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string UploadKey => GetUploadKey();

    public string Path { get; set; }
    public bool IsStatsUploadingEnabled { get; set; }
    public bool IsLiveTrackingEnabled { get; set; }

    public CollectorSettings()
    {
        InitializeComponent();

        chkStatsUploadEnabled.DataBindings.Add("Checked", this, "IsStatsUploadingEnabled",
            false, DataSourceUpdateMode.OnPropertyChanged);
        chkLiveTrackingEnabled.DataBindings.Add("Checked", this, "IsLiveTrackingEnabled",
            false, DataSourceUpdateMode.OnPropertyChanged);

        Path = "";
        IsStatsUploadingEnabled = true;
        IsLiveTrackingEnabled = true;
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;

        Version version = SettingsHelper.ParseVersion(element["Version"]);
        Path = SettingsHelper.ParseString(element["Path"]);
        txtPath.Text = GetUploadKey();
        IsStatsUploadingEnabled = element["IsStatsUploadingEnabled"] == null || SettingsHelper.ParseBool(element["IsStatsUploadingEnabled"]);
        IsLiveTrackingEnabled = element["IsLiveTrackingEnabled"] == null || SettingsHelper.ParseBool(element["IsLiveTrackingEnabled"]);
    }

    private string GetUploadKey()
    {
        if (!string.IsNullOrEmpty(Path))
        {
            string key = Path;
            SaveUploadKey(key);
            return key;
        }

        if (!string.IsNullOrEmpty(txtPath.Text))
        {
            return txtPath.Text;
        }

        string filePath = System.IO.Path.Combine(UploadKeyFolder, UploadKeyFile);
        if (!File.Exists(filePath))
        {
            return "";
        }

        return File.ReadAllText(filePath).Trim();
    }

    private void SaveUploadKey(string key)
    {
        string filePath = System.IO.Path.Combine(UploadKeyFolder, UploadKeyFile);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, key);
        Path = "";
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
        return SettingsHelper.CreateSetting(document, parent, "Version", "1.0.0") ^
            SettingsHelper.CreateSetting(document, parent,
                "IsStatsUploadingEnabled", IsStatsUploadingEnabled) ^
            SettingsHelper.CreateSetting(document, parent,
                "IsLiveTrackingEnabled", IsLiveTrackingEnabled);
    }

    private void txtPath_TextChanged(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void txtPath_Leave(object sender, EventArgs e)
    {
        SaveUploadKey(txtPath.Text);
    }
}
