using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Services;

namespace Game.Weapons
{
    public class InitWeaponSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            await new WeaponSystem().Init();
            return IService.Result.Success;
        }
    }
}