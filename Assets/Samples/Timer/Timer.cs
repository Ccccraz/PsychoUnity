using Components;
using UnityEngine;

namespace Samples.Timer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerCenter.Instance.Create("001", TimerCenter.TimerType.Normal);
            TimerCenter.Instance.SetSchedule("001", 5000, 0, 5, Task1);
            TimerCenter.Instance.AddTask("001", Task2);
            TimerCenter.Instance.Start("001");
        }

        private void Task1()
        {
            print("OK");
        }

        private void Task2()
        {
            print("OK again");
        }
    }
}
