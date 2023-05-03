using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.MazeSystem;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Game.Scenes.NetGamePlay.Commands;
using Maniac.Utils;
using ToolBox.Tags;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scenes.NetGamePlay.Entrance
{
    public class EntranceController : MonoLocator<EntranceController>
    {
        private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;
        private MazeGenerator _mazeGenerator => Locator<MazeGenerator>.Instance;
        private Maze _currentMaze => _mazeGenerator.CurrentMaze;
        
        [SerializeField] private Tag _playerTag;
        [SerializeField] private Tag _localPlayerTag;
        [SerializeField] private List<EntrancePathCollider> _entrancePathColliders;
        public Tag PlayerTag => _playerTag;
        public Tag LocalPlayerTag => _localPlayerTag;

        public override void Awake()
        {
            base.Awake();
            _entrancePathColliders.ForEach(x => x.Init(this));
        }

        public async UniTask OnPlayerEnter(Direction entranceDirection)
        {
            _netMessageTransmitter.SendNetMessage(new UpdateChosenDirectionNetMessage(entranceDirection),new List<ulong>()
            {
                NetworkManager.ServerClientId
            });
        }

        public async UniTask OnPlayerExit()
        {
            _netMessageTransmitter.SendNetMessage(new UpdateChosenDirectionNetMessage(),new List<ulong>()
            {
                NetworkManager.ServerClientId
            });
        }
    }
}