using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class HandleLocalPlayerBecomeHostCommand : Command
    {
        private LocalData _localData => Locator<LocalData>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public override async UniTask Execute()
        {
            if(_lobbySystem.HostLobbyToPing.Value == null)
            {
                bool isLocalPlayerBecomeJoinedLobbyHost = _lobbySystem.JoinedLobby.Value.HostId == _localData.LocalPlayer.Id;
                if (isLocalPlayerBecomeJoinedLobbyHost)
                {
                    _lobbySystem.HostLobbyToPing.Value = _lobbySystem.JoinedLobby.Value;
                    await new ShowInformationDialogCommand(LanguageTable.Information_BecomeHostHeader, LanguageTable.Information_BecomeHostBody).Execute();
                }
            }
        }
    }
}