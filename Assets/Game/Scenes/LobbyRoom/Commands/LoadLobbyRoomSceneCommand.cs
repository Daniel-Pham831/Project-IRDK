using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Lobby.Commands;
using Game.Networking.Network;
using Game.Networking.Network.Commands;
using Game.Scenes.MainMenu.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Scenes.LobbyRoom.Commands
{
    public class LoadLobbyRoomSceneCommand : Command
    {
        private LoadingScreen _loadingScreen;
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public override async UniTask Execute()
        {
            var lobbyRoomCommandsGroup = new SequenceCommandServiceGroup("Load Lobby Room");
            lobbyRoomCommandsGroup.OnFailed += OnLoadLobbyRoomFailed;
            
            lobbyRoomCommandsGroup.Add(new LoadEmptySceneCommand());
            lobbyRoomCommandsGroup.Add(new LoadSceneCommand(SceneName.LobbyRoom));
            lobbyRoomCommandsGroup.Add(new InitNetworkModelHubCommand());
            lobbyRoomCommandsGroup.Add(new StartNetworkSessionCommand());
            lobbyRoomCommandsGroup.Add(new CheckDataSyncingCommand());
            lobbyRoomCommandsGroup.Add(new RequestSpawnNetPlayerCommand());
            lobbyRoomCommandsGroup.Add(new ApplyHostRandomSeedCommand());
            lobbyRoomCommandsGroup.Add(new ShowScreenCommand<LobbyRoomDetailScreen>());

            _loadingScreen = await _uiManager.Show<LoadingScreen>(lobbyRoomCommandsGroup.Progress);
            await lobbyRoomCommandsGroup.Run();
            await _loadingScreen.Close();
        }

        private async void OnLoadLobbyRoomFailed()
        {
            if (_loadingScreen != null)
            {
                await new ShowInformationDialogCommand(LanguageTable.Information_ConnectionTimeOutHeader,
                    LanguageTable.Information_ConnectionTimeOutBody).Execute();
                
                _loadingScreen.Close();
                await new LeaveLobbyCommand().Execute();
            }
        }
    }
}