#if DEBUG

/**
 * XGamePad
 * An event-driven C# wrapper for XInput devices.
 * Author: Jean-Philippe Steinmetz <caskater47@gmail.com>
 * Homepage: http://www.caskater4.com/engineering/xgamepad
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program.  If not,
 * see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using Microsoft.DirectX.XInput;

namespace XGamePad
{
    public enum GamepadState : int
    {
        Connected,
        Disconnected
    }

    public enum GamepadButtons : ushort
    {
        DPadUp = 0x1,
        DPadDown = 0x2,
        DPadLeft = 0x4,
        DPadRight = 0x8,
        Start = 0x10,
        Back = 0x20,
        LeftThumb = 0x40,
        RightThumb = 0x80,
        LeftShoulder = 0x100,
        RightShoulder = 0x200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000
    }

    public enum GamepadThumbs : int
    {
        Left,
        Right
    }

    public enum GamepadTriggers : int
    {
        Left,
        Right
    }

    public class GamepadButtonEventArgs : EventArgs
    {
        public readonly int DeviceID;
        public readonly GamepadButtons Button;
        public readonly bool IsPressed;

        public GamepadButtonEventArgs(int DeviceID, GamepadButtons Button, bool IsPressed)
        {
            this.DeviceID = DeviceID;
            this.Button = Button;
            this.IsPressed = IsPressed;
        }
    }

    public class GamepadDPadEventArgs : EventArgs
    {
        public readonly int DeviceID;
        public readonly GamepadButtons Buttons;
        public GamepadDPadEventArgs(int DeviceID, GamepadButtons Buttons)
        {
            this.DeviceID = DeviceID;
            this.Buttons = Buttons;
        }
    }

    public class GamepadThumbEventArgs : EventArgs
    {
        public readonly int DeviceID;
        public readonly GamepadThumbs Thumb;
        public readonly short XPosition;
        public readonly short YPosition;
        public readonly bool IsPressed;

        public GamepadThumbEventArgs(int DeviceID, GamepadThumbs Thumb, short XPosition, short YPosition, bool IsPressed)
        {
            this.DeviceID = DeviceID;
            this.Thumb = Thumb;
            this.XPosition = XPosition;
            this.YPosition = YPosition;
            this.IsPressed = IsPressed;
        }
    }

    public class GamepadTriggerEventArgs : EventArgs
    {
        public readonly int DeviceID;
        public readonly GamepadTriggers Trigger;
        public readonly byte Value;

        public GamepadTriggerEventArgs(int DeviceID, GamepadTriggers Trigger, byte Value)
        {
            this.DeviceID = DeviceID;
            this.Trigger = Trigger;
            this.Value = Value;
        }
    }

    public class GamepadEventArgs : EventArgs
    {
        public readonly int DeviceID;
        public readonly GamepadState State;

        public GamepadEventArgs(int DeviceID, GamepadState State)
        {
            this.DeviceID = DeviceID;
            this.State = State;
        }
    }

    public class Gamepad
    {
        public readonly int deviceID;
        private bool isConnected = false;
        /**
         * Gamepad last state variables
         **/
        private bool lastAButtonState = false;
        private bool lastBButtonState = false;
        private bool lastXButtonState = false;
        private bool lastYButtonState = false;
        private bool lastStartButtonState = false;
        private bool lastBackButtonState = false;
        private bool lastLeftThumbButtonState = false;
        private bool lastRightThumbButtonState = false;
        private bool lastLeftShoulderButtonState = false;
        private bool lastRightShoulderButtonState = false;
        private GamepadButtons lastDPadState = 0;
        private byte lastLeftTrigger = 0;
        private byte lastRightTrigger = 0;
        private bool leftTriggerPressed = false;
        private bool rightTriggerPressed = false;

        /**
         * Event handlers
         **/
        public delegate void ButtonHandler(object sender, GamepadButtonEventArgs evt);
        public delegate void DPadHandler(object sender, GamepadDPadEventArgs evt);
        public delegate void ThumbHandler(object sender, GamepadThumbEventArgs evt);
        public delegate void TriggerHandler(object sender, GamepadTriggerEventArgs evt);
        public delegate void ConnectionHandler(object sender, GamepadEventArgs evt);

        /**
         * Events for gamepad
         **/
        public event ButtonHandler OnAButtonPress;
        public event ButtonHandler OnAButtonRelease;
        public event ButtonHandler OnBButtonPress;
        public event ButtonHandler OnBButtonRelease;
        public event ButtonHandler OnXButtonPress;
        public event ButtonHandler OnXButtonRelease;
        public event ButtonHandler OnYButtonPress;
        public event ButtonHandler OnYButtonRelease;
        public event ButtonHandler OnStartButtonPress;
        public event ButtonHandler OnStartButtonRelease;
        public event ButtonHandler OnBackButtonPress;
        public event ButtonHandler OnBackButtonRelease;
        public event ButtonHandler OnLeftThumbButtonPress;
        public event ButtonHandler OnLeftThumbButtonRelease;
        public event ButtonHandler OnRightThumbButtonPress;
        public event ButtonHandler OnRightThumbButtonRelease;
        public event ButtonHandler OnLeftShoulderButtonPress;
        public event ButtonHandler OnLeftShoulderButtonRelease;
        public event ButtonHandler OnRightShoulderButtonPress;
        public event ButtonHandler OnRightShoulderButtonRelease;

        public event DPadHandler OnDPadChange;
        public event DPadHandler OnDPadPress;
        public event DPadHandler OnDPadRelease;

        public event ThumbHandler OnLeftThumbUpdate;
        public event ThumbHandler OnRightThumbUpdate;

        public event TriggerHandler OnLeftTriggerChange;
        public event TriggerHandler OnLeftTriggerPress;
        public event TriggerHandler OnLeftTriggerRelease;
        public event TriggerHandler OnRightTriggerChange;
        public event TriggerHandler OnRightTriggerPress;
        public event TriggerHandler OnRightTriggerRelease;

        public event ConnectionHandler OnConnect;
        public event ConnectionHandler OnDisconnect;
        /**
         * Creates a new instance of the Gamepad object, to grab user control
         * Parameter deviceIndex must be a value between 0 and 3.
         **/
        public Gamepad(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex > 3) return;
            
            deviceID = deviceIndex;
        }

        /**
         * This updates the state of the device and triggers events
         * to any registered listeners.
         **/
        public void Update()
        {
            try
            {
                XInputState gpState;
                XInputMethods.GetState(deviceID, out gpState);
                XInputGamepad gp = gpState.Gamepad;

                // If previously disconnected, notify of connect
                if (!isConnected && OnConnect != null)
                {
                    isConnected = true;
                    OnConnect(this, new GamepadEventArgs(deviceID, GamepadState.Connected));
                }

                if (isConnected)
                {
                    // A Button
                    // Check to see if anything changed
                    if (!(lastAButtonState == gp.IsAButtonDown))
                    {
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.A, gp.IsAButtonDown);

                        if (gp.IsAButtonDown) // If button changed to press state
                        {
                            if (OnAButtonPress != null) OnAButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnAButtonRelease != null) OnAButtonRelease(this, btnEvt);
                        }

                        lastAButtonState = gp.IsAButtonDown;
                    }

                    // B Button
                    // Check to see if anything changed
                    if (!(lastBButtonState == gp.IsBButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.B, gp.IsBButtonDown);

                        if (gp.IsBButtonDown) // If button changed to press state
                        {
                            if (OnBButtonPress != null) OnBButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnBButtonRelease != null) OnBButtonRelease(this, btnEvt);
                        }

                        lastBButtonState = gp.IsBButtonDown;
                    }

                    // X Button
                    // Check to see if anything changed
                    if (!(lastXButtonState == gp.IsXButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.X, gp.IsXButtonDown);

                        if (gp.IsXButtonDown) // If button changed to press state
                        {
                            if (OnXButtonPress != null) OnXButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnXButtonRelease != null) OnXButtonRelease(this, btnEvt);
                        }

                        lastXButtonState = gp.IsXButtonDown;
                    }

                    // Y Button
                    // Check to see if anything changed
                    if (!(lastYButtonState == gp.IsYButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.Y, gp.IsYButtonDown);

                        if (gp.IsYButtonDown) // If button changed to press state
                        {
                            if (OnYButtonPress != null) OnYButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnYButtonRelease != null) OnYButtonRelease(this, btnEvt);
                        }

                        lastYButtonState = gp.IsYButtonDown;
                    }

                    // Back Button
                    // Check to see if anything changed
                    if (!(lastBackButtonState == gp.IsBackButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.Back, gp.IsBackButtonDown);

                        if (gp.IsBackButtonDown) // If button changed to press state
                        {
                            if (OnBackButtonPress != null) OnBackButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnBackButtonRelease != null) OnBackButtonRelease(this, btnEvt);
                        }

                        lastBackButtonState = gp.IsBackButtonDown;
                    }

                    // Start Button
                    // Check to see if anything changed
                    if (!(lastStartButtonState == gp.IsStartButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.Start, gp.IsStartButtonDown);

                        if (gp.IsStartButtonDown) // If button changed to press state
                        {
                            if (OnStartButtonPress != null) OnStartButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnStartButtonRelease != null) OnStartButtonRelease(this, btnEvt);
                        }

                        lastStartButtonState = gp.IsStartButtonDown;
                    }

                    // Left Thumb Button
                    // Check to see if anything changed
                    if (!(lastLeftThumbButtonState == gp.IsLeftThumbButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.LeftThumb, gp.IsLeftThumbButtonDown);

                        if (gp.IsLeftThumbButtonDown) // If button changed to press state
                        {
                            if (OnLeftThumbButtonPress != null) OnLeftThumbButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnLeftThumbButtonRelease != null) OnLeftThumbButtonRelease(this, btnEvt);
                        }

                        lastLeftThumbButtonState = gp.IsLeftThumbButtonDown;
                    }

                    // Right Thumb Button
                    // Check to see if anything changed
                    if (!(lastRightThumbButtonState == gp.IsRightThumbButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.RightThumb, gp.IsRightThumbButtonDown);

                        if (gp.IsRightThumbButtonDown) // If button changed to press state
                        {
                            if (OnRightThumbButtonPress != null) OnRightThumbButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnRightThumbButtonRelease != null) OnRightThumbButtonRelease(this, btnEvt);
                        }

                        lastRightThumbButtonState = gp.IsRightThumbButtonDown;
                    }

                    // Left Shoulder Button
                    // Check to see if anything changed
                    if (!(lastLeftShoulderButtonState == gp.IsLeftShoulderButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.LeftShoulder, gp.IsLeftShoulderButtonDown);

                        if (gp.IsLeftShoulderButtonDown) // If button changed to press state
                        {
                            if (OnLeftShoulderButtonPress != null) OnLeftShoulderButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnLeftShoulderButtonRelease != null) OnLeftShoulderButtonRelease(this, btnEvt);
                        }

                        lastLeftShoulderButtonState = gp.IsLeftShoulderButtonDown;
                    }

                    // Right Shoulder Button
                    // Check to see if anything changed
                    if (!(lastRightShoulderButtonState == gp.IsRightShoulderButtonDown))
                    {
                        // Something has changed, lets dispatch events
                        GamepadButtonEventArgs btnEvt = new GamepadButtonEventArgs(deviceID, GamepadButtons.RightShoulder, gp.IsRightShoulderButtonDown);

                        if (gp.IsRightShoulderButtonDown) // If button changed to press state
                        {
                            if (OnRightShoulderButtonPress != null) OnRightShoulderButtonPress(this, btnEvt);
                        }
                        else // If button changed to release state
                        {
                            if (OnRightShoulderButtonRelease != null) OnRightShoulderButtonRelease(this, btnEvt);
                        }

                        lastRightShoulderButtonState = gp.IsRightShoulderButtonDown;
                    }

                    // DPad
                    // Check to see if DPad state has changed.
                    GamepadButtons tmpButtons = (GamepadButtons)gp.Buttons;
                    GamepadButtons dpNewState = (GamepadButtons)((int)tmpButtons % 0x10); // Remove other button states
                    if (dpNewState != lastDPadState) // A change has occured
                    {
                        GamepadDPadEventArgs padEvt = new GamepadDPadEventArgs(deviceID, dpNewState);

                        if (dpNewState == 0) // DPad has been released
                        {
                            if (OnDPadRelease != null) OnDPadRelease(this, padEvt);
                        }
                        else if (lastDPadState == 0) // DPad has been pressed
                        {
                            if (OnDPadPress != null) OnDPadPress(this, padEvt);
                        }

                        if (OnDPadChange != null) OnDPadChange(this, padEvt);

                        lastDPadState = dpNewState;
                    }

                    // Left Trigger
                    // Check to see if left trigger has changed.
                    if (lastLeftTrigger != gp.LeftTrigger) // A change has occured
                    {
                        GamepadTriggerEventArgs trigEvt = new GamepadTriggerEventArgs(deviceID, GamepadTriggers.Left, gp.LeftTrigger);

                        if (!leftTriggerPressed) // Trigger was pressed
                        {
                            leftTriggerPressed = true;
                            if (OnLeftTriggerPress != null) OnLeftTriggerPress(this, trigEvt);
                        }
                        else if (gp.LeftTrigger == 0) // Trigger was released
                        {
                            if (OnLeftTriggerRelease != null) OnLeftTriggerRelease(this, trigEvt);
                            leftTriggerPressed = false;
                        }

                        if (OnLeftTriggerChange != null) OnLeftTriggerChange(this, trigEvt);

                        lastLeftTrigger = gp.LeftTrigger;
                    }

                    // Right Trigger
                    // Check to see if right trigger has changed.
                    if (lastRightTrigger != gp.RightTrigger) // A change has occured
                    {
                        GamepadTriggerEventArgs trigEvt = new GamepadTriggerEventArgs(deviceID, GamepadTriggers.Right, gp.RightTrigger);

                        if (!rightTriggerPressed) // Trigger was pressed
                        {
                            rightTriggerPressed = true;
                            if (OnRightTriggerPress != null) OnRightTriggerPress(this, trigEvt);
                        }
                        else if (gp.RightTrigger == 0) // Trigger was released
                        {
                            if (OnRightTriggerRelease != null) OnRightTriggerRelease(this, trigEvt);
                            rightTriggerPressed = false;
                        }

                        if (OnRightTriggerChange != null) OnRightTriggerChange(this, trigEvt);

                        lastRightTrigger = gp.RightTrigger;
                    }

                    // Left Thumb Joystick
                    GamepadThumbEventArgs joyEvt = new GamepadThumbEventArgs(deviceID, GamepadThumbs.Left, gp.LeftThumbX, gp.LeftThumbY, gp.IsLeftThumbButtonDown);
                    if (OnLeftThumbUpdate != null) OnLeftThumbUpdate(this, joyEvt);

                    // Right Thumb Joystick
                    joyEvt = new GamepadThumbEventArgs(deviceID, GamepadThumbs.Right, gp.RightThumbX, gp.RightThumbY, gp.IsRightThumbButtonDown);
                    if (OnRightThumbUpdate != null) OnRightThumbUpdate(this, joyEvt);

                    // Update complete
                }
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == 1167) // This is a disconnect error
                {
                    if (OnDisconnect != null) OnDisconnect(this, new GamepadEventArgs(deviceID, GamepadState.Disconnected));
                    isConnected = false;
                }
                else
                {
                    throw e; // Unknown error, rethrow
                }
            }
        }

        /**
         * Sets the vibration level for the gamepad controller
         * The leftSpeed adjusts the low frequency motor and the rightSpeed adjusts the high frequency motor
         **/
        public void SetVibration(ushort leftSpeed, ushort rightSpeed)
        {
            XInputMethods.SetVibration(deviceID, leftSpeed, rightSpeed);
        }

        // Get the DirectSound guids for sending/receiving audio to the controller's headset
        public void GetAudioGuids(out Guid renderGuid, out Guid captureGuid)
        {
            XInputMethods.GetAudioGuids(deviceID, out renderGuid, out captureGuid);
        }
    }
}

#endif