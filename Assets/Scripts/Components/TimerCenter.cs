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
            if (VerifyTimer(name))
            {
                _timersDic[name].SetTimer(duration, delay, times, action);
            }
        }

        public void AddTask(string name, UnityAction action)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].AddTask(action);
            }
        }

        public void Start(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Start();
            }
        }

        public void Stop(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Stop();
            }
        }

        public void Pause(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Pause();
            }
        }

        public void ReStart(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Restart();
            }
        }

        public void Destroy(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Destroy();
            }
        }

        private bool VerifyTimer(string name)
        {
            if (!_timerName.Contains(name))
            {
                Debug.Log($"Timer {name} does not exist.");
                return false;
            }

            if (!_timersDic.ContainsKey(name))
            {
                Debug.Log($"Timer {name} was not found in the dic");
                return false;
            }

            return true;
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

        public abstract void Pause();

        public abstract void Restart();

        public abstract void Stop();

        public abstract void Destroy();

    }

    public class TimerNormal : TimerBase
    {
        private int _duration;
        private int _delay;
        private int _times;
        private UnityAction _action;

        private Task _taskHandler;

        public override void SetTimer(int duration, int delay, int times, UnityAction action)
        {
            _duration = duration;
            _delay = delay;
            _times = times;
            _action = action;
        }

        public override void Start()
        {
            _taskHandler = Timing();
        }

        public override void AddTask(UnityAction action)
        {
            _action += action;
        }

        public override void Stop()
        {
            _taskHandler.Dispose();
        }

        public override void Pause()
        {
            _taskHandler.Wait();
        }

        public override void Restart()
        {
            _taskHandler.Start();
        }

        public override void Destroy(){
            _taskHandler.Dispose();
        }

        private async Task Timing()
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

        //TODO
        public override void Pause()
        {
        }

        //TODO
        public override void Restart()
        {
        }

        public override void Stop()
        {
            _startTiming = false;
            _duration = 0;
            _delay = 0;
            _times = 0;
            _action = null;
        }

        public override void Destroy()
        {
            _startTiming = false;
            _keepThread = false;
            _timingThread.Join();
        }

        private void Timing()
        {
            SetCPUCore();
            
            var count = 0;
            long startPoint = 0;
            long endPoint = 0;
            
            while (_keepThread)
            {
                WaitForTiming();
                
                Delay(ref startPoint, ref endPoint);
                
                MainTiming(ref startPoint, ref endPoint, ref count);
            }
        }

        private void SetCPUCore()
        {
            var threadHandle = GetCurrentThread();
            SetThreadAffinityMask(threadHandle, _core);
        }

        private void WaitForTiming()
        {
            while (!_startTiming)
            {
                
            }
        }

        private void Delay(ref long startPoint, ref long endPoint)
        {
            if (_delay <= 0) return;
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

        //TODO Use event optimize pause
        private void MainTiming(ref long startPoint, ref long endPoint, ref int count)
        {
            while (count < _times && _startTiming)
            {
                QueryPerformanceCounter(ref startPoint);
                while (_startTiming)
                {
                    QueryPerformanceCounter(ref endPoint);
                    if ((endPoint - startPoint) <= _duration) continue;
                    Debug.Log($"endPoint: {endPoint}, startPoint: {startPoint}, duration: {_duration}");
                    _action.Invoke();
                    count++;
                    break;
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
