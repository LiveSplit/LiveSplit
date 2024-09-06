using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;

namespace LiveSplit.Video;

public partial class VideoSettings : UserControl
{
    public string MRL => HttpUtility.UrlPathEncode("file:///" + VideoPath.Replace('\\', '/').Replace("%", "%25"));
    public string VideoPath { get; set; }
    public TimeSpan Offset { get; set; }
    public new float Height { get; set; }
    public new float Width { get; set; }
    public LayoutMode Mode { get; set; }

    protected ITimeFormatter TimeFormatter { get; set; }

    public string OffsetString
    {
        get => TimeFormatter.Format(Offset);
        set
        {
            if (Regex.IsMatch(value, "[^0-9:.,-]"))
            {
                return;
            }

            try
            {
                Offset = TimeSpanParser.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }

    public VideoSettings()
    {
        InitializeComponent();

        TimeFormatter = new ShortTimeFormatter();

        VideoPath = "";
        Width = 200;
        Height = 200;
        Offset = TimeSpan.Zero;

        txtVideoPath.DataBindings.Add("Text", this, "VideoPath", false, DataSourceUpdateMode.OnPropertyChanged);
        txtOffset.DataBindings.Add("Text", this, "OffsetString");
    }

    public void SetSettings(XmlNode node)
    {
        var element = (XmlElement)node;
        VideoPath = SettingsHelper.ParseString(element["VideoPath"]);
        OffsetString = SettingsHelper.ParseString(element["Offset"]);
        Height = SettingsHelper.ParseFloat(element["Height"]);
        Width = SettingsHelper.ParseFloat(element["Width"]);
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
        SettingsHelper.CreateSetting(document, parent, "VideoPath", VideoPath) ^
        SettingsHelper.CreateSetting(document, parent, "Offset", OffsetString) ^
        SettingsHelper.CreateSetting(document, parent, "Height", Height) ^
        SettingsHelper.CreateSetting(document, parent, "Width", Width);
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "Video Files|*.avi;*.mpeg;*.mpg;*.mp4;*.mov;*.wmv;*.m4v;*.flv;*.mkv;*.ogg|All Files (*.*)|*.*"
        };
        if (File.Exists(VideoPath))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(VideoPath);
            dialog.FileName = Path.GetFileName(VideoPath);
        }

        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            VideoPath = txtVideoPath.Text = dialog.FileName;
        }
    }

    private void VideoSettings_Load(object sender, EventArgs e)
    {
        if (Mode == LayoutMode.Horizontal)
        {
            trkHeightWidth.DataBindings.Clear();
            trkHeightWidth.Minimum = 100;
            trkHeightWidth.Maximum = 400;
            trkHeightWidth.DataBindings.Add("Value", this, "Width", false, DataSourceUpdateMode.OnPropertyChanged);
            lblHeightWidth.Text = "Width:";
        }
        else
        {
            trkHeightWidth.DataBindings.Clear();
            trkHeightWidth.Minimum = 100;
            trkHeightWidth.Maximum = 300;
            trkHeightWidth.DataBindings.Add("Value", this, "Height", false, DataSourceUpdateMode.OnPropertyChanged);
            lblHeightWidth.Text = "Height:";
        }
    }
}
