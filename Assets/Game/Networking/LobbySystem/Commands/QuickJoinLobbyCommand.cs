using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies;

namespace Game.Networking.LobbySystem.Commands
{
    public class QuickJoinLobbyCommand : Command
    {
        private readonly QuickJoinLobbyOptions _options;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public QuickJoinLobbyCommand(QuickJoinLobbyOptions options = default)
        {
            _options = options;
        }

        public override async UniTask Execute()
        {
            var joinedLobby = await _lobbySystem.QuickJoinLobby(_options);

            if (joinedLobby != null)
            {
                // ShowLobbyRoomDetailScreen
            }
            else
            {
                // show join fail
            }
        }
    }
}