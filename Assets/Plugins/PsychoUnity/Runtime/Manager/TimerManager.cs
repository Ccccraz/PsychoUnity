using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using ThreadPriority = System.Threading.ThreadPriority;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// Provides two timers with different resolutions. One of the high-resolution timers will occupy a CPU core,
    /// which consumes more performance. Allows creation of multiple timers.
    /// </summary>
    public class TimerManager : Singleton<TimerManager>
    {
        private readonly Dictionary<string, TimerBase> _timersDic = new Dictionary<string, TimerBase>();

        /// <summary>
        /// Retrieves the frequency of the performance counter, return the current performance-counter frequency,
        /// in counts per second.
        /// </summary>
        public static long Frequency
        {
            get
            {
                TimerHighResolution.QueryPerformanceFrequency(out var value);
                return value;
            }
        }

        /// <summary>
        /// Calling Win32API. Retrieves the current value of the performance counter, which is a high resolution
        /// (less than 1us) time stamp that can be used for time-interval measurements.
        /// </summary>
        /// <returns> the current performance-counter value, in counts </returns>
        public static long GetTimestamp()
        {
            long timestamp = 0;
            TimerHighResolution.QueryPerformanceCounter(ref timestamp);

            return timestamp;
        }

        /// <summary>
        /// Creating a Timer
        /// </summary>
        /// <param name="name"> Timer name </param>
        /// <param name="type"> Type of timer </param>
        /// <param name="core"> If you need to create a high resolution timer,
        /// you need to specify the cpu core on which the timer will run, the default is 4. </param>
        public void Create(string name, TimerType type, int core = 4)
        {
            if (!_timersDic.ContainsKey(name))
            {
                _timersDic[name] = type is TimerType.Normal ? new TimerNormal() : new TimerHighResolution(core);
            }
            else
            {
                Debug.Log($"Timer {name} already exist");
            }
        }

        /// <summary>
        /// Setting up the timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        /// <param name="duration"> time duration </param>
        /// <param name="delay"> The amount of time that needs to be delayed before the first timing starts </param>
        /// <param name="times"> Number of times the timer needs to loop. When the value is -1, the timer will loop indefinitely </param>
        /// <param name="action"> Functions to be executed after the end of timing </param>
        public void SetSchedule(string name, int duration, int delay = 0, int times = 1, UnityAction action = null)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].SetTimer(duration, delay, times, action);
            }
        }

        /// <summary>
        /// Adding tasks to the timer
        /// </summary>
        /// <param name="name"> target recorder name </param>
        /// <param name="action"> Tasks to be added </param>
        public void AddTask(string name, UnityAction action)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].AddTask(action);
            }
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        public void Start(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Start();
            }
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        public void Stop(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Stop();
            }
        }

        /// <summary>
        /// Pause the timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        public void Pause(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Pause();
            }
        }

        /// <summary>
        /// CContinue the paused timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        public void Continue(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Continue();
            }
        }

        /// <summary>
        /// Destroy the timer
        /// </summary>
        /// <param name="name"> target timer name </param>
        public void Destroy(string name)
        {
            if (VerifyTimer(name))
            {
                _timersDic[name].Destroy();
            }
        }

        private bool VerifyTimer(string name)
        {
            if (_timersDic.ContainsKey(name)) return true;
            Debug.Log($"Timer {name} does not exist");
            return false;

        }

        /// <summary>
        /// Timer type: If a high-precision timer is required, select the HighResolution type
        /// </summary>
        public enum TimerType
        {
            Normal,
            HighResolution
        }
    }

    internal abstract class TimerBase
    {
        internal abstract void SetTimer(int duration, int delay, int times, UnityAction action);

        internal abstract void Start();

        internal abstract void AddTask(UnityAction action);

        internal abstract void Pause();

        internal abstract void Continue();

        internal abstract void Stop();

        internal abstract void Destroy();

    }

    internal class TimerNormal : TimerBase
    {
        // Timer basic fields
        private int _duration;
        private int _delay;
        private int _times;
        private UnityAction _action;
        
        // Timer basic status
        private bool _isTiming;
        private bool _isPause;

        // Pause method related fields
        private Task _taskHandler;
        private CancellationTokenSource _cts;

        private long _startTime; // Delay or timing start time
        private long _pauseTime; // Pause time
        
        private long _remainTime; // Remaining delay after pause is triggered during delay
        private long _remainDelayTime; // Remaining delay after pause is triggered during timing
        
        private bool _hasDelay; // If delay is required, record the delay status
        private int _count; // Records the current timing times

        internal override void SetTimer(int duration, int delay, int times, UnityAction action)
        {
            _duration = duration;
            _delay = delay;
            _times = times;
            _action = action;
        }

        internal override void Start()
        {
            if (_isTiming) return;
            
            _cts = new CancellationTokenSource();
            
            _remainTime = _duration;
            _remainDelayTime = _delay;
            _count = 0;
            
            _taskHandler = Timing(_cts.Token);
            _isTiming = true;
        }

        internal override void AddTask(UnityAction action)
        {
            _action += action;
        }

        internal override void Stop()
        {
            Destroy();
        }

        internal override void Pause()
        {
            if (!_isTiming) return; // Check whether is still timing
            
            if (_isPause) return; // Check whether is has been pause
            
            _cts.Cancel();
            _isPause = true;

            // Records the status at pause
            _pauseTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            
            if (_delay > 0 && !_hasDelay)
            {
                _remainDelayTime -= (_pauseTime - _startTime);
            }
            else
            {
                _remainDelayTime = 0;
                _remainTime -= ( _pauseTime - _startTime);
            }
            
            Debug.Log($"{_remainTime}, {_remainDelayTime}, {_pauseTime}, {_startTime}");
        }

        internal override void Continue()
        {
            if (!_isTiming) return;

            if (!_isPause) return;
            
            _cts = new CancellationTokenSource();
            _taskHandler = Timing(_cts.Token);
            _isPause = false;

        }

        internal override void Destroy(){
            _cts.Cancel();
            _taskHandler.Dispose();
        }
        
        private async Task Timing(CancellationToken cancellationToken)
        {
            if (_remainDelayTime > 0)
            {
                _startTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                
                // Detecting pause signal
                try
                {
                    await Task.Delay((int)_remainDelayTime, cancellationToken);
                }
                catch (Exception exception)
                {
                    if (exception is TaskCanceledException)
                    {
                        Debug.Log($"Delaying start time: {_startTime}, pause time: {new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}");
                        return;
                    }
                }
                
                _hasDelay = true;
            }

            // Timing loop, if times == -1 then infinite loop
            while (_count < _times || _times == -1)
            {
                _startTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                
                // Detecting pause signal
                try
                {
                    await Task.Delay((int)_remainTime, cancellationToken);
                }
                catch (Exception exception)
                {
                    if (exception is TaskCanceledException)
                    {
                        Debug.Log($"Timing start time: {_startTime}, pause time: {new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}");
                        return;
                    }
                }
                
                _action.Invoke();
                _count++;
                _remainTime = _duration;
            }

            _isTiming = false;
            _hasDelay = false;
        }
    }

    internal class TimerHighResolution : TimerBase
    {
        private long _duration; // 
        private int _delay;
        private int _times; // Times
        private UnityAction _action;
        
        private readonly Thread _timingThread;
        private bool _keepThread;
        private bool _startTiming;
        private bool _pauseTiming;

        private long _startPoint;
        private long _endPoint;

        private readonly IntPtr _core;

        internal TimerHighResolution(int core)
        {
            _timingThread = new Thread(Timing)
            {
                Priority = ThreadPriority.Highest
            };
            _keepThread = true;
            _core = (IntPtr)core;
            _timingThread.Start();
        }

        internal override void SetTimer(int duration, int delay, int times, UnityAction action)
        {
            QueryPerformanceFrequency(out var frequency);
            _duration = frequency / 1000 * duration;
            _delay = delay;
            _times = times;
            _action = action;
        }

        internal override void AddTask(UnityAction action)
        {
            _action += action;
        }

        internal override void Start()
        {
            _startTiming = true;
        }

        //TODO
        internal override void Pause()
        {
            _pauseTiming = true;
        }

        //TODO
        internal override void Continue()
        {
            _pauseTiming = false;
        }

        internal override void Stop()
        {
            _startTiming = false;
        }

        internal override void Destroy()
        {
            _startTiming = false;
            _keepThread = false;
            _timingThread.Join();
        }

        private void Timing()
        {
            SetCPUCore();
            
            var count = 0;
            
            while (_keepThread)
            {
                WaitForTiming();
                
                Delay();
                
                MainTiming(ref count);
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
                if (_keepThread == false)
                {
                    break;
                }
            }
        }

        private void Delay()
        {
            if (_delay <= 0) return;
            QueryPerformanceCounter(ref _startPoint);
            while (_startTiming)
            {
                QueryPerformanceCounter(ref _endPoint);
                if ((_endPoint - _startPoint) > _delay)
                {
                    break;
                }
            }
        }
        private void MainTiming(ref int count)
        {
            var duration = _duration;
            var remainTime = _duration;
            QueryPerformanceCounter(ref _startPoint);
            while (count < _times && _startTiming)
            {
                while (_pauseTiming)
                {
                    duration = remainTime;
                    QueryPerformanceCounter(ref _startPoint);
                }
                remainTime = duration - (_endPoint - _startPoint);
                QueryPerformanceCounter(ref _endPoint);
                if (remainTime > 0) continue;
                _action.Invoke();
                count++;
                QueryPerformanceCounter(ref _startPoint);
            }
        }
        
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();
    }
}
