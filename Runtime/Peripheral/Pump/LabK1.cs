using System;
using System.IO.Ports;
using PsychoUnity.Manager;
using UnityEngine;

namespace PsychoUnity.Peripheral.Pump
{
    /// <summary>
    /// Innofluid Co., Ltd. (申辰流体科技（上海）有限公司) LabK1 
    /// </summary>
    public class LabK1
    {
        private SerialPortConfig _config;
        
        private const string PumpName = "LabK1"; // default pump name
        
        private const int BaudRate = 9600; // default baud rate
        private const int DataBits = 8; // default data bits
        private const StopBits Stop = StopBits.One; // default stop bits
        private const Parity ParityBits = Parity.Even; // default parity bits

        private const byte ReadCmd = 0x03; // predefined read command
        private const byte WriteInt = 0x06; // predefined write command for int type data
        private const byte WriteFloat = 0x10; // predefined write command for float type data

        #region LabK1 Registers
        
        private readonly byte[] _startStopRegister = { 0x03, 0xE8 }; // start and stop the pump register
        private readonly byte[] _directionRegister = { 0x03, 0xE9 }; // control pump rotation direction register
        private readonly byte[] _speedRegister = { 0x03, 0xEA }; // control pump rotation speed register

        private readonly byte[] _startCmd = { 0x00, 0x01 }; // pump start command
        private readonly byte[] _stopCmd = { 0x00, 0x00 }; // pump stop command

        private readonly byte[] _clockwise = { 0x00, 0x01 }; // clockwise direction
        private readonly byte[] _anticlockwise = { 0x00, 0x00 }; // anticlockwise direction

        private readonly byte[] _numOfRegister = { 0x00, 0x02 };
        private readonly byte[] _sizeOfFloat = { 0x04 };

        #endregion

        /// <summary>
        /// Auto-detect the port number and initialize the LabK1 pump
        /// </summary>
        public LabK1()
        {
            var portName = AutoDetect();
            if (portName != null)
            {
                InitLabK1(portName);
            }
            else
            {
                Debug.LogError("Can't detect correct port number, please point it manually");
            }
        }
        
        /// <summary>
        /// Initialize the LabK1 pump with the specified port name
        /// </summary>
        /// <param name="portName"> port name of labk1 </param>
        public LabK1(string portName)
        {
            InitLabK1(portName);
        }

        /// <summary>
        /// Initialize the LabK1 pump with the specified port name and baud rate
        /// </summary>
        /// <param name="portName"></param>
        private void InitLabK1(string portName)
        {
            _config = new SerialPortConfig(portName, BaudRate, DataBits, Stop, ParityBits);
            SerialComManager.Instance.AddSerialCom(PumpName, portName, BaudRate);
            SerialComManager.Instance.SetSerialCom(PumpName, _config);
            SerialComManager.Instance.Open(PumpName);
        }

        /// <summary>
        /// Detect the port number of LabK1 pump by vendor ID and product ID
        /// </summary>
        /// <returns> port name </returns>
        private string AutoDetect()
        {
            return "COM9";
        }

        /// <summary>
        /// build pump command
        /// </summary>
        /// <param name="pumpAddress"> address of pump </param>
        /// <param name="cmdId"> command id </param>
        /// <param name="cmdRegister"> register need to control </param>
        /// <param name="cmd"> which command need to send </param>
        /// <returns> command </returns>
        private static byte[] BuildMsg(byte pumpAddress, byte cmdId, byte[] cmdRegister, byte[] cmd)
        {
            var result = new byte[2 + cmdRegister.Length + cmd.Length + 2];
            result[0] = pumpAddress;
            result[1] = cmdId;
            
            Buffer.BlockCopy(cmdRegister, 0, result, 2, cmdRegister.Length);
            Buffer.BlockCopy(cmd, 0, result, 2 + cmdRegister.Length, cmd.Length);
            
            var crc = CalcCrc(result);
            crc.CopyTo(result, result.Length - 2);

            return result;
        }

        private static byte[] BuildMshFloat(byte pumpAddress, byte cmdId, byte[] cmdRegister, byte[] numOfRegister, byte[] sizeOfFloat = null, byte[] msg = null)
        {
            var bufLength = 2 + cmdRegister.Length + numOfRegister.Length + (sizeOfFloat?.Length ?? 0) + (msg?.Length ?? 0) + 2;
            var result = new byte[bufLength];
            result[0] = pumpAddress;
            result[1] = cmdId;
            
            Buffer.BlockCopy(cmdRegister, 0, result, 2, cmdRegister.Length);
            Buffer.BlockCopy(numOfRegister, 0, result, 2 + cmdRegister.Length, numOfRegister.Length);
            
            
            if (sizeOfFloat != null)
                Buffer.BlockCopy(sizeOfFloat, 0, result, 2 + cmdRegister.Length + numOfRegister.Length,
                    sizeOfFloat.Length);

            if (msg != null)
                Buffer.BlockCopy(msg, 0, result, 3 + cmdRegister.Length + numOfRegister.Length, msg.Length);
            
            var crc = CalcCrc(result);
            crc.CopyTo(result, result.Length - 2);

            return result;
        }

        private static byte[] CalcCrc(byte[] msg)
        {
            var crc = new byte[2];
            uint crcRegister = 0xFFFF;
            
            for (var i = 0; i < msg.Length - 2; i++)
            {
                crcRegister ^= msg[i];
                for (var j = 0; j < 8; j++)
                {
                    if ((crcRegister & 0x0001) != 0)
                    {
                        crcRegister >>= 1;
                        crcRegister ^= 0xA001;
                    }
                    else
                    {
                        crcRegister >>= 1;
                    }
                }
            }

            crc[0] = (byte)(crcRegister & 0xFF);
            crc[1] = (byte)(crcRegister >> 8);

            return crc;
        }

        public void GiveReward(byte pumpID = 0x01)
        {
            var msg = BuildMsg(pumpID, WriteInt, _startStopRegister, _startCmd);
            SerialComManager.Instance.Write(PumpName, msg);
        }

        public void StopReward(byte pumpID = 0x01)
        {
            var msg = BuildMsg(pumpID, WriteInt, _startStopRegister, _stopCmd);
            SerialComManager.Instance.Write(PumpName, msg);
        }

        public void SetSpeed(float speed, byte pumpId = 0x01)
        {
            var data = BitConverter.GetBytes(speed);
            Array.Reverse(data);
            var msg = BuildMshFloat(pumpId, WriteFloat, _speedRegister, _numOfRegister, _sizeOfFloat, data);
            SerialComManager.Instance.Write(PumpName, msg);
        }

        public float GetSpeed(byte pumpId = 0x01)
        {
            var msg = BuildMshFloat(pumpId, ReadCmd, _speedRegister, _numOfRegister);
            float result = -1;
            SerialComManager.Instance.Write(PumpName, msg);

            var receiveBuf = new byte[100];
            SerialComManager.Instance.Read(PumpName, msg);
            
            if (receiveBuf.Length < 9) return result;
            
            for (var i = 0; i < receiveBuf.Length; i++)
            {
                if (receiveBuf[i] != pumpId || receiveBuf[i + 1] != ReadCmd) continue;
                byte[] tmp =  {receiveBuf[i + 3], receiveBuf[i + 4], receiveBuf[i + 5], receiveBuf[i + 6]};
                Array.Reverse(tmp);
                result = BitConverter.ToSingle(tmp);
            }
            return result;
        }
    }
}