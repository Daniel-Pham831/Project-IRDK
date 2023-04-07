using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

namespace Game.Networking.Network
{
    public class NetworkSystem : NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private BuildSettingConfig _buildSettingConfig;
        
        public NetworkManager NetworkManager { get; private set; }
        public UnityTransport UnityTransport { get; private set; }

        public async UniTask Init()
        {
            _buildSettingConfig = _dataBase.Get<BuildSettingConfig>();
            
            var newObj = new GameObject("NetworkSystem");
            Object.DontDestroyOnLoad(newObj);
            NetworkManager = newObj.AddComponent<NetworkManager>();
            UnityTransport = newObj.AddComponent<UnityTransport>();
            NetworkManager.NetworkConfig ??= new NetworkConfig();
            NetworkManager.NetworkConfig.NetworkTransport = UnityTransport;
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
        }

        public async UniTask SetRelayServerData(RelayServerData relayServerData)
        {
            UnityTransport.SetRelayServerData(relayServerData);
        }
    }
}