using System.Text;
using Components;
using UnityEngine;

namespace Samples.TCPCom
{
    public class Server : MonoBehaviour
    {
        void Start()
        {
            SocketComCenter.Instance.SetServer("localhost", 12345, 1024);
        }

        void Update()
        {
            if (SocketComCenter.Instance.StreamAvailable)
            {
               print(Encoding.UTF8.GetString(SocketComCenter.Instance.Data, 0, SocketComCenter.Instance.DataSize)); 
            }
        }

        private void OnDisable()
        {
            SocketComCenter.Instance.CloseServer();
        }
    }
}
