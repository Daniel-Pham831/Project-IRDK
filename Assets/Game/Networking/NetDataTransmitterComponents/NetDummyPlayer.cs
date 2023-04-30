using Game.Networking.NetMessages;
using Maniac.MessengerSystem.Base;
using Maniac.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetDataTransmitterComponents
{
    public class NetDummyPlayer : NetworkBehaviour
    {
        private GameObject _transmitter;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            this.gameObject.name = "NetDummyPlayer - Owner" + (IsServer ? " - Server" : "Client");
            RegisterNetworkEvents(true);
            Messenger.SendMessage(new LocalClientNetworkSpawn());
            NetworkObject.DestroyWithScene = false;
        }

        public override void OnNetworkDespawn()
        {
            Destroy(_transmitter);
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
    }
}