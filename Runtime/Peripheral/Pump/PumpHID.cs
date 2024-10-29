using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace PsychoUnity.Peripheral.Pump
{
    /// <summary>
    /// This is the structure of the HID report for the pump
    /// Create a command using the Create method
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = KSize)]
    public struct PumpHidCmd : IInputDeviceCommandInfo
    {
        private static FourCC Type => new('H', 'I', 'D', 'O');
        private const int ID = 0;
        private const int KSize = InputDeviceCommand.BaseCommandSize + 21;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public byte reportId;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)]
        public float startCmd;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 5)]
        public float durationCmd;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 9)]
        public float stopCmd;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 13)]
        public float speedCmd;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 17)]
        public float reverseCmd;

        public FourCC typeStatic => Type;


        /// <summary>
        /// Initializes a new pump command
        /// </summary>
        /// <param name="start">Indicates whether to start the pump. Set to 1 to start the pump.</param>
        /// <param name="duration">Specifies the duration for which the pump should run, in milliseconds. Set to 0 for indefinite operation.</param>
        /// <param name="stop">Indicates whether to stop the pump. Set to 1 to stop the pump.</param>
        /// <param name="speed">Specifies the speed at which the pump should operate.</param>
        /// <param name="reverse">Specifies whether to reverse the direction of the pump's operation. Set to true to reverse the direction.</param>
        /// <returns>pump command</returns>
        public static PumpHidCmd Create(float start, float duration, float stop, float speed, float reverse)
        {
            return new PumpHidCmd
            {
                baseCommand = new InputDeviceCommand(Type, KSize),
                reportId = ID,
                startCmd = start,
                durationCmd = duration,
                stopCmd = stop,
                speedCmd = speed,
                reverseCmd = reverse
            };
        }
    }

    /// <summary>
    /// Controls the pump using the HID interface
    /// </summary>
    public static class SimiaPump
    {
        /// <summary>
        /// Finds the pump device
        /// </summary>
        /// <returns>pump device</returns>
        private static Joystick GetPump()
        {
            var devices = Joystick.all;
            return devices.FirstOrDefault(device => device.name.Contains("simia pump"));
        }

        /// <summary>
        /// Gives a reward to the animal indefinitely
        /// </summary>
        public static void GiveReward()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(1, 0, 0, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        /// <summary>
        /// Gives a reward to the animal for a specified duration
        /// </summary>
        /// <param name="duration"> duration in milliseconds </param>
        public static void GiveReward(int duration)
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(1, duration, 0, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        /// <summary>
        /// Stops the reward
        /// </summary>
        public static void StopReward()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(0, 0, 1, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        /// <summary>
        /// Reverse the pump rotation direction
        /// </summary>
        public static void Reverse()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(0, 0, 0, 0, 1);
            pump.device.ExecuteCommand(ref cmd);
        }
    }
}