using Maniac.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Maniac.CoolDownSystem
{
    public class Cooldown
    {
        private CooldownManager cooldownManager => Locator<CooldownManager>.Instance;
        private bool isActive;
        private float duration;
        private float cooldownTimer;

        public Cooldown(float duration)
        {
            this.duration = duration;
            Deactivate();
            cooldownManager.AddToManager(Update);
        }

        public delegate void BecameInactiveEventHandler();
        public event BecameInactiveEventHandler BecameInactive;

        public bool IsActive => isActive;
        public float Duration => duration;
        public float Timer => cooldownTimer;
        public void Activate() => Activate(duration);

        public void Activate(float customDuration)
        {
            isActive = true;
            cooldownTimer = customDuration;
        }

        public void Deactivate()
        {
            isActive = false;
            cooldownTimer = 0;
            OnBecameInactive();
        }

        public void ChangeDuration(float newDurationValue) =>
            duration = newDurationValue;

        private void OnBecameInactive()
        {
            BecameInactive?.Invoke();
        }

        private void Update()
        {
            if (!isActive) return;
            
            if (cooldownTimer > 0f)
                cooldownTimer = Mathf.Clamp(cooldownTimer - Time.deltaTime, 0f, duration);
            else
                Deactivate();
        }
    }
}