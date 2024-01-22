using System;
using System.Text;
using Components;
using UnityEngine;

namespace Samples.TCPCom
{
    public class Server : MonoBehaviour
    {
        private byte[] _buf;
        void Start()
        {
            _buf = new byte[1024];
            SocketComCenter.Instance.SetServer("127.0.0.1", 12345, _buf);
        }

        void Update()
        {
            if (!SocketComCenter.Instance.DataAvailable) return;
            print(Encoding.UTF8.GetString(_buf, 0, SocketComCenter.Instance.DataSize));
            Array.Clear(_buf, 0, _buf.Length);
        }

        private void OnDestroy()
        {
            SocketComCenter.Instance.CloseServer();
        }
    }
}
