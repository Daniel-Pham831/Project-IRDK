using System;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.CoolDownSystem
{
    public class CooldownManager : MonoBehaviour
    {
        private Action cooldownUpdates;

        private void Awake()
        {
            Locator<CooldownManager>.Set(this,true);
        }

        private void OnDestroy()
        {
            Locator<CooldownManager>.Remove(this);
        }

        private void Update() =>
            cooldownUpdates?.Invoke();

        public void AddToManager(Action callback)
        {
            cooldownUpdates += callback;
        }

        public void RemoveFromManager(Action callback)
        {
            cooldownUpdates += callback;
        }
    }
}