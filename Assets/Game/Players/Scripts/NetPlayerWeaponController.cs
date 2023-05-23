using System;
using System.Collections.Generic;
using Game.Weapons;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerWeaponController : NetworkBehaviour
    {
        [SerializeField] private NetPlayerInput _input;
        
        private List<Weapon> _availableWeapons = new List<Weapon>();
        public ReactiveProperty<Weapon> CurrentWeapon { get; private set; } = new ReactiveProperty<Weapon>(null);

        private void Awake()
        {
            _input.IsFirePressed.Subscribe(value =>
            {
                if (value)
                    PerformWeaponAttack();
            }).AddTo(this);

            _input.RawInputVectorReactive.Subscribe(UpdateWeaponRotation).AddTo(this);
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            base.OnNetworkSpawn();

            GetCurrentWeapon();
        }

        private void GetCurrentWeapon()
        {
            var weapons = GetComponentsInChildren<Weapon>();
            foreach (var weapon in weapons)
            {
                _availableWeapons.Add(weapon);
            }

            if (_availableWeapons.Count > 0)
                CurrentWeapon.Value = _availableWeapons[0];
        }

        private void UpdateWeaponRotation(Vector2 inputDirection)
        {
            if(CurrentWeapon.Value == null)
                return;

            CurrentWeapon.Value.Rotate(inputDirection);
        }
        
        private void PerformWeaponAttack()
        {
            if (CurrentWeapon.Value == null)
                return;
            
            CurrentWeapon.Value.Attack();
        }
    }
}