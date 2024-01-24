using Components;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerCenter.Instance.Create("001", TimerCenter.TimerType.HighResolution, 2);
            TimerCenter.Instance.SetSchedule("001", 2000, 0, 5, TaskOne);
            TimerCenter.Instance.AddTask("001", TaskTwo);
            TimerCenter.Instance.Start("001");
        }

        void TaskOne()
        {
            print("Time over");
        }

        void TaskTwo()
        {
            print("Over");
        }

        private void OnDestroy()
        {
            TimerCenter.Instance.Close("001");
        }
    }
}
