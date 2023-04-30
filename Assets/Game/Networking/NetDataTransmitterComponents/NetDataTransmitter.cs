using System.Collections.Generic;
using Game.CloudProfileSystem;
using Game.Networking.Lobby;
using Game.Networking.NetMessages;
using Game.Networking.Network;
using Game.Networking.Network.NetworkModels;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.TimeSystem;
using Maniac.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using UnityEngine;

namespace Game.Networking.NetDataTransmitterComponents
{
    public class TransportDataTransmitter : MonoBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private NetModelHub _hub => Locator<NetModelHub>.Instance;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private UnityTransport _transport => _networkSystem.UnityTransport;

        private UserProfile _userProfile;
        private NetConfig _config;

        private async void Awake()
        {
            _config = _dataBase.GetConfig<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }

        // public void SendToServer(HubModel hubModelToSend, List<ulong> toClientIds = null)
        // {
        //     _networkSystem.
        // }
    }
    
    
    public class NetDataTransmitter : NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private NetModelHub _hub => Locator<NetModelHub>.Instance;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        private UserProfile _userProfile;
        private NetConfig _config;

        private async void Awake()
        {
            _config = _dataBase.GetConfig<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            this.gameObject.name = "NetDataTransmitter - Owner" + (IsServer ? " - Server" : "Client");
            _hub.SetNetDataTransmitter(this);
            RegisterNetworkEvents(true);
            Locator<NetDataTransmitter>.Set(this);
            Messenger.SendMessage(new LocalClientNetworkSpawn());
            NetworkObject.DestroyWithScene = false;
        }
        
        private void OnClientConnectedCallback(ulong clientId)
        {   
            Messenger.SendMessage(new ClientConnectedMessage()
            {
                ClientId = clientId
            });
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            Messenger.SendMessage(new ClientDisconnectedMessage()
            {
                ClientId = clientId
            });
        }

        private async void OnTransportFailure()
        {
            Messenger.SendMessage(new TransportFailureMessage());
            Destroy(gameObject);
        }

        public override void OnNetworkDespawn()
        {
            if(IsOwner)
            {
                Locator<NetDataTransmitter>.Remove();
            }

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

        // [ServerRpc]
        // public void SendNetModelServerRpc(HubModel hubModelToSend, byte[] sendToClientIds = null,
        //     ServerRpcParams param = default)
        // {
        //     List<ulong> toClientIdsNetworkList = new List<ulong>();
        //     if (sendToClientIds != null)
        //         toClientIdsNetworkList = Helper.Deserialize<List<ulong>>(sendToClientIds);
        //     
        //     if (toClientIdsNetworkList.Count != 0)
        //     {
        //         // send to specific clients
        //         SendNetModelClientRpc(
        //             hubModelToSend,
        //             param.Receive.SenderClientId,
        //             new ClientRpcParams()
        //             {
        //                 Send = new ClientRpcSendParams()
        //                 {
        //                     TargetClientIds = toClientIdsNetworkList
        //                 }
        //             }
        //         );
        //     }
        //     else
        //     {
        //         // send to all clients
        //         SendNetModelClientRpc(
        //             hubModelToSend,
        //             param.Receive.SenderClientId
        //         );
        //     }
        // }
        //
        // [ClientRpc]
        // private void SendNetModelClientRpc(HubModel hubModelReceived,ulong senderClientId,ClientRpcParams param = default)
        // {
        //     if (senderClientId == NetworkManager.Singleton.LocalClientId) return;
        //     
        //     _hub.ReceiveHubModel(hubModelReceived);
        // }
    }
}