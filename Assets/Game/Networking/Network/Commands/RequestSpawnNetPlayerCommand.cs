using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Environments;
using Game.Networking.NetDataTransmitterComponents;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.Network.Commands
{
    public class RequestSpawnNetPlayerCommand : Command
    {
        private NetSpawner _netSpawner => Locator<NetSpawner>.Instance;
        private NetPlayerSpawnPointManager _netPlayerSpawnPointManager => Locator<NetPlayerSpawnPointManager>.Instance;
        public override async UniTask Execute()
        {
            _netSpawner.RequestServerToSpawn<NetPlayer>(_netPlayerSpawnPointManager.GetRandomSpawnPoint());
        }
    }
}