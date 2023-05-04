using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Game.Scenes.NetGamePlay.Commands;
using Maniac.LanguageTableSystem;
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
        private ShowAboveNotificationCommand _warningNotiCommand;

        public override void Awake()
        {
            base.Awake();
            
            if (_networkManager.IsServer)
            {
                foreach (var clientId in _networkManager.ConnectedClientsIds)
                {
                    _playersChosenDirection.Value.Add(clientId,Direction.None);
                }
                
                _playersChosenDirection.Subscribe(OnPlayerChosenDirectionChanged).AddTo(this);
                
                _netMessageTransmitter.Register<UpdateChosenDirectionNetMessage>(this);
            }

            if (_networkManager.IsClient)
            {
                _netMessageTransmitter.Register<MoveToNextCellNetMessage>(this);
                _netMessageTransmitter.Register<AllPlayersMuchChooseDirectionWarningNetMessage>(this);
            }
        }
        
        public async void OnNetMessageReceived(NetMessage message)
        {
            switch (message)
            {
                case UpdateChosenDirectionNetMessage updateChosenDirectionToServerNetMessage:
                    OnUpdateChosenDirection(updateChosenDirectionToServerNetMessage);
                    break;
                
                case AllPlayersMuchChooseDirectionWarningNetMessage:
                    await OnAllPlayersMuchChooseDirectionWarning();
                    break;
                
                case MoveToNextCellNetMessage moveToNextCellNetMessage:
                    await OnMoveToNextCell(moveToNextCellNetMessage);
                    break;
            }
        }

        // This is for clients
        private async UniTask OnMoveToNextCell(MoveToNextCellNetMessage moveToNextCellNetMessage)
        {
            // this shouldn't be happen but just for sure
            if (moveToNextCellNetMessage.CellDirection == Direction.None) return;
            
            Debug.Log($"Move To Next Cell {moveToNextCellNetMessage.CellDirection}");
            await new MoveToNextCellCommand(moveToNextCellNetMessage.CellDirection).Execute();
            
            
            // Start spawn creep
        }

        // This is for clients
        private async UniTask OnAllPlayersMuchChooseDirectionWarning()
        {
            if (_warningNotiCommand == null)
            {
                _warningNotiCommand = new ShowAboveNotificationCommand(
                    LanguageTable.AboveNoti_SamePathWarning,
                    () => Debug.Log("Warning Noti Closed"),
                    null,
                    1.5f
                );
            }

            await _warningNotiCommand.Execute();
        }

        // This is for host-server only
        private void OnUpdateChosenDirection(UpdateChosenDirectionNetMessage updateChosenDirectionNetMessage)
        {
            bool shouldNotify = updateChosenDirectionNetMessage.ChosenDirection != Direction.None;
            _playersChosenDirection.Value[updateChosenDirectionNetMessage.SenderID] =
                updateChosenDirectionNetMessage.ChosenDirection;

            if (shouldNotify)
                _playersChosenDirection.SetValueAndForceNotify(_playersChosenDirection.Value);
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
            if (playersChosenDirection.Values.All(x => x == Direction.None)) return;
            
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