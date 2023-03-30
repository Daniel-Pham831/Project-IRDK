using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Game.Networking.Lobby
{
    public class InitLobbySystemService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var lobbySystem = new LobbySystem.LobbySystem();
            Locator<LobbySystem.LobbySystem>.Set(lobbySystem);
            await lobbySystem.Init();

            return IService.Result.Success;
        }
    }
}