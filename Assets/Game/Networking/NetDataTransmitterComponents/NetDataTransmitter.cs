using System.Collections.Generic;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetDataTransmitterComponents
{
    public class NetDataTransmitter : MonoBehaviour, IMessageListener
    {
        protected NetworkManager _networkManager => NetworkManager.Singleton;
        protected NetworkTransport _transport => _networkManager.NetworkConfig.NetworkTransport;
        
        protected NetModelHub _netModelHub;
        
        protected virtual void Awake()
        {
            Locator<NetDataTransmitter>.Set(this,true);
        }

        public void Init()
        {
            RegisterEvents(true);

        }
        
        public void SetHubModel(NetModelHub hubModel)
        {
            _netModelHub = hubModel;
        }

        protected virtual void OnDestroy()
        {
            RegisterEvents(false);
            Locator<NetDataTransmitter>.Remove(this);
        }
        
        protected virtual void RegisterMessages(bool shouldRegister)
        {
            if (shouldRegister)
            {
                Messenger.Register<ClientConnectedMessage>(this);
                Messenger.Register<ClientDisconnectedMessage>(this);
                Messenger.Register<TransportFailureMessage>(this);
                Messenger.Register<LocalClientNetworkSpawn>(this);
            }
            else
            {
                Messenger.Unregister<ClientConnectedMessage>(this);
                Messenger.Unregister<ClientDisconnectedMessage>(this);
                Messenger.Unregister<TransportFailureMessage>(this);
                Messenger.Unregister<LocalClientNetworkSpawn>(this);
            }
        }

        protected virtual void RegisterEvents(bool shouldRegister)
        {
            if (shouldRegister)
            {
                _networkManager.CustomMessagingManager.OnUnnamedMessage += OnUnnamedMessageReceived;
            }
            else
            {
                _networkManager.CustomMessagingManager.OnUnnamedMessage -= OnUnnamedMessageReceived;
            }

            RegisterMessages(shouldRegister);
        }
        
        public void SendToServer(HubModel modelToSend)
        {
            var dataInBytes = modelToSend.ToBytes();
            var writeSize = FastBufferWriter.GetWriteSize(dataInBytes);
            using (var writer = new FastBufferWriter(writeSize, Allocator.Temp))
            {
                if (writer.TryBeginWrite(dataInBytes.Length))
                {
                    writer.WriteValueSafe(dataInBytes);
                    _networkManager.CustomMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId,
                        writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
                else
                {
                    Debug.Log($"Fail {dataInBytes.Length}");
                }
            }
        }

        protected virtual void OnUnnamedMessageReceived(ulong clientId, FastBufferReader reader)
        {
            byte[] data = new byte[reader.Length];
            reader.ReadValueSafe(out data);
            var hubModel = Helper.Deserialize<HubModel>(data);
            if (hubModel == null) return;

            if (_networkManager.IsServer)
            {
                if (hubModel.ToClientIds == null || hubModel.ToClientIds.Count == 0)
                {
                    hubModel.ToClientIds = new List<ulong>(_networkManager.ConnectedClientsIds);
                }

                // update manually for server
                hubModel.ToClientIds.Remove(_networkManager.LocalClientId);

                // Remove the client that sent the data, due to that client already has the data
                hubModel.ToClientIds.Remove(clientId);

                SendDataToClients(data, hubModel.ToClientIds);
            }

            _netModelHub.ReceiveHubModel(hubModel);
        }

        private void SendDataToClients(byte[] dataToSend,IReadOnlyList<ulong> clientIds)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("Only server can send data to clients");
                return;
            }
            
            var writeSize = FastBufferWriter.GetWriteSize(dataToSend);
            using (var writer = new FastBufferWriter(writeSize, Allocator.Temp))
            {
                if (writer.TryBeginWrite(dataToSend.Length))
                {
                    writer.WriteValueSafe(dataToSend);
                    _networkManager.CustomMessagingManager.SendUnnamedMessage(clientIds,
                        writer, NetworkDelivery.ReliableFragmentedSequenced);
                    Debug.Log($"Send data to clients + {clientIds.Count} + {dataToSend.Length}");
                }
                else
                {
                    Debug.Log($"Fail {dataToSend.Length}");
                }
            }
        }

        public void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case ApplicationQuitMessage:
                case TransportFailureMessage:
                    Destroy(this.gameObject);
                    break;
            }
        }
    }
}