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
        private Dictionary<string, Spawner<Object>> _spawners = new Dictionary<string, Spawner<Object>>();
        private Dictionary<int, Timer> _timerForReleaseAfterMonos = new Dictionary<int, Timer>();

        public void Initialize()
        {
            Locator<SpawnerManager>.Set(this,true);
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

            var result = GetHelper(prefab);
            return result;
        }

        private T GetHelper<T>(T prefab) where T : Object
        {
            if (prefab == null)
                return null;
            
            var key = GetKeyFromObject(prefab);
            if (!_spawners.ContainsKey(key))
            {
                var newSpawner = new Spawner<Object>(prefab);
                _spawners.Add(key,newSpawner);
            }

            var obj = _spawners[key].Pool.Get();
            if (obj.GetType() != typeof(T) && obj is GameObject go)
            {
                return go.GetComponent<T>();
            }
            
            return obj as T;
        }

        public void Release<T>(T objectToRelease) where T : Object
        {
            if(objectToRelease is GameObject go)
                ReleaseHelper(go);
            else if (objectToRelease is MonoBehaviour mono)
                ReleaseHelper(mono.gameObject);
            else
                Debug.LogError($"You can not release {objectToRelease.name} because it is not a GameObject or MonoBehaviour");
        }

        private void ReleaseHelper(Object objToRelease)
        {
            if (objToRelease == null) return;
            
            var key = GetKeyFromObject(objToRelease);
            CheckReleaseAfter(objToRelease); // Make sure to clear release after on this mono

            if (!_spawners.ContainsKey(key))
            {
                Debug.LogError($"There is no {key} spawner. Please investigate!");
                Object.Destroy(objToRelease);
                return;
            }

            _spawners[key].Pool.Release(objToRelease);
        }

        private string GetKeyFromObject(Object obj)
        {
            if (obj == null) return "";
            
            var indexOfIndicator = obj.name.IndexOf(SpawnerManager.Indicator, StringComparison.Ordinal);
            
            if (indexOfIndicator == -1) return obj.name;
            
            return obj.name.Substring(0, obj.name.IndexOf(SpawnerManager.Indicator, StringComparison.Ordinal));
        }

        public void ReleaseAfter<T>(T objToRelease, float durationInSeconds) where T : Object
        {
            if(objToRelease == null) return;
            
            if(objToRelease is GameObject go)
                ReleaseAfterHelper(go, durationInSeconds);
            else if (objToRelease is MonoBehaviour mono)
                ReleaseAfterHelper(mono.gameObject, durationInSeconds);
            else
                Debug.LogError(
                    $"You can not release after {objToRelease.name} because it is not a GameObject or MonoBehaviour");
        }
        
        private void ReleaseAfterHelper(GameObject monoToRelease, float duration)
        {
            var objectId = monoToRelease.GetInstanceID();
            if (_timerForReleaseAfterMonos.ContainsKey(objectId))
            {
                var timerToRemove = _timerForReleaseAfterMonos[objectId];
                timerToRemove.DeActiveTimer();
                _timerForReleaseAfterMonos.Remove(objectId);
            }
            
            var timer = _timeManager.GetFreeTimer();
            _timerForReleaseAfterMonos.Add(objectId,timer);
            timer.Start(duration, () =>
            {
                Release(monoToRelease);
            });
        }
        
        private void CheckReleaseAfter(Object obj)
        {
            var objectId = obj.GetInstanceID();
            if (!_timerForReleaseAfterMonos.ContainsKey(objectId)) return;
            
            var timer = _timerForReleaseAfterMonos[objectId];
            timer.DeActiveTimer();
            _timerForReleaseAfterMonos.Remove(objectId,out var freeTimer);
        }
    }
}