using System;
using Components;
using UnityEngine;

namespace Samples.Events
{

    public class Action1 : MonoBehaviour
    {
        private void Start()
        {
            EventCenter.Instance.AddEventListener("UnderAttack", ReduceHp);
        }

        private void ReduceHp()
        {
           print("HP - 1"); 
        }
    }
}