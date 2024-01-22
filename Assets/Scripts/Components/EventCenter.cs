using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Components
{

    /// <summary>
    /// 提供对参数传递的封装
    /// </summary>
    public interface IEventInfo
    {
    }

    /// <summary>
    /// 不进行参数传递的事件
    /// </summary>
    public class EventInfo : IEventInfo
    {
        public UnityAction Action;
    }

    /// <summary>
    /// 传递一个参数的事件
    /// </summary>
    /// <typeparam name="T"> 被传递参数的类型 </typeparam>
    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> Action;
    }

    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventCenter : Singleton<EventCenter>
    {
        /// <summary>
        /// 事件注册表，全局只能有一个事件注册表
        /// </summary>
        private readonly Dictionary<Enum, IEventInfo> _eventDic = new Dictionary<Enum, IEventInfo>();

        private readonly Dictionary<Enum, int> _eventCountDic = new Dictionary<Enum, int>();

        /// <summary>
        /// 添加对目标事件的订阅
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        /// <param name="action"> 订阅事件的委托 </param>
        public void AddEventListener(Enum eventName, UnityAction action)
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
        /// 添加对目标事件的订阅，并传递参数
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        /// <param name="action"> 订阅事件的委托 </param>
        /// <typeparam name="T"> 向委托传递的参数类型 </typeparam>
        public void AddEventListener<T>(Enum eventName, UnityAction<T> action)
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
        /// 移除对目标事件的订阅
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        /// <param name="action"> 订阅目标事件的委托 </param>
        public void RemoveEventListener(Enum eventName, UnityAction action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo eventInfo)
            {
                eventInfo.Action -= action;
            }
        }
        
        /// <summary>
        /// 移除对目标事件的订阅，适用于带有参数传递的事件
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        /// <param name="action"> 订阅目标事件的委托 </param>
        /// <typeparam name="T"> 需要传递的参数类型 </typeparam>
        public void RemoveEventListener<T>(Enum eventName, UnityAction<T> action)
        {
            if (_eventDic.TryGetValue(eventName, out var value) && value is EventInfo<T> eventInfo)
            {
                eventInfo.Action -= action;
            }
        }

        /// <summary>
        /// 调用所有订阅了目标事件的委托
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        public void EventTrigger(Enum eventName)
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
        /// 调用所有订阅了目标事件的委托, 并传递参数
        /// </summary>
        /// <param name="eventName"> 目标事件名 </param>
        /// <param name="param"> 需要传递的参数 </param>
        /// <typeparam name="T"> 需要传递参数的类型 </typeparam>
        public void EventTrigger<T>(Enum eventName, T param)
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

        public bool CheckEvent(Enum eventName)
        {
            return _eventCountDic.ContainsKey(eventName);
        }

        public int GetCount(Enum eventName)
        {
            return _eventCountDic[eventName];
        }

        /// <summary>
        /// 情况当前注册的所有事件
        /// </summary>
        public void Clear()
        {
            _eventDic.Clear();
            _eventCountDic.Clear();
        }
    }
}