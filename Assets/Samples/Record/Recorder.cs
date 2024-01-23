using System;
using System.Collections;
using Components;
using UnityEngine;

namespace Samples.Record
{
    public class Recorder : MonoBehaviour
    {
        private string[] _titleName;
        void Start()
        {
            _titleName = new[] { "x", "y", "z"};
            RecordCenter.Instance.AddRecorder("001", "002", _titleName);
        }

        private void OnDestroy()
        {
            RecordCenter.Instance.Close("001"); 
        }
    }
    
    public struct SampleDataFormat : IEnumerable
    {
        public int X;
        public int Y;
        public int Z;
        public IEnumerator GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }
    }
}
