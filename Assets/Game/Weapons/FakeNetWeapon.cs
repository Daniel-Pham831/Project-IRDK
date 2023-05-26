using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Game.Players.Scripts;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Resource.DatabaseConfigs.Weapons;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Weapons
{
    public class FakeNetWeapon : NetworkBehaviour
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private WeaponConfig _weaponConfig => _dataBase.GetConfig<WeaponConfig>();

        [SerializeField] private Transform rotator;
        [SerializeField] private SpriteRenderer graphics;
        [SerializeField] private NetPlayerInput _netPlayerInput;

        private NetPlayerModelHandler _netPlayerModelHandler;
        private ReactiveProperty<NetPlayerModel> _ownerPlayerReactiveModel;
        private string _oldWeaponGraphicsId;

        public override async void OnNetworkSpawn()
        {
            if (IsOwner) return;
            
            _netPlayerModelHandler = _netModelHub.GetHandler<NetPlayerModelHandler>();
            _ownerPlayerReactiveModel = await _netPlayerModelHandler.GetReactiveModelByClientId(OwnerClientId);
            if (_ownerPlayerReactiveModel == null) return;

            _ownerPlayerReactiveModel.Subscribe(OnOwnerPlayerModelChanged).AddTo(this);
        }

        private void OnOwnerPlayerModelChanged(NetPlayerModel model)
        {
            if (model == null) return;

            if (model.WeaponGraphicsId != _oldWeaponGraphicsId)
            {
                UpdateWeapon(model.WeaponGraphicsId);
                _oldWeaponGraphicsId = model.WeaponGraphicsId;
            }
        }

        private void Update()
        {
            UpdateWeaponRotation();
        }

        public void UpdateWeaponRotation()
        {
            rotator.Rotate2D(_netPlayerInput.RawInputVector);
        }

        public async UniTask UpdateWeapon(string weaponId)
        {
            var weaponData = _weaponConfig.GetWeaponDataById(weaponId);
            
            graphics.sprite = weaponData?.Info.WeaponSprite;
        }
    }
}