using Components;
using UnityEngine;

namespace Samples.Timer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerCenter.Instance.Create("001", TimerCenter.TimerType.HighResolution);
            TimerCenter.Instance.SetSchedule("001", 5000, 0, 1, Task1);
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
