using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Components
{
    public class TimerCenter : Singleton<TimerCenter>
    {
        private readonly Dictionary<string, TimerBase> _timersDic = new Dictionary<string, TimerBase>();
        private readonly HashSet<string> _timerName = new HashSet<string>();

        public void Create(string name, TimerType type, int core = 4)
        {
            if (!_timerName.Add(name))
            {
                Debug.Log($"Timer {name} already exist");
                return;
            }

            _timersDic[name] = type is TimerType.Normal ? new TimerNormal() : new TimerHighResolution(core);
        }

        public void SetSchedule(string name, int duration, int delay = 0, int times = 1, UnityAction action = null)
        {
            if (!_timerName.Contains(name))
            {
                Debug.Log($"Timer {name} does not exist.");
                return;
            }

            if (_timersDic.TryGetValue(name, out var timer))
            {
                timer.SetTimer(duration, delay, times, action);
            }
            else
            {
                Debug.Log($"Timer {name} was not found in the dic");
            }
        }

        public void AddTask(string name, UnityAction action)
        {
            if (!_timerName.Contains(name))
            {
                Debug.Log($"Timer {name} does not exist.");
                return;
            }

            if (_timersDic.TryGetValue(name, out var timer))
            {
                timer.AddTask(action);
            }
            else
            {
                Debug.Log($"Timer {name} was not found in the dic");
            }
            
        }

        public void Start(string name)
        {
            if (!_timerName.Contains(name))
            {
                Debug.Log($"Timer {name} does not exist.");
                return;
            }

            if (_timersDic.TryGetValue(name, out var timer))
            {
                timer.Start();
            }
            else
            {
                Debug.Log($"Timer {name} was not found in the dic");
            }
        }

        public void Close(string name)
        {
            if (!_timerName.Contains(name))
            {
                return;
            }

            if (_timersDic.TryGetValue(name, out var timer))
            {
                timer.Destroy();
                _timerName.Remove(name);
                _timersDic.Remove(name);
            }
        }

        public enum TimerType
        {
            Normal,
            HighResolution
        }
    }

    public abstract class TimerBase
    {
        public abstract void SetTimer(int duration, int delay, int times, UnityAction action);

        public abstract void Start();

        public abstract void AddTask(UnityAction action);

        public abstract void Destroy();

    }

    public class TimerNormal : TimerBase
    {
        private int _duration;
        private int _delay;
        private int _times;
        private UnityAction _action;

        public override void SetTimer(int duration, int delay, int times, UnityAction action)
        {
            _duration = duration;
            _delay = delay;
            _times = times;
            _action = action;
        }

        public override void Start()
        {
            Timing();
        }

        public override void AddTask(UnityAction action)
        {
            _action += action;
        }

        public override void Destroy() {
            
        }

        private async void Timing()
        {
            if (_delay > 0)
            {
                await Task.Delay(_delay);
            }

            var count = 0;
            while (count < _times)
            {
                await Task.Delay(_duration);
                _action.Invoke();

                count++;
            }
        }
    }
    public class TimerHighResolution : TimerBase
    {
        private long _duration; // 
        private int _delay;
        private int _times; // Times
        private UnityAction _action;
        
        private readonly Thread _timingThread;
        private bool _keepThread;
        private bool _startTiming;

        private readonly IntPtr _core;
        
        public TimerHighResolution(int core)
        {
            _timingThread = new Thread(Timing)
            {
                Priority = ThreadPriority.Highest
            };
            _keepThread = true;
            _core = (IntPtr)core;
            _timingThread.Start();
        }

        public override void SetTimer(int duration, int delay, int times, UnityAction action)
        {
            QueryPerformanceFrequency(out var frequency);
            _duration = frequency / 1000 * duration;
            _delay = delay;
            _times = times;
            _action = action;
        }
        
        public override void AddTask(UnityAction action)
        {
            _action += action;
        }

        public override void Start()
        {
            _startTiming = true;
        }

        public override void Destroy()
        {
            _startTiming = false;
            _keepThread = false;
            _timingThread.Join();
        }

        
        private void Timing()
        {
            var threadHandle = GetCurrentThread();
            var affinityMask = _core;
            
            SetThreadAffinityMask(threadHandle, affinityMask);
            
            var count = 0;
            long startPoint = 0;
            long endPoint = 0;
            
            while (_keepThread)
            {

                while (!_startTiming)
                {
                    
                }

                if (_delay > 0)
                {
                    QueryPerformanceCounter(ref startPoint);
                    while (true)
                    {
                        QueryPerformanceCounter(ref endPoint);
                        if ((endPoint - startPoint) > _delay)
                        {
                            break;
                        }
                    }
                }

                while (count < _times)
                {
                    QueryPerformanceCounter(ref startPoint);
                    while (true)
                    {
                        QueryPerformanceCounter(ref endPoint);
                        if ((endPoint - startPoint) > _duration)
                        {
                            _action.Invoke();
                            count++;
                            break;
                        }
                    }
                }
            }
        }
        
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();
    }
}
