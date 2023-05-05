using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Game.Trader
{
    public class InitTraderSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var traderSystem = new TraderSystem();
            await traderSystem.Init();
            return IService.Result.Success;
        }
    }
}