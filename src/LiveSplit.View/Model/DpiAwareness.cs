using LiveSplit.UI;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace LiveSplit.View;

public static class DpiAwareness
{
    private const int PROCESS_DPI_UNAWARE = 0;
    private const int PROCESS_SYSTEM_DPI_AWARE = 1;
    private const int PROCESS_PER_MONITOR_DPI_AWARE = 2;
    private const int MDT_EFFECTIVE_DPI = 0;
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    private const int LOGPIXELSX = 88;
    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new(-4);

    private static bool initialized;
    private static bool processDpiAware;

    public static bool IsProcessDpiAware => processDpiAware;

    public static bool ReadEnabledSetting(string settingsPath)
    {
        bool defaultValue = SupportsDpiAwareness;
        if (!File.Exists(settingsPath))
        {
            return defaultValue;
        }

        try
        {
            var document = new XmlDocument();
            document.Load(settingsPath);
            XmlElement setting = document["Settings"]?["EnableDPIAwareness"];
            if (setting == null)
            {
                return defaultValue;
            }

            return bool.TryParse(setting.InnerText, out bool enabled) ? enabled : defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static bool Initialize(bool enabled)
    {
        if (initialized)
        {
            return processDpiAware;
        }

        initialized = true;
        if (!enabled || !SupportsDpiAwareness)
        {
            return false;
        }

        if (TryEnablePerMonitorV2())
        {
            processDpiAware = true;
            return true;
        }

        processDpiAware = IsProcessDpiAwareByQuery();
        return processDpiAware;
    }

    public static int GetDpiForWindow(IntPtr windowHandle)
    {
        if (!processDpiAware)
        {
            return DpiScale.DefaultDpi;
        }

        try
        {
            uint dpi = GetDpiForWindowNative(windowHandle);
            if (dpi > 0)
            {
                return (int)dpi;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        try
        {
            IntPtr monitor = MonitorFromWindow(windowHandle, MONITOR_DEFAULTTONEAREST);
            if (TryGetDpiForMonitor(monitor, out uint dpi))
            {
                return (int)dpi;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        return GetSystemDpi();
    }

    public static int GetDpiForPoint(Point screenPoint)
    {
        if (!processDpiAware)
        {
            return DpiScale.DefaultDpi;
        }

        try
        {
            var nativePoint = new NativePoint(screenPoint.X, screenPoint.Y);
            IntPtr monitor = MonitorFromPoint(nativePoint, MONITOR_DEFAULTTONEAREST);
            if (TryGetDpiForMonitor(monitor, out uint dpi))
            {
                return (int)dpi;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        return GetSystemDpi();
    }

    private static bool SupportsDpiAwareness
        => Environment.OSVersion.Version.Major >= 6;

    private static bool TryEnablePerMonitorV2()
    {
        try
        {
            if (SetProcessDpiAwarenessContextNative(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
            {
                return true;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        try
        {
            if (SetProcessDpiAwarenessNative(PROCESS_PER_MONITOR_DPI_AWARE) == 0)
            {
                return true;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        try
        {
            return SetProcessDPIAwareNative();
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        return false;
    }

    private static bool IsProcessDpiAwareByQuery()
    {
        try
        {
            if (GetProcessDpiAwarenessNative(IntPtr.Zero, out int awareness) == 0)
            {
                return awareness is PROCESS_SYSTEM_DPI_AWARE or PROCESS_PER_MONITOR_DPI_AWARE;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        return false;
    }

    private static bool TryGetDpiForMonitor(IntPtr monitor, out uint dpi)
    {
        dpi = 0;
        if (monitor == IntPtr.Zero)
        {
            return false;
        }

        return GetDpiForMonitorNative(monitor, MDT_EFFECTIVE_DPI, out dpi, out _) == 0 && dpi > 0;
    }

    private static int GetSystemDpi()
    {
        IntPtr deviceContext = GetDC(IntPtr.Zero);
        if (deviceContext == IntPtr.Zero)
        {
            return DpiScale.DefaultDpi;
        }

        try
        {
            int dpi = GetDeviceCaps(deviceContext, LOGPIXELSX);
            return dpi > 0 ? dpi : DpiScale.DefaultDpi;
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, deviceContext);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct NativePoint
    {
        public readonly int X;
        public readonly int Y;

        public NativePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [DllImport("user32.dll", EntryPoint = "SetProcessDpiAwarenessContext", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContextNative(IntPtr dpiAwarenessContext);

    [DllImport("shcore.dll", EntryPoint = "SetProcessDpiAwareness")]
    private static extern int SetProcessDpiAwarenessNative(int dpiAwareness);

    [DllImport("user32.dll", EntryPoint = "SetProcessDPIAware")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDPIAwareNative();

    [DllImport("shcore.dll", EntryPoint = "GetProcessDpiAwareness")]
    private static extern int GetProcessDpiAwarenessNative(IntPtr processHandle, out int dpiAwareness);

    [DllImport("shcore.dll", EntryPoint = "GetDpiForMonitor")]
    private static extern int GetDpiForMonitorNative(IntPtr monitorHandle, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport("user32.dll", EntryPoint = "GetDpiForWindow")]
    private static extern uint GetDpiForWindowNative(IntPtr windowHandle);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr windowHandle, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(NativePoint point, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr windowHandle);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr windowHandle, IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr deviceContext, int index);
}
