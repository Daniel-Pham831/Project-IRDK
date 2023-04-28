using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MazeSystem
{
    [Serializable]
    public class Cell
    {
        public Cell(Vector2Int position,Maze currentMaze)
        {
            Position = position;
            _currentMaze = currentMaze;
        }

        private Maze _currentMaze;
        public Vector2Int Position;
        public List<Wall> Walls = new() { Wall.Top, Wall.Right, Wall.Bot, Wall.Left };

        public void ClearWalls(List<Wall> wallsToClear)
        {
            RemoveUnClearableWalls(wallsToClear);
            Walls.RemoveAll(wallsToClear.Contains);
        }

        public void ClearRandomWall()
        {
            if (Walls.Count == 0) return;
            var randomWall = Walls[UnityEngine.Random.Range(0, Walls.Count)];
            var neighBorToThatWall = GetNeighborToThatWall(randomWall);
            if (neighBorToThatWall == null) return;
            
            _currentMaze.BreakWallsBetween(this.Position, neighBorToThatWall.Position);
        }
        
        public Cell GetNeighborToThatWall(Wall wall)
        {
            var neighborPosition = Position + GetWallDirection(wall);
            Cell result = null;
            try
            {
                result = _currentMaze.Cells[neighborPosition.x, neighborPosition.y];
            }
            catch
            {
                // ignored
            }

            return result;
        }

        private Vector2Int GetWallDirection(Wall wall)
        {
            return wall switch
            {
                Wall.Top => Vector2Int.up,
                Wall.Right => Vector2Int.right,
                Wall.Bot => Vector2Int.down,
                Wall.Left => Vector2Int.left,
                _ => throw new ArgumentOutOfRangeException(nameof(wall), wall, null)
            };
        }


        private void RemoveUnClearableWalls(List<Wall> wallsToClear)
        {
            if (Position.y == 0 && wallsToClear.Contains(Wall.Bot))
                wallsToClear.Remove(Wall.Bot);

            if (Position.x == 0 && wallsToClear.Contains(Wall.Left))
                wallsToClear.Remove(Wall.Left);

            if (Position.y == _currentMaze.Dimension.y - 1 && wallsToClear.Contains(Wall.Top))
                wallsToClear.Remove(Wall.Top);

            if (Position.x == _currentMaze.Dimension.x - 1 && wallsToClear.Contains(Wall.Right))
                wallsToClear.Remove(Wall.Right);
        }

        public bool IsCellValid()
        {
            return Position.x >= 0 &&
                   Position.x <= _currentMaze.Dimension.x - 1 &&
                   Position.y >= 0 &&
                   Position.y <= _currentMaze.Dimension.y - 1;
        }
    }
}