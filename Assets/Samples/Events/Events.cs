using Components;
using UnityEngine;

namespace Samples.Events
{
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
            EventCenter.Instance.EventTrigger("UnderAttack");
        }
    }
}