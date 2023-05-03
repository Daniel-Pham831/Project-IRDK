using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;
using UnityEditor.Timeline;
using UnityEngine;

namespace Game.Scenes.NetGamePlay.Scripts
{
    public class NetGamePlayController : MonoLocator<NetGamePlayController> , INetMessageListener
    {
        private NetworkManager _networkManager => NetworkManager.Singleton;
        
        private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;
        
        // This is for host-server only
        private ReactiveProperty<Dictionary<ulong,Direction>> _playersChosenDirection = new ReactiveProperty<Dictionary<ulong, Direction>>(new Dictionary<ulong, Direction>());

        public override void Awake()
        {
            if (_networkManager.IsServer)
            {
                foreach (var clientId in _networkManager.ConnectedClientsIds)
                {
                    _playersChosenDirection.Value.Add(clientId,Direction.None);
                }
                
                _playersChosenDirection.Subscribe(OnPlayerChosenDirectionChanged).AddTo(this);
                
                _netMessageTransmitter.Register<UpdateChosenDirectionToServerNetMessage>(this);
            }

            if (_networkManager.IsClient)
            {
                _netMessageTransmitter.Register<MoveToNextCellNetMessage>(this);
                _netMessageTransmitter.Register<AllPlayersMuchChooseDirectionWarningNetMessage>(this);
            }
        }
        
        public void OnNetMessageReceived(NetMessage message)
        {
            switch (message)
            {
                case UpdateChosenDirectionToServerNetMessage updateChosenDirectionToServerNetMessage:
                    bool shouldNotify = updateChosenDirectionToServerNetMessage.ChosenDirection != Direction.None;
                        _playersChosenDirection.Value[updateChosenDirectionToServerNetMessage.SenderID] =
                            updateChosenDirectionToServerNetMessage.ChosenDirection;
                    
                    if (shouldNotify)
                        _playersChosenDirection.SetValueAndForceNotify(_playersChosenDirection.Value);
                    break;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _netMessageTransmitter.UnregisterAll(this);
        }

        public async UniTask Init()
        {
            await UniTask.CompletedTask;
        }
        
        // Check If _playersChosenDirection is full of Direction which is not None
        // If it is, send MoveToNextCellNetMessage to all clients
        // If it isn't, send AllPlayersMuchChooseDirectionMessage to all clients
        private async void OnPlayerChosenDirectionChanged(Dictionary<ulong, Direction> playersChosenDirection)
        {
            if (ShouldMoveToNextCell())
            {
                var direction = playersChosenDirection.First().Value;
                _netMessageTransmitter.SendNetMessage(new MoveToNextCellNetMessage(direction));
            }
            else
            {
                _netMessageTransmitter.SendNetMessage(new AllPlayersMuchChooseDirectionWarningNetMessage());
            }
        }

        private bool ShouldMoveToNextCell()
        {
            var allDirectionValues = _playersChosenDirection.Value.Values;
            var isNotNone = allDirectionValues.Any(x=>x != Direction.None);
            var isAllTheSameDirection = allDirectionValues.Distinct().Count() == 1;
            
            return isNotNone && isAllTheSameDirection;
        }

        
    }
}