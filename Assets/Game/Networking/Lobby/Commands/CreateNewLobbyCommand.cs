using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Lobby.Models;
using Game.Networking.Network;
using Game.Networking.Relay;
using Game.Networking.Relay.Commands;
using Game.Scenes;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using Unity.Networking.Transport.Relay;

namespace Game.Networking.Lobby.Commands
{
    public class CreateNewLobbyCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public override async UniTask Execute()
        {
            var model = (LobbyModel)await ShowScreenCommand.Create<CreateLobbyScreen>().ExecuteAndReturnResult();
            if (model == null) return;

            await new ShowConnectToServerCommand().Execute();
            try
            {
                _uiManager.Close<LobbyScreen>();
                await _lobbySystem.CreateLobby(model);
                var relayData = await _relaySystem.CreateRelay(model.MaxPlayers);
                await new UpdateRelayDataForLobbyCommand(relayData).ExecuteAndGetResult();
                _networkSystem.SetRelayServerData(new RelayServerData(relayData.Item1, "dtls"));
                _networkSystem.NetworkManager.StartHost();
            }
            catch
            {
                await ShowCreateLobbyFail();
                return;
            }

            await new HideConnectToServerCommand().Execute();
            await new LoadEmptySceneCommand().Execute();
            await new LoadSceneCommand(new LoadSceneCommand.Param(SceneName.LobbyRoom,true)).Execute();
        }

        private async UniTask ShowCreateLobbyFail()
        {
            await new HideConnectToServerCommand().Execute();
            await new ShowInformationDialogCommand(LanguageTable.Information_FailToCreateLobbyHeader,
                LanguageTable.Information_FailToCreateLobbyBody).Execute();
        }

    }
}