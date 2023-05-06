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
        public MazeLevelConfig DefaultMazeLevelConfig => MazeLevelConfigs[0];
        public Color StartCellColor;
        public Color EndCellColor;
        public Color NormalCellColor;
        public Color YouAreHereColor;
        public Sprite TraderSmallSprite;
    }

    [Serializable]
    public class MazeLevelConfig
    {
        public Difficulty Difficulty;
        public Vector2Int Dimensions;
        public float DestructionPercent;
        public int NumOfTraders;
    }
}
