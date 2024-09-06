using System;
using System.Collections.Generic;

using LiveSplit.Options;

namespace LiveSplit.AutoSplittingRuntime;

public class ASRSetting
{
    public string Id { get; }
    public string Label { get; }
    public bool Value { get; set; }
    public bool DefaultValue { get; }
    public string Parent { get; }
    public string ToolTip { get; set; }

    public ASRSetting(string id, bool default_value, string label, string parent)
    {
        Id = id;
        Value = default_value;
        DefaultValue = default_value;
        Label = label;
        Parent = parent;
    }

    public override string ToString()
    {
        return Label;
    }
}

public class ASRSettings
{
    // Dict for easy access per key
    public Dictionary<string, ASRSetting> Settings { get; set; }
    // List for preserved insertion order (Dict provides that as well, but not guaranteed)
    public List<ASRSetting> OrderedSettings { get; }

    public Dictionary<string, ASRSetting> BasicSettings { get; }

    public ASRSettingsBuilder Builder;
    public ASRSettingsReader Reader;

    public ASRSettings()
    {
        Settings = [];
        OrderedSettings = [];
        BasicSettings = [];
        Builder = new ASRSettingsBuilder(this);
        Reader = new ASRSettingsReader(this);
    }

    public void AddSetting(string name, bool default_value, string description, string parent)
    {
        description ??= name;

        if (parent != null && !Settings.ContainsKey(parent))
        {
            throw new ArgumentException($"Parent for setting '{name}' is not a setting: {parent}");
        }

        if (Settings.ContainsKey(name))
        {
            throw new ArgumentException($"Setting '{name}' was already added");
        }

        var setting = new ASRSetting(name, default_value, description, parent);
        Settings.Add(name, setting);
        OrderedSettings.Add(setting);
    }

    public bool GetSettingValue(string name)
    {
        // Don't cause error if setting doesn't exist, but still inform script
        // author since that usually shouldn't happen.
        if (Settings.ContainsKey(name))
        {
            return GetSettingValueRecursive(Settings[name]);
        }

        Log.Info("[ASR] Custom Setting Key doesn't exist: " + name);

        return false;
    }

    public void AddBasicSetting(string name)
    {
        BasicSettings.Add(name, new ASRSetting(name, true, "", null));
    }

    public bool GetBasicSettingValue(string name)
    {
        if (BasicSettings.ContainsKey(name))
        {
            return BasicSettings[name].Value;
        }

        return false;
    }

    public bool IsBasicSettingPresent(string name)
    {
        return BasicSettings.ContainsKey(name);
    }

    /// <summary>
    /// Returns true only if this setting and all it's parent settings are true.
    /// </summary>
    private bool GetSettingValueRecursive(ASRSetting setting)
    {
        if (!setting.Value)
        {
            return false;
        }

        if (setting.Parent == null)
        {
            return setting.Value;
        }

        return GetSettingValueRecursive(Settings[setting.Parent]);
    }
}

/// <summary>
/// Interface for adding settings via the ASR Script.
/// </summary>
public class ASRSettingsBuilder
{
    public string CurrentDefaultParent { get; set; }
    private readonly ASRSettings _s;

    public ASRSettingsBuilder(ASRSettings s)
    {
        _s = s;
    }

    public void Add(string id, bool default_value = true, string description = null, string parent = null)
    {
        parent ??= CurrentDefaultParent;

        _s.AddSetting(id, default_value, description, parent);
    }

    public void SetToolTip(string id, string text)
    {
        if (!_s.Settings.ContainsKey(id))
        {
            throw new ArgumentException($"Can't set tooltip, '{id}' is not a setting");
        }

        _s.Settings[id].ToolTip = text;
    }
}

/// <summary>
/// Interface for reading settings via the ASR Script.
/// </summary>
public class ASRSettingsReader
{
    private readonly ASRSettings _s;

    public ASRSettingsReader(ASRSettings s)
    {
        _s = s;
    }

    public dynamic this[string id] => _s.GetSettingValue(id);

    public bool ContainsKey(string key)
    {
        return _s.Settings.ContainsKey(key);
    }

    public bool StartEnabled => _s.GetBasicSettingValue("start");

    public bool ResetEnabled => _s.GetBasicSettingValue("reset");

    public bool SplitEnabled => _s.GetBasicSettingValue("split");
}
