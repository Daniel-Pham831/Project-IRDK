using System;
using System.Collections.Generic;
using Game.Networking.Network.NetworkModels;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;
using NetworkTransport = Unity.Netcode.NetworkTransport;

namespace Game.Scenes.TestScene
{
    
    [Serializable]
    [MemoryPackable]
    public partial class TestModel
    {
        public string Data;
    }
    
    public class Transmitter<T> : MonoBehaviour where T : class
    {
        protected NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        protected NetworkManager _networkManager => NetworkManager.Singleton;
        protected NetworkTransport _transport => _networkManager.NetworkConfig.NetworkTransport;
        
        protected virtual void Awake()
        {
            Locator<T>.Set(this as T,true);
            RegisterEvents(true);
        }

        protected virtual void OnDestroy()
        {
            Locator<T>.Remove(this as T);
            RegisterEvents(false);
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
        }

        protected virtual void OnUnnamedMessageReceived(ulong clientId, FastBufferReader reader)
        {
            byte[] data = new byte[reader.Length];
            reader.ReadValueSafe(out data);
            OnDataReceived(data,clientId);
        }

        protected virtual void OnDataReceived(byte[] data, ulong clientId){}
    }
}