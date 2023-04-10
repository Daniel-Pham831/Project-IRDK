using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.Lobby;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Game.Networking.NetPlayerComponents
{
    public class NetPlayer : NetworkBehaviour
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
            _config = _dataBase.Get<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            this.gameObject.name = "NetPlayer - Owner" + (IsServer ? " - Server" : "Client");
            _hub.SetNetPlayer(this);
            RegisterNetworkEvents(true);
            Locator<NetPlayer>.Set(this);
            Messenger.SendMessage(new LocalClientNetworkSpawn());
            base.OnNetworkSpawn();
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
        }

        public override void OnNetworkDespawn()
        {
            if(IsOwner)
            {
                Locator<NetPlayer>.Remove();
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

        [ServerRpc]
        public void SendNetModelServerRpc(HubModel hubModelToSend, byte[] sendToClientIds = null,
            ServerRpcParams param = default)
        {
            Debug.Log($"Received On Server + {gameObject.name}");
            
            List<ulong> toClientIdsNetworkList = new List<ulong>();
            if (sendToClientIds != null)
                toClientIdsNetworkList = Helper.Deserialize<List<ulong>>(sendToClientIds);
            
            if (toClientIdsNetworkList.Count != 0)
            {
                // send to specific clients
                SendNetModelClientRpc(
                    hubModelToSend,
                    param.Receive.SenderClientId,
                    new ClientRpcParams()
                    {
                        Send = new ClientRpcSendParams()
                        {
                            TargetClientIds = toClientIdsNetworkList
                        }
                    }
                );
            }
            else
            {
                // send to all clients
                SendNetModelClientRpc(
                    hubModelToSend,
                    param.Receive.SenderClientId
                );
            }
        }

        [ClientRpc]
        private void SendNetModelClientRpc(HubModel hubModelReceived,ulong senderClientId,ClientRpcParams param = default)
        {
            if (senderClientId == NetworkManager.Singleton.LocalClientId) return;
            
            _hub.ReceiveHubModel(hubModelReceived);
        }
    }
}