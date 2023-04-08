using System;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
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
        public NetworkVariable<float> LastSendPingTime { get; private set; } = new NetworkVariable<float>();
        
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
            LastSendPingTime.OnValueChanged += UpdatePingInMilliSeconds;
            HandlePingMessage();
            base.OnNetworkSpawn();
        }

        private void UpdatePingInMilliSeconds(float previousvalue, float newvalue)
        {
            PingInMilliSeconds.Value = (Time.realtimeSinceStartup - _lastSendPingTime)*1000;
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
                SendPingServerRpc();
                HandlePingMessage();
            },_config.SendPingInterval);
        }

        [ServerRpc]
        private void SendPingServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (OwnerClientId == serverRpcParams.Receive.SenderClientId)
            {
                LastSendPingTime.Value = Time.realtimeSinceStartup;
            }
        }

        [ClientRpc]
        private void SendPingClientRpc(ClientRpcParams clientRpcParams = default)
        {
            PingInMilliSeconds.Value = (Time.realtimeSinceStartup - _lastSendPingTime)*1000;
        }
    }
}