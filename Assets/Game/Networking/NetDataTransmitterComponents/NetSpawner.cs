using Game.CloudProfileSystem;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetDataTransmitterComponents
{
    public class NetSpawner : NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        
        private UserProfile _userProfile;
        private NetConfig _netConfig;

        private async void Awake()
        {
            _netConfig = _dataBase.GetConfig<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            Locator<NetSpawner>.Set(this);
        }

        public override void OnNetworkDespawn()
        {
            Locator<NetSpawner>.Remove();
        }

        public void RequestServerToSpawn<T>(Vector3 position)
        {
            FixedString64Bytes spawnObjectTypeName = typeof(T).FullName;
            RequestToSpawnServerRpc(spawnObjectTypeName, position);
        }

        [ServerRpc]
        private void RequestToSpawnServerRpc(FixedString64Bytes spawnTypeName, Vector3 spawnPosition,ServerRpcParams param = default)
        {
            var prefab = _netConfig.GetNetPrefab(spawnTypeName.ToString());

            var spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedObject.NetworkObject.SpawnWithOwnership(param.Receive.SenderClientId);
        }
    }
}