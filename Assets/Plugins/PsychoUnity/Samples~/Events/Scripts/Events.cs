using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.Events.Scripts
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
            EventManager.Instance.EventTrigger("UnderAttack");
        }
    }
}