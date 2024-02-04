using System;
using System.Text;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.TCPCom.Scripts
{
    public class Server : MonoBehaviour
    {
        private byte[] _buf;
        void Start()
        {
            _buf = new byte[1024];
            SocketComManager.Instance.SetServer("127.0.0.1", 12345, _buf);
        }

        void Update()
        {
            if (!SocketComManager.Instance.DataAvailable) return;
            print(Encoding.UTF8.GetString(_buf, 0, SocketComManager.Instance.DataSize));
            Array.Clear(_buf, 0, _buf.Length);
        }

        private void OnDestroy()
        {
            SocketComManager.Instance.CloseServer();
        }
    }
}
