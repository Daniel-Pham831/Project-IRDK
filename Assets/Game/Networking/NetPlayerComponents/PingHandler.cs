using Game.CloudProfileSystem;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetPlayerComponents
{
    public class PingHandler: NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private NetConfig _config;
        
        public FloatReactiveProperty PingInMilliSeconds { get; private set; } = new FloatReactiveProperty();
        private float _lastSendPingTime;
        
        private async void Awake()
        {
            _config = _dataBase.Get<NetConfig>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            Locator<PingHandler>.Set(this);
            HandlePingMessage();
            base.OnNetworkSpawn();
        }
        
        public override void OnNetworkDespawn()
        {
            if(IsOwner)
                Locator<PingHandler>.Remove();
            
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
    }
}