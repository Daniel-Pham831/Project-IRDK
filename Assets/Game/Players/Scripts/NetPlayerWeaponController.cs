using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Weapons;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerWeaponController : NetworkBehaviour
    {
        [SerializeField] private NetPlayerInput _input;
        [SerializeField] private Transform weaponHolder;
        
        private List<Weapon> _availableWeapons = new List<Weapon>();
        public ReactiveProperty<Weapon> CurrentWeapon { get; private set; } = new ReactiveProperty<Weapon>(null);

        private void Awake()
        {
            _input.IsFirePressed.Subscribe(async value =>
            {
                if (value)
                    await PerformWeaponAttack();
            }).AddTo(this);

            _input.RawInputVectorReactive.Subscribe(UpdateWeaponRotation).AddTo(this);

            CurrentWeapon.Subscribe(value =>
            {
                foreach (var weapon in _availableWeapons)
                {
                    weapon.gameObject.SetActive(weapon.WeaponId == value.WeaponId);
                }
            }).AddTo(this);
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            base.OnNetworkSpawn();
        }

        private void UpdateWeaponRotation(Vector2 inputDirection)
        {
            if(CurrentWeapon.Value == null)
                return;

            CurrentWeapon.Value.Rotate(inputDirection);
        }
        
        private async UniTask PerformWeaponAttack()
        {
            if (CurrentWeapon.Value == null)
                return;
            
            await CurrentWeapon.Value.Attack();
        }
        
        public void AddWeapon(Weapon weapon)
        {
            if(weapon == null || DoesPlayerHaveWeapon(weapon.WeaponId))
                return;
            
            Transform weaponTransform;
            (weaponTransform = weapon.transform).SetParent(weaponHolder);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            weapon.gameObject.SetActive(false);
            _availableWeapons.Add(weapon);
            
            if(_availableWeapons.Count == 1) // Equip the first weapon in
                EquipWeapon(weapon.WeaponId);
        }
        
        private bool DoesPlayerHaveWeapon(string weaponId)
        {
            return _availableWeapons.FirstOrDefault(weapon => weapon.WeaponId == weaponId) != null;
        }
        
        public void EquipWeapon(string weaponId)
        {
            var weapon = _availableWeapons.FirstOrDefault(w => w.WeaponId == weaponId);
            if (weapon == null)
                return;

            CurrentWeapon.Value = weapon;
        }
    }
}