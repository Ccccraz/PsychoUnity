using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class SocketComManager : Singleton<SocketComManager>
    {
        private bool _dataAvailable;

        public bool DataAvailable
        {
            get
            {
                var value = _dataAvailable;
                _dataAvailable = false;
                return value;
            }
        }

        public int DataSize { get; private set; }
        
        private TcpListener _server;
        private NetworkStream _stream;

        public async void SetServer(string hostname, int port, byte[] buf)
        {
            _server = new TcpListener(IPAddress.Parse(hostname), port);
            _server.Start();
            Debug.Log("Server created");

            var client = await _server.AcceptTcpClientAsync();
            Debug.Log("Connected");
            
            _stream = client.GetStream();
            ReadMsg(buf);
        }

        private async void ReadMsg(byte[] buf)
        {
            while (_stream != null)
            {
                
                DataSize = await _stream.ReadAsync(buf, 0, buf.Length);
                if (DataSize > 0)
                {
                    _dataAvailable = true;
                }
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
        }
    }
}
