using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// TODO SocketCOMManager => NetComManger
    /// Managing Network Communications
    /// <remarks> Will be deprecated in future releases </remarks>
    /// </summary>
    [Obsolete("Will be deprecated in future releases")]
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

        /// <summary>
        /// Create and start tcp server
        /// </summary>
        /// <param name="hostname"> hostname </param>
        /// <param name="port"> port </param>
        /// <param name="buf"> data buf </param>
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

        /// <summary>
        /// Asynchronous sending of string messages
        /// </summary>
        /// <param name="msg"> message </param>
        public void SendStringAsync(string msg)
        {
            SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        /// <summary>
        /// Asynchronous sending messages
        /// </summary>
        /// <param name="msg"> message </param>
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

        /// <summary>
        /// Close and destroy tcp server
        /// </summary>
        public void CloseServer()
        {
            _server.Stop();
        }
    }
}
