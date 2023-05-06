using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game
{
    public class HidePlayerInGameScreenCommand : Command
    {
        public override async UniTask Execute()
        {
            var playerInGameScreen = Locator<UIManager>.Instance.GetShowedUI<PlayerInGameScreen>() as PlayerInGameScreen;

            if (playerInGameScreen != null)
            {
                await playerInGameScreen.Close();
            }
        }
    }
}