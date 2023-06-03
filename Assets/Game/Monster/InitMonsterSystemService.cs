using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Assets.Game.Monster
{

    public class InitMonsterSystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            await new MonsterSystem().Init();
            return IService.Result.Success;
        }
    }
}
