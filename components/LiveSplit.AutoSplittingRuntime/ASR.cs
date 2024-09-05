using System;
using System.Runtime.InteropServices;
using System.Text;

using TimeSpan = System.TimeSpan;

namespace LiveSplit.AutoSplittingRuntime;

public class RuntimeRef
{
    internal nint ptr;
    internal RuntimeRef(nint ptr)
    {
        this.ptr = ptr;
    }
}

public class RuntimeRefMut : RuntimeRef
{
    internal RuntimeRefMut(nint ptr) : base(ptr) { }
}

public class Runtime : RuntimeRefMut, IDisposable
{
    private void Drop()
    {
        if (ptr != 0)
        {
            ASRNative.Runtime_drop(ptr);
            ptr = 0;
        }
    }
    ~Runtime()
    {
        Drop();
    }
    public void Dispose()
    {
        Drop();
        GC.SuppressFinalize(this);
    }
    public Runtime(
        string path,
        SettingsMap settingsMap,
        StateDelegate state,
        Action start,
        Action split,
        Action skipSplit,
        Action undoSplit,
        Action reset,
        SetGameTimeDelegate setGameTime,
        Action pauseGameTime,
        Action resumeGameTime,
        LogDelegate log
    ) : base(0)
    {
        nint settingsMapPtr = settingsMap?.ptr ?? 0;
        if (settingsMap != null)
        {
            settingsMap.ptr = 0;
        }

        ptr = ASRNative.Runtime_new(
            path,
            settingsMapPtr,
            state,
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
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't load the module provided.");
        }
    }
    internal Runtime(nint ptr) : base(ptr) { }

    public bool Step()
    {
        if (ptr == 0)
        {
            return false;
        }

        return ASRNative.Runtime_step(ptr);
    }

    public TimeSpan TickRate()
    {
        if (ptr == 0)
        {
            return TimeSpan.Zero;
        }

        return new TimeSpan((long)ASRNative.Runtime_tick_rate(ptr));
    }

    public Widgets GetSettingsWidgets()
    {
        if (ptr == 0)
        {
            return null;
        }

        return new Widgets(ASRNative.Runtime_get_settings_widgets(ptr));
    }

    public void SettingsMapSetBool(string key, bool value)
    {
        if (ptr == 0)
        {
            return;
        }

        ASRNative.Runtime_settings_map_set_bool(ptr, key, value ? (byte)1 : (byte)0);
    }

    public void SettingsMapSetString(string key, string value)
    {
        if (ptr == 0)
        {
            return;
        }

        ASRNative.Runtime_settings_map_set_string(ptr, key, value);
    }

    public SettingsMap GetSettingsMap()
    {
        if (ptr == 0)
        {
            return null;
        }

        return new SettingsMap(ASRNative.Runtime_get_settings_map(ptr));
    }

    public void SetSettingsMap(SettingsMap settingsMap)
    {
        if (ptr == 0)
        {
            return;
        }

        nint settingsMapPtr = settingsMap.ptr;
        if (settingsMapPtr == 0)
        {
            return;
        }

        settingsMap.ptr = 0;
        ASRNative.Runtime_set_settings_map(ptr, settingsMapPtr);
    }

    public bool AreSettingsChanged(SettingsMapRef previousSettingsMap, WidgetsRef previousWidgets)
    {
        if (ptr == 0)
        {
            return false;
        }

        if (previousSettingsMap.ptr == 0)
        {
            return false;
        }

        if (previousWidgets.ptr == 0)
        {
            return false;
        }

        return ASRNative.Runtime_are_settings_changed(ptr, previousSettingsMap.ptr, previousWidgets.ptr) != 0;
    }
}

