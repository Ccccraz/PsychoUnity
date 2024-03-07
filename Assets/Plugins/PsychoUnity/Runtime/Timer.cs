using System;
using PsychoUnity.Manager;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace PsychoUnity
{
    public static class Timer
    {
        public static void Timing(int duration, UnityAction action)
        {
            Timing(duration, 0, 1, action);
        }

        public static void Timing(int duration, int delay, int times, UnityAction action)
        {
            var name = Guid.NewGuid().ToString();
            TimerManager.Instance.Create(name, TimerType.Normal);
            TimerManager.Instance.SetSchedule(name, duration, delay, times, action);
            TimerManager.Instance.Start(name);
        }
    }
}
