using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;

namespace Game.Networking.Network.Commands
{
    public class GenerateMazeCommand : Command
    {
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private Maze.MazeSystem MazeSystem => Locator<Maze.MazeSystem>.Instance;
        private MazeConfig _mazeConfig;

        public override async UniTask Execute()
        {
            _mazeConfig = _dataBase.GetConfig<MazeConfig>();
            await MazeSystem.GenerateNewMaze(_mazeConfig.DefaultMazeDimensions);
        }
    }
}