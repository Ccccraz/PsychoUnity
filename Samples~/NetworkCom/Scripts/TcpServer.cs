using System.Text;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.NetworkCom.Scripts
{
    public class Server : MonoBehaviour
    {
        private void Start()
        {
            NetworkComManager.Instance.Create("001", "127.0.0.1", 8888, NetWorkType.TcpServer);
            var _ = NetworkComManager.Instance.InitAsync("001");
            EventManager.Instance.AddEventListener<byte[]>("001", GetData);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var _ = NetworkComManager.Instance.SendMsgAsync("001", "hello\n");
            }
        }

        private void OnDestroy()
        {
            NetworkComManager.Instance.Stop("001");
            NetworkComManager.Instance.Clear();
        }

        private static void GetData(byte[] buffer)
        {
            print(Encoding.UTF8.GetString(buffer));
        }
    }
}
