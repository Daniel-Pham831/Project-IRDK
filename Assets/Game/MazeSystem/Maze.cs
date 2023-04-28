using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.MazeSystem
{
    [Serializable]
    public class Maze
    {
        public Maze(Vector2Int dimension)
        {
            Dimension = dimension;
            Cells = new Cell[Dimension.x, Dimension.y];
            for (int i = 0; i < Dimension.x; i++)
            {
                for (int j = 0; j < Dimension.y; j++)
                {
                    Cells[i, j] = new Cell(new Vector2Int(i, j), this);
                }
            }
            
            SetupStartAndEndCell();
        }

        private void SetupStartAndEndCell()
        {
            StartCell = new Cell(new Vector2Int(-1, Random.Range(0,Dimension.y)), this);
            EndCell = new Cell(new Vector2Int(Dimension.x, Random.Range(0,Dimension.y)), this);
            ConnectStartAndEndWithMaze();
        }

        private void ConnectStartAndEndWithMaze()
        {
            StartCell.Walls.Remove(Wall.Right);
            EndCell.Walls.Remove(Wall.Left);
            Cells[0, StartCell.Position.y].Walls.Remove(Wall.Left);
            Cells[Dimension.x - 1, EndCell.Position.y].Walls.Remove(Wall.Right);
        }

        public Vector2Int Dimension;
        public Cell[,] Cells;
        public Cell StartCell;
        public Cell EndCell;

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

                if (direction.y > 0)
                {
                    Cells[aPosition.x, aPosition.y].ClearWalls(new() { Wall.Top });
                    Cells[bPosition.x, bPosition.y].ClearWalls(new() { Wall.Bot });
                }

                if (direction.y < 0)
                {
                    Cells[aPosition.x, aPosition.y].ClearWalls(new() { Wall.Bot });
                    Cells[bPosition.x, bPosition.y].ClearWalls(new() { Wall.Top });
                }

                if (direction.x > 0)
                {
                    Cells[aPosition.x, aPosition.y].ClearWalls(new() { Wall.Right });
                    Cells[bPosition.x, bPosition.y].ClearWalls(new() { Wall.Left });
                }

                if (direction.x < 0)
                {
                    Cells[aPosition.x, aPosition.y].ClearWalls(new() { Wall.Left });
                    Cells[bPosition.x, bPosition.y].ClearWalls(new() { Wall.Right });
                }
            }
            catch
            {
                Debug.Log(aPosition);
                Debug.Log(bPosition);
            }
        }
    }
}