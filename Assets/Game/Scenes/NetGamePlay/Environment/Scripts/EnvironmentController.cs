using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Maze;
using Maniac.CameraSystem;
using Maniac.Utils;
using UniRx;
using UnityEngine;

namespace Game.Scenes.NetGamePlay.Environment.Scripts
{
    public class EnvironmentController : MonoLocator<EnvironmentController>
    {
        private Maze.MazeSystem MazeSystem => Locator<Maze.MazeSystem>.Instance;
        
        [SerializeField] private List<PathGraphic> _pathGraphics;
        [SerializeField] private List<WallGraphic> _wallGraphics;
        [SerializeField] private PolygonCollider2D _confiner;
        public PolygonCollider2D Confiner => _confiner;
        private Cell _currentCell;
        private Maze.Maze _currentMaze;

        public override void Awake()
        {
            base.Awake();
            
            _currentMaze = MazeSystem.CurrentMaze;
            ObserveMaze();
        }

        public async UniTask Init()
        {
            await UniTask.CompletedTask;
        }

        private void ObserveMaze()
        {
            _currentMaze.CurrentObserveCell.Subscribe(SetupEnvironment).AddTo(this);
            _currentMaze.NotifyCellChanged();
        }

        private void SetupEnvironment(Cell cell)
        {
            _currentCell = cell;
            SetupWallGraphics();
            SetupPathGraphics();
        }

        private void SetupWallGraphics()
        {
            foreach (var wallGraphic in _wallGraphics)
            {
                wallGraphic.GraphicObject.SetActive(_currentCell.Walls.Contains(wallGraphic.WallType));
            }
        }

        private void SetupPathGraphics()
        {
            var paths = _currentCell.GetPaths();
            foreach (var pathGraphic in _pathGraphics)
            {
                pathGraphic.GraphicObject.SetActive(paths.Contains(pathGraphic.PathType));
            }
        }

        #region Inner classes

        [Serializable]
        public class PathGraphic
        {
            public Direction PathType;
            public GameObject GraphicObject;
        }
        
        [Serializable]
        public class WallGraphic
        {
            public Direction WallType;
            public GameObject GraphicObject;
        }

        #endregion
    }
}