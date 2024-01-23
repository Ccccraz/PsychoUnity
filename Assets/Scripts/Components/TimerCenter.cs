using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class TimerCenter : Singleton<TimerCenter>
    {
        private readonly Dictionary<string, UnityAction> _timerDic = new Dictionary<string, UnityAction>();
        private readonly Dictionary<string, UnityAction> _highResolutionTimerDic = new Dictionary<string, UnityAction>();

        public void Register(string timerName)
        {
            if (_timerDic.ContainsKey(timerName))
            {
                Debug.Log("Already exist");
            }
            else
            {
                _timerDic.Add(timerName, null);
            }
        }

        public void Register(string timerName, UnityAction action)
        {
            if (_timerDic.ContainsKey(timerName))
            {
                _timerDic[timerName] += action;
            }
            else
            {
                _timerDic.Add(timerName, action);
            }
        }

        public void RegisterHighResolution()
        {
            
        }

        public void Start(string name, int duration)
        {
            if (_timerDic.TryGetValue(name, out var value))
            {
                Timing(duration, value);
            }
        }

        public void StartHighResolution()
        {
            
        }

        private static async void Timing(int duration, UnityAction action)
        {
            await Task.Delay(duration);
            action.Invoke();
        }

        public static void HighResolutionTiming(object obj)
        {
            var para = (TimerPara)obj;
            var count = 0;
            long startPoint = 0;
            long endPoint = 0;

            QueryPerformanceCounter(ref startPoint);

            while (true)
            {
                QueryPerformanceCounter(ref endPoint);
                if ((endPoint - startPoint) > para.Duration)
                {
                    para.Task.Invoke();
                    count++;
                    if (count >= para.Count)
                    {
                       return; 
                    }
                }
            }
        }
        
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private struct TimerPara
        {
            public float Duration;
            public int Count;
            public UnityAction Task;
        }
    }
}
