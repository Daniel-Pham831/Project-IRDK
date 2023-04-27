using Cysharp.Threading.Tasks;
using Game.MazeSystem;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;

namespace Game.Networking.Network.Commands
{
    public class GenerateMazeCommand : Command
    {
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private MazeGenerator mazeGenerator => Locator<MazeGenerator>.Instance;
        private MazeConfig _mazeConfig;

        public override async UniTask Execute()
        {
            _mazeConfig = _dataBase.GetConfig<MazeConfig>();
            await mazeGenerator.GenerateNewMaze(_mazeConfig.DefaultMazeDimensions);
        }
    }
}