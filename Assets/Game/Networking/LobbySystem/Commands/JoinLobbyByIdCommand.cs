using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class JoinLobbyByIdCommand : Command
    {
        private readonly string _joinId;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public JoinLobbyByIdCommand(string joinId)
        {
            _joinId = joinId;
        }

        public override async UniTask Execute()
        {
            var joinedLobby = await _lobbySystem.JoinLobbyById(_joinId);

            if (joinedLobby != null)
            {
                // ShowLobbyRoomDetailScreen
                await new ShowLobbyRoomDetailScreenCommand().Execute();
            }
            else
            {
                // show join fail
            }
        }
    }
}