using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Coin
{
    public class InitCoinSystemCommand : Command
    {
        public override async UniTask Execute()
        {
            new CoinSystem().Init();

            await UniTask.CompletedTask;
        }
    }
}