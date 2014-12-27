using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace LiveSplit.Model.Input
{
    public sealed class HotkeyHook : IDisposable
    {
        public static HotkeyHook Instance { get; set; }
        
        static HotkeyHook()
        {
            Instance = new HotkeyHook();
        }

        private HotkeyHook() 
        {
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                {
                    KeyPressed(this, args);
                }
            };
        }

        public class KeyPressedEventArgs : EventArgs
        {
            public ModifierKeys Modifier { get; private set; }
            public Keys Key { get; private set; }

            internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
            {
                Modifier = modifier;
                Key = key;
            }
        }

        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 786;
            public event EventHandler<KeyPressedEventArgs> KeyPressed;
            public Window()
            {
                CreateHandle(new CreateParams());
            }
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)((int)m.LParam >> 16 & 65535);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 65535);
                    if (KeyPressed != null)
                    {
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                    }
                }
            }
            public void Dispose()
            {
                DestroyHandle();
            }
        }
        private Window _window = new Window();
        private int _currentId;
        public event EventHandler<KeyPressedEventArgs> KeyPressed;
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _currentId++;
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
            {
                throw new InvalidOperationException("Couldn’t register the hot key.");
            }
        }

        public void UnregisterAllHotkeys()
        {
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }
            _currentId = 0;
        }

        public void Dispose()
        {
            UnregisterAllHotkeys();
            _window.Dispose();
        }
    }
}
