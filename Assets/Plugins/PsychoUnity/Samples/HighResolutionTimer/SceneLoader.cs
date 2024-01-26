using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.HighResolutionTimer
{
    public class SceneLoader : MonoBehaviour
    {
        public GameObject prefix;
        private void Awake()
        {
            DontDestroyOnLoad(prefix);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Samples/HighResolutionTimer/SubScene");
            }
        }
    }
}
