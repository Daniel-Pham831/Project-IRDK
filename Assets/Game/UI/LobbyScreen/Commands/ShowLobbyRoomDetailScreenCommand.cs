using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Commands
{
    public class ShowLobbyRoomDetailScreenCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public override async UniTask Execute()
        {
            await _uiManager.Show<LobbyRoomDetailScreen>();
        }
    }
}