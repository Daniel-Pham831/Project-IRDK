using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Networking.Environments;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Players.Commands
{
    public class UpdatePlayerPositionInGamePlayCommand : Command
    {
        private readonly Direction _spawnDirection;
        
        private NetPlayer _localNetPlayer => Locator<NetPlayer>.Instance;
        private NetPlayerSpawnPointManager _netPlayerSpawnPointManager => Locator<NetPlayerSpawnPointManager>.Instance;
        
        public UpdatePlayerPositionInGamePlayCommand(Direction spawnDirection = Direction.None)
        {
            _spawnDirection = spawnDirection;
        }

        public override async UniTask Execute()
        {
            _localNetPlayer.transform.position = _spawnDirection != Direction.None
                ? _netPlayerSpawnPointManager.GetRandomSpawnPointAtDirection(_spawnDirection)
                : _netPlayerSpawnPointManager.GetRandomSpawnPoint();
        }
    }
}