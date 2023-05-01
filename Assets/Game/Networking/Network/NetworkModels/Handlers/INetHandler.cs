using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.Network.NetworkModels.Models;
using Game.Networking.NormalMessages;
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
            _config = _dataBase.GetConfig<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }

        public ReactiveProperty<T> LocalClientModel { get; set; } = new ReactiveProperty<T>();

        public ReactiveProperty<Dictionary<ulong, ReactiveProperty<T>>> OtherClientReactiveModels { get; set; } =
            new ReactiveProperty<Dictionary<ulong, ReactiveProperty<T>>>();

        public ReactiveProperty<Dictionary<ulong, ReactiveProperty<T>>> AllClientReactiveModels { get; set; } =
            new ReactiveProperty<Dictionary<ulong, ReactiveProperty<T>>>();

        public string HandlerKey => GetType().Name;
        public NetModelHub Hub { get; private set; }

        public async UniTask InitHandler(NetModelHub hub)
        {
            Hub = hub;
            SubscribeReactiveProperty();
            RegisterMessages(true);
            GetOtherHandlers(hub);
        }

        protected virtual void GetOtherHandlers(NetModelHub hub){}

        private void SubscribeReactiveProperty()
        {
            InitAllReactiveData();

            LocalClientModel.Subscribe(value =>
            {
                if (value == null) return;
                
                UpdateToAllClientsModels(value);
                SendModelToAll(value);
            }).AddTo(this);

            OtherClientReactiveModels.Subscribe(v =>
            {
                foreach (var value in v.Values)
                {
                    if (value?.Value == null) continue;
                    
                    UpdateToAllClientsModels(value.Value);
                }
            }).AddTo(this);
        }

        private void InitAllReactiveData()
        {
            OtherClientReactiveModels.Value = new Dictionary<ulong, ReactiveProperty<T>>();
            AllClientReactiveModels.Value = new Dictionary<ulong, ReactiveProperty<T>>();
        }

        private void UpdateToAllClientsModels(T value)
        {
            var clientReactiveModelsDict = AllClientReactiveModels.Value;
            if (!clientReactiveModelsDict.ContainsKey(value.ClientId))
            {
                var reactiveProperty = new ReactiveProperty<T>();
                clientReactiveModelsDict.Add(value.ClientId, reactiveProperty);
            }
            
            clientReactiveModelsDict[value.ClientId].SetValueAndForceNotify(value);
            AllClientReactiveModels.SetValueAndForceNotify(AllClientReactiveModels.Value);
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

            var otherClientReactiveModelDict = OtherClientReactiveModels.Value;
            if (model.ShouldRemove)
            {
                if (otherClientReactiveModelDict.ContainsKey(model.ClientId))
                    otherClientReactiveModelDict.Remove(model.ClientId);
            }
            else
            {
                if (!otherClientReactiveModelDict.ContainsKey(model.ClientId))
                {
                    var reactiveProperty = new ReactiveProperty<T>();
                    otherClientReactiveModelDict.Add(model.ClientId, reactiveProperty);
                }
                
                otherClientReactiveModelDict[model.ClientId].SetValueAndForceNotify(model);
            }
            
            OtherClientReactiveModels.SetValueAndForceNotify(OtherClientReactiveModels.Value);
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
                
                case LocalClientNetworkSpawn:
                    HandleLocalClientNetworkSpawn();
                    break;
            }
        }

        protected virtual void HandleLocalClientNetworkSpawn(){}

        // Only for server
        private void HandleClientConnected(ClientConnectedMessage message)
        {
            //Sync all the clients data to the connected client
            var connectedClientId = message.ClientId;

            var baseNetModels = AllClientReactiveModels.Value.Values.ToList();
            foreach (var clientModel in baseNetModels)
            {
                if (clientModel.Value != null)
                    SendModelToClients(clientModel.Value, new List<ulong>() { connectedClientId });
            }
        }

        // Only for server
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
            ReceiveModel(Helper.Serialize(removeModel));
        }

        private void SendModelToClients(T removeModel,List<ulong> toClientIds)
        {
            Hub.SendModelToClients(HandlerKey, Helper.Serialize(removeModel), Helper.Serialize(toClientIds));
        }

        public void Dispose()
        {
            LocalClientModel?.Dispose();
            OtherClientReactiveModels?.Dispose();
            AllClientReactiveModels?.Dispose();
        }
        
        public T GetModelByPlayerId(string playerId)
        {
            if (LocalClientModel.Value?.PlayerId == playerId) return LocalClientModel.Value;
            
            foreach (var netPlayerModel in AllClientReactiveModels.Value.Values)
            {
                if (netPlayerModel.Value?.PlayerId == playerId)
                    return netPlayerModel.Value;
            }

            return null;
        }
        
        public T GetModelByClientId(ulong clientId)
        {
            if (LocalClientModel.Value?.ClientId == clientId) return LocalClientModel.Value;
            
            foreach (var netPlayerModel in AllClientReactiveModels.Value.Values)
            {
                if (netPlayerModel.Value?.ClientId == clientId)
                    return netPlayerModel.Value;
            }

            return null;
        }

        public ReactiveProperty<T> GetReactiveModelByPlayerId(string playerId)
        {
            if (LocalClientModel.Value?.PlayerId == playerId) return LocalClientModel;
            
            foreach (var netPlayerModel in AllClientReactiveModels.Value.Values)
            {
                if (netPlayerModel.Value?.PlayerId == playerId)
                    return netPlayerModel;
            }

            return null;
        }

        public ReactiveProperty<T> GetReactiveModelByClientId(ulong clientId)
        {
            if (LocalClientModel.Value?.ClientId == clientId) return LocalClientModel;
            
            foreach (var netPlayerModel in AllClientReactiveModels.Value.Values)
            {
                if (netPlayerModel.Value?.ClientId == clientId)
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