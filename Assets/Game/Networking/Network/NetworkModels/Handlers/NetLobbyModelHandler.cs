using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.MessengerSystem.Messages;
using MemoryPack;

namespace Game.Networking.Network.NetworkModels.Handlers
{
    [MemoryPackable]
    public partial class NetLobbyModel : BaseNetModel
    {
        public bool IsReady { get; set; } = false;
    }

    public class NetLobbyModelHandler : NetHandler<NetLobbyModel>
    {
        private NetPlayerModelHandler _playerModelHandler;
        protected override void GetOtherHandlers(NetModelHub hub)
        {
            _playerModelHandler = hub.GetHandler<NetPlayerModelHandler>();
        }

        protected override async void HandleLocalClientNetworkSpawn()
        {
            await UniTask.WaitUntil(() => _playerModelHandler.LocalClientModel.Value != null);
            
            var localNetLobbyModel = CreateNewLocalModel();
            localNetLobbyModel.IsReady = true;
            
            LocalClientModel.Value = localNetLobbyModel;
        }

        public bool IsAllClientsNetLobbyModelReady()
        {
            foreach (var netLobbyModel in AllClientReactiveModels.Value.Values.ToList())
            {
                if (!netLobbyModel.Value.IsReady) return false;
            }

            return true;
        }
    }
}