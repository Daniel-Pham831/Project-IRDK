using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Lobby.Models;
using Game.Networking.Network;
using Game.Networking.Relay;
using Maniac.Command;
using Maniac.UISystem.Command;
using Maniac.Utils;
using Unity.Networking.Transport.Relay;

namespace Game.Networking.Lobby.Commands
{
    public class CreateNewLobbyCommand : Command
    {
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;

        public CreateNewLobbyCommand(Action onSuccess, Action onFail)
        {
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public override async UniTask Execute()
        {
            var model = (LobbyModel)await ShowScreenCommand.Create<CreateLobbyScreen>().ExecuteAndReturnResult();
            if (model == null) return;

            await new ShowConnectToServerCommand().Execute();
            try
            {
                await _lobbySystem.CreateLobby(model);
                var relayData = await _relaySystem.CreateRelay(model.MaxPlayers);
                await new UpdateRelayDataForLobbyCommand(relayData).ExecuteAndGetResult();
                _networkSystem.SetRelayServerData(new RelayServerData(relayData.Item1, "dtls"));
                _networkSystem.NetworkManager.StartHost();
            }
            catch
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