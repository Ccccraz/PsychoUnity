using System;
using PsychoUnity.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PsychoUnity.Samples.Recorder
{
    public class Recorder : MonoBehaviour
    {
        private Data _data;

        private void Start()
        {
            _data = new Data();
            RecordManager.Instance.Create("001", _data);
        }

        private void Update()
        {
            _data.ItemOne = Random.Range(0, 10);
            _data.ItemTwo = Random.Range(0, 10);
            _data.ItemThree = Random.Range(0, 10);
            _data.ItemFour = Random.Range(0, 10);
            _data.ItemFive = Vector3.forward;
            
            RecordManager.Instance.Write("001");
        }

        private class Data : IRecorderData
        {
            public int ItemOne;
            public long ItemTwo;
            public float ItemThree;
            public double ItemFour;

            public Vector3 ItemFive;
        }

        private void OnDestroy()
        {
            RecordManager.Instance.Stop("001");
        }
    }
}
