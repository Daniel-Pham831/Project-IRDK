using Cysharp.Threading.Tasks;
using Game.Networking.Network;
using Game.Networking.Relay;
using Game.Scenes.MainMenu.Commands;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.Lobby.Commands
{
    public class LeaveLobbyCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;

        public override async UniTask Execute()
        {
            _networkSystem.NetworkManager.Shutdown();
            await _lobbySystem.LeaveLobby();
            await new LoadMainMenuSceneCommand(true).Execute();
        }
    }
}