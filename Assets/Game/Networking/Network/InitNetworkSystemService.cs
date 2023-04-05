using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.Networking.Network
{
    public class InitNetworkSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var networkSystem = new NetworkSystem();
            Locator<NetworkSystem>.Set(networkSystem);

            await networkSystem.Init();

            return IService.Result.Success;
        }
    }
}