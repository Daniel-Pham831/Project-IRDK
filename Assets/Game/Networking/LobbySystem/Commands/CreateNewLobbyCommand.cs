using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.LobbySystem.Models;
using Maniac.Command;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class CreateNewLobbyCommand : Command
    {
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private RelaySystem.RelaySystem _relaySystem => Locator<RelaySystem.RelaySystem>.Instance;

        public CreateNewLobbyCommand(Action onSuccess, Action onFail)
        {
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public override async UniTask Execute()
        {
            var model = (LobbyModel)await ShowScreenCommand.Create<CreateLobbyScreen>().ExecuteAndReturnResult();

            await new ShowConnectToServerCommand().Execute();
            
            var lobby = await _lobbySystem.CreateLobby(model);
            var relayData = await _relaySystem.CreateRelay(model.MaxPlayers);
            var updateSuccess = await new UpdateRelayDataForLobbyCommand(relayData).ExecuteAndGetResult();
            

            if (lobby == null || relayData == (null, string.Empty) || !updateSuccess)
            {
                await FailToCreateError();
                return;
            }

            await new HideConnectToServerCommand().Execute();
            // Move to room scene
        }

        private async UniTask FailToCreateError()
        {
            await new HideConnectToServerCommand().Execute();
            _onFail?.Invoke();
        }
    }
}