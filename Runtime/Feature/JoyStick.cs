using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

struct Status
{
    public int Left;
    public int Right;
    public int Down;
    public int Up;
}

public class JoyStick : MonoBehaviour
{
    public GameObject target;
    public string portName;
    public int baudRate;
    public float speed;
    private SerialPort _serialPort;
    private byte[] _buf;
    private byte[] _headerBuf;
    private Status _status;
    private bool _isGetData = false;
    void Start()
    {
        _serialPort = new SerialPort(portName, baudRate);
        _serialPort.Open();
        _buf = new byte[16];
        _headerBuf = new byte[2];
        _status = new Status();
    }

    void Update()
    {
        if (GetData())
        {
            UpdatePosition();
        }
    }

    bool GetData()
    {
        if (_serialPort.BytesToRead >= 2)
        {
            if (_serialPort.Read(_headerBuf, 0, 2) == 2 && _headerBuf[0] == 0x00 && _headerBuf[1] == 0x04)
            {
                while (_serialPort.BytesToRead < 16)
                {
                }
            }
        }
        if (_serialPort.Read(_headerBuf, 0, 2) == 2)
        {
            if (_headerBuf[0] == 0x00 && _headerBuf[1] == 0x04)
            {
                _serialPort.Read(_buf, 0, 16);
                _status.Left = BitConverter.ToInt32(_buf, 0);
                _status.Right = BitConverter.ToInt32(_buf, 4);
                _status.Down = BitConverter.ToInt32(_buf, 8);
                _status.Up = BitConverter.ToInt32(_buf, 12);
                _isGetData = true;
            }
        }

        return _isGetData;
    }

    void UpdatePosition()
    {
        if (_status.Up == 0)
        {
            Debug.Log("ok");
            _isGetData = false;
        }
    }
}