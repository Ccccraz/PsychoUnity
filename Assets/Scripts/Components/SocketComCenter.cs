using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Components
{
    public class SocketComCenter : Singleton<SocketComCenter>
    {
        public bool StreamAvailable { get; private set; }
        public byte[] Data { get; private set; }
        public int DataSize { get; private set; }
        
        private TcpListener _server;
        private NetworkStream _stream;

        public async void SetServer(string hostname, int port, int bufSize)
        {
            Data = new byte[bufSize];
            
            _server = new TcpListener(IPAddress.Parse(hostname), port);
            _server.Start();

            using var client = await _server.AcceptTcpClientAsync();
            Debug.Log("Connected");
            
            _stream = client.GetStream();
            StreamAvailable = true;
            ReadMsg();
        }

        private async void ReadMsg()
        {
            while (_stream.DataAvailable)
            {
                Array.Clear(Data, 0, Data.Length);
                DataSize = await _stream.ReadAsync(Data, 0, Data.Length);
            } 
        }

        public void SendStringAsync(string msg)
        {
            SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        public async void SendAsync(byte[] msg)
        {
            if (_stream.CanWrite)
            {
                await _stream.WriteAsync(msg, 0, msg.Length);
            }
            else
            {
                Debug.Log("client is not available");
            }
        }

        public void CloseServer()
        {
            _server.Stop();
            _stream.DisposeAsync();
        }
    }
}
