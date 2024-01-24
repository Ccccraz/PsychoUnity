using Components;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Timer : MonoBehaviour
    {
        private void Awake()
        {
            TimerCenter.Instance.Create("001", TimerCenter.TimerType.HighResolution);
        }

        private void OnDestroy()
        {
            TimerCenter.Instance.Destroy("001");
        }
    }
}
