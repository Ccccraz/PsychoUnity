using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace Components
{
    public class SerialComCenter : Singleton<SerialComCenter>
    {
        private readonly Dictionary<string, SerialPort> _serialComDic = new Dictionary<string, SerialPort>();
        
        public void AddSerialCom(string serialComName, string portName, int baudRate)
        {
            AddSerialCom(serialComName);
            SetSerialCom(serialComName, portName, baudRate);
        }

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

        public void Read(string serialComName, ref byte[] msgBuf, int msgSize)
        {
            _serialComDic[serialComName].Read(msgBuf, 0, msgSize);
        }

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

        public void Close(string serialComName)
        {
            if (!_serialComDic.TryGetValue(serialComName, out var serialPort)) return;

            if (!serialPort.IsOpen) return;
            
            serialPort.Close();
        }

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
