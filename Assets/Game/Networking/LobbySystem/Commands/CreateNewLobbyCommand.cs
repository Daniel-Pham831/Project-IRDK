using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class CreateNewLobbyCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        public override async UniTask Execute()
        {
            var lobbyName = (string)await ShowScreenCommand.Create<CreateLobbyScreen>().ExecuteAndReturnResult();

            if (lobbyName == null) return;
            
            var createdLobby = await _lobbySystem.CreateLobby(lobbyName);
            if (createdLobby != null)
            {
                // ShowLobbyRoomDetailScreen
            }
            else
            {
                // show created fail
            }
        }
    }
}