using System;
using System.Collections.Generic;
using Game.Enums;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Maze
{
    [Serializable]
    public class Maze
    {
        public Vector2Int Dimension;
        public Cell[,] Cells;
        public List<Cell> CellList = new List<Cell>();
        public Cell StartCell;
        public Cell EndCell;
        public ReactiveProperty<Cell> CurrentObserveCell = new ReactiveProperty<Cell>(null);

        #region Maze Initialization 

         public Maze(Vector2Int dimension)
        {
            Dimension = dimension;
            Cells = new Cell[Dimension.x, Dimension.y];
            CellList.Clear();
            for (int i = 0; i < Dimension.x; i++)
            {
                for (int j = 0; j < Dimension.y; j++)
                {
                    Cells[i, j] = new Cell(new Vector2Int(i, j), this);
                    CellList.Add(Cells[i, j]);
                }
            }

            SetupStartAndEndCell();
        }

        private void SetupStartAndEndCell()
        {
            StartCell = new Cell(new Vector2Int(-1, Random.Range(0, Dimension.y)), this);
            EndCell = new Cell(new Vector2Int(Dimension.x, Random.Range(0, Dimension.y)), this);
            ConnectStartAndEndWithMaze();
            CurrentObserveCell.Value = StartCell;
        }

        private void ConnectStartAndEndWithMaze()
        {
            StartCell.Walls.Remove(Direction.Right);
            EndCell.Walls.Remove(Direction.Left);
            Cells[0, StartCell.Position.y].Walls.Remove(Direction.Left);
            Cells[Dimension.x - 1, EndCell.Position.y].Walls.Remove(Direction.Right);
        }

        public bool IsCellValid(Vector2Int randomNeighbor)
        {
            try
            {
                return Cells[randomNeighbor.x, randomNeighbor.y].IsCellValid();
            }
            catch
            {
                return false;
            }
        }

        public void BreakWallsBetween(Vector2Int aPosition, Vector2Int bPosition)
        {
            try
            {
                var direction = bPosition - aPosition;

                switch (direction.y)
                {
                    case > 0:
                        Cells[aPosition.x, aPosition.y].ClearWalls(new() { Direction.Top });
                        Cells[bPosition.x, bPosition.y].ClearWalls(new() { Direction.Bottom });
                        break;
                    case < 0:
                        Cells[aPosition.x, aPosition.y].ClearWalls(new() { Direction.Bottom });
                        Cells[bPosition.x, bPosition.y].ClearWalls(new() { Direction.Top });
                        break;
                }

                switch (direction.x)
                {
                    case > 0:
                        Cells[aPosition.x, aPosition.y].ClearWalls(new() { Direction.Right });
                        Cells[bPosition.x, bPosition.y].ClearWalls(new() { Direction.Left });
                        break;
                    case < 0:
                        Cells[aPosition.x, aPosition.y].ClearWalls(new() { Direction.Left });
                        Cells[bPosition.x, bPosition.y].ClearWalls(new() { Direction.Right });
                        break;
                }
            }
            catch
            {
                Debug.Log(aPosition);
                Debug.Log(bPosition);
            }
        }

        #endregion

        #region Maze Control

        public void NotifyCellChanged()
        {
            CurrentObserveCell.SetValueAndForceNotify(CurrentObserveCell.Value);
        }
        
        public bool IsDirectionValid(Direction direction)
        {
            return !CurrentObserveCell.Value.Walls.Contains(direction);
        }
        
        public void MoveToDirection(Direction direction)
        {
            var nextCellPosition = CurrentObserveCell.Value.Position + direction.ToVector2Int();
            if(StartCell.Position == nextCellPosition)
            {
                CurrentObserveCell.Value = StartCell;
                return;
            }
            else if(EndCell.Position == nextCellPosition)
            {
                CurrentObserveCell.Value = EndCell;
                return;
            }
            
            try
            {
                CurrentObserveCell.Value = Cells[nextCellPosition.x, nextCellPosition.y];
            }
            catch
            {
                Debug.LogError($"There is no cell in {direction} direction");
                CurrentObserveCell.Value = null;
            }
        }
        
        #endregion
    }
}