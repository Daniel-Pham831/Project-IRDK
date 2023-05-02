using System;
using Game.Networking.Network;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetDataTransmitterComponents
{
    public class PingHandler: NetworkBehaviour,IDisposable
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private NetConfig _config;
        
        public FloatReactiveProperty PingInMilliSeconds { get; private set; } = new FloatReactiveProperty();
        private float _lastSendPingTime;
        
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private NetworkManager _networkManager => _networkSystem.NetworkManager;
        private NetworkTransport _transport => _networkManager.NetworkConfig.NetworkTransport;
        
        private async void Awake()
        {
            _config = _dataBase.GetConfig<NetConfig>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            Locator<PingHandler>.Set(this);
            HandlePingMessage();
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            Locator<PingHandler>.Remove();

            base.OnNetworkDespawn();
        }

        private void HandlePingMessage()
        {
            _timeManager.OnTimeOut(() =>
            {
                if (this == null) return;
                
                _lastSendPingTime = Time.realtimeSinceStartup;
                CheckPingToServer();
                // SendPingToServerRpc();
                HandlePingMessage();
            },_config.SendPingInterval);
        }

        private void CheckPingToServer()
        {
            PingInMilliSeconds.Value = _transport.GetCurrentRtt(NetworkManager.ServerClientId);
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

        public void Dispose()
        {
            PingInMilliSeconds?.Dispose();
        }
    }
}