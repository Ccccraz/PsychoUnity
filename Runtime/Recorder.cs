using System.Collections;
using JetBrains.Annotations;
using PsychoUnity.Manager;

namespace PsychoUnity
{
    public static class Recorder
    {
        public static void CreateRecorder(string name, IRecorderData data, [CanBeNull] string custom, string prefix = "Assets/Data")
        {
            RecordManager.Instance.Create(name, data, custom, prefix);
        }

        internal static void CreateRecorderNode(string name, IDictionary data, [CanBeNull] string custom = null, string prefix = "Assets/Data")
        {
            RecordManager.Instance.CreateNode(name, data, custom, prefix);
        }
        
        public static void Write(string name)
        {
            RecordManager.Instance.Write(name);
        }

        public static void Destroy(string name)
        {
            RecordManager.Instance.Destroy(name);
        }
    }
}
