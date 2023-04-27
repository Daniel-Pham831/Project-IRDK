using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Game.MazeSystem
{
    public class InitMazeGeneratorService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            new MazeGenerator().Init();
            return IService.Result.Success;
        }
    }
}