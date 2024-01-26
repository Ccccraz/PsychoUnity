using System;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.Timer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerManager.Instance.Create("001", TimerManager.TimerType.Normal);
            TimerManager.Instance.SetSchedule("001", 5000, 0, 5, Task1);
            TimerManager.Instance.AddTask("001", Task2);
            TimerManager.Instance.Start("001");
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
            }
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
