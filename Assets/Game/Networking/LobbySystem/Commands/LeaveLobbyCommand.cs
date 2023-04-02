using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class LeaveLobbyCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public override async UniTask Execute()
        {
            await _lobbySystem.LeaveLobby();
        }
    }
}