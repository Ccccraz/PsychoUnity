using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Record
{
    public class Controller : MonoBehaviour
    {
        public GameObject prefix;
        private void Awake()
        {
            DontDestroyOnLoad(prefix);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Samples/Record/Trial");
            }
        }
    }
}
