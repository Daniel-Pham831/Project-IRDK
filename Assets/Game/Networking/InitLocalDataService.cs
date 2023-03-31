using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.Networking
{
    public class InitLocalDataService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            Locator<LocalData>.Set(new LocalData());

            return IService.Result.Success;
        }
    }
}