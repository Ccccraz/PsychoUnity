using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace PsychoUnity.Peripheral
{
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    public struct PumpHidCmd : IInputDeviceCommandInfo
    {
        public static FourCC Type => new('H', 'I', 'D', 'O');
        internal const int id = 0;
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 21;

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
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = id,
                startCmd = start,
                durationCmd = duration,
                stopCmd = stop,
                speedCmd = speed,
                reverseCmd = reverse
            };
        }
    }
}