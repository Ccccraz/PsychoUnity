using System;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.Events
{

    public class Action1 : MonoBehaviour
    {
        private void Start()
        {
            EventManager.Instance.AddEventListener("UnderAttack", ReduceHp);
        }

        private void ReduceHp()
        {
           print("HP - 1"); 
        }
    }
}