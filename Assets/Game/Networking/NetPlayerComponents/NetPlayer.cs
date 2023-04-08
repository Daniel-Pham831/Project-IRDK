using Game.CloudProfileSystem;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetPlayerComponents
{
    public class NetPlayer : NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;

        private UserProfile _userProfile;
        private NetConfig _config;

        public FloatReactiveProperty PingInMilliSeconds { get; private set; } = new FloatReactiveProperty();
        private float _lastSendPingTime;
        
        // Shared
        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks;

        public NetworkList<NetPlayerModel> NetPlayerModels { get; private set; } = new NetworkList<NetPlayerModel>();

        private async void Awake()
        {
            _config = _dataBase.Get<NetConfig>();
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            SendNetPlayerModelToServerRpc(
                new NetPlayerModel()
                {
                    ClientId = NetworkManager.Singleton.LocalClientId,
                    Name = _userProfile.DisplayName
                }
            );
            
            Locator<NetPlayer>.Set(this);
            base.OnNetworkSpawn();
        }

        [ServerRpc]
        private void SendNetPlayerModelToServerRpc(NetPlayerModel netPlayerModel,ServerRpcParams param = default)
        {
            NetPlayerModels.Add(netPlayerModel);
        }

        public override void OnNetworkDespawn()
        {
            if(IsOwner)
                Locator<NetPlayer>.Remove();
            
            base.OnNetworkDespawn();
        }
    }
}