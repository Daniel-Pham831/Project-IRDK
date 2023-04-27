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

        private void RemoveUnClearableWalls(List<Wall> wallsToClear)
        {
            if (Position.y == 0 && wallsToClear.Contains(Wall.Left))
                wallsToClear.Remove(Wall.Left);

            if (Position.x == 0 && wallsToClear.Contains(Wall.Top))
                wallsToClear.Remove(Wall.Top);

            if (Position.y == _currentMaze.Dimension.y - 1 && wallsToClear.Contains(Wall.Right))
                wallsToClear.Remove(Wall.Right);

            if (Position.x == _currentMaze.Dimension.x - 1 && wallsToClear.Contains(Wall.Bot))
                wallsToClear.Remove(Wall.Bot);
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