using System;
using UnityEngine;

namespace Game.Enums
{
    [Serializable]
    public enum Direction
    {
        None,
        Top,
        Right,
        Bottom,
        Left,
    }

    public static class DirectionExtensions
    {
        public static Vector2Int ToVector2Int(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Top:
                    return Vector2Int.up;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Bottom:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        } 
    }
}