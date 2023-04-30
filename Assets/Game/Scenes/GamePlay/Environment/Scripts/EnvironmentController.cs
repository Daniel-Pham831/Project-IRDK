using System;
using System.Collections.Generic;
using Game.MazeSystem;
using Maniac.Utils;
using UniRx;
using UnityEngine;

namespace Game.Scenes.GamePlay.Environment.Scripts
{
    public class EnvironmentController : MonoLocator<EnvironmentController>
    {
        [SerializeField] private List<PathGraphic> _pathGraphics;
        [SerializeField] private List<WallGraphic> _wallGraphics;
        private Cell _currentCell;
        private Maze _currentMaze;

        public void InitEnvironment(Maze maze)
        {
            _currentMaze = maze;
            ObserveMaze();
            _currentMaze.NotifyCellChanged();
        }

        private void ObserveMaze()
        {
            _currentMaze.CurrentObserveCell.Subscribe(SetupEnvironment).AddTo(this);
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