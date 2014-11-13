#if DEBUG

/**
 * Wrapper for XInput
 * Provided by http://www.pluralsight.com/blogs/dbox/archive/2005/12/10/17384.aspx
 * Modified by Jean-Philippe Steinmetz <caskater47@gmail.com>
 **/

namespace Microsoft.DirectX.XInput
{
    using System;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    public enum XInputDeviceType : byte
    {
        GamePad = 1
    }
    public enum XInputDeviceSubtype : byte
    {
        GamePad = 1
    }
    [Flags]
    public enum XInputCapabilityFlags : ushort
    {
        VoiceSupported = 4
    }

    [Flags]
    public enum XInputButtons : ushort
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

    [Serializable]
    public struct XInputCapabilities
    {
        public XInputDeviceType Type;
        public XInputDeviceSubtype SubType;
        public XInputCapabilityFlags Flags;
        public XInputGamepad Gamepad;
        public XInputVibration Vibration;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3} {4})", Type, SubType, Flags, Gamepad, Vibration);
        }
    }

    [Serializable]
    public struct XInputGamepad
    {
        public XInputButtons Buttons;
        public byte LeftTrigger;
        public byte RightTrigger;
        public short LeftThumbX;
        public short LeftThumbY;
        public short RightThumbX;
        public short RightThumbY;

        public bool IsAButtonDown
        {
            get { return (Buttons & XInputButtons.A) != 0; }
        }
        public bool IsBButtonDown
        {
            get { return (Buttons & XInputButtons.B) != 0; }
        }
        public bool IsXButtonDown
        {
            get { return (Buttons & XInputButtons.X) != 0; }
        }
        public bool IsYButtonDown
        {
            get { return (Buttons & XInputButtons.Y) != 0; }
        }
        public bool IsStartButtonDown
        {
            get { return (Buttons & XInputButtons.Start) != 0; }
        }
        public bool IsBackButtonDown
        {
            get { return (Buttons & XInputButtons.Back) != 0; }
        }
        public bool IsDPadLeftButtonDown
        {
            get { return (Buttons & XInputButtons.DPadLeft) != 0; }
        }
        public bool IsDPadRightButtonDown
        {
            get { return (Buttons & XInputButtons.DPadRight) != 0; }
        }
        public bool IsDPadUpButtonDown
        {
            get { return (Buttons & XInputButtons.DPadUp) != 0; }
        }
        public bool IsDPadDownButtonDown
        {
            get { return (Buttons & XInputButtons.DPadDown) != 0; }
        }
        public bool IsLeftThumbButtonDown
        {
            get { return (Buttons & XInputButtons.LeftThumb) != 0; }
        }
        public bool IsRightThumbButtonDown
        {
            get { return (Buttons & XInputButtons.RightThumb) != 0; }
        }
        public bool IsLeftShoulderButtonDown
        {
            get { return (Buttons & XInputButtons.LeftShoulder) != 0; }
        }
        public bool IsRightShoulderButtonDown
        {
            get { return (Buttons & XInputButtons.RightShoulder) != 0; }
        }


        public override bool Equals(object obj)
        {
            if (obj is XInputGamepad)
                return this.Equals((XInputGamepad)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return (int)Buttons + LeftTrigger + RightTrigger;
        }

        public bool Equals(XInputGamepad rhs)
        {
            return
                Buttons == rhs.Buttons
                && LeftTrigger == rhs.LeftTrigger
                && RightTrigger == rhs.RightTrigger
                && LeftThumbX == rhs.LeftThumbX
                && LeftThumbY == rhs.LeftThumbY
                && RightThumbX == rhs.RightThumbX
                && RightThumbY == rhs.RightThumbY;
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3} {4} {5} {6})", Buttons, LeftTrigger, RightTrigger, LeftThumbX, LeftThumbY, RightThumbX, RightThumbY);
        }
    }

    [Serializable]
    public struct XInputState
    {
        public uint PacketNumber;
        public XInputGamepad Gamepad;

        public bool Equals(XInputState rhs)
        {
            return PacketNumber == rhs.PacketNumber
                    && Gamepad.Equals(rhs.Gamepad);
        }

        public override bool Equals(object obj)
        {
            if (obj is XInputState)
                return this.Equals((XInputState)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return (int)PacketNumber;
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", PacketNumber, Gamepad);
        }
    }

    [Serializable]
    public struct XInputVibration
    {
        public ushort LeftMotorSpeed;
        public ushort RightMotorSpeed;

        public XInputVibration(ushort left, ushort right)
        {
            this.LeftMotorSpeed = left;
            this.RightMotorSpeed = right;
        }
    }


    public static class XInputMethods
    {
        // Get the capabilities of controller 0, 1, 2, or 3
        public static void GetCapabilities(int controllerNumber, out XInputCapabilities caps)
        {
            XInputMethods.ProcessResult(
                XInputMethods.XInputGetCapabilities(controllerNumber, 0, out caps)
                );
        }

        // Poll the state of the controller's buttons, thumbsticks, and triggers
        public static void GetState(int controllerNumber, out XInputState state)
        {
            XInputMethods.ProcessResult(
                XInputMethods.XInputGetState(controllerNumber, out state)
                );
        }

        // Set the vibration speed of a controller
        public static void SetVibration(int controllerNumber, ushort leftSpeed, ushort rightSpeed)
        {
            XInputVibration vibe = new XInputVibration(leftSpeed, rightSpeed);
            XInputMethods.ProcessResult(
                XInputMethods.XInputSetState(controllerNumber, ref vibe)
                );
        }

        // Get the DirectSound guids for sending/receiving audio to the controller's headset
        public static void GetAudioGuids(int controllerNumber, out Guid renderGuid, out Guid captureGuid)
        {
            XInputMethods.ProcessResult(XInputMethods.XInputGetDSoundAudioDeviceGuids(controllerNumber, out renderGuid, out captureGuid));
        }



        static void ProcessResult(uint err)
        {
            if (err != 0)
                throw new Win32Exception((int)err);
        }


        const string DllName = "XInput9_1_0.dll";

        [DllImport(DllName)]
        extern static uint XInputGetState(
            int index,  // [in] Index of the gamer associated with the device
            out XInputState state // [out] Receives the current state
        );

        [DllImport(DllName)]
        extern static uint XInputSetState(
            int index,  // [in] Index of the gamer associated with the device
            ref XInputVibration vibration// [in, out] The vibration information to send to the controller
        );

        [DllImport(DllName)]
        extern static uint XInputGetCapabilities(
            int dwUserIndex,   // [in] Index of the gamer associated with the device
            uint dwFlags,       // [in] Input flags that identify the device type
            out XInputCapabilities capabilities  // [out] Receives the capabilities
        );

        [DllImport(DllName)]
        extern static uint XInputGetDSoundAudioDeviceGuids(
            int dwUserIndex,          // [in] Index of the gamer associated with the device
            out Guid soundRenderGuid,    // [out] DSound device ID for render
            out Guid SoundCaptureGuid    // [out] DSound device ID for capture
        );
    }
}

#endif