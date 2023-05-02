using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Lobby.Commands;
using Game.Networking.Network.Commands;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Scenes.NetGamePlay.Commands
{
    public class LoadNetGamePlaySceneCommand : Command
    {
        private LoadingScreen _loadingScreen;
        private UIManager _uiManager => Locator<UIManager>.Instance;
        
        public override async UniTask Execute()
        {
            var netGamePlayCommandsGroup = new SequenceCommandServiceGroup("Load Net Game Play");
            netGamePlayCommandsGroup.OnFailed += OnLoadNetGamePlayFailed;
            
            netGamePlayCommandsGroup.Add(new LoadEmptySceneCommand());
            netGamePlayCommandsGroup.Add(new LoadSceneCommand(SceneName.NetGamePlay));
            netGamePlayCommandsGroup.Add(new InitNetGamePlayCommand());
            netGamePlayCommandsGroup.Add(new RequestSpawnNetPlayerCommand());
            
            
            _loadingScreen = await _uiManager.Show<LoadingScreen>(netGamePlayCommandsGroup.Progress);
            await netGamePlayCommandsGroup.Run();
            await _loadingScreen.Close();
        }

        private async void OnLoadNetGamePlayFailed()
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