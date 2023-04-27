using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Maniac.RunnerSystem
{
    public class InitRunnerSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            Runner.InitRunner();
            return IService.Result.Success;
        }
    }
}