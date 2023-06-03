using Cysharp.Threading.Tasks;
using Game.Maze;
using Game.Trader;
using Maniac.Command;
using Maniac.Utils;
using System;

namespace Assets.Game.Monster
{
    public class SpawnMonstersCommand : Command
    {
        private MonsterSystem _monsterSystem => Locator<MonsterSystem>.Instance;
        private TraderSystem traderSystem => Locator<TraderSystem>.Instance;
        private MazeSystem mazeSystem => Locator<MazeSystem>.Instance;


        public override async UniTask Execute()
        {
            if (!IsSpawnable()) return;

            _monsterSystem.monsterSpawner.SpawnMonster();
        }

        private bool IsSpawnable()
        {
            var curentCell = mazeSystem.CurrentMaze.CurrentObserveCell.Value;
            return !traderSystem.DoesCellContainTrader(curentCell);
        }
    }
}
