using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maniac.CoolDownSystem;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Resource.DatabaseConfigs.Weapons;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game.Weapons
{
    public class Weapon : MonoBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private WeaponSystem _weaponSystem => Locator<WeaponSystem>.Instance;
        private WeaponConfig _weaponConfig => _dataBase.GetConfig<WeaponConfig>();
        
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

        private Cooldown _shootCooldown;
        
        private Cooldown _reloadCooldown;
        public Cooldown ReloadCooldown => _reloadCooldown;
        
        private Vector2 _oldInputDirection;
        private ShowAboveNotificationCommand _outOfTotalAmmoCommand;

        public IntReactiveProperty Ammo { get; private set; } = new IntReactiveProperty(0);
        public IntReactiveProperty TotalAmmo { get; private set; } = new IntReactiveProperty(0);
        
        public bool IsWeaponHasInfinityAmmo => TotalAmmo.Value <= -1;
        public bool IsOutOfTotalAmmo => TotalAmmo.Value == 0;

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
            ResetAmmoAmount();
        }

        private void ResetAmmoAmount()
        {
            Ammo.Value = WeaponData.MagCapacity;
            TotalAmmo.Value = WeaponData.MagCapacity * WeaponData.NumOfMags;
        }

        public void SetWeaponTier(WeaponTier tier)
        {
            Tier.SetValueAndForceNotify(tier);
        }

        protected virtual void SetupWeaponStats()
        {
            if (Stats == null) return;
            
            _shootCooldown ??= new Cooldown();
            _shootCooldown.UpdateTotalDuration(Stats.TimeBetweenShotsInSeconds);
            
            _reloadCooldown ??= new Cooldown();
            _reloadCooldown.UpdateTotalDuration(Stats.ReloadTimeInSeconds);
        }

        public virtual async UniTask Attack()
        {
            if(_shootCooldown.IsOnCooldown.Value || _reloadCooldown.IsOnCooldown.Value) return;
            
            await PerformAttack();
            _shootCooldown.StartCooldown();
        }

        protected virtual async UniTask PerformAttack()
        {
            // Shoot a bullet
            var spawnedBullet = _weaponSystem.GetNewBullet(WeaponId);
            if(spawnedBullet == null) return;

            spawnedBullet.transform.position = firePoint.position;
            var bulletData = Stats.ToBulletData();
            bulletData.Direction = rotator.right;
            await spawnedBullet.Setup(bulletData);

            Ammo.Value--;
            if (Ammo.Value <= 0)
            {
                if(!IsOutOfTotalAmmo)
                     Reload();
                else
                {
                    await ShowOutOfTotalAmmoCommand();
                }
            }
        }

        private async UniTask ShowOutOfTotalAmmoCommand()
        {
            if (_outOfTotalAmmoCommand == null)
            {
                _outOfTotalAmmoCommand = new ShowAboveNotificationCommand(
                    LanguageTable.AboveNoti_OutOfTotalAmmoWarning,
                    () => Debug.Log("Warning Noti Closed"),
                    null,
                    1f
                );
            }

            await _outOfTotalAmmoCommand.Execute();
        }

        private void Reload()
        {
            if (_reloadCooldown.IsOnCooldown.Value) return;

            _reloadCooldown.StartCooldown(() =>
            {
                if (this != null)
                {
                    ReloadAmmo();
                }
            });
        }

        private void ReloadAmmo()
        {
            if (IsWeaponHasInfinityAmmo)
            {
                Ammo.Value = WeaponData.MagCapacity;
            }
            else
            {
                var ammoToReload = Mathf.Min(WeaponData.MagCapacity, TotalAmmo.Value);
                Ammo.Value = ammoToReload;
                TotalAmmo.Value -= ammoToReload;
            }
        }

        public virtual void Rotate(Vector2 inputDirection)
        {
            rotator.Rotate2D(inputDirection);
        }

#if UNITY_EDITOR
        private List<string> GetAllWeaponIds()
        {
            return DataBase.ActiveDatabase.GetConfig<WeaponConfig>().GetAllWeaponIds();
        }
#endif
    }
}