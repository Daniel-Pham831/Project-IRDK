using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.NetDataTransmitterComponents;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.Network.NetworkModels
{
    public class NetModelHub : MonoBehaviour , IMessageListener
    {
        // private readonly List<INetHandler> _handlers = new List<INetHandler>();
        private readonly Dictionary<string, INetHandler> _handlers = new Dictionary<string, INetHandler>();
        private NetDataTransmitter _netDataTransmitter => Locator<NetDataTransmitter>.Instance;

        private void Awake()
        {
            Messenger.Register<LeaveLobbyMessage>(this);
            Locator<NetModelHub>.Set(this);
        }

        public T GetHandler<T>() where T : INetHandler
        {
            var handlerKey = typeof(T).Name;
            if (_handlers.TryGetValue(handlerKey, out var handler))
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
            _netDataTransmitter.SetHubModel(this);
            AddHandlers();
            await InitAllHandlerAsync();
        }

        private void AddHandlers()
        {
            var handlerTypes = new List<Type>();
            var subTypes = typeof(NetHandler<>).GetAllSubclasses2();
            handlerTypes.AddRange(subTypes);

            foreach (var handlerType in handlerTypes)
            {
                if (gameObject.AddComponent(handlerType) is INetHandler handler)
                    _handlers.Add(handler.HandlerKey, handler);
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
            _netDataTransmitter.SendToServer(new HubModel()
            {
                HandlerKey = handlerKey,
                Data = modelToSendInBytes
            });
        }
        
        public void SendModelToClients(string handlerKey, byte[] modelToSendInBytes, List<ulong> toClientIds = null)
        {
            _netDataTransmitter.SendToServer(
                new HubModel()
                {
                    HandlerKey = handlerKey,
                    Data = modelToSendInBytes,
                    ToClientIds = toClientIds
                }
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