public class SettingsMapRef
{
    internal nint ptr;
    internal SettingsMapRef(nint ptr)
    {
        this.ptr = ptr;
    }
    public ulong GetLength()
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.SettingsMap_len(ptr);
    }
    public string GetKey(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.SettingsMap_get_key(ptr, (nuint)index);
    }
    public SettingValueRef GetValue(ulong index)
    {
        if (ptr == 0)
        {
            return null;
        }

        return new SettingValueRef(ASRNative.SettingsMap_get_value(ptr, (nuint)index));
    }
    public SettingValueRef KeyGetValue(string key)
    {
        if (ptr == 0)
        {
            return null;
        }

        nint valuePtr = ASRNative.SettingsMap_get_value_by_key(ptr, key);
        if (valuePtr != 0)
        {
            return new SettingValueRef(valuePtr);
        }
        else
        {
            return null;
        }
    }
}

public class SettingsMapRefMut : SettingsMapRef
{
    internal SettingsMapRefMut(nint ptr) : base(ptr) { }
    public void Insert(string key, SettingValue value)
    {
        if (ptr == 0)
        {
            return;
        }

        nint valuePtr = value.ptr;
        if (valuePtr == 0)
        {
            return;
        }

        value.ptr = 0;
        ASRNative.SettingsMap_insert(ptr, key, valuePtr);
    }
}

public class SettingsMap : SettingsMapRefMut, IDisposable
{
    private void Drop()
    {
        if (ptr != 0)
        {
            ASRNative.SettingsMap_drop(ptr);
            ptr = 0;
        }
    }
    ~SettingsMap()
    {
        Drop();
    }
    public void Dispose()
    {
        Drop();
        GC.SuppressFinalize(this);
    }
    public SettingsMap() : base(0)
    {
        ptr = ASRNative.SettingsMap_new();
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the settings map.");
        }
    }
    internal SettingsMap(nint ptr) : base(ptr) { }
}

public class SettingsListRef
{
    internal nint ptr;
    internal SettingsListRef(nint ptr)
    {
        this.ptr = ptr;
    }
    public ulong GetLength()
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.SettingsList_len(ptr);
    }
    public SettingValueRef Get(ulong index)
    {
        if (ptr == 0)
        {
            return null;
        }

        return new SettingValueRef(ASRNative.SettingsList_get(ptr, (nuint)index));
    }
}

public class SettingsListRefMut : SettingsListRef
{
    internal SettingsListRefMut(nint ptr) : base(ptr) { }
    public void Push(SettingValue value)
    {
        if (ptr == 0)
        {
            return;
        }

        nint valuePtr = value.ptr;
        if (valuePtr == 0)
        {
            return;
        }

        value.ptr = 0;
        ASRNative.SettingsList_push(ptr, valuePtr);
    }
}

public class SettingsList : SettingsListRefMut, IDisposable
{
    private void Drop()
    {
        if (ptr != 0)
        {
            ASRNative.SettingsList_drop(ptr);
            ptr = 0;
        }
    }
    ~SettingsList()
    {
        Drop();
    }
    public void Dispose()
    {
        Drop();
        GC.SuppressFinalize(this);
    }
    public SettingsList() : base(0)
    {
        ptr = ASRNative.SettingsList_new();
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the settings list.");
        }
    }
    internal SettingsList(nint ptr) : base(ptr) { }
}

public class SettingValueRef
{
    internal nint ptr;
    internal SettingValueRef(nint ptr)
    {
        this.ptr = ptr;
    }
    public string GetKind()
    {
        if (ptr == 0)
        {
            return "";
        }

        nuint ty = ASRNative.SettingValue_get_type(ptr);
        return (ulong)ty switch
        {
            1 => "map",
            2 => "list",
            3 => "bool",
            4 => "i64",
            5 => "f64",
            6 => "string",
            _ => "",
        };
    }
    public SettingsMapRef GetMap()
    {
        if (ptr == 0)
        {
            return null;
        }

        return new SettingsMapRef(ASRNative.SettingValue_get_map(ptr));
    }
    public SettingsListRef GetList()
    {
        if (ptr == 0)
        {
            return null;
        }

        return new SettingsListRef(ASRNative.SettingValue_get_list(ptr));
    }
    public bool GetBool()
    {
        if (ptr == 0)
        {
            return false;
        }

        return ASRNative.SettingValue_get_bool(ptr) != 0;
    }
    public long GetI64()
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.SettingValue_get_i64(ptr);
    }
    public double GetF64()
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.SettingValue_get_f64(ptr);
    }
    public string GetString()
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.SettingValue_get_string(ptr);
    }
}

