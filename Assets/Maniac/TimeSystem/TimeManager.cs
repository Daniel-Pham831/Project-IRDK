using System;
using System.Collections.Generic;
using Maniac.Utils.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maniac.TimeSystem
{
    public class TimeManager
    {
        public static float DeltaTime { get; private set; }
        public static float FixedDeltaTime { get; private set; }
        public static float Time { get; private set; }
        private float _timeMultiplier = 1f;
        private Queue<Timer> _freeTimers = new Queue<Timer>();
        private List<Timer> _dependentActiveTimers = new List<Timer>();
        private float _previousTimeMultiplier;

        public void Init()
        {
            Time = 0;
            _previousTimeMultiplier = _timeMultiplier = 1f;

            var timeUpdator = new GameObject("Time Updator");
            timeUpdator.AddComponent<TimeUpdator>();
            
            Object.DontDestroyOnLoad(timeUpdator);
        }

        public void OnTimeOut(Action callback, float duration)
        {
            if (duration == 0)
            {
                callback?.Invoke();

                return;
            }

            Timer timer = GetFreeTimer();
            timer.Start(duration, callback);
        }

        public void AddDependentActiveTimer(Timer timer)
        {
            _dependentActiveTimers.Add(timer);
        }

        public void Update(float dependentGamePlayDeltaTime = 0)
        {
            dependentGamePlayDeltaTime *= _timeMultiplier;
            UpdateActiveTimers(dependentGamePlayDeltaTime);
            DeltaTime = dependentGamePlayDeltaTime;
            Time += dependentGamePlayDeltaTime;
        }
        
        public void FixedUpdate(float fixedDeltaTime = 0)
        {
            fixedDeltaTime *= _timeMultiplier;
            FixedDeltaTime = fixedDeltaTime;
        }

        private void UpdateActiveTimers(float dependentGamePlayDeltaTime)
        {
            if (_dependentActiveTimers.Count == 0)
                return;

            for (int i = _dependentActiveTimers.Count - 1; i >= 0; i--)
            {
                Timer currentTimer = _dependentActiveTimers[i];
                currentTimer.Update(dependentGamePlayDeltaTime);

                if (!currentTimer.IsTimerActive)
                {
                    _dependentActiveTimers.Remove(currentTimer);
                    ReturnFreeTimer(currentTimer);
                }
            }
        }

        public void SpeedUp(float multiplier)
        {
            _timeMultiplier = multiplier;
        }

        public Timer GetFreeTimer()
        {
            return _freeTimers.Count == 0 ? new Timer() : _freeTimers.Dequeue();
        }

        public void ReturnFreeTimer(Timer freeTimer)
        {
            freeTimer.DeActiveTimer();
            _freeTimers.Enqueue(freeTimer);
        }

        public void Pause()
        {
            _previousTimeMultiplier = _timeMultiplier;
            _timeMultiplier = 0f;
        }

        public void UnPause()
        {
            _timeMultiplier = _previousTimeMultiplier;
        }

        public void RemoveActiveTimer(Timer timer)
        {
            _dependentActiveTimers.Remove(timer);
        }
    }
}