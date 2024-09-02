using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormsColor;

public static class MouseHook
{
    public static event EventHandler MouseAction = delegate { };

    public static void Start()
    {
        _hookID = SetHook(_proc);

    }
    public static void stop()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private static readonly LowLevelMouseProc _proc = HookCallback;
    private static nint _hookID = 0;

    private static nint SetHook(LowLevelMouseProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using ProcessModule curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_MOUSE_LL, proc,
          GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    private static nint HookCallback(
      int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
        {
            var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MouseAction(null, new EventArgs());
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private const int WH_MOUSE_LL = 14;

    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public nint dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook,
      LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint CallNextHookEx(nint hhk, int nCode,
      nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);

}
