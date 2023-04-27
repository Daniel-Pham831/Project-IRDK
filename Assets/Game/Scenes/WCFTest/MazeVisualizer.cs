using System;
using System.Collections.Generic;
using Game.MazeSystem;
using Game.Networking.Network.Commands;
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
        private MazeGenerator _mazeGenerator;
        [SerializeField] private CellVisualizer cellVisualizerPrefab;
        private List<CellVisualizer> _cellVisualizers = new List<CellVisualizer>();

        private void OnValidate()
        {
            Rows = Mathf.Clamp(Rows, 0, Rows);
            Columns = Mathf.Clamp(Columns, 0, Columns);
            CellSize = Mathf.Clamp(CellSize, 0f, CellSize);

            // Offset.x = (float)((Columns / 2f - 0.5) * -1f);
            // Offset.y = (float)((Rows / 2f - 0.5) * -1f);
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
            _mazeGenerator = new MazeGenerator();
        }

        [Button]
        public async void GenerateMaze()
        {
            await _mazeGenerator.GenerateNewMaze(new Vector2Int(Columns, Rows));
            _mazeGenerator.ExportToJSON();
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
            foreach (var cell in _mazeGenerator.CurrentMaze.Cells)
            {
                var cellVisualizer = Instantiate(cellVisualizerPrefab, transform);
                cellVisualizer.Setup(cell);
                _cellVisualizers.Add(cellVisualizer);
            }
        }
    }
}