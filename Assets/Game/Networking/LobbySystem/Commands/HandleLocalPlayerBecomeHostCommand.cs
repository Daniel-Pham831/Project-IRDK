using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using Unity.Services.Authentication;

namespace Game.Networking.LobbySystem.Commands
{
    public class HandleLocalPlayerBecomeHostCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public override async UniTask Execute()
        {
            if(_lobbySystem.HostLobbyToPing.Value == null)
            {
                bool isLocalPlayerBecomeJoinedLobbyHost = _lobbySystem.JoinedLobby.Value.HostId == AuthenticationService.Instance.PlayerId;
                if (isLocalPlayerBecomeJoinedLobbyHost)
                {
                    _lobbySystem.HostLobbyToPing.Value = _lobbySystem.JoinedLobby.Value;
                    await new ShowInformationDialogCommand(LanguageTable.Information_BecomeHostHeader, LanguageTable.Information_BecomeHostBody).Execute();
                }
            }
        }
    }
}