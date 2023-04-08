using System;
using Game.Networking.NetMessages;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.Scripts
{
    public class NetPlayer : NetworkBehaviour
    {
        public const float GameTick = 30f;
        public const int BufferSize = 1024;
        
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;

        private NetConfig _config;

        public FloatReactiveProperty PingInMilliSeconds { get; private set; } = new FloatReactiveProperty();
        private float _lastSendPingTime;
        
        // Shared
        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks;
        
        private void Awake()
        {
            _config = _dataBase.Get<NetConfig>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            Locator<NetPlayer>.Set(this);
            HandlePingMessage();
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            if(IsOwner)
                Locator<NetPlayer>.Remove();
            
            base.OnNetworkDespawn();
        }

        private void HandlePingMessage()
        {
            _timeManager.OnTimeOut(() =>
            {
                if (this == null) return;
                
                _lastSendPingTime = Time.realtimeSinceStartup;
                SendPingToServerRpc();
                HandlePingMessage();
            },_config.SendPingInterval);
        }

        [ServerRpc]
        private void SendPingToServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (OwnerClientId == serverRpcParams.Receive.SenderClientId)
            {
                SendPingToClientRpc();
            }
        }

        [ClientRpc]
        private void SendPingToClientRpc(ClientRpcParams clientRpcParams = default)
        {
            PingInMilliSeconds.Value = (Time.realtimeSinceStartup - _lastSendPingTime)*1000;
        }

        //
        // [ServerRpc]
        // public void SendDataToServerRpc(string key, object data, ServerRpcParams serverRpcParams = default)
        // {
        //     if (OwnerClientId == serverRpcParams.Receive.SenderClientId)
        //     {
        //         SendDataToClientRpc(key,data);
        //     }
        // }
        //
        // [ClientRpc]
        // private void SendDataToClientRpc(string key, object data, ClientRpcParams clientRpcParams = default)
        // {
        //     Messenger.SendMessage(new ServerMessage(key,data));
        // }
    }
}