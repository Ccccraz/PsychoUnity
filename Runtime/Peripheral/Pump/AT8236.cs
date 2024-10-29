using System;
using PsychoUnity.Manager;

namespace PsychoUnity.Peripheral.Pump
{
    public class At8236
    {
        private const string DeviceName = "PumpAT8236";

        private readonly byte[] _cmdStart = { 0x00 };
        private readonly byte[] _cmdStop = { 0x01 };
        private readonly byte[] _cmdReverse = { 0x02 };
        private readonly byte[] _cmdSetSpeed = { 0x03 };
        private readonly byte[] _cmdGetSpeed = { 0x04 };

        public At8236(string portName, int baudRate)
        {
            SerialComManager.Instance.AddSerialCom(DeviceName, portName, baudRate);
            SerialComManager.Instance.EnableDtr(DeviceName, true);
            SerialComManager.Instance.Open(DeviceName);
        }

        public void GiveReward()
        {
            SerialComManager.Instance.SendMsg(DeviceName, _cmdStart);
        }

        public void GiveRewardToMilliSeconds(int duration)
        {
            var data = BitConverter.GetBytes(duration);
            var msg = new byte[_cmdStart.Length + data.Length];
            Buffer.BlockCopy(_cmdStart, 0, msg, 0, msg.Length);
            Buffer.BlockCopy(data, 0, msg, msg.Length, data.Length);
            SerialComManager.Instance.SendMsg(DeviceName, msg);
        }

        public void StopReward()
        {
            SerialComManager.Instance.SendMsg(DeviceName, _cmdStop);
        }

        public void Reverse()
        {
            SerialComManager.Instance.SendMsg(DeviceName, _cmdReverse);
        }

        public void SetDirection()
        {
        }

        public void SetSpeed(int speed)
        {
            var data = BitConverter.GetBytes(speed);
            var msg = new byte[_cmdSetSpeed.Length + data.Length];
            Buffer.BlockCopy(_cmdSetSpeed, 0, msg, 0, msg.Length);
            Buffer.BlockCopy(data, 0, msg, msg.Length, data.Length);
            SerialComManager.Instance.SendMsg(DeviceName, msg);
        }

        public void GetSpeed()
        {
            SerialComManager.Instance.SendMsg(DeviceName, _cmdGetSpeed);
        }

        public void Destroy()
        {
            SerialComManager.Instance.Close(DeviceName);
        }
    }
}