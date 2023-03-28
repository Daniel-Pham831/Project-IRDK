using System;
using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.Utils;
using UniRx;

namespace Game.Services
{
    public class BootstrapLoadingServiceGroup : SequenceServiceGroup
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public BootstrapLoadingServiceGroup(string serviceGroupName) : base(serviceGroupName){}

        public override async UniTask<IService.Result> Execute()
        {
            var bootstrapLoadingScreen = await _uiManager.Show<BootstrapLoadingScreen>(Progress);
            await base.Execute();
            await bootstrapLoadingScreen.Close();
            return IService.Result.Success;
        }
    }
}