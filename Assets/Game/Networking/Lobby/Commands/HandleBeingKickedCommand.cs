using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using Unity.Services.Authentication;

namespace Game.Networking.Lobby.Commands
{
    public class HandleBeingKickedCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            bool isBeingKicked = _lobbySystem.JoinedLobby.Value == null && IsContainLocalPlayerInLobby();
            if (isBeingKicked)
            {
                _lobbySystem.JoinedLobby.Value = null;
                _lobbySystem.HostLobbyToPing.Value = null;
                
                await new ShowInformationDialogCommand(LanguageTable.Information_BeingKickedHeader, LanguageTable.Information_BeingKickedBody).Execute();
                await new LeaveLobbyCommand().Execute();
            }
        }

        private bool IsContainLocalPlayerInLobby()
        {
            return _lobbySystem.JoinedLobby.Value.Players.FirstOrDefault(x =>
                x.Id == AuthenticationService.Instance.PlayerId) != null;
        }
    }
}