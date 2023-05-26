using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Game.Weapons;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerWeaponController : NetworkBehaviour
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;

        [SerializeField] private NetPlayerInput _input;
        [SerializeField] private Transform weaponHolder;
        
        private List<Weapon> _availableWeapons = new List<Weapon>();
        public ReactiveProperty<Weapon> CurrentWeapon { get; private set; } = new ReactiveProperty<Weapon>(null);
        
        private NetPlayerModelHandler _netPlayerModelHandler;
        private int _currentWeaponIndex = 0;
        private ReactiveProperty<NetPlayerModel> _localReactivePlayerModel;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _netPlayerModelHandler = _netModelHub.GetHandler<NetPlayerModelHandler>();
                _localReactivePlayerModel = _netPlayerModelHandler.LocalClientModel;
            }
            
            SubscribeEvents();

            base.OnNetworkSpawn();
        }

        private void SubscribeEvents()
        {
            if(IsOwner)
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
                        var isCurrentWeapon = weapon.WeaponId == value.WeaponId;
                        weapon.gameObject.SetActive(isCurrentWeapon);
                    }

                    var finalId = value == null ? "" : value.WeaponId;
                    _localReactivePlayerModel.Value.WeaponGraphicsId = finalId;
                    _localReactivePlayerModel.SelfNotify();
                }).AddTo(this);
            }
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
                EquipWeapon(_currentWeaponIndex);
        }
        
        private bool DoesPlayerHaveWeapon(string weaponId)
        {
            return _availableWeapons.FirstOrDefault(weapon => weapon.WeaponId == weaponId) != null;
        }
        
        public void EquipWeapon(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= _availableWeapons.Count)
                return;

            _currentWeaponIndex = weaponIndex;
            CurrentWeapon.Value = _availableWeapons[weaponIndex];
        }


        public void NextWeapon()
        {
            var nextWeaponIndex = (_currentWeaponIndex + 1) % _availableWeapons.Count;
            EquipWeapon(nextWeaponIndex);
        }

        public void PreviousWeapon()
        {
            var previousWeaponIndex = (_currentWeaponIndex - 1) % _availableWeapons.Count;
            EquipWeapon(previousWeaponIndex);
        }
    }
}