using System.Text;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.NetworkCom.Scripts
{
    public class TcpServerManually : MonoBehaviour
    {
        private void Start()
        {
            NetworkComManager.Instance.Create("001", "127.0.0.1", 8888, NetWorkType.TcpServer, WorkMode.Manual);
            var _ = NetworkComManager.Instance.InitAsync("001");

            TimerManager.Instance.Create("T001", TimerManager.TimerType.Normal);
            TimerManager.Instance.SetSchedule("T001", 200, 0, -1, ReadData);
            TimerManager.Instance.Start("T001");
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
            TimerManager.Instance.Stop("T001");
            TimerManager.Instance.Destroy("T001");
            NetworkComManager.Instance.Stop("001");
            NetworkComManager.Instance.Clear();
        }

        private static async void ReadData()
        {
            if (!NetworkComManager.Instance.CheckConnect("001")) return;
            var buffer = new byte[1024];
            await NetworkComManager.Instance.ReadAsync("001", buffer);
            print(Encoding.UTF8.GetString(buffer));
        }
    }
}