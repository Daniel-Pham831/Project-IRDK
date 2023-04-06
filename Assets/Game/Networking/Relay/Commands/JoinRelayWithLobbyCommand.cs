using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Relay.Models;

namespace Game.Networking.Relay.Commands
{
    public class JoinRelayWithLobbyCommand : ResultCommand<JoinAllocation>
    {
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private readonly Unity.Services.Lobbies.Models.Lobby _joinedLobby;

        public  JoinRelayWithLobbyCommand(Unity.Services.Lobbies.Models.Lobby joinedLobby)
        {
            _joinedLobby = joinedLobby;
        }

        public override async UniTask Execute()
        {
            _result = null;
            try
            {
                var relayJoinCode = _joinedLobby.Data[LobbyDataKey.RelayJoinCode].Value;
                _result = await _relaySystem.JoinRelay(relayJoinCode);
            }
            catch
            {
                // ignored
            }
        }
    }
}