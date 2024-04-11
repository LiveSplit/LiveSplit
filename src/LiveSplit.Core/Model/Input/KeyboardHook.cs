using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LiveSplit.Model.Input
{
    public class KeyboardHook
    {
        protected Dictionary<Keys, bool> RegisteredKeys { get; set; }
        public event KeyEventHandler KeyPressed;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetAsyncKeyState(Keys vkey);

        public KeyboardHook()
        {
            RegisteredKeys = new Dictionary<Keys, bool>();
        }

        public void RegisterHotKey(Keys key)
        {
            if (!RegisteredKeys.ContainsKey(key))
                RegisteredKeys.Add(key, false);
        }

        public void UnregisterAllHotkeys()
        {
            RegisteredKeys.Clear();
        }

        public void Poll()
        {
            foreach (var keyPair in RegisteredKeys.ToList())
            {
                var key = keyPair.Key;
                var modifiersDown = true;
                var modifiers = Keys.None;
                if (key.HasFlag(Keys.Shift))
                {
                    modifiersDown &= IsKeyDown(Keys.ShiftKey);
                    modifiers |= Keys.Shift;
                }
                if (key.HasFlag(Keys.Control))
                {
                    modifiersDown &= IsKeyDown(Keys.ControlKey);
                    modifiers |= Keys.Control;
                }
                if (key.HasFlag(Keys.Alt))
                {
                    modifiersDown &= IsKeyDown(Keys.Menu);
                    modifiers |= Keys.Alt;
                }

                var keyWithoutModifiers = key & ~modifiers;
                var isPressed = IsKeyDown(keyWithoutModifiers);
                var wasPressedBefore = keyPair.Value;
                RegisteredKeys[key] = isPressed;

                if (modifiersDown && isPressed && !wasPressedBefore)
                    KeyPressed?.Invoke(this, new KeyEventArgs(key));
            }
        }

        protected bool IsKeyDown(Keys key)
        {
            var result = GetAsyncKeyState(key);
            return ((result >> 15) & 1) == 1;
        }
    }
}
