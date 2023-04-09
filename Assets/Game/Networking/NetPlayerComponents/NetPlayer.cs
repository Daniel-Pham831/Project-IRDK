using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.Lobby;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetPlayerComponents
{
    public class NetPlayer : NetworkBehaviour ,IDisposable
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        private UserProfile _userProfile;
        private NetConfig _config;

        public NetworkList<NetPlayerModel> NetPlayerModels;

        private async void Awake()
        {
            _config = _dataBase.Get<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            NetPlayerModels = new NetworkList<NetPlayerModel>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            SendNetPlayerModelToServerRpc(
                new NetPlayerModel()
                {
                    ClientId = NetworkManager.Singleton.LocalClientId,
                    Name = _userProfile.DisplayName
                }
            );

            RegisterNetworkEvents(true);
            Locator<NetPlayer>.Set(this);
            base.OnNetworkSpawn();
        }
        
        private void OnClientConnectedCallback(ulong clientId)
        {
          // do nothing
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = 0; i < NetPlayerModels.Count; i++)
            {
                if(NetPlayerModels[i].ClientId == clientId)
                {
                    NetPlayerModels.RemoveAt(i);
                    break;
                }
            }
        }

        private async void OnTransportFailure()
        {
            await _lobbySystem.LeaveLobby();
        }

        [ServerRpc]
        private void SendNetPlayerModelToServerRpc(NetPlayerModel netPlayerModel,ServerRpcParams param = default)
        {
            if (!NetPlayerModels.Contains(netPlayerModel))
            {
                NetPlayerModels.Add(netPlayerModel);
            }
        }

        public override void OnNetworkDespawn()
        {
            if(IsOwner)
                Locator<NetPlayer>.Remove();

            RegisterNetworkEvents(false);
            
            base.OnNetworkDespawn();
        }

        private void RegisterNetworkEvents(bool shouldRegister)
        {
            if (shouldRegister)
            {
                if (IsServer)
                {
                    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                    NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                }

                if (IsOwner)
                {
                    NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
                }
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
            }
        }

        public void Dispose()
        {
            NetPlayerModels?.Dispose();
        }
    }
}