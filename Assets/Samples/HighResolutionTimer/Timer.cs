using Components;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerCenter.Instance.Create("001", TimerCenter.TimerType.HighResolution);
            TimerCenter.Instance.SetSchedule("001", 60000, 0, 2, TaskOne);
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
