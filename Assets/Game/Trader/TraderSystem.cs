using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Maze;
using Maniac.DataBaseSystem;
using Maniac.DataBaseSystem.Trader;
using Maniac.RandomSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Game.Trader
{
    public class TraderSystem
    {
        private MazeLevelConfig _mazeLevelConfig;
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private Randomer _randomer => Locator<Randomer>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TraderConfig _traderConfig => _dataBase.GetConfig<TraderConfig>();
        
        public List<Vector2> TraderPositions { get; private set; } = new List<Vector2>();

        public async UniTask Init()
        {
            Locator<TraderSystem>.Set(this,true);
        }
        
        public async UniTask GenerateTraders(MazeLevelConfig mazeLevelConfig)
        {
            TraderPositions.Add(_mazeSystem.CurrentMaze.StartCell.Position);
            var currentMazeCellList = _mazeSystem.CurrentMaze.CellList;

            while(TraderPositions.Count < mazeLevelConfig.NumOfTraders)
            {
                var randomCell = currentMazeCellList.TakeRandomUnity();

                bool shouldContinue = false;
                foreach (var position in TraderPositions)
                {
                    if (ArePositionsNearBy(position, randomCell.Position))
                    {
                        shouldContinue = true;
                        break;
                    }
                }
                
                if(shouldContinue) continue;
                TraderPositions.Add(randomCell.Position);
            }
        }

        private bool ArePositionsNearBy(Vector2 positionA, Vector2 positionB)
        {
            var magnitude = (positionB - positionA).magnitude;
            return magnitude <= 1;
        }

        public bool DoesCellContainTrader(Cell cell)
        {
            return TraderPositions.Contains(cell.Position);
        }
    }
}