using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Network.Commands;
using Maniac.Command;
using Maniac.Services;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Scenes.MainMenu.Commands
{
    public class LoadMainMenuSceneCommand : Command
    {
        private readonly bool _shouldLoadWithLoadingScreen;
        private UIManager _uiManager => Locator<UIManager>.Instance;
        private LoadingScreen _loadingScreen;
        public LoadMainMenuSceneCommand(bool shouldLoadWithLoadingScreen = false)
        {
            _shouldLoadWithLoadingScreen = shouldLoadWithLoadingScreen;
        }

        public override async UniTask Execute()
        {
            var mainMenuCommandGroup = new SequenceCommandServiceGroup("Load MainMenu Room");
            
            mainMenuCommandGroup.Add(new LoadEmptySceneCommand());
            mainMenuCommandGroup.Add(new LoadSceneCommand(SceneName.MainMenu));
            mainMenuCommandGroup.Add(new ShowScreenCommand<MainMenuScreen>());

            if(_shouldLoadWithLoadingScreen)
                _loadingScreen = await _uiManager.Show<LoadingScreen>(mainMenuCommandGroup.Progress);
            
            await mainMenuCommandGroup.Run();
            
            if(_shouldLoadWithLoadingScreen)
                await _loadingScreen.Close();
        }
    }
}