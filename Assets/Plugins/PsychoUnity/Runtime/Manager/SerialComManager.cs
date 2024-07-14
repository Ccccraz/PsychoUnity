using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public struct SerialPortConfig
    {
        public readonly string PortName;
        public readonly int BaudRate;
        public readonly int DataBits;
        public readonly StopBits StopBits;
        public readonly Parity ParityBits;
        public readonly Handshake FlowCtrl;

        public SerialPortConfig(string portName, int baudRate, int dataBits = 8,
            StopBits stopBits = StopBits.None, Parity parityBits = Parity.None, Handshake flowCtrl = Handshake.None)
        {
            PortName = portName;
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            ParityBits = parityBits;
            FlowCtrl = flowCtrl;
        }
    }

    /// <summary>
    /// Serial Device Manager
    /// </summary>
    public class SerialComManager : Singleton<SerialComManager>
    {
        private readonly Dictionary<string, SerialCom> _serialComDic = new Dictionary<string, SerialCom>();

        /// <summary>
        /// Add a serial device
        /// </summary>
        /// <param name="deviceName"> Define a name for the serial device </param>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public bool AddSerialCom(string deviceName, string portName, int baudRate)
        {
            var serialPort = CheckSerialPort(deviceName);

            if (serialPort != null)
            {
                Debug.LogWarning("Serial port is existed");
                return false;
            }

            _serialComDic.Add(deviceName, new SerialCom(portName, baudRate));

            return true;
        }

        /// <summary>
        /// complete config struct for serialPort
        /// </summary>
        /// <summary>
        /// Serial port setup
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="config">your config</param>
        public void SetSerialCom(string deviceName, SerialPortConfig config)
        {
            var serialCom = CheckSerialPort(deviceName);
            if (serialCom == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            }
            
            serialCom.Set(config);
        }

        public void EnableDtr(string serialComName, bool enable)
        {
            var serialCom = CheckSerialPort(serialComName);

            if (serialCom == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            }
            
            serialCom.EnableDtr((enable));
        }

        public void EnableRts(string serialComName, bool enable)
        {
            var serialCom = CheckSerialPort(serialComName);

            if (serialCom == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            }
            
            serialCom.EnableRts(enable);
        }

        public SerialPort GetSerialPort(string serialComName)
        {
            var serialCom = CheckSerialPort(serialComName);

            if (serialCom != null) return serialCom.Get();
            Debug.LogError("Serial port doesn't exist");
            return null;
        }

        /// <summary>
        /// Open a serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        public void Open(string serialComName)
        {
            var serialCom = CheckSerialPort(serialComName);
            if (serialCom == null)
            {
                Debug.LogWarning("SerialPort was not exist that you want to open");
                return;
            }

            if (serialCom.IsOpen())
            {
                Debug.LogWarning("SerialPort is already opened");
                return;
            }

            try
            {
                serialCom.Begin();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Write a message to a serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        /// <param name="msgBuf"> message buffer </param>
        public void Write(string serialComName, byte[] msgBuf)
        {
            try
            {
                _serialComDic[serialComName].Write(msgBuf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Read message from serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        /// <param name="msgBuf"> message buffer </param>
        public void Read(string serialComName, byte[] msgBuf)
        {
            try
            {
                _serialComDic[serialComName].Read(msgBuf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public void ListenMsg(string serialComName)
        {
            try
            {
                _serialComDic[serialComName].ListenMsg();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public void SendMsg(string serialComName, byte[] msg)
        {
            try
            {
                _serialComDic[serialComName].SendMsg(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Close the serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        public void Close(string serialComName)
        {
            var serialCom = CheckSerialPort(serialComName);
            if (serialCom == null) return;

            if (!serialCom.IsOpen()) return;

            serialCom.Close();
        }

        /// <summary>
        /// Close all serial device
        /// </summary>
        public void Clear()
        {
            foreach (var item in _serialComDic)
            {
                Close(item.Key);
            }

            _serialComDic.Clear();
        }

        /// <summary>
        /// Check that the serial device is exists
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        /// <returns> Returns true if it exists, false otherwise </returns>
        private SerialCom CheckSerialPort(string serialComName)
        {
            return _serialComDic.GetValueOrDefault(serialComName);
        }
    }

    internal class SerialCom
    {
        private enum State
        {
            WaitHeader1,
            WaitHeader2,
            WaitLen,
            WaitData,
            WaitCrc,
            Received
        }

        private readonly SerialPort _serialPort;
        private State _state;
        private readonly byte[] _header = { 0x59, 0x49 };
        private readonly List<byte> _data = new List<byte>(3);

        internal SerialCom(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _state = State.WaitHeader1;
            _data.AddRange(_header);
            _data.Add(0x00);
        }

        internal void Set(SerialPortConfig config)
        {
            _serialPort.PortName = config.PortName;
            _serialPort.BaudRate = config.BaudRate;
            _serialPort.DataBits = config.DataBits;
            _serialPort.StopBits = config.StopBits;
            _serialPort.Parity = config.ParityBits;
            _serialPort.Handshake = config.FlowCtrl;
        }

        internal void EnableDtr(bool enable)
        {
            _serialPort.DtrEnable = enable;
        }

        internal void EnableRts(bool enable)
        {
            _serialPort.RtsEnable = enable;
        }

        internal void Begin()
        {
            _serialPort.Open();
        }

        internal void Read(byte[] buf)
        {
            _serialPort.Read(buf, 0, buf.Length);
        }

        internal void Write(byte[] buf)
        {
            _serialPort.Write(buf, 0, buf.Length);
        }

        internal void ListenMsg()
        {
            byte[] incomingByte = new byte[1];
            switch (_state)
            {
                case State.WaitHeader1:
                    _serialPort.Read(incomingByte, 0, incomingByte.Length);
                    if (incomingByte[0] == _header[0])
                    {
                        _state = State.WaitHeader2;
                    }

                    break;
                
                case State.WaitHeader2:
                    _serialPort.Read(incomingByte, 0, incomingByte.Length);
                    _state = incomingByte[0] == _header[1] ? State.WaitLen : State.WaitHeader1;

                    break;
                
                case State.WaitLen:
                    var numRead = _serialPort.Read(incomingByte, 0, incomingByte.Length);
                    if (numRead >= 1)
                    {
                        _data[2] = incomingByte[0];
                        _state = State.WaitData;
                    }
                    else
                    {
                        _state = State.WaitHeader1;
                    }
                    break;
                
                case State.WaitData:
                    _state = GetData() ? State.WaitCrc : State.WaitHeader1;
                    break;
                
                case State.WaitCrc:
                    _state = CheckCrc() ? State.Received : State.WaitHeader1;
                    break;
                
                case State.Received:
                    Trigger();
                    _state = State.WaitHeader1;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool GetData()
        {
            _data.RemoveRange(3, _data.Count - 3);
            var buf = new byte[_data[2]];
            var num = _serialPort.Read(buf, 0, buf.Length);
            
            if (num != _data[2]) return false;
            
            _data.AddRange(buf);
            
            return true;
        }

        internal void SendMsg(byte[] msg)
        {
            var dataBuf = new List<byte>(){_header[0], _header[1], (byte)msg.Length};
            dataBuf.AddRange(msg);
            dataBuf.AddRange(Crc16.CalcCrc(dataBuf));
            _serialPort.Write(dataBuf.ToArray(), 0, dataBuf.Count);
        }

        private bool CheckCrc()
        {
            var buf = new byte[2];
            _serialPort.Read(buf, 0, buf.Length);
            var crc = Crc16.CalcCrc(_data);

            return buf[0] == crc[0] && buf[1] == crc[1];
        }

        private void Trigger()
        {
            EventManager.Instance.EventTrigger(_data[3].ToString(), _data);
        }

        internal SerialPort Get()
        {
            return _serialPort;
        }

        internal bool IsOpen()
        {
            return _serialPort.IsOpen;
        }

        internal void Close()
        {
            _serialPort.Close();
        }
    }

    public static class Crc16
    {
        private const ushort PolyNominal = 0x1021;
        private static readonly ushort[] Table = new ushort[256];
        static Crc16()
        {
            for (var i = 0; i < Table.Length; i++)
            {
                ushort temp = 0;
                var a = (ushort)(i << 8);
                for (var j = 0; j < 8; j++)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ PolyNominal);
                    }
                    else
                    {
                        temp <<= 1;
                    }

                    a <<= 1;
                }

                Table[i] = temp;
            }
        }

        public static byte[] CalcCrc(List<byte> data)
        {
            ushort crc = 0x0000;
            
            foreach (var t in data)
            {
                crc = (ushort)((crc << 8) ^ Table[((crc >> 8) ^ (0xff & t))]);
            }

            var result =  BitConverter.GetBytes(crc);
            (result[0], result[1]) = (result[1], result[0]);

            return result;
        }
    }
}