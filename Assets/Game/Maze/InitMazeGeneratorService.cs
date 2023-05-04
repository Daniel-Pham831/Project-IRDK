using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Game.Maze
{
    public class InitMazeGeneratorService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            new Game.Maze.MazeSystem().Init();
            return IService.Result.Success;
        }
    }
}