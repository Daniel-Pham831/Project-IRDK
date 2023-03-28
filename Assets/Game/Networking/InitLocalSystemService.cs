using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.Networking
{
    public class InitLocalSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            Locator<LocalSystem>.Set(new LocalSystem());

            return IService.Result.Success;
        }
    }
}