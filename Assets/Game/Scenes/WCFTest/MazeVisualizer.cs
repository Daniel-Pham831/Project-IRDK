using System;
using System.Collections.Generic;
using Game.Networking.Network.Commands;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    public class MazeVisualizer : MonoBehaviour
    {
        public int Columns = 1;
        public int Rows = 1;
        public float CellSize = 1f;

        // private Vector2 Offset;
        private Maze.MazeSystem _mazeSystem;
        [SerializeField] private CellVisualizer cellVisualizerPrefab;
        private List<CellVisualizer> _cellVisualizers = new List<CellVisualizer>();

        private void OnValidate()
        {
            Rows = Mathf.Clamp(Rows, 0, Rows);
            Columns = Mathf.Clamp(Columns, 0, Columns);
            CellSize = Mathf.Clamp(CellSize, 0f, CellSize);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    Gizmos.DrawWireCube(new Vector3(column , row ),
                        new Vector3(CellSize, CellSize, 0));
                }
            }
        }

        private async void Start()
        {
            _mazeSystem = new Maze.MazeSystem();
        }

        [Button]
        public async void GenerateMaze()
        {
            await _mazeSystem.GenerateNewMaze(new Vector2Int(Columns, Rows));
            ClearAllConstructedCellVisualizers();
            ConstructCellVisualizers();
        }

        [Button]
        public async void RandomlyDestructMaze()
        {
            var destructionPercent =
                DataBase.ActiveDatabase.GetConfig<MazeConfig>().MazeLevelConfigs[^1].DestructionPercent;
            foreach (var cell in _mazeSystem.CurrentMaze.Cells)
            {
                if (Helper.IsPercentTrigger(destructionPercent))
                {
                    cell.ClearRandomWall();
                }
            }

            ClearAllConstructedCellVisualizers();
            ConstructCellVisualizers();
        }

        private void ClearAllConstructedCellVisualizers()
        {
            foreach (var cellVisualizer in _cellVisualizers)
            {
                Destroy(cellVisualizer.gameObject);
            }
            _cellVisualizers.Clear();
        }
        
        private void ConstructCellVisualizers()
        {
            foreach (var cell in _mazeSystem.CurrentMaze.Cells)
            {
                var cellVisualizer = Instantiate(cellVisualizerPrefab, transform);
                cellVisualizer.Setup(cell);
                _cellVisualizers.Add(cellVisualizer);
            }
            
            var startCellVisualizer = Instantiate(cellVisualizerPrefab, transform);
            startCellVisualizer.Setup(_mazeSystem.CurrentMaze.StartCell);
            _cellVisualizers.Add(startCellVisualizer);
            
            var endCellVisualizer = Instantiate(cellVisualizerPrefab, transform);
            endCellVisualizer.Setup(_mazeSystem.CurrentMaze.EndCell);
            _cellVisualizers.Add(endCellVisualizer);
        }
    }
}