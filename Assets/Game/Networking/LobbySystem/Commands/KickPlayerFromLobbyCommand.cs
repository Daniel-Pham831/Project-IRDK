using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class KickPlayerFromLobbyCommand : Command
    {
        private readonly string _lobbyId;
        private readonly string _playerId;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public KickPlayerFromLobbyCommand(string lobbyId, string playerId)
        {
            _lobbyId = lobbyId;
            _playerId = playerId;
        }

        public override async UniTask Execute()
        {
            var success = await _lobbySystem.KickPlayerFromLobby(_lobbyId, _playerId);
            if (success)
            {
                // Show success Kick Player 
            }
            else
            {
                // Show Fail to Kick player
            }
        }
    }
}