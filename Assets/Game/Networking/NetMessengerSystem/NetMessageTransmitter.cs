using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.Network;
using Game.Networking.NormalMessages;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Game.Networking.NetMessengerSystem
{
    public class NetMessageTransmitter : IMessageListener
    {
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private NetworkManager _networkManager => _networkSystem.NetworkManager;
        private NetworkTransport _transport => _networkManager.NetworkConfig.NetworkTransport;
        private Dictionary<Type, List<INetMessageListener>> messagesMap =
            new Dictionary<Type, List<INetMessageListener>>();
        private Dictionary<FixedString32Bytes,Type> _messageTypes = new Dictionary<FixedString32Bytes, Type>();

        public async UniTask Init()
        {
            RegisterEvents(true);
            Locator<NetMessageTransmitter>.Set(this, true);

            await UniTask.CompletedTask;
        }

        private void Reset()
        {
            RegisterEvents(false);
            Locator<NetMessageTransmitter>.Remove(this);
        }

        private void RegisterEvents(bool shouldRegister)
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

        private void RegisterMessages(bool shouldRegister)
        {
            if (shouldRegister)
            {
                Messenger.Register<ApplicationQuitMessage>(this);
                Messenger.Register<TransportFailureMessage>(this);
            }
            else
            {
                Messenger.Unregister<ApplicationQuitMessage>(this);
                Messenger.Unregister<TransportFailureMessage>(this);
            }
        }
        
        public void SendNetMessage<T>(T messageToSend, List<ulong> toClientIds = null) where T : NetMessage , new ()
        {
            var dataInBytes = messageToSend.ToBytes();
            var sendModel = new NetMessageTransmitModel(messageToSend.Type, dataInBytes);
            var modelToSendInBytes = Helper.Serialize(sendModel);
            
            var writeSize = FastBufferWriter.GetWriteSize(modelToSendInBytes);
            using (var writer = new FastBufferWriter(writeSize, Allocator.Temp))
            {
                if (!writer.TryBeginWrite(modelToSendInBytes.Length))
                {
                    Debug.Log($"Fail {modelToSendInBytes.Length}");
                    return;
                }

                writer.WriteBytesSafe(modelToSendInBytes,writeSize,0);
                if (_networkManager.IsServer)
                {
                    toClientIds ??= _networkManager.ConnectedClientsIds.ToList();
                    toClientIds.Remove(_networkManager.LocalClientId);
                    _networkManager.CustomMessagingManager.SendUnnamedMessage(toClientIds,
                        writer, NetworkDelivery.ReliableFragmentedSequenced);                          

                    InvokeMessage(messageToSend);
                }
                else
                {
                    _networkManager.CustomMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId,
                        writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
            }
        }

        private void OnUnnamedMessageReceived(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnUnnamedMessageReceived");
            byte[] data = new byte[reader.Length];
            reader.ReadBytesSafe(ref data, reader.Length, 0);
            var receivedModel = Helper.Deserialize<NetMessageTransmitModel>(data);
            
            var type = _messageTypes.TryGetValue(receivedModel.MessageType,out var messageType) ? messageType : null;
            if (type != null)
            {
                var netMessage = MemoryPackSerializer.Deserialize(type,receivedModel.Data) as NetMessage;
                if (netMessage == null) return;

                InvokeMessage(netMessage);
            }
        }

        public void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case ApplicationQuitMessage:
                case TransportFailureMessage:
                    Reset();
                    break;
            }
        }

        private void Register(INetMessageListener messageListener, Type type)
        {
            if (!messagesMap.ContainsKey(type))
            {
                messagesMap.Add(type, new List<INetMessageListener>());
            }

            List<INetMessageListener> listeners = messagesMap[type];
            if (listeners.IndexOf(messageListener) < 0)
            {
                listeners.Add(messageListener);
            }
        }

        public void Register<T>(INetMessageListener messageListener) where T : INetMessage
        {
            var type = typeof(T);
            if (!_messageTypes.ContainsKey(type.Name))
            {
                _messageTypes.Add(type.Name,type);
            }

            Register(messageListener, type);
        }

        public void Unregister<T>(INetMessageListener messageListener) where T : INetMessage
        {
            var type = typeof(T);

            Unregister(messageListener, type);
        }

        private void Unregister(INetMessageListener messageListener, Type typeToRemove)
        {
            if (messagesMap.ContainsKey(typeToRemove))
            {
                List<INetMessageListener> listeners = messagesMap[typeToRemove];
                if (listeners.IndexOf(messageListener) != -1)
                {
                    listeners.Remove(messageListener);
                }
            }
        }

        public void ResetMessenger()
        {
            messagesMap.Clear();
        }

        public void UnregisterAll(INetMessageListener messageListener)
        {
            foreach (KeyValuePair<Type, List<INetMessageListener>> item in messagesMap)
            {
                if (item.Value.Contains(messageListener))
                {
                    item.Value.Remove(messageListener);
                }
            }
        }

        public void InvokeMessage(INetMessage messageToSend)
        {
            if (messagesMap.TryGetValue(messageToSend.GetType(), out List<INetMessageListener> listeners))
            {
                List<INetMessageListener> copyListeners = listeners.ToList(); // to fix modified list bug
                copyListeners.ForEach(x => x.OnMessageReceived(messageToSend));
            }
        }
        
        [Serializable]
        [MemoryPackable()]
        public partial class NetMessageTransmitModel
        {
            public FixedString32Bytes MessageType;
            public byte[] Data;
        }
    }
}
