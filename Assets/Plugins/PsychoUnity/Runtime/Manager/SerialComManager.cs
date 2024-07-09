using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// Serial Device Manager
    /// </summary>
    public class SerialComManager : Singleton<SerialComManager>
    {
        private readonly Dictionary<string, SerialPort> _serialComDic = new Dictionary<string, SerialPort>();

        /// <summary>
        /// Add a serial device
        /// </summary>
        /// <param name="deviceName"> Define a name for the serial device </param>
        public bool AddSerialCom(string deviceName)
        {
            var serialPort = CheckSerialPort(deviceName);

            if (serialPort != null)
            {
                Debug.LogWarning("Serial port is existed");
                return false;
            }

            _serialComDic.Add(deviceName, new SerialPort());

            return true;
        }

        /// <summary>
        /// Add and setup a serial device
        /// </summary>
        /// <param name="serialComName"> Define a name for the serial device </param>
        /// <param name="portName"> communication port </param>
        /// <param name="baudRate"> baud rate </param>
        public void AddSerialCom(string serialComName, string portName, int baudRate)
        {
            if (!AddSerialCom(serialComName)) return;

            _serialComDic[serialComName].PortName = portName;
            _serialComDic[serialComName].BaudRate = baudRate;
        }

        /// <summary>
        /// complete config struct for serialPort
        /// </summary>
        public struct SerialPortConfig
        {
            public readonly string SerialComName;
            public readonly string PortName;
            public readonly int BaudRate;
            public readonly int DataBits;
            public readonly StopBits StopBits;
            public readonly Parity ParityBits;
            public readonly Handshake FlowCtrl;

            public SerialPortConfig(string serialComName, string portName, int baudRate, int dataBits = 8,
                StopBits stopBits = StopBits.None, Parity parityBits = Parity.None, Handshake flowCtrl = Handshake.None)
            {
                SerialComName = serialComName;
                PortName = portName;
                BaudRate = baudRate;
                DataBits = dataBits;
                StopBits = stopBits;
                ParityBits = parityBits;
                FlowCtrl = flowCtrl;
            }
        }

        /// <summary>
        /// Serial port setup
        /// </summary>
        /// <param name="config">your config</param>
        public void SetSerialCom(SerialPortConfig config)
        {
            var serialPort = CheckSerialPort(config.SerialComName);
            if (serialPort == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            };
            serialPort.PortName = config.PortName;
            serialPort.BaudRate = config.BaudRate;
            serialPort.DataBits = config.DataBits;
            serialPort.StopBits = config.StopBits;
            serialPort.Parity = config.ParityBits;
            serialPort.Handshake = config.FlowCtrl;
        }

        public void EnableDtr(string serialComName, bool enable)
        {
            var serialPort = CheckSerialPort(serialComName);

            if (serialPort == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            }

            serialPort.DtrEnable = enable;
        }

        public void EnableRts(string serialComName, bool enable)
        {
            var serialPort = CheckSerialPort(serialComName);

            if (serialPort == null)
            {
                Debug.LogError("Serial port doesn't exist");
                return;
            }

            serialPort.RtsEnable = enable;
        }

        public SerialPort GetSerialPort(string serialComName)
        {
            var serialPort = CheckSerialPort(serialComName);

            if (serialPort != null) return serialPort;
            Debug.LogError("Serial port doesn't exist");
            return null;
        }

        /// <summary>
        /// Open a serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        public void Open(string serialComName)
        {
            var serialPort = CheckSerialPort(serialComName);
            if (serialPort == null)
            {
                Debug.LogWarning("SerialPort was not exist that you want to open");
                return;
            }

            if (serialPort.PortName == null || serialPort.BaudRate == 0)
            {
                Debug.LogWarning("SerialPort has not enough parameter that you want to open");
                return;
            }

            if (serialPort.IsOpen)
            {
                Debug.LogWarning("SerialPort is already opened");
                return;
            }

            try
            {
                serialPort.Open();
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
        /// <param name="msgSize"> message size </param>
        public void Write(string serialComName, ref byte[] msgBuf, int msgSize)
        {
            try
            {
                _serialComDic[serialComName].Write(msgBuf, 0, msgSize);
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
        /// <param name="msgSize"> message size </param>
        public void Read(string serialComName, ref byte[] msgBuf, int msgSize)
        {
            try
            {
                _serialComDic[serialComName].Read(msgBuf, 0, msgSize);
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
            var serialPort = CheckSerialPort(serialComName);
            if (serialPort == null) return;

            if (!serialPort.IsOpen) return;

            serialPort.Close();
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
        private SerialPort CheckSerialPort(string serialComName)
        {
            return _serialComDic.GetValueOrDefault(serialComName);
        }
    }
}