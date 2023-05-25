using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.CoolDownSystem;
using Maniac.DataBaseSystem;
using Resource.DatabaseConfigs.Weapons;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public string WeaponId => weaponId;
        
        [ValueDropdown("GetAllWeaponIds")]
        [SerializeField] private string weaponId;
        [SerializeField] private Transform rotator;
        [SerializeField] private SpriteRenderer graphics;
        [SerializeField] private Transform firePoint;

        public WeaponData WeaponData { get; private set; } = null;
        public ReactiveProperty<WeaponTier> Tier { get; private set; } =
            new ReactiveProperty<WeaponTier>(WeaponTier.Standard);
        public WeaponStats Stats { get; private set; } = null;

        private Cooldown _attackCoolDown;
        private Vector2 _oldInputDirection;
        
        public IntReactiveProperty Ammo { get; private set; } = new IntReactiveProperty(0);
        public IntReactiveProperty TotalAmmo { get; private set; } = new IntReactiveProperty(0);
        
        private void Awake()
        {
            Tier.Subscribe(tier =>
            {
                if (WeaponData == null) return;

                Stats = WeaponData.GetStatsByTier(tier);
                SetupWeaponStats();
            }).AddTo(this);
        }

        public void SetWeaponData(WeaponData weaponData)
        {
            WeaponData = weaponData;
            Tier.SetValueAndForceNotify(WeaponTier.Standard);
        }
        
        public void SetWeaponTier(WeaponTier tier)
        {
            Tier.SetValueAndForceNotify(tier);
        }

        protected virtual void SetupWeaponStats()
        {
            if (Stats == null) return;
            
            _attackCoolDown ??= new Cooldown();
            _attackCoolDown.ChangeDuration(Stats.TimeBetweenShots);
        }

        public virtual async UniTask Attack()
        {
            if(_attackCoolDown.IsOnCooldown) return;
            
            await PerformAttack();
            _attackCoolDown.StartCooldown();
        }

        protected virtual async UniTask PerformAttack()
        {
            
        }
        
        public virtual void Rotate(Vector2 inputDirection)
        {
            if (inputDirection == Vector2.zero)
                return;
            
            var angle = Vector2.SignedAngle(rotator.right, inputDirection);
            rotator.Rotate(Vector3.forward,angle);
            
            var localScale = rotator.localScale;
            localScale = new Vector3( localScale.x,inputDirection.x > 0 ? 1 : inputDirection.x < 0 ? -1 : rotator.localScale.y, localScale.z);
            rotator.localScale = localScale;
        }

#if UNITY_EDITOR
        private List<string> GetAllWeaponIds()
        {
            return DataBase.ActiveDatabase.GetConfig<WeaponConfig>().GetAllWeaponIds();
        }
#endif
    }
}