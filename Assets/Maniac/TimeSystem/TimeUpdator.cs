using System;
using Maniac.CoolDownSystem;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.TimeSystem
{
    public class TimeUpdator:MonoBehaviour
    {
        private TimeManager _timeManager => Locator<TimeManager>.Instance;

        private void Awake()
        {
            this.gameObject.AddComponent<CooldownManager>();
        }

        private void Update()
        {
            _timeManager.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _timeManager.FixedUpdate(Time.fixedDeltaTime);
        }
    }
}