using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Logic : MonoBehaviour
    {
        private void Start()
        {
            TimerManager.Instance.SetSchedule("001", 5000, 0, 5, TaskOne);
            TimerManager.Instance.AddTask("001", TaskTwo);
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
                TimerManager.Instance.ReStart("001");
            }
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
            TimerManager.Instance.Stop("001");
        }
    }
}