public class SettingValueRefMut : SettingValueRef
{
    internal SettingValueRefMut(nint ptr) : base(ptr) { }
}

public class SettingValue : SettingValueRefMut, IDisposable
{
    private void Drop()
    {
        if (ptr != 0)
        {
            ASRNative.SettingValue_drop(ptr);
            ptr = 0;
        }
    }
    ~SettingValue()
    {
        Drop();
    }
    public void Dispose()
    {
        Drop();
        GC.SuppressFinalize(this);
    }
    public SettingValue(bool value) : base(0)
    {
        ptr = ASRNative.SettingValue_new_bool(value ? (byte)1 : (byte)0);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    public SettingValue(long value) : base(0)
    {
        ptr = ASRNative.SettingValue_new_i64(value);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    public SettingValue(double value) : base(0)
    {
        ptr = ASRNative.SettingValue_new_f64(value);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    public SettingValue(string value) : base(0)
    {
        ptr = ASRNative.SettingValue_new_string(value);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    public SettingValue(SettingsMap value) : base(0)
    {
        nint valuePtr = value.ptr;
        if (valuePtr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }

        value.ptr = 0;
        ptr = ASRNative.SettingValue_new_map(valuePtr);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    public SettingValue(SettingsList value) : base(0)
    {
        nint valuePtr = value.ptr;
        if (valuePtr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }

        value.ptr = 0;
        ptr = ASRNative.SettingValue_new_list(valuePtr);
        if (ptr == 0)
        {
            throw new ArgumentException("Couldn't create the setting value.");
        }
    }
    internal SettingValue(nint ptr) : base(ptr) { }
}

public class WidgetsRef
{
    internal nint ptr;
    internal WidgetsRef(nint ptr)
    {
        this.ptr = ptr;
    }

    public ulong GetLength()
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.Widgets_len(ptr);
    }

    public string GetKey(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_key(ptr, (nuint)index);
    }

    public string GetDescription(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_description(ptr, (nuint)index);
    }

    public string GetTooltip(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_tooltip(ptr, (nuint)index);
    }

    public uint GetHeadingLevel(ulong index)
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.Widgets_get_heading_level(ptr, (nuint)index);
    }

    public string GetType(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        nuint ty = ASRNative.Widgets_get_type(ptr, (nuint)index);
        return (ulong)ty switch
        {
            1 => "bool",
            2 => "title",
            3 => "choice",
            4 => "file-select",
            _ => "",
        };
    }

    public bool GetBool(ulong index, SettingsMapRef settingsMap)
    {
        if (ptr == 0)
        {
            return false;
        }

        if (settingsMap.ptr == 0)
        {
            return false;
        }

        return ASRNative.Widgets_get_bool(ptr, (nuint)index, settingsMap.ptr) != 0;
    }

    public ulong GetChoiceCurrentIndex(ulong index, SettingsMapRef settingsMap)
    {
        if (ptr == 0)
        {
            return 0;
        }

        if (settingsMap.ptr == 0)
        {
            return 0;
        }

        return ASRNative.Widgets_get_choice_current_index(ptr, (nuint)index, settingsMap.ptr);
    }

    public ulong GetChoiceOptionsLength(ulong index)
    {
        if (ptr == 0)
        {
            return 0;
        }

        return ASRNative.Widgets_get_choice_options_len(ptr, (nuint)index);
    }

    public string GetChoiceOptionKey(ulong index, ulong optionIndex)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_choice_option_key(ptr, (nuint)index, (nuint)optionIndex);
    }

    public string GetChoiceOptionDescription(ulong index, ulong optionIndex)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_choice_option_description(ptr, (nuint)index, (nuint)optionIndex);
    }

    public string GetFileSelectFilter(ulong index)
    {
        if (ptr == 0)
        {
            return "";
        }

        return ASRNative.Widgets_get_file_select_filter(ptr, (nuint)index);
    }
}

public class WidgetsRefMut : WidgetsRef
{
    internal WidgetsRefMut(nint ptr) : base(ptr) { }
}

public class Widgets : WidgetsRefMut, IDisposable
{
    private void Drop()
    {
        if (ptr != 0)
        {
            ASRNative.Widgets_drop(ptr);
            ptr = 0;
        }
    }
    ~Widgets()
    {
        Drop();
    }
    public void Dispose()
    {
        Drop();
        GC.SuppressFinalize(this);
    }
    internal Widgets(nint ptr) : base(ptr) { }
}

public delegate int StateDelegate();
public delegate void SetGameTimeDelegate(long gameTime);
public delegate void LogDelegate(nint messagePtr, nuint messageLen);

public static class ASRNative
{
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint Runtime_new(
        ASRString path,
        nint settings_map,
        StateDelegate state,
        Action start,
        Action split,
        Action skipSplit,
        Action undoSplit,
        Action reset,
        SetGameTimeDelegate set_game_time,
        Action pause_game_time,
        Action resume_game_time,
        LogDelegate log
    );
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Runtime_drop(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool Runtime_step(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong Runtime_tick_rate(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint Runtime_get_settings_widgets(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Runtime_settings_map_set_bool(nint self, ASRString key, byte value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Runtime_settings_map_set_string(nint self, ASRString key, ASRString value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint Runtime_get_settings_map(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Runtime_set_settings_map(nint self, nint settings_map);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte Runtime_are_settings_changed(nint self, nint previous_settings_map, nint previous_widgets);

    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingsMap_new();
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SettingsMap_drop(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SettingsMap_insert(nint self, ASRString key, nint value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint SettingsMap_len(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString SettingsMap_get_key(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingsMap_get_value(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingsMap_get_value_by_key(nint self, ASRString key);

    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingsList_new();
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SettingsList_drop(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SettingsList_push(nint self, nint value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint SettingsList_len(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingsList_get(nint self, nuint index);

    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_map(nint value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_list(nint value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_bool(byte value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_i64(long value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_f64(double value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_new_string(ASRString value);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SettingValue_drop(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint SettingValue_get_type(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_get_map(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint SettingValue_get_list(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SettingValue_get_bool(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern long SettingValue_get_i64(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern double SettingValue_get_f64(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString SettingValue_get_string(nint self);

    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Widgets_drop(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint Widgets_len(nint self);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_key(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_description(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_tooltip(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint Widgets_get_heading_level(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint Widgets_get_type(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte Widgets_get_bool(nint self, nuint index, nint settings_map);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint Widgets_get_choice_current_index(nint self, nuint index, nint settings_map);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint Widgets_get_choice_options_len(nint self, nuint index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_choice_option_key(nint self, nuint index, nuint option_index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_choice_option_description(nint self, nuint index, nuint option_index);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString Widgets_get_file_select_filter(nint self, nuint index);

    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint get_buf_len();
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString path_to_wasi(ASRString original_path);
    [DllImport("asr_capi", CallingConvention = CallingConvention.Cdecl)]
    public static extern ASRString wasi_to_path(ASRString wasi_path);
}

public class ASRString : SafeHandle
{
    private bool needToFree;

    public ASRString() : base((nint)0, false) { }

    public override bool IsInvalid => false;

    public static implicit operator ASRString(string managedString)
    {
        var asrString = new ASRString();

        int len = Encoding.UTF8.GetByteCount(managedString);
        byte[] buffer = new byte[len + 1];
        Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, buffer, 0);
        nint nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
        Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);

        asrString.SetHandle(nativeUtf8);
        asrString.needToFree = true;
        return asrString;
    }

    public static string FromPtrLen(nint ptr, nuint len)
    {
        if (ptr == 0 || (ulong)len > int.MaxValue)
        {
            return null;
        }

        unsafe
        {
            return Encoding.UTF8.GetString((byte*)ptr, (int)len);
        }
    }

    public static implicit operator string(ASRString asrString)
    {
        return FromPtrLen(asrString.handle, ASRNative.get_buf_len());
    }

    protected override bool ReleaseHandle()
    {
        if (needToFree)
        {
            Marshal.FreeHGlobal(handle);
        }

        return true;
    }
}
