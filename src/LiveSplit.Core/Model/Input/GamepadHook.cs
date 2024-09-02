using System;
using System.Collections.Generic;
using System.Linq;

using LiveSplit.Options;

using SharpDX.DirectInput;

namespace LiveSplit.Model.Input;

public struct GamepadButton
{
    public string GamepadName;
    public string Button;

    public GamepadButton(string gamepadName, string button)
    {
        GamepadName = gamepadName;
        Button = button;
    }

    public static bool operator ==(GamepadButton a, GamepadButton b)
    {
        return a.GamepadName == b.GamepadName && a.Button == b.Button;
    }

    public static bool operator !=(GamepadButton a, GamepadButton b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is GamepadButton button)
        {
            return this == button;
        }

        return base.Equals(obj);
    }

    public override readonly int GetHashCode()
    {
        return GamepadName.GetHashCode() ^ Button.GetHashCode();
    }
}

public class GamepadHook
{
    protected List<Joystick> Joysticks { get; set; }
    public event EventHandlerT<GamepadButton> ButtonPressed;
    protected int LastScrollWheelValue;

    public GamepadHook()
    {
        var input = new DirectInput();
        IList<DeviceInstance> devices = input.GetDevices();
        Joysticks = devices
            .Where(x => x.Type != DeviceType.Keyboard)
            .Select(x => new Joystick(input, x.InstanceGuid)).ToList();

        for (int ind = 0; ind < Joysticks.Count; ind++)
        {
            Joystick joystick = Joysticks[ind];
            try
            {
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Joysticks.RemoveAt(ind);
                ind--;
            }
        }
    }

    public Joystick GetMouse()
    {
        return Joysticks.FirstOrDefault(x => x.Information.Type == DeviceType.Mouse);
    }

    protected bool IsPressed(Joystick joystick, JoystickOffset button, int value)
    {
        int shortMaskMax = 0xFF00;
        int shortMaskMin = 0xFF;

        if (joystick.Information.Type == DeviceType.Mouse)
        {
            if (button is JoystickOffset.X
                or JoystickOffset.Y
                or JoystickOffset.Buttons0
                or JoystickOffset.Buttons1)
            {
                return false;
            }

            if (button == JoystickOffset.Z)
            {
                return true;
            }
        }

        if (button is JoystickOffset.Z
            or JoystickOffset.X
            or JoystickOffset.Y
            or JoystickOffset.RotationX
            or JoystickOffset.RotationY
            or JoystickOffset.RotationZ
            or JoystickOffset.Sliders0
            or JoystickOffset.Sliders1)
        {
            return value >= shortMaskMax || value <= shortMaskMin;
        }

        if (button is JoystickOffset.PointOfViewControllers0
            or JoystickOffset.PointOfViewControllers1
            or JoystickOffset.PointOfViewControllers2
            or JoystickOffset.PointOfViewControllers3)
        {
            return value != -1;
        }

        return value != 0;
    }

    protected string ToString(Joystick joystick, JoystickOffset button, int value)
    {
        int shortMaskMax = 0xFF00;

        string originalName = button.ToString();

        if (joystick.Information.Type == DeviceType.Mouse)
        {
            if (button == JoystickOffset.Z)
            {
                string result = value < LastScrollWheelValue ? "Scroll_Down" : "Scroll_Up";
                LastScrollWheelValue = value;
                return result;
            }

            if (button == JoystickOffset.Buttons2)
            {
                return "Middle";
            }
        }

        if (button is JoystickOffset.Z
           or JoystickOffset.X
           or JoystickOffset.Y
           or JoystickOffset.RotationX
           or JoystickOffset.RotationY
           or JoystickOffset.RotationZ
           or JoystickOffset.Sliders0
           or JoystickOffset.Sliders1)
        {
            return originalName + (value >= shortMaskMax ? '+' : '-');
        }

        if (button is JoystickOffset.PointOfViewControllers0
            or JoystickOffset.PointOfViewControllers1
            or JoystickOffset.PointOfViewControllers2
            or JoystickOffset.PointOfViewControllers3)
        {
            if (button == JoystickOffset.PointOfViewControllers0)
            {
                originalName = "POV_0_";
            }
            else if (button == JoystickOffset.PointOfViewControllers1)
            {
                originalName = "POV_1_";
            }
            else if (button == JoystickOffset.PointOfViewControllers2)
            {
                originalName = "POV_2_";
            }
            else
            {
                originalName = "POV_3_";
            }

            if (value < 2250)
            {
                return originalName + "Up";
            }
            else if (value < 6750)
            {
                return originalName + "UpRight";
            }
            else if (value < 11250)
            {
                return originalName + "Right";
            }
            else if (value < 15750)
            {
                return originalName + "DownRight";
            }
            else if (value < 20250)
            {
                return originalName + "Down";
            }
            else if (value < 24750)
            {
                return originalName + "DownLeft";
            }
            else if (value < 29250)
            {
                return originalName + "Left";
            }
            else if (value < 33750)
            {
                return originalName + "UpLeft";
            }
            else
            {
                return originalName + "Up";
            }
        }

        return originalName;
    }

    public void Poll()
    {
        try
        {
            Joystick brokenJoystick = null;

            int i = 0;

            foreach (Joystick joystick in Joysticks)
            {
                try
                {
                    joystick.Poll();
                    JoystickUpdate[] states = joystick.GetBufferedData();
                    foreach (JoystickUpdate state in states)
                    {
                        if (IsPressed(joystick, state.Offset, state.Value) && ButtonPressed != null)
                        {
                            string buttonName = ToString(joystick, state.Offset, state.Value);
                            ButtonPressed(this, new GamepadButton(joystick.Information.InstanceName + " " + joystick.Information.InstanceGuid, buttonName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    brokenJoystick = joystick;
                    break;
                }

                ++i;
            }

            if (brokenJoystick != null)
            {
                Joysticks.RemoveAt(i);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
