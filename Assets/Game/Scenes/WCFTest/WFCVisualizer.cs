using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    public class WFCSystem
    {
        public ReactiveProperty<WFCCell[,]> WfcCells = new ReactiveProperty<WFCCell[,]>();
        public List<WFCCell> WfcCellsList = new List<WFCCell>();

        public void Setup(int columns, int rows)
        {
            WfcCells.Value = new WFCCell[columns, rows];
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    WfcCells.Value[column, row] = new WFCCell(column,row);
                    WfcCellsList.Add(WfcCells.Value[column, row]);
                }
            }
        }

        public void Collapse(string id, int column, int row)
        {
            WfcCells.Value[column, row].Collapse(id);
            
            var topNeighbor = GetCell(column, row + 1);
            var bottomNeighbor = GetCell(column, row - 1);
            var leftNeighbor = GetCell(column - 1, row);
            var rightNeighbor = GetCell(column + 1, row);

            topNeighbor?.PossibleIds.Remove(id);
            bottomNeighbor?.PossibleIds.Remove(id);
            leftNeighbor?.PossibleIds.Remove(id);
            rightNeighbor?.PossibleIds.Remove(id);
        }

        private WFCCell GetCell(int column, int row)
        {
            if (column < 0 || column >= WfcCells.Value.GetLength(0) || row < 0 || row >= WfcCells.Value.GetLength(1))
            {
                return null;
            }
            return WfcCells.Value[column, row];
        }
    }

    public class WFCCell
    {
        private WFCVisualizer _wfcVisualizer => Locator<WFCVisualizer>.Instance;
        public int Column;
        public int Row;
        public ReactiveCollection<string> PossibleIds = new ReactiveCollection<string>();
        public ReactiveProperty<Sprite> FinalSprite = new ReactiveProperty<Sprite>(null);
        
        private string _id;

        public WFCCell(int column, int row)
        {
            Column = column;
            Row = row;
            foreach (var constraint in _wfcVisualizer.Constraints)
            {
                PossibleIds.Add(constraint.Id);
            }
        }
        
        public void Collapse(string id)
        {
            _id = id;
            FinalSprite.Value = Locator<WFCVisualizer>.Instance.GetSpriteWithId(_id);
            PossibleIds.Clear();
        }
    }

    public class WFCVisualizer : MonoBehaviour
    {
        public List<CellInfo> Constraints;
        public int Columns = 1;
        public int Rows = 1;
        public float CellSize = 1f;
        private WFCSystem _newWFCSystem;
        public WFCCellVisualizer WfcCellVisualizerPrefab;
        public Transform WfcCellVisualizerParent;

        private void Awake()
        {
            Locator<WFCVisualizer>.Set(this);
            _newWFCSystem = new WFCSystem();
            Locator<WFCSystem>.Set(_newWFCSystem,true);
        }

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
                    Gizmos.DrawWireCube(new Vector3(column * CellSize , row * CellSize),
                        new Vector3(CellSize, CellSize, 0));
                }
            }
        }

        public Sprite GetSpriteWithId(string id)
        {
            return Constraints.FirstOrDefault(x => x.Id == id)?.SpriteImg;
        }
        
        [Button]
        public async void ResetWFCGrid()
        {
            _newWFCSystem.Setup(Columns, Rows);
            SetupWFCVisualizer();
        }

        private void SetupWFCVisualizer()
        {
            foreach (var cell in _newWFCSystem.WfcCellsList)
            {
                var wfcCellVisualizer = Instantiate(WfcCellVisualizerPrefab, WfcCellVisualizerParent);
                wfcCellVisualizer.Setup(cell);
            }
        }


        [Serializable]
        public class CellInfo
        {
            public string Id;
            public Sprite SpriteImg;
            public List<string> Neighbours;
        }
    }
}