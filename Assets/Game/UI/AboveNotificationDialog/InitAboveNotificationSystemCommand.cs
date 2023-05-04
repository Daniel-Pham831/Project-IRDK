using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Services;
using Maniac.Utils;

namespace Game
{
    public class InitAboveNotificationSystemService: Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var aboveNoti = new AboveNotificationSystem();
            Locator<AboveNotificationSystem>.Set(aboveNoti);
            await aboveNoti.Init();
            return IService.Result.Success;
        }
    }
}