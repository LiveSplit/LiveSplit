using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI;

namespace LiveSplit.AutoSplittingRuntime;

public partial class ComponentSettings : UserControl
{
    private string scriptPath;
    public string ScriptPath
    {
        get => scriptPath;
        set
        {
            if (value != scriptPath)
            {
                scriptPath = value;
                ReloadRuntime(null);
            }
        }
    }
    private readonly bool fixedScriptPath = false;

    public Runtime runtime = null;

    public SettingsMap previousMap = null;
    public Widgets previousWidgets = null;

    private static readonly LogDelegate log = (messagePtr, messageLen) =>
    {
        string message = ASRString.FromPtrLen(messagePtr, messageLen);
        Log.Info($"[Auto Splitting Runtime] {message}");
    };

    private readonly StateDelegate getState;
    private readonly Action start;
    private readonly Action split;
    private readonly Action skipSplit;
    private readonly Action undoSplit;
    private readonly Action reset;
    private readonly SetGameTimeDelegate setGameTime;
    private readonly Action pauseGameTime;
    private readonly Action resumeGameTime;

    public ComponentSettings(TimerModel model)
    {
        InitializeComponent();

        scriptPath = "";

        txtScriptPath.DataBindings.Add("Text", this, "ScriptPath", false,
            DataSourceUpdateMode.OnPropertyChanged);

        getState = () =>
        {
            return model.CurrentState.CurrentPhase switch
            {
                TimerPhase.NotRunning => 0,
                TimerPhase.Running => 1,
                TimerPhase.Paused => 2,
                TimerPhase.Ended => 3,
                _ => 0,
            };
        };
        start = model.Start;
        split = model.Split;
        skipSplit = model.SkipSplit;
        undoSplit = model.UndoSplit;
        reset = model.Reset;
        setGameTime = (ticks) => model.CurrentState.SetGameTime(new TimeSpan(ticks));
        pauseGameTime = () => model.CurrentState.IsGameTimePaused = true;
        resumeGameTime = () => model.CurrentState.IsGameTimePaused = false;
    }

    public ComponentSettings(TimerModel model, string scriptPath)
        : this(model)
    {
        ScriptPath = scriptPath;
        fixedScriptPath = true;
        btnSelectFile.Enabled = false;
        txtScriptPath.Enabled = false;
    }

