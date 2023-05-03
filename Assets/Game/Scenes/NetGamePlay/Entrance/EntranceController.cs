using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.MazeSystem;
using Game.Scenes.NetGamePlay.Commands;
using Maniac.Utils;
using ToolBox.Tags;
using UnityEngine;

namespace Game.Scenes.NetGamePlay.Entrance
{
    public class EntranceController : MonoLocator<EntranceController>
    {
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
            // await new MovePlayersToCellAtDirectionCommand()
        }

        public async UniTask OnPlayerExit(Direction entranceDirection)
        {
            throw new NotImplementedException();
        }
    }
}