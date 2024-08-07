using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.Events.Scripts
{
    public class Action2 : MonoBehaviour
    {
        void Start()
        {
            EventManager.Instance.AddEventListener("UnderAttack", BloodState);
        }

        private void BloodState()
        {
            print("Blooding...");
        }
    }
}
