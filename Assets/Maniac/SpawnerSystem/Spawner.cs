using System;
using Maniac.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Maniac.SpawnerSystem
{
    public class Spawner<T> where T : Object
    {
        private readonly Object _prefab;
        private readonly ObjectPool<T> _pool;
        public ObjectPool<T> Pool => _pool;
        private int counter = 0;

        public Spawner(T prefab)
        {
            _prefab = prefab;
            _pool = new ObjectPool<T>(CreateFunction, GetFunction, ReleaseFunction, DestroyFunction, false);
        }
        
        private T CreateFunction()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab);
            obj.name = $"{_prefab.name}{SpawnerManager.Indicator}{counter++}";

            return obj as T;
        }

        private void GetFunction(T obj)
        {
            if (obj == null) return;

            switch (obj)
            {
                case GameObject gobj:
                    if(!gobj.activeSelf)
                        gobj.SetActive(true);
                    break;

                case MonoBehaviour mono:
                    if(!mono.gameObject.activeSelf)
                        mono.gameObject.SetActive(true);
                    break;
            }
        }

        private void ReleaseFunction(T obj)
        {
            if (obj == null) return;

            switch (obj)
            {
                case GameObject gobj:
                    if(gobj.activeSelf)
                        gobj.SetActive(false);
                    break;

                case MonoBehaviour mono:
                    if(mono.gameObject.activeSelf)
                        mono.gameObject.SetActive(false);
                    break;
            }
        }

        private void DestroyFunction(T obj)
        {
            if (obj == null) return;

            Object.Destroy(obj);
        }

        public void Reset()
        {
            counter = 0;
            _pool.Clear();
        }
    }
}