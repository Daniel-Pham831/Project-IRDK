using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.NormalMessages;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetMessengerSystem
{
    public class NetMessageTransmitter : IMessageListener
    {
        protected NetworkManager _networkManager => NetworkManager.Singleton;
        protected NetworkTransport _transport => _networkManager.NetworkConfig.NetworkTransport;

        public async UniTask Init()
        {
            Locator<NetMessageTransmitter>.Set(this, true);
            RegisterMessages(true);
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

        public void SendNetMessage(NetMessage messageToSend, List<ulong> toClientIds = null)
        {
            var dataInBytes = messageToSend.ToBytes();
            var writeSize = FastBufferWriter.GetWriteSize(dataInBytes);
            using (var writer = new FastBufferWriter(writeSize, Allocator.Temp))
            {
                if (!writer.TryBeginWrite(dataInBytes.Length))
                {
                    Debug.Log($"Fail {dataInBytes.Length}");
                    return;
                }

                writer.WriteValueSafe(dataInBytes);
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
            reader.ReadValueSafe(out data);
            var netMessage = data.ToNetMessage();
            if (netMessage == null) return;

            InvokeMessage(netMessage);
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


        private Dictionary<Type, List<INetMessageListener>> messagesMap =
            new Dictionary<Type, List<INetMessageListener>>();

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
    }
}
