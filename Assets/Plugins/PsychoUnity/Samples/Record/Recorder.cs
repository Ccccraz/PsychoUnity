using System;
using System.Collections;
using PsychoUnity.Manager;
using UnityEngine;

namespace Samples.Record
{
    public class Recorder : MonoBehaviour
    {
        private string[] _titleName;
        void Start()
        {
            _titleName = new[] { "x", "y", "z"};
            RecordManager.Instance.AddRecorder("001", "002", _titleName);
        }

        private void OnDestroy()
        {
            RecordManager.Instance.Close("001"); 
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
