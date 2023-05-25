using System;
using Cysharp.Threading.Tasks;
using Maniac.Utils;
using UniRx;
using UnityEngine;

namespace Maniac.CoolDownSystem
{
    public class Cooldown : IDisposable
    {
        private CooldownManager cooldownManager => Locator<CooldownManager>.Instance;
        
        public Action OnEndCooldown;
        public BoolReactiveProperty IsOnCooldown = new BoolReactiveProperty(false);
        public FloatReactiveProperty TotalDuration = new FloatReactiveProperty(0);
        public FloatReactiveProperty DurationLeft = new FloatReactiveProperty(0);
        public FloatReactiveProperty DurationLeftPercent = new FloatReactiveProperty(0);
        
        public Cooldown(float totalDuration = 0)
        {
            TotalDuration.Value = totalDuration;
            EndCooldown();
            DurationLeft.Subscribe(value =>
            {
                DurationLeftPercent.Value = TotalDuration.Value != 0 ? value / TotalDuration.Value : 0;
            });
            cooldownManager.AddToManager(Update);
        }
        
        ~Cooldown()
        {
            Dispose(true);
        }

        public Cooldown StartCooldown(Action onEndCooldownCallback = null)
        {
            if(onEndCooldownCallback != null)
                OnEndCooldown = onEndCooldownCallback;
            
            return StartCooldown(TotalDuration.Value);
        }

        private Cooldown StartCooldown(float duration)
        {
            IsOnCooldown.Value = true;
            DurationLeft.Value = duration;
            return this;
        }

        public Cooldown EndCooldown()
        {
            IsOnCooldown.Value = false;
            DurationLeft.Value = 0;
            OnEndCooldown?.Invoke();
            return this;
        }

        public Cooldown ChangeDuration(float newDurationValue)
        { 
            TotalDuration.Value = newDurationValue;
            return this;
        }

        private void Update()
        {
            if (!IsOnCooldown.Value) return;
            
            if (DurationLeft.Value > 0f)
                DurationLeft.Value = Mathf.Clamp(DurationLeft.Value - Time.deltaTime, 0f, TotalDuration.Value);
            else
                EndCooldown();
        }

        private void ReleaseUnmanagedResources()
        {
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                IsOnCooldown?.Dispose();
                TotalDuration?.Dispose();
                DurationLeft?.Dispose();
                DurationLeftPercent?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}