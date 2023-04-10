using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Game.Networking.Network.NetworkModels.Handlers
{
    public interface INetHandler
    {
        public NetModelHub Hub { get; }
        public string HandlerKey { get; }
        public UniTask InitHandler(NetModelHub hub);
        public void ReceiveModel(byte[] modelReceiveInBytes);
    }

    public abstract class NetHandler<T> : MonoBehaviour, INetHandler, IDisposable, IMessageListener
        where T : BaseNetModel, new()
    {
        protected DataBase _dataBase => Locator<DataBase>.Instance;
        protected CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;

        protected UserProfile _userProfile;
        protected NetConfig _config;

        protected virtual async void Awake()
        {
            _config = _dataBase.Get<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }

        public ReactiveProperty<T> LocalClientModel { get; set; } = new ReactiveProperty<T>();

        public ReactiveProperty<Dictionary<ulong, T>> OtherClientModels { get; set; } =
            new ReactiveProperty<Dictionary<ulong, T>>();

        public ReactiveProperty<Dictionary<ulong, T>> AllClientModels { get; set; } =
            new ReactiveProperty<Dictionary<ulong, T>>();

        public string HandlerKey => GetType().Name;
        public NetModelHub Hub { get; private set; }

        public async UniTask InitHandler(NetModelHub hub)
        {
            Hub = hub;
            SubscribeReactiveProperty();
            RegisterMessages(true);
        }

        private void SubscribeReactiveProperty()
        {
            OtherClientModels.Value = new Dictionary<ulong, T>();
            AllClientModels.Value = new Dictionary<ulong, T>();
            
            LocalClientModel.Subscribe(value =>
            {
                if (value == null) return;
                
                UpdateToAllClientsModels(value);
                SendModelToAll(value);
            }).AddTo(this);

            OtherClientModels.Subscribe(v =>
            {
                foreach (var value in v.Values)
                {
                    if (value == null) continue;
                    
                    UpdateToAllClientsModels(value);
                }
            }).AddTo(this);
        }

        private void UpdateToAllClientsModels(T value)
        {
            var clientModelsValue = AllClientModels.Value;
            if (!clientModelsValue.ContainsKey(value.ClientId))
                clientModelsValue.Add(value.ClientId, value);

            clientModelsValue[value.ClientId] = value;
            
            AllClientModels.SetValueAndForceNotify(AllClientModels.Value);
        }

        protected virtual void RegisterMessages(bool shouldRegister)
        {
            if (shouldRegister)
            {
                Messenger.Register<ClientConnectedMessage>(this);
                Messenger.Register<ClientDisconnectedMessage>(this);
                Messenger.Register<TransportFailureMessage>(this);
            }
            else
            {
                Messenger.Unregister<ClientConnectedMessage>(this);
                Messenger.Unregister<ClientDisconnectedMessage>(this);
                Messenger.Unregister<TransportFailureMessage>(this);
            }
        }

        private void OnDestroy()
        {
            RegisterMessages(false);
        }

        private void SendModelToAll(T modelToSend)
        {
            Hub.SendModelToAll(HandlerKey, Helper.Serialize(modelToSend));
        }

        public virtual void ReceiveModel(byte[] modelReceiveInBytes)
        {
            var model = Helper.Deserialize<T>(modelReceiveInBytes);
            if (model == null) return;

            var baseNetModelsDict = OtherClientModels.Value;
            if (model.ShouldRemove)
            {
                if (baseNetModelsDict.ContainsKey(model.ClientId))
                    baseNetModelsDict.Remove(model.ClientId);
            }
            else
            {
                if (!baseNetModelsDict.ContainsKey(model.ClientId))
                    baseNetModelsDict.Add(model.ClientId, model);

                baseNetModelsDict[model.ClientId] = model;
            }
            
            OtherClientModels.SetValueAndForceNotify(OtherClientModels.Value);
        }

        public virtual void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case ClientConnectedMessage message:
                    HandleClientConnected(message);
                    break;
                
                case ClientDisconnectedMessage message:
                    HandleClientDisconnected(message);
                    break;
            }
        }

        private void HandleClientConnected(ClientConnectedMessage message)
        {
            var connectedClientId = message.ClientId;

            var baseNetModels = AllClientModels.Value.Values.ToList();
            foreach (var clientModel in baseNetModels)
            {
                SendModelToClients(clientModel, new List<ulong>() { connectedClientId });
            }
        }

        // This method will be called on server-side
        // We need to sync data to all the other connected clients
        private void HandleClientDisconnected(ClientDisconnectedMessage receivedMessage)
        {
            var removeModel = new T()
            {
                ClientId = receivedMessage.ClientId,
                ShouldRemove = true,
            };

            SendModelToAll(removeModel);
        }

        private void SendModelToClients(T removeModel,List<ulong> toClientIds)
        {
            Hub.SendModelToClients(HandlerKey, Helper.Serialize(removeModel), Helper.Serialize(toClientIds));
        }

        public void Dispose()
        {
            LocalClientModel?.Dispose();
            OtherClientModels?.Dispose();
            AllClientModels?.Dispose();
        }
        
        public T GetModelByPlayerId(string playerId)
        {
            foreach (var netPlayerModel in AllClientModels.Value.Values)
            {
                if (netPlayerModel.PlayerId == playerId)
                    return netPlayerModel;
            }

            return null;
        }
        
        public T GetModelByClientId(ulong clientId)
        {
            foreach (var netPlayerModel in AllClientModels.Value.Values)
            {
                if (netPlayerModel.ClientId == clientId)
                    return netPlayerModel;
            }

            return null;
        }

        public T CreateNewLocalModel()
        {
            return new T()
            {
                ClientId = NetworkManager.Singleton.LocalClientId,
                PlayerId = AuthenticationService.Instance.PlayerId,
                ShouldRemove = false,
            };
        }
    }
}