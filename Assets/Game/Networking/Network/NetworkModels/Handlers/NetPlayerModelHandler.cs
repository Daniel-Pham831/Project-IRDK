using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Maniac.Utils.Extension;
using MemoryPack;
using UniRx;
using UnityEngine;

namespace Game.Networking.Network.NetworkModels.Handlers
{
    [MemoryPackable]
    public partial class NetPlayerModel : BaseNetModel
    {
        public string Name { get; set; }
        public string CharacterGraphicsId { get; set; } = "default";
    }

    public class NetPlayerModelHandler : NetHandler<NetPlayerModel>
    {
        private CharacterConfig _characterConfig;
        
        protected override async void Awake()
        {
            _characterConfig = _dataBase.GetConfig<CharacterConfig>();
            base.Awake();
        }

        protected override void HandleLocalClientNetworkSpawn()
        {
            Debug.Log("Send NetPlayerModel to server");
            var localClientNetPlayerModel = CreateNewLocalModel();
            
            // Get Local Player Name
            localClientNetPlayerModel.Name = _userProfile.DisplayName;
            localClientNetPlayerModel.CharacterGraphicsId = _characterConfig.GetDefaultCharacterId();

            LocalClientModel.Value = localClientNetPlayerModel;
        }
    }
}