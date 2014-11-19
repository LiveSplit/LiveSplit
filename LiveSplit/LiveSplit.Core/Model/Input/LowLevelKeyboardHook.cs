using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LiveSplit.Model.Input
{
    public class LowLevelKeyboardHook
    {
        protected KeyboardInput Input { get; set; }
        protected List<Keys> RegisteredKeys { get; set; }
        public int HookState { get; set; }
        public event KeyEventHandler KeyPressed;
        protected KeyboardHook SafetyHook { get; set; }
        protected TripleDateTime UnstableStateTime { get; set; }
        private object Lock { get; set; }

        public LowLevelKeyboardHook()
        {
            RegisteredKeys = new List<Keys>();
            Input = new KeyboardInput();
            SafetyHook = new KeyboardHook();
            HookState = 0;
            Input.KeyBoardKeyPressed += Input_KeyBoardKeyPressed;
            SafetyHook.KeyPressed += SafetyHook_KeyPressed;
            Lock = new int();
        }

        void SafetyHook_KeyPressed(object sender, KeyEventArgs e)
        {
            lock (Lock)
            {
                if (HookState >= 0)
                    KeyPressed(this, e);

                HookState++;
                //Debug.WriteLine(HookState);

                if (HookState == 1 || HookState < 0)
                    UnstableStateTime = TripleDateTime.Now;


                /*if (HookState >= 2)
                {
                    Input.RegisterHook();
                    HookState = 0;
                }*/
            }
        }

        void Input_KeyBoardKeyPressed(object sender, KeyEventArgs e)
        {
            if (RegisteredKeys.Contains(e.KeyCode | e.Modifiers) && KeyPressed != null)
            {
                lock (Lock)
                {
                    if (HookState <= 0)
                        KeyPressed(this, e);

                    HookState--;
                    //Debug.WriteLine(HookState);

                    if (HookState == -1 || HookState > 0)
                        UnstableStateTime = TripleDateTime.Now;

                    /*if (HookState < -1)
                        HookState = -1;*/
                    
                }
            }
        }

        public void RegisterHotKey(Keys key)
        {
            if (!RegisteredKeys.Contains(key))
            {
                RegisteredKeys.Add(key);
                SafetyHook.RegisterHotKey(key);
            }
        }

        public void UnregisterAllHotkeys()
        {
            RegisteredKeys.Clear();
            SafetyHook.UnregisterAllHotkeys();
        }

        public void Poll()
        {
            SafetyHook.Poll();
            lock (Lock)
            {
                if (HookState > 0 && (TripleDateTime.Now - UnstableStateTime).TotalSeconds >= 2)
                {
                    Input.RegisterHook();
                    HookState = 0;
                }
                else if (HookState < 0 && (TripleDateTime.Now - UnstableStateTime).TotalSeconds >= 2)
                    HookState = 0;
            }
        }
    }

    public class KeyboardInput : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80,
        }

        public event KeyEventHandler KeyBoardKeyPressed;

        private WindowsHookHelper.HookDelegate keyBoardDelegate;
        private IntPtr keyBoardHandle;
        private const Int32 WH_KEYBOARD_LL = 13;
        private const Int32 WH_KEYBOARD = 2;
        private bool disposed;

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string module);

        private Keys modifiers;
        private Control messageLoopControl;

        public KeyboardInput()
        {
            keyBoardDelegate = KeyboardHookDelegate;

            var semaphore = new Semaphore(0, 1);

            new Thread(() =>
            {
                messageLoopControl = new Control();
                messageLoopControl.CreateControl();

                Thread.CurrentThread.IsBackground = true;

                semaphore.Release();

                Application.Run();
            }) { Name = "Hotkey Message Loop" }.Start();

            semaphore.WaitOne();

            RegisterHook();
        }

        public void RegisterHook()
        {
            //Debug.WriteLine("Registering");
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
            {
                IntPtr hModule = GetModuleHandle(module.ModuleName);
                messageLoopControl.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (keyBoardHandle != IntPtr.Zero)
                            WindowsHookHelper.UnhookWindowsHookEx(keyBoardHandle);
                    }
                    catch { }
                    keyBoardHandle = WindowsHookHelper.SetWindowsHookEx(
                        WH_KEYBOARD_LL, keyBoardDelegate, hModule, 0);
                }));
            }
        }

        private IntPtr KeyboardHookDelegate(
            Int32 Code, IntPtr wParam, IntPtr lParam)
        {
            //TickCount = Environment.TickCount;
            //Thread.Sleep(1000);
            if (Code < 0)
            {
                return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);
            }

            var hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0104) //KeyDown
            {
                Keys key = (Keys)hookStruct.vkCode;

                if (key == Keys.LControlKey || key == Keys.RControlKey)
                {
                    key = Keys.ControlKey;
                    if (modifiers.HasFlag(Keys.Control))
                        return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);
                }
                else if (key == Keys.LMenu || key == Keys.RMenu)
                {
                    key = Keys.Menu;
                    if (modifiers.HasFlag(Keys.Alt))
                        return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);
                }
                else if (key == Keys.LShiftKey || key == Keys.RShiftKey)
                {
                    key = Keys.ShiftKey;
                    if (modifiers.HasFlag(Keys.Shift))
                        return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);
                }

                if (KeyBoardKeyPressed != null)
                {
                    KeyBoardKeyPressed(this, new KeyEventArgs(key | modifiers));
                    //System.Diagnostics.Debug.WriteLine((key | modifiers).ToString());
                }

                if (key == Keys.ControlKey)
                    modifiers |= Keys.Control;
                else if (key == Keys.Menu)
                    modifiers |= Keys.Alt;
                else if (key == Keys.ShiftKey)
                    modifiers |= Keys.Shift;
                else if (key == Keys.Delete && modifiers.HasFlag(Keys.Control) && modifiers.HasFlag(Keys.Alt))
                {
                    modifiers &= ~Keys.Control;
                    modifiers &= ~Keys.Alt;
                }
            }
            else if (wParam == (IntPtr)0x0101 || wParam == (IntPtr)0x0105) //KeyUp
            {
                Keys key = (Keys)hookStruct.vkCode;

                if (key == Keys.LControlKey || key == Keys.RControlKey)
                    modifiers &= ~Keys.Control;
                else if (key == Keys.LMenu || key == Keys.RMenu)
                    modifiers &= ~Keys.Alt;
                else if (key == Keys.LShiftKey || key == Keys.RShiftKey)
                    modifiers &= ~Keys.Shift;
            }

            return WindowsHookHelper.CallNextHookEx(
                keyBoardHandle, Code, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (keyBoardHandle != IntPtr.Zero)
                {
                    WindowsHookHelper.UnhookWindowsHookEx(
                        keyBoardHandle);
                }

                disposed = true;
            }
        }

        ~KeyboardInput()
        {
            Dispose(false);
        }
    }

    public class WindowsHookHelper
    {
        public delegate IntPtr HookDelegate(
            Int32 Code, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);


        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookEx(
            Int32 idHook, HookDelegate lpfn, IntPtr hmod,
            Int32 dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
    }
}
