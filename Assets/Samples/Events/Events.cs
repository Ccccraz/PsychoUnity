using Components;
using UnityEngine;

namespace Samples.Events
{
    enum SampleEvents
    {
        UnderAttack
    }

    public class Events : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnderAttack(); 
            }
        }

        private void UnderAttack()
        {
            EventCenter.Instance.EventTrigger(SampleEvents.UnderAttack);
        }
    }
}