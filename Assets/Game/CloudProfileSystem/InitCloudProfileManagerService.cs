using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.CloudProfileSystem
{
    public class InitCloudProfileManagerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var cloudProfileManager = new CloudProfileManager();
            Locator<CloudProfileManager>.Set(cloudProfileManager);

            await cloudProfileManager.Init();

            return IService.Result.Success;
        }
    }
}