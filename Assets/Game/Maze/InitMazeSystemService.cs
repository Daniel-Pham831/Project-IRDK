using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Game.Maze
{
    public class InitMazeSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            new MazeSystem().Init();
            return IService.Result.Success;
        }
    }
}