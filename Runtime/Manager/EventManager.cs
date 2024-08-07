using System.Collections.Generic;
using UnityEngine.Events;

namespace PsychoUnity.Manager
{

    /// <summary>
    /// 提供对参数传递的封装
    /// </summary>
    internal interface IEventInfo
    {
    }

    /// <summary>
    /// 不进行参数传递的事件
    /// </summary>
    internal class EventInfo : IEventInfo
    {
        public UnityAction Action;
    }

    /// <summary>
    /// 传递一个参数的事件
    /// </summary>
    /// <typeparam name="T"> 被传递参数的类型 </typeparam>
    internal class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> Action;
    }

    /// <summary>
    /// Global Event Manager
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 事件注册表，全局只能有一个事件注册表
        /// </summary>
        private readonly Dictionary<string, IEventInfo> _eventDic = new Dictionary<string, IEventInfo>();

        private readonly Dictionary<string, int> _eventCountDic = new Dictionary<string, int>();

        /// <summary>
        /// Adding a subscription to a specific event
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <param name="action"> Functions to be executed when an event is triggered </param>
        public void AddEventListener(string eventName, UnityAction action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo eventInfo)
            {
                eventInfo.Action += action;
            }
            else
            {
                _eventDic.Add(eventName, new EventInfo() { Action = action });
                _eventCountDic.Add(eventName, 0);
            }
        }

        /// <summary>
        /// Adding a subscription to a specific event and passing parameters to the subscriber.
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <param name="action"> Functions to be executed when an event is triggered </param>
        /// <typeparam name="T"> Types of parameters that need to be passed to the subscription function </typeparam>
        public void AddEventListener<T>(string eventName, UnityAction<T> action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo<T> eventInfo)
            {
                eventInfo.Action += action;
            }
            else
            {
                _eventDic.Add(eventName, new EventInfo<T>() { Action = action });
                _eventCountDic.Add(eventName, 0);
            }
        }
        
        /// <summary>
        /// Remove subscription to target event
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <param name="action"> Functions that need to remove subscriptions to target events </param>
        public void RemoveEventListener(string eventName, UnityAction action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo eventInfo)
            {
                eventInfo.Action -= action;
            }
        }
        
        /// <summary>
        /// Remove subscription to target event for function with parameter passing
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <param name="action"> Functions that need to remove subscriptions to target events </param>
        /// <typeparam name="T"> Types of parameters that need to be passed to the subscription function </typeparam>
        public void RemoveEventListener<T>(string eventName, UnityAction<T> action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo<T> eventInfo)
            {
                eventInfo.Action -= action;
            }
        }

        /// <summary>
        /// Trigger event
        /// </summary>
        /// <param name="eventName"> event name </param>
        public void EventTrigger(string eventName)
        {
            if (_eventDic.TryGetValue(eventName, out var action) && action is EventInfo eventInfo)
            {
                eventInfo.Action?.Invoke();
            }

            if (_eventCountDic.ContainsKey(eventName))
            {
                _eventCountDic[eventName]++;
            }
        }
        
        /// <summary>
        /// Trigger the event and pass the parameters
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <param name="param"> Parameters passed to subscribers </param>
        /// <typeparam name="T"> The type of parameter passed to the subscribers </typeparam>
        public void EventTrigger<T>(string eventName, T param)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo<T> eventInfo)
            {
                eventInfo.Action?.Invoke(param);
            }
            
            if (_eventCountDic.ContainsKey(eventName))
            {
                _eventCountDic[eventName]++;
            }
        }

        /// <summary>
        /// Check if there are subscribers to an event
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <returns> Returns true if it exists, false otherwise </returns>
        public bool CheckEvent(string eventName)
        {
            return _eventCountDic.ContainsKey(eventName);
        }

        /// <summary>
        /// Get the number of times the target event was triggered
        /// </summary>
        /// <param name="eventName"> event name </param>
        /// <returns> Number of times the target event was triggered </returns>
        public int GetCount(string eventName)
        {
            return _eventCountDic[eventName];
        }

        /// <summary>
        /// Clear all events
        /// </summary>
        public void Clear()
        {
            _eventDic.Clear();
            _eventCountDic.Clear();
        }
    }
}