using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.HighResolutionTimer
{
    public class Timer : MonoBehaviour
    {
        private void Awake()
        {
            TimerManager.Instance.Create("001", TimerManager.TimerType.HighResolution);
        }

        private void OnDestroy()
        {
            TimerManager.Instance.Destroy("001");
        }
    }
}
