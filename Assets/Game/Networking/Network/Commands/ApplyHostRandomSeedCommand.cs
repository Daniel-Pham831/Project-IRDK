using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Maniac.Command;
using Maniac.RandomSystem;
using Maniac.Utils;
using Unity.Netcode;

namespace Game.Networking.Network.Commands
{
    public class ApplyHostRandomSeedCommand : Command
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private NetLobbyModelHandler _netLobbyModelHandler;
        
        public override async UniTask Execute()
        {
            if (NetworkManager.Singleton.IsHost) return;
            
            _netLobbyModelHandler ??= _netModelHub.GetHandler<NetLobbyModelHandler>();
            var hostNetLobbyModel = _netLobbyModelHandler
                .GetModelByPlayerId(Locator<LobbySystem>.Instance.JoinedLobby.Value.HostId);
            
            if(hostNetLobbyModel == null)
                throw new Exception("Host NetLobbyModel is null!");
            
            Locator<Randomer>.Instance.SetSeed(hostNetLobbyModel.RandomSeed);
            await UniTask.CompletedTask;
        }
    }
}