using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Maze;
using Maniac.DataBaseSystem;
using Maniac.RandomSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Game.Trader
{
    public class TraderSystem
    {
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private Randomer _randomer => Locator<Randomer>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        
        public List<Vector2> TraderPositions { get; private set; } = new List<Vector2>();

        public async UniTask Init()
        {
            Locator<TraderSystem>.Set(this,true);
        }
        
        public async UniTask GenerateTraders(MazeLevelConfig mazeLevelConfig)
        {
            var randomCells = _mazeSystem.CurrentMaze.CellList.TakeRandomWithSeed(_randomer.Seed, mazeLevelConfig.NumOfTraders);
            TraderPositions.Clear();
            
            foreach (var cell in randomCells)
            {
                TraderPositions.Add(cell.Position);
            }
            
            TraderPositions.Add(_mazeSystem.CurrentMaze.StartCell.Position);
        }

        public bool DoesCellContainTrader(Cell cell)
        {
            return TraderPositions.Contains(cell.Position);
        }
    }
}