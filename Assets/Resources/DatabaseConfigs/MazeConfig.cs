using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Maniac;

namespace Maniac.DataBaseSystem
{
    public class MazeConfig : DataBaseConfig
    {
        public Vector2Int DefaultMazeDimensions;
        public List<MazeLevelConfig> MazeLevelConfigs;
    }

    [Serializable]
    public struct MazeLevelConfig
    {
        public Difficulty Difficulty;
        public Vector2Int Dimensions;
        public float DestructionPercent;
    }
}
