using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem.Command;

namespace Game
{
    public class ShowPlayerInGameScreenCommand : Command
    {
        public override async UniTask Execute()
        {
            await new ShowScreenCommand<PlayerInGameScreen>().Execute();
        }
    }
}