    public void ReloadRuntime(SettingsMap settingsMap)
    {
        try
        {
            if (runtime != null)
            {
                runtime.Dispose();
                runtime = null;
                previousMap?.Dispose();
                previousMap = null;
                previousWidgets?.Dispose();
                previousWidgets = null;
                BuildTree();
            }

            if (!string.IsNullOrEmpty(ScriptPath))
            {
                runtime = new Runtime(
                    ScriptPath,
                    settingsMap,
                    getState,
                    start,
                    split,
                    skipSplit,
                    undoSplit,
                    reset,
                    setGameTime,
                    pauseGameTime,
                    resumeGameTime,
                    log
                );
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public void BuildTree()
    {
        settingsTable.Controls.Clear();
        settingsTable.RowCount = 0;
        settingsTable.RowStyles.Clear();

        if (runtime != null)
        {
            previousWidgets?.Dispose();
            Widgets widgets = previousWidgets = runtime.GetSettingsWidgets();

            previousMap?.Dispose();
            SettingsMap settingsMap = previousMap = runtime.GetSettingsMap();

            ulong len = widgets.GetLength();

            int margin = 3;

            for (ulong i = 0; i < len; i++)
            {
                string desc = widgets.GetDescription(i);
                string tooltip = widgets.GetTooltip(i);
                string ty = widgets.GetType(i);

                switch (ty)
                {
                    case "bool":
                    {
                        var checkbox = new CheckBox
                        {
                            Text = desc,
                            Tag = widgets.GetKey(i),
                            Margin = new Padding(margin, 0, 0, 0),
                            Checked = widgets.GetBool(i, settingsMap)
                        };
                        checkbox.CheckedChanged += Checkbox_CheckedChanged;
                        checkbox.Anchor |= AnchorStyles.Right;
                        toolTip.SetToolTip(checkbox, tooltip);
                        settingsTable.Controls.Add(checkbox, 0, settingsTable.RowStyles.Count);
                        settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, checkbox.Height));
                        break;
                    }
                    case "title":
                    {
                        int headingLevel = (int)widgets.GetHeadingLevel(i);
                        margin = 20 * headingLevel;
                        var label = new Label
                        {
                            Margin = new Padding(margin, 3, 0, 0),
                            Text = desc
                        };
                        margin += 20;
                        label.Font = new Font(label.Font.FontFamily, 10, FontStyle.Underline);
                        label.Anchor |= AnchorStyles.Right;
                        toolTip.SetToolTip(label, tooltip);
                        settingsTable.Controls.Add(label, 0, settingsTable.RowStyles.Count);
                        settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, label.Height));
                        break;
                    }
                    case "choice":
                    {
                        var label = new Label
                        {
                            Text = desc,
                            Margin = new Padding(margin, 0, 0, 0)
                        };
                        label.Anchor |= AnchorStyles.Right;
                        toolTip.SetToolTip(label, tooltip);
                        settingsTable.Controls.Add(label, 0, settingsTable.RowStyles.Count);
                        settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, label.Height));

                        var combo = new ComboBox
                        {
                            Tag = widgets.GetKey(i),
                            Margin = new Padding(margin, 0, 0, 0),
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };
                        combo.Anchor |= AnchorStyles.Right;
                        toolTip.SetToolTip(combo, tooltip);
                        ulong choicesLen = widgets.GetChoiceOptionsLength(i);
                        for (ulong choiceIndex = 0; choiceIndex < choicesLen; choiceIndex++)
                        {
                            var choice = new Choice
                            {
                                description = widgets.GetChoiceOptionDescription(i, choiceIndex),
                                key = widgets.GetChoiceOptionKey(i, choiceIndex)
                            };
                            combo.Items.Add(choice);
                        }

                        combo.SelectedIndex = (int)widgets.GetChoiceCurrentIndex(i, settingsMap);
                        combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
                        settingsTable.Controls.Add(combo, 0, settingsTable.RowStyles.Count);
                        settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, combo.Height + 5));
                        break;
                    }
                    case "file-select":
                    {
                        var button = new Button
                        {
                            Tag = new FileSelectInfo(widgets.GetKey(i), widgets.GetFileSelectFilter(i)),
                            Text = desc,
                            Margin = new Padding(margin, 0, 0, 0),
                        };
                        button.Click += FileSelect_Click;
                        button.Anchor |= AnchorStyles.Right;
                        toolTip.SetToolTip(button, tooltip);
                        settingsTable.Controls.Add(button, 0, settingsTable.RowStyles.Count);
                        settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, button.Height));
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }

    private void Combo_SelectedIndexChanged(object sender, EventArgs e)
    {
        var combo = (ComboBox)sender;
        if (combo.Tag is not string)
        {
            return;
        }

        if (combo.SelectedItem is not Choice)
        {
            return;
        }

        var choice = (Choice)combo.SelectedItem;

        if (runtime != null)
        {
            runtime.SettingsMapSetString((string)combo.Tag, choice.key);
            SettingsMap prev = previousMap;
            previousMap = runtime.GetSettingsMap();
            prev?.Dispose();
        }
    }

    private void FileSelect_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button.Tag is not FileSelectInfo)
        {
            return;
        }

        var tag = (FileSelectInfo)button.Tag;
        var dialog = new OpenFileDialog()
        {
            Filter = tag.filter
        };
        string oldWindowsPath = "";
        if (runtime != null)
        {
            using SettingsMap settingsMap = runtime.GetSettingsMap();
            if (settingsMap != null)
            {
                SettingValueRef value = settingsMap.KeyGetValue(tag.key);
                if (value != null)
                {
                    oldWindowsPath = ASRNative.wasi_to_path(value.GetString());
                }
            }
        }

        if (File.Exists(oldWindowsPath))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(oldWindowsPath);
            dialog.FileName = Path.GetFileName(oldWindowsPath);
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string newWindowsPath = dialog.FileName;
            string newWasiPath = ASRNative.path_to_wasi(newWindowsPath);
            if (runtime != null)
            {
                runtime.SettingsMapSetString(tag.key, newWasiPath);
                SettingsMap prev = previousMap;
                previousMap = runtime.GetSettingsMap();
                prev?.Dispose();
            }
        }
    }

    private class Choice
    {
        public string key;
        public string description;

        public override string ToString()
        {
            return description;
        }
    }

    private readonly struct FileSelectInfo
    {
        public FileSelectInfo(string k, string f)
        {
            key = k;
            filter = f;
        }
        public readonly string key;
        public readonly string filter;
    }

    private void Checkbox_CheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        if (checkbox.Tag is not string)
        {
            return;
        }

        if (runtime != null)
        {
            runtime.SettingsMapSetBool((string)checkbox.Tag, checkbox.Checked);
            SettingsMap prev = previousMap;
            previousMap = runtime.GetSettingsMap();
            prev?.Dispose();
        }
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        XmlElement settings_node = document.CreateElement("Settings");

        settings_node.AppendChild(SettingsHelper.ToElement(document, "Version", "1.0"));
        if (!fixedScriptPath)
        {
            settings_node.AppendChild(SettingsHelper.ToElement(document, "ScriptPath", scriptPath));
        }

        AppendCustomSettingsToXml(document, settings_node);

        return settings_node;
    }

    // Loads the settings of this component from Xml. This might happen more than once
    // (e.g. when the settings dialog is cancelled, to restore previous settings).
    public void SetSettings(XmlNode settings)
    {
        var element = (XmlElement)settings;
        if (!element.IsEmpty)
        {
            SettingsMap settingsMap = ParseCustomSettingsFromXml(element);
            if (!fixedScriptPath)
            {
                string newScriptPath = SettingsHelper.ParseString(element["ScriptPath"], string.Empty);
                if (newScriptPath != scriptPath)
                {
                    scriptPath = newScriptPath;
                    ReloadRuntime(settingsMap);
                    return;
                }
            }

            if (runtime != null)
            {
                SettingsMap prev = previousMap;
                previousMap = settingsMap ?? new SettingsMap();
                prev?.Dispose();
                runtime.SetSettingsMap(previousMap);
                return;
            }

            settingsMap?.Dispose();
        }
    }

    private void AppendCustomSettingsToXml(XmlDocument document, XmlNode parent)
    {
        XmlElement asrParent = document.CreateElement("CustomSettings");

        if (runtime != null)
        {
            using SettingsMap settingsMap = runtime.GetSettingsMap();
            if (settingsMap != null)
            {
                BuildMap(document, asrParent, settingsMap);
            }
        }

        parent.AppendChild(asrParent);
    }

    private static void BuildMap(XmlDocument document, XmlElement parent, SettingsMapRef settingsMap)
    {
        ulong len = settingsMap.GetLength();
        for (ulong i = 0; i < len; i++)
        {
            XmlElement element = BuildValue(document, settingsMap.GetValue(i));

            if (element != null)
            {
                XmlAttribute id = SettingsHelper.ToAttribute(document, "id", settingsMap.GetKey(i));
                element.Attributes.Prepend(id);
                parent.AppendChild(element);
            }
        }
    }

    private static void BuildList(XmlDocument document, XmlElement parent, SettingsListRef settingsList)
    {
        ulong len = settingsList.GetLength();
        for (ulong i = 0; i < len; i++)
        {
            XmlElement element = BuildValue(document, settingsList.Get(i));

            if (element != null)
            {
                parent.AppendChild(element);
            }
        }
    }

    private static XmlElement BuildValue(XmlDocument document, SettingValueRef value)
    {
        XmlElement element = document.CreateElement("Setting");

        string type = value.GetKind();

        XmlAttribute typeAttr = SettingsHelper.ToAttribute(document, "type", type);
        element.Attributes.Append(typeAttr);

        switch (type)
        {
            case "map":
            {
                BuildMap(document, element, value.GetMap());
                break;
            }
            case "list":
            {
                BuildList(document, element, value.GetList());
                break;
            }
            case "bool":
            {
                element.InnerText = value.GetBool().ToString(CultureInfo.InvariantCulture);
                break;
            }
            case "i64":
            {
                element.InnerText = value.GetI64().ToString(CultureInfo.InvariantCulture);
                break;
            }
            case "f64":
            {
                element.InnerText = value.GetF64().ToString(CultureInfo.InvariantCulture);
                break;
            }
            case "string":
            {
                XmlAttribute attribute = SettingsHelper.ToAttribute(document, "value", value.GetString());
                element.Attributes.Append(attribute);
                break;
            }
            default:
            {
                return null;
            }
        }

        return element;
    }

    /// <summary>
    /// Parses custom settings, stores them and updates the checked state of already added tree nodes.
    /// </summary>
    ///
    private SettingsMap ParseCustomSettingsFromXml(XmlElement data)
    {
        try
        {
            XmlElement customSettingsNode = data["CustomSettings"];

            if (customSettingsNode == null)
            {
                return null;
            }

            return ParseMap(customSettingsNode);
        }
        catch
        {
            return null;
        }
    }

    private SettingsMap ParseMap(XmlElement mapNode)
    {
        var map = new SettingsMap();

        foreach (XmlElement element in mapNode.ChildNodes)
        {
            if (element.Name != "Setting")
            {
                return null;
            }

            string id = element.Attributes["id"].Value;

            if (id == null)
            {
                return null;
            }

            SettingValue value = ParseValue(element);
            if (value == null)
            {
                return null;
            }

            map.Insert(id, value);
        }

        return map;
    }

    private SettingsList ParseList(XmlElement listNode)
    {
        var list = new SettingsList();

        foreach (XmlElement element in listNode.ChildNodes)
        {
            if (element.Name != "Setting")
            {
                return null;
            }

            SettingValue value = ParseValue(element);
            if (value == null)
            {
                return null;
            }

            list.Push(value);
        }

        return list;
    }

    private SettingValue ParseValue(XmlElement element)
    {
        string type = element.Attributes["type"].Value;

        if (type == "bool")
        {
            bool value = SettingsHelper.ParseBool(element);
            return new SettingValue(value);
        }
        else if (type == "i64")
        {
            long value = long.Parse(element.InnerText);
            return new SettingValue(value);
        }
        else if (type == "f64")
        {
            double value = SettingsHelper.ParseDouble(element);
            return new SettingValue(value);
        }
        else if (type == "string")
        {
            string value = element.Attributes["value"].Value;
            return new SettingValue(value);
        }
        else if (type == "map")
        {
            SettingsMap value = ParseMap(element);
            if (value == null)
            {
                return null;
            }

            return new SettingValue(value);
        }
        else if (type == "list")
        {
            SettingsList value = ParseList(element);
            if (value == null)
            {
                return null;
            }

            return new SettingValue(value);
        }
        else
        {
            return null;
        }
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "WebAssembly module (*.wasm)|*.wasm|All Files (*.*)|*.*"
        };
        if (File.Exists(ScriptPath))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(ScriptPath);
            dialog.FileName = Path.GetFileName(ScriptPath);
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            scriptPath = txtScriptPath.Text = dialog.FileName;
        }
    }

    private void ComponentSettings_Load(object sender, EventArgs e)
    {
    }
}
