using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.Networking.Relay
{
    public class InitRelaySystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var relaySystem = new RelaySystem();
            Locator<RelaySystem>.Set(relaySystem);

            await relaySystem.Init();

            return IService.Result.Success;
        }
    }
}