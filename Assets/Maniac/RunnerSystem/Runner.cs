using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maniac.RunnerSystem
{
    public class CoroutineRunner : MonoBehaviour
    {
        
    }

    public static class Runner 
    {
        private static CoroutineRunner _coroutineRunner;

        public static void InitRunner()
        {
            GameObject go = new GameObject("Runner");
            _coroutineRunner = go.AddComponent<CoroutineRunner>();
            GameObject.DontDestroyOnLoad(_coroutineRunner);
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return _coroutineRunner.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                _coroutineRunner.StopCoroutine(coroutine);
            }
        }
    }
}