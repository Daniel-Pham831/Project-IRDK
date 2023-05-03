using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.NetDataTransmitterComponents;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Game.Networking.Network.NetworkModels.Models;
using Game.Networking.NormalMessages;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.Network.NetworkModels
{
    public struct HubModel : INetworkSerializable
    {
        public ushort HandlerCode;
        public byte[] Data;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HandlerCode);
            serializer.SerializeValue(ref Data);
        }
    }
    
    public class NetModelHub : MonoBehaviour , IMessageListener
    {
        private readonly Dictionary<ushort, INetHandler> _handlers = new Dictionary<ushort, INetHandler>();
        private NetDataTransmitter _netDataTransmitter;

        private void Awake()
        {
            Messenger.Register<LeaveLobbyMessage>(this);
            Locator<NetModelHub>.Set(this);
        }

        public T GetHandler<T>() where T : INetHandler
        {
            var handlerKey = typeof(T).Name;
            var handlerCode = HandlerCode.GetCode(handlerKey);
            if (_handlers.TryGetValue(handlerCode, out var handler))
            {
                return handler is T value ? value : default;
            }

            throw new Exception($"Cannot find {handlerKey}. Please check!");
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

        public void SetNetDataTransmitter(NetDataTransmitter netDataTransmitter)
        {
            _netDataTransmitter = netDataTransmitter;
        }

        private void AddHandlers()
        {
            var handlerTypes = new List<Type>();
            var subTypes = typeof(NetHandler<>).GetAllSubclasses2();
            handlerTypes.AddRange(subTypes);

            foreach (var handlerType in handlerTypes)
            {
                if (gameObject.AddComponent(handlerType) is INetHandler handler)
                    _handlers.Add(HandlerCode.GetCode(handler.HandlerKey), handler);
            }
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
            Debug.Log($"{handlerKey} Send To Server : Size {modelToSendInBytes.Length}");

            _netDataTransmitter.SendNetModelServerRpc(
                new HubModel()
                {
                    HandlerCode = HandlerCode.GetCode(handlerKey),
                    Data = modelToSendInBytes
                }
            );
        }
        
        public void SendModelToClients(string handlerKey, byte[] modelToSendInBytes, byte[] toClientIds)
        {
            Debug.Log($"Send To Server + {gameObject.name}");
            
            _netDataTransmitter.SendNetModelServerRpc(
                new HubModel()
                {
                    HandlerCode = HandlerCode.GetCode(handlerKey),
                    Data = modelToSendInBytes
                }
                ,toClientIds
            );
        }

        public void ReceiveHubModel(HubModel hubModel)
        {
            var key = hubModel.HandlerCode;
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