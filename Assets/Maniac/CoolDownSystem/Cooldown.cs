using System;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.CoolDownSystem
{
    public class Cooldown
    {
        private CooldownManager cooldownManager => Locator<CooldownManager>.Instance;
        private bool _isOnCooldown;
        private float _totalDuration;
        private float _durationLeft;

        public Cooldown(float totalDuration = 0)
        {
            this._totalDuration = totalDuration;
            EndCooldown();
            cooldownManager.AddToManager(Update);
        }
        
        ~Cooldown()
        {
            cooldownManager.RemoveFromManager(Update);
        }

        public Action OnEndCooldown;

        public bool IsOnCooldown => _isOnCooldown;
        public float TotalDuration => _totalDuration;
        public float DurationLeft => _durationLeft;
        public void StartCooldown() => StartCooldown(_totalDuration);

        private void StartCooldown(float duration)
        {
            _isOnCooldown = true;
            _durationLeft = duration;
        }

        public void EndCooldown()
        {
            _isOnCooldown = false;
            _durationLeft = 0;
            OnEndCooldown?.Invoke();
        }

        public void ChangeDuration(float newDurationValue) =>
            _totalDuration = newDurationValue;

        private void Update()
        {
            if (!_isOnCooldown) return;
            
            if (_durationLeft > 0f)
                _durationLeft = Mathf.Clamp(_durationLeft - Time.deltaTime, 0f, _totalDuration);
            else
                EndCooldown();
        }
    }
}