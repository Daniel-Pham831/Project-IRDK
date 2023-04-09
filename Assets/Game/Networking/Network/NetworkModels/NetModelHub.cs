using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.NetMessages;
using Game.Networking.NetPlayerComponents;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.Network.NetworkModels
{
    public struct HubModel : INetworkSerializable
    {
        public FixedString64Bytes HandlerKey;
        public byte[] Data;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HandlerKey);
            serializer.SerializeValue(ref Data);
        }
    }
    
    public class NetModelHub : MonoBehaviour , IMessageListener
    {
        // private readonly List<INetHandler> _handlers = new List<INetHandler>();
        private readonly Dictionary<string, INetHandler> _handlers = new Dictionary<string, INetHandler>();
        private NetPlayer _netPlayer;

        private void Awake()
        {
            Messenger.Register<LeaveLobbyMessage>(this);
            Locator<NetModelHub>.Set(this);
        }

        private void OnDestroy()
        {
            Messenger.Unregister<LeaveLobbyMessage>(this);
            Locator<NetModelHub>.Remove();
        }

        public async UniTask Init()
        {
            AddHandlers();
            await InitAllHandlerAsync();
        }

        public void SetNetPlayer(NetPlayer netPlayer)
        {
            _netPlayer = netPlayer;
        }

        private void AddHandlers()
        {
            var netPlayerModelHandler = gameObject.AddComponent<NetPlayerModelHandler>();
            
            _handlers.Add(netPlayerModelHandler.HandlerKey,netPlayerModelHandler);
        }

        private async UniTask InitAllHandlerAsync()
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach (var handler in _handlers.Values)
            {
                tasks.Add(handler.InitHandler(this));
            }

            await UniTask.WhenAll(tasks);
        }

        public void SendModelToAll(string handlerKey, byte[] modelToSendInBytes)
        {
            _netPlayer.SendNetModelServerRpc(
                new HubModel()
                {
                    HandlerKey = handlerKey,
                    Data = modelToSendInBytes
                }
            );
        }
        
        public void SendModelToClients(string handlerKey, byte[] modelToSendInBytes, byte[] toClientIds)
        {
            _netPlayer.SendNetModelServerRpc(
                new HubModel()
                {
                    HandlerKey = handlerKey,
                    Data = modelToSendInBytes
                }
                ,toClientIds
            );
        }

        public void ReceiveHubModel(HubModel hubModel)
        {
            var key = hubModel.HandlerKey.ToString();
            if (_handlers.ContainsKey(key))
            {
                _handlers[key].ReceiveModel(hubModel.Data);
            }
        }

        public void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case LeaveLobbyMessage:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}