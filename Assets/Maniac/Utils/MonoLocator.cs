using System;
using UnityEngine;

namespace Maniac.Utils
{
    public class MonoLocator<T> : MonoBehaviour where T : MonoBehaviour
    {
        public virtual void Awake()
        {
            Locator<T>.Set(this as T,true);
        }

        public virtual void OnDestroy()
        {
            Locator<T>.Remove(this as T);
        }
    }
}