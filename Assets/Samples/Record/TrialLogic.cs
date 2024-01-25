using PsychoUnity.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Record
{
    public class TrialLogic : MonoBehaviour
    {
        private SampleDataFormat _data;
    
        void Start()
        {
            _data = new SampleDataFormat();
            print("reloaded");
        }

        // Update is called once per frame
        void Update()
        {
            _data.X = Random.Range(10, 20);
            _data.Y = Random.Range(10, 20);
            _data.Z = Random.Range(10, 20);
            RecordManager.Instance.Write("001", ref _data);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
