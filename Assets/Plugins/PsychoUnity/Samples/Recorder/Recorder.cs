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
            _data = new Data
            {
                Result = true
            };
            
            // Create your Recorder
            // Name your Recorder and pass in the data you need to record
            RecordManager.Instance.Create("001", _data);
            
            // You can customize where the Data is stored. The default is Assets/Data/
            RecordManager.Instance.Create("002", _data, "Example", "D:/Example");
        }

        private void Update()
        {
            _data.Behavior = Random.Range(0, 10);
            _data.Count = Random.Range(0, 10);
            _data.Timestamp = Random.Range(0, 10);
            _data.Status = Random.Range(0, 10);
            _data.Position = Random.insideUnitSphere;
            _data.MousePosition = Random.insideUnitCircle;
            _data.Result = !_data.Result;
            
            // In this example, the manual mode Recorder is shown,
            // and you need to call the Write() method whenever you need to record
            RecordManager.Instance.Write("001");
            RecordManager.Instance.Write("002");
        }

        private void OnDestroy()
        {
            RecordManager.Instance.Destroy("001");
        }

        // Define the data to be recorded here
        // You must inherit IRecorderData
        // Supported data types: int, float, bool and other common value types, Unity Vector3 and Unity Vector2
        private class Data : IRecorderData
        {
            public int Behavior;
            public long Count;
            public float Timestamp;
            public double Status;

            public Vector3 Position;
            public Vector2 MousePosition;

            public bool Result;
        }
        
    }
}
