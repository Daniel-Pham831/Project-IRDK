using Cysharp.Threading.Tasks;
using Game.Networking.NetMessages;
using Game.Networking.Network.NetworkModels.Models;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using MemoryPack;
using UniRx;

namespace Game.Networking.Network.NetworkModels.Handlers
{
    [MemoryPackable]
    public partial class NetPlayerModel : BaseNetModel
    {
        public string Name { get; set; }
    }
    
    public class NetPlayerModelHandler : NetHandler<NetPlayerModel>
    {
        protected override async void Awake()
        {
            Locator<NetPlayerModelHandler>.Set(this);
            base.Awake();
        }

        private void OnDestroy()
        {
            Locator<NetPlayerModelHandler>.Remove();
        }

        public override void OnMessagesReceived(Message receivedMessage)
        {
            base.OnMessagesReceived(receivedMessage);
            
            switch (receivedMessage)
            {
                case LocalClientNetworkSpawn:
                    HandleLocalClientNetworkSpawn();
                    break;
            }
        }

        private void HandleLocalClientNetworkSpawn()
        {
            var localClientNetPlayerModel = CreateNewLocalModel();
            localClientNetPlayerModel.Name = _userProfile.DisplayName;

            LocalClientModel.Value = localClientNetPlayerModel;
        }

        protected override void RegisterMessages(bool shouldRegister)
        {
            if (shouldRegister)
            {
                Messenger.Register<LocalClientNetworkSpawn>(this);
            }
            else
            {
                Messenger.Unregister<LocalClientNetworkSpawn>(this);
            }
            
            base.RegisterMessages(shouldRegister);
        }
    }
}