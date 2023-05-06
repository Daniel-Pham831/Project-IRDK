using Cysharp.Threading.Tasks;
using Game.Maze;
using Game.Trader;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;

namespace Game.Networking.Network.Commands
{
    public class GenerateMapCommand : Command
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private TraderSystem _traderSystem => Locator<TraderSystem>.Instance;
        private MazeConfig _mazeConfig;

        public override async UniTask Execute()
        {
            _mazeConfig = _dataBase.GetConfig<MazeConfig>();
            await _mazeSystem.GenerateNewMaze(_mazeConfig.DefaultMazeLevelConfig);
            await _traderSystem.GenerateTraders(_mazeConfig.DefaultMazeLevelConfig);
        }
    }
}