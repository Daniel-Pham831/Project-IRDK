using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Services
{
    public class ShowSplashBannerService : Service
    {
        private readonly float _splashShowTimeInSeconds = 3f;
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public override async UniTask<IService.Result> Execute()
        {
            var splashBannerScreen = await _uiManager.Show<SplashBannerScreen>();
            await UniTask.Delay((int)(_splashShowTimeInSeconds * 1000));

            await splashBannerScreen.Close();

            return IService.Result.Success;
        }
    }
}