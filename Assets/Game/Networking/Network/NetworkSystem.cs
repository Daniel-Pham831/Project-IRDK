using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using Environment = Maniac.DataBaseSystem.Environment;
using Object = UnityEngine.Object;

namespace Game.Networking.Network
{
    public class NetworkSystem
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private BuildSettingConfig _buildSettingConfig;
        private NetConfig _netConfig;
        
        public NetworkManager NetworkManager { get; private set; }
        public UnityTransport UnityTransport { get; private set; }

        public async UniTask Init()
        {
            _buildSettingConfig = _dataBase.GetConfig<BuildSettingConfig>();
            _netConfig = _dataBase.GetConfig<NetConfig>();
            
            var newObj = new GameObject("NetworkSystem");
            Object.DontDestroyOnLoad(newObj);
            NetworkManager = newObj.AddComponent<NetworkManager>();
            UnityTransport = newObj.AddComponent<UnityTransport>();
            NetworkManager.NetworkConfig ??= new NetworkConfig();
            NetworkManager.NetworkConfig.NetworkTransport = UnityTransport;
            await AddAllNetworkPrefabs();
            NetworkManager.NetworkConfig.NetworkTransport.Initialize(NetworkManager);
            
            if (_buildSettingConfig.GetTargetEnvironmentName == Environment.Develop ||
                _buildSettingConfig.GetTargetEnvironmentName == Environment.Staging)
            {
                NetworkManager.LogLevel = LogLevel.Developer;
            }
            else
            {
                NetworkManager.LogLevel = LogLevel.Nothing;
            }
            
            Locator<NetworkManager>.Set(NetworkManager);
        }

        private async UniTask AddAllNetworkPrefabs()
        {
            NetworkManager.NetworkConfig.PlayerPrefab = _netConfig.NetDummyPlayer.gameObject;
            foreach (var prefab in _netConfig.NetworkPrefabs)
            {
                NetworkManager.AddNetworkPrefab(prefab.gameObject);
            }
        }

        public async UniTask SetRelayServerData(RelayServerData relayServerData)
        {
            UnityTransport.SetRelayServerData(relayServerData);
        }
    }
}