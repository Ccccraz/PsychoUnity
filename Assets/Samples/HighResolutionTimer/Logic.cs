using System;
using Components;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Logic : MonoBehaviour
    {
        private void Start()
        {
            TimerCenter.Instance.SetSchedule("001", 2000, 0, 5, TaskOne);
            TimerCenter.Instance.AddTask("001", TaskTwo);
            TimerCenter.Instance.Start("001");
        }
        private static void TaskOne()
        {
            print("TaskOne");
        }

        private static void TaskTwo()
        {
            print("TaskTwo");
        }

        private void OnDestroy()
        {
            TimerCenter.Instance.Stop("001");
        }
    }
}
