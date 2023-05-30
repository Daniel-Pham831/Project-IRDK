using System;
using System.Collections.Generic;
using Maniac.TimeSystem;
using Maniac.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maniac.SpawnerSystem
{
    public class SpawnerManager
    {
        public static readonly string Indicator = " [Clone]-";

        private TimeManager _timeManager => Locator<TimeManager>.Instance;

        private Dictionary<string, Spawner<Object>> _spawners;
        private Dictionary<string, Timer> _timerForReleaseAfterMonos = new Dictionary<string, Timer>();

        public void Initialize()
        {
            _spawners = new Dictionary<string, Spawner<Object>>();
        }

        public void ResetAllSpawner()
        {
            foreach (var spawner in _spawners.Values)
            {
                spawner.Reset();
            }

            ResetAllReleaseAfters();
        }

        private void ResetAllReleaseAfters()
        {
            foreach (var timer in _timerForReleaseAfterMonos.Values)
            {
                timer.DeActiveTimer();
            }
            _timerForReleaseAfterMonos.Clear();
        }

        public T Get<T>(T prefab) where T : Object
        {
            if (prefab == null)
            {
                throw new Exception("Null Exception! You can not spawn null object");
            }

            var result = GetHelper(prefab) as T;
            ClearReleaseAfter(result);
            return result;
        }

        private Object GetHelper(Object prefab)
        {
            if (prefab == null)
                return null;
            
            var key = prefab.name;
            if (!_spawners.ContainsKey(key))
            {
                var newSpawner = new Spawner<Object>(prefab);
                _spawners.Add(key,newSpawner);
            }

            return _spawners[key].Pool.Get();
        }

        public void Release<T>(T objectToRelease) where T : Object
        {
            ReleaseHelper(objectToRelease);
        }

        private void ReleaseHelper(Object objToRelease)
        {
            if (objToRelease == null) return;
            CheckReleaseAfter(objToRelease.name); // Make sure to clear release after on this mono
            
            var key = objToRelease.name.Substring(0,objToRelease.name.IndexOf(SpawnerManager.Indicator, StringComparison.Ordinal));

            if (!_spawners.ContainsKey(key))
            {
                Debug.LogError($"There is no {key} spawner. Please investigate!");
                Object.Destroy(objToRelease);
                return;
            }

            _spawners[key].Pool.Release(objToRelease);
        }

        public void ReleaseAfter<T>(T objToRelease, float durationInSeconds) where T : Object
        {
            ReleaseAfterHelper(objToRelease, durationInSeconds);
        }
        
        private void ClearReleaseAfter<T>(T objectToClear) where T : Object
        {
            CheckReleaseAfter(objectToClear.name);
        }

        private void ReleaseAfterHelper(Object monoToRelease, float duration)
        {
            var key = monoToRelease.name;
            var timer = _timeManager.GetFreeTimer();
            if (_timerForReleaseAfterMonos.ContainsKey(key))
            {
                _timerForReleaseAfterMonos[key].DeActiveTimer();
                _timerForReleaseAfterMonos.Remove(key);
            }
            _timerForReleaseAfterMonos.Add(key,timer);
            
            timer.Start(duration, () =>
            {
                CheckReleaseAfter(key);
                
                Release(monoToRelease);
            });
        }
        
        private void CheckReleaseAfter(string key)
        {
            if (!_timerForReleaseAfterMonos.ContainsKey(key)) return;
            
            var timer = _timerForReleaseAfterMonos[key];
            timer.DeActiveTimer();
            _timerForReleaseAfterMonos.Remove(key,out var freeTimer);
            _timeManager.ReturnFreeTimer(freeTimer);
        }
    }
}