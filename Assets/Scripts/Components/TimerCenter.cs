using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class TimerCenter : Singleton<TimerCenter>
    {
        private readonly Dictionary<string, UnityAction> _timerDic = new Dictionary<string, UnityAction>();
        private readonly Dictionary<string, double> _timerInfoDic = new Dictionary<string, double>();

        public void Register(string name, UnityAction action, double duration)
        {
            if (_timerDic.TryGetValue(name, out var value))
            {
                value += action;
            }
            else
            {
                _timerDic.Add(name, action);
                _timerInfoDic.Add(name, duration);
            }
        }

        public void Start(string name)
        {
            if (_timerInfoDic.TryGetValue(name, out var value))
            {
               Timing(value); 
               _timerDic[name]?.Invoke();
            }
            else
            {
                Debug.Log("不存在的计时器");
            }
        }

        private static void Timing(double duration)
        {
            
        }
    }
}
