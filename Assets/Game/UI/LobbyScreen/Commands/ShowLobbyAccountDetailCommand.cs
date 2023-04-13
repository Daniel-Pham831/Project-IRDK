using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Maniac.Command;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Commands
{
    public class ShowLobbyAccountDetailCommand: Command
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;

        public override async UniTask Execute()
        {
            var netPlayerModelHandler = _netModelHub.GetHandler<NetPlayerModelHandler>();
            var oldLocalClientCharacterId = netPlayerModelHandler.LocalClientModel.Value.CharacterGraphicsId;

            var newCharacterId = (string)(await new ShowScreenCommand<LobbyAccountDetailScreen>().ExecuteAndReturnResult());

            if (newCharacterId != oldLocalClientCharacterId)
            {
                netPlayerModelHandler.LocalClientModel.Value.CharacterGraphicsId = newCharacterId;
                netPlayerModelHandler.LocalClientModel.SetValueAndForceNotify(netPlayerModelHandler.LocalClientModel.Value);
            }
        }
    }
}