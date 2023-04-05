using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Networking.Lobby.Commands
{
    public class HandleBeingKickedCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            bool isBeingKicked = _lobbySystem.JoinedLobby.Value == null;
            if (isBeingKicked)
            {
                _lobbySystem.JoinedLobby.Value = null;
                _lobbySystem.HostLobbyToPing.Value = null;
                
                await new ShowInformationDialogCommand(LanguageTable.Information_BeingKickedHeader, LanguageTable.Information_BeingKickedBody).Execute();
                await _uiManager.Close<LobbyRoomDetailScreen>();
            }
        }
    }
}