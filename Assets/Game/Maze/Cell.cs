using System;
using System.Collections.Generic;
using Game.Enums;
using UnityEngine;

namespace Game.Maze
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
        public List<Direction> Walls = new() { Direction.Top, Direction.Right, Direction.Bottom, Direction.Left };

        public void ClearWalls(List<Direction> wallsToClear)
        {
            RemoveUnClearableWalls(wallsToClear);
            Walls.RemoveAll(wallsToClear.Contains);
        }

        // if there is wall at the given direction, then there will be no path at that direction
        public List<Direction> GetPaths()
        {
            var paths = new List<Direction>();
            if (!Walls.Contains(Direction.Top)) paths.Add(Direction.Top);
            if (!Walls.Contains(Direction.Right)) paths.Add(Direction.Right);
            if (!Walls.Contains(Direction.Bottom)) paths.Add(Direction.Bottom);
            if (!Walls.Contains(Direction.Left)) paths.Add(Direction.Left);
            return paths;
        }

        public void ClearRandomWall()
        {
            if (Walls.Count == 0) return;
            var randomWall = Walls[UnityEngine.Random.Range(0, Walls.Count)];
            var neighBorToThatWall = GetNeighborToThatWall(randomWall);
            if (neighBorToThatWall == null) return;
            
            _currentMaze.BreakWallsBetween(this.Position, neighBorToThatWall.Position);
        }
        
        public Cell GetNeighborToThatWall(Direction wall)
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

        private Vector2Int GetWallDirection(Direction wall)
        {
            return wall switch
            {
                Direction.Top => Vector2Int.up,
                Direction.Right => Vector2Int.right,
                Direction.Bottom => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                _ => throw new ArgumentOutOfRangeException(nameof(wall), wall, null)
            };
        }


        private void RemoveUnClearableWalls(List<Direction> wallsToClear)
        {
            if (Position.y == 0 && wallsToClear.Contains(Direction.Bottom))
                wallsToClear.Remove(Direction.Bottom);

            if (Position.x == 0 && wallsToClear.Contains(Direction.Left))
                wallsToClear.Remove(Direction.Left);

            if (Position.y == _currentMaze.Dimension.y - 1 && wallsToClear.Contains(Direction.Top))
                wallsToClear.Remove(Direction.Top);

            if (Position.x == _currentMaze.Dimension.x - 1 && wallsToClear.Contains(Direction.Right))
                wallsToClear.Remove(Direction.Right);
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