using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LiveSplit.Model.Input;

public class KeyboardHook
{
    protected Dictionary<Keys, bool> RegisteredKeys { get; set; }
    public event KeyEventHandler KeyPressed;

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern short GetAsyncKeyState(Keys vkey);

    public KeyboardHook()
    {
        RegisteredKeys = [];
    }

    public void RegisterHotKey(Keys key)
    {
        if (!RegisteredKeys.ContainsKey(key))
        {
            RegisteredKeys.Add(key, false);
        }
    }

    public void UnregisterAllHotkeys()
    {
        RegisteredKeys.Clear();
    }

    public void Poll()
    {
        foreach (KeyValuePair<Keys, bool> keyPair in RegisteredKeys.ToList())
        {
            Keys key = keyPair.Key;
            bool modifiersDown = true;
            Keys modifiers = Keys.None;
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

            Keys keyWithoutModifiers = key & ~modifiers;
            bool isPressed = IsKeyDown(keyWithoutModifiers);
            bool wasPressedBefore = keyPair.Value;
            RegisteredKeys[key] = isPressed;

            if (modifiersDown && isPressed && !wasPressedBefore)
            {
                KeyPressed?.Invoke(this, new KeyEventArgs(key));
            }
        }
    }

    protected bool IsKeyDown(Keys key)
    {
        short result = GetAsyncKeyState(key);
        return ((result >> 15) & 1) == 1;
    }
}
