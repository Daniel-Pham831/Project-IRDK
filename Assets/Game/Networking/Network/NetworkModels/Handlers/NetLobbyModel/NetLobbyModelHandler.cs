using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Game.Networking.Network.NetworkModels.Models;
using MemoryPack;
using Unity.Mathematics;
using Unity.Netcode;

namespace Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel
{
    [MemoryPackable]
    public partial class NetLobbyModel : BaseNetModel
    {
        public bool IsDataSyncReady { get; set; } = false;
        public int RandomSeed { get; set; } = 0;
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
            localNetLobbyModel.IsDataSyncReady = true;
            if (NetworkManager.Singleton.IsHost)
            {
                localNetLobbyModel.RandomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            
            LocalClientModel.Value = localNetLobbyModel;
        }

        public bool IsAllClientsDataSyncReady()
        {
            if (AllClientReactiveModels.Value.Values.Count == 0)
                return false;
            
            foreach (var netLobbyModel in AllClientReactiveModels.Value.Values.ToList())
            {
                if (!netLobbyModel.Value.IsDataSyncReady) return false;
            }

            return true;
        }
    }
}