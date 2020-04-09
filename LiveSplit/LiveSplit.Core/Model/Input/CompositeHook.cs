using LiveSplit.Options;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Model.Input
{
    public delegate void EventHandlerT<T>(object sender, T value);

    public class KeyOrButton
    {
        public bool IsButton { get; protected set; }
        public bool IsKey { get { return !IsButton; } set { IsButton = !value; } }

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
                var split = stringRepresentation.Split(new char[] { ' ' }, 2);
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
                return Key.ToString();
            else
                return Button.Button.ToString() + " " + Button.GamepadName;
        }

        public static bool operator ==(KeyOrButton a, KeyOrButton b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            if ((object)a == null || (object)b == null)
                return false;

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
    }

    public class CompositeHook
    {
        protected LowLevelKeyboardHook KeyboardHook { get; set; }
        protected GamepadHook GamepadHook { get; set; }

        private bool hasPolledGamepadHook;

        protected List<GamepadButton> RegisteredButtons { get; set;}

        public event KeyEventHandler KeyPressed;
        public event EventHandlerT<GamepadButton> ButtonPressed;
        public event EventHandlerT<GamepadButton> AnyGamepadButtonPressed;
        public event EventHandlerT<KeyOrButton> KeyOrButtonPressed;

        public event EventHandler GamepadHookInitialized;

        public CompositeHook()
        {
            KeyboardHook = new LowLevelKeyboardHook();
            RegisteredButtons = new List<GamepadButton>();
            KeyboardHook.KeyPressed += KeyboardHook_KeyPressed;
            InitializeGamepadHook();
        }

        public Joystick GetMouse()
        {
            return GamepadHook.GetMouse();
        }

        void InitializeGamepadHook()
        {
            hasPolledGamepadHook = false;
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
            });
        }

        void KeyboardHook_KeyPressed(object sender, KeyEventArgs e)
        {
            KeyPressed?.Invoke(this, e);
            KeyOrButtonPressed?.Invoke(this, new KeyOrButton(e.KeyCode | e.Modifiers));
        }

        void GamepadHook_ButtonPressed(object sender, GamepadButton e)
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
                RegisterHotKey(keyOrButton.Key);
            else
                RegisterGamepadButton(keyOrButton.Button);
        }

        public void Poll()
        {
            if (GamepadHook != null)
            {
                if (!hasPolledGamepadHook)
                {
                    hasPolledGamepadHook = true;
                    GamepadHookInitialized?.Invoke(this, null);
                }
                GamepadHook.Poll();
            }
            KeyboardHook.Poll();
        }

        public void UnregisterAllHotkeys()
        {
            RegisteredButtons.Clear();
            KeyboardHook.UnregisterAllHotkeys();
        }
    }
}
