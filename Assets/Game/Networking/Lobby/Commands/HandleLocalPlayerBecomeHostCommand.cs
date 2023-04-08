using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using Unity.Services.Authentication;

namespace Game.Networking.Lobby.Commands
{
    public class HandleLocalPlayerBecomeHostCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public override async UniTask Execute()
        {
            if(_lobbySystem.HostLobbyToPing.Value == null)
            {
                bool isLocalPlayerBecomeJoinedLobbyHost = _lobbySystem.JoinedLobby?.Value.HostId == AuthenticationService.Instance.PlayerId;
                if (isLocalPlayerBecomeJoinedLobbyHost)
                {
                    await new ShowInformationDialogCommand(LanguageTable.Information_ConnectionTimeOutHeader, LanguageTable.Information_ConnectionTimeOutBody).Execute();
                    await new LeaveLobbyCommand().Execute();
                }
            }
        }
    }
}