using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using LiveSplit.Options;

using SharpDX.DirectInput;

namespace LiveSplit.Model.Input;

public delegate void EventHandlerT<T>(object sender, T value);

public class KeyOrButton
{
    public bool IsButton { get; protected set; }
    public bool IsKey { get => !IsButton; set => IsButton = !value; }

    public Keys Key { get; protected set; }
    public GamepadButton Button { get; protected set; }

    public KeyOrButton(Keys key)
    {
        Key = key;
        IsKey = true;
    }

    public KeyOrButton(GamepadButton button)
    {
        Button = button;
        IsButton = true;
    }

    public KeyOrButton(string stringRepresentation)
    {
        if (stringRepresentation.Contains(' ') && !stringRepresentation.Contains(", "))
        {
            string[] split = stringRepresentation.Split([' '], 2);
            Button = new GamepadButton(split[1], split[0]);
            IsButton = true;
        }
        else
        {
            Key = (Keys)Enum.Parse(typeof(Keys), stringRepresentation, true);
            IsKey = true;
        }
    }

    public override string ToString()
    {
        if (IsKey)
        {
            return Key.ToString();
        }
        else
        {
            return Button.Button.ToString() + " " + Button.GamepadName;
        }
    }

    public static bool operator ==(KeyOrButton a, KeyOrButton b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        if (a.IsKey && b.IsKey)
        {
            return a.Key == b.Key;
        }
        else if (a.IsButton && b.IsButton)
        {
            return a.Button == b.Button;
        }

        return false;
    }

    public static bool operator !=(KeyOrButton a, KeyOrButton b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is KeyOrButton other
            && this == other;
    }

    public override int GetHashCode()
    {
        return IsKey ? Key.GetHashCode() : Button.GetHashCode();
    }
}

public class CompositeHook
{
    protected LowLevelKeyboardHook KeyboardHook { get; set; }
    protected GamepadHook GamepadHook { get; set; }

    private bool allowGamepads;
    public bool AllowGamepads
    {
        get => allowGamepads;
        set
        {
            allowGamepads = value;
            if (allowGamepads)
            {
                InitializeGamepadHook();
            }
            else
            {
                CallInitializeEvent();
            }
        }
    }
    private bool gamepadHookInitialized;
    private bool initializeEventCalled;

    protected List<GamepadButton> RegisteredButtons { get; set; }

    public event KeyEventHandler KeyPressed;
    public event EventHandlerT<GamepadButton> ButtonPressed;
    public event EventHandlerT<GamepadButton> AnyGamepadButtonPressed;
    public event EventHandlerT<KeyOrButton> KeyOrButtonPressed;

    public event EventHandler GamepadHookInitialized;

    public CompositeHook() : this(false) { }

    public CompositeHook(bool allowGamepads)
    {
        KeyboardHook = new LowLevelKeyboardHook();
        RegisteredButtons = [];
        KeyboardHook.KeyPressed += KeyboardHook_KeyPressed;

        gamepadHookInitialized = false;
        initializeEventCalled = false;
        AllowGamepads = allowGamepads;
    }

    public Joystick GetMouse()
    {
        return GamepadHook.GetMouse();
    }

    private void InitializeGamepadHook()
    {
        if (gamepadHookInitialized)
        {
            return;
        }

        gamepadHookInitialized = true;

        Task.Factory.StartNew(() =>
        {
            try
            {
                GamepadHook = new GamepadHook();
                GamepadHook.ButtonPressed += GamepadHook_ButtonPressed;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            CallInitializeEvent();

            if (GamepadHook != null)
            {
                while (true)
                {
                    Thread.Sleep(25);
                    try
                    {
                        try
                        {
                            if (AllowGamepads)
                            {
                                GamepadHook.Poll();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                    catch { }
                }
            }
        });
    }

    private void CallInitializeEvent()
    {
        if (!initializeEventCalled && GamepadHookInitialized != null)
        {
            GamepadHookInitialized.Invoke(this, null);
            initializeEventCalled = true;
        }
    }

    private void KeyboardHook_KeyPressed(object sender, KeyEventArgs e)
    {
        KeyPressed?.Invoke(this, e);
        KeyOrButtonPressed?.Invoke(this, new KeyOrButton(e.KeyCode | e.Modifiers));
    }

    private void GamepadHook_ButtonPressed(object sender, GamepadButton e)
    {
        AnyGamepadButtonPressed?.Invoke(this, e);

        if (RegisteredButtons.Contains(e))
        {
            ButtonPressed?.Invoke(this, e);
            KeyOrButtonPressed?.Invoke(this, new KeyOrButton(e));
        }
    }

    public void RegisterHotKey(Keys key)
    {
        KeyboardHook.RegisterHotKey(key);
    }

    public void RegisterGamepadButton(string gamePadName, string button)
    {
        RegisterGamepadButton(new GamepadButton(gamePadName, button));
    }

    public void RegisterGamepadButton(GamepadButton button)
    {
        RegisteredButtons.Add(button);
    }

    public void RegisterHotKey(KeyOrButton keyOrButton)
    {
        if (keyOrButton.IsKey)
        {
            RegisterHotKey(keyOrButton.Key);
        }
        else
        {
            RegisterGamepadButton(keyOrButton.Button);
        }
    }

    public void Poll()
    {
        KeyboardHook.Poll();
    }

    public void UnregisterAllHotkeys()
    {
        RegisteredButtons.Clear();
        KeyboardHook.UnregisterAllHotkeys();
    }
}
