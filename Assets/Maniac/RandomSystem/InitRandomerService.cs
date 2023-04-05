using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Maniac.RandomSystem
{
    public class InitRandomerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var randomer = new Randomer();
            Locator<Randomer>.Set(randomer);
            randomer.Init();
            return IService.Result.Success;
        }
    }
}