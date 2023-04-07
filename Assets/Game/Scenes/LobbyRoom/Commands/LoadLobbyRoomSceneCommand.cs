using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Network;
using Game.Networking.Network.Commands;
using Maniac.Command;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Scenes.LobbyRoom.Commands
{
    public class LoadLobbyRoomSceneCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;

        public override async UniTask Execute()
        {
            var lobbyRoomCommandsGroup = new SequenceCommandServiceGroup("Load Lobby Room");
            
            lobbyRoomCommandsGroup.Add(new LoadEmptySceneCommand());
            lobbyRoomCommandsGroup.Add(new LoadSceneCommand(SceneName.LobbyRoom));
            lobbyRoomCommandsGroup.Add(new StartNetworkSessionCommand());
            lobbyRoomCommandsGroup.Add(new ShowScreenCommand<LobbyRoomDetailScreen>());

            var loadingScreen = await _uiManager.Show<LoadingScreen>(lobbyRoomCommandsGroup.Progress);
            await lobbyRoomCommandsGroup.Run();
            await loadingScreen.Close();
        }
    }
}