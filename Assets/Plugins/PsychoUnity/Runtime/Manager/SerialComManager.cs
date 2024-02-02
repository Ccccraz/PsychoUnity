using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// Serial Device Manager
    /// </summary>
    [Obsolete]
    public class SerialComManager : Singleton<SerialComManager>
    {
        private readonly Dictionary<string, SerialPort> _serialComDic = new Dictionary<string, SerialPort>();
        
        /// <summary>
        /// Add and setup a serial device
        /// </summary>
        /// <param name="serialComName"> Define a name for the serial device </param>
        /// <param name="portName"> communication port </param>
        /// <param name="baudRate"> baud rate </param>
        public void AddSerialCom(string serialComName, string portName, int baudRate)
        {
            AddSerialCom(serialComName);
            SetSerialCom(serialComName, portName, baudRate);
        }

        /// <summary>
        /// Add a serial device
        /// </summary>
        /// <param name="deviceName"> Define a name for the serial device </param>
        public void AddSerialCom(string deviceName)
        {
            if (_serialComDic.ContainsKey(deviceName))
            {
                Debug.Log("Device is already exists");
            }
            else
            {
                _serialComDic.Add(deviceName, new SerialPort());
            }
        }

        /// <summary>
        /// Setup a serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        /// <param name="portName"> communication port name </param>
        /// <param name="baudRate"> baud rate </param>
        public void SetSerialCom(string serialComName, string portName, int baudRate)
        {
            if (!_serialComDic.TryGetValue(serialComName, out var serialPort))
            {
                Debug.Log("SerialPort does not exist that you want to config");
                return;
            }
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
        }

        /// <summary>
        /// Open a serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        public void Open(string serialComName)
        {
            if (_serialComDic.TryGetValue(serialComName, out var serialPort))
            {
                Debug.Log("SerialPort was not exist that you want to open");
                return;
            }

            if (serialPort.PortName == null || serialPort.BaudRate == 0)
            {
                Debug.Log("SerialPort has not enough parameter that you want to open");
                return;
            }

            if (serialPort.IsOpen)
            {
                Debug.Log("SerialPort is already opened");
                return;
            }

            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
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
            if (CheckSerialPort(serialComName))
            {
                _serialComDic[serialComName].Write(msgBuf, 0, msgSize);
            }
            else
            {
                Debug.Log($"Can't write to {serialComName}");
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
            _serialComDic[serialComName].Read(msgBuf, 0, msgSize);
        }

        /// <summary>
        /// Check that the serial device is exists
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        /// <returns> Returns true if it exists, false otherwise </returns>
        public bool CheckSerialPort(string serialComName)
        {
            if (!_serialComDic.TryGetValue(serialComName, out var serialPort))
            {
                Debug.Log("SerialPort does not exist that you want to write");
                return false;
            }

            if (!serialPort.IsOpen)
            {
                Debug.Log("SerialPort was not open that you want to write");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Close the serial device
        /// </summary>
        /// <param name="serialComName"> target device name </param>
        public void Close(string serialComName)
        {
            if (!_serialComDic.TryGetValue(serialComName, out var serialPort)) return;

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
    }
}
