using PsychoUnity.Manager;
using UnityEngine;

namespace PsychoUnity.Samples.Timer
{
    public class Timer : MonoBehaviour
    {
        private void Start()
        {
            TimerManager.Instance.Create("001", TimerManager.TimerType.Normal);
            TimerManager.Instance.Create("002", TimerManager.TimerType.Normal);
            TimerManager.Instance.SetSchedule("001", 5000, 10000, 5, Task1);
            // Infinite loop when times == -1
            TimerManager.Instance.SetSchedule("002", 1000, 0, -1, Task3);
            TimerManager.Instance.AddTask("001", Task2);
            TimerManager.Instance.Start("001");
            TimerManager.Instance.Start("002");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TimerManager.Instance.Pause("001");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TimerManager.Instance.Continue("001");
                TimerManager.Instance.Continue("002");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                TimerManager.Instance.Destroy("001");
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                TimerManager.Instance.Pause("002");
            }
        }

        private void Task1()
        {
            print("Task One");
        }

        private void Task2()
        {
            print("Task Two");
        }

        private void Task3()
        {
            print("looping");
        }
    }
}
