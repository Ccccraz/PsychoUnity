using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace PsychoUnity.Peripheral
{
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

    public static class SimiaPump
    {
        private static Joystick GetPump()
        {
            var devices = Joystick.all;
            return devices.FirstOrDefault(device => device.name.Contains("simia pump"));
        }

        public static void GiveReward()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(1, 0, 0, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        public static void GiveReward(int duration)
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(1, duration, 0, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        public static void StopReward()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(0, 0, 1, 0, 0);
            pump.device.ExecuteCommand(ref cmd);
        }

        public static void Reverse()
        {
            var pump = GetPump();
            if (pump == null) return;
            var cmd = PumpHidCmd.Create(0, 0, 0, 0, 1);
            pump.device.ExecuteCommand(ref cmd);
        }
    }
}