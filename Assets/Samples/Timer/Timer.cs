using Components;
using UnityEngine;

namespace Samples.Timer
{
    public class Timer : MonoBehaviour
    {
        void Start()
        {
            TimerCenter.Instance.Register("001");
            TimerCenter.Instance.Register("001", Task1);
            TimerCenter.Instance.Start("001", 10000);
